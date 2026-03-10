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

-----------------------------------------------------------------------------
-- 10. sp_CreaLanguage
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaLanguage]
    @Code NVARCHAR(10),
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[Languages] ([Code], [Name])
        VALUES (@Code, @Name);

        SELECT SCOPE_IDENTITY() AS LanguageId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 11. sp_CreaAIVideo
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaAIVideo]
    @Dir_Path NVARCHAR(MAX) = NULL,
    @Title NVARCHAR(MAX) = NULL,
    @Url_Video NVARCHAR(MAX) = NULL,
    @Play_Priority INT = NULL,
    @IsLandscape BIT = NULL,
    @ID_Session INT,
    @Creation_User INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[AIVideos]
        ([Dir_Path], [Title], [Url_Video], [Play_Priority], [IsLandscape], [ID_Session], [DataCreation], [Creation_User])
        VALUES
        (@Dir_Path, @Title, @Url_Video, @Play_Priority, @IsLandscape, @ID_Session, GETDATE(), @Creation_User);

        SELECT SCOPE_IDENTITY() AS AIVideoId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 12. sp_CreaAPIToken
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaAPIToken]
    @Token NVARCHAR(MAX),
    @Platform NVARCHAR(MAX),
    @DataExpiration DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[APITokens] ([DataCreation], [Token], [Platform], [DataExpiration])
        VALUES (GETDATE(), @Token, @Platform, @DataExpiration);

        SELECT SCOPE_IDENTITY() AS APITokenId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 13. sp_CreaAdAnalytics
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaAdAnalytics]
    @SessionId INT,
    @VideoId INT,
    @NumViews INT = NULL,
    @NumClick INT = NULL,
    @Platform NVARCHAR(MAX),
    @SendDate DATETIME2,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[AdAnalytics]
        ([SessionId], [VideoId], [NumViews], [NumClick], [Platform], [SendDate], [LangID])
        VALUES
        (@SessionId, @VideoId, @NumViews, @NumClick, @Platform, @SendDate, @LangID);

        SELECT SCOPE_IDENTITY() AS AdAnalyticsId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 14. sp_CreaAdCampaign
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaAdCampaign]
    @Name NVARCHAR(MAX),
    @Description NVARCHAR(MAX) = NULL,
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @Budget DECIMAL(18,2) = NULL,
    @Creation_User INT = NULL,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[AdCampaigns]
        ([Name], [Description], [StartDate], [EndDate], [Budget], [CreationTime], [Creation_User], [LangID])
        VALUES
        (@Name, @Description, @StartDate, @EndDate, @Budget, GETDATE(), @Creation_User, @LangID);

        SELECT SCOPE_IDENTITY() AS AdCampaignId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 15. sp_CreaAdSession
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaAdSession]
    @ID_Campaing INT,
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @Creation_User INT = NULL,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[AdSessions]
        ([ID_Campaing], [StartDate], [EndDate], [CreationTime], [Creation_User], [LangID])
        VALUES
        (@ID_Campaing, @StartDate, @EndDate, GETDATE(), @Creation_User, @LangID);

        SELECT SCOPE_IDENTITY() AS AdSessionId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 16. sp_CreaContentLink
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaContentLink]
    @ContentId INT,
    @LinkUrl NVARCHAR(500),
    @Description NVARCHAR(255) = NULL,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[ContentLinks]
        ([ContentId], [LinkUrl], [Description], [CreatedAt], [LangID])
        VALUES
        (@ContentId, @LinkUrl, @Description, GETDATE(), @LangID);

        SELECT SCOPE_IDENTITY() AS ContentLinkId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 17. sp_CreaEventLink
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaEventLink]
    @EventId INT,
    @LinkUrl NVARCHAR(500),
    @Description NVARCHAR(255) = NULL,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[EventLinks]
        ([EventId], [LinkUrl], [Description], [LangID])
        VALUES
        (@EventId, @LinkUrl, @Description, @LangID);

        SELECT SCOPE_IDENTITY() AS EventLinkId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 18. sp_CreaUploadedFile
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaUploadedFile]
    @FileName NVARCHAR(MAX),
    @FilePath NVARCHAR(MAX),
    @FileType NVARCHAR(MAX),
    @FileSize BIGINT,
    @OpenAIFileId NVARCHAR(MAX) = NULL,
    @VectorStoreId NVARCHAR(MAX) = NULL,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[UploadedFiles]
        ([FileName], [FilePath], [FileType], [FileSize], [OpenAIFileId], [VectorStoreId], [UploadDate], [LangID])
        VALUES
        (@FileName, @FilePath, @FileType, @FileSize, @OpenAIFileId, @VectorStoreId, GETDATE(), @LangID);

        SELECT SCOPE_IDENTITY() AS UploadedFileId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 19. sp_CreaUser
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaUser]
    @Username NVARCHAR(MAX),
    @Password NVARCHAR(MAX),
    @Email NVARCHAR(MAX),
    @StatusId INT,
    @SuperAdmin BIT = 0,
    @CreationUserId INT = NULL,
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[Users]
        ([Username], [Password], [Email], [StatusId], [StatusTime], [CreationTime], [CreationUserId], [IsDeleted], [SuperAdmin], [VerificationToken], [LangID])
        VALUES
        (@Username, @Password, @Email, @StatusId, GETDATE(), GETDATE(), @CreationUserId, 0, @SuperAdmin, NEWID(), @LangID);

        SELECT SCOPE_IDENTITY() AS UserId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 20. sp_CreaUserStatus
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaUserStatus]
    @Name NVARCHAR(MAX),
    @ResourceKey NVARCHAR(MAX),
    @LangID INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[UserStatuses] ([Name], [ResourceKey], [LangID])
        VALUES (@Name, @ResourceKey, @LangID);

        SELECT SCOPE_IDENTITY() AS UserStatusId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 21. sp_CreaWebAPILog
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaWebAPILog]
    @RequestMethod NVARCHAR(MAX),
    @RequestUrl NVARCHAR(MAX),
    @RequestBody NVARCHAR(MAX),
    @ResponseBody NVARCHAR(MAX),
    @ResponseCode INT,
    @ResponseMessage NVARCHAR(MAX),
    @UserAgent NVARCHAR(MAX),
    @AdditionalInfo NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[WebAPILogs]
        ([DateTimeStamp], [RequestMethod], [RequestUrl], [RequestBody], [ResponseBody], [ResponseCode], [ResponseMessage], [UserAgent], [AdditionalInfo])
        VALUES
        (GETDATE(), @RequestMethod, @RequestUrl, @RequestBody, @ResponseBody, @ResponseCode, @ResponseMessage, @UserAgent, @AdditionalInfo);

        SELECT SCOPE_IDENTITY() AS WebAPILogId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
-- 22. sp_CreaWineAI
-----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_CreaWineAI]
    @Question NVARCHAR(MAX),
    @Answer NVARCHAR(MAX),
    @Dispositivo NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO [dbo].[WineAIs] ([CreationDate], [Question], [Answer], [Dispositivo])
        VALUES (GETDATE(), @Question, @Answer, @Dispositivo);

        SELECT SCOPE_IDENTITY() AS WineAIId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
