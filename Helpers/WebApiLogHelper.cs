using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AI_Integration.Helpers
{
    /// <summary>
    /// Helper centralizzato per creare e persistere WebAPILog.
    /// È stateless: passa l'IUnitOfWork quando salvi.
    /// </summary>
    public static class WebApiLogHelper
    {
        public static WebAPILog NewLog(
            string method,
            string url,
            string? body,
            string? userAgent,
            string? more = null)
            => new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = method,
                RequestUrl = url,
                RequestBody = body ?? string.Empty,
                UserAgent = userAgent ?? string.Empty,
                AdditionalInfo = more ?? string.Empty
            };

        public static async Task LogAsync(
            IUnitOfWork unitOfWork,
            WebAPILog log,
            int code,
            string message,
            string responseBody,
            string? more = null)
        {
            log.ResponseCode = code;
            log.ResponseMessage = message;
            log.ResponseBody = responseBody;

            if (!string.IsNullOrWhiteSpace(more))
            {
                log.AdditionalInfo = string.IsNullOrWhiteSpace(log.AdditionalInfo)
                    ? more
                    : $"{log.AdditionalInfo} | {more}";
            }

            var sql = "EXEC [dbo].[sp_CreaWebAPILog] @RequestMethod={0}, @RequestUrl={1}, @RequestBody={2}, @ResponseBody={3}, @ResponseCode={4}, @ResponseMessage={5}, @UserAgent={6}, @AdditionalInfo={7}";
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(sql,
                log.RequestMethod ?? (object)DBNull.Value,
                log.RequestUrl ?? (object)DBNull.Value,
                log.RequestBody ?? (object)DBNull.Value,
                log.ResponseBody ?? (object)DBNull.Value,
                log.ResponseCode,
                log.ResponseMessage ?? (object)DBNull.Value,
                log.UserAgent ?? (object)DBNull.Value,
                log.AdditionalInfo ?? (object)DBNull.Value
            );
        }

        public static Task LogOkAsync(IUnitOfWork uow, WebAPILog log, string body, string? more = null)
    => LogAsync(uow, log, 200, "OK", body, more);

        public static Task LogBadRequestAsync(IUnitOfWork uow, WebAPILog log, string body, string? more = null)
            => LogAsync(uow, log, 400, "Bad Request", body, more);

        public static Task LogNotFoundAsync(IUnitOfWork uow, WebAPILog log, string body, string? more = null)
            => LogAsync(uow, log, 404, "Not Found", body, more);

        public static Task LogNoContentAsync(IUnitOfWork uow, WebAPILog log, string body, string? more = null)
            => LogAsync(uow, log, 204, "No Content", body, more);

        public static Task LogErrorAsync(IUnitOfWork uow, WebAPILog log, Exception ex, string? more = null)
            => LogAsync(uow, log, 500, "Internal Server Error", $"{{ success = false, message = '{ex.Message}' }}", more);

    }
}
