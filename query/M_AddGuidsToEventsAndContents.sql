-- Migration: Add Guid column to Events and Contents
-- Date: 2024-03-27

-- Update Events
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[Events]') AND name = 'Guid')
BEGIN
    ALTER TABLE [dbo].[Events] ADD [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID());
END
GO

-- Update Contents
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[Contents]') AND name = 'Guid')
BEGIN
    ALTER TABLE [dbo].[Contents] ADD [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID());
END
GO

-- Ensure existing records have unique GUIDs (the DEFAULT constraint handles new ones)
-- and add unique constraint if not already present via the column definition
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Events_Guid' AND object_id = OBJECT_ID('[dbo].[Events]'))
BEGIN
    ALTER TABLE [dbo].[Events] ADD CONSTRAINT [UQ_Events_Guid] UNIQUE ([Guid]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Contents_Guid' AND object_id = OBJECT_ID('[dbo].[Contents]'))
BEGIN
    ALTER TABLE [dbo].[Contents] ADD CONSTRAINT [UQ_Contents_Guid] UNIQUE ([Guid]);
END
GO
