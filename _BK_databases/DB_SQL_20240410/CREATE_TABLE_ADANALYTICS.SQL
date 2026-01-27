USE [Vitinerario]
GO

/****** Object:  Table [dbo].[AdAnalytics]    Script Date: 10/04/2024 16:54:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AdAnalytics](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ID_Session] [int] NOT NULL,
	[ID_Video] [int] NOT NULL,
	[numViews] [int] NULL,
	[numClick] [int] NULL,
	[platform] [nvarchar](50) NULL,
 CONSTRAINT [PK_AdAnalytics] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AdAnalytics] ADD  CONSTRAINT [DF_AdAnalytics_numViews]  DEFAULT ((0)) FOR [numViews]
GO

ALTER TABLE [dbo].[AdAnalytics] ADD  CONSTRAINT [DF_AdAnalytics_numClick]  DEFAULT ((0)) FOR [numClick]
GO

ALTER TABLE [dbo].[AdAnalytics]  WITH CHECK ADD  CONSTRAINT [FK_AdAnalytics_AdSession] FOREIGN KEY([ID_Session])
REFERENCES [dbo].[AdSession] ([Id])
GO

ALTER TABLE [dbo].[AdAnalytics] CHECK CONSTRAINT [FK_AdAnalytics_AdSession]
GO

ALTER TABLE [dbo].[AdAnalytics]  WITH CHECK ADD  CONSTRAINT [FK_AdAnalytics_AIVideo] FOREIGN KEY([ID_Video])
REFERENCES [dbo].[AIVideo] ([Id])
GO

ALTER TABLE [dbo].[AdAnalytics] CHECK CONSTRAINT [FK_AdAnalytics_AIVideo]
GO


