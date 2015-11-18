CREATE TABLE [dbo].[Task](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](20) NOT NULL,
	[description] [nvarchar](4000) NOT NULL,
	[priority] [int] NOT NULL,
	[estimated_workhours] [int] NOT NULL,
	[max_developers] [int] NULL,
	[project_id] [int] NOT NULL,
	[state] [int] NOT NULL,
 CONSTRAINT [PK_TASK] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [task_fk0] FOREIGN KEY([project_id])
REFERENCES [dbo].[Project] ([id])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [task_fk0]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [task_fk1] FOREIGN KEY([state])
REFERENCES [dbo].[TaskState] ([id])
GO

ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [task_fk1]