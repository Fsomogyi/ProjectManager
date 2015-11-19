CREATE TABLE [dbo].[TaskStateChange](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[TaskId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[TaskState] [int] NOT NULL,
	[Reason] [nvarchar](4000) NULL,
	[Accepted] [bit] NOT NULL,
 CONSTRAINT [PK_TASK_STATE_CHANGE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TaskStateChange]  WITH CHECK ADD  CONSTRAINT [FK_task_state_change_user] FOREIGN KEY([UserId])
REFERENCES [dbo].[ProjectUser] ([Id])
GO

ALTER TABLE [dbo].[TaskStateChange] CHECK CONSTRAINT [FK_task_state_change_user]
GO
ALTER TABLE [dbo].[TaskStateChange]  WITH CHECK ADD  CONSTRAINT [task_state_change_fk1] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Task] ([Id])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[TaskStateChange] CHECK CONSTRAINT [task_state_change_fk1]
GO
ALTER TABLE [dbo].[TaskStateChange]  WITH CHECK ADD  CONSTRAINT [task_state_change_fk2] FOREIGN KEY([TaskState])
REFERENCES [dbo].[TaskState] ([Id])
GO

ALTER TABLE [dbo].[TaskStateChange] CHECK CONSTRAINT [task_state_change_fk2]