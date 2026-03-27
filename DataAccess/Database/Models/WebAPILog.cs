using System;
namespace AI_Integration.DataAccess.Database.Models
{
    public class WebAPILog
    {
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletionDate { get; set; }
        public int IDLog { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public string RequestMethod { get; set; }
        public string RequestUrl { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string UserAgent { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
