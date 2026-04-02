-- Use the database
USE [Vitinerario];
GO

-- Variables to hold generated IDs
DECLARE @NewsId INT;
DECLARE @BlogId INT;
DECLARE @EventId INT;
DECLARE @GalleryId INT;

-- =========================================================
-- 1. Insert a News
-- =========================================================

INSERT INTO dbo.Contents (Title, [Text], PublishDate, CoverImage, ContentType, IsPublished, CreatedAt, UpdatedAt)
VALUES ('New AI Features Released', 'We are excited to announce new AI integration features...', GETDATE(), 'https://example.com/images/ai-news.jpg', 'news', 1, GETDATE(), GETDATE());

SET @NewsId = SCOPE_IDENTITY();

-- Insert an image for the news
INSERT INTO dbo.ContentImages (ContentId, ImageUrl, Caption, Position, CreatedAt)
VALUES (@NewsId, 'https://example.com/images/ai-news-1.jpg', 'AI Diagram', 1, GETDATE());

-- Insert a link for the news
INSERT INTO dbo.ContentLinks (ContentId, LinkUrl, Description, CreatedAt)
VALUES (@NewsId, 'https://example.com/ai-release-notes', 'Release Notes', GETDATE());


-- =========================================================
-- 2. Insert an Article (Blog)
-- =========================================================

INSERT INTO dbo.Contents (Title, [Text], PublishDate, CoverImage, ContentType, IsPublished, CreatedAt, UpdatedAt)
VALUES ('How AI is changing the World', 'Artificial intelligence is revolutionizing various industries...', GETDATE(), 'https://example.com/images/ai-blog.jpg', 'blog', 1, GETDATE(), GETDATE());

SET @BlogId = SCOPE_IDENTITY();

-- Insert an image for the article
INSERT INTO dbo.ContentImages (ContentId, ImageUrl, Caption, Position, CreatedAt)
VALUES (@BlogId, 'https://example.com/images/ai-blog-1.jpg', 'Industry Chart', 1, GETDATE());

-- Insert a link for the article
INSERT INTO dbo.ContentLinks (ContentId, LinkUrl, Description, CreatedAt)
VALUES (@BlogId, 'https://example.com/ai-world', 'Read more about AI in the world', GETDATE());


-- =========================================================
-- 3. Insert an Event
-- =========================================================

INSERT INTO dbo.Events (Title, Description, EventDate, CoverImage, CreatedAt)
VALUES ('AI Summit 2024', 'Join us for the biggest AI conference of the year.', '2024-11-20', 'https://example.com/images/ai-summit.jpg', GETDATE());

SET @EventId = SCOPE_IDENTITY();

-- Insert a link for the event
INSERT INTO dbo.EventLinks (EventId, LinkUrl, Description)
VALUES (@EventId, 'https://example.com/ai-summit-tickets', 'Get Tickets Here');

-- Insert a gallery for the event
INSERT INTO dbo.Galleries (EventId, Title, CreatedAt)
VALUES (@EventId, 'AI Summit 2024 - Speakers', GETDATE());

SET @GalleryId = SCOPE_IDENTITY();

-- Update Event with Gallery ID
UPDATE dbo.Events
SET GalleryId = @GalleryId
WHERE Id = @EventId;

-- Insert photos into the gallery
INSERT INTO dbo.PhotoGallery (GalleryId, ImageUrl, Caption, CreatedAt)
VALUES (@GalleryId, 'https://example.com/images/ai-speaker-1.jpg', 'Keynote Speaker 1', GETDATE()),
       (@GalleryId, 'https://example.com/images/ai-speaker-2.jpg', 'Keynote Speaker 2', GETDATE());

GO
