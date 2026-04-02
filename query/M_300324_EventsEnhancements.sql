-- Add new fields to Events table
ALTER TABLE [dbo].[Events] ADD [HasVariantPrice] BIT NOT NULL DEFAULT 1;
ALTER TABLE [dbo].[Events] ADD [HasNeeds] BIT NOT NULL DEFAULT 1;
ALTER TABLE [dbo].[Events] ADD [ProgramPdf] NVARCHAR(500) NULL;

-- Create VariantPrices table
CREATE TABLE [dbo].[VariantPrices] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [EventId] INT NOT NULL,
    [Price] DECIMAL(18, 2) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsDeleted] BIT DEFAULT 0 NULL,
    [DeletionDate] DATETIME NULL,
    CONSTRAINT [PK_VariantPrices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VariantPrices_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events] ([Id]) ON DELETE CASCADE
);

-- Create EventNeeds table
CREATE TABLE [dbo].[EventNeeds] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [EventId] INT NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [IsDeleted] BIT DEFAULT 0 NULL,
    [DeletionDate] DATETIME NULL,
    CONSTRAINT [PK_EventNeeds] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventNeeds_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events] ([Id]) ON DELETE CASCADE
);
