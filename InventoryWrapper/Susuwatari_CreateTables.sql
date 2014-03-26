USE [Inventory]
GO

/****** Object:  Table [dbo].[Movements]    Script Date: 04/12/2013 16:22:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Movements]') AND type in (N'U'))
DROP TABLE [dbo].[Movements]
GO

USE [Inventory]
GO

/****** Object:  Table [dbo].[Movements]    Script Date: 04/12/2013 16:22:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Movements](
	[SourceSZ] [nvarchar](100) NULL,
	[SourceLot] [nvarchar](100) NULL,
	[DestnSZ] [nvarchar](100) NULL,
	[DestnLot] [nvarchar](100) NULL,
	[SourceQuantity] [decimal](18, 1) NULL,
	[DestnQuantity] [decimal](18, 1) NULL,
	[ProdID] [nvarchar](50) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SourceSP] [nvarchar](50) NULL,
	[DestnSP] [nvarchar](50) NULL,
	[SourceM] [nvarchar](50) NULL,
	[DestnM] [nvarchar](50) NULL,
 CONSTRAINT [PK_Movements] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [Inventory]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BalanceBreakdown_BalanceBreakdown]') AND parent_object_id = OBJECT_ID(N'[dbo].[SurveyBreakdown]'))
ALTER TABLE [dbo].[SurveyBreakdown] DROP CONSTRAINT [FK_BalanceBreakdown_BalanceBreakdown]
GO

USE [Inventory]
GO

/****** Object:  Table [dbo].[SurveyBreakdown]    Script Date: 04/12/2013 16:23:20 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SurveyBreakdown]') AND type in (N'U'))
DROP TABLE [dbo].[SurveyBreakdown]
GO

USE [Inventory]
GO

/****** Object:  Table [dbo].[SurveyBreakdown]    Script Date: 04/12/2013 16:23:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SurveyBreakdown](
	[Timestamp] [datetime] NOT NULL,
	[SZ] [nvarchar](100) NOT NULL,
	[SP] [nvarchar](50) NOT NULL,
	[M] [nvarchar](50) NOT NULL,
	[Lot] [nvarchar](100) NOT NULL,
	[Balance] [decimal](18, 1) NULL,
 CONSTRAINT [PK_BalanceBreakdown] PRIMARY KEY CLUSTERED 
(
	[Timestamp] ASC,
	[SZ] ASC,
	[SP] ASC,
	[M] ASC,
	[Lot] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SurveyBreakdown]  WITH CHECK ADD  CONSTRAINT [FK_BalanceBreakdown_BalanceBreakdown] FOREIGN KEY([Timestamp], [SZ], [SP])
REFERENCES [dbo].[Surveys] ([Timestamp], [SZ], [SP])
GO

ALTER TABLE [dbo].[SurveyBreakdown] CHECK CONSTRAINT [FK_BalanceBreakdown_BalanceBreakdown]
GO
USE [Inventory]
GO

/****** Object:  Table [dbo].[Surveys]    Script Date: 04/12/2013 16:23:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Surveys](
	[Timestamp] [datetime] NOT NULL,
	[SZ] [nvarchar](100) NOT NULL,
	[SP] [nvarchar](50) NOT NULL,
	[Balance] [decimal](18, 1) NULL,
	[DefaultMaterial] [nvarchar](50) NOT NULL,
	[DefaultLot] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Balances] PRIMARY KEY CLUSTERED 
(
	[Timestamp] ASC,
	[SZ] ASC,
	[SP] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Surveys] ADD  CONSTRAINT [DF_Surveys_DefaultMaterial]  DEFAULT (N'') FOR [DefaultMaterial]
GO

ALTER TABLE [dbo].[Surveys] ADD  CONSTRAINT [DF_Surveys_DefaultLot]  DEFAULT (N'') FOR [DefaultLot]
GO

USE [Inventory]
GO

/****** Object:  View [dbo].[GetSPBalanceView]    Script Date: 04/12/2013 16:23:47 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[GetSPBalanceView]'))
DROP VIEW [dbo].[GetSPBalanceView]
GO

USE [Inventory]
GO

/****** Object:  View [dbo].[GetSPBalanceView]    Script Date: 04/12/2013 16:23:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[GetSPBalanceView]
AS
SELECT     [Sample Period], SZ, Balance, SP
FROM         dbo.GetSPBalanceFoo() AS GetSPBalanceFoo_1

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "GetSPBalanceFoo_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 189
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GetSPBalanceView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GetSPBalanceView'
GO



