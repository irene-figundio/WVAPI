-- Migration: Create StorageMappings and HeroImages tables
-- Date: 2024-03-27

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StorageMappings' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[StorageMappings] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ParentType] NVARCHAR(50) NOT NULL,
        [ParentId] INT NOT NULL,
        [StorageArea] NVARCHAR(50) NOT NULL,
        [ProgressiveNumber] INT NOT NULL,
        [FolderName] NVARCHAR(255) NOT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_StorageMappings] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE INDEX [IX_StorageMappings_Parent] ON [dbo].[StorageMappings] ([ParentType], [ParentId], [StorageArea]);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HeroImages' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[HeroImages] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(255) NOT NULL,
        [ImageUrl] NVARCHAR(500) NOT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE()),
        [IsDeleted] BIT DEFAULT 0,
        [DeletionDate] DATETIME NULL,
        CONSTRAINT [PK_HeroImages] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO
