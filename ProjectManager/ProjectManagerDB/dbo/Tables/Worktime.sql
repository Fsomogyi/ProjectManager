CREATE TABLE [dbo].[Worktime](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[task_id] [int] NOT NULL,
	[start_time] [datetime] NOT NULL,
	[end_time] [datetime] NOT NULL,
 CONSTRAINT [PK_WORKTIME] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Worktime]  WITH CHECK ADD  CONSTRAINT [FK_worktime_user] FOREIGN KEY([user_id])
REFERENCES [dbo].[ProjectUser] ([id])
GO

ALTER TABLE [dbo].[Worktime] CHECK CONSTRAINT [FK_worktime_user]
GO
ALTER TABLE [dbo].[Worktime]  WITH CHECK ADD  CONSTRAINT [worktime_fk1] FOREIGN KEY([task_id])
REFERENCES [dbo].[Task] ([id])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Worktime] CHECK CONSTRAINT [worktime_fk1]