USE [VitinerarioTest]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-----------------------------------------------------------------------------
-- 1. sp_CreaGalleryEInserisciFoto (FORNITA DALL'UTENTE)
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaGalleryEInserisciFoto]
    @EventId INT,
    @GalleryTitle NVARCHAR(255),
    @LangID INT,
    @NumeroImmagini INT,
    @LinkBaseUgualePerTutte NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @NuovaGalleryId INT;
    DECLARE @LinkBasePulito NVARCHAR(500);

    BEGIN TRY
        IF @EventId IS NULL OR @EventId <= 0
            THROW 50001, 'EventId non valido.', 1;

        IF @GalleryTitle IS NULL OR LTRIM(RTRIM(@GalleryTitle)) = ''
            THROW 50002, 'GalleryTitle non può essere vuoto.', 1;

        IF @LangID IS NULL
            THROW 50003, 'LangID non può essere NULL.', 1;

        IF @NumeroImmagini IS NULL OR @NumeroImmagini <= 0
            THROW 50004, 'NumeroImmagini deve essere maggiore di 0.', 1;

        IF @LinkBaseUgualePerTutte IS NULL OR LTRIM(RTRIM(@LinkBaseUgualePerTutte)) = ''
            THROW 50005, 'LinkBaseUgualePerTutte non può essere vuoto.', 1;

        SET @LinkBasePulito = RTRIM(LTRIM(@LinkBaseUgualePerTutte));

        IF RIGHT(@LinkBasePulito, 1) = '/'
            SET @LinkBasePulito = LEFT(@LinkBasePulito, LEN(@LinkBasePulito) - 1);

        IF NOT EXISTS
        (
            SELECT 1
            FROM [dbo].[Events]
            WHERE [Id] = @EventId
        )
            THROW 50006, 'Evento non trovato nella tabella Events.', 1;

        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Galleries]
        (
            [EventId],
            [Title],
            [CreatedAt],
            [LangID]
        )
        VALUES
        (
            @EventId,
            @GalleryTitle,
            GETDATE(),
            @LangID
        );

        SET @NuovaGalleryId = SCOPE_IDENTITY();

        ;WITH Numeri AS
        (
            SELECT TOP (@NumeroImmagini)
                ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Num
            FROM sys.all_objects
        )
        INSERT INTO [dbo].[PhotoGalleries]
        (
            [GalleryId],
            [ImageUrl],
            [Caption],
            [CreatedAt],
            [LangID]
        )
        SELECT
            @NuovaGalleryId,
            CONCAT(@LinkBasePulito, '/', Num, '.png'),
            CONCAT(N'Immagine ', Num),
            GETDATE(),
            @LangID
        FROM Numeri;

        COMMIT TRANSACTION;

        SELECT
            @NuovaGalleryId AS GalleryIdCreata,
            @EventId AS EventId,
            @NumeroImmagini AS NumeroImmaginiInserite,
            @LinkBasePulito AS LinkBaseUsato;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 2. sp_CreaContentImages (FORNITA DALL'UTENTE)
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaContentImages]
    @NArt INT,
    @ContentID_IT INT,
    @ContentID_EN INT,
    @NumeroImmagini INT,
    @BaseUrl NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @BaseUrlPulito NVARCHAR(500);

    BEGIN TRY
        IF @NArt IS NULL OR @NArt <= 0
            THROW 51001, 'NArt non valido.', 1;

        IF @ContentID_IT IS NULL OR @ContentID_IT <= 0
            THROW 51002, 'ContentID_IT non valido.', 1;

        IF @ContentID_EN IS NULL OR @ContentID_EN <= 0
            THROW 51003, 'ContentID_EN non valido.', 1;

        IF @NumeroImmagini IS NULL OR @NumeroImmagini <= 0
            THROW 51004, 'NumeroImmagini deve essere maggiore di 0.', 1;

        IF @BaseUrl IS NULL OR LTRIM(RTRIM(@BaseUrl)) = ''
            THROW 51005, 'BaseUrl non può essere vuoto.', 1;

        SET @BaseUrlPulito = LTRIM(RTRIM(@BaseUrl));

        IF RIGHT(@BaseUrlPulito, 1) = '/'
            SET @BaseUrlPulito = LEFT(@BaseUrlPulito, LEN(@BaseUrlPulito) - 1);

        IF NOT EXISTS (SELECT 1 FROM [dbo].[Contents] WHERE [Id] = @ContentID_IT)
            THROW 51006, 'ContentID_IT non trovato nella tabella Contents.', 1;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[Contents] WHERE [Id] = @ContentID_EN)
            THROW 51007, 'ContentID_EN non trovato nella tabella Contents.', 1;

        BEGIN TRANSACTION;

        ;WITH Numeri AS
        (
            SELECT TOP (@NumeroImmagini)
                ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Num
            FROM sys.all_objects
        )
        INSERT INTO [dbo].[ContentImages]
        (
            [ContentId],
            [ImageUrl],
            [Caption],
            [Position],
            [CreatedAt],
            [LangID]
        )
        SELECT
            @ContentID_IT AS ContentId,
            CONCAT(@BaseUrlPulito, '/art', @NArt, '_photo', Num, '.png') AS ImageUrl,
            NULL AS Caption,
            Num AS Position,
            GETDATE() AS CreatedAt,
            2 AS LangID -- Assumendo LangID 2 = IT
        FROM Numeri

        UNION ALL

        SELECT
            @ContentID_EN AS ContentId,
            CONCAT(@BaseUrlPulito, '/art', @NArt, '_photo', Num, '.png') AS ImageUrl,
            CONCAT(N'Articolo ', @NArt, N' - Immagine ', Num) AS Caption,
            Num AS Position,
            GETDATE() AS CreatedAt,
            1 AS LangID -- Assumendo LangID 1 = EN
        FROM Numeri;

        COMMIT TRANSACTION;

        SELECT
            @NArt AS NumeroArticolo,
            @ContentID_IT AS ContentIdIT,
            @ContentID_EN AS ContentIdEN,
            @NumeroImmagini AS NumeroImmaginiPerLingua,
            (@NumeroImmagini * 2) AS TotaleRecordInseriti,
            @BaseUrlPulito AS BaseUrlUsato;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 3. sp_CreaContent
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaContent]
    @Title NVARCHAR(255),
    @Text NVARCHAR(MAX),
    @PublishDate DATETIME2,
    @CoverImage NVARCHAR(500) = NULL,
    @ContentType NVARCHAR(20),
    @IsPublished BIT = 1,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[Contents]
        ([Title], [Text], [PublishDate], [CoverImage], [ContentType], [IsPublished], [CreatedAt], [UpdatedAt], [LangID])
        VALUES
        (@Title, @Text, @PublishDate, @CoverImage, @ContentType, @IsPublished, GETDATE(), GETDATE(), @LangID);

        SELECT SCOPE_IDENTITY() AS ContentId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 4. sp_CreaEvent
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaEvent]
    @Title NVARCHAR(255),
    @Description NVARCHAR(MAX),
    @EventDate DATETIME2,
    @CoverImage NVARCHAR(500) = NULL,
    @BookingEndDate DATETIME2 = NULL,
    @Location NVARCHAR(500) = NULL,
    @Organizer NVARCHAR(255) = NULL,
    @ContactInfo NVARCHAR(255) = NULL,
    @Price DECIMAL(18,2) = 0,
    @IsOnline BIT = 0,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[Events]
        (
            [Title], [Description], [EventDate], [CoverImage], [CreatedAt],
            [BookingEndDate], [Location], [Organizer], [ContactInfo], [Price], [IsOnline], [LangID]
        )
        VALUES
        (
            @Title, @Description, @EventDate, @CoverImage, GETDATE(),
            @BookingEndDate, @Location, @Organizer, @ContactInfo, @Price, @IsOnline, @LangID
        );

        SELECT SCOPE_IDENTITY() AS EventId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 5. sp_CreaExpert
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaExpert]
    @Name NVARCHAR(255),
    @Bio NVARCHAR(MAX) = NULL,
    @PhotoUrl NVARCHAR(500) = NULL,
    @Email NVARCHAR(255) = NULL,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[Experts]
        ([Name], [Bio], [PhotoUrl], [Email], [CreatedAt], [LangID])
        VALUES
        (@Name, @Bio, @PhotoUrl, @Email, GETDATE(), @LangID);

        SELECT SCOPE_IDENTITY() AS ExpertId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 6. sp_CollegaExpertAContent
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CollegaExpertAContent]
    @ContentId INT,
    @ExpertId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [dbo].[ContentExperts] WHERE [ContentId] = @ContentId AND [ExpertId] = @ExpertId)
        BEGIN
            INSERT INTO [dbo].[ContentExperts] ([ContentId], [ExpertId])
            VALUES (@ContentId, @ExpertId);
            SELECT SCOPE_IDENTITY() AS ContentExpertId;
        END
        ELSE
        BEGIN
            SELECT 0 AS ContentExpertId; -- Already exists
        END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 7. sp_CollegaExpertAEvent
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CollegaExpertAEvent]
    @EventId INT,
    @ExpertId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [dbo].[EventExperts] WHERE [EventId] = @EventId AND [ExpertId] = @ExpertId)
        BEGIN
            INSERT INTO [dbo].[EventExperts] ([EventId], [ExpertId])
            VALUES (@EventId, @ExpertId);
            SELECT SCOPE_IDENTITY() AS EventExpertId;
        END
        ELSE
        BEGIN
            SELECT 0 AS EventExpertId; -- Already exists
        END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 8. sp_CreaPodcast
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaPodcast]
    @Title NVARCHAR(255),
    @Description NVARCHAR(MAX),
    @PublishDate DATETIME2,
    @CoverImage NVARCHAR(500) = NULL,
    @YoutubeUrl NVARCHAR(500) = NULL,
    @SpotifyUrl NVARCHAR(500) = NULL,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[Podcasts]
        ([Title], [Description], [PublishDate], [CoverImage], [YoutubeUrl], [SpotifyUrl], [CreatedAt], [LangID])
        VALUES
        (@Title, @Description, @PublishDate, @CoverImage, @YoutubeUrl, @SpotifyUrl, GETDATE(), @LangID);

        SELECT SCOPE_IDENTITY() AS PodcastId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 9. sp_CreaPartner
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaPartner]
    @Description NVARCHAR(MAX) = NULL,
    @LinkUrl NVARCHAR(500) = NULL,
    @ImageUrl NVARCHAR(500) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[Partners]
        ([Description], [LinkUrl], [ImageUrl], [IsActive])
        VALUES
        (@Description, @LinkUrl, @ImageUrl, @IsActive);

        SELECT SCOPE_IDENTITY() AS PartnerId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
