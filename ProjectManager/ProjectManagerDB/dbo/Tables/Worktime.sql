CREATE TABLE [dbo].[Worktime](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUserId] [int] NOT NULL,
	[TaskId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
 CONSTRAINT [PK_WORKTIME] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Worktime]  WITH CHECK ADD  CONSTRAINT [FK_worktime_user] FOREIGN KEY([ProjectUserId])
REFERENCES [dbo].[ProjectUser] ([Id])
GO

ALTER TABLE [dbo].[Worktime] CHECK CONSTRAINT [FK_worktime_user]
GO
ALTER TABLE [dbo].[Worktime]  WITH CHECK ADD  CONSTRAINT [worktime_fk1] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Task] ([Id])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Worktime] CHECK CONSTRAINT [worktime_fk1]