USE [master]
GO

CREATE DATABASE [BookStore];
GO
USE [BookStore]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Books](
	[ISBN] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Genre] [nvarchar](50) NOT NULL,
	[Author] [nvarchar](255) NOT NULL,
	[PublishedOn] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ISBN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [master]
GO
ALTER DATABASE [BookStore] SET  READ_WRITE 
GO
USE [BookStore]

GO
INSERT [dbo].[Books] ([ISBN], [Title], [Genre], [Author], [PublishedOn]) VALUES (N'6a35ea24-517f-48b5-8583-417ac8769aa3', N'Title 1', N'Genre 1', N'Author 1', CAST(N'2010-07-21' AS Date))
GO
INSERT [dbo].[Books] ([ISBN], [Title], [Genre], [Author], [PublishedOn]) VALUES (N'8a5b76e7-d50d-476b-b238-505162e1ed53', N'Title 2', N'Genre 2', N'Author 2', CAST(N'2012-02-11' AS Date))
GO
INSERT [dbo].[Books] ([ISBN], [Title], [Genre], [Author], [PublishedOn]) VALUES (N'23ef84ec-f0b0-4e5c-bce6-bbeb67169ce6', N'Title 3', N'Genre 3', N'Author 3', CAST(N'2020-03-12' AS Date))
GO
INSERT [dbo].[Books] ([ISBN], [Title], [Genre], [Author], [PublishedOn]) VALUES (N'581bd051-fa55-4f07-86ea-bcd6a7f8cfb4', N'Title 4', N'Genre 4', N'Author 4', CAST(N'2009-02-14' AS Date))
GO
INSERT [dbo].[Books] ([ISBN], [Title], [Genre], [Author], [PublishedOn]) VALUES (N'b8d9e4c4-c613-4de0-9afd-c657cb6357e0', N'Title 5', N'Genre 5', N'Author 5', CAST(N'2013-05-10' AS Date))
GO
INSERT [dbo].[Books] ([ISBN], [Title], [Genre], [Author], [PublishedOn]) VALUES (N'e42e1d92-9972-4061-9a35-ecdb47784d09', N'Title 6', N'Genre 6', N'Author 6', CAST(N'2015-01-20' AS Date))
GO
INSERT [dbo].[Books] ([ISBN], [Title], [Genre], [Author], [PublishedOn]) VALUES (N'cf7c3e46-b1ce-407d-8afe-faea3d1230bf', N'Title 7', N'Genre 7', N'Author 7', CAST(N'2017-06-22' AS Date))
GO

CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Application] [nvarchar](50) NOT NULL,
	[Logged] [datetime] NOT NULL,
	[Level] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Logger] [nvarchar](250) NULL,
	[Callsite] [nvarchar](max) NULL,
	[Exception] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

GO