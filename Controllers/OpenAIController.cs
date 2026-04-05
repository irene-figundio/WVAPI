using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Helpers;
using AI_Integration.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;


namespace AI_Integration.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly OpenAISettings _openAISettings;
        private readonly string apiKey = ""; 
       private readonly string assistantId = "";
        // 31-07-2025 : Irene
        // Apikey and AssistantId are set from app configuration

        private static readonly HttpClient httpClient = new HttpClient();
        public OpenAIController(
            IHttpClientFactory httpClientFactory, 
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


        [HttpPost("assistantcall")]
        public async Task<IActionResult> AssistantCall([FromBody] Message message, [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/openai/assistantcall",
                RequestBody = message?.ToString() ?? "No message content",
                UserAgent = userAgent,
                AdditionalInfo = ""
            };

            if (message == null)
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "ERROR: Message body is null";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest("Message body is required");
            }

            string threadId = null;

            try
            {
                // // Step 2: Create a Thread
                // threadId = await CreateThreadAsync(this.assistantId);
                // Console.WriteLine("Thread ID: " + threadId);

                // // Step 3: Add a Message to the Thread
                // await AddMessageToThreadAsync(threadId, "user", message.content);
                // Console.WriteLine("Message added to thread.");

                // // Step 4: Create a Run
                //var runId = await CreateAndPollRunAsync(this.assistantId, threadId);
                //Console.WriteLine("Run completed. Run ID: " + runId);

                // // Step 5: List Messages Added to the Thread
                // var assistantMessage = await ListMessagesAsync(threadId);

                // log.ResponseBody = assistantMessage;
                var assistantMessage = await AssistantSent(message.content);

                 _unitOfWork.WineAI.AddWineAI(message.content, assistantMessage, userAgent);
                log.ResponseCode = 200;
                log.ResponseMessage = "OK";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();

                // Delete the thread after processing the response
                //bool isDeleted = await DeleteThreadAsync(threadId);
                //if (!isDeleted)
                //{
                //    log.AdditionalInfo += "Failed to delete thread.";
                //    Console.WriteLine("Failed to delete the thread.");
                //}

                return Ok(new { answer = assistantMessage });
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

        #region OLD CODE


        private async Task<string> CreateThreadAsync(string assistantId)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // Add the required OpenAI-Beta header
            client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

            var threadData = new
            {
                // assistant_id = assistantId, // Pass the assistant ID if required
                metadata = new { } // Include any additional metadata required by the API
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(threadData), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/threads", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to create thread. Error: {errorContent}");
                throw new Exception("Error creating thread: " + errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseContent);
            return result.id;
        }

        private async Task AddMessageToThreadAsync(string threadId, string role, string content)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // Add the required OpenAI-Beta header
            if (!client.DefaultRequestHeaders.Contains("OpenAI-Beta"))
            {
                client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
            }

            var messageData = new
            {
                role = role,    // Ensure role is correct, typically "user" or "assistant"
                content = content // The content of the message
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(messageData), Encoding.UTF8, "application/json");

            // Log the request content for debugging purposes
            Console.WriteLine("Requesting to add message with data: " + JsonConvert.SerializeObject(messageData));

            // Use the correct endpoint for adding a message to the thread
            var response = await client.PostAsync($"https://api.openai.com/v1/threads/{threadId}/messages", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to add message to thread. Error: {errorContent}");
                throw new Exception("Error adding message to thread: " + errorContent);
            }

            Console.WriteLine("Message added to thread successfully.");
        }


        private async Task<string> CreateAndPollRunAsync(string assistantId, string threadId)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // Add the required OpenAI-Beta header
            if (!client.DefaultRequestHeaders.Contains("OpenAI-Beta"))
            {
                client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
            }

            // Create the payload dynamically based on whether instructions are provided
            var runData = new
            {
                assistant_id = assistantId
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(runData), Encoding.UTF8, "application/json");

            // Log the request content for debugging purposes
            Console.WriteLine("Requesting to create a run with data: " + JsonConvert.SerializeObject(runData));

            var response = await client.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to create run. Error: {errorContent}");
                throw new Exception("Error creating run: " + errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic runResult = JsonConvert.DeserializeObject(responseContent);
            string runId = runResult.id;

            // Polling for the run to complete
            while (true)
            {
                var statusResponse = await client.GetAsync($"https://api.openai.com/v1/threads/{threadId}/runs/{runId}");
                if (!statusResponse.IsSuccessStatusCode)
                {
                    string errorStatusContent = await statusResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to get run status. Error: {errorStatusContent}");
                    throw new Exception("Error checking run status: " + errorStatusContent);
                }

                var statusContent = await statusResponse.Content.ReadAsStringAsync();
                dynamic statusResult = JsonConvert.DeserializeObject(statusContent);

                if (statusResult.status == "completed" || statusResult.status == "in_progress")
                {
                    Console.WriteLine("Run completed successfully.");
                    return runId;
                }
                else if (statusResult.status == "failed" || statusResult.status == "canceled")
                {
                    throw new Exception($"Run failed with status: {statusResult.status}");
                }

                // Wait before polling again
                await Task.Delay(1000);
            }
        }

        private async Task<string> ListMessagesAsync(string threadId)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            // Add the required OpenAI-Beta header
            if (!client.DefaultRequestHeaders.Contains("OpenAI-Beta"))
            {
                client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
            }
            try
            {
                var response = await client.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages");

                if (!response.IsSuccessStatusCode)
                {
                    // Log the error status and content for debugging
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return $"Request failed with status code {response.StatusCode}";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic messages = JsonConvert.DeserializeObject(responseContent);

                foreach (var message in messages.data)
                {
                    if (message.role == "assistant")
                    {
                        return message.content;
                    }
                }
                return "No response from assistant.";
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Exception: {ex.Message}");
                return "An error occurred while retrieving messages.";
            }
        }


        private async Task<bool> DeleteThreadAsync(string threadId)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await client.DeleteAsync($"https://api.openai.com/v1/threads/{threadId}");
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Failed to delete thread. Response: " + responseBody);
                return false;
            }

            Console.WriteLine("Thread deleted successfully.");
            return true;
        }

        #endregion

        [HttpPost("uploadfile")]
        public async Task<IActionResult> UploadFileAndAttachToAssistant([FromForm] IFormFile file)
        {
            // 0. Log di base
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/OpenAI/uploadfile",
                RequestBody = file?.FileName ?? "No file",
                AdditionalInfo = "Upload + VectorStore + PATCH Assistant"
            };

            if (file == null || file.Length == 0)
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Nessun file ricevuto.";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest("Nessun file ricevuto.");
            }

            // 1. validazione
            var allowedExt = new[] { ".txt", ".pdf", ".csv", ".docx" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExt.Contains(ext))
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Formato non supportato";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest("Formato file non supportato. Usa: .txt, .pdf, .csv, .docx");
            }
            if (file.Length > 5 * 1024 * 1024)
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "File troppo grande";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest("Il file è troppo grande. Max 5MB.");
            }

            // 2. salva localmente
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var safeName = Path.GetFileName(file.FileName);
            var localFilePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}{Path.GetExtension(safeName)}");
            using (var fs = new FileStream(localFilePath, FileMode.Create))
                await file.CopyToAsync(fs);

            try
            {
                // 3. upload su OpenAI /v1/files
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);

                using var form = new MultipartFormDataContent();
                using var stream = System.IO.File.OpenRead(localFilePath);
                form.Add(new StreamContent(stream), "file", safeName);
                form.Add(new StringContent("assistants"), "purpose");

                var uploadRes = await client.PostAsync("https://api.openai.com/v1/files", form);
                var uploadJson = await uploadRes.Content.ReadAsStringAsync();
                if (!uploadRes.IsSuccessStatusCode)
                    throw new Exception($"Errore upload file: {uploadJson}");

                dynamic uploadObj = JsonConvert.DeserializeObject(uploadJson);
                string fileId = uploadObj.id;

                // 4. crea Vector Store su /v2/vector_stores
                client.DefaultRequestHeaders.Remove("OpenAI-Beta");
                client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

                var vsName = $"VS_{Path.GetFileNameWithoutExtension(safeName)}_{DateTime.UtcNow.Ticks}";
                var vsPayload = new { name = vsName, file_ids = new List<string> { fileId } };
                var vsContent = new StringContent(
                    JsonConvert.SerializeObject(vsPayload),
                    Encoding.UTF8, "application/json"
                );

                var vsRes = await client.PostAsync("https://api.openai.com/v1/vector_stores", vsContent);
                var vsJson = await vsRes.Content.ReadAsStringAsync();
                if (!vsRes.IsSuccessStatusCode)
                    throw new Exception($"Errore creazione Vector Store: {vsJson}");

                dynamic vsObj = JsonConvert.DeserializeObject(vsJson);
                string vectorStoreId = vsObj.id;

                // 5. PATCH assistant per aggiornare tool_settings.file_search
                var patchData = new
                {
                    tool_resources = new
                    {
                        file_search = new
                        {
                            vector_store_ids = new List<string> { vectorStoreId }
                        }
                    }
                };

                var patchReq = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"https://api.openai.com/v1/assistants/{assistantId}"
                )
                {
                    Content = new StringContent(
                    JsonConvert.SerializeObject(patchData),
                    Encoding.UTF8,
                    "application/json"
                  )
                };

                // Assicurati di rimuovere vecchi header prima di aggiungerne di nuovi
                patchReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                patchReq.Headers.Remove("OpenAI-Beta");
                patchReq.Headers.Add("OpenAI-Beta", "assistants=v2");

                var patchRes = await client.SendAsync(patchReq);
                var patchBody = await patchRes.Content.ReadAsStringAsync();

                if (!patchRes.IsSuccessStatusCode)
                    throw new Exception($"Errore PATCH assistant: {patchBody}");

                // 6. log e salva DB
                var uploaded = new UploadedFile
                {
                    FileName = safeName,
                    FilePath = localFilePath,
                    FileSize = file.Length,
                    FileType = file.ContentType,
                    OpenAIFileId = fileId,
                    VectorStoreId = vectorStoreId,
                    UploadDate = DateTime.UtcNow
                };
                await _unitOfWork.InsertAsync(uploaded);
                await _unitOfWork.SaveChangesAsync();

                log.ResponseCode = 200;
                log.ResponseMessage = "OK";
                log.ResponseBody = JsonConvert.SerializeObject(new
                {
                    file_id = fileId,
                    vector_store_id = vectorStoreId
                });
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new
                {
                    message = "Caricamento completato; Vector Store creato e assistant aggiornato",
                    file_id = fileId,
                    vector_store_id = vectorStoreId
                });
            }
            catch (Exception ex)
            {
                log.ResponseCode = 500;
                log.ResponseMessage = ex.Message;
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return StatusCode(500, $"Errore interno: {ex.Message}");
            }
        }


        private async Task<string> AssistantSent(string message)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.apiKey);
            // Add the required OpenAI - Beta header
            if (!client.DefaultRequestHeaders.Contains("OpenAI-Beta"))
            {
                client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
            };
            // Definisci il messaggio di sistema per personalizzare il comportamento dell'assistente
            var messages = new[]
            {
                new { role = "system", content = "You are a personal somelier and your name is \"Vitinerario\".\r\nWhenever you answer, introduce yourself and greet the person.\r\nIn addition to the name, the hints and the taste of the wine, it also tells the story of the vine that you recommend.\r\nAlways reply in a friendly and informal tone\r\n\r\n" },
                new { role = "user", content = message }
             };

            var requestBody = new
            {
                model = "gpt-4o-mini",  // Cambia modello se necessario               
                messages = messages
            };


            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Errore nella richiesta: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

            return jsonResponse.choices[0].message.content.ToString();
        }

        //[HttpGet("files")]
        //public IActionResult GetUploadedFiles()
        //{
        //    var files = _unitOfWork.UploadedFile.GetAll()
        //        .Select(f => new {
        //            f.FileName,
        //            f.FilePath,
        //            f.FileType,
        //            f.FileSize,
        //            f.UploadDate
        //        }).ToList();

        //    return Ok(files);
        //}

        [HttpGet("files")]
        public async Task<IActionResult> GetUploadedFiles()
        {
            var files = await _unitOfWork.Query<UploadedFile>().Where(e => e.IsDeleted != true)
                .OrderByDescending(f => f.UploadDate)
                .Select(f => new
                {
                    f.FileName,
                    f.FilePath,
                    f.FileType,
                    f.FileSize,
                    f.UploadDate,
                    f.OpenAIFileId,
                    f.VectorStoreId
                })
                .ToListAsync();

            return Ok(files);
        }
    }
}
