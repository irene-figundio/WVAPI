USE [Vitinerario]
GO

/****** Object:  Table [dbo].[UploadedFiles]    Script Date: 16/04/2025 12:19:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UploadedFiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FilePath] [nvarchar](500) NOT NULL,
	[FileType] [nvarchar](100) NULL,
	[FileSize] [bigint] NOT NULL,
	[OpenAIFileId] [nvarchar](100) NOT NULL,
	[UploadDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UploadedFiles] ADD  DEFAULT (getdate()) FOR [UploadDate]
GO


