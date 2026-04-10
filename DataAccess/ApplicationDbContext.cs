using Abp.Domain.Entities;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Xml.Linq;


namespace AI_Integration.DataAccess
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<WineAI> WineAI { get; set; } = null!;
        public virtual DbSet<WebAPILog> WebAPILog { get; set; } = null!;
        public virtual DbSet<AIVideo> AIVideo { get; set; } = null!;
        public virtual DbSet<APIToken> APIToken { get; set; } = null!;
        public virtual DbSet<AdAnalytics> AdAnalytics { get; set; } = null!;
        public virtual DbSet<AdCampaign> AdCampaign { get; set; } = null!;
        public virtual DbSet<AdSession> AdSession { get; set; } = null!;
        public virtual DbSet<UploadedFile> UploadedFiles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!; // <--- nuovo
        public virtual DbSet<UserStatus> UserStatuses { get; set; } = null!; // <--- nuovo
        public virtual DbSet<Content> Contents { get; set; } = null!;
        public virtual DbSet<ContentImage> ContentImages { get; set; } = null!;
        public virtual DbSet<ContentLink> ContentLinks { get; set; } = null!;
        public virtual DbSet<Podcast> Podcasts { get; set; } = null!;
        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<EventLink> EventLinks { get; set; } = null!;
        public virtual DbSet<Gallery> Galleries { get; set; } = null!;
        public virtual DbSet<PhotoGallery> PhotoGalleries { get; set; } = null!;
        public virtual DbSet<Language> Languages { get; set; } = null!;
        public virtual DbSet<Expert> Experts { get; set; } = null!;
        public virtual DbSet<EventExpert> EventExperts { get; set; } = null!;
        public virtual DbSet<ContentExpert> ContentExperts { get; set; } = null!;
        public virtual DbSet<Partner> Partners { get; set; } = null!;

        public virtual DbSet<EventCategory> EventCategories { get; set; } = null!;
        public virtual DbSet<ContentCategory> ContentCategories { get; set; } = null!;
        public virtual DbSet<Trip> Trips { get; set; } = null!;
        public virtual DbSet<Stay> Stays { get; set; } = null!;
        public virtual DbSet<ItineraryDay> ItineraryDays { get; set; } = null!;
        public virtual DbSet<ItineraryStop> ItineraryStops { get; set; } = null!;
        public virtual DbSet<TripMust> TripMusts { get; set; } = null!;
        public virtual DbSet<VariantPrice> VariantPrices { get; set; } = null!;
        public virtual DbSet<EventNeed> EventNeeds { get; set; } = null!;
        public virtual DbSet<StorageMapping> StorageMappings { get; set; } = null!;
        public virtual DbSet<HeroImage> HeroImages { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=Vitinerario;User Id=sa;Password=sa1;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WineAI>(entity =>
            {
                entity.ToTable("WineAI", "dbo");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.CreationDate);
                entity.Property(e => e.Question).HasMaxLength(150);
                entity.Property(e => e.Answer).HasMaxLength(250);
                entity.Property(e => e.Dispositivo).HasMaxLength(50);
            });
            modelBuilder.Entity<WebAPILog>(entity =>
            {
                entity.ToTable("WebAPILog", "dbo");
                entity.HasKey(e => e.IDLog);
                entity.Property(e => e.IDLog).ValueGeneratedOnAdd(); // Set the primary key
                                                                     // Additional configurations for properties
                entity.Property(e => e.DateTimeStamp).IsRequired(); // DateTimeStamp is required
                entity.Property(e => e.RequestMethod).HasMaxLength(10); // RequestMethod can have max length of 10 characters
                entity.Property(e => e.RequestUrl).HasMaxLength(255); // RequestUrl can have max length of 255 characters
                entity.Property(e => e.RequestBody).HasColumnType("nvarchar(max)"); // RequestBody is of type nvarchar(max)
                entity.Property(e => e.ResponseMessage).HasMaxLength(255); // ResponseMessage can have max length of 255 characters
                
                entity.Property(e => e.UserAgent).HasMaxLength(255); // UserAgent can have max length of 255 characters
                entity.Property(e => e.AdditionalInfo).HasColumnType("nvarchar(max)"); // AdditionalInfo is of type nvarchar(max)

            });
            //modelBuilder.Entity<AIVideo>(entity => {
            //    entity.ToTable("AIVideo", "dbo");
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Set the primary key
            //    entity.HasOne<AdSession>()
            //          .WithMany(session => session.Videos)
            //          .HasForeignKey(video => video.ID_Session);
            //});
            modelBuilder.Entity<APIToken>(entity => {
                entity.ToTable("APIToken", "dbo");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();              
            });

            //modelBuilder.Entity<AdSession>(entity => {
            //    entity.ToTable("AdSession", "dbo");
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Id).ValueGeneratedOnAdd();
            //    entity.HasOne<AdCampaign>()
            //        .WithMany(campaign => campaign.Sessions)
            //        .HasForeignKey(session => session.ID_Campaing);// Set the primary key// Set the primary key
            //});


            modelBuilder.Entity<AdCampaign>(e =>
            {
                e.ToTable("AdCampaign","dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd(); // Set the primary key

                e.Property(x => x.Name).IsRequired();
                e.Property(x => x.Budget).HasColumnType("decimal(18,2)");

                // 1:N AdCampaign -> AdSession
                e.HasMany(x => x.Sessions)
                 .WithOne(s => s.Campaign)
                 .HasForeignKey(s => s.ID_Campaing)
                 .OnDelete(DeleteBehavior.Restrict); // o Cascade, secondo necessità

                // Auditing (se vuoi solo esplicitare i nomi colonna)
                e.Property(x => x.Creation_User).HasColumnName("Creation_User");
                e.Property(x => x.LastModification_User).HasColumnName("LastModification_User");
                e.Property(x => x.Deletion_User).HasColumnName("Deletion_User");
            });

            modelBuilder.Entity<AdSession>(e =>
            {
                e.ToTable("AdSession", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd(); // Set the primary key
                e.HasOne(x => x.Campaign)
                 .WithMany(c => c.Sessions)
                 .HasForeignKey(x => x.ID_Campaing)
                 .HasConstraintName("ID_Campaign"); // come DDL
            });

            modelBuilder.Entity<AIVideo>(e =>
            {
                e.ToTable("AIVideo","dbo");
                e.HasKey(x => x.Id);
                e.Property(e => e.Id).ValueGeneratedOnAdd();
                e.HasOne(v => v.Session)
                 .WithMany(s => s.Videos)
                 .HasForeignKey(v => v.ID_Session)
                 .HasConstraintName("FK_AIVideo_AdSession");

                // Auditing
                e.Property(x => x.Creation_User).HasColumnName("Creation_User");
                e.Property(x => x.LastModification_User).HasColumnName("LastModification_User");
                e.Property(x => x.Deletion_User).HasColumnName("Deletion_User");
            });




            modelBuilder.Entity<AdAnalytics>(entity => {
                entity.ToTable("AdAnalytics", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Set the primary key
            });

            modelBuilder.Entity<UploadedFile>(entity =>
            {
                entity.ToTable("UploadedFiles", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Set the primary key
                entity.Property(e => e.FileName).HasMaxLength(255); // Set max length for FileName
                entity.Property(e => e.FilePath).HasMaxLength(255); // Set max length for FilePath
                entity.Property(e => e.UploadDate).HasColumnType("datetime"); // Set type for UploadDate
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Status)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.StatusId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.CreationUser)
                .WithMany(c => c.CreatedUsers)
                .HasForeignKey(u => u.CreationUserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.LastModificationUser)
                .WithMany(c => c.ModifiedUsers)
                .HasForeignKey(u => u.LastModificationUserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.DeletionUser)
                .WithMany(c => c.DeletedUsers)
                .HasForeignKey(u => u.DeletionUserId);

            modelBuilder.Entity<Content>(e =>
            {
                e.ToTable("Contents", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Guid).HasDefaultValueSql("NEWID()");
                e.HasIndex(x => x.Guid).IsUnique();
                e.Property(x => x.Title).IsRequired().HasMaxLength(255);
                e.Property(x => x.Text).IsRequired();
                e.Property(x => x.PublishDate).HasColumnType("date");
                e.Property(x => x.ContentType).IsRequired().HasMaxLength(20);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
                e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");
            });

            modelBuilder.Entity<ContentImage>(e =>
            {
                e.ToTable("ContentImages", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
                e.HasOne(x => x.Content)
                 .WithMany(c => c.ContentImages)
                 .HasForeignKey(x => x.ContentId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ContentLink>(e =>
            {
                e.ToTable("ContentLinks", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
                e.HasOne(x => x.Content)
                 .WithMany(c => c.ContentLinks)
                 .HasForeignKey(x => x.ContentId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Podcast>(e =>
            {
                e.ToTable("Podcasts", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Title).IsRequired().HasMaxLength(255);
                e.Property(x => x.Description).IsRequired();
                e.Property(x => x.PublishDate).HasColumnType("date");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            });

            modelBuilder.Entity<Event>(e =>
            {
                e.ToTable("Events", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Guid).HasDefaultValueSql("NEWID()");
                e.HasIndex(x => x.Guid).IsUnique();
                e.Property(x => x.Title).IsRequired().HasMaxLength(255);
                e.Property(x => x.Description).IsRequired();
                e.Property(x => x.EventDate).HasColumnType("date");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            });
            modelBuilder.Entity<Event>()
    .ToTable("Events", "dbo", tb => tb.HasTrigger("trg_Events_SetStartParts"));

            modelBuilder.Entity<EventLink>(e =>
            {
                e.ToTable("EventLinks", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasOne(x => x.Event)
                 .WithMany(ev => ev.EventLinks)
                 .HasForeignKey(x => x.EventId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Gallery>(e =>
            {
                e.ToTable("Galleries", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
                e.HasOne(x => x.Event)
                 .WithOne(ev => ev.Gallery)
                 .HasForeignKey<Gallery>(x => x.EventId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PhotoGallery>(e =>
            {
                e.ToTable("PhotoGallery", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
                e.HasOne(x => x.Gallery)
                 .WithMany(g => g.Photos)
                 .HasForeignKey(x => x.GalleryId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Trip>(e =>
            {
                e.ToTable("Trips", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasOne(x => x.Event)
                 .WithMany()
                 .HasForeignKey(x => x.EventId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ItineraryDay>(e =>
            {
                e.ToTable("ItineraryDays", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasOne(x => x.Trip)
                 .WithMany(t => t.ItineraryDays)
                 .HasForeignKey(x => x.TripId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Stay>(e =>
            {
                e.ToTable("Stays", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasOne(x => x.Trip)
                 .WithMany(t => t.Stays)
                 .HasForeignKey(x => x.TripId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.ItineraryDay)
                 .WithMany(d => d.Stays)
                 .HasForeignKey(x => x.ItineraryDayId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ItineraryStop>(e =>
            {
                e.ToTable("ItineraryStops", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasOne(x => x.ItineraryDay)
                 .WithMany(d => d.ItineraryStops)
                 .HasForeignKey(x => x.DayId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TripMust>(e =>
            {
                e.ToTable("TripMusts", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasOne(x => x.Trip)
                 .WithMany(t => t.TripMusts)
                 .HasForeignKey(x => x.TripId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VariantPrice>(e =>
            {
                e.ToTable("VariantPrices", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasOne(x => x.Event)
                 .WithMany()
                 .HasForeignKey(x => x.EventId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EventNeed>(e =>
            {
                e.ToTable("EventNeeds", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasOne(x => x.Event)
                 .WithMany()
                 .HasForeignKey(x => x.EventId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StorageMapping>(e =>
            {
                e.ToTable("StorageMappings", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasIndex(x => new { x.ParentType, x.ParentId, x.StorageArea }).HasDatabaseName("IX_StorageMappings_Parent");
            });

            modelBuilder.Entity<HeroImage>(e =>
            {
                e.ToTable("HeroImages", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
