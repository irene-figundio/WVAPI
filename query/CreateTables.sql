-- Script for creating tables based on EF Core models in Vitinerario project

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Languages')
BEGIN
    CREATE TABLE [dbo].[Languages](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Code] [nvarchar](10) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        CONSTRAINT [PK_Languages] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AIVideos')
BEGIN
    CREATE TABLE [dbo].[AIVideos](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [Dir_Path] [nvarchar](max) NULL,
        [Title] [nvarchar](max) NULL,
        [Url_Video] [nvarchar](max) NULL,
        [Play_Priority] [int] NULL,
        [IsLandscape] [bit] NULL,
        [ID_Session] [int] NOT NULL,
        [DataCreation] [datetime2](7) NOT NULL,
        [Creation_User] [int] NULL,
        [DataUpdate] [datetime2](7) NULL,
        [LastModification_User] [int] NULL,
        [IsDeleted] [bit] NULL,
        [DeletionTime] [datetime2](7) NULL,
        [Deletion_User] [int] NULL,
        CONSTRAINT [PK_AIVideos] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'APITokens')
BEGIN
    CREATE TABLE [dbo].[APITokens](
        [ID] [int] IDENTITY(1,1) NOT NULL,
        [DataCreation] [datetime2](7) NOT NULL,
        [Token] [nvarchar](max) NOT NULL,
        [Platform] [nvarchar](max) NOT NULL,
        [DataExpiration] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_APITokens] PRIMARY KEY CLUSTERED ([ID] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AdAnalytics')
BEGIN
    CREATE TABLE [dbo].[AdAnalytics](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [SessionId] [int] NOT NULL,
        [VideoId] [int] NOT NULL,
        [NumViews] [int] NULL,
        [NumClick] [int] NULL,
        [Platform] [nvarchar](max) NOT NULL,
        [SendDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_AdAnalytics] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AdCampaigns')
BEGIN
    CREATE TABLE [dbo].[AdCampaigns](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [Name] [nvarchar](max) NOT NULL,
        [Description] [nvarchar](max) NULL,
        [StartDate] [datetime2](7) NOT NULL,
        [EndDate] [datetime2](7) NOT NULL,
        [Budget] [decimal](18, 2) NULL,
        [CreationTime] [datetime2](7) NULL,
        [Creation_User] [int] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModification_User] [int] NULL,
        [IsDeleted] [bit] NULL,
        [DeletionTime] [datetime2](7) NULL,
        [Deletion_User] [int] NULL,
        CONSTRAINT [PK_AdCampaigns] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AdSessions')
BEGIN
    CREATE TABLE [dbo].[AdSessions](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [ID_Campaing] [int] NOT NULL,
        [StartDate] [datetime2](7) NOT NULL,
        [EndDate] [datetime2](7) NOT NULL,
        [CreationTime] [datetime2](7) NULL,
        [Creation_User] [int] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModification_User] [int] NULL,
        [IsDeleted] [bit] NULL,
        [DeletionTime] [datetime2](7) NULL,
        [Deletion_User] [int] NULL,
        CONSTRAINT [PK_AdSessions] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Contents')
BEGIN
    CREATE TABLE [dbo].[Contents](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [Title] [nvarchar](255) NOT NULL,
        [Text] [nvarchar](max) NOT NULL,
        [PublishDate] [datetime2](7) NOT NULL,
        [CoverImage] [nvarchar](500) NULL,
        [ContentType] [nvarchar](20) NOT NULL,
        [IsPublished] [bit] NULL,
        [CreatedAt] [datetime2](7) NULL,
        [UpdatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_Contents] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ContentImages')
BEGIN
    CREATE TABLE [dbo].[ContentImages](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [ContentId] [int] NOT NULL,
        [ImageUrl] [nvarchar](500) NOT NULL,
        [Caption] [nvarchar](255) NULL,
        [Position] [int] NULL,
        [CreatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_ContentImages] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ContentLinks')
BEGIN
    CREATE TABLE [dbo].[ContentLinks](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [ContentId] [int] NOT NULL,
        [LinkUrl] [nvarchar](500) NOT NULL,
        [Description] [nvarchar](255) NULL,
        [CreatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_ContentLinks] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Events')
BEGIN
    CREATE TABLE [dbo].[Events](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [Title] [nvarchar](255) NOT NULL,
        [Description] [nvarchar](max) NOT NULL,
        [EventDate] [datetime2](7) NOT NULL,
        [CoverImage] [nvarchar](500) NULL,
        [GalleryId] [int] NULL,
        [CreatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EventLinks')
BEGIN
    CREATE TABLE [dbo].[EventLinks](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [EventId] [int] NOT NULL,
        [LinkUrl] [nvarchar](500) NOT NULL,
        [Description] [nvarchar](255) NULL,
        CONSTRAINT [PK_EventLinks] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Galleries')
BEGIN
    CREATE TABLE [dbo].[Galleries](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [EventId] [int] NOT NULL,
        [Title] [nvarchar](255) NULL,
        [CreatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_Galleries] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PhotoGalleries')
BEGIN
    CREATE TABLE [dbo].[PhotoGalleries](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [GalleryId] [int] NOT NULL,
        [ImageUrl] [nvarchar](500) NOT NULL,
        [Caption] [nvarchar](255) NULL,
        [CreatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_PhotoGalleries] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Podcasts')
BEGIN
    CREATE TABLE [dbo].[Podcasts](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [Title] [nvarchar](255) NOT NULL,
        [Description] [nvarchar](max) NOT NULL,
        [PublishDate] [datetime2](7) NOT NULL,
        [CoverImage] [nvarchar](500) NULL,
        [YoutubeUrl] [nvarchar](500) NULL,
        [SpotifyUrl] [nvarchar](500) NULL,
        [CreatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_Podcasts] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UploadedFiles')
BEGIN
    CREATE TABLE [dbo].[UploadedFiles](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [FileName] [nvarchar](max) NOT NULL,
        [FilePath] [nvarchar](max) NOT NULL,
        [FileType] [nvarchar](max) NOT NULL,
        [FileSize] [bigint] NOT NULL,
        [OpenAIFileId] [nvarchar](max) NOT NULL,
        [VectorStoreId] [nvarchar](max) NOT NULL,
        [UploadDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_UploadedFiles] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE [dbo].[Users](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [Username] [nvarchar](max) NOT NULL,
        [Password] [nvarchar](max) NOT NULL,
        [Email] [nvarchar](max) NOT NULL,
        [StatusId] [int] NOT NULL,
        [StatusTime] [datetime2](7) NOT NULL,
        [LastLoginTime] [datetime2](7) NULL,
        [LastPasswordModificationTime] [datetime2](7) NULL,
        [CreationTime] [datetime2](7) NOT NULL,
        [CreationUserId] [int] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModificationUserId] [int] NULL,
        [IsDeleted] [bit] NOT NULL,
        [DeletionTime] [datetime2](7) NULL,
        [DeletionUserId] [int] NULL,
        [SuperAdmin] [bit] NOT NULL,
        [VerificationToken] [nvarchar](max) NOT NULL,
        [ResetPasswordCode] [nvarchar](max) NULL,
        [ResetPasswordCodeExpiration] [datetime2](7) NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserStatuses')
BEGIN
    CREATE TABLE [dbo].[UserStatuses](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        [Name] [nvarchar](max) NOT NULL,
        [ResourceKey] [nvarchar](max) NOT NULL,
        CONSTRAINT [PK_UserStatuses] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WebAPILogs')
BEGIN
    CREATE TABLE [dbo].[WebAPILogs](
        [IDLog] [int] IDENTITY(1,1) NOT NULL,
        [DateTimeStamp] [datetime2](7) NOT NULL,
        [RequestMethod] [nvarchar](max) NOT NULL,
        [RequestUrl] [nvarchar](max) NOT NULL,
        [RequestBody] [nvarchar](max) NOT NULL,
        [ResponseBody] [nvarchar](max) NOT NULL,
        [ResponseCode] [int] NOT NULL,
        [ResponseMessage] [nvarchar](max) NOT NULL,
        [UserAgent] [nvarchar](max) NOT NULL,
        [AdditionalInfo] [nvarchar](max) NOT NULL,
        CONSTRAINT [PK_WebAPILogs] PRIMARY KEY CLUSTERED ([IDLog] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WineAIs')
BEGIN
    CREATE TABLE [dbo].[WineAIs](
        [ID] [int] IDENTITY(1,1) NOT NULL,
        [CreationDate] [datetime2](7) NOT NULL,
        [Question] [nvarchar](max) NOT NULL,
        [Answer] [nvarchar](max) NOT NULL,
        [Dispositivo] [nvarchar](max) NOT NULL,
        CONSTRAINT [PK_WineAIs] PRIMARY KEY CLUSTERED ([ID] ASC)
    )
END
GO
