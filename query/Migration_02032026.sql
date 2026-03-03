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



