using AI_Integration.DataAccess.Database.Models;

namespace AI_Integration.DataAccess.Database.Repositories.interfaces
{
    public interface IUnitOfWork
    {
        IWineAIRepository WineAI { get; }
        IWebAPILogRepository WebAPILog { get; }
        IAIVideoRepository AIVideo { get; }
        IAPITokenRepository APIToken { get; }

        IAdAnalyticsRepository ADAnalytics { get; }

        IAdCampaignRepository ADCampaign { get; }
        IAdSessionRepository ADSession { get; }
        IUploadedFileRepository UploadedFile { get; }
        IUsersRepository Users { get; }    // <--- nuovo
        IContentRepository Contents { get; }
        IContentImageRepository ContentImages { get; }
        IContentLinkRepository ContentLinks { get; }
        IPodcastRepository Podcasts { get; }
        IEventRepository Events { get; }
        IEventLinkRepository EventLinks { get; }
        IGalleryRepository Galleries { get; }
        IPhotoGalleryRepository PhotoGalleries { get; }

        ApplicationDbContext Context { get; }

        void Save();
        IRepository<T> Repository<T>() where T : class;
        IQueryable<T> Query<T>(bool asNoTracking = true) where T : class;
        Task<T?> GetByIdAsync<T>(object id, CancellationToken ct = default) where T : class;
        Task InsertAsync<T>(T entity, CancellationToken ct = default) where T : class;
        void Update<T>(T entity) where T : class;
        void Remove<T>(T entity) where T : class;
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

}
