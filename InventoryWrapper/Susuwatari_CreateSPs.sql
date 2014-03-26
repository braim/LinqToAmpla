USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[DeleteAllAfter]    Script Date: 04/12/2013 16:43:39 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteAllAfter]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteAllAfter]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[DeleteAllAfter]    Script Date: 04/12/2013 16:43:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteAllAfter]
	-- Add the parameters for the stored procedure here
	@timestamp timestamp

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete from Movements where Timestamp>@timestamp
END

GO
USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[GetLotBalance]    Script Date: 04/12/2013 16:43:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetLotBalance]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetLotBalance]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[GetLotBalance]    Script Date: 04/12/2013 16:43:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLotBalance]
	-- Add the parameters for the stored procedure here
	@Material nvarchar(100) = NULL,
	@SZ nvarchar(100),
	@SP nvarchar(50)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
	declare @PreTime datetime
	declare @SetQuery varchar(max)
		set @PreTime = (SELECT TOP (1) [TimeStamp] From Surveys where [sp]=@SP and [SZ]=@SZ order by [Timestamp])

	SELECT SZ,SP,M,Lot,  SUM(Balance)  as Balance FROM
	(
		select Survey2.[SZ],Survey2.[SP],SurveyBreakdown.M,SurveyBreakdown.Lot,SurveyBreakdown.Balance
		from SurveyBreakdown
		CROSS APPLY 
		(
			SELECT TOP 1 *
			FROM Surveys
			Where Surveys.[TimeStamp] = [SurveyBreakdown].[TimeStamp] and Surveys.SZ = [SurveyBreakdown].SZ and Surveys.SP = [SurveyBreakdown].SP 
			and Surveys.[SP]=@SP and Surveys.[SZ]=@SZ
		) Survey2

	UNION
	
		select @SZ as SZ, @SP as SP, DestnM as M,DestnLot as Lot,SUM(DestnQuantity) as Balance from Movements 
		where [Timestamp]> ISNULL( @PreTime,'1900-01-01') and [DestnSZ]=@SZ and [DestnSP]=@SP 
		group by DestnLot , DestnM 
		
	UNION 
	
		select @SZ as SZ, @SP as SP,  SourceM as M,SourceLot as Lot,-1*SUM(SourceQuantity) as Balance from Movements
		where [Timestamp]> ISNULL( @PreTime,'1900-01-01') and [SourceSZ]=@SZ and [SourceSP]=@SP 
		group by SourceLot , SourceM
	) foo
	Where M = @Material OR M like ISNULL(@Material,'%')
	group by SZ,SP,M,Lot
	
END

GO
USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[GetLotBalanceWithQ]    Script Date: 04/12/2013 16:43:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetLotBalanceWithQ]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetLotBalanceWithQ]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[GetLotBalanceWithQ]    Script Date: 04/12/2013 16:43:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLotBalanceWithQ] 
	-- Add the parameters for the stored procedure here
	@SZ nvarchar(100),
	@SP nvarchar(50),
	@LOTKEY nvarchar(100),
	@QKEY nvarchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
		declare @PreTime datetime
	declare @SetQuery varchar(max)
		set @PreTime = (SELECT TOP (1) [TimeStamp] From Surveys where [sp]=@SP and [SZ]=@SZ order by [Timestamp])
select * from 

	(SELECT SZ,SP,M,Lot,  SUM(Balance)  as Balance FROM
	(
		select Survey2.[SZ],Survey2.[SP],SurveyBreakdown.M,SurveyBreakdown.Lot,SurveyBreakdown.Balance
		from SurveyBreakdown
		CROSS APPLY 
		(
			SELECT TOP 1 *
			FROM Surveys
			Where Surveys.[TimeStamp] = [SurveyBreakdown].[TimeStamp] and Surveys.SZ = [SurveyBreakdown].SZ and Surveys.SP = [SurveyBreakdown].SP 
			and Surveys.[SP]=@SP and Surveys.[SZ]=@SZ
		) Survey2

	UNION
	
		select @SZ as SZ, @SP as SP, DestnM as M,DestnLot as Lot,SUM(DestnQuantity) as Balance from Movements 
		where [Timestamp]> ISNULL( @PreTime,'1900-01-01') and [DestnSZ]=@SZ and [DestnSP]=@SP 
		group by DestnLot , DestnM 
		
	UNION 
	
		select @SZ as SZ, @SP as SP,  SourceM as M,SourceLot as Lot,-1*SUM(SourceQuantity) as Balance from Movements
		where [Timestamp]> ISNULL( @PreTime,'1900-01-01') and [SourceSZ]=@SZ and [SourceSP]=@SP 
		group by SourceLot , SourceM
	) foo
	
	group by SZ,SP,M,Lot)As LotBalance
	
	
	inner join
	
	
(select ExternalIdentifier as Lot,LOTKEY,QKEY ,

CONVERT(real,[CHPP Module Feed Rate])as [FeedRate] ,
CONVERT(real,[Yield]) as [Yield],
CONVERT(real,[Throughput])as [Throughput],
CONVERT(real,[TM]) as [TM],
CONVERT(real,[ASH]) as [ASH],
CONVERT(real,[IM]) as [IM],
CONVERT(real,[VM]) as [VM],
CONVERT(real,[FC]) as [FC],
CONVERT(real,[TS]) as [TS],
CONVERT(real,[CVK]) as [CVK],
CONVERT(real,[CSN]) as [CSN],
CONVERT(real,[FLU]) as [FLU],
CONVERT(real,[Ro Max]) as [RoMax],
CONVERT(real,[Fe2O3]) as [Fe2O3],
CONVERT(real,[-2]) as [Minus2]
from 

(select * FROM
 (select Field.ReportingPointId,LotDataField.SetId,ExternalIdentifier, 
 Name = (CASE Field.Name WHEN @LOTKEY THEN 'LOTKEY' ELSE Field.Name END)
 , LotDataField.DataValue  from [Xstrata_AmplaData].[dbo].[LotDataField] , [Xstrata_AmplaData].[dbo].Field,[Xstrata_AmplaData].[dbo].Lot where Lot.LotId = LotDataField.SetId and  LotDataField.FieldId = Field.FieldId and ItemFullName like 'XC.XCQ.%Inventory.Lot.Fields.%' and LotDataField.IsActive = 1
 )AS SourceTable
PIVOT
( MAX(DataValue)
FOR Name IN ([LOTKEY],[ConNote])
)AS PIVOTTABLE)As LotRecords
 Left Join
(select * FROM
 (select Field.ReportingPointId,QualityDataField.SetId, 
 Name = (CASE Field.Name WHEN @QKEY THEN 'QKEY' ELSE Field.Name END)
 , QualityDataField.DataValue  from [Xstrata_AmplaData].[dbo].QualityDataField , [Xstrata_AmplaData].[dbo].Field where QualityDataField.FieldId = Field.FieldId and ItemFullName like 'XC.XCQ.Mines%Lab%' and QualityDataField.IsActive = 1
 )AS SourceTable
PIVOT
( MAX(DataValue)
FOR Name IN ([QKEY],[CHPP Module Feed Rate],[Yield],[Throughput],[TM],[ASH],[IM],[VM],[FC],[TS],[CVK],[CSN],[FLU],[Ro Max],[Fe2O3],[-2])
)AS PIVOTTABLE
)as CHPPQ
on LotRecords.LOTKEY = CHPPQ.[QKEY]  ) as LotQuality

on LotBalance.Lot = LotQuality.Lot

END

GO
USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[GetSPBalance]    Script Date: 04/12/2013 16:44:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSPBalance]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSPBalance]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[GetSPBalance]    Script Date: 04/12/2013 16:44:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetSPBalance]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	   select GETDATE() as [Sample Period],SZ,SP,SUM(B) as Balance from (
   
   select  SZ,SP,SUM(Balance) as B from (
  -- INPUT BALANCE OF SZ-SP
  select MM.DestnSZ as SZ,MM.DestnSP as SP,  SUM(MM.DestnQuantity) as Balance,MM.Timestamp as MMT,Foo.Timestamp as SURT from 
  Movements MM 
  LEFT JOIN 
  (select m1.* 
   from Surveys m1 LEFT JOIN Surveys m2
   on (m1.SZ = m2.SZ and m1.SP = m2.SP and m1.Timestamp <m2.Timestamp)
   where m2.Timestamp IS NULL)as Foo
   
   on MM.DestnSP = Foo.SP and MM.DestnSZ = Foo.SZ 
   group by DestnSZ,DestnSP, MM.Timestamp,Foo.Timestamp) as myselect
   where MMT>SURT or SURT is null
   group by SZ,SP

UNION
   
   select m1.SZ , m1.SP,m1.Balance as B
   from Surveys m1 LEFT JOIN Surveys m2
   on (m1.SZ = m2.SZ and m1.SP = m2.SP and m1.Timestamp <m2.Timestamp)
   where m2.Timestamp IS NULL
   
UNION 
   
   select  SZ,SP,-1*SUM(Balance) as B from (
  -- INPUT BALANCE OF SZ-SP
  select MM.SourceSZ as SZ,MM.SourceSP as SP,   SUM(MM.SourceQuantity) as Balance,MM.Timestamp as MMT,Foo.Timestamp as SURT from 
  Movements MM 
  LEFT JOIN 
  (select m1.* 
   from Surveys m1 LEFT JOIN Surveys m2
   on (m1.SZ = m2.SZ and m1.SP = m2.SP and m1.Timestamp <m2.Timestamp)
   where m2.Timestamp IS NULL)as Foo
   
   on MM.SourceSP = Foo.SP and MM.SourceSZ = Foo.SZ 
   group by SourceSZ,SourceSP, MM.Timestamp,Foo.Timestamp) as myselect
   where MMT>SURT or SURT is null
   group by SZ,SP
   )as D 
   group by SZ,SP order by SZ
END

GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[InsertAndBreakdownSurvey]    Script Date: 04/12/2013 16:44:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertAndBreakdownSurvey]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertAndBreakdownSurvey]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[InsertAndBreakdownSurvey]    Script Date: 04/12/2013 16:44:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertAndBreakdownSurvey]
	-- Add the parameters for the stored procedure here
	@Timestamp datetime,
	@SZ nvarchar(100),
	@SP nvarchar(50),
	@Balance decimal,
	@DefaultMaterial nvarchar(50) = null,
	@DefaultLot nvarchar(100)=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
insert into Surveys values (@Timestamp,@SZ,@SP,@Balance,@DefaultMaterial,@DefaultLot)
EXEC	[dbo].[ReBreakdownBalance]
		@Timestamp = @Timestamp ,
		@SZ = @SZ,
		@SP = @SP
END

GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[LotExistsInWC]    Script Date: 04/12/2013 16:44:22 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LotExistsInWC]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[LotExistsInWC]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[LotExistsInWC]    Script Date: 04/12/2013 16:44:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[LotExistsInWC] 
	-- Add the parameters for the stored procedure here
	@SZ nvarchar(50) ,
	@Lot nvarchar(50) ,
	@Material nvarchar(50)
	
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT COUNT(ID) from Movements where 
		DestnLot = @lot and DestnM = @Material and DestnSZ = @SZ
END

GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[PerformMovement]    Script Date: 04/12/2013 16:44:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PerformMovement]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[PerformMovement]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[PerformMovement]    Script Date: 04/12/2013 16:44:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[PerformMovement]
@SourceSZ nvarchar(100),
@SourceLot nvarchar(100),
@DestnSZ nvarchar(100),
@DestnLot nvarchar(100),
@SourceQuantity float,	
@DestnQuantity float,
@ProdID nvarchar(50),
@TimeStamp datetime,
@SourceSP nvarchar(50),
@DestnSP nvarchar (50),
@SourceM nvarchar(50),
@DestnM nvarchar (50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;
     INSERT INTO [Inventory].[dbo].[Movements]
           ([SourceSZ]
           ,[SourceLot]
           ,[DestnSZ]
           ,[DestnLot]
           ,[SourceQuantity]
           ,[DestnQuantity]
           ,[ProdID]
           ,[Timestamp]
           ,[SourceSP]
           ,[DestnSP]
           ,[SourceM]
           ,[DestnM])
     VALUES
           (@Sourcesz,@SourceLot,@DestnSZ,@DestnLot,@SourceQuantity,@DestnQuantity,@ProdID,@TimeStamp,@SourceSP,@DestnSP,@SourceM,@DestnM)
           
      select @@IDENTITY 

END

GO
USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[ReBreakdownBalance]    Script Date: 04/12/2013 16:44:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReBreakdownBalance]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ReBreakdownBalance]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[ReBreakdownBalance]    Script Date: 04/12/2013 16:44:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ReBreakdownBalance] 
	-- Add the parameters for the stored procedure here
	@Timestamp datetime,
	@SZ nvarchar(100),
	@SP nvarchar(50)

AS
BEGIN


	
	

	
	
	
	
	BEGIN TRY
	
	
	declare @TotalCurrentBalance real;
	declare @TotalNewBalance real;
	declare @PreTime datetime;
	declare @DefaultMaterial nvarchar(50)
	declare @DefaultLot nvarchar(100)
	set @TotalNewBalance = (Select TOP(1) Balance From Surveys where [Timestamp]=@Timestamp and [SP]=@SP and [SZ]=@SZ)

	if @TotalNewBalance is NULL RAISERROR('Balance Not Found',15,1,1)
		set @DefaultMaterial = (Select TOP(1) DefaultMaterial From Surveys where [Timestamp]=@Timestamp and [SP]=@SP and [SZ]=@SZ)
	set @DefaultLot = (Select TOP(1) DefaultLot From Surveys where [Timestamp]=@Timestamp and [SP]=@SP and [SZ]=@SZ)
	set @PreTime = (SELECT TOP (1) [TimeStamp] From Surveys where [sp]=@SP and [SZ]=@SZ and [TimeStamp]<@Timestamp order by [Timestamp])
	--select @PreTime 
	delete from SurveyBreakdown where [Timestamp]=@Timestamp and [SP]=@SP and [SZ]=@SZ

	SELECT @TotalCurrentBalance = SUM(Balance) FROM
	(
	select Balances2.[SZ],Balances2.[SP],SurveyBreakdown.M,SurveyBreakdown.Lot,SurveyBreakdown.Balance
	from SurveyBreakdown
	CROSS APPLY 
	(
		SELECT TOP 1 *
		FROM Surveys
		Where Surveys.[TimeStamp] = [SurveyBreakdown].[TimeStamp] and Surveys.SZ = [SurveyBreakdown].SZ and Surveys.SP = [SurveyBreakdown].SP 
		and 
		Surveys.[TimeStamp]< @Timestamp and Surveys.[SP]=@SP and Surveys.[SZ]=@SZ
		order by Surveys.[TimeStamp]
	) Balances2

	UNION
	select @SZ as SZ, @SP as SP, DestnM as M,DestnLot as Lot,SUM(DestnQuantity) as Balance from Movements 
	where [Timestamp]> ISNULL( @PreTime,'2000-01-01') and [DestnSZ]=@SZ and [DestnSP]=@SP and [Timestamp]<=@Timestamp
	group by DestnLot , DestnM UNION 
	
	select @SZ as SZ, @SP as SP,  SourceM as M,SourceLot as Lot,-1*SUM(SourceQuantity) as Balance from Movements
		where [Timestamp]> ISNULL( @PreTime,'2000-01-01') and [SourceSZ]=@SZ and [SourceSP]=@SP and [Timestamp]<=@Timestamp
	group by SourceLot , SourceM
	) foo
	------------------------------------------------------------------------------------------------------------------
		if @TotalCurrentBalance is NULL
	BEGIN 
		INSERT INTO [Inventory].[dbo].[SurveyBreakdown] values
		(@Timestamp,@SZ,@SP,@DefaultMaterial,@DefaultLot,@TotalNewBalance)
	END 
	ELSE
	BEGIN
				INSERT INTO [Inventory].[dbo].[SurveyBreakdown]
	
		
	SELECT @Timestamp,SZ,SP,M,Lot,ISNULL(@TotalNewBalance*  SUM(Balance)/@TotalCurrentBalance,0)  as Balance FROM
	(
		select Balances2.[SZ],Balances2.[SP],SurveyBreakdown.M,SurveyBreakdown.Lot,SurveyBreakdown.Balance
		from SurveyBreakdown
		CROSS APPLY 
		(
			SELECT TOP 1 *
			FROM Surveys
			Where Surveys.[TimeStamp] = [SurveyBreakdown].[TimeStamp] and Surveys.SZ = [SurveyBreakdown].SZ and Surveys.SP = [SurveyBreakdown].SP 
			and 
			Surveys.[TimeStamp]< @Timestamp and Surveys.[SP]=@SP and Surveys.[SZ]=@SZ
		) Balances2

	UNION
	
		select @SZ as SZ, @SP as SP, DestnM as M,DestnLot as Lot,SUM(DestnQuantity) as Balance from Movements 
		where [Timestamp]> ISNULL( @PreTime,'2000-01-01') and [DestnSZ]=@SZ and [DestnSP]=@SP and [Timestamp]<=@Timestamp
		group by DestnLot , DestnM 
		
	UNION 
	
		select @SZ as SZ, @SP as SP,  SourceM as M,SourceLot as Lot,-1*SUM(SourceQuantity) as Balance from Movements
		where [Timestamp]> ISNULL( @PreTime,'2000-01-01') and [SourceSZ]=@SZ and [SourceSP]=@SP and [Timestamp]<=@Timestamp
		group by SourceLot , SourceM
	) foo
	group by SZ,SP,M,Lot
	END
	END TRY
	BEGIN CATCH
		declare @errormessage nvarchar(50)
		SELECT @errormessage = ERROR_MESSAGE()
		RAISERROR(@errormessage,15,1,1) 
	END CATCH
	
	
END

GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[UndoMovementsFrom]    Script Date: 04/12/2013 16:44:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UndoMovementsFrom]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UndoMovementsFrom]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[UndoMovementsFrom]    Script Date: 04/12/2013 16:44:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UndoMovementsFrom]
	-- Add the parameters for the stored procedure here
	@FromDate datetime
AS
BEGIN
	

   -- delete from [dbo].[Movements] where Movements.Timestamp>=@FromDate
   update [dbo].[Movements] set DestnQuantity =0, SourceQuantity=0 where Movements.Timestamp>=@FromDate
   select @@ROWCOUNT
END

GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[Zap]    Script Date: 04/12/2013 16:44:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Zap]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Zap]
GO

USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[Zap]    Script Date: 04/12/2013 16:44:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Zap]

AS
BEGIN
 Delete from Movements 
  Delete from SurveyBreakdown
 Delete from Surveys

END

GO





