USE [Vitinerario]
GO

/****** Object:  Table [dbo].[WebAPILog]    Script Date: 10/04/2024 16:58:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WebAPILog](
	[IDLog] [int] IDENTITY(1,1) NOT NULL,
	[DateTimeStamp] [datetime] NOT NULL,
	[RequestMethod] [nvarchar](10) NOT NULL,
	[RequestUrl] [nvarchar](255) NULL,
	[RequestBody] [nvarchar](max) NULL,
	[ResponseCode] [int] NOT NULL,
	[ResponseMessage] [nvarchar](255) NULL,
	[ResponseBody] [nvarchar](max) NULL,
	[UserAgent] [nvarchar](255) NULL,
	[AdditionalInfo] [nvarchar](max) NULL,
 CONSTRAINT [PK__WebAPILo__95D002088BF23056] PRIMARY KEY CLUSTERED 
(
	[IDLog] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


