-- Migration M_260326: Add Trips, Stays, ItineraryDays, ItineraryStops, TripMusts

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Trips')
BEGIN
    CREATE TABLE [dbo].[Trips](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [EventId] [int] NOT NULL,
        [DepartureCity] [nvarchar](255) NOT NULL,
        [DepartureCountry] [nvarchar](255) NOT NULL,
        [ArrivalCity] [nvarchar](255) NOT NULL,
        [ArrivalCountry] [nvarchar](255) NULL,
        [DurationDays] [int] NOT NULL,
        [DurationNights] [int] NOT NULL,
        [MaxGuests] [int] NOT NULL,
        [Status] [nvarchar](50) NOT NULL, -- done, cancelled, booking, started, in_progress

        -- Audit fields
        [CreationTime] [datetime2](7) NULL,
        [Creation_User] [int] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModification_User] [int] NULL,
        [IsDeleted] [bit] NULL DEFAULT 0,
        [DeletionTime] [datetime2](7) NULL,
        [Deletion_User] [int] NULL,

        CONSTRAINT [PK_Trips] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Trips_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events] ([Id])
    )
    CREATE INDEX [IX_Trips_EventId] ON [dbo].[Trips] ([EventId])
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ItineraryDays')
BEGIN
    CREATE TABLE [dbo].[ItineraryDays](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [TripId] [int] NOT NULL,
        [DayNumber] [int] NOT NULL,
        [Title] [nvarchar](255) NOT NULL,
        [Description] [nvarchar](max) NULL,

        -- Audit fields
        [CreationTime] [datetime2](7) NULL,
        [Creation_User] [int] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModification_User] [int] NULL,
        [IsDeleted] [bit] NULL DEFAULT 0,
        [DeletionTime] [datetime2](7) NULL,
        [Deletion_User] [int] NULL,

        CONSTRAINT [PK_ItineraryDays] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ItineraryDays_Trips] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips] ([Id])
    )
    CREATE INDEX [IX_ItineraryDays_TripId] ON [dbo].[ItineraryDays] ([TripId])
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stays')
BEGIN
    CREATE TABLE [dbo].[Stays](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [TripId] [int] NOT NULL,
        [Name] [nvarchar](255) NOT NULL,
        [Description] [nvarchar](max) NULL,
        [Image] [nvarchar](500) NULL,
        [Location] [nvarchar](500) NULL,
        [OrderIndex] [int] NOT NULL DEFAULT 0,
        [ItineraryDayId] [int] NULL,

        -- Audit fields
        [CreationTime] [datetime2](7) NULL,
        [Creation_User] [int] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModification_User] [int] NULL,
        [IsDeleted] [bit] NULL DEFAULT 0,
        [DeletionTime] [datetime2](7) NULL,
        [Deletion_User] [int] NULL,

        CONSTRAINT [PK_Stays] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Stays_Trips] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips] ([Id]),
        CONSTRAINT [FK_Stays_ItineraryDays] FOREIGN KEY ([ItineraryDayId]) REFERENCES [dbo].[ItineraryDays] ([Id])
    )
    CREATE INDEX [IX_Stays_TripId] ON [dbo].[Stays] ([TripId])
    CREATE INDEX [IX_Stays_ItineraryDayId] ON [dbo].[Stays] ([ItineraryDayId])
    CREATE INDEX [IX_Stays_OrderIndex] ON [dbo].[Stays] ([OrderIndex])
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ItineraryStops')
BEGIN
    CREATE TABLE [dbo].[ItineraryStops](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [DayId] [int] NOT NULL,
        [Time] [time](7) NULL,
        [Title] [nvarchar](255) NOT NULL,
        [Description] [nvarchar](max) NULL,
        [Type] [nvarchar](50) NOT NULL, -- activity, meal, transfer, experience
        [OrderIndex] [int] NOT NULL DEFAULT 0,

        -- Audit fields
        [CreationTime] [datetime2](7) NULL,
        [Creation_User] [int] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModification_User] [int] NULL,
        [IsDeleted] [bit] NULL DEFAULT 0,
        [DeletionTime] [datetime2](7) NULL,
        [Deletion_User] [int] NULL,

        CONSTRAINT [PK_ItineraryStops] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ItineraryStops_ItineraryDays] FOREIGN KEY ([DayId]) REFERENCES [dbo].[ItineraryDays] ([Id])
    )
    CREATE INDEX [IX_ItineraryStops_DayId] ON [dbo].[ItineraryStops] ([DayId])
    CREATE INDEX [IX_ItineraryStops_OrderIndex] ON [dbo].[ItineraryStops] ([OrderIndex])
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TripMusts')
BEGIN
    CREATE TABLE [dbo].[TripMusts](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [TripId] [int] NOT NULL,
        [Text] [nvarchar](max) NOT NULL,
        [TypeId] [int] NOT NULL, -- 1: Inclusion, 2: Exclusion (o altro valore secondo architettura)

        -- Audit fields
        [CreationTime] [datetime2](7) NULL,
        [Creation_User] [int] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModification_User] [int] NULL,
        [IsDeleted] [bit] NULL DEFAULT 0,
        [DeletionTime] [datetime2](7) NULL,
        [Deletion_User] [int] NULL,

        CONSTRAINT [PK_TripMusts] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_TripMusts_Trips] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips] ([Id])
    )
    CREATE INDEX [IX_TripMusts_TripId] ON [dbo].[TripMusts] ([TripId])
END
GO
