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
/*===================================================*/

ALTER TABLE Contents
ADD LangID NVARCHAR(10) NOT NULL DEFAULT 1;


ALTER TABLE ContentImages
ADD LangID NVARCHAR(10) NOT NULL DEFAULT 1;

ALTER TABLE ContentLinks
ADD LangID NVARCHAR(10) NOT NULL DEFAULT 1;

ALTER TABLE Events
ADD LangID NVARCHAR(10) NOT NULL DEFAULT 1;

ALTER TABLE EventLinks
ADD LangID NVARCHAR(10) NOT NULL DEFAULT 1;

ALTER TABLE Galleries
ADD LangID NVARCHAR(10) NOT NULL DEFAULT 1;

ALTER TABLE PhotoGalleries
ADD LangID NVARCHAR(10) NOT NULL DEFAULT 1;

ALTER TABLE Podcasts
ADD LangID NVARCHAR(10) NOT NULL DEFAULT 1;

/*===================================================*/



IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Experts')
BEGIN
    CREATE TABLE [dbo].[Experts](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](255) NOT NULL,
        [Bio] [nvarchar](max) NULL,
        [PhotoUrl] [nvarchar](500) NULL,
        [Email] [nvarchar](255) NULL,
        [CreatedAt] [datetime2](7) NULL,
        [LangID] [int] NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Experts] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EventExperts')
BEGIN
    CREATE TABLE [dbo].[EventExperts](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [EventId] [int] NOT NULL,
        [ExpertId] [int] NOT NULL,
        CONSTRAINT [PK_EventExperts] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ContentExperts')
BEGIN
    CREATE TABLE [dbo].[ContentExperts](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ContentId] [int] NOT NULL,
        [ExpertId] [int] NOT NULL,
        CONSTRAINT [PK_ContentExperts] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

ALTER TABLE Events
ADD
 [BookingEndDate] [datetime2](7) NULL,
        [Location] [nvarchar](500) NULL,
        [Organizer] [nvarchar](255) NULL,
        [ContactInfo] [nvarchar](255) NULL,
        [Price] [decimal](18, 2) NOT NULL DEFAULT 0,
        [IsOnline] [bit] NOT NULL DEFAULT 0
		GO