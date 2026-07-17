USE [Savi]
GO
INSERT [dbo].[Shifts] ([Id], [Practice], [Date], [StartTime], [EndTime], [HourlyRate], [Role], [Location], [Completed]) VALUES (N'355fe12a-fc60-448a-8179-27bf3c4a56e1', 1, CAST(N'2026-07-16' AS Date), CAST(N'09:00:00' AS Time), CAST(N'17:00:00' AS Time), 20, 2, N'string', 1)
GO
INSERT [dbo].[Shifts] ([Id], [Practice], [Date], [StartTime], [EndTime], [HourlyRate], [Role], [Location], [Completed]) VALUES (N'0188b93b-7aa3-49d5-98ad-5318db623512', 1, CAST(N'2026-07-15' AS Date), CAST(N'09:00:00' AS Time), CAST(N'17:00:00' AS Time), 0, 2, N'string', 1)
GO
INSERT [dbo].[Shifts] ([Id], [Practice], [Date], [StartTime], [EndTime], [HourlyRate], [Role], [Location], [Completed]) VALUES (N'2edb6119-4f20-46bb-90ab-a85128ab42ea', 1, CAST(N'2026-07-15' AS Date), CAST(N'09:00:00' AS Time), CAST(N'17:00:00' AS Time), 0, 2, N'string', 0)
GO
INSERT [dbo].[Shifts] ([Id], [Practice], [Date], [StartTime], [EndTime], [HourlyRate], [Role], [Location], [Completed]) VALUES (N'e897ba5d-44ff-4e0c-8eb2-d53bce2554d2', 4, CAST(N'2026-07-17' AS Date), CAST(N'09:00:00' AS Time), CAST(N'17:00:00' AS Time), 10, 2, N'string', 1)
GO
INSERT [dbo].[Shifts] ([Id], [Practice], [Date], [StartTime], [EndTime], [HourlyRate], [Role], [Location], [Completed]) VALUES (N'e293572a-5d88-4d78-aa80-ece990046cf1', 1, CAST(N'2026-07-17' AS Date), CAST(N'09:00:00' AS Time), CAST(N'17:00:00' AS Time), 15, 2, N'string', 1)
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash], [PasswordSalt], [Role], [Practice]) VALUES (N'5631956a-7a2c-4862-a483-0a5c7c9f2831', N'CateCavity', N'qyUcxkApPGSODJkpRTFpzhlIY65IwVoGM1VkW35+rg0=', N'Lc1PZhom8P5bTCHCZ2vbfQ==', 2, 5)
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash], [PasswordSalt], [Role], [Practice]) VALUES (N'815ef3b4-ca84-4f18-be71-24b0172dd894', N'RootCanal', N'6BPOyjjOzTiWH3b6V7o/ARBBh/zfvDnt3GrMHDzcnXY=', N'hTJY0ytyzMi1YlFFoqOQIg==', 2, 0)
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash], [PasswordSalt], [Role], [Practice]) VALUES (N'c9972fdf-3211-42c2-939c-4690aa5621e1', N'Joker', N'o97UbfUMvAtx5oRm9lt/qmTQTg637sFtO1n/4Wm1XeM=', N'EHfGtqBDa16xNJTyqVFrQw==', 1, 1)
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash], [PasswordSalt], [Role], [Practice]) VALUES (N'f06f5808-bbd2-49c6-8496-a9bd5ec9e6d7', N'BuckTooth', N'bW4hUsEoJ4jRMocWk4jrftorgfKSO89rUHfsqQNrzAY=', N'sUTtj+cSSdVSkyk+SWHBJA==', 1, 4)
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash], [PasswordSalt], [Role], [Practice]) VALUES (N'fc15913f-2448-44f6-bb75-b188a7c37a51', N'DrDrill', N'8K/Slr/1mCGWbZKO0X34EEr4+teqQ+1dOlg/SxBBzDY=', N'H01hU513nSyCFw6mhSnTsA==', 2, 5)
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash], [PasswordSalt], [Role], [Practice]) VALUES (N'68a84c6b-889b-40f9-ba7e-df4dd1d4e710', N'admin', N'rOCh82d/TlENeb2mIjYM2wK0I64mH319EDCfL4vOSbk=', N'rIP+Zx9YZpJ8AHEsih8s1Q==', 0, NULL)
GO
INSERT [dbo].[Timesheets] ([Id], [StartTime], [EndTime], [UnpaidBreakMinutes], [Notes], [BusinessReference], [ShiftId], [UserId]) VALUES (N'940ca1e4-ca9d-485b-b852-4f9e6d6187d3', CAST(N'09:00:00' AS Time), CAST(N'16:00:00' AS Time), 30, N'', N'Duplicate timesheet ignored: DrDrill, 09:00 - 16:00, break: 30', N'e897ba5d-44ff-4e0c-8eb2-d53bce2554d2', N'fc15913f-2448-44f6-bb75-b188a7c37a51')
GO
INSERT [dbo].[Timesheets] ([Id], [StartTime], [EndTime], [UnpaidBreakMinutes], [Notes], [BusinessReference], [ShiftId], [UserId]) VALUES (N'7c3377da-044d-4317-8427-5340db64612c', CAST(N'09:00:00' AS Time), CAST(N'17:00:00' AS Time), 60, N'', NULL, N'355fe12a-fc60-448a-8179-27bf3c4a56e1', N'fc15913f-2448-44f6-bb75-b188a7c37a51')
GO
INSERT [dbo].[Timesheets] ([Id], [StartTime], [EndTime], [UnpaidBreakMinutes], [Notes], [BusinessReference], [ShiftId], [UserId]) VALUES (N'8a8161f8-776d-4c0b-8118-5d85eb5304d2', CAST(N'09:00:00' AS Time), CAST(N'17:00:00' AS Time), 10, N'', NULL, N'0188b93b-7aa3-49d5-98ad-5318db623512', N'815ef3b4-ca84-4f18-be71-24b0172dd894')
GO
INSERT [dbo].[Timesheets] ([Id], [StartTime], [EndTime], [UnpaidBreakMinutes], [Notes], [BusinessReference], [ShiftId], [UserId]) VALUES (N'3acd308b-c0b0-466d-9621-aaaa733af5f3', CAST(N'09:00:00' AS Time), CAST(N'17:00:00' AS Time), 25, N'', NULL, N'e293572a-5d88-4d78-aa80-ece990046cf1', N'fc15913f-2448-44f6-bb75-b188a7c37a51')
GO
