USE [Vitinerario]
GO

/****** Object:  Table [dbo].[AIVideo]    Script Date: 10/04/2024 16:56:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AIVideo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Dir_Path] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[Url_Video] [nvarchar](max) NULL,
	[Play_Priority] [int] NULL,
	[IsLandscape] [bit] NULL,
	[DataCreation] [datetime] NOT NULL,
	[IsDeleted] [bit] NULL,
	[DataUpdate] [datetime] NULL,
	[ID_Session] [int] NOT NULL,
 CONSTRAINT [PK__AIVideo__3214EC07F14B5F19] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AIVideo]  WITH CHECK ADD  CONSTRAINT [FK_AIVideo_AdSession] FOREIGN KEY([ID_Session])
REFERENCES [dbo].[AdSession] ([Id])
GO

ALTER TABLE [dbo].[AIVideo] CHECK CONSTRAINT [FK_AIVideo_AdSession]
GO


