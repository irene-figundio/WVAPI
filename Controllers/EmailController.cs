using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using static Abp.Net.Mail.EmailSettingNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AI_Integration.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IOptions<MailSettings> _mailSettings;
        private MailSettings mailSettings;
        private readonly IUnitOfWork _unitOfWork;
        public MailHelper.MailHelper _mailHelper { get; set; }

        public EmailController(IOptions<MailSettings> _MailSettings, IUnitOfWork unitOfWork)
        {
            _mailSettings = _MailSettings;           
            _unitOfWork = unitOfWork;
            this.mailSettings = _mailSettings.Value;

            _mailHelper = new MailHelper.MailHelper
            {
                FromEmail = mailSettings.From,
                FromEmailPwd = mailSettings.SMTP.Password,
                Host = mailSettings.SMTP.Server,
                Port = mailSettings.SMTP.Port,
                EnableSSL = false,
                SenderName = mailSettings.Sender
            };

        }
        // POST api/email/sendemail
        //[HttpPost("SendEmail")]

        //public IActionResult Post([FromBody] EmailMessage model, [FromHeader(Name = "User-Agent")] string userAgent)
        //{
        //    var log = new WebAPILog
        //    {
        //        DateTimeStamp = DateTime.Now,
        //        RequestMethod = "POST",
        //        RequestUrl = "api/email/sendemail",
        //        RequestBody = model.ToString(),
        //        UserAgent = userAgent,
        //        AdditionalInfo = ""
        //    };

        //    try
        //    {
        //        this.mailSettings = _mailSettings.Value;

        //        if (string.IsNullOrEmpty(model.Email))
        //        {
        //            log.ResponseCode = 400;
        //            log.ResponseMessage = "Bad Request";
        //            log.ResponseBody = "{ success = false, message = 'Email is required.' }";
        //            _unitOfWork.WebAPILog.AddLog(log);
        //            return BadRequest(new { success = false, message = "Email is required." });
        //        }

        //        var toEmail = mailSettings.SMTP.User;

        //        // Creazione del messaggio email
        //        MailMessage message = new MailMessage();
        //        message.From = new MailAddress(model.Email);
        //        message.To.Add(toEmail);
        //        message.Subject = model.Subject;
        //        message.Body = model.Body;

        //        // Invio dell'email utilizzando il server SMTP
        //        using (SmtpClient smtpClient = new SmtpClient(mailSettings.SMTP.Server, mailSettings.SMTP.Port))
        //        {
        //            smtpClient.UseDefaultCredentials = false;
        //            smtpClient.Credentials = new NetworkCredential(mailSettings.SMTP.User, mailSettings.SMTP.Password);
        //            smtpClient.EnableSsl = true;
        //           // smtpClient.Timeout = mailSettings.SMTP.Timeout;
        //            smtpClient.Timeout = 30000;
        //            smtpClient.Send(message);
        //        }

        //        log.ResponseBody = "{success = true, message = 'Email sent successfully.' }";
        //        log.ResponseCode = 200;
        //        log.ResponseMessage = "OK";
        //        _unitOfWork.WebAPILog.AddLog(log);
        //        return Ok(new { success = true, message = "Email sent successfully." });
        //    }
        //    catch (SmtpException smtpEx)
        //    {
        //        log.ResponseCode = 500;
        //        log.ResponseMessage = $"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}";
        //        _unitOfWork.WebAPILog.AddLog(log);
        //        return StatusCode(500, new { success = false, message = $"SMTP Error: {smtpEx.Message}" });
        //    }
        //    catch (Exception ex)
        //    {
        //        log.ResponseCode = 500;
        //        log.ResponseMessage = $"Si è verificato un errore: {ex.Message}";
        //        _unitOfWork.WebAPILog.AddLog(log);
        //        return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        //    }
        //}



        [HttpPost("SendEmail")]
        public async Task<IActionResult> Post([FromBody] EmailMessage model, [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/email/sendemail",
                RequestBody = model.ToString(),
                UserAgent = userAgent,
                AdditionalInfo = ""
            };
            var email = model.Email;
            if (string.IsNullOrEmpty(email))
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = "{ success = false, message = 'Email is required.' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = "Email is required." });
            }

            try
            {
                
                var mailHelper = this._mailHelper;
                var toEmail = mailSettings.SMTP.User;
                //mailHelper.EnableSSL= true;
                var request = "Nuova richiesta utente da " + email + "  <br> <br> " + model.Body;
                mailHelper.SendEmail(toEmail, "Nuova richiesta utente", request);

                log.ResponseBody = "{success = true, message = 'Email sent successfully.' }";
                log.ResponseCode = 200;
                log.ResponseMessage = "OK";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new { success = true, message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                log.ResponseCode = 500;
                log.ResponseMessage = $"Si è verificato un errore: {ex.Message}";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        //// PUT api/<EmailController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<EmailController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
