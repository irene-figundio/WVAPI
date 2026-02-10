-- Rename tables to match PascalCase pattern
EXEC sp_rename 'contents', 'Contents';
EXEC sp_rename 'content_images', 'ContentImages';
EXEC sp_rename 'content_links', 'ContentLinks';
EXEC sp_rename 'podcasts', 'Podcasts';
EXEC sp_rename 'events', 'Events';
EXEC sp_rename 'event_links', 'EventLinks';
EXEC sp_rename 'galleries', 'Galleries';
EXEC sp_rename 'photo_gallery', 'PhotoGallery';

-- Rename columns to match PascalCase models (Ad Campaign pattern)
-- Contents
EXEC sp_rename 'Contents.title', 'Title', 'COLUMN';
EXEC sp_rename 'Contents.content', 'Text', 'COLUMN';
EXEC sp_rename 'Contents.publish_date', 'PublishDate', 'COLUMN';
EXEC sp_rename 'Contents.cover_image', 'CoverImage', 'COLUMN';
EXEC sp_rename 'Contents.content_type', 'ContentType', 'COLUMN';
EXEC sp_rename 'Contents.is_published', 'IsPublished', 'COLUMN';
EXEC sp_rename 'Contents.created_at', 'CreatedAt', 'COLUMN';
EXEC sp_rename 'Contents.updated_at', 'UpdatedAt', 'COLUMN';

-- ContentImages
EXEC sp_rename 'ContentImages.content_id', 'ContentId', 'COLUMN';
EXEC sp_rename 'ContentImages.image_url', 'ImageUrl', 'COLUMN';
EXEC sp_rename 'ContentImages.caption', 'Caption', 'COLUMN';
EXEC sp_rename 'ContentImages.position', 'Position', 'COLUMN';
EXEC sp_rename 'ContentImages.created_at', 'CreatedAt', 'COLUMN';

-- ContentLinks
EXEC sp_rename 'ContentLinks.content_id', 'ContentId', 'COLUMN';
EXEC sp_rename 'ContentLinks.link_url', 'LinkUrl', 'COLUMN';
EXEC sp_rename 'ContentLinks.description', 'Description', 'COLUMN';
EXEC sp_rename 'ContentLinks.created_at', 'CreatedAt', 'COLUMN';

-- Podcasts
EXEC sp_rename 'Podcasts.title', 'Title', 'COLUMN';
EXEC sp_rename 'Podcasts.description', 'Description', 'COLUMN';
EXEC sp_rename 'Podcasts.publish_date', 'PublishDate', 'COLUMN';
EXEC sp_rename 'Podcasts.cover_image', 'CoverImage', 'COLUMN';
EXEC sp_rename 'Podcasts.youtube_url', 'YoutubeUrl', 'COLUMN';
EXEC sp_rename 'Podcasts.spotify_url', 'SpotifyUrl', 'COLUMN';
EXEC sp_rename 'Podcasts.created_at', 'CreatedAt', 'COLUMN';

-- Events
EXEC sp_rename 'Events.title', 'Title', 'COLUMN';
EXEC sp_rename 'Events.description', 'Description', 'COLUMN';
EXEC sp_rename 'Events.event_date', 'EventDate', 'COLUMN';
EXEC sp_rename 'Events.cover_image', 'CoverImage', 'COLUMN';
EXEC sp_rename 'Events.gallery_id', 'GalleryId', 'COLUMN';
EXEC sp_rename 'Events.created_at', 'CreatedAt', 'COLUMN';

-- EventLinks
EXEC sp_rename 'EventLinks.event_id', 'EventId', 'COLUMN';
EXEC sp_rename 'EventLinks.link_url', 'LinkUrl', 'COLUMN';
EXEC sp_rename 'EventLinks.description', 'Description', 'COLUMN';

-- Galleries
EXEC sp_rename 'Galleries.event_id', 'EventId', 'COLUMN';
EXEC sp_rename 'Galleries.title', 'Title', 'COLUMN';
EXEC sp_rename 'Galleries.created_at', 'CreatedAt', 'COLUMN';

-- PhotoGallery
EXEC sp_rename 'PhotoGallery.gallery_id', 'GalleryId', 'COLUMN';
EXEC sp_rename 'PhotoGallery.image_url', 'ImageUrl', 'COLUMN';
EXEC sp_rename 'PhotoGallery.caption', 'Caption', 'COLUMN';
EXEC sp_rename 'PhotoGallery.created_at', 'CreatedAt', 'COLUMN';

-- Seed data
-- Contents
INSERT INTO Contents (Title, Text, PublishDate, ContentType, IsPublished)
VALUES ('Primo Articolo News', 'Contenuto del primo articolo news.', GETDATE(), 'news', 1),
       ('Benvenuti nel Blog', 'Questo è il primo post del nostro blog.', GETDATE(), 'blog', 1);

-- ContentImages
INSERT INTO ContentImages (ContentId, ImageUrl, Caption, Position)
VALUES (1, 'https://example.com/image1.jpg', 'Didascalia immagine 1', 1);

-- ContentLinks
INSERT INTO ContentLinks (ContentId, LinkUrl, Description)
VALUES (1, 'https://example.com/more-info', 'Link di approfondimento');

-- Podcasts
INSERT INTO Podcasts (Title, Description, PublishDate, YoutubeUrl)
VALUES ('Episodio 1: AI in 2024', 'Discussione sulle tendenze AI.', GETDATE(), 'https://youtube.com/watch?v=123');

-- Events
INSERT INTO Events (Title, Description, EventDate, CoverImage)
VALUES ('Conferenza Tech 2024', 'Grande evento tech a Milano.', '2024-10-15', 'https://example.com/event.jpg');

-- EventLinks
INSERT INTO EventLinks (EventId, LinkUrl, Description)
VALUES (1, 'https://techconf.com/tickets', 'Acquista Biglietti');

-- Galleries
INSERT INTO Galleries (EventId, Title)
VALUES (1, 'Foto Conferenza Tech 2024');

-- PhotoGallery
INSERT INTO PhotoGallery (GalleryId, ImageUrl, Caption)
VALUES (1, 'https://example.com/gallery1.jpg', 'Foto del palco');
