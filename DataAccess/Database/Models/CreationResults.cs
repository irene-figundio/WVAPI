using System;

namespace AI_Integration.DataAccess.Database.Models
{
    public class ContentCreationResult
    {
        public Guid ContentGuid { get; set; }
        public int ContentId { get; set; }
    }

    public class EventCreationResult
    {
        public Guid EventGuid { get; set; }
        public int EventId { get; set; }
    }

    public class ExpertCreationResult
    {
        public int ExpertId { get; set; }
    }

    public class ContentExpertCreationResult
    {
        public int ContentExpertId { get; set; }
    }

    public class EventExpertCreationResult
    {
        public int EventExpertId { get; set; }
    }

    public class PodcastCreationResult
    {
        public int PodcastId { get; set; }
    }

    public class PartnerCreationResult
    {
        public int PartnerId { get; set; }
    }

    public class LanguageCreationResult
    {
        public int LanguageId { get; set; }
    }

    public class AIVideoCreationResult
    {
        public int AIVideoId { get; set; }
    }

    public class APITokenCreationResult
    {
        public int APITokenId { get; set; }
    }

    public class AdAnalyticsCreationResult
    {
        public int AdAnalyticsId { get; set; }
    }

    public class AdCampaignCreationResult
    {
        public int AdCampaignId { get; set; }
    }

    public class AdSessionCreationResult
    {
        public int AdSessionId { get; set; }
    }

    public class ContentLinkCreationResult
    {
        public int ContentLinkId { get; set; }
    }

    public class EventLinkCreationResult
    {
        public int EventLinkId { get; set; }
    }

    public class UploadedFileCreationResult
    {
        public int UploadedFileId { get; set; }
    }

    public class UserCreationResult
    {
        public int UserId { get; set; }
    }

    public class UserStatusCreationResult
    {
        public int UserStatusId { get; set; }
    }

    public class WebAPILogCreationResult
    {
        public int WebAPILogId { get; set; }
    }

    public class WineAICreationResult
    {
        public int WineAIId { get; set; }
    }
}
