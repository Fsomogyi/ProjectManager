CREATE TABLE [dbo].[Assignment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUserId] [int] NOT NULL,
	[TaskId] [int] NOT NULL,
	[Accepted] [bit] NOT NULL,
 CONSTRAINT [PK_ASSIGNMENT] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Assignment]  WITH CHECK ADD  CONSTRAINT [assignment_fk1] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Task] ([Id])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Assignment] CHECK CONSTRAINT [assignment_fk1]
GO
ALTER TABLE [dbo].[Assignment]  WITH CHECK ADD  CONSTRAINT [assignment_fk2] FOREIGN KEY([ProjectUserId])
REFERENCES [dbo].[ProjectUser] ([Id])
GO

ALTER TABLE [dbo].[Assignment] CHECK CONSTRAINT [assignment_fk2]