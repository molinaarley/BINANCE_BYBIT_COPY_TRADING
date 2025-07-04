USE [master]
GO
/****** Object:  Database [Binance]    Script Date: 31/05/2025 01:26:17 ******/
CREATE DATABASE [Binance]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Binance', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Binance.mdf' , SIZE = 54272KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Binance_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Binance_log.ldf' , SIZE = 1219712KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Binance] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Binance].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Binance] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Binance] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Binance] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Binance] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Binance] SET ARITHABORT OFF 
GO
ALTER DATABASE [Binance] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Binance] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Binance] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Binance] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Binance] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Binance] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Binance] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Binance] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Binance] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Binance] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Binance] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Binance] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Binance] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Binance] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Binance] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Binance] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Binance] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Binance] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Binance] SET RECOVERY FULL 
GO
ALTER DATABASE [Binance] SET  MULTI_USER 
GO
ALTER DATABASE [Binance] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Binance] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Binance] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Binance] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Binance', N'ON'
GO
USE [Binance]
GO
/****** Object:  User [linagma32046com26487_artelcom]    Script Date: 31/05/2025 01:26:17 ******/
CREATE USER [linagma32046com26487_artelcom] FOR LOGIN [linagma32046com26487_artelcom] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [linagma32046com26487_artelcom]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[LastName] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Binance_ByBit_Order]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_ByBit_Order](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ByBit_OrderId] [varchar](250) NOT NULL,
	[ByBit_OrderLinkId] [varchar](250) NOT NULL,
	[Binance_Order_Id] [int] NOT NULL,
	[CreatedOn] [datetime] NULL,
	[Id_Telegrame] [bigint] NOT NULL,
	[Amount] [float] NULL,
 CONSTRAINT [PK_Binance_ByBit_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_ByBit_Order_Audit]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_ByBit_Order_Audit](
	[Id] [int] NOT NULL,
	[ByBit_OrderId] [varchar](250) NOT NULL,
	[ByBit_OrderLinkId] [varchar](250) NOT NULL,
	[Binance_Order_Id] [int] NOT NULL,
	[CreatedOn] [datetime] NULL,
	[Id_Telegrame] [bigint] NOT NULL,
	[DeleteOn] [datetime] NULL,
	[Amount] [float] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_ByBit_Users]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_ByBit_Users](
	[Id_Telegrame] [bigint] NOT NULL,
	[PhoneNumber_Telegrame] [nvarchar](250) NULL,
	[Isactive] [bit] NULL,
	[Createdate] [datetime] NOT NULL,
	[LastName] [nvarchar](250) NULL,
	[Name] [nvarchar](250) NULL,
	[ApiKey] [varchar](100) NULL,
	[SecretKey] [varchar](100) NULL,
 CONSTRAINT [PK_Binance_ByBit_Users] PRIMARY KEY CLUSTERED 
(
	[Id_Telegrame] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_MonitoringCoinWalletBalance]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Binance_MonitoringCoinWalletBalance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NULL,
	[Equity] [float] NOT NULL,
	[UnrealisedPnl] [float] NOT NULL,
	[walletBalance] [float] NOT NULL,
	[Id_Telegrame] [bigint] NOT NULL,
	[Amount] [float] NULL,
 CONSTRAINT [PK_Binance_MonitoringCoinWalletBalance] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Binance_MonitoringCoinWalletBalanceObjectiveProcess]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Binance_MonitoringCoinWalletBalanceObjectiveProcess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NULL,
	[Equity] [float] NOT NULL,
	[EquityObjective] [float] NOT NULL,
	[UnrealisedPnl] [float] NOT NULL,
	[walletBalance] [float] NOT NULL,
	[EndDate] [datetime] NULL,
	[Id_Telegrame] [bigint] NOT NULL,
 CONSTRAINT [PK_Binance_MonitoringCoinWalletBalanceObjectiveProcess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Binance_MonitoringProcess]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Binance_MonitoringProcess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_Binance_MonitoringProcess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Binance_Order]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_Order](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Symbol] [varchar](250) NOT NULL,
	[Amount] [float] NULL,
	[EntryPrice] [float] NULL,
	[Leverage] [float] NULL,
	[MarkPrice] [float] NULL,
	[Pnl] [float] NULL,
	[TradeBefore] [bit] NULL,
	[UpdateTime] [datetime] NULL,
	[CreatedOn] [datetime] NULL,
	[UpdateTimeStamp] [float] NULL,
	[Yellow] [bit] NULL,
	[EncryptedUid] [varchar](100) NOT NULL,
	[Side] [varchar](10) NOT NULL,
	[IsForClosed] [bit] NOT NULL,
 CONSTRAINT [PK_Binance_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_Order_Audit]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_Order_Audit](
	[Id] [int] NOT NULL,
	[Symbol] [varchar](250) NOT NULL,
	[Amount] [float] NULL,
	[EntryPrice] [float] NULL,
	[Leverage] [float] NULL,
	[MarkPrice] [float] NULL,
	[Pnl] [float] NULL,
	[TradeBefore] [bit] NULL,
	[UpdateTime] [datetime] NULL,
	[CreatedOn] [datetime] NULL,
	[UpdateTimeStamp] [float] NULL,
	[Yellow] [bit] NULL,
	[EncryptedUid] [varchar](100) NOT NULL,
	[Side] [varchar](10) NOT NULL,
	[IsForClosed] [bit] NOT NULL,
	[DeleteOn] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_Order_Trade_History]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_Order_Trade_History](
	[Symbol] [varchar](250) NOT NULL,
	[Order_No] [varchar](250) NOT NULL,
	[CreatedOn] [datetime] NULL,
	[Leverage] [float] NULL,
	[Side] [varchar](10) NOT NULL,
	[Order_Type] [varchar](250) NOT NULL,
	[Filled_Type] [varchar](250) NOT NULL,
	[Filled_Qty] [varchar](250) NULL,
	[Filled_Price] [varchar](250) NULL,
	[Order_Price] [varchar](250) NULL,
	[Trading_Fee_Rate] [varchar](250) NULL,
	[Fees_Paid] [varchar](250) NULL,
	[Transaction_ID] [varchar](250) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_Trader]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_Trader](
	[EncryptedUid] [varchar](100) NOT NULL,
	[nickName] [varchar](100) NOT NULL,
	[CreatedOn] [datetime2](7) NULL,
	[RankTrader] [int] NULL,
	[PositionShared] [bit] NULL,
	[FollowerCount] [int] NULL,
	[UpdateTime] [varchar](100) NULL,
	[TopEliableTredor] [int] NULL,
 CONSTRAINT [PK_Binance_Trader] PRIMARY KEY CLUSTERED 
(
	[EncryptedUid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_Trader_Performance_RetList]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_Trader_Performance_RetList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EncryptedUid] [varchar](100) NOT NULL,
	[CreatedOn] [datetime2](7) NULL,
	[PeriodType] [varchar](50) NULL,
	[StatisticsType] [varchar](5) NULL,
	[Value] [float] NULL,
	[rank] [int] NULL,
	[GuidNewItem] [varchar](50) NULL,
 CONSTRAINT [PK_Binance_Trader_Performance_RetList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_Trader_Performance_RetList_Audit]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_Trader_Performance_RetList_Audit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EncryptedUid] [varchar](100) NOT NULL,
	[CreatedOn] [datetime2](7) NULL,
	[PeriodType] [varchar](50) NULL,
	[StatisticsType] [varchar](5) NULL,
	[Value] [float] NULL,
	[rank] [int] NULL,
	[GuidNewItem] [varchar](50) NULL,
 CONSTRAINT [PK_Binance_Trader_Performance_RetList_Audit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_Trader_TypeData]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_Trader_TypeData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EncryptedUid] [varchar](100) NOT NULL,
	[CreatedOn] [datetime2](7) NULL,
	[Pnl] [float] NULL,
	[Roi] [float] NULL,
	[TypeData] [varchar](15) NULL,
 CONSTRAINT [PK_Binance_Trader_TypeData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Binance_TraderUrlForUpdatePositionBinanceQuery]    Script Date: 31/05/2025 01:26:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Binance_TraderUrlForUpdatePositionBinanceQuery](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EncryptedUid] [varchar](100) NOT NULL,
	[Binance_MonitoringCoinWalletBalanceObjectiveProcess_Id] [int] NOT NULL,
 CONSTRAINT [PK_Binance_TraderUrlForUpdatePositionBinanceQuery] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 31/05/2025 01:26:17 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [RoleNameIndex]    Script Date: 31/05/2025 01:26:17 ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 31/05/2025 01:26:17 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 31/05/2025 01:26:17 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 31/05/2025 01:26:17 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [EmailIndex]    Script Date: 31/05/2025 01:26:17 ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UserNameIndex]    Script Date: 31/05/2025 01:26:17 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Binance_ByBit_Order_Binance_Order_Id]    Script Date: 31/05/2025 01:26:17 ******/
CREATE NONCLUSTERED INDEX [IX_Binance_ByBit_Order_Binance_Order_Id] ON [dbo].[Binance_ByBit_Order]
(
	[Binance_Order_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Binance_ByBit_Order_Id_Telegrame]    Script Date: 31/05/2025 01:26:17 ******/
CREATE NONCLUSTERED INDEX [IX_Binance_ByBit_Order_Id_Telegrame] ON [dbo].[Binance_ByBit_Order]
(
	[Id_Telegrame] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Binance_Order_EncryptedUid]    Script Date: 31/05/2025 01:26:17 ******/
CREATE NONCLUSTERED INDEX [IX_Binance_Order_EncryptedUid] ON [dbo].[Binance_Order]
(
	[EncryptedUid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Binance_ByBit_Order] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Binance_ByBit_Order_Audit] ADD  DEFAULT (getdate()) FOR [DeleteOn]
GO
ALTER TABLE [dbo].[Binance_ByBit_Users] ADD  DEFAULT (CONVERT([bit],(0))) FOR [Isactive]
GO
ALTER TABLE [dbo].[Binance_ByBit_Users] ADD  DEFAULT (getdate()) FOR [Createdate]
GO
ALTER TABLE [dbo].[Binance_Order_Audit] ADD  DEFAULT (getdate()) FOR [DeleteOn]
GO
ALTER TABLE [dbo].[Binance_Trader_Performance_RetList] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Binance_Trader_Performance_RetList_Audit] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Binance_Trader_TypeData] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Binance_ByBit_Order]  WITH CHECK ADD  CONSTRAINT [FK_Binance_ByBit_Order_Binance_ByBit_Users] FOREIGN KEY([Id_Telegrame])
REFERENCES [dbo].[Binance_ByBit_Users] ([Id_Telegrame])
GO
ALTER TABLE [dbo].[Binance_ByBit_Order] CHECK CONSTRAINT [FK_Binance_ByBit_Order_Binance_ByBit_Users]
GO
ALTER TABLE [dbo].[Binance_ByBit_Order]  WITH CHECK ADD  CONSTRAINT [FK_Binance_ByBit_Order_Binance_Order] FOREIGN KEY([Binance_Order_Id])
REFERENCES [dbo].[Binance_Order] ([Id])
GO
ALTER TABLE [dbo].[Binance_ByBit_Order] CHECK CONSTRAINT [FK_Binance_ByBit_Order_Binance_Order]
GO
ALTER TABLE [dbo].[Binance_MonitoringCoinWalletBalance]  WITH CHECK ADD  CONSTRAINT [FK_Binance_MonitoringCoinWalletBalance_Binance_ByBit_Users] FOREIGN KEY([Id_Telegrame])
REFERENCES [dbo].[Binance_ByBit_Users] ([Id_Telegrame])
GO
ALTER TABLE [dbo].[Binance_MonitoringCoinWalletBalance] CHECK CONSTRAINT [FK_Binance_MonitoringCoinWalletBalance_Binance_ByBit_Users]
GO
ALTER TABLE [dbo].[Binance_MonitoringCoinWalletBalanceObjectiveProcess]  WITH CHECK ADD  CONSTRAINT [FK_Binance_MonitoringCoinWalletBalanceObjectiveProcess_Binance_ByBit_Users] FOREIGN KEY([Id_Telegrame])
REFERENCES [dbo].[Binance_ByBit_Users] ([Id_Telegrame])
GO
ALTER TABLE [dbo].[Binance_MonitoringCoinWalletBalanceObjectiveProcess] CHECK CONSTRAINT [FK_Binance_MonitoringCoinWalletBalanceObjectiveProcess_Binance_ByBit_Users]
GO
ALTER TABLE [dbo].[Binance_Order]  WITH CHECK ADD  CONSTRAINT [FK_Binance_Order_Binance_Trader] FOREIGN KEY([EncryptedUid])
REFERENCES [dbo].[Binance_Trader] ([EncryptedUid])
GO
ALTER TABLE [dbo].[Binance_Order] CHECK CONSTRAINT [FK_Binance_Order_Binance_Trader]
GO
ALTER TABLE [dbo].[Binance_Trader_Performance_RetList]  WITH CHECK ADD  CONSTRAINT [FK_Binance_Trader_Performance_RetList_Binance_Trader] FOREIGN KEY([EncryptedUid])
REFERENCES [dbo].[Binance_Trader] ([EncryptedUid])
GO
ALTER TABLE [dbo].[Binance_Trader_Performance_RetList] CHECK CONSTRAINT [FK_Binance_Trader_Performance_RetList_Binance_Trader]
GO
ALTER TABLE [dbo].[Binance_Trader_Performance_RetList_Audit]  WITH CHECK ADD  CONSTRAINT [FK_Binance_Trader_Performance_RetList_Audit_Binance_Trader] FOREIGN KEY([EncryptedUid])
REFERENCES [dbo].[Binance_Trader] ([EncryptedUid])
GO
ALTER TABLE [dbo].[Binance_Trader_Performance_RetList_Audit] CHECK CONSTRAINT [FK_Binance_Trader_Performance_RetList_Audit_Binance_Trader]
GO
ALTER TABLE [dbo].[Binance_Trader_TypeData]  WITH CHECK ADD  CONSTRAINT [FK_Binance_Trader_TypeData_Binance_Trader] FOREIGN KEY([EncryptedUid])
REFERENCES [dbo].[Binance_Trader] ([EncryptedUid])
GO
ALTER TABLE [dbo].[Binance_Trader_TypeData] CHECK CONSTRAINT [FK_Binance_Trader_TypeData_Binance_Trader]
GO
ALTER TABLE [dbo].[Binance_TraderUrlForUpdatePositionBinanceQuery]  WITH CHECK ADD  CONSTRAINT [FK_Binance_TraderUrlForUpdatePositionBinanceQuery_Binance_MonitoringCoinWalletBalanceObjectiveProcess] FOREIGN KEY([Binance_MonitoringCoinWalletBalanceObjectiveProcess_Id])
REFERENCES [dbo].[Binance_MonitoringCoinWalletBalanceObjectiveProcess] ([Id])
GO
ALTER TABLE [dbo].[Binance_TraderUrlForUpdatePositionBinanceQuery] CHECK CONSTRAINT [FK_Binance_TraderUrlForUpdatePositionBinanceQuery_Binance_MonitoringCoinWalletBalanceObjectiveProcess]
GO
USE [master]
GO
ALTER DATABASE [Binance] SET  READ_WRITE 
GO
