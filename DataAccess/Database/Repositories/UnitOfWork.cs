using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.impl;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly ConcurrentDictionary<Type, object> _repos = new();
        private bool _disposed;

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (_repos.TryGetValue(type, out var repo))
                return (IRepository<T>)repo;

            var newRepo = new EfRepository<T>(_db);
            _repos[type] = newRepo;
            return newRepo;
        }
        public IQueryable<T> Query<T>(bool asNoTracking = true) where T : class
            => Repository<T>().Query(asNoTracking);

        public Task<T?> GetByIdAsync<T>(object id, CancellationToken ct = default) where T : class
            => Repository<T>().GetByIdAsync(id, ct);

        public Task InsertAsync<T>(T entity, CancellationToken ct = default) where T : class
            => Repository<T>().InsertAsync(entity, ct);

        public void Update<T>(T entity) where T : class => Repository<T>().Update(entity);
        public void Remove<T>(T entity) where T : class => Repository<T>().Remove(entity);
        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

        public void Dispose()
        {
            if (_disposed) return;
            _db.Dispose();
            _disposed = true;
        }
        public IWineAIRepository WineAI { get; private set; }

        public IWebAPILogRepository WebAPILog { get; private set; }

        public IAIVideoRepository AIVideo { get; private set; }

        public IAPITokenRepository APIToken { get; private set; }

        public IAdAnalyticsRepository ADAnalytics { get; private set; }

        public IAdCampaignRepository ADCampaign { get; private set; }

        public IAdSessionRepository ADSession { get; private set; }
        public IUploadedFileRepository UploadedFile { get; private set; }
        public IUsersRepository Users { get; private set; } // <--- nuovo
        public IContentRepository Contents { get; private set; }
        public IContentImageRepository ContentImages { get; private set; }
        public IContentLinkRepository ContentLinks { get; private set; }
        public IPodcastRepository Podcasts { get; private set; }
        public IEventRepository Events { get; private set; }
        public IEventLinkRepository EventLinks { get; private set; }
        public IGalleryRepository Galleries { get; private set; }
        public IPhotoGalleryRepository PhotoGalleries { get; private set; }

        public ApplicationDbContext Context => _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            WineAI = new WineAIRepository(db);
            WebAPILog = new WebAPILogRepository(db);
            AIVideo = new AIVideoRepository(db);
            APIToken = new APITokenRepository(db);
            ADAnalytics = new AdAnalyticsRepository(db);
            ADCampaign = new AdCampaignRepository(db);
            ADSession = new AdSessionRepository(db);
            UploadedFile = new UploadedFileRepository(db);
            Users = new UsersRepository(db);
            Contents = new ContentRepository(db);
            ContentImages = new ContentImageRepository(db);
            ContentLinks = new ContentLinkRepository(db);
            Podcasts = new PodcastRepository(db);
            Events = new EventRepository(db);
            EventLinks = new EventLinkRepository(db);
            Galleries = new GalleryRepository(db);
            PhotoGalleries = new PhotoGalleryRepository(db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }


    }

}
