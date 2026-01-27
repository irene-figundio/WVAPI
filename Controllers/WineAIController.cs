
using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Helpers;
using AI_Integration.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WineAIController : ControllerBase
    {
        private IHttpClientFactory? _httpClientFactory;
        private readonly IUnitOfWork _unitOfWork;

        private readonly OpenAISettings _openAISettings;
        private readonly string apiKey = "";
        private readonly string assistantId = "";
        // 31-07-2025 : Irene
        // Apikey and AssistantId are set from app configuration
        public WineAIController(IHttpClientFactory httpClientFactory,
            IUnitOfWork unitOfWork,
            IOptions<OpenAISettings> openAISettings)
        {
            _httpClientFactory = httpClientFactory;
            _unitOfWork = unitOfWork;
            _openAISettings = openAISettings.Value;
            if (!string.IsNullOrEmpty(_openAISettings.ApiKey))
            {
                apiKey = _openAISettings.ApiKey;
            }
            if (!string.IsNullOrEmpty(_openAISettings.AssistantId))
            {
                assistantId = _openAISettings.AssistantId;
            }
        }

        // GET: api/wineai/generatetext
        [HttpPost("generatetext")]
        public async Task<IActionResult> GenerateText([FromBody] Message message, [FromHeader(Name = "User-Agent")] string userAgent)
        {
            //crea un oggetto WebAPILog e lo inizializza
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/wineai/generatetext",
                RequestBody = message.ToString(),
                UserAgent = userAgent,
                AdditionalInfo = ""
            };
            try
            {

                // Imposta la chiave API delle OpenAI

                IList<Message> messages = new List<Message>();
                messages.Add(message);

                // Oggetto di input
                var input = new PromptModel
                {
                   model = "gpt-3.5-turbo",
                   name= "Vitinerario",
                    messages = (List<Message>)messages,
                    // "messages"= [{ "role": "user", "content": "Say this is a test!"}],
                    // = model,
                    max_tokens =370,
                    temperature = 0.7,
                    frequency_penalty = 0,
                    presence_penalty = 0
                };             


                // Serializza l'oggetto di input in formato JSON
                string requestBody = JsonConvert.SerializeObject(input);
                // Crea un client HTTP
                var client = _httpClientFactory.CreateClient();

                // Imposta l'header dell'autorizzazione con la chiave API
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Esegui la richiesta POST all'endpoint delle OpenAI API
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions?", new StringContent(requestBody, Encoding.UTF8, "application/json"));

                // Leggi la risposta
                string responseBody = await response.Content.ReadAsStringAsync();
                log.ResponseBody = responseBody;
                var respAI = JsonConvert.DeserializeObject<WineAIResponse>(responseBody);
                if (respAI.Choices != null && respAI.Choices.Count > 0 )
                {
                    var res = new ResAI 
                    { 
                        answer = respAI.Choices[0].Message.content
                    };
                    var resJson = JsonConvert.SerializeObject(res);
                    
                    _unitOfWork.WineAI.AddWineAI(message.content,res.answer,userAgent);
                    log.ResponseCode = 200;
                    log.ResponseMessage ="OK";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok(resJson);
                }
                // Restituisci il testo generato al client
                log.ResponseCode = 500;
                log.ResponseMessage = "ERROR: Si è verificato un errore con la chiamata OpenAI";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return StatusCode(500, $"Si è verificato un errore con la chiamata OpenAI");
            }
            catch (Exception ex)
            {
                log.ResponseCode = 500;
                log.ResponseMessage = $"Si è verificato un errore: {ex.Message}";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return StatusCode(500, $"Si è verificato un errore: {ex.Message}");
            }
        }

        [HttpPost("assistantcall")]
        public async Task<IActionResult> AssistantCall([FromBody] Message message, [FromHeader(Name = "User-Agent")] string userAgent)
        {
            //crea un oggetto WebAPILog e lo inizializza
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/wineai/assistantcall",
                RequestBody = message.ToString(),
                UserAgent = userAgent,
                AdditionalInfo = ""
            };
            ThreadMessage rispostaAssistant = new ThreadMessage();
            try
            {

                // Imposta la chiave API delle OpenAI

                IList<Message> messages = new List<Message>();
                messages.Add(message);
                var thread_id = "";
                //var thread_id = 0; Crea il thread e il run per l'assistente Vitinerario inserendo il messaggio
                //POST   https://api.openai.com/v1/threads/runs
                ThreadRun run = await CreateThreadAndRun(apiKey, assistantId, messages);
                if (run == null || string.IsNullOrEmpty(run.thread_id))
                {
                    log.ResponseCode = 500;
                    log.ResponseMessage = "ERROR: Si è verificato un errore con la chiamata OpenAI (CreateThreadAndRun)";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return StatusCode(500, $"Si è verificato un errore con la chiamata OpenAI");
                }
                thread_id = run.thread_id;
                //Recupera l'ultimo messaggio del thread 
                //con la chiamata GET https://api.openai.com/v1/threads/{thread_id}/messages   recupero tutta la lista (messaggio + risposta)
                IList<ThreadMessage> objects = await GetThreadMessages(apiKey, thread_id);
                if (objects != null)
                {
                    rispostaAssistant = objects.FirstOrDefault(x => x.Role == "assistant");
                }
                else
                {
                    log.ResponseCode = 500;
                    log.ResponseMessage = "ERROR: Si è verificato un errore con la chiamata OpenAI (GetThreadMessages)";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return StatusCode(500, $"Si è verificato un errore con la chiamata OpenAI");
                }

                //elimina il thread 
                //DELETE  https://api.openai.com/v1/threads/{thread_id}
                ThreadDeleted deleted = await DeleteThread(apiKey, thread_id);

                if (deleted == null || deleted.Deleted == false)
                {
                    log.ResponseCode = 500;
                    log.ResponseMessage = "ERROR: Si è verificato un errore con la chiamata OpenAI (deleteThread)";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return StatusCode(500, $"Si è verificato un errore con la chiamata OpenAI");
                }

                log.ResponseBody = rispostaAssistant.ToString();

                if (!string.IsNullOrEmpty(rispostaAssistant.Content[0].Text.Value))
                {
                    var res = new ResAI
                    {
                        answer = rispostaAssistant.Content[0].Text.Value
                    };
                    var resJson = JsonConvert.SerializeObject(res);

                    _unitOfWork.WineAI.AddWineAI(message.content, res.answer, userAgent);
                    log.ResponseCode = 200;
                    log.ResponseMessage = "OK";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok(resJson);
                }
                // Restituisci il testo generato al client
                log.ResponseCode = 500;
                log.ResponseMessage = "ERROR: Si è verificato un errore con la chiamata OpenAI";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return StatusCode(500, $"Si è verificato un errore con la chiamata OpenAI");
            }
            catch (Exception ex)
            {
                log.ResponseCode = 500;
                log.ResponseMessage = $"Si è verificato un errore: {ex.Message}";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return StatusCode(500, $"Si è verificato un errore: {ex.Message}");
            }
        }

        private async Task<ThreadRun> CreateThreadAndRun(string apiKey, string assistantId, IList<Message> messages)
        {
            //crea un oggetto WebAPILog e lo inizializza
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "https://api.openai.com/v1/threads/runs",
                AdditionalInfo = ""
            };
            try
            {
                ThreadRun result = new ThreadRun();
                // Crea un client HTTP
                var client = _httpClientFactory.CreateClient();
                // Imposta l'header dell'autorizzazione con la chiave API
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("OpenAI-Beta", $"assistants=v2");

                string apiUrl = "https://api.openai.com/v1/threads/runs";

                var requestData = new
                {
                    assistant_id = assistantId,
                    thread = new
                    {
                        messages = messages

                    }
                };
                string requestBody = JsonConvert.SerializeObject(requestData);
                log.RequestBody = requestBody;
                var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, requestContent);
                // Leggi la risposta
                string responseBody = await response.Content.ReadAsStringAsync();
                log.ResponseBody = responseBody;

                var respAI = JsonConvert.DeserializeObject<ThreadRun>(responseBody);
                if (respAI != null)
                {
                    result = respAI;
                    // Restituisci il testo generato al client
                    log.ResponseCode = 200;
                    log.ResponseMessage = "OK";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    log.ResponseCode = 500;
                    log.ResponseMessage = "ERROR: Si è verificato un errore con la chiamata OpenAI";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    result = null;
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<IList<ThreadMessage>> GetThreadMessages(string apiKey, string thread_id)
        {
            //crea un oggetto WebAPILog e lo inizializza
            string apiUrl = "https://api.openai.com/v1/threads/" + thread_id + "/messages";
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "GET",
                RequestUrl = apiUrl,
                AdditionalInfo = ""
            };
            IList<ThreadMessage> result = new List<ThreadMessage>();
            try
            {

                // Crea un client HTTP
                var client = _httpClientFactory.CreateClient();
                // Imposta l'header dell'autorizzazione con la chiave API
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("OpenAI-Beta", $"assistants=v2");

                var response = await client.GetAsync(apiUrl);

                // Leggi la risposta
                string responseBody = await response.Content.ReadAsStringAsync();
                log.ResponseBody = responseBody;

                var respAI = JsonConvert.DeserializeObject<ChatLog>(responseBody);
                if (respAI != null)
                {
                    result = respAI.Data;
                    if (result == null || result.Count < 0)
                    {
                        log.ResponseCode = 500;
                        log.ResponseMessage = "ERROR: Si è verificato un errore con la chiamata OpenAI";
                        await _unitOfWork.InsertAsync(log);
                        await _unitOfWork.SaveChangesAsync();
                        result = null;
                    }
                    else
                    {
                        // Restituisci il testo generato al client
                        log.ResponseCode = 200;
                        log.ResponseMessage = "OK";
                        await _unitOfWork.InsertAsync(log);
                        await _unitOfWork.SaveChangesAsync();
                    }

                }
                else
                {
                    log.ResponseCode = 500;
                    log.ResponseMessage = "ERROR: Si è verificato un errore con la chiamata OpenAI";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }

        }
        private async Task<ThreadDeleted> DeleteThread(string apiKey, string thread_id)
        {
            //crea un oggetto WebAPILog e lo inizializza
            string apiUrl = "https://api.openai.com/v1/threads/" + thread_id;
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "DELETE",
                RequestUrl = apiUrl,
                AdditionalInfo = ""
            };

            ThreadDeleted result = new ThreadDeleted();
            try
            {

                // Crea un client HTTP
                var client = _httpClientFactory.CreateClient();
                // Imposta l'header dell'autorizzazione con la chiave API
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("OpenAI-Beta", $"assistants=v2");

                // var response = await client.DeleteAsync(apiKey);
                var response = await client.DeleteAsync(apiUrl);
                // Leggi la risposta
                string responseBody = await response.Content.ReadAsStringAsync();
                log.ResponseBody = responseBody;

                var respAI = JsonConvert.DeserializeObject<ThreadDeleted>(responseBody);
                if (respAI != null)
                {
                    result = respAI;
                    // Restituisci il testo generato al client
                    log.ResponseCode = 200;
                    log.ResponseMessage = "OK";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    log.ResponseCode = 500;
                    log.ResponseMessage = "ERROR: Si è verificato un errore con la chiamata OpenAI";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    result = null;
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }




    }
}
