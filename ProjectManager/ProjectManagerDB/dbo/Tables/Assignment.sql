CREATE TABLE [dbo].[Assignment](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[task_id] [int] NOT NULL,
	[accepted] [bit] NOT NULL,
 CONSTRAINT [PK_ASSIGNMENT] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Assignment]  WITH CHECK ADD  CONSTRAINT [assignment_fk1] FOREIGN KEY([task_id])
REFERENCES [dbo].[Task] ([id])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Assignment] CHECK CONSTRAINT [assignment_fk1]
GO
ALTER TABLE [dbo].[Assignment]  WITH CHECK ADD  CONSTRAINT [assignment_fk2] FOREIGN KEY([user_id])
REFERENCES [dbo].[ProjectUser] ([id])
GO

ALTER TABLE [dbo].[Assignment] CHECK CONSTRAINT [assignment_fk2]