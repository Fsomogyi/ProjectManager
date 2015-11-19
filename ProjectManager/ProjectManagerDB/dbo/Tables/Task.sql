CREATE TABLE [dbo].[Task](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[Priority] [int] NOT NULL,
	[EstimatedWorkHours] [int] NOT NULL,
	[MaxDevelopers] [int] NULL,
	[ProjectId] [int] NOT NULL,
	[State] [int] NOT NULL,
 CONSTRAINT [PK_TASK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [task_fk0] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([Id])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [task_fk0]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [task_fk1] FOREIGN KEY([State])
REFERENCES [dbo].[TaskState] ([Id])
GO

ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [task_fk1]