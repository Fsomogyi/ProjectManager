CREATE TABLE [dbo].[TaskStateChange](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[task_id] [int] NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[state] [int] NOT NULL,
	[reason] [nvarchar](4000) NULL,
	[accepted] [bit] NOT NULL,
 CONSTRAINT [PK_TASK_STATE_CHANGE] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TaskStateChange]  WITH CHECK ADD  CONSTRAINT [FK_task_state_change_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[ProjectUser] ([id])
GO

ALTER TABLE [dbo].[TaskStateChange] CHECK CONSTRAINT [FK_task_state_change_user]
GO
ALTER TABLE [dbo].[TaskStateChange]  WITH CHECK ADD  CONSTRAINT [task_state_change_fk1] FOREIGN KEY([task_id])
REFERENCES [dbo].[Task] ([id])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[TaskStateChange] CHECK CONSTRAINT [task_state_change_fk1]
GO
ALTER TABLE [dbo].[TaskStateChange]  WITH CHECK ADD  CONSTRAINT [task_state_change_fk2] FOREIGN KEY([state])
REFERENCES [dbo].[TaskState] ([id])
GO

ALTER TABLE [dbo].[TaskStateChange] CHECK CONSTRAINT [task_state_change_fk2]