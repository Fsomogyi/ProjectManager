CREATE TABLE [dbo].[Comment](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[task_id] [int] NOT NULL,
	[content] [nvarchar](4000) NOT NULL,
	[timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_COMMENT] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [comment_fk1] FOREIGN KEY([task_id])
REFERENCES [dbo].[Task] ([id])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Comment] CHECK CONSTRAINT [comment_fk1]
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [comment_fk2] FOREIGN KEY([user_id])
REFERENCES [dbo].[ProjectUser] ([id])
GO

ALTER TABLE [dbo].[Comment] CHECK CONSTRAINT [comment_fk2]