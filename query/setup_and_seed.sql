-- Use the database
USE [Vitinerario];
GO

-- Rename tables to match PascalCase pattern
IF OBJECT_ID('dbo.contents', 'U') IS NOT NULL EXEC sp_rename 'dbo.contents', 'Contents';
IF OBJECT_ID('dbo.content_images', 'U') IS NOT NULL EXEC sp_rename 'dbo.content_images', 'ContentImages';
IF OBJECT_ID('dbo.content_links', 'U') IS NOT NULL EXEC sp_rename 'dbo.content_links', 'ContentLinks';
IF OBJECT_ID('dbo.podcasts', 'U') IS NOT NULL EXEC sp_rename 'dbo.podcasts', 'Podcasts';
IF OBJECT_ID('dbo.events', 'U') IS NOT NULL EXEC sp_rename 'dbo.events', 'Events';
IF OBJECT_ID('dbo.event_links', 'U') IS NOT NULL EXEC sp_rename 'dbo.event_links', 'EventLinks';
IF OBJECT_ID('dbo.galleries', 'U') IS NOT NULL EXEC sp_rename 'dbo.galleries', 'Galleries';
IF OBJECT_ID('dbo.photo_gallery', 'U') IS NOT NULL EXEC sp_rename 'dbo.photo_gallery', 'PhotoGallery';
GO

-- Rename columns to match PascalCase models
-- Contents
IF COL_LENGTH('dbo.Contents', 'title') IS NOT NULL EXEC sp_rename 'dbo.Contents.title', 'Title', 'COLUMN';
IF COL_LENGTH('dbo.Contents', 'content') IS NOT NULL EXEC sp_rename 'dbo.Contents.content', 'Text', 'COLUMN';
IF COL_LENGTH('dbo.Contents', 'publish_date') IS NOT NULL EXEC sp_rename 'dbo.Contents.publish_date', 'PublishDate', 'COLUMN';
IF COL_LENGTH('dbo.Contents', 'cover_image') IS NOT NULL EXEC sp_rename 'dbo.Contents.cover_image', 'CoverImage', 'COLUMN';
IF COL_LENGTH('dbo.Contents', 'content_type') IS NOT NULL EXEC sp_rename 'dbo.Contents.content_type', 'ContentType', 'COLUMN';
IF COL_LENGTH('dbo.Contents', 'is_published') IS NOT NULL EXEC sp_rename 'dbo.Contents.is_published', 'IsPublished', 'COLUMN';
IF COL_LENGTH('dbo.Contents', 'created_at') IS NOT NULL EXEC sp_rename 'dbo.Contents.created_at', 'CreatedAt', 'COLUMN';
IF COL_LENGTH('dbo.Contents', 'updated_at') IS NOT NULL EXEC sp_rename 'dbo.Contents.updated_at', 'UpdatedAt', 'COLUMN';

-- ContentImages
IF COL_LENGTH('dbo.ContentImages', 'content_id') IS NOT NULL EXEC sp_rename 'dbo.ContentImages.content_id', 'ContentId', 'COLUMN';
IF COL_LENGTH('dbo.ContentImages', 'image_url') IS NOT NULL EXEC sp_rename 'dbo.ContentImages.image_url', 'ImageUrl', 'COLUMN';
IF COL_LENGTH('dbo.ContentImages', 'caption') IS NOT NULL EXEC sp_rename 'dbo.ContentImages.caption', 'Caption', 'COLUMN';
IF COL_LENGTH('dbo.ContentImages', 'position') IS NOT NULL EXEC sp_rename 'dbo.ContentImages.position', 'Position', 'COLUMN';
IF COL_LENGTH('dbo.ContentImages', 'created_at') IS NOT NULL EXEC sp_rename 'dbo.ContentImages.created_at', 'CreatedAt', 'COLUMN';

-- ContentLinks
IF COL_LENGTH('dbo.ContentLinks', 'content_id') IS NOT NULL EXEC sp_rename 'dbo.ContentLinks.content_id', 'ContentId', 'COLUMN';
IF COL_LENGTH('dbo.ContentLinks', 'link_url') IS NOT NULL EXEC sp_rename 'dbo.ContentLinks.link_url', 'LinkUrl', 'COLUMN';
IF COL_LENGTH('dbo.ContentLinks', 'description') IS NOT NULL EXEC sp_rename 'dbo.ContentLinks.description', 'Description', 'COLUMN';
IF COL_LENGTH('dbo.ContentLinks', 'created_at') IS NOT NULL EXEC sp_rename 'dbo.ContentLinks.created_at', 'CreatedAt', 'COLUMN';

-- Podcasts
IF COL_LENGTH('dbo.Podcasts', 'title') IS NOT NULL EXEC sp_rename 'dbo.Podcasts.title', 'Title', 'COLUMN';
IF COL_LENGTH('dbo.Podcasts', 'description') IS NOT NULL EXEC sp_rename 'dbo.Podcasts.description', 'Description', 'COLUMN';
IF COL_LENGTH('dbo.Podcasts', 'publish_date') IS NOT NULL EXEC sp_rename 'dbo.Podcasts.publish_date', 'PublishDate', 'COLUMN';
IF COL_LENGTH('dbo.Podcasts', 'cover_image') IS NOT NULL EXEC sp_rename 'dbo.Podcasts.cover_image', 'CoverImage', 'COLUMN';
IF COL_LENGTH('dbo.Podcasts', 'youtube_url') IS NOT NULL EXEC sp_rename 'dbo.Podcasts.youtube_url', 'YoutubeUrl', 'COLUMN';
IF COL_LENGTH('dbo.Podcasts', 'spotify_url') IS NOT NULL EXEC sp_rename 'dbo.Podcasts.spotify_url', 'SpotifyUrl', 'COLUMN';
IF COL_LENGTH('dbo.Podcasts', 'created_at') IS NOT NULL EXEC sp_rename 'dbo.Podcasts.created_at', 'CreatedAt', 'COLUMN';

-- Events
IF COL_LENGTH('dbo.Events', 'title') IS NOT NULL EXEC sp_rename 'dbo.Events.title', 'Title', 'COLUMN';
IF COL_LENGTH('dbo.Events', 'description') IS NOT NULL EXEC sp_rename 'dbo.Events.description', 'Description', 'COLUMN';
IF COL_LENGTH('dbo.Events', 'event_date') IS NOT NULL EXEC sp_rename 'dbo.Events.event_date', 'EventDate', 'COLUMN';
IF COL_LENGTH('dbo.Events', 'cover_image') IS NOT NULL EXEC sp_rename 'dbo.Events.cover_image', 'CoverImage', 'COLUMN';
IF COL_LENGTH('dbo.Events', 'gallery_id') IS NOT NULL EXEC sp_rename 'dbo.Events.gallery_id', 'GalleryId', 'COLUMN';
IF COL_LENGTH('dbo.Events', 'created_at') IS NOT NULL EXEC sp_rename 'dbo.Events.created_at', 'CreatedAt', 'COLUMN';

-- EventLinks
IF COL_LENGTH('dbo.EventLinks', 'event_id') IS NOT NULL EXEC sp_rename 'dbo.EventLinks.event_id', 'EventId', 'COLUMN';
IF COL_LENGTH('dbo.EventLinks', 'link_url') IS NOT NULL EXEC sp_rename 'dbo.EventLinks.link_url', 'LinkUrl', 'COLUMN';
IF COL_LENGTH('dbo.EventLinks', 'description') IS NOT NULL EXEC sp_rename 'dbo.EventLinks.description', 'Description', 'COLUMN';

-- Galleries
IF COL_LENGTH('dbo.Galleries', 'event_id') IS NOT NULL EXEC sp_rename 'dbo.Galleries.event_id', 'EventId', 'COLUMN';
IF COL_LENGTH('dbo.Galleries', 'title') IS NOT NULL EXEC sp_rename 'dbo.Galleries.title', 'Title', 'COLUMN';
IF COL_LENGTH('dbo.Galleries', 'created_at') IS NOT NULL EXEC sp_rename 'dbo.Galleries.created_at', 'CreatedAt', 'COLUMN';

-- PhotoGallery
IF COL_LENGTH('dbo.PhotoGallery', 'gallery_id') IS NOT NULL EXEC sp_rename 'dbo.PhotoGallery.gallery_id', 'GalleryId', 'COLUMN';
IF COL_LENGTH('dbo.PhotoGallery', 'image_url') IS NOT NULL EXEC sp_rename 'dbo.PhotoGallery.image_url', 'ImageUrl', 'COLUMN';
IF COL_LENGTH('dbo.PhotoGallery', 'caption') IS NOT NULL EXEC sp_rename 'dbo.PhotoGallery.caption', 'Caption', 'COLUMN';
IF COL_LENGTH('dbo.PhotoGallery', 'created_at') IS NOT NULL EXEC sp_rename 'dbo.PhotoGallery.created_at', 'CreatedAt', 'COLUMN';
GO

-- Seed data in separate batch to avoid "Invalid column name" errors during parsing
-- Contents
INSERT INTO dbo.Contents (Title, [Text], PublishDate, ContentType, IsPublished)
VALUES ('Primo Articolo News', 'Contenuto del primo articolo news.', GETDATE(), 'news', 1),
       ('Benvenuti nel Blog', 'Questo è il primo post del nostro blog.', GETDATE(), 'blog', 1);
GO

-- ContentImages
INSERT INTO dbo.ContentImages (ContentId, ImageUrl, Caption, Position)
VALUES (1, 'https://example.com/image1.jpg', 'Didascalia immagine 1', 1);
GO

-- ContentLinks
INSERT INTO dbo.ContentLinks (ContentId, LinkUrl, Description)
VALUES (1, 'https://example.com/more-info', 'Link di approfondimento');
GO

-- Podcasts
INSERT INTO dbo.Podcasts (Title, Description, PublishDate, YoutubeUrl)
VALUES ('Episodio 1: AI in 2024', 'Discussione sulle tendenze AI.', GETDATE(), 'https://youtube.com/watch?v=123');
GO

-- Events
INSERT INTO dbo.Events (Title, Description, EventDate, CoverImage)
VALUES ('Conferenza Tech 2024', 'Grande evento tech a Milano.', '2024-10-15', 'https://example.com/event.jpg');
GO

-- EventLinks
INSERT INTO dbo.EventLinks (EventId, LinkUrl, Description)
VALUES (1, 'https://techconf.com/tickets', 'Acquista Biglietti');
GO

-- Galleries
INSERT INTO dbo.Galleries (EventId, Title)
VALUES (1, 'Foto Conferenza Tech 2024');
GO

-- PhotoGallery
INSERT INTO dbo.PhotoGallery (GalleryId, ImageUrl, Caption)
VALUES (1, 'https://example.com/gallery1.jpg', 'Foto del palco');
GO
