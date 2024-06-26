/****** Object:  Table [dbo].[voters_ledger]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[voters_ledger](
	[idNum] [int] NOT NULL,
	[lastName] [nvarchar](50) NULL,
	[firstName] [nvarchar](50) NULL,
	[fathersName] [nvarchar](50) NULL,
	[cityId] [int] NULL,
	[ballotId] [int] NULL,
	[spare1] [nvarchar](50) NULL,
	[residenceId] [int] NULL,
	[residenceName] [nvarchar](50) NULL,
	[spare2] [nvarchar](50) NULL,
	[streetId] [int] NULL,
	[streetName] [nvarchar](50) NULL,
	[houseNumber] [int] NULL,
	[entrance] [nvarchar](10) NULL,
	[appartment] [nvarchar](10) NULL,
	[houseLetter] [nvarchar](5) NULL,
	[ballotSerial] [int] NULL,
	[spare3] [nvarchar](50) NULL,
	[spare4] [nvarchar](50) NULL,
	[zipCode] [int] NULL,
	[spare5] [nvarchar](50) NULL,
	[campaignYear] [int] NOT NULL,
 CONSTRAINT [PK_Pinkas] PRIMARY KEY CLUSTERED 
(
	[idNum] ASC,
	[campaignYear] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[voters_ledger_dynamic]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[voters_ledger_dynamic](
	[IdNum] [int] NOT NULL,
	[email1] [nvarchar](200) NULL,
	[email2] [nvarchar](200) NULL,
	[phone1] [nvarchar](20) NULL,
	[phone2] [nvarchar](20) NULL,
 CONSTRAINT [PK_voters_ledger_dynamic] PRIMARY KEY CLUSTERED 
(
	[IdNum] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[VotersLedgerFull]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[VotersLedgerFull]
AS
SELECT dbo.voters_ledger.*, dbo.voters_ledger_dynamic.email1, dbo.voters_ledger_dynamic.email2, dbo.voters_ledger_dynamic.phone1, dbo.voters_ledger_dynamic.phone2
FROM     dbo.voters_ledger LEFT OUTER JOIN
                  dbo.voters_ledger_dynamic ON dbo.voters_ledger.idNum = dbo.voters_ledger_dynamic.IdNum
GO
/****** Object:  Table [dbo].[voter_campaign_support_statuses]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[voter_campaign_support_statuses](
	[IdNum] [int] NOT NULL,
	[campaignId] [int] NOT NULL,
	[supportStatus] [bit] NOT NULL,
 CONSTRAINT [PK_voter_campaign_support_statuses] PRIMARY KEY CLUSTERED 
(
	[IdNum] ASC,
	[campaignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaigns]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaigns](
	[campaignId] [int] IDENTITY(1,1) NOT NULL,
	[campaignName] [nvarchar](200) NOT NULL,
	[campaignGuid] [uniqueidentifier] NOT NULL,
	[campaignCreatorUserId] [int] NOT NULL,
	[campaignDescription] [nvarchar](500) NULL,
	[campaignCreationDate] [datetime] NOT NULL,
	[campaignIsActive] [bit] NOT NULL,
	[campaignInviteGuid] [uniqueidentifier] NULL,
	[isMunicipal] [bit] NOT NULL,
	[isSubCampaign] [bit] NOT NULL,
	[campaignLogoUrl] [nvarchar](2083) NULL,
	[cityId] [smallint] NOT NULL,
	[isCustomCampaign] [bit] NOT NULL,
 CONSTRAINT [PK_campaigns] PRIMARY KEY CLUSTERED 
(
	[campaignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[VotersSupportStatuses]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[VotersSupportStatuses]
AS
SELECT dbo.VotersLedgerFull.idNum, dbo.VotersLedgerFull.lastName, dbo.VotersLedgerFull.firstName, dbo.VotersLedgerFull.fathersName, dbo.VotersLedgerFull.cityId, dbo.VotersLedgerFull.ballotId, dbo.VotersLedgerFull.spare1, 
                  dbo.VotersLedgerFull.residenceId, dbo.VotersLedgerFull.residenceName, dbo.VotersLedgerFull.spare2, dbo.VotersLedgerFull.streetId, dbo.VotersLedgerFull.streetName, dbo.VotersLedgerFull.houseNumber, 
                  dbo.VotersLedgerFull.entrance, dbo.VotersLedgerFull.appartment, dbo.VotersLedgerFull.houseLetter, dbo.VotersLedgerFull.ballotSerial, dbo.VotersLedgerFull.spare3, dbo.VotersLedgerFull.spare4, dbo.VotersLedgerFull.zipCode, 
                  dbo.VotersLedgerFull.spare5, dbo.VotersLedgerFull.campaignYear, dbo.VotersLedgerFull.email1, dbo.VotersLedgerFull.email2, dbo.VotersLedgerFull.phone1, dbo.VotersLedgerFull.phone2, 
                  dbo.voter_campaign_support_statuses.campaignId, dbo.voter_campaign_support_statuses.supportStatus, dbo.campaigns.campaignName, dbo.campaigns.campaignGuid
FROM     dbo.campaigns RIGHT OUTER JOIN
                  dbo.voter_campaign_support_statuses ON dbo.campaigns.campaignId = dbo.voter_campaign_support_statuses.campaignId RIGHT OUTER JOIN
                  dbo.VotersLedgerFull ON dbo.voter_campaign_support_statuses.IdNum = dbo.VotersLedgerFull.idNum
GO
/****** Object:  Table [dbo].[ballots]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ballots](
	[cityId] [smallint] NOT NULL,
	[innerCityBallotId] [float] NOT NULL,
	[ballotAddress] [nvarchar](100) NOT NULL,
	[ballotLocation] [nvarchar](100) NOT NULL,
	[accessible] [bit] NOT NULL,
	[elligibleVoters] [smallint] NOT NULL,
	[ballotId] [int] IDENTITY(12128,1) NOT NULL,
 CONSTRAINT [PK_imported_ballots] PRIMARY KEY CLUSTERED 
(
	[ballotId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[VotersLedgerFullWithBallots]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[VotersLedgerFullWithBallots]
AS
SELECT dbo.VotersLedgerFull.idNum, dbo.VotersLedgerFull.lastName, dbo.VotersLedgerFull.firstName, dbo.VotersLedgerFull.fathersName, dbo.VotersLedgerFull.cityId, dbo.VotersLedgerFull.ballotId, dbo.VotersLedgerFull.spare1, 
                  dbo.VotersLedgerFull.residenceId, dbo.VotersLedgerFull.residenceName, dbo.VotersLedgerFull.spare2, dbo.VotersLedgerFull.streetId, dbo.VotersLedgerFull.streetName, dbo.VotersLedgerFull.houseNumber, 
                  dbo.VotersLedgerFull.entrance, dbo.VotersLedgerFull.appartment, dbo.VotersLedgerFull.houseLetter, dbo.VotersLedgerFull.ballotSerial, dbo.VotersLedgerFull.spare3, dbo.VotersLedgerFull.spare4, dbo.VotersLedgerFull.zipCode, 
                  dbo.VotersLedgerFull.spare5, dbo.VotersLedgerFull.campaignYear, dbo.VotersLedgerFull.email1, dbo.VotersLedgerFull.email2, dbo.VotersLedgerFull.phone1, dbo.VotersLedgerFull.phone2, dbo.ballots.innerCityBallotId, 
                  dbo.ballots.ballotAddress, dbo.ballots.ballotLocation, dbo.ballots.accessible, dbo.ballots.elligibleVoters
FROM     dbo.VotersLedgerFull LEFT OUTER JOIN
                  dbo.ballots ON dbo.VotersLedgerFull.ballotId = dbo.ballots.ballotId
GO
/****** Object:  Table [dbo].[cities]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cities](
	[cityId] [smallint] NOT NULL,
	[cityName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_cities_imported] PRIMARY KEY CLUSTERED 
(
	[cityId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[CityBallots]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CityBallots]
AS
SELECT ballots.innerCityBallotId, ballots.ballotAddress, ballots.ballotLocation, ballots.accessible, ballots.elligibleVoters, ballots.ballotId, cities.cityName, cities.cityId AS Expr1
FROM     ballots INNER JOIN
                  cities ON ballots.cityId = cities.cityId
GO
/****** Object:  Table [dbo].[campaign_advisor_analysis_samples]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaign_advisor_analysis_samples](
	[resultsId] [int] NOT NULL,
	[sampleText] [nvarchar](400) NOT NULL,
	[isArticle] [bit] NOT NULL,
 CONSTRAINT [PK_campaign_advisor_analysis_samples] PRIMARY KEY CLUSTERED 
(
	[resultsId] ASC,
	[sampleText] ASC,
	[isArticle] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaign_advisor_results_details]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaign_advisor_results_details](
	[resultsId] [int] NOT NULL,
	[topic] [nvarchar](50) NOT NULL,
	[total] [int] NOT NULL,
	[positive] [float] NOT NULL,
	[negative] [float] NOT NULL,
	[neutral] [float] NOT NULL,
	[hate] [float] NOT NULL,
	[rowType] [int] NOT NULL,
 CONSTRAINT [PK_campaign_advisor_results_details] PRIMARY KEY CLUSTERED 
(
	[resultsId] ASC,
	[topic] ASC,
	[rowType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaign_advisor_results_overview]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaign_advisor_results_overview](
	[resultsId] [int] IDENTITY(1,1) NOT NULL,
	[resultsGuid] [uniqueidentifier] NOT NULL,
	[timePerformed] [datetime] NOT NULL,
	[campaignId] [int] NOT NULL,
	[resultsTitle] [nvarchar](150) NOT NULL,
	[analysisTarget] [nvarchar](100) NOT NULL,
	[targetTwitterHandle] [nvarchar](50) NULL,
	[maxDaysBack] [int] NOT NULL,
	[gptResponse] [nvarchar](max) NULL,
	[additionalUserRequests] [nvarchar](400) NULL,
 CONSTRAINT [PK_campaign_advisor_results_overview] PRIMARY KEY CLUSTERED 
(
	[resultsId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaign_users]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaign_users](
	[campaignId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[roleId] [int] NOT NULL,
 CONSTRAINT [PK_campaign_users] PRIMARY KEY CLUSTERED 
(
	[campaignId] ASC,
	[userId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[custom_ballots]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[custom_ballots](
	[cityName] [nvarchar](100) NULL,
	[innerCityBallotId] [float] NOT NULL,
	[ballotAddress] [nvarchar](100) NULL,
	[ballotLocation] [nvarchar](100) NULL,
	[accessible] [bit] NULL,
	[elligibleVoters] [smallint] NULL,
	[campaignId] [int] NOT NULL,
	[ballotId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_custom_ballots_1] PRIMARY KEY CLUSTERED 
(
	[innerCityBallotId] ASC,
	[campaignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[custom_events]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[custom_events](
	[eventId] [int] IDENTITY(1,1) NOT NULL,
	[eventGuid] [uniqueidentifier] NOT NULL,
	[eventName] [nvarchar](100) NOT NULL,
	[eventDescription] [nvarchar](500) NULL,
	[eventLocation] [nvarchar](100) NULL,
	[eventStartTime] [datetime] NULL,
	[eventEndTime] [datetime] NULL,
	[campaignId] [int] NULL,
	[maxAttendees] [int] NULL,
	[numAttending] [int] NOT NULL,
	[eventCreatorId] [int] NOT NULL,
	[isOpenJoin] [bit] NOT NULL,
	[eventOf] [int] NULL,
 CONSTRAINT [PK_custom_events] PRIMARY KEY CLUSTERED 
(
	[eventId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[custom_voters_ledgers]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[custom_voters_ledgers](
	[ledgerId] [int] IDENTITY(1,1) NOT NULL,
	[campaignId] [int] NOT NULL,
	[ledgerGuid] [uniqueidentifier] NOT NULL,
	[ledgerName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_custom_voters_ledgers] PRIMARY KEY CLUSTERED 
(
	[ledgerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[custom_voters_ledgers_content]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[custom_voters_ledgers_content](
	[identifier] [int] NOT NULL,
	[ledgerId] [int] NOT NULL,
	[lastName] [nvarchar](50) NULL,
	[firstName] [nvarchar](50) NULL,
	[cityName] [nvarchar](50) NULL,
	[ballotId] [float] NULL,
	[streetName] [nvarchar](50) NULL,
	[houseNumber] [int] NULL,
	[entrance] [nvarchar](10) NULL,
	[appartment] [nvarchar](10) NULL,
	[houseLetter] [nvarchar](5) NULL,
	[zipCode] [int] NULL,
	[email1] [nvarchar](200) NULL,
	[email2] [nvarchar](200) NULL,
	[phone1] [nvarchar](30) NULL,
	[phone2] [nvarchar](30) NULL,
	[supportStatus] [bit] NULL,
 CONSTRAINT [PK_customs_voters_ledgers_content] PRIMARY KEY CLUSTERED 
(
	[identifier] ASC,
	[ledgerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[defined_permissions]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[defined_permissions](
	[permissionId] [int] IDENTITY(1,1) NOT NULL,
	[permissionType] [nvarchar](10) NOT NULL,
	[permissionTarget] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_permissions] PRIMARY KEY CLUSTERED 
(
	[permissionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[election_results]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[election_results](
	[partyName] [nvarchar](50) NOT NULL,
	[electionYear] [nvarchar](20) NOT NULL,
	[cityId] [int] NOT NULL,
	[ballotId] [float] NOT NULL,
	[votes] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[events_watchers]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[events_watchers](
	[userId] [int] NOT NULL,
	[eventId] [int] NOT NULL,
 CONSTRAINT [PK_events_watchers] PRIMARY KEY CLUSTERED 
(
	[userId] ASC,
	[eventId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[financial_data]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[financial_data](
	[campaignId] [int] NOT NULL,
	[amount] [float] NOT NULL,
	[financialType] [int] NOT NULL,
	[isExpense] [bit] NOT NULL,
	[dataGuid] [uniqueidentifier] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[dataTitle] [nvarchar](50) NULL,
	[dataDescription] [nvarchar](500) NULL,
	[creatorUserId] [int] NOT NULL,
 CONSTRAINT [PK_financial_data] PRIMARY KEY CLUSTERED 
(
	[dataGuid] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[financial_types]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[financial_types](
	[typeId] [int] IDENTITY(1,1) NOT NULL,
	[typeName] [nvarchar](100) NOT NULL,
	[campaignId] [int] NOT NULL,
	[typeGuid] [uniqueidentifier] NOT NULL,
	[typeDescription] [nvarchar](300) NULL,
 CONSTRAINT [PK_financial_types] PRIMARY KEY CLUSTERED 
(
	[typeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_assign_capable_users]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_assign_capable_users](
	[jobId] [int] NOT NULL,
	[userId] [int] NOT NULL,
 CONSTRAINT [PK_users_who_can_assign_to_jobs] PRIMARY KEY CLUSTERED 
(
	[jobId] ASC,
	[userId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_assignments]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_assignments](
	[jobId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[salary] [int] NOT NULL,
	[assignedBy] [int] NOT NULL,
 CONSTRAINT [PK_job_assignments] PRIMARY KEY CLUSTERED 
(
	[jobId] ASC,
	[userId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_type_assign_capable_users]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_type_assign_capable_users](
	[jobTypeId] [int] NOT NULL,
	[userId] [int] NOT NULL,
 CONSTRAINT [PK_job_type_assign_capable_users] PRIMARY KEY CLUSTERED 
(
	[jobTypeId] ASC,
	[userId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[job_types]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[job_types](
	[jobTypeId] [int] IDENTITY(1,1) NOT NULL,
	[jobTypeName] [nvarchar](50) NOT NULL,
	[campaignId] [int] NOT NULL,
	[jobTypeDescription] [nvarchar](200) NULL,
	[isCustomJobType] [bit] NOT NULL,
 CONSTRAINT [PK_job_types] PRIMARY KEY CLUSTERED 
(
	[jobTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[jobs]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[jobs](
	[jobId] [int] IDENTITY(1,1) NOT NULL,
	[campaignId] [int] NOT NULL,
	[jobGuid] [uniqueidentifier] NOT NULL,
	[jobName] [nvarchar](50) NOT NULL,
	[jobDescription] [nvarchar](200) NULL,
	[jobLocation] [nvarchar](100) NULL,
	[jobStartTime] [datetime] NULL,
	[jobEndTime] [datetime] NULL,
	[jobDefaultSalary] [int] NOT NULL,
	[peopleNeeded] [int] NOT NULL,
	[peopleAssigned] [int] NOT NULL,
	[jobTypeId] [int] NOT NULL,
 CONSTRAINT [PK_jobs] PRIMARY KEY CLUSTERED 
(
	[jobId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[parties]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[parties](
	[partyId] [int] IDENTITY(1,1) NOT NULL,
	[partyLetter] [nvarchar](5) NULL,
	[campaignId] [int] NOT NULL,
	[partyName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_parties] PRIMARY KEY CLUSTERED 
(
	[partyId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[party_platforms]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[party_platforms](
	[partyName] [nvarchar](100) NOT NULL,
	[partyPlatform] [nvarchar](max) NOT NULL,
	[electionYear] [nvarchar](20) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[party_profiles]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[party_profiles](
	[partyName] [nvarchar](50) NOT NULL,
	[politicalLeaning] [decimal](6, 5) NOT NULL,
	[religious] [decimal](6, 5) NOT NULL,
	[economicPolicy] [decimal](6, 5) NOT NULL,
	[zionist] [decimal](6, 5) NOT NULL,
	[arab] [decimal](6, 5) NOT NULL,
	[securityPolicy] [decimal](6, 5) NOT NULL,
	[settlements] [decimal](6, 5) NOT NULL,
	[immigration] [decimal](6, 5) NOT NULL,
	[socialPolicy] [decimal](6, 5) NOT NULL,
	[education] [decimal](6, 5) NOT NULL,
	[environmentalPolicy] [decimal](6, 5) NOT NULL,
	[rightsForLGBTQ] [decimal](6, 5) NOT NULL,
	[foreignPolicy] [decimal](6, 5) NOT NULL,
	[laborPolicy] [decimal](6, 5) NOT NULL,
	[ageDemographics] [decimal](6, 5) NOT NULL,
	[gender] [decimal](6, 5) NOT NULL,
	[ethnicityFocused] [decimal](6, 5) NOT NULL,
	[immigrationStatus] [decimal](6, 5) NOT NULL,
	[nationalism] [decimal](6, 5) NOT NULL,
	[politicalSystem] [decimal](6, 5) NOT NULL,
	[corruption] [decimal](6, 5) NOT NULL,
	[politicalExperience] [decimal](6, 5) NOT NULL,
	[seperationOfPowers] [decimal](6, 5) NOT NULL,
	[ruralIssues] [decimal](6, 5) NOT NULL,
	[publicTransporation] [decimal](6, 5) NOT NULL,
 CONSTRAINT [PK_party_profiles] PRIMARY KEY CLUSTERED 
(
	[partyName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[permission_sets]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[permission_sets](
	[userId] [int] NOT NULL,
	[campaignId] [int] NOT NULL,
	[permissionId] [int] NOT NULL,
 CONSTRAINT [PK_permission_sets] PRIMARY KEY CLUSTERED 
(
	[userId] ASC,
	[campaignId] ASC,
	[permissionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[phone_verification_codes]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[phone_verification_codes](
	[userId] [int] NOT NULL,
	[phoneNumber] [nvarchar](20) NOT NULL,
	[verificationCode] [nvarchar](6) NOT NULL,
	[expires] [datetime] NOT NULL,
 CONSTRAINT [PK_phone_verification_codes] PRIMARY KEY CLUSTERED 
(
	[userId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[public_board_announcements]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[public_board_announcements](
	[announcementId] [int] IDENTITY(1,1) NOT NULL,
	[announcementContent] [nvarchar](4000) NOT NULL,
	[publisherId] [int] NOT NULL,
	[publishingDate] [datetime] NOT NULL,
	[campaignId] [int] NOT NULL,
	[announcementTitle] [nvarchar](100) NOT NULL,
	[announcementGuid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_public_board_announcements] PRIMARY KEY CLUSTERED 
(
	[announcementId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[public_board_events]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[public_board_events](
	[eventId] [int] NOT NULL,
	[publishingDate] [datetime] NOT NULL,
	[publisherId] [int] NOT NULL,
 CONSTRAINT [PK_public_board_events] PRIMARY KEY CLUSTERED 
(
	[eventId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[roles](
	[roleId] [int] IDENTITY(1,1) NOT NULL,
	[roleName] [nvarchar](50) NOT NULL,
	[campaignId] [int] NOT NULL,
	[roleDescription] [nvarchar](150) NULL,
	[roleLevel] [int] NOT NULL,
	[isCustomRole] [bit] NOT NULL,
 CONSTRAINT [PK_roles] PRIMARY KEY CLUSTERED 
(
	[roleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sms_messages]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sms_messages](
	[messageId] [int] IDENTITY(1,1) NOT NULL,
	[messageGuid] [uniqueidentifier] NOT NULL,
	[messageContents] [nvarchar](500) NOT NULL,
	[campaignId] [int] NOT NULL,
	[senderId] [int] NOT NULL,
	[messageDate] [datetime] NOT NULL,
	[sentCount] [int] NOT NULL,
 CONSTRAINT [PK_sms_messages] PRIMARY KEY CLUSTERED 
(
	[messageId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sms_messages_phone_numbers]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sms_messages_phone_numbers](
	[messageId] [int] NOT NULL,
	[phoneNumber] [nvarchar](20) NOT NULL,
	[isSuccess] [bit] NOT NULL,
 CONSTRAINT [PK_sms_messages_phone_numbers] PRIMARY KEY CLUSTERED 
(
	[messageId] ASC,
	[phoneNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[streets]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[streets](
	[cityId] [smallint] NOT NULL,
	[streetId] [smallint] NOT NULL,
	[streetName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_streets] PRIMARY KEY CLUSTERED 
(
	[cityId] ASC,
	[streetId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_work_preferences]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_work_preferences](
	[userId] [int] NOT NULL,
	[campaignId] [int] NOT NULL,
	[userPreferencesText] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_user_work_preferences] PRIMARY KEY CLUSTERED 
(
	[userId] ASC,
	[campaignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[userId] [int] IDENTITY(1,1) NOT NULL,
	[email] [nvarchar](200) NOT NULL,
	[firstNameEng] [nvarchar](50) NULL,
	[lastNameEng] [nvarchar](50) NULL,
	[idNum] [int] NULL,
	[displayNameEng] [nvarchar](100) NULL,
	[profilePicUrl] [nvarchar](2083) NULL,
	[firstNameHeb] [nvarchar](50) NULL,
	[lastNameHeb] [nvarchar](50) NULL,
	[authenticated] [bit] NOT NULL,
	[phoneNumber] [nvarchar](20) NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[userId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users_events]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users_events](
	[userId] [int] NOT NULL,
	[eventId] [int] NOT NULL,
 CONSTRAINT [PK_users_events] PRIMARY KEY CLUSTERED 
(
	[userId] ASC,
	[eventId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users_notified_on_publish]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users_notified_on_publish](
	[userId] [int] NOT NULL,
	[campaignId] [int] NOT NULL,
	[viaSms] [bit] NOT NULL,
	[viaEmail] [bit] NOT NULL,
 CONSTRAINT [PK_users_notified_on_publish] PRIMARY KEY CLUSTERED 
(
	[userId] ASC,
	[campaignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users_public_board_preferences]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users_public_board_preferences](
	[userId] [int] NOT NULL,
	[campaignId] [int] NOT NULL,
	[isPreferred] [bit] NOT NULL,
 CONSTRAINT [PK_users_public_board_preferences] PRIMARY KEY CLUSTERED 
(
	[userId] ASC,
	[campaignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users_schedule_managers]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users_schedule_managers](
	[permissionGiver] [int] NOT NULL,
	[permissionReceiver] [int] NOT NULL,
 CONSTRAINT [PK_users_personal_schedule_management_capable_users] PRIMARY KEY CLUSTERED 
(
	[permissionGiver] ASC,
	[permissionReceiver] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users_to_notify_on_join]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users_to_notify_on_join](
	[userId] [int] NOT NULL,
	[campaignId] [int] NOT NULL,
	[viaEmail] [bit] NOT NULL,
	[viaSms] [bit] NOT NULL,
 CONSTRAINT [PK_users_to_notify_on_join] PRIMARY KEY CLUSTERED 
(
	[userId] ASC,
	[campaignId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[votes]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[votes](
	[campaignId] [int] NOT NULL,
	[ballotId] [int] NOT NULL,
	[isCustomBallot] [bit] NOT NULL,
	[partyId] [int] NOT NULL,
	[numVotes] [int] NOT NULL,
 CONSTRAINT [PK_votes] PRIMARY KEY CLUSTERED 
(
	[campaignId] ASC,
	[ballotId] ASC,
	[isCustomBallot] ASC,
	[partyId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ballotLocation_ballots]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [ballotLocation_ballots] ON [dbo].[ballots]
(
	[ballotLocation] ASC
)
INCLUDE([innerCityBallotId],[ballotAddress],[accessible],[elligibleVoters]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [cityId_ballots]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [cityId_ballots] ON [dbo].[ballots]
(
	[cityId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ballots_cityId_innerCityBallotId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_ballots_cityId_innerCityBallotId] ON [dbo].[ballots]
(
	[cityId] ASC,
	[innerCityBallotId] ASC
)
INCLUDE([ballotId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_campaign_advisor_results_overview_campaignId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_campaign_advisor_results_overview_campaignId] ON [dbo].[campaign_advisor_results_overview]
(
	[campaignId] ASC
)
INCLUDE([resultsId],[timePerformed],[resultsGuid],[resultsTitle],[analysisTarget],[targetTwitterHandle],[maxDaysBack],[gptResponse],[additionalUserRequests]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_campaign_advisor_results_overview_resultsGuid_unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_campaign_advisor_results_overview_resultsGuid_unique] ON [dbo].[campaign_advisor_results_overview]
(
	[resultsGuid] ASC
)
INCLUDE([resultsId],[timePerformed],[campaignId],[resultsTitle],[analysisTarget],[targetTwitterHandle],[maxDaysBack],[gptResponse],[additionalUserRequests]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_campaigns_campaignGuid_Unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_campaigns_campaignGuid_Unique] ON [dbo].[campaigns]
(
	[campaignGuid] ASC
)
INCLUDE([campaignId],[campaignName],[campaignInviteGuid],[campaignLogoUrl],[campaignCreatorUserId],[campaignDescription],[campaignCreationDate],[campaignIsActive],[isMunicipal],[isSubCampaign]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_campaigns_campaignInviteGuid]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_campaigns_campaignInviteGuid] ON [dbo].[campaigns]
(
	[campaignInviteGuid] ASC
)
INCLUDE([campaignId],[campaignName],[campaignGuid],[campaignLogoUrl]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_cities_cityName_Unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_cities_cityName_Unique] ON [dbo].[cities]
(
	[cityName] ASC
)
INCLUDE([cityId]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_custom_ballots_ballot_id]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_custom_ballots_ballot_id] ON [dbo].[custom_ballots]
(
	[ballotId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_custom_events_eventGuid_unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_custom_events_eventGuid_unique] ON [dbo].[custom_events]
(
	[eventGuid] ASC
)
INCLUDE([eventId],[eventName],[eventDescription],[eventStartTime],[eventEndTime],[eventOf],[campaignId],[maxAttendees],[numAttending],[eventCreatorId],[eventLocation],[isOpenJoin]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_custom_voters_ledgers_campaignId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_custom_voters_ledgers_campaignId] ON [dbo].[custom_voters_ledgers]
(
	[campaignId] ASC
)
INCLUDE([ledgerId],[ledgerGuid],[ledgerName]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_custom_voters_ledgers_ledgerGuid]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_custom_voters_ledgers_ledgerGuid] ON [dbo].[custom_voters_ledgers]
(
	[ledgerGuid] ASC
)
INCLUDE([ledgerId],[campaignId],[ledgerName]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_permissions_permissionType_permissionTarget_unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_permissions_permissionType_permissionTarget_unique] ON [dbo].[defined_permissions]
(
	[permissionType] ASC,
	[permissionTarget] ASC
)
INCLUDE([permissionId]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_election_results_partyName_electionYear]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_election_results_partyName_electionYear] ON [dbo].[election_results]
(
	[partyName] ASC,
	[electionYear] ASC
)
INCLUDE([cityId],[ballotId],[votes]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_financial_data_campaignId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_financial_data_campaignId] ON [dbo].[financial_data]
(
	[campaignId] ASC
)
INCLUDE([amount],[financialType],[isExpense],[dataGuid],[dateCreated],[dataTitle],[dataDescription],[creatorUserId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_financial_data_dateCreated]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_financial_data_dateCreated] ON [dbo].[financial_data]
(
	[dateCreated] ASC
)
INCLUDE([amount],[campaignId],[isExpense],[dataGuid],[financialType],[dataTitle],[dataDescription],[creatorUserId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_financial_data_financialType]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_financial_data_financialType] ON [dbo].[financial_data]
(
	[financialType] ASC
)
INCLUDE([amount],[campaignId],[isExpense],[dataGuid],[dateCreated],[dataTitle],[dataDescription],[creatorUserId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_financial_types_campaignId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_financial_types_campaignId] ON [dbo].[financial_types]
(
	[campaignId] ASC
)
INCLUDE([typeId],[typeName],[typeGuid],[typeDescription]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_financial_types_typeGuid_unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_financial_types_typeGuid_unique] ON [dbo].[financial_types]
(
	[typeGuid] ASC
)
INCLUDE([typeId],[typeName],[campaignId],[typeDescription]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_job_types_campaignId_jobTypeName_unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_job_types_campaignId_jobTypeName_unique] ON [dbo].[job_types]
(
	[campaignId] ASC,
	[jobTypeName] ASC
)
INCLUDE([jobTypeId],[jobTypeDescription],[isCustomJobType]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_job_campaignId_jobTypeId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_job_campaignId_jobTypeId] ON [dbo].[jobs]
(
	[campaignId] ASC,
	[jobTypeId] ASC
)
INCLUDE([jobId],[jobGuid],[jobName],[jobDescription],[jobLocation],[jobStartTime],[jobEndTime],[jobDefaultSalary],[peopleNeeded],[peopleAssigned]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_jobs_campaignId_jobEndTime]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_jobs_campaignId_jobEndTime] ON [dbo].[jobs]
(
	[campaignId] ASC,
	[jobEndTime] ASC
)
INCLUDE([jobId],[jobDescription],[jobName],[jobStartTime],[jobLocation],[jobDefaultSalary],[peopleNeeded],[peopleAssigned],[jobGuid],[jobTypeId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_jobs_campaignId_jobGuid_unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_jobs_campaignId_jobGuid_unique] ON [dbo].[jobs]
(
	[campaignId] ASC,
	[jobGuid] ASC
)
INCLUDE([jobId],[jobName],[jobDescription],[jobLocation],[jobStartTime],[jobEndTime],[jobDefaultSalary],[peopleNeeded],[peopleAssigned],[jobTypeId]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_jobs_campaignId_jobLocation]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_jobs_campaignId_jobLocation] ON [dbo].[jobs]
(
	[campaignId] ASC,
	[jobLocation] ASC
)
INCLUDE([jobId],[jobDescription],[jobName],[jobStartTime],[jobEndTime],[jobDefaultSalary],[peopleNeeded],[peopleAssigned],[jobGuid],[jobTypeId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_jobs_campaignId_jobName]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_jobs_campaignId_jobName] ON [dbo].[jobs]
(
	[campaignId] ASC,
	[jobName] ASC
)
INCLUDE([jobId],[jobDescription],[jobLocation],[jobStartTime],[jobEndTime],[jobDefaultSalary],[peopleNeeded],[peopleAssigned],[jobGuid],[jobTypeId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_jobs_campaignId_jobStartTime]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_jobs_campaignId_jobStartTime] ON [dbo].[jobs]
(
	[campaignId] ASC,
	[jobStartTime] ASC
)
INCLUDE([jobId],[jobDescription],[jobName],[jobLocation],[jobEndTime],[jobDefaultSalary],[peopleNeeded],[peopleAssigned],[jobGuid],[jobTypeId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_jobs_campaignId_peopleNeeded_peopleAssigned]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_jobs_campaignId_peopleNeeded_peopleAssigned] ON [dbo].[jobs]
(
	[campaignId] ASC,
	[peopleNeeded] ASC,
	[peopleAssigned] ASC
)
INCLUDE([jobId],[jobDescription],[jobName],[jobStartTime],[jobEndTime],[jobDefaultSalary],[jobLocation],[jobGuid],[jobTypeId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_party_platforms_partyName_electionYear]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_party_platforms_partyName_electionYear] ON [dbo].[party_platforms]
(
	[partyName] ASC,
	[electionYear] ASC
)
INCLUDE([partyPlatform]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_public_board_announcements_announcementGuid_Unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_public_board_announcements_announcementGuid_Unique] ON [dbo].[public_board_announcements]
(
	[announcementGuid] ASC
)
INCLUDE([announcementId],[announcementTitle],[announcementContent],[publishingDate],[publisherId],[campaignId]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [roles_roleName_campaignId_unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [roles_roleName_campaignId_unique] ON [dbo].[roles]
(
	[roleName] ASC,
	[campaignId] ASC
)
INCLUDE([roleId],[roleLevel],[roleDescription]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_sms_messages_messageDate]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_sms_messages_messageDate] ON [dbo].[sms_messages]
(
	[messageDate] ASC
)
INCLUDE([messageId],[messageContents],[campaignId],[messageGuid],[senderId],[sentCount]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_sms_messages_messageGuid_unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_sms_messages_messageGuid_unique] ON [dbo].[sms_messages]
(
	[messageGuid] ASC
)
INCLUDE([messageId],[messageContents],[campaignId],[senderId],[messageDate],[sentCount]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_sms_messages_senderId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_sms_messages_senderId] ON [dbo].[sms_messages]
(
	[senderId] ASC
)
INCLUDE([messageId],[messageContents],[campaignId],[messageGuid],[messageDate],[sentCount]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_streets_streetName_cityId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_streets_streetName_cityId] ON [dbo].[streets]
(
	[streetName] ASC,
	[cityId] ASC
)
INCLUDE([streetId]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_users_email_unique]    Script Date: 26/05/2024 16:14:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_users_email_unique] ON [dbo].[users]
(
	[email] ASC
)
INCLUDE([userId],[firstNameEng],[lastNameEng],[idNum],[displayNameEng],[profilePicUrl],[firstNameHeb],[lastNameHeb],[authenticated]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_users_firstNameEng]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_users_firstNameEng] ON [dbo].[users]
(
	[firstNameEng] ASC
)
INCLUDE([firstNameHeb],[lastNameEng],[displayNameEng],[profilePicUrl],[email],[phoneNumber],[lastNameHeb]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_users_firstNameHeb]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_users_firstNameHeb] ON [dbo].[users]
(
	[firstNameHeb] ASC
)
INCLUDE([firstNameEng],[lastNameEng],[displayNameEng],[profilePicUrl],[email],[phoneNumber],[lastNameHeb]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_users_idNum]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_users_idNum] ON [dbo].[users]
(
	[idNum] ASC
)
INCLUDE([userId],[authenticated]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_users_lastNameEng]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_users_lastNameEng] ON [dbo].[users]
(
	[lastNameEng] ASC
)
INCLUDE([firstNameHeb],[firstNameEng],[displayNameEng],[profilePicUrl],[email],[phoneNumber],[lastNameHeb]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_users_lastNameHeb]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_users_lastNameHeb] ON [dbo].[users]
(
	[lastNameHeb] ASC
)
INCLUDE([firstNameHeb],[lastNameEng],[displayNameEng],[profilePicUrl],[email],[phoneNumber],[firstNameEng]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_users_phoneNumber]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_users_phoneNumber] ON [dbo].[users]
(
	[phoneNumber] ASC
)
INCLUDE([firstNameHeb],[lastNameEng],[displayNameEng],[profilePicUrl],[email],[firstNameEng],[lastNameHeb]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_voters_ledger_ballotId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_voters_ledger_ballotId] ON [dbo].[voters_ledger]
(
	[ballotId] ASC
)
INCLUDE([lastName],[cityId],[fathersName],[firstName],[spare1],[residenceId],[residenceName],[spare2],[streetId],[streetName],[houseNumber],[entrance],[appartment],[houseLetter],[ballotSerial],[spare3],[spare4],[zipCode],[spare5]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_voters_ledger_cityId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_voters_ledger_cityId] ON [dbo].[voters_ledger]
(
	[cityId] ASC
)
INCLUDE([lastName],[firstName],[fathersName],[ballotId],[spare1],[residenceId],[residenceName],[spare2],[streetId],[streetName],[houseNumber],[entrance],[appartment],[houseLetter],[ballotSerial],[spare3],[spare4],[zipCode],[spare5]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_voters_ledger_cityId_streetId]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_voters_ledger_cityId_streetId] ON [dbo].[voters_ledger]
(
	[cityId] ASC,
	[streetId] ASC
)
INCLUDE([lastName],[firstName],[fathersName],[ballotId],[spare1],[residenceId],[residenceName],[spare2],[streetName],[houseNumber],[entrance],[appartment],[houseLetter],[ballotSerial],[spare3],[spare4],[zipCode],[spare5]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_voters_ledger_firstName]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_voters_ledger_firstName] ON [dbo].[voters_ledger]
(
	[firstName] ASC
)
INCLUDE([lastName],[cityId],[fathersName],[ballotId],[spare1],[residenceId],[residenceName],[spare2],[streetId],[streetName],[houseNumber],[entrance],[appartment],[houseLetter],[ballotSerial],[spare3],[spare4],[zipCode],[spare5]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_voters_ledger_lastName]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_voters_ledger_lastName] ON [dbo].[voters_ledger]
(
	[lastName] ASC
)
INCLUDE([firstName],[cityId],[fathersName],[ballotId],[spare1],[residenceId],[residenceName],[spare2],[streetId],[streetName],[houseNumber],[entrance],[appartment],[houseLetter],[ballotSerial],[spare3],[spare4],[zipCode],[spare5]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_voters_ledger_dynamic_phone1]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_voters_ledger_dynamic_phone1] ON [dbo].[voters_ledger_dynamic]
(
	[phone1] ASC
)
INCLUDE([IdNum],[email1],[email2],[phone2]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_voters_ledger_dynamic_phone2]    Script Date: 26/05/2024 16:14:08 ******/
CREATE NONCLUSTERED INDEX [IX_voters_ledger_dynamic_phone2] ON [dbo].[voters_ledger_dynamic]
(
	[phone2] ASC
)
INCLUDE([IdNum],[email1],[email2],[phone1]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[campaign_advisor_analysis_samples] ADD  CONSTRAINT [DF_campaign_advisor_analysis_samples_isArticle]  DEFAULT ((1)) FOR [isArticle]
GO
ALTER TABLE [dbo].[campaign_advisor_results_overview] ADD  CONSTRAINT [DF_campaign_advisor_results_overview_maxDaysBack]  DEFAULT ((0)) FOR [maxDaysBack]
GO
ALTER TABLE [dbo].[campaign_users] ADD  CONSTRAINT [DF_campaign_users_roleId]  DEFAULT ((1)) FOR [roleId]
GO
ALTER TABLE [dbo].[campaigns] ADD  CONSTRAINT [DF_campaigns_isMunicipal]  DEFAULT ((1)) FOR [isMunicipal]
GO
ALTER TABLE [dbo].[campaigns] ADD  CONSTRAINT [DF_campaigns_isSubCampaign]  DEFAULT ((0)) FOR [isSubCampaign]
GO
ALTER TABLE [dbo].[campaigns] ADD  CONSTRAINT [DF_campaigns_cityId]  DEFAULT ((-1)) FOR [cityId]
GO
ALTER TABLE [dbo].[campaigns] ADD  CONSTRAINT [DF_campaigns_isCustomCampaign]  DEFAULT ((0)) FOR [isCustomCampaign]
GO
ALTER TABLE [dbo].[custom_events] ADD  CONSTRAINT [DF_custom_events_numAttending]  DEFAULT ((0)) FOR [numAttending]
GO
ALTER TABLE [dbo].[custom_events] ADD  CONSTRAINT [DF_custom_events_isFreeToJoin]  DEFAULT ((0)) FOR [isOpenJoin]
GO
ALTER TABLE [dbo].[custom_events] ADD  CONSTRAINT [DF_custom_events_eventOf]  DEFAULT ((0)) FOR [eventOf]
GO
ALTER TABLE [dbo].[financial_data] ADD  CONSTRAINT [DF_financial_data_amount]  DEFAULT ((1)) FOR [amount]
GO
ALTER TABLE [dbo].[financial_data] ADD  CONSTRAINT [DF_financial_data_financialType]  DEFAULT ((1)) FOR [financialType]
GO
ALTER TABLE [dbo].[financial_data] ADD  CONSTRAINT [DF_financial_data_isExpense]  DEFAULT ((0)) FOR [isExpense]
GO
ALTER TABLE [dbo].[job_types] ADD  CONSTRAINT [DF_job_types_customJobType]  DEFAULT ((1)) FOR [isCustomJobType]
GO
ALTER TABLE [dbo].[jobs] ADD  CONSTRAINT [DF_jobs_jobDefaultSalary]  DEFAULT ((0)) FOR [jobDefaultSalary]
GO
ALTER TABLE [dbo].[jobs] ADD  CONSTRAINT [DF_jobs_peopleNeeded]  DEFAULT ((1)) FOR [peopleNeeded]
GO
ALTER TABLE [dbo].[jobs] ADD  CONSTRAINT [DF_jobs_peopleAssigned]  DEFAULT ((0)) FOR [peopleAssigned]
GO
ALTER TABLE [dbo].[jobs] ADD  CONSTRAINT [DF_jobs_jobTypeId]  DEFAULT ((1)) FOR [jobTypeId]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_politicalLeaning]  DEFAULT ((0)) FOR [politicalLeaning]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_religious]  DEFAULT ((0)) FOR [religious]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_economicPolicy]  DEFAULT ((0)) FOR [economicPolicy]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_zionist]  DEFAULT ((0)) FOR [zionist]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_arab]  DEFAULT ((0)) FOR [arab]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_securityPolicy]  DEFAULT ((0)) FOR [securityPolicy]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_settlements]  DEFAULT ((0)) FOR [settlements]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_immigration]  DEFAULT ((0)) FOR [immigration]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_socialPolicy]  DEFAULT ((0)) FOR [socialPolicy]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_education]  DEFAULT ((0)) FOR [education]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_environmentalPolicy]  DEFAULT ((0)) FOR [environmentalPolicy]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_rightsForLGBTQ]  DEFAULT ((0)) FOR [rightsForLGBTQ]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_foreignPolicy]  DEFAULT ((0)) FOR [foreignPolicy]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_laborPolicy]  DEFAULT ((0)) FOR [laborPolicy]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_ageDemographics]  DEFAULT ((0)) FOR [ageDemographics]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_gender]  DEFAULT ((0)) FOR [gender]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_ethnicityFocused]  DEFAULT ((0)) FOR [ethnicityFocused]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_immigrationStatus]  DEFAULT ((0)) FOR [immigrationStatus]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_nationalism]  DEFAULT ((0)) FOR [nationalism]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_politicalSystem]  DEFAULT ((0)) FOR [politicalSystem]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_corruption]  DEFAULT ((0)) FOR [corruption]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_politicalExperience]  DEFAULT ((0)) FOR [politicalExperience]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_seperationOfPowers]  DEFAULT ((0)) FOR [seperationOfPowers]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_ruralIssues]  DEFAULT ((0)) FOR [ruralIssues]
GO
ALTER TABLE [dbo].[party_profiles] ADD  CONSTRAINT [DF_party_profiles_publicTransporation]  DEFAULT ((0)) FOR [publicTransporation]
GO
ALTER TABLE [dbo].[roles] ADD  CONSTRAINT [DF_roles_roleLevel]  DEFAULT ((0)) FOR [roleLevel]
GO
ALTER TABLE [dbo].[roles] ADD  CONSTRAINT [DF_roles_customUserRole]  DEFAULT ((0)) FOR [isCustomRole]
GO
ALTER TABLE [dbo].[sms_messages] ADD  CONSTRAINT [DF_sms_messages_sentCount]  DEFAULT ((0)) FOR [sentCount]
GO
ALTER TABLE [dbo].[sms_messages_phone_numbers] ADD  CONSTRAINT [DF_sms_messages_phone_numbers_isSuccess]  DEFAULT ((1)) FOR [isSuccess]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_authenticated]  DEFAULT ((0)) FOR [authenticated]
GO
ALTER TABLE [dbo].[users_notified_on_publish] ADD  CONSTRAINT [DF_users_notified_on_publish_viaSms]  DEFAULT ((0)) FOR [viaSms]
GO
ALTER TABLE [dbo].[users_notified_on_publish] ADD  CONSTRAINT [DF_users_notified_on_publish_viaEmail]  DEFAULT ((0)) FOR [viaEmail]
GO
ALTER TABLE [dbo].[users_public_board_preferences] ADD  CONSTRAINT [DF_users_public_board_preferences_campaignId]  DEFAULT ((1)) FOR [campaignId]
GO
ALTER TABLE [dbo].[users_to_notify_on_join] ADD  CONSTRAINT [DF_users_to_notify_on_join_email]  DEFAULT ((0)) FOR [viaEmail]
GO
ALTER TABLE [dbo].[users_to_notify_on_join] ADD  CONSTRAINT [DF_users_to_notify_on_join_sms]  DEFAULT ((0)) FOR [viaSms]
GO
ALTER TABLE [dbo].[ballots]  WITH CHECK ADD  CONSTRAINT [FK_ballots_cities] FOREIGN KEY([cityId])
REFERENCES [dbo].[cities] ([cityId])
GO
ALTER TABLE [dbo].[ballots] CHECK CONSTRAINT [FK_ballots_cities]
GO
ALTER TABLE [dbo].[campaign_advisor_analysis_samples]  WITH CHECK ADD  CONSTRAINT [FK_campaign_advisor_analysis_samples_campaign_advisor_analysis_samples] FOREIGN KEY([resultsId])
REFERENCES [dbo].[campaign_advisor_results_overview] ([resultsId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[campaign_advisor_analysis_samples] CHECK CONSTRAINT [FK_campaign_advisor_analysis_samples_campaign_advisor_analysis_samples]
GO
ALTER TABLE [dbo].[campaign_advisor_results_details]  WITH CHECK ADD  CONSTRAINT [FK_campaign_advisor_results_details_campaign_advisor_results_overview] FOREIGN KEY([resultsId])
REFERENCES [dbo].[campaign_advisor_results_overview] ([resultsId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[campaign_advisor_results_details] CHECK CONSTRAINT [FK_campaign_advisor_results_details_campaign_advisor_results_overview]
GO
ALTER TABLE [dbo].[campaign_users]  WITH CHECK ADD  CONSTRAINT [FK_campaign_users_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[campaign_users] CHECK CONSTRAINT [FK_campaign_users_campaigns]
GO
ALTER TABLE [dbo].[campaign_users]  WITH CHECK ADD  CONSTRAINT [FK_campaign_users_roles] FOREIGN KEY([roleId])
REFERENCES [dbo].[roles] ([roleId])
ON UPDATE CASCADE
ON DELETE SET DEFAULT
GO
ALTER TABLE [dbo].[campaign_users] CHECK CONSTRAINT [FK_campaign_users_roles]
GO
ALTER TABLE [dbo].[campaign_users]  WITH CHECK ADD  CONSTRAINT [FK_campaign_users_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[campaign_users] CHECK CONSTRAINT [FK_campaign_users_users]
GO
ALTER TABLE [dbo].[campaigns]  WITH CHECK ADD  CONSTRAINT [FK_campaigns_campaigns] FOREIGN KEY([cityId])
REFERENCES [dbo].[cities] ([cityId])
GO
ALTER TABLE [dbo].[campaigns] CHECK CONSTRAINT [FK_campaigns_campaigns]
GO
ALTER TABLE [dbo].[custom_ballots]  WITH CHECK ADD  CONSTRAINT [FK_custom_ballots_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[custom_ballots] CHECK CONSTRAINT [FK_custom_ballots_campaigns]
GO
ALTER TABLE [dbo].[custom_events]  WITH CHECK ADD  CONSTRAINT [FK_custom_events_users] FOREIGN KEY([eventOf])
REFERENCES [dbo].[users] ([userId])
GO
ALTER TABLE [dbo].[custom_events] CHECK CONSTRAINT [FK_custom_events_users]
GO
ALTER TABLE [dbo].[custom_voters_ledgers]  WITH CHECK ADD  CONSTRAINT [FK_custom_voters_ledgers_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[custom_voters_ledgers] CHECK CONSTRAINT [FK_custom_voters_ledgers_campaigns]
GO
ALTER TABLE [dbo].[custom_voters_ledgers_content]  WITH CHECK ADD  CONSTRAINT [FK_custom_voters_ledgers_content_custom_voters_ledgers] FOREIGN KEY([ledgerId])
REFERENCES [dbo].[custom_voters_ledgers] ([ledgerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[custom_voters_ledgers_content] CHECK CONSTRAINT [FK_custom_voters_ledgers_content_custom_voters_ledgers]
GO
ALTER TABLE [dbo].[events_watchers]  WITH CHECK ADD  CONSTRAINT [FK_events_watchers_custom_events] FOREIGN KEY([eventId])
REFERENCES [dbo].[custom_events] ([eventId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[events_watchers] CHECK CONSTRAINT [FK_events_watchers_custom_events]
GO
ALTER TABLE [dbo].[events_watchers]  WITH CHECK ADD  CONSTRAINT [FK_events_watchers_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[events_watchers] CHECK CONSTRAINT [FK_events_watchers_users]
GO
ALTER TABLE [dbo].[financial_data]  WITH CHECK ADD  CONSTRAINT [FK_financial_data_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
GO
ALTER TABLE [dbo].[financial_data] CHECK CONSTRAINT [FK_financial_data_campaigns]
GO
ALTER TABLE [dbo].[financial_data]  WITH CHECK ADD  CONSTRAINT [FK_financial_data_financial_types] FOREIGN KEY([financialType])
REFERENCES [dbo].[financial_types] ([typeId])
ON DELETE SET DEFAULT
GO
ALTER TABLE [dbo].[financial_data] CHECK CONSTRAINT [FK_financial_data_financial_types]
GO
ALTER TABLE [dbo].[financial_data]  WITH CHECK ADD  CONSTRAINT [FK_financial_data_users] FOREIGN KEY([creatorUserId])
REFERENCES [dbo].[users] ([userId])
GO
ALTER TABLE [dbo].[financial_data] CHECK CONSTRAINT [FK_financial_data_users]
GO
ALTER TABLE [dbo].[financial_types]  WITH CHECK ADD  CONSTRAINT [FK_financial_types_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[financial_types] CHECK CONSTRAINT [FK_financial_types_campaigns]
GO
ALTER TABLE [dbo].[job_assign_capable_users]  WITH CHECK ADD  CONSTRAINT [FK_users_who_can_assign_to_jobs_jobs] FOREIGN KEY([jobId])
REFERENCES [dbo].[jobs] ([jobId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[job_assign_capable_users] CHECK CONSTRAINT [FK_users_who_can_assign_to_jobs_jobs]
GO
ALTER TABLE [dbo].[job_assign_capable_users]  WITH CHECK ADD  CONSTRAINT [FK_users_who_can_assign_to_jobs_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[job_assign_capable_users] CHECK CONSTRAINT [FK_users_who_can_assign_to_jobs_users]
GO
ALTER TABLE [dbo].[job_assignments]  WITH CHECK ADD  CONSTRAINT [FK_job_assignments_job_assignments] FOREIGN KEY([jobId], [userId])
REFERENCES [dbo].[job_assignments] ([jobId], [userId])
GO
ALTER TABLE [dbo].[job_assignments] CHECK CONSTRAINT [FK_job_assignments_job_assignments]
GO
ALTER TABLE [dbo].[job_assignments]  WITH CHECK ADD  CONSTRAINT [FK_job_assignments_jobs] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[job_assignments] CHECK CONSTRAINT [FK_job_assignments_jobs]
GO
ALTER TABLE [dbo].[job_assignments]  WITH CHECK ADD  CONSTRAINT [FK_job_assignments_users] FOREIGN KEY([assignedBy])
REFERENCES [dbo].[users] ([userId])
GO
ALTER TABLE [dbo].[job_assignments] CHECK CONSTRAINT [FK_job_assignments_users]
GO
ALTER TABLE [dbo].[job_type_assign_capable_users]  WITH CHECK ADD  CONSTRAINT [FK_job_type_assign_capable_users_job_types] FOREIGN KEY([jobTypeId])
REFERENCES [dbo].[job_types] ([jobTypeId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[job_type_assign_capable_users] CHECK CONSTRAINT [FK_job_type_assign_capable_users_job_types]
GO
ALTER TABLE [dbo].[job_type_assign_capable_users]  WITH CHECK ADD  CONSTRAINT [FK_job_type_assign_capable_users_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[job_type_assign_capable_users] CHECK CONSTRAINT [FK_job_type_assign_capable_users_users]
GO
ALTER TABLE [dbo].[jobs]  WITH CHECK ADD  CONSTRAINT [FK_jobs_campaigns] FOREIGN KEY([jobTypeId])
REFERENCES [dbo].[job_types] ([jobTypeId])
ON DELETE SET DEFAULT
GO
ALTER TABLE [dbo].[jobs] CHECK CONSTRAINT [FK_jobs_campaigns]
GO
ALTER TABLE [dbo].[parties]  WITH CHECK ADD  CONSTRAINT [FK_parties_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[parties] CHECK CONSTRAINT [FK_parties_campaigns]
GO
ALTER TABLE [dbo].[permission_sets]  WITH CHECK ADD  CONSTRAINT [FK_permission_sets_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[permission_sets] CHECK CONSTRAINT [FK_permission_sets_campaigns]
GO
ALTER TABLE [dbo].[phone_verification_codes]  WITH CHECK ADD  CONSTRAINT [FK_phone_verification_codes_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[phone_verification_codes] CHECK CONSTRAINT [FK_phone_verification_codes_users]
GO
ALTER TABLE [dbo].[public_board_announcements]  WITH CHECK ADD  CONSTRAINT [FK_public_board_announcements_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
GO
ALTER TABLE [dbo].[public_board_announcements] CHECK CONSTRAINT [FK_public_board_announcements_campaigns]
GO
ALTER TABLE [dbo].[public_board_events]  WITH CHECK ADD  CONSTRAINT [FK_public_board_events_custom_events] FOREIGN KEY([eventId])
REFERENCES [dbo].[custom_events] ([eventId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[public_board_events] CHECK CONSTRAINT [FK_public_board_events_custom_events]
GO
ALTER TABLE [dbo].[user_work_preferences]  WITH CHECK ADD  CONSTRAINT [FK_user_work_preferences_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[user_work_preferences] CHECK CONSTRAINT [FK_user_work_preferences_campaigns]
GO
ALTER TABLE [dbo].[user_work_preferences]  WITH CHECK ADD  CONSTRAINT [FK_user_work_preferences_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[user_work_preferences] CHECK CONSTRAINT [FK_user_work_preferences_users]
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD  CONSTRAINT [FK_users_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
GO
ALTER TABLE [dbo].[users] CHECK CONSTRAINT [FK_users_users]
GO
ALTER TABLE [dbo].[users_events]  WITH CHECK ADD  CONSTRAINT [FK_users_events_custom_events] FOREIGN KEY([eventId])
REFERENCES [dbo].[custom_events] ([eventId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[users_events] CHECK CONSTRAINT [FK_users_events_custom_events]
GO
ALTER TABLE [dbo].[users_events]  WITH CHECK ADD  CONSTRAINT [FK_users_events_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[users_events] CHECK CONSTRAINT [FK_users_events_users]
GO
ALTER TABLE [dbo].[users_notified_on_publish]  WITH CHECK ADD  CONSTRAINT [FK_users_notified_on_publish_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
GO
ALTER TABLE [dbo].[users_notified_on_publish] CHECK CONSTRAINT [FK_users_notified_on_publish_campaigns]
GO
ALTER TABLE [dbo].[users_notified_on_publish]  WITH CHECK ADD  CONSTRAINT [FK_users_notified_on_publish_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[users_notified_on_publish] CHECK CONSTRAINT [FK_users_notified_on_publish_users]
GO
ALTER TABLE [dbo].[users_public_board_preferences]  WITH CHECK ADD  CONSTRAINT [FK_users_public_board_preferences_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[users_public_board_preferences] CHECK CONSTRAINT [FK_users_public_board_preferences_campaigns]
GO
ALTER TABLE [dbo].[users_public_board_preferences]  WITH CHECK ADD  CONSTRAINT [FK_users_public_board_preferences_users] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[users_public_board_preferences] CHECK CONSTRAINT [FK_users_public_board_preferences_users]
GO
ALTER TABLE [dbo].[users_schedule_managers]  WITH CHECK ADD  CONSTRAINT [FK_users_personal_schedule_management_capable_users_users] FOREIGN KEY([permissionReceiver])
REFERENCES [dbo].[users] ([userId])
GO
ALTER TABLE [dbo].[users_schedule_managers] CHECK CONSTRAINT [FK_users_personal_schedule_management_capable_users_users]
GO
ALTER TABLE [dbo].[users_schedule_managers]  WITH CHECK ADD  CONSTRAINT [FK_users_personal_schedule_management_capable_users_users_personal_schedule_management_capable_users] FOREIGN KEY([permissionGiver])
REFERENCES [dbo].[users] ([userId])
GO
ALTER TABLE [dbo].[users_schedule_managers] CHECK CONSTRAINT [FK_users_personal_schedule_management_capable_users_users_personal_schedule_management_capable_users]
GO
ALTER TABLE [dbo].[users_to_notify_on_join]  WITH CHECK ADD  CONSTRAINT [FK_users_to_notify_on_join_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[users_to_notify_on_join] CHECK CONSTRAINT [FK_users_to_notify_on_join_campaigns]
GO
ALTER TABLE [dbo].[users_to_notify_on_join]  WITH CHECK ADD  CONSTRAINT [FK_users_to_notify_on_join_users_to_notify_on_join] FOREIGN KEY([userId])
REFERENCES [dbo].[users] ([userId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[users_to_notify_on_join] CHECK CONSTRAINT [FK_users_to_notify_on_join_users_to_notify_on_join]
GO
ALTER TABLE [dbo].[voter_campaign_support_statuses]  WITH CHECK ADD  CONSTRAINT [FK_voter_campaign_support_statuses_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[voter_campaign_support_statuses] CHECK CONSTRAINT [FK_voter_campaign_support_statuses_campaigns]
GO
ALTER TABLE [dbo].[votes]  WITH CHECK ADD  CONSTRAINT [FK_votes_campaigns] FOREIGN KEY([campaignId])
REFERENCES [dbo].[campaigns] ([campaignId])
GO
ALTER TABLE [dbo].[votes] CHECK CONSTRAINT [FK_votes_campaigns]
GO
ALTER TABLE [dbo].[votes]  WITH CHECK ADD  CONSTRAINT [FK_votes_parties] FOREIGN KEY([partyId])
REFERENCES [dbo].[parties] ([partyId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[votes] CHECK CONSTRAINT [FK_votes_parties]
GO
/****** Object:  StoredProcedure [dbo].[CreateModel]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================

-- Author:           Moty Uner

-- Create date: 29-Jul-2013

-- Description:      Generates a C# class for a table

-- =============================================

CREATE PROCEDURE [dbo].[CreateModel]

       -- Add the parameters for the stored procedure here

       @tableName nvarchar(100)

AS

BEGIN

       -- SET NOCOUNT ON added to prevent extra result sets from

       -- interfering with SELECT statements.

       SET NOCOUNT ON;

 

    -- Insert statements for procedure here

 

       DECLARE @result varchar(max) = ''

       SET @result = @result + 'public class ' + @TableName + CHAR(13) + '{'

 

       SELECT @result = @result + CHAR(13)

              + ' public ' + ColumnType + ' ' + ColumnName + ' { get; set; } '

       FROM

       (

              SELECT  UPPER(LEFT(c.COLUMN_NAME, 1)) + RIGHT(c.COLUMN_NAME, LEN(c.COLUMN_NAME)-1)  AS ColumnName

                     , CASE c.DATA_TYPE  

                           WHEN 'bigint' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'Int64?' ELSE 'Int64' END

                           WHEN 'binary' THEN 'Byte[]'

                           WHEN 'bit' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'bool?' ELSE 'bool' END           

                           WHEN 'char' THEN 'string'

                           WHEN 'date' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                       

                           WHEN 'datetime' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                       

                           WHEN 'datetime2' THEN 

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                       

                           WHEN 'datetimeoffset' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'DateTimeOffset?' ELSE 'DateTimeOffset' END                                   

                           WHEN 'decimal' THEN 

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'Decimal?' ELSE 'Decimal' END                                   

                           WHEN 'float' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'double?' ELSE 'double' END                                   

                           WHEN 'image' THEN 'Byte[]'

                           WHEN 'int' THEN 

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'int?' ELSE 'int' END

                           WHEN 'money' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'Decimal?' ELSE 'Decimal' END                                               

                           WHEN 'nchar' THEN 'string'

                           WHEN 'ntext' THEN 'string'

                           WHEN 'numeric' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'Decimal?' ELSE 'Decimal' END                                                           

                           WHEN 'nvarchar' THEN 'string'

                           WHEN 'real' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'double?' ELSE 'double' END                                                                       

                           WHEN 'smalldatetime' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                                   

                           WHEN 'smallint' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'Int16?' ELSE 'Int16'END           

                           WHEN 'smallmoney' THEN 

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'Decimal?' ELSE 'Decimal' END                                                                       

                           WHEN 'text' THEN 'string'

                           WHEN 'time' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'TimeSpan?' ELSE 'TimeSpan' END                                                                                   

                           WHEN 'timestamp' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'DateTime?' ELSE 'DateTime' END                                   

                           WHEN 'tinyint' THEN

                                  CASE C.IS_NULLABLE

                                         WHEN 'YES' THEN 'Byte?' ELSE 'Byte' END                                               

                           WHEN 'uniqueidentifier' THEN 'Guid'

                           WHEN 'varbinary' THEN 'Byte[]'

                           WHEN 'varchar' THEN 'string'

                           ELSE 'Object'

                     END AS ColumnType

                     , c.ORDINAL_POSITION

       FROM    INFORMATION_SCHEMA.COLUMNS c

       WHERE   c.TABLE_NAME = @TableName  

       ) t

       ORDER BY t.ORDINAL_POSITION

 

       SET @result = @result + CHAR(13)

       SET @result = @result  + '}' + CHAR(13)

 

       PRINT @result

END
GO
/****** Object:  StoredProcedure [dbo].[GenerateInserts]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GenerateInserts]

(

      @table_name varchar(776),           -- The table/view for which the INSERT statements will be generated using the existing data

      @target_table varchar(776) = NULL, -- Use this parameter to specify a different table name into which the data will be inserted

      @include_column_list bit = 1,       -- Use this parameter to include/ommit column list in the generated INSERT statement

      @from varchar(800) = NULL,          -- Use this parameter to filter the rows based on a filter condition (using WHERE)

      @include_timestamp bit = 0,         -- Specify 1 for this parameter, if you want to include the TIMESTAMP/ROWVERSION column's data in the INSERT statement

      @debug_mode bit = 0,                -- If @debug_mode is set to 1, the SQL statements constructed by this procedure will be printed for later examination

      @owner varchar(64) = NULL,          -- Use this parameter if you are not the owner of the table

      @ommit_images bit = 0,              -- Use this parameter to generate INSERT statements by omitting the 'image' columns

      @ommit_identity bit = 0,            -- Use this parameter to ommit the identity columns

      @top int = NULL,              -- Use this parameter to generate INSERT statements only for the TOP n rows

      @cols_to_include varchar(8000) = NULL,    -- List of columns to be included in the INSERT statement

      @cols_to_exclude varchar(8000) = NULL,    -- List of columns to be excluded from the INSERT statement

      @disable_constraints bit = 0,       -- When 1, disables foreign key constraints and enables them after the INSERT statements

      @ommit_computed_cols bit = 0        -- When 1, computed columns will not be included in the INSERT statement

     

)

AS

BEGIN

 

/***********************************************************************************************************

Procedure:  sp_generate_inserts  (Build 22)

            (Copyright © 2002 Narayana Vyas Kondreddi. All rights reserved.)

                                         

Purpose:    To generate INSERT statements from existing data.

            These INSERTS can be executed to regenerate the data at some other location.

            This procedure is also useful to create a database setup, where in you can

            script your data along with your table definitions.

 

Written by: Narayana Vyas Kondreddi

              http://vyaskn.tripod.com     http://vyaskn.tripod.com/code.htm#inserts

 

Acknowledgements:

            Divya Kalra -- For beta testing

            Mark Charsley     -- For reporting a problem with scripting uniqueidentifier columns with NULL values

            Artur Zeygman     -- For helping me simplify a bit of code for handling non-dbo owned tables

            Joris Laperre   -- For reporting a regression bug in handling text/ntext columns

 

Tested on: SQL Server 7.0 and SQL Server 2000 and SQL Server 2005

 

Date created:     January 17th 2001 21:52 GMT

 

Date modified:    May 1st 2002 19:50 GMT

 

Email:            vyaskn@hotmail.com

 

NOTE:       This procedure may not work with tables with too many columns.

            Results can be unpredictable with huge text columns or SQL Server 2000's sql_variant data types

            Whenever possible, Use @include_column_list parameter to ommit column list in the INSERT statement, for better results

            IMPORTANT: This procedure is not tested with internation data (Extended characters or Unicode). If needed

            you might want to convert the datatypes of character variables in this procedure to their respective unicode counterparts

            like nchar and nvarchar

 

            ALSO NOTE THAT THIS PROCEDURE IS NOT UPDATED TO WORK WITH NEW DATA TYPES INTRODUCED IN SQL SERVER 2005 / YUKON

           

 

Example 1:  To generate INSERT statements for table 'titles':

           

            EXEC sp_generate_inserts 'titles'

 

Example 2: To ommit the column list in the INSERT statement: (Column list is included by default)

            IMPORTANT: If you have too many columns, you are advised to ommit column list, as shown below,

            to avoid erroneous results

           

            EXEC sp_generate_inserts 'titles', @include_column_list = 0

 

Example 3:  To generate INSERT statements for 'titlesCopy' table from 'titles' table:

 

            EXEC sp_generate_inserts 'titles', 'titlesCopy'

 

Example 4:  To generate INSERT statements for 'titles' table for only those titles

            which contain the word 'Computer' in them:

            NOTE: Do not complicate the FROM or WHERE clause here. It's assumed that you are good with T-SQL if you are using this parameter

 

            EXEC sp_generate_inserts 'titles', @from = "from titles where title like '%Computer%'"

 

Example 5: To specify that you want to include TIMESTAMP column's data as well in the INSERT statement:

            (By default TIMESTAMP column's data is not scripted)

 

            EXEC sp_generate_inserts 'titles', @include_timestamp = 1

 

Example 6:  To print the debug information:

 

            EXEC sp_generate_inserts 'titles', @debug_mode = 1

 

Example 7: If you are not the owner of the table, use @owner parameter to specify the owner name

            To use this option, you must have SELECT permissions on that table

 

            EXEC sp_generate_inserts Nickstable, @owner = 'Nick'

 

Example 8: To generate INSERT statements for the rest of the columns excluding images

            When using this otion, DO NOT set @include_column_list parameter to 0.

 

            EXEC sp_generate_inserts imgtable, @ommit_images = 1

 

Example 9: To generate INSERT statements excluding (ommiting) IDENTITY columns:

            (By default IDENTITY columns are included in the INSERT statement)

 

            EXEC sp_generate_inserts mytable, @ommit_identity = 1

 

Example 10:       To generate INSERT statements for the TOP 10 rows in the table:

           

            EXEC sp_generate_inserts mytable, @top = 10

 

Example 11:       To generate INSERT statements with only those columns you want:

           

            EXEC sp_generate_inserts titles, @cols_to_include = "'title','title_id','au_id'"

 

Example 12:       To generate INSERT statements by omitting certain columns:

           

            EXEC sp_generate_inserts titles, @cols_to_exclude = "'title','title_id','au_id'"

 

Example 13: To avoid checking the foreign key constraints while loading data with INSERT statements:

           

            EXEC sp_generate_inserts titles, @disable_constraints = 1

 

Example 14:       To exclude computed columns from the INSERT statement:

            EXEC sp_generate_inserts MyTable, @ommit_computed_cols = 1

***********************************************************************************************************/

 

SET NOCOUNT ON

 

--Making sure user only uses either @cols_to_include or @cols_to_exclude

IF ((@cols_to_include IS NOT NULL) AND (@cols_to_exclude IS NOT NULL))

      BEGIN

            RAISERROR('Use either @cols_to_include or @cols_to_exclude. Do not use both the parameters at once',16,1)

            RETURN -1 --Failure. Reason: Both @cols_to_include and @cols_to_exclude parameters are specified

      END

 

--Making sure the @cols_to_include and @cols_to_exclude parameters are receiving values in proper format

IF ((@cols_to_include IS NOT NULL) AND (PATINDEX('''%''',@cols_to_include) = 0))

      BEGIN

            RAISERROR('Invalid use of @cols_to_include property',16,1)

            PRINT 'Specify column names surrounded by single quotes and separated by commas'

            PRINT 'Eg: EXEC sp_generate_inserts titles, @cols_to_include = "''title_id'',''title''"'

            RETURN -1 --Failure. Reason: Invalid use of @cols_to_include property

      END

 

IF ((@cols_to_exclude IS NOT NULL) AND (PATINDEX('''%''',@cols_to_exclude) = 0))

      BEGIN

            RAISERROR('Invalid use of @cols_to_exclude property',16,1)

            PRINT 'Specify column names surrounded by single quotes and separated by commas'

            PRINT 'Eg: EXEC sp_generate_inserts titles, @cols_to_exclude = "''title_id'',''title''"'

            RETURN -1 --Failure. Reason: Invalid use of @cols_to_exclude property

      END

 

 

--Checking to see if the database name is specified along wih the table name

--Your database context should be local to the table for which you want to generate INSERT statements

--specifying the database name is not allowed

IF (PARSENAME(@table_name,3)) IS NOT NULL

      BEGIN

            RAISERROR('Do not specify the database name. Be in the required database and just specify the table name.',16,1)

            RETURN -1 --Failure. Reason: Database name is specified along with the table name, which is not allowed

      END

 

--Checking for the existence of 'user table' or 'view'

--This procedure is not written to work on system tables

--To script the data in system tables, just create a view on the system tables and script the view instead

 

IF @owner IS NULL

      BEGIN

            IF ((OBJECT_ID(@table_name,'U') IS NULL) AND (OBJECT_ID(@table_name,'V') IS NULL))

                  BEGIN

                        RAISERROR('User table or view not found.',16,1)

                        PRINT 'You may see this error, if you are not the owner of this table or view. In that case use @owner parameter to specify the owner name.'

                        PRINT 'Make sure you have SELECT permission on that table or view.'

                        RETURN -1 --Failure. Reason: There is no user table or view with this name

                  END

      END

ELSE

      BEGIN

            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @table_name AND (TABLE_TYPE = 'BASE TABLE' OR TABLE_TYPE = 'VIEW') AND TABLE_SCHEMA = @owner)

                  BEGIN

                        RAISERROR('User table or view not found.',16,1)

                        PRINT 'You may see this error, if you are not the owner of this table. In that case use @owner parameter to specify the owner name.'

                        PRINT 'Make sure you have SELECT permission on that table or view.'

                        RETURN -1 --Failure. Reason: There is no user table or view with this name       

                  END

      END

 

--Variable declarations

DECLARE           @Column_ID int,        

            @Column_List varchar(8000),

            @Column_Name varchar(128),

            @Start_Insert varchar(786),

            @Data_Type varchar(128),

            @Actual_Values varchar(8000), --This is the string that will be finally executed to generate INSERT statements

            @IDN varchar(128)       --Will contain the IDENTITY column's name in the table

 

--Variable Initialization

SET @IDN = ''

SET @Column_ID = 0

SET @Column_Name = ''

SET @Column_List = ''

SET @Actual_Values = ''

 

IF @owner IS NULL

      BEGIN

            SET @Start_Insert = 'INSERT INTO ' + '[' + RTRIM(COALESCE(@target_table,@table_name)) + ']'

      END

ELSE

      BEGIN

            SET @Start_Insert = 'INSERT ' + '[' + LTRIM(RTRIM(@owner)) + '].' + '[' + RTRIM(COALESCE(@target_table,@table_name)) + ']'           

      END

 

 

--To get the first column's ID

 

SELECT      @Column_ID = MIN(ORDINAL_POSITION)

FROM  INFORMATION_SCHEMA.COLUMNS (NOLOCK)

WHERE       TABLE_NAME = @table_name AND

(@owner IS NULL OR TABLE_SCHEMA = @owner)

 

 

 

--Loop through all the columns of the table, to get the column names and their data types

WHILE @Column_ID IS NOT NULL

      BEGIN

            SELECT      @Column_Name = QUOTENAME(COLUMN_NAME),

            @Data_Type = DATA_TYPE

            FROM INFORMATION_SCHEMA.COLUMNS (NOLOCK)

            WHERE       ORDINAL_POSITION = @Column_ID AND

            TABLE_NAME = @table_name AND

            (@owner IS NULL OR TABLE_SCHEMA = @owner)

 

 

 

            IF @cols_to_include IS NOT NULL --Selecting only user specified columns

            BEGIN

                  IF CHARINDEX( '''' + SUBSTRING(@Column_Name,2,LEN(@Column_Name)-2) + '''',@cols_to_include) = 0

                  BEGIN

                        GOTO SKIP_LOOP

                  END

            END

 

            IF @cols_to_exclude IS NOT NULL --Selecting only user specified columns

            BEGIN

                  IF CHARINDEX( '''' + SUBSTRING(@Column_Name,2,LEN(@Column_Name)-2) + '''',@cols_to_exclude) <> 0

                  BEGIN

                        GOTO SKIP_LOOP

                  END

            END

 

            --Making sure to output SET IDENTITY_INSERT ON/OFF in case the table has an IDENTITY column

            IF (SELECT COLUMNPROPERTY( OBJECT_ID(QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + @table_name),SUBSTRING(@Column_Name,2,LEN(@Column_Name) - 2),'IsIdentity')) = 1

            BEGIN

                  IF @ommit_identity = 0 --Determing whether to include or exclude the IDENTITY column

                        SET @IDN = @Column_Name

                  ELSE

                        GOTO SKIP_LOOP               

            END

           

            --Making sure whether to output computed columns or not

            IF @ommit_computed_cols = 1

            BEGIN

                  IF (SELECT COLUMNPROPERTY( OBJECT_ID(QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + @table_name),SUBSTRING(@Column_Name,2,LEN(@Column_Name) - 2),'IsComputed')) = 1

                  BEGIN

                        GOTO SKIP_LOOP                           

                  END

            END

           

            --Tables with columns of IMAGE data type are not supported for obvious reasons

            IF(@Data_Type in ('image'))

                  BEGIN

                        IF (@ommit_images = 0)

                              BEGIN

                                    RAISERROR('Tables with image columns are not supported.',16,1)

                                    PRINT 'Use @ommit_images = 1 parameter to generate INSERTs for the rest of the columns.'

                                    PRINT 'DO NOT ommit Column List in the INSERT statements. If you ommit column list using @include_column_list=0, the generated INSERTs will fail.'

                                    RETURN -1 --Failure. Reason: There is a column with image data type

                              END

                        ELSE

                              BEGIN

                              GOTO SKIP_LOOP

                              END

                  END

 

            --Determining the data type of the column and depending on the data type, the VALUES part of

            --the INSERT statement is generated. Care is taken to handle columns with NULL values. Also

            --making sure, not to lose any data from flot, real, money, smallmomey, datetime columns

            SET @Actual_Values = @Actual_Values  +

            CASE

                  WHEN @Data_Type IN ('char','varchar','nchar','nvarchar')

                        THEN

                              'COALESCE('''''''' + REPLACE(RTRIM(' + @Column_Name + '),'''''''','''''''''''')+'''''''',''NULL'')'

                  WHEN @Data_Type IN ('date','datetime','smalldatetime')

                        THEN

                              'COALESCE('''''''' + RTRIM(CONVERT(char,' + @Column_Name + ',109))+'''''''',''NULL'')'

                  WHEN @Data_Type IN ('uniqueidentifier')

                        THEN 

                              'COALESCE('''''''' + REPLACE(CONVERT(char(255),RTRIM(' + @Column_Name + ')),'''''''','''''''''''')+'''''''',''NULL'')'

                  WHEN @Data_Type IN ('text','ntext')

                        THEN 

                              'COALESCE('''''''' + REPLACE(CONVERT(char(8000),' + @Column_Name + '),'''''''','''''''''''')+'''''''',''NULL'')'                             

                  WHEN @Data_Type IN ('binary','varbinary')

                        THEN 

                              'COALESCE(RTRIM(CONVERT(char,' + 'CONVERT(int,' + @Column_Name + '))),''NULL'')' 

                  WHEN @Data_Type IN ('timestamp','rowversion')

                        THEN 

                              CASE

                                    WHEN @include_timestamp = 0

                                          THEN

                                                '''DEFAULT'''

                                          ELSE

                                                'COALESCE(RTRIM(CONVERT(char,' + 'CONVERT(int,' + @Column_Name + '))),''NULL'')' 

                              END

                  WHEN @Data_Type IN ('float','real','money','smallmoney')

                        THEN

                              'COALESCE(LTRIM(RTRIM(' + 'CONVERT(char, ' +  @Column_Name  + ',2)' + ')),''NULL'')'

                  ELSE

                        'COALESCE(LTRIM(RTRIM(' + 'CONVERT(char, ' +  @Column_Name  + ')' + ')),''NULL'')'

            END   + '+' +  ''',''' + ' + '

           

            --Generating the column list for the INSERT statement

            SET @Column_List = @Column_List +  @Column_Name + ','

 

            SKIP_LOOP: --The label used in GOTO

 

            SELECT      @Column_ID = MIN(ORDINAL_POSITION)

            FROM INFORMATION_SCHEMA.COLUMNS (NOLOCK)

            WHERE       TABLE_NAME = @table_name AND

            ORDINAL_POSITION > @Column_ID AND

            (@owner IS NULL OR TABLE_SCHEMA = @owner)

 

 

      --Loop ends here!

      END

 

--To get rid of the extra characters that got concatenated during the last run through the loop

SET @Column_List = LEFT(@Column_List,len(@Column_List) - 1)

SET @Actual_Values = LEFT(@Actual_Values,len(@Actual_Values) - 6)

 

IF LTRIM(@Column_List) = ''

      BEGIN

            RAISERROR('No columns to select. There should at least be one column to generate the output',16,1)

            RETURN -1 --Failure. Reason: Looks like all the columns are ommitted using the @cols_to_exclude parameter

      END

 

--Forming the final string that will be executed, to output the INSERT statements

IF (@include_column_list <> 0)

      BEGIN

            SET @Actual_Values =

                  'SELECT ' + 

                  CASE WHEN @top IS NULL OR @top < 0 THEN '' ELSE ' TOP ' + LTRIM(STR(@top)) + ' ' END +

                  '''' + RTRIM(@Start_Insert) +

                  ' ''+' + '''(' + RTRIM(@Column_List) +  '''+' + ''')''' +

                  ' +''VALUES(''+ ' +  @Actual_Values  + '+'')''' + ' ' +

                  COALESCE(@from,' FROM ' + CASE WHEN @owner IS NULL THEN '' ELSE '[' + LTRIM(RTRIM(@owner)) + '].' END + '[' + rtrim(@table_name) + ']' + '(NOLOCK)')

      END

ELSE IF (@include_column_list = 0)

      BEGIN

            SET @Actual_Values =

                  'SELECT ' +

                  CASE WHEN @top IS NULL OR @top < 0 THEN '' ELSE ' TOP ' + LTRIM(STR(@top)) + ' ' END +

                  '''' + RTRIM(@Start_Insert) +

                  ' '' +''VALUES(''+ ' +  @Actual_Values + '+'')''' + ' ' +

                  COALESCE(@from,' FROM ' + CASE WHEN @owner IS NULL THEN '' ELSE '[' + LTRIM(RTRIM(@owner)) + '].' END + '[' + rtrim(@table_name) + ']' + '(NOLOCK)')

      END  

 

--Determining whether to ouput any debug information

IF @debug_mode =1

      BEGIN

            PRINT '/*****START OF DEBUG INFORMATION*****'

            PRINT 'Beginning of the INSERT statement:'

            PRINT @Start_Insert

            PRINT ''

            PRINT 'The column list:'

            PRINT @Column_List

            PRINT ''

            PRINT 'The SELECT statement executed to generate the INSERTs'

            PRINT @Actual_Values

            PRINT ''

            PRINT '*****END OF DEBUG INFORMATION*****/'

            PRINT ''

      END

           

PRINT '--INSERTs generated by ''sp_generate_inserts'' stored procedure written by Vyas'

PRINT '--Build number: 22'

PRINT '--Problems/Suggestions? Contact Vyas @ vyaskn@hotmail.com'

PRINT '--http://vyaskn.tripod.com'

PRINT ''

PRINT 'SET NOCOUNT ON'

PRINT ''

 

 

--Determining whether to print IDENTITY_INSERT or not

IF (@IDN <> '')

      BEGIN

            PRINT 'SET IDENTITY_INSERT ' + QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + QUOTENAME(@table_name) + ' ON'

            PRINT 'GO'

            PRINT ''

      END

 

 

IF @disable_constraints = 1 AND (OBJECT_ID(QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + @table_name, 'U') IS NOT NULL)

      BEGIN

            IF @owner IS NULL

                  BEGIN

                        SELECT      'ALTER TABLE ' + QUOTENAME(COALESCE(@target_table, @table_name)) + ' NOCHECK CONSTRAINT ALL' AS '--Code to disable constraints temporarily'

                  END

            ELSE

                  BEGIN

                        SELECT      'ALTER TABLE ' + QUOTENAME(@owner) + '.' + QUOTENAME(COALESCE(@target_table, @table_name)) + ' NOCHECK CONSTRAINT ALL' AS '--Code to disable constraints temporarily'

                  END

 

            PRINT 'GO'

      END

 

PRINT ''

PRINT 'PRINT ''Inserting values into ' + '[' + RTRIM(COALESCE(@target_table,@table_name)) + ']' + ''''

 

 

--All the hard work pays off here!!! You'll get your INSERT statements, when the next line executes!

EXEC (@Actual_Values)

 

PRINT 'PRINT ''Done'''

PRINT ''

 

 

IF @disable_constraints = 1 AND (OBJECT_ID(QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + @table_name, 'U') IS NOT NULL)

      BEGIN

            IF @owner IS NULL

                  BEGIN

                        SELECT      'ALTER TABLE ' + QUOTENAME(COALESCE(@target_table, @table_name)) + ' CHECK CONSTRAINT ALL'  AS '--Code to enable the previously disabled constraints'

                  END

            ELSE

                  BEGIN

                        SELECT      'ALTER TABLE ' + QUOTENAME(@owner) + '.' + QUOTENAME(COALESCE(@target_table, @table_name)) + ' CHECK CONSTRAINT ALL' AS '--Code to enable the previously disabled constraints'

                  END

 

            PRINT 'GO'

      END

 

PRINT ''

IF (@IDN <> '')

      BEGIN

            PRINT 'SET IDENTITY_INSERT ' + QUOTENAME(COALESCE(@owner,USER_NAME())) + '.' + QUOTENAME(@table_name) + ' OFF'

            PRINT 'GO'

      END

 

PRINT 'SET NOCOUNT OFF'

 

 

SET NOCOUNT OFF

RETURN 0 --Success. We are done!

END

 

--GO

--

--PRINT 'Created the procedure'

--GO

--

--

----Mark procedure as system object

--EXEC sys.sp_MS_marksystemobject sp_generate_inserts

--GO

--

--PRINT 'Granting EXECUTE permission on sp_generate_inserts to all users'

--GRANT EXEC ON sp_generate_inserts TO public

--

--SET NOCOUNT OFF

--GO

--

--PRINT 'Done'

--

--

--
GO
/****** Object:  StoredProcedure [dbo].[GenerateSteets]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Moty
-- Create Date: 04-Jan-2023
-- Description: 
-- =============================================
CREATE PROCEDURE [dbo].[GenerateSteets]

AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	IF OBJECT_ID('tempdb..#Streets') IS NOT NULL DROP TABLE #Streets

	CREATE TABLE #Streets
	(
		ID INT NOT NULL,
		[cityId] [smallint] NOT NULL,
		[streetId] [smallint] NOT NULL,
		[streetName] [nvarchar](100) NOT NULL
	)

	INSERT #Streets(ID, cityId, streetId, streetName)
	SELECT ROW_NUMBER() OVER(PARTITION BY cityId ORDER BY cityId) AS ID, cityId, streetId, streetName FROM streets

	IF OBJECT_ID('tempdb..#MinMax') IS NOT NULL DROP TABLE #MinMax

	CREATE TABLE #MinMax
	(
		cityId INT NOT NULL,
		minStreetId [smallint] NOT NULL,
		maxStreetId [smallint] NOT NULL
	)
	INSERT #MinMax(cityId, minStreetId, maxStreetId)
	SELECT cityId, MIN(ID) AS minStreetId,  MAX(ID) AS maxStreetId FROM #Streets GROUP BY cityId

	--SELECT	RAND() * (maxStreetId - minStreetId) + minStreetId AS streetId, #Streets.cityId 
	--FROM	#Streets INNER JOIN 
	--		#MinMax ON #MinMax.cityId = #Streets.cityId 
	--ORDER BY #Streets.cityId

	--SELECT streetId, #Streets.cityId 
	--FROM #Streets INNER JOIN
	--(
		SELECT	distinct ABS(CHECKSUM(NEWID())) % ((maxStreetId - minStreetId) + minStreetId) + 1 AS minMaxstreetId, #Streets.cityId 
		FROM	#Streets INNER JOIN 
				#MinMax ON #MinMax.cityId = #Streets.cityId
		WHERE	#MinMax.cityId = 9700
	--) AS rs ON rs.minMaxstreetId = ID AND rs.cityId = #Streets.cityId




END
GO
/****** Object:  StoredProcedure [dbo].[InsertDefinedPermission]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Inserts a new defined permission to the table. For internal use only, not to be used by the server.
-- =============================================
CREATE PROCEDURE [dbo].[InsertDefinedPermission]
(
    -- Add the parameters for the stored procedure here
	@permissionTarget nvarchar(50)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    INSERT 
	INTO defined_permissions(permissionType, permissionTarget) 
	VALUES ('edit', @permissionTarget);

	INSERT 
	INTO defined_permissions(permissionType, permissionTarget) 
	VALUES ('view', @permissionTarget);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AddAllPermissions]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 18/01/2023
-- Description: Adds all permissions to a user. Only run when creating a new campaign, to give the owner full permissions.
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddAllPermissions]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@campaignId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	/*
	DECLARE @permissionId int;

    DECLARE cur CURSOR STATIC READ_ONLY FORWARD_ONLY FOR 
		SELECT permissionID
		FROM defined_permissions
		ORDER BY permissionId;

	OPEN cur

	FETCH NEXT FROM cur   
	INTO @permissionId

	WHILE @@fetch_status = 0
	BEGIN

		INSERT INTO permission_sets VALUES (@userId, @campaignId, @permissionId);

		FETCH NEXT FROM cur   
		INTO @permissionId

	END
	*/


	INSERT permission_sets (userId, campaignId, permissionId)
	SELECT @userId, @campaignId, permissionId
	FROM defined_permissions
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AdvisorAnalysisDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/05/2023
-- Description: Completely deletes an analysis from the database.
-- =============================================
CREATE PROCEDURE [dbo].[usp_AdvisorAnalysisDelete]
(
    -- Add the parameters for the stored procedure here
	@resultsGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	-- Cascading deletes due to FK relations will delete the details and samples.
    DELETE FROM campaign_advisor_results_overview
	WHERE resultsGuid = @resultsGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AdvisorAnalysisDetailsAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/05/2023
-- Description: Adds a new row of analysis results to the results details table, detailing the sentiment found for each topic.
-- =============================================
CREATE PROCEDURE [dbo].[usp_AdvisorAnalysisDetailsAdd]
(
    -- Add the parameters for the stored procedure here
	@resultsGuid uniqueidentifier,
	@topic nvarchar(50),
	@total int,
	@positive float,
	@negative float,
	@neutral float,
	@hate float,
	@rowType int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @resultsId int = (SELECT resultsId FROM campaign_advisor_results_overview WHERE resultsGuid = @resultsGuid);

	INSERT INTO campaign_advisor_results_details(resultsId, topic, total, positive, negative, neutral, hate, rowType)
	VALUES (@resultsId, @topic, @total, @positive, @negative, @neutral, @hate, @rowType);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AdvisorAnalysisDetailsGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/05/2023
-- Description: Gets all the rows of analysis details for a single analysis result
-- =============================================
CREATE PROCEDURE [dbo].[usp_AdvisorAnalysisDetailsGet]
(
    -- Add the parameters for the stored procedure here
	@resultsGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT topic, total, positive, negative, neutral, hate, rowType
	FROM campaign_advisor_results_details
	WHERE resultsId = (SELECT resultsId FROM campaign_advisor_results_overview WHERE resultsGuid = @resultsGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AdvisorAnalysisOverviewAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/05/2023
-- Description: Adds a new analysis results overview entry to the corresponding table. This is done prior to adding the results themselves.
-- =============================================
CREATE PROCEDURE [dbo].[usp_AdvisorAnalysisOverviewAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@resultsTitle nvarchar(150) = NULL,
	@analysisTarget nvarchar(100),
	@targetTwitterHandle nvarchar(50),
	@maxDaysBack int = 0,
	@additionalUserRequests nvarchar(400) = NULL,
	@newResultsGuid uniqueidentifier OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SET @newResultsGuid = NEWID();

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	WHILE EXISTS(SELECT resultsId FROM campaign_advisor_results_overview WHERE resultsGuid = @newResultsGuid)
		SET @newResultsGuid = NEWID();

	INSERT INTO campaign_advisor_results_overview(resultsGuid, timePerformed, campaignId, analysisTarget, targetTwitterHandle, maxDaysBack, additionalUserRequests, resultsTitle)
	VALUES (@newResultsGuid, GETDATE(), @campaignId, @analysisTarget, @targetTwitterHandle, @maxDaysBack, @additionalUserRequests, @resultsTitle);

END
GO
/****** Object:  StoredProcedure [dbo].[usp_AdvisorAnalysisOverviewGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/05/2023
-- Description: Gets the complete overview of a single advisor's anlysis results
-- =============================================
CREATE PROCEDURE [dbo].[usp_AdvisorAnalysisOverviewGet]
(
    -- Add the parameters for the stored procedure here
	@resultsGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT resultsGuid, timePerformed, resultsTitle, analysisTarget, targetTwitterHandle, maxDaysBack, gptResponse, additionalUserRequests
	FROM campaign_advisor_results_overview
	WHERE resultsGuid = @resultsGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AdvisorAnalysisOverviewGptResponseUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/05/2023
-- Description: Adds or updates the GPT response field on a single analysis
-- =============================================
CREATE PROCEDURE [dbo].[usp_AdvisorAnalysisOverviewGptResponseUpdate]
(
    -- Add the parameters for the stored procedure here
	@resultsGuid uniqueidentifier,
	@gptResponse nvarchar(MAX)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    UPDATE campaign_advisor_results_overview
	SET gptResponse = @gptResponse
	WHERE resultsGuid = @resultsGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AdvisorAnalysisSampleAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/05/2023
-- Description: Adds a new text sample to an analysis
-- =============================================
CREATE PROCEDURE [dbo].[usp_AdvisorAnalysisSampleAdd]
(
    -- Add the parameters for the stored procedure here
	@resultsGuid uniqueidentifier,
	@sampleText nvarchar(400),
	@isArticle bit = 1
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @resultsId int = (SELECT resultsId FROM campaign_advisor_results_overview WHERE resultsGuid = @resultsGuid);

	INSERT INTO campaign_advisor_analysis_samples(resultsId, sampleText, isArticle)
	VALUES (@resultsId, @sampleText, @isArticle);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AdvisorAnalysisSamplesGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/05/2023
-- Description: Gets all samples related to a specific analysis result
-- =============================================
CREATE PROCEDURE [dbo].[usp_AdvisorAnalysisSamplesGet]
(
    -- Add the parameters for the stored procedure here
	@resultsGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT sampleText, isArticle
	FROM campaign_advisor_analysis_samples
	WHERE resultsId = (SELECT resultsId FROM campaign_advisor_results_overview WHERE resultsGuid = @resultsGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AllBallotsForCampaignGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Gets a list of all ballots for a campaign, both cusotm ballots and built in ones
-- =============================================
CREATE PROCEDURE [dbo].[usp_AllBallotsForCampaignGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int, @isCustomCampaign bit, @cityId int;
	SELECT @campaignId = campaignId, @isCustomCampaign = isCustomCampaign, @cityId = cityId FROM campaigns WHERE campaignGuid = @campaignGuid;

	-- CampaignNotFound status code
	IF @campaignId IS NULL OR @isCustomCampaign IS NULL RETURN 50017;

	IF @isCustomCampaign = 1
	BEGIN

		SELECT ballotId, cityName, innerCityBallotId, ballotAddress, ballotLocation, accessible, elligibleVoters, 1 AS isCustomBallot
		FROM custom_ballots
		WHERE campaignId = @campaignId;

	END
	ELSE
	BEGIN

		SELECT ballotId, cityName, innerCityBallotId, ballotAddress, ballotLocation, accessible, elligibleVoters, 0 AS isCustomBallot
		FROM ballots
		INNER JOIN cities
		ON ballots.cityId = cities.cityId
		WHERE ballots.cityId = @cityId OR @cityId = -1
		UNION
		SELECT ballotId, cityName, innerCityBallotId, ballotAddress, ballotLocation, accessible, elligibleVoters, 1 AS isCustomBallot
		FROM custom_ballots
		WHERE campaignId = @campaignId;

	END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_AnalysisOverviewsForCampaignGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/05/2023
-- Description: Gets the title and GUIDs of every analysis result of a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_AnalysisOverviewsForCampaignGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT resultsTitle, resultsGuid, analysisTarget, timePerformed
	FROM campaign_advisor_results_overview
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_BallotGetForUser]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 13/03/2023
-- Description: Gets the ballot the user is assigned to on election day
-- =============================================
CREATE PROCEDURE [dbo].[usp_BallotGetForUser]
(
    -- Add the parameters for the stored procedure here
	@userId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT innerCityBallotId, ballotAddress, ballotLocation, accessible, cityName
	FROM ballots
	INNER JOIN cities
	ON ballots.cityId = cities.cityId
	INNER JOIN voters_ledger
	ON ballots.ballotId = voters_ledger.ballotId
	WHERE idNum = (SELECT idNum FROM users WHERE userId = @userId);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 11/01/2023
-- Description: Add a campaign to the campaigns table, and also link its creator to it as a candidate.
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignAdd]
(
    -- Add the parameters for the stored procedure here
    @campaignName nvarchar(200),
    @campaignCreatorUserId int,
	@campaignDescription nvarchar(500) = Null,
	@isMunicipal bit,
	@campaignLogoUrl nvarchar(2083) = NULL,
	@cityName nvarchar(200),
	@isCustomCampaign bit = 0
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON
	BEGIN TRANSACTION 

		DECLARE @campaignGuid uniqueidentifier = NEWID();

		-- Unlikely that this loop will ever run, and even if it does, unlikely to run more than once.
		WHILE EXISTS(SELECT * FROM campaigns WHERE campaignGuid = @campaignGuid)
			SET @campaignGuid = NEWID()

		DECLARE @campaignCreationDate datetime = GETDATE();
		DECLARE @campaignIsActive bit = 1;
		DECLARE @cityId int = (SELECT cityId FROM cities WHERE cityName = @cityName);

		-- Return -1 to signify an error if the city name was not valid
		IF @cityId IS NULL 
		BEGIN
			ROLLBACK
			RETURN -1;
		END;

		-- Insert the new campaign into the campaigns table.
		INSERT 
		INTO campaigns(campaignName, campaignGuid, campaignCreatorUserId, campaignDescription,
			campaignCreationDate, campaignIsActive, campaignLogoUrl, isMunicipal, cityId, isCustomCampaign)
		VALUES(@campaignName, @campaignGuid, @campaignCreatorUserId, @campaignDescription,
			@campaignCreationDate, @campaignIsActive, @campaignLogoUrl, @isMunicipal,
			(SELECT cityId FROM cities WHERE cityName = @cityName), @isCustomCampaign); 

		-- Link the creator of the campaign to it as a candidate.
		DECLARE @campaignId int = SCOPE_IDENTITY();
		DECLARE @roleId int = 5;
	
		INSERT 
		INTO campaign_users(campaignId, userId, roleId) 
		VALUES(@campaignId, @campaignCreatorUserId, @roleId);

		EXEC usp_AddAllPermissions 
			@userId = @campaignCreatorUserId,
			@campaignId = @campaignId
	COMMIT
	RETURN @campaignId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignAdminStaffGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 12/04/2023
-- Description: Gets a list of the info of every member with an admin role in a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignAdminStaffGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT roleName, roleLevel, firstNameEng, firstNameHeb, profilePicUrl, lastNameEng, lastNameHeb, displayNameEng
	FROM campaign_users
	INNER JOIN users
	ON campaign_users.userId = users.userId
	LEFT JOIN roles
	ON campaign_users.roleId = roles.roleId
	WHERE campaign_users.campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)
			AND roleLevel > 0;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignBasicInfoGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 31/01/2023
-- Description: Gets the basic info of a campaign by its Guid
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignBasicInfoGet]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT campaignName, campaignGuid, campaignDescription, campaignCreationDate, campaignLogoUrl, isMunicipal, cityName, isCustomCampaign
	FROM campaigns
	INNER JOIN cities
	ON campaigns.cityId = cities.cityId
	WHERE campaignGuid = @campaignGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignByInviteGuidGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 13/01/2023
-- Description: Gets a campaign's Guid by the campaign's invite Guid
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignByInviteGuidGet]
(
    -- Add the parameters for the stored procedure here
    @campaignInviteGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    SELECT campaignGuid, campaignName, isCustomCampaign FROM campaigns WHERE campaignInviteGuid = @campaignInviteGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 20/01/2023
-- Description: Deletes a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignDelete]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DELETE FROM campaigns WHERE campaignGuid = @campaignGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignGuidByIdGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 13/01/2023
-- Description: Gets a campaign's GUID by its id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignGuidByIdGet]
(
    -- Add the parameters for the stored procedure here
    @campaignId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT campaignGuid FROM campaigns WHERE campaignId = @campaignId
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignInfoGetByInviteGuid]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 07/04/2023
-- Description: Gets basic info about a campaign by its invite Guid
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignInfoGetByInviteGuid]
(
    -- Add the parameters for the stored procedure here
	@campaignInviteGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT campaignName, campaignGuid, campaignDescription, campaignCreationDate, isMunicipal, campaignLogoUrl, cityName, isCustomCampaign
	FROM campaigns
	LEFT JOIN cities 
	ON campaigns.cityId = cities.cityId
	WHERE campaignInviteGuid = @campaignInviteGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignInviteGuidDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 13/01/2023
-- Description: Deletes the invite Guid from a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignInviteGuidDelete]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    UPDATE campaigns SET campaignInviteGuid = NULL WHERE campaignGuid = @campaignGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignInviteGuidGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 13/01/2023
-- Description: Gets a campaign's invite GUID by the campaign's GUID
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignInviteGuidGet]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT campaignInviteGuid FROM campaigns WHERE campaignGuid = @campaignGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignInviteGuidUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 13/01/2023
-- Description: Adds or updates an invite link to a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignInviteGuidUpdate]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @inviteGuid uniqueidentifier = NEWID()

	-- Unlikely to ever run even once.
	WHILE EXISTS(SELECT campaignGuid FROM campaigns WHERE campaignInviteGuid = @inviteGuid)
		SET @inviteGuid = NEWID()

    -- Insert statements for procedure here
    UPDATE campaigns SET campaignInviteGuid = @inviteGuid WHERE campaignGuid = @campaignGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignNameByGuidGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/01/2023
-- Description: Gets the name of a campaign by its GUID
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignNameByGuidGet]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT campaignName FROM campaigns WHERE campaignGuid = @campaignGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignRolesGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Gets the list of all roles a campaign has
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignRolesGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT roleName, roleLevel, roleDescription, isCustomRole
	FROM roles 
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid) OR campaignId = -1;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignTypeAndCityByGuidGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 16/01/2023
-- Description: Gets the type of a campaign and its city by the campaign's GUID
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignTypeAndCityByGuidGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT isMunicipal, cityName, isCustomCampaign FROM campaigns JOIN cities ON campaigns.cityId = cities.cityId WHERE campaignGuid = @campaignGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 13/01/2023
-- Description: Updates a campaign's information
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignUpdate]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier,
    @campaignDescription nvarchar(500) = NULL,
	@campaignLogoUrl nvarchar(2083) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	UPDATE campaigns SET 
		campaignDescription = ISNULL(@campaignDescription, campaignDescription),
		campaignLogoUrl = ISNULL(@campaignLogoUrl, campaignLogoUrl)
	WHERE campaignGuid = @campaignGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CampaignUsersGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 19/01/2023
-- Description: Gets all the users in a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_CampaignUsersGet]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT email, firstNameEng, lastNameEng, firstNameHeb, lastNameHeb, profilePicUrl, roleName, roleLevel 
	FROM campaign_users 
	JOIN users
	ON campaign_users.userId = users.userId
	JOIN roles 
	ON campaign_users.roleId = roles.roleId
	WHERE campaign_users.campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CitiesGetAll]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 05/04/2023
-- Description: Gets the list of all cities in the cities table
-- =============================================
CREATE PROCEDURE [dbo].[usp_CitiesGetAll]
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT * FROM cities;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomBallotAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Adds a new custom ballot to the custom ballots table
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomBallotAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@cityName nvarchar(100) = NULL,
	@innerCityBallotId float,
	@ballotAddress nvarchar(100) = NULL,
	@ballotLocation nvarchar(100) = NULL,
	@accessible bit = NULL,
	@elligibleVoters int = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- CampaignNotFound status code
	IF @campaignId IS NULL RETURN 50017;

	-- DuplicateKey status code
	IF EXISTS(SELECT * FROM custom_ballots WHERE campaignId = @campaignId AND innerCityBallotId = @innerCityBallotId) RETURN 50001;


    INSERT INTO custom_ballots(cityName, innerCityBallotId, ballotAddress, ballotLocation, accessible, elligibleVoters, campaignId)
	VALUES (@cityName, @innerCityBallotId, @ballotAddress, @ballotLocation, @accessible, @elligibleVoters, @campaignId);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomBallotDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Deletes an existing custom ballot
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomBallotDelete]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@innerCityBallotId float
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- CampaignNotFound status code
	IF @campaignId IS NULL RETURN 50017;

	DELETE FROM custom_ballots
	WHERE campaignId = @campaignId AND innerCityBallotId = @innerCityBallotId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomBallotUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Updates an existing custom ballot
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomBallotUpdate]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@cityName nvarchar(100) = NULL,
	@innerCityBallotId float,
	@ballotAddress nvarchar(100) = NULL,
	@ballotLocation nvarchar(100) = NULL,
	@accessible bit = NULL,
	@elligibleVoters int = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- CampaignNotFound status code
	IF @campaignId IS NULL RETURN 50017;

	UPDATE custom_ballots
	SET     cityName = ISNULL(@cityName, cityName),
			ballotAddress = ISNULL(@ballotAddress, ballotAddress),
			ballotLocation = ISNULL(@ballotLocation, ballotLocation),
			accessible = ISNULL(@accessible, accessible),
			elligibleVoters = ISNULL(@elligibleVoters, elligibleVoters)
	WHERE innerCityBallotId = @innerCityBallotId AND campaignId = @campaignId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomVotersLedgerAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 16/04/2023
-- Description: Adds a new custom voters ledger to the database (not including the contents)
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomVotersLedgerAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@ledgerName nvarchar(100),
	@newLedgerGuid uniqueidentifier OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	-- Setting to Guid.Empty to avoid null value errors on server side in case of procedure failure
	SET @newLedgerGuid = '00000000-0000-0000-0000-000000000000'

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Campaign Not Found status code
	IF @campaignId IS NULL RETURN 50017;

	DECLARE @ledgerGuid UNIQUEIDENTIFIER = NEWID();

	WHILE EXISTS (SELECT * FROM custom_voters_ledgers WHERE ledgerGuid = @ledgerGuid)
		SET @ledgerGuid = NEWID();

	INSERT INTO custom_voters_ledgers(campaignId, ledgerGuid, ledgerName)
	VALUES (@campaignId, @ledgerGuid, @ledgerName);

	SET @newLedgerGuid = @ledgerGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomVotersLedgerDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 16/04/2023
-- Description: Deletes an existing custom voters ledger
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomVotersLedgerDelete]
(
    -- Add the parameters for the stored procedure here
	@ledgerGuid uniqueidentifier,
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @ledgerId int = (SELECT ledgerId FROM custom_voters_ledgers WHERE ledgerGuid = @ledgerGuid);

	-- Status code LedgerNotFound
	IF @ledgerId IS NULL RETURN 50027;

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	IF EXISTS(SELECT * FROM custom_voters_ledgers WHERE ledgerId = @ledgerId AND campaignId = @campaignId)
	BEGIN

		DELETE FROM custom_voters_ledgers
		WHERE ledgerId = @ledgerId;

	END

	-- Status code BoundaryViolation
	ELSE RETURN 50002;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomVotersLedgerFilter]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 22/04/2023
-- Description: Filters a custom voters ledger and returns all rows that match the filter
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomVotersLedgerFilter]
(
    -- Add the parameters for the stored procedure here
	@ledgerGuid uniqueidentifier,
	@identifier int = NULL,
	@cityName nvarchar(100) = NULL,
	@streetName nvarchar(100) = NULL,
	@ballotId float = 0,
	@supportStatus bit = NULL,
	@firstName nvarchar(100) = NULL,
	@lastName nvarchar(100) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT identifier, lastName, firstName, cityName, ballotId, streetName, houseNumber, entrance, appartment, houseLetter, zipCode,
			email1, email2, phone1, phone2, supportStatus
	FROM custom_voters_ledgers_content
	WHERE identifier = ISNULL(@identifier, identifier)
		AND (cityName = @cityName OR @cityName IS NULL)
		AND (streetName = @streetName OR @streetName IS NULL)
		AND (ballotId = @ballotId OR @ballotId IS NULL)
		AND (supportStatus = @supportStatus OR @supportStatus IS NULL)
		AND (firstName = @firstName OR @firstName IS NULL)
		AND (lastName = @lastName OR @lastName IS NULL)
		AND ledgerId = (SELECT ledgerId FROM custom_voters_ledgers WHERE ledgerGuid = @ledgerGuid);

END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomVotersLedgerGetForCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 16/04/2023
-- Description: Gets the name and guid of all of a campaign's custom voter ledgers
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomVotersLedgerGetForCampaign]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT ledgerName, ledgerGuid
	FROM custom_voters_ledgers
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomVotersLedgerImport]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 25/04/2023
-- Description: Imports a custom voters efficiently by having the ledger sent as JSON objects
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomVotersLedgerImport]
(
    -- Add the parameters for the stored procedure here
    @ledgerGuid uniqueidentifier,
    @jsonLedger NVARCHAR(MAX),
	@shouldDeleteOnUnmatch BIT = 0
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @ledgerId INT = (SELECT ledgerId FROM custom_voters_ledgers WHERE ledgerGuid = @ledgerGuid);

	-- First case: @shouldDeleteOnUnmatch is enabled, so there is a case to delete records that do not match.
	IF @shouldDeleteOnUnmatch = 1
	BEGIN

		WITH dst AS (SELECT ledgerId, identifier, firstName, lastName, cityName, ballotId, streetName, houseNumber, entrance, appartment,
													houseLetter, zipCode, email1, email2, phone1, phone2, supportStatus
					FROM custom_voters_ledgers_content
					WHERE ledgerId = @ledgerId)

		MERGE dst
		USING
		(
			SELECT @ledgerId AS ledgerId, identifier, firstName, lastName, cityName, ballotId, streetName, houseNumber, entrance, appartment,
													houseLetter, zipCode, email1, email2, phone1, phone2, supportStatus FROM OPENJSON(@jsonLedger) WITH(
			identifier INT '$.Identifier',
			firstName NVARCHAR(50) '$.FirstName',
			lastName NVARCHAR(50) '$.LastName',
			cityName NVARCHAR(50) '$.CityName',
			ballotId FLOAT '$.BallotId',
			streetName NVARCHAR(50) '$.StreetName',
			houseNumber INT '$.HouseNumber',
			entrance NVARCHAR(10) '$.Entrance',
			Appartment NVARCHAR(10) '$.Appartment',
			houseLetter NVARCHAR(5) '$.HouseLetter',
			zipCode INT '$.ZipCode',
			email1 NVARCHAR(200) '$.Email1',
			email2 NVARCHAR(200) '$.Email2',
			phone1 NVARCHAR(30) '$.Phone1',
			phone2 NVARCHAR(30) '$.Phone2',
			supportStatus BIT '$.SupportStatus'
			) 
		) AS src ON src.identifier = dst.identifier AND src.ledgerId = dst.ledgerId
		WHEN NOT MATCHED BY SOURCE THEN DELETE

		WHEN MATCHED THEN UPDATE SET dst.lastName = src.lastName, dst.firstName = src.firstName, dst.cityName = src.cityName,
									dst.ballotId = src.ballotId, dst.streetName = src.streetName, dst.houseNumber = src.houseNumber,
									dst.entrance = src.entrance, dst.appartment = src.appartment, dst.houseLetter = src.houseLetter,
									dst.zipCode = src.zipCode, dst.email1 = src.email1, dst.email2 = src.email2, dst.phone1 = src.phone1,
									dst.phone2 = src.phone2, dst.supportStatus = src.supportStatus

		WHEN NOT MATCHED THEN 
		INSERT (ledgerId, identifier, firstName, lastName, cityName, ballotId, streetName, houseNumber, entrance, appartment,
													houseLetter, zipCode, email1, email2, phone1, phone2, supportStatus)
		VALUES (src.ledgerId, src.identifier, src.firstName, src.lastName, src.cityName, src.ballotId, src.streetName, src.houseNumber, src.entrance, src.appartment,
												src.houseLetter, src.zipCode, src.email1, src.email2, src.phone1, src.phone2, src.supportStatus);

	END

	-- Otherwise, the code is a near copy of the first, just without the delete behaviour
	ELSE
	BEGIN
			MERGE custom_voters_ledgers_content AS dst
		USING
		(
			SELECT @ledgerId AS ledgerId, identifier, firstName, lastName, cityName, ballotId, streetName, houseNumber, entrance, appartment,
													houseLetter, zipCode, email1, email2, phone1, phone2, supportStatus FROM OPENJSON(@jsonLedger) WITH(
			identifier INT '$.Identifier',
			firstName NVARCHAR(50) '$.FirstName',
			lastName NVARCHAR(50) '$.LastName',
			cityName NVARCHAR(50) '$.CityName',
			ballotId FLOAT '$.BallotId',
			streetName NVARCHAR(50) '$.StreetName',
			houseNumber INT '$.HouseNumber',
			entrance NVARCHAR(10) '$.Entrance',
			Appartment NVARCHAR(10) '$.Appartment',
			houseLetter NVARCHAR(5) '$.HouseLetter',
			zipCode INT '$.ZipCode',
			email1 NVARCHAR(200) '$.Email1',
			email2 NVARCHAR(200) '$.Email2',
			phone1 NVARCHAR(30) '$.Phone1',
			phone2 NVARCHAR(30) '$.Phone2',
			supportStatus BIT '$.SupportStatus'
			) 
		) AS src ON src.identifier = dst.identifier AND src.ledgerId = dst.ledgerId

		WHEN MATCHED THEN UPDATE SET dst.lastName = src.lastName, dst.firstName = src.firstName, dst.cityName = src.cityName,
									dst.ballotId = src.ballotId, dst.streetName = src.streetName, dst.houseNumber = src.houseNumber,
									dst.entrance = src.entrance, dst.appartment = src.appartment, dst.houseLetter = src.houseLetter,
									dst.zipCode = src.zipCode, dst.email1 = src.email1, dst.email2 = src.email2, dst.phone1 = src.phone1,
									dst.phone2 = src.phone2, dst.supportStatus = src.supportStatus

		WHEN NOT MATCHED THEN 
		INSERT (ledgerId, identifier, firstName, lastName, cityName, ballotId, streetName, houseNumber, entrance, appartment,
													houseLetter, zipCode, email1, email2, phone1, phone2, supportStatus)
		VALUES (src.ledgerId, src.identifier, src.firstName, src.lastName, src.cityName, src.ballotId, src.streetName, src.houseNumber, src.entrance, src.appartment,
												src.houseLetter, src.zipCode, src.email1, src.email2, src.phone1, src.phone2, src.supportStatus);
	END

END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomVotersLedgerRowAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 22/04/2023
-- Description: Adds a new row to a custom voters ledger
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomVotersLedgerRowAdd]
(
    -- Add the parameters for the stored procedure here
	@ledgerGuid uniqueidentifier,
	@identifier int,
	@lastName nvarchar(50) = NULL,
	@firstName nvarchar(50) = NULL,
	@cityName nvarchar(50) = NULL,
	@ballotId int = NULL,
	@streetName nvarchar(50) = NULL,
	@houseNumber int = NULL,
	@entrance nvarchar(10) = NULL,
	@appartment nvarchar(10) = NULL,
	@houseLetter nvarchar(5) = NULL,
	@zipCode int = NULL,
	@email1 nvarchar(200) = NULL,
	@email2 nvarchar(200) = NULL,
	@phone1 nvarchar(200) = NULL,
	@phone2 nvarchar(200) = NULL,
	@supportStatus bit = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @ledgerId int = (SELECT ledgerId FROM custom_voters_ledgers WHERE ledgerGuid = @ledgerGuid);

	-- LedgerNotFound status code.
	IF @ledgerId IS NULL RETURN 50027;

	-- DuplicateKey status code
	IF EXISTS(SELECT * FROM custom_voters_ledgers_content WHERE ledgerId = @ledgerId AND identifier = @identifier) RETURN 50001;

	INSERT INTO custom_voters_ledgers_content(identifier, ledgerId, lastName, firstName, cityName, ballotId, streetName, houseNumber, entrance,
												appartment, houseLetter, zipCode, email1, email2, phone1, phone2, supportStatus)
	VALUES (@identifier, @ledgerId, @lastName, @firstName, @cityName, @ballotId, @streetName, @houseNumber, @entrance, @appartment, @houseLetter,
			@zipCode, @email1, @email2, @phone1, @phone2, @supportStatus);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomVotersLedgerRowDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 22/04/2023
-- Description: Deletes a specific row from a custom voters ledger
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomVotersLedgerRowDelete]
(
    -- Add the parameters for the stored procedure here
	@ledgerGuid uniqueidentifier,
	@identifier int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @ledgerId int = (SELECT ledgerId FROM custom_voters_ledgers WHERE ledgerGuid = @ledgerGuid);

	-- LedgerNotFound status code.
	IF @ledgerId IS NULL RETURN 50027;

	-- LedgerRowNotFound status code
	IF NOT EXISTS(SELECT * FROM custom_voters_ledgers_content WHERE ledgerId = @ledgerId AND identifier = @identifier) RETURN 50028;

	DELETE FROM custom_voters_ledgers_content
	WHERE ledgerId = @ledgerId AND identifier = @identifier;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomVotersLedgerRowUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 22/04/2023
-- Description: Updates an existing row in a custom voters ledger
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomVotersLedgerRowUpdate]
(
    -- Add the parameters for the stored procedure here
	@ledgerGuid uniqueidentifier,
	@identifier int,
	@lastName nvarchar(50) = NULL,
	@firstName nvarchar(50) = NULL,
	@cityName nvarchar(50) = NULL,
	@ballotId float = NULL,
	@streetName nvarchar(50) = NULL,
	@houseNumber int = NULL,
	@entrance nvarchar(10) = NULL,
	@appartment nvarchar(10) = NULL,
	@houseLetter nvarchar(5) = NULL,
	@zipCode int = NULL,
	@email1 nvarchar(200) = NULL,
	@email2 nvarchar(200) = NULL,
	@phone1 nvarchar(200) = NULL,
	@phone2 nvarchar(200) = NULL,
	@supportStatus bit = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @ledgerId int = (SELECT ledgerId FROM custom_voters_ledgers WHERE ledgerGuid = @ledgerGuid);

	-- LedgerNotFound status code.
	IF @ledgerId IS NULL RETURN 50027;

	-- LedgerRowNotFound status code
	IF NOT EXISTS(SELECT * FROM custom_voters_ledgers_content WHERE ledgerId = @ledgerId AND identifier = @identifier) RETURN 50028;

	UPDATE custom_voters_ledgers_content
	SET		lastName = ISNULL(@lastName, lastName),
			firstName = ISNULL(@firstName, firstName),
			cityName = ISNULL(@cityName, cityName),
			ballotId = ISNULL(@ballotId, ballotId),
			streetName = ISNULL(@streetName, streetName),
			houseNumber = ISNULL(@houseNumber, houseNumber),
			entrance = ISNULL(@entrance, entrance),
			appartment = ISNULL(@appartment, appartment),
			houseLetter = ISNULL(@houseLetter, houseLetter),
			zipCode = ISNULL(@zipCode, zipCode),
			email1 = ISNULL(@email1, email1),
			email2 = ISNULL(@email2, email2),
			phone1 = ISNULL(@phone1, phone1),
			phone2 = ISNULL(@phone2, phone2),
			supportStatus = ISNULL(@supportStatus, supportStatus)
	WHERE  ledgerId = @ledgerId AND identifier = @identifier;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CustomVotersLedgerUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 16/04/2023
-- Description: Updates an existing custom voter's ledger's name
-- =============================================
CREATE PROCEDURE [dbo].[usp_CustomVotersLedgerUpdate]
(
    -- Add the parameters for the stored procedure here
	@ledgerGuid uniqueidentifier,
	@newLedgerName nvarchar(100),
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @ledgerId int = (SELECT ledgerId FROM custom_voters_ledgers WHERE ledgerGuid = @ledgerGuid);

	-- Status code LedgerNotFound
	IF @ledgerId IS NULL RETURN 50027;

		DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	IF EXISTS(SELECT * FROM custom_voters_ledgers WHERE ledgerId = @ledgerId AND campaignId = @campaignId)
	BEGIN

		UPDATE custom_voters_ledgers
		SET ledgerName = @newLedgerName
		WHERE ledgerId = @ledgerId;

	END

	-- Status code BoundaryViolation
	ELSE RETURN 50002;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteExpiredCodes]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 25/01/2023
-- Description: Deletes expired verification codes from the database
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteExpiredCodes]

AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DELETE FROM phone_verification_codes WHERE expires < GETDATE();
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 20/02/2023
-- Description: Adds a new event to the custom_events table
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventAdd]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@eventName nvarchar(100),
	@eventDescription nvarchar(500) = NULL,
	@eventLocation nvarchar(100) = NULL,
	@eventStartTime datetime = NULL,
	@eventEndTime datetime = NULL,
	@campaignGuid uniqueidentifier = NULL,
	@maxAttendees int = NULL,
	@newEventGuid uniqueidentifier OUTPUT,
	@newEventId int OUTPUT,
	@isOpenJoin bit = 0,
	@eventOf int = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = NULL;

	IF @campaignGuid IS NOT NULL
	BEGIN

		SET @campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

		-- Status code for campaign not found
		IF @campaignId IS NULL 
		BEGIN

		-- Some values have to be returned, or else the server has an exception due to casting null to non nullable DB types, and that's a bad time.
			SET @newEventGuid = '00000000-0000-0000-0000-000000000000'
			SET @newEventId = 0
			
			RETURN 50017

		END

	END

	DECLARE @eventGuid uniqueidentifier = NEWID();

	WHILE EXISTS(SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid)
		SET @eventGuid = NEWID();

	INSERT INTO custom_events(eventGuid, eventName, eventDescription, eventLocation, eventStartTime, eventEndTime, campaignId, maxAttendees, eventCreatorId, isOpenJoin, eventOf)
	VALUES (@eventGuid, @eventName, @eventDescription, @eventLocation, @eventStartTime, @eventEndTime, @campaignId, @maxAttendees, @userId, @isOpenJoin, @eventOf);

	SET @newEventGuid = @eventGuid;
	SET @newEventId = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventAddWatcher]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 21/02/2023
-- Description: Adds a new watcher to an event. Watchers are not participants, and do not count towards the current numAttending of the event.
-- Also removes the user as a participant in the event if they are already one.
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventAddWatcher]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@eventGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON

    -- Insert statements for procedure here
	DECLARE @eventId int = (SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid);

	-- Status code EventNotFound
	IF @eventId IS NULL RETURN 50018;

	-- Status code DuplicateKey
	IF EXISTS(SELECT * FROM events_watchers WHERE userId = @userId AND eventId = @eventId) RETURN 50001;

	-- Transaction to ensure data integrity in case of unexpected critical error
	BEGIN TRAN T;

		INSERT INTO events_watchers(userId, eventId)
		VALUES (@userId, @eventId);

		IF EXISTS(SELECT * FROM users_events WHERE userId = @userId AND eventId = @eventId)
		BEGIN

			DELETE FROM users_events
			WHERE userId = @userId AND eventId = @eventId;

			UPDATE custom_events
			SET numAttending = numAttending - 1
			WHERE eventId = @eventId;

		END

	COMMIT TRAN T;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventAssignTo]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 21/02/2023
-- Description: Assigns a user to an event, via either their email (when assigning someone else) or their user id (when assigning self).
-- Also removes the user from being a watcher of the event (as they are now a participant), so that they will count towards attendance.
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventAssignTo]
(
    -- Add the parameters for the stored procedure here
	@userId int = NULL,
	@userEmail nvarchar(500) = NULL,
	@eventGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON

    -- Insert statements for procedure here


	-- Status code ParameterMustNotBeNullOrEmpty
    IF @userId IS NULL AND @userEmail IS NULL RETURN 50016;
	
	-- Status code TooManyValuesProvided
	IF @userId IS NOT NULL AND @userEmail IS NOT NULL RETURN 50019;

	DECLARE @eventId int = (SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid);

	-- Status code EventNotFound
	IF @eventId IS NULL RETURN 50018;

	IF EXISTS(SELECT eventId 
				FROM custom_events 
				WHERE maxAttendees IS NOT NULL
					AND numAttending >= maxAttendees
					AND eventId = @eventId)
		-- Status code EventAlreadyFull
		RETURN 50020;

	-- If @userEmail is not null at this point, then userId is null and needs to be set. Else, use the provided userId.
	IF @userEmail IS NOT NULL SET @userId = (SELECT userId FROM users WHERE email = @userEmail);

	-- Status code UserNotFound
	IF @userId IS NULL RETURN 50007;

	-- Status code DuplicateKey
	IF EXISTS(SELECT * FROM users_events WHERE userId = @userId AND eventId = @eventId) RETURN 50001;

	-- Transaction to ensure data integrity in case of unpredicted critical failure
	BEGIN TRAN T;

		INSERT INTO users_events(userId, eventId)
		VALUES (@userId, @eventId);

		UPDATE custom_events
		SET numAttending = numAttending + 1
		WHERE eventId = @eventId

		DELETE FROM events_watchers
		WHERE eventId = @eventId AND userId = @userId;

	COMMIT TRAN T;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 20/02/2023
-- Description: Deletes an event from the custom_events table
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventDelete]
(
    -- Add the parameters for the stored procedure here
	@eventGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    IF NOT EXISTS(SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid)
		-- Status code for event not found
		RETURN 50018

	DELETE FROM custom_events
	WHERE eventGuid = @eventGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 23/02/2023
-- Description: Gets a single event's details
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventGet]
(
    -- Add the parameters for the stored procedure here
    @eventGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT eventGuid, eventName, eventDescription, eventLocation, eventStartTime, eventEndTime, maxAttendees, numAttending,
			firstNameHeb, lastNameHeb, displayNameEng, profilePicUrl, email, phoneNumber, isOpenJoin, campaignGuid
	FROM custom_events
	LEFT JOIN users
	ON userId = eventCreatorId
	LEFT JOIN campaigns
	ON campaigns.campaignId = custom_events.campaignId
	WHERE eventGuid = @eventGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventGetParticipants]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 23/02/2023
-- Description: Gets the list of participants in an event
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventGetParticipants]
(
    -- Add the parameters for the stored procedure here
	@eventGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @eventId int = (SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid);

	-- Status code EventNotFound
	IF @eventId IS NULL RETURN 50018;

	SELECT firstNameHeb, lastNameHeb, displayNameEng, profilePicUrl, email, phoneNumber
	FROM users_events
	INNER JOIN users
	ON users.userId = users_events.userId
	WHERE eventId = @eventId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventGetWatchers]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 03/03/2023
-- Description: Gets all the watchers of a specific event
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventGetWatchers]
(
    -- Add the parameters for the stored procedure here
	@eventGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @eventId int = (SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid);

	-- Status code EventNotFound
	IF @eventId IS NULL RETURN 50018;

    SELECT firstNameHeb, lastNameHeb, displayNameEng, profilePicUrl, email, phoneNumber
	FROM events_watchers
	INNER JOIN users
	ON events_watchers.userId = users.userId
	WHERE eventId = @eventId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventParticipationDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 21/02/2023
-- Description: Removes a user's participation in an event
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventParticipationDelete]
(
    -- Add the parameters for the stored procedure here
	@userId int = NULL,
	@eventGuid uniqueidentifier,
	@userEmail nvarchar(500) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	-- Status code ParameterMustNotBeNullOrEmpty
    IF @userId IS NULL AND @userEmail IS NULL RETURN 50016;
	
	-- Status code TooManyValuesProvided
	IF @userId IS NOT NULL AND @userEmail IS NOT NULL RETURN 50019;

	DECLARE @eventId int = (SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid);

	-- Status code EventNotFound
	IF @eventId IS NULL RETURN 50018;

	-- If @userEmail is not null at this point, then userId is null and needs to be set. Else, use the provided userId.
	IF @userEmail IS NOT NULL SET @userId = (SELECT userId FROM users WHERE email = @userEmail);

	-- Status code UserNotFound
	IF @userId IS NULL RETURN 50007;

	-- Transaction to ensure data integrity in case of critical error
	BEGIN TRAN T;

			DELETE FROM users_events
			WHERE userId = @userId AND eventId = @eventId;

			UPDATE custom_events
			SET numAttending = numAttending - 1
			WHERE eventId = @eventId;

	COMMIT TRAN T;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventScheduleManagedUsersGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/02/2023
-- Description: Gets the list of all users for which the given user is a schedule manager of
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventScheduleManagedUsersGet]
(
    -- Add the parameters for the stored procedure here
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT firstNameHeb, lastNameHeb, displayNameEng, profilePicUrl, email, phoneNumber
	FROM users_schedule_managers
	INNER JOIN users
	ON userId = permissionGiver
	WHERE permissionReceiver = @userId
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventScheduleManagerAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/02/2023
-- Description: Adds a new schedule manager to a user
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventScheduleManagerAdd]
(
    -- Add the parameters for the stored procedure here
	@giverUserId int,
	@receiverEmail nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @permissionReceiver int = (SELECT userId FROM users WHERE email = @receiverEmail);

	-- Status code UserNotFound
	IF @permissionReceiver IS NULL RETURN 50007;

    -- Status code DuplicateKey
    IF EXISTS (SELECT * FROM users_schedule_managers WHERE permissionReceiver = @permissionReceiver AND permissionGiver = @giverUserId) RETURN 50001;

	INSERT INTO users_schedule_managers(permissionGiver, permissionReceiver)
	VALUES (@giverUserId, @permissionReceiver);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventScheduleManagerRemove]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/02/2023
-- Description: Removes a schedule manager for a specific user
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventScheduleManagerRemove]
(
    -- Add the parameters for the stored procedure here
	@giverUserId int,
	@receiverEmail nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @permissionReceiver int = (SELECT userId FROM users WHERE email = @receiverEmail);

	-- Status code UserNotFound;
	IF @permissionReceiver IS NULL RETURN 50007;

	DELETE FROM users_schedule_managers
	WHERE permissionGiver = @giverUserId AND permissionReceiver = @permissionReceiver;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventScheduleManagersGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/02/2023
-- Description: Gets all users who can add events to a different user's schedule, for that specific user
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventScheduleManagersGet]
(
    -- Add the parameters for the stored procedure here
	@userEmail nvarchar(200) = NULL,
	@userId int = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	--Status code ParameterMustNotBeNullOrEmpty
	IF @userEmail IS NULL AND @userId IS NULL RETURN 50016;

	--Status code TooManyValuesProvided
	IF @userEmail IS NOT NULL AND @userId IS NOT NULL RETURN 50019;

    -- Insert statements for procedure here
    SELECT userId, email, displayNameEng, profilePicUrl, phoneNumber, firstNameHeb, lastNameHeb
	FROM users_schedule_managers
	INNER JOIN users
	ON userId = permissionReceiver
	WHERE permissionGiver = (SELECT userId FROM users WHERE email = @userEmail)
			OR permissionGiver = @userId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventsGetCreatorUserId]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 23/02/2023
-- Description: Gets the user id of the creator of a specific event.
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventsGetCreatorUserId]
(
    -- Add the parameters for the stored procedure here
    @eventGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT userId
	FROM custom_events
	INNER JOIN users
	ON userId = eventCreatorId
	WHERE eventGuid = @eventGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventsGetForCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 23/02/2023
-- Description: Gets all events of a specific campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventsGetForCampaign]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT eventGuid, eventName, eventDescription, eventLocation, eventStartTime, eventEndTime, maxAttendees, numAttending, email,
			firstNameEng, firstNameHeb, displayNameEng, profilePicUrl, phoneNumber, isOpenJoin
	FROM custom_events
	LEFT JOIN users
	ON userId = eventCreatorId
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventsGetForUser]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 21/02/2023
-- Description: Gets all the events a single user is assigned to
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventsGetForUser]
(
    -- Add the parameters for the stored procedure here
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT eventGuid, eventName, eventDescription, eventLocation, eventStartTime, eventEndTime, maxAttendees,
			unionized.participating, campaignName, campaignGuid, campaignLogoUrl, isOpenJoin, numAttending
	FROM (SELECT *, 1 AS participating FROM users_events UNION SELECT *, 0 AS participating FROM events_watchers) AS unionized
	INNER JOIN custom_events
	ON custom_events.eventId = unionized.eventId
	LEFT JOIN campaigns
	ON campaigns.campaignId = custom_events.campaignId
	WHERE userId = @userId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventsGetPersonal]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/03/2023
-- Description: Gets all of a user's personal events
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventsGetPersonal]
(
    -- Add the parameters for the stored procedure here
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	SELECT eventGuid, eventName, eventDescription, eventLocation, eventStartTime, eventEndTime, maxAttendees,
			numAttending, email, firstNameEng, firstNameHeb, displayNameEng, profilePicUrl, phoneNumber, isOpenJoin
	FROM custom_events
	INNER JOIN users
	ON eventCreatorId = userId
	WHERE eventOf = @userId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 20/02/2023
-- Description: Updates an existing event
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventUpdate]
(
    -- Add the parameters for the stored procedure here
	@eventDescription nvarchar(500) = NULL,
	@eventLocation nvarchar(100) = NULL,
	@eventStartTime datetime = NULL,
	@eventEndTime datetime = NULL,
	@campaignGuid uniqueidentifier = NULL,
	@maxAttendees int = NULL,
	@eventName nvarchar(100) = NULL,
	@eventGuid uniqueidentifier,
	@isOpenJoin bit = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = NULL;

	IF @campaignGuid IS NOT NULL
	BEGIN

		SET @campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

		-- Status code for campaign not found
		IF @campaignId IS NULL RETURN 50017

	END

	IF NOT EXISTS(SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid)
		-- Status code for event not found
		RETURN 50018;

	UPDATE custom_events

	SET		eventDescription = ISNULL(@eventDescription, eventDescription),
			eventLocation = ISNULL(@eventLocation, eventLocation),
			eventStartTime = ISNULL(@eventStartTime, eventStartTime),
			eventEndTime = ISNULL(@eventEndTime, eventEndTime),
			campaignId = ISNULL(@campaignId, campaignId),
			maxAttendees = ISNULL(@maxAttendees, maxAttendees),
			eventName = ISNULL(@eventName, eventName),
			isOpenJoin = ISNULL(@isOpenJoin, isOpenJoin)

	WHERE  eventGuid = @eventGuid
END
GO
/****** Object:  StoredProcedure [dbo].[usp_EventWatcherDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 21/02/2023
-- Description: Removes a watcher from an event
-- =============================================
CREATE PROCEDURE [dbo].[usp_EventWatcherDelete]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@eventGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @eventId int = (SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid);

	-- Status code EventNotFound
	IF @eventId IS NULL RETURN 50018;

	DELETE FROM events_watchers
	WHERE userId = @userId AND eventId = @eventId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FinancialDataAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/03/2023
-- Description: Adds a new financial data entry for a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_FinancialDataAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid UNIQUEIDENTIFIER,
	@typeGuid UNIQUEIDENTIFIER,
	@amount FLOAT,
	@isExpense BIT,
	@dateCreated DATETIME,
	@dataTitle NVARCHAR(50),
	@dataDescription NVARCHAR(500),
	@creatorUserId INT,
	@newDataGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here

	-- Setting the output to Guid.Empty to avoid errors from this being null when returned in case of failure.
	SET @newDataGuid = '00000000-0000-0000-0000-000000000000'

    DECLARE @campaignId INT = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	-- Status code UserNotFound
	IF NOT EXISTS (SELECT userId FROM users WHERE userId = @creatorUserId) RETURN 50007;

	DECLARE @typeId INT = (SELECT typeId FROM financial_types WHERE typeGuid = @typeGuid);

	-- Status code FinancialTypeNotFound
	IF @typeId IS NULL RETURN 50024;

	SET @newDataGuid = NEWID();

	WHILE EXISTS (SELECT dataGuid FROM financial_data WHERE dataGuid = @newDataGuid)
		SET @newDataGuid = NEWID();

	INSERT INTO financial_data(campaignId, amount, financialType, isExpense, dataGuid, dateCreated, dataTitle, dataDescription, creatorUserId)
	VALUES (@campaignId, @amount, @typeId, @isExpense, @newDataGuid, ISNULL(@dateCreated, GETDATE()), @dataTitle, @dataDescription, @creatorUserId)
	
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FinancialDataDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/03/2023
-- Description: Deletes a financial data entry.
-- =============================================
CREATE PROCEDURE [dbo].[usp_FinancialDataDelete]
(
    -- Add the parameters for the stored procedure here
	@dataGuid UNIQUEIDENTIFIER
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	-- Status code FinancialDataNotFound
    IF NOT EXISTS(SELECT dataGuid FROM financial_data WHERE dataGuid = @dataGuid) RETURN 50026;

	DELETE FROM financial_data
	WHERE dataGuid = @dataGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FinancialDataGetForCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/03/2023
-- Description: Gets all financial data for a campaign, ordered by date. If typeGuid is provided, will be limited only to that type.
-- =============================================
CREATE PROCEDURE [dbo].[usp_FinancialDataGetForCampaign]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid UNIQUEIDENTIFIER,
	@typeGuid UNIQUEIDENTIFIER = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT amount, isExpense, dataGuid, dateCreated, dataTitle, dataDescription, typeName, typeGuid,
			email, firstNameHeb, lastNameHeb, displayNameEng, phoneNumber, profilePicUrl
	FROM financial_data
	INNER JOIN financial_types 
	ON financial_data.financialType = financial_types.typeId
	LEFT JOIN users 
	ON financial_data.creatorUserId = users.userId
	WHERE financial_data.campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)
		  AND financial_types.typeGuid = ISNULL(@typeGuid, typeGuid)
	ORDER BY dateCreated DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FinancialDataUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/03/2023
-- Description: Updates an existing financial data entry. All values left as null will not be updated.
-- =============================================
CREATE PROCEDURE [dbo].[usp_FinancialDataUpdate]
(
    -- Add the parameters for the stored procedure here
	@dataGuid UNIQUEIDENTIFIER,
	@typeGuid UNIQUEIDENTIFIER = NULL,
	@amount FLOAT = NULL,
	@isExpense BIT = NULL,
	@dateCreated DATETIME = NULL,
	@dataTitle NVARCHAR(50) = NULL,
	@dataDescription NVARCHAR(500) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @typeId INT = NULL;

	IF @typeGuid IS NOT NULL
	BEGIN

		SET @typeId = (SELECT typeId FROM financial_types WHERE typeGuid = @typeGuid);

		-- Status code FinancialTypeNotFound
		IF @typeId IS NULL RETURN 50024;

	END

	-- Status code FinancialDataNotFound
	IF NOT EXISTS (SELECT dataGuid FROM financial_data WHERE dataGuid = @dataGuid) RETURN 50026;

	UPDATE financial_data

	SET		financialType = ISNULL(@typeId, financialType),
			amount = ISNULL(@amount, amount),
			isExpense = ISNULL(@isExpense, isExpense),
			dateCreated = ISNULL(@dateCreated, dateCreated),
			dataTitle = ISNULL(@dataTitle, dataTitle),
			dataDescription = ISNULL(@dataDescription, dataDescription)

	WHERE	dataGuid = @dataGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FinancialSummaryGetForCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/03/2023
-- Description: Gets a campaign's financial summary. - sum of incomes and expenses and total balance - divided by types
-- =============================================
CREATE PROCEDURE [dbo].[usp_FinancialSummaryGetForCampaign]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid UNIQUEIDENTIFIER
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT typeName, isExpense, SUM(amount) AS totalAmount, typeGuid
	FROM financial_data
	INNER JOIN financial_types
	ON typeId = financialType
	WHERE financial_data.campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)
	GROUP BY typeName, isExpense, typeGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FinancialTypeAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/03/2023
-- Description: Adds a new financial type for a campaign. Financial types are a way to collect expense and income records under a single common type, for better sorting.
-- =============================================
CREATE PROCEDURE [dbo].[usp_FinancialTypeAdd]
(
    -- Add the parameters for the stored procedure here
	@typeName NVARCHAR(100),
	@campaignGuid UNIQUEIDENTIFIER,
	@typeDescription NVARCHAR(300) = NULL,
	@newTypeGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	-- Settings the output parameter to Guid.Empty
	SET @newTypeGuid = '00000000-0000-0000-0000-000000000000'

    DECLARE @campaignId INT = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	-- Status code TooManyEntries
	IF (SELECT COUNT (*) FROM financial_types WHERE campaignId = @campaignId) >= 100 RETURN 50004;

	SET @newTypeGuid = NEWID();

	WHILE EXISTS(SELECT typeId FROM financial_types WHERE typeGuid = @newTypeGuid)
		SET @newTypeGuid = NEWID();

	INSERT INTO financial_types(typeName, campaignId, typeGuid, typeDescription)
	VALUES (@typeName, @campaignId, @newTypeGuid, @typeDescription);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FinancialTypeDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/03/2023
-- Description: Deletes a financial type for a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_FinancialTypeDelete]
(
    -- Add the parameters for the stored procedure here
	@typeGuid UNIQUEIDENTIFIER
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here

	
	DECLARE @typeId INT = (SELECT typeId FROM financial_types WHERE typeGuid = @typeGuid);

	-- Status code FinancialTypeNotFound
    IF @typeId IS NULL RETURN 50024;

	-- Status code SqlIllegalValue - deleting the "other" type is absolutely not allowed
	IF @typeId = 1 RETURN 50025;

	DELETE FROM financial_types
	WHERE typeId = @typeId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FinancialTypesGetForCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/03/2023
-- Description: Gets all of a campaign's defined financial types
-- =============================================
CREATE PROCEDURE [dbo].[usp_FinancialTypesGetForCampaign] (
	-- Add the parameters for the stored procedure here
	@campaignGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON

	-- Insert statements for procedure here
	SELECT typeName, typeGuid, typeDescription
	FROM financial_types
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid) OR campaignId = -1;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FinancialTypeUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/03/2023
-- Description: Updates an existing financial type
-- =============================================
CREATE PROCEDURE [dbo].[usp_FinancialTypeUpdate] (
	-- Add the parameters for the stored procedure here
	@typeGuid UNIQUEIDENTIFIER,
	@typeName NVARCHAR(100) = NULL,
	@typeDescription NVARCHAR(300) = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON

	-- Insert statements for procedure here
	-- Status code FinancialTypeNotFound
	DECLARE @typeId INT = (SELECT typeId FROM financial_types WHERE typeGuid = @typeGuid);

	-- Status code FinancialTypeNotFound
    IF @typeId IS NULL RETURN 50024;

	-- Status code SqlIllegalValue - deleting the "other" type is absolutely not allowed
	IF @typeId = 1 RETURN 50025;

	UPDATE financial_types
	SET typeName = ISNULL(@typeName, typeName)
	   ,typeDescription = ISNULL(@typeDescription, typeDescription)
	WHERE typeId = @typeId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetVoteCounts]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Gets the current vote counts for all parties across all ballots
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetVoteCounts]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT ballotId, partyId, isCustomBallot, numVotes
	FROM votes
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_IsUserInCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 13/01/2023
-- Description: Checks if a user is already part of a campaign or not
-- =============================================
CREATE PROCEDURE [dbo].[usp_IsUserInCampaign]
(
    -- Add the parameters for the stored procedure here
    @userId int,
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
	IF EXISTS(SELECT * FROM campaign_users WHERE campaignId = @campaignId AND userId = @userId) 
		RETURN 1;
	ELSE 
		RETURN 0;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      usp_AddJob
-- Create Date: 27/01/2023
-- Description: Adds a new job for a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobName nvarchar(50),
	@jobDescription nvarchar(200) = NULL,
	@jobLocation nvarchar(100) = NULL,
	@jobStartTime datetime = NULL,
	@jobEndTime datetime = NULL,
	@jobDefaultSalary int = 0,
	@peopleNeeded int = 1,
	@jobTypeName nvarchar(50) = 'Other',
	@newJobGuid uniqueidentifier OUTPUT,
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON;

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
	DECLARE @jobGuid UNIQUEIDENTIFIER = NEWID()

	-- Unlikely 
	WHILE EXISTS(SELECT * FROM jobs WHERE campaignId = @campaignId AND jobGuid = @jobGuid)
		SET @jobGuid = NEWID()

	BEGIN TRAN T;

		INSERT 
		INTO jobs(campaignId, jobGuid, jobName, jobDescription, jobLocation,
			jobStartTime, jobEndTime, jobDefaultSalary, peopleNeeded, jobTypeId)
		VALUES (@campaignId, @jobGuid, @jobName, @jobDescription,
				@jobLocation, @jobStartTime, @jobEndTime, @jobDefaultSalary, @peopleNeeded,
				(SELECT jobTypeId FROM job_types WHERE jobTypeName = @jobTypeName AND (campaignId = @campaignId OR campaignId = -1)));

		SET @newJobGuid = @jobGuid;

		INSERT INTO job_assign_capable_users(jobId, userId)
		VALUES (SCOPE_IDENTITY(), @userId);

	COMMIT TRAN T;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobAssign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/01/2023
-- Description: Assigns a user to a job by adding them and the job to the job_assignments table
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobAssign]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier,
	@userEmail nvarchar(200),
	@salary int = NULL,
	@assigningUserId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @userId int = (SELECT userId FROM users WHERE email = @userEmail);

	-- User not found status code
	IF @userId IS NULL RETURN 50007;

	DECLARE @jobId int = (SELECT jobId 
							FROM jobs 
							WHERE jobGuid = @jobGuid AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid));

	-- Job not found status code
	IF @jobId IS NULL RETURN 50012;

	-- Job fully manned status code
	IF (SELECT peopleNeeded FROM jobs WHERE jobId = @jobId) <= (SELECT peopleAssigned FROM jobs WHERE jobId = @jobId) RETURN 50014;

	-- Duplicate key status code
	IF EXISTS(SELECT * FROM job_assignments WHERE jobId = @jobId AND userId = @userId) RETURN 50001;

	BEGIN TRAN T;

		INSERT INTO job_assignments(jobId, userId, salary, assignedBy)
		VALUES (
				@jobId,
				@userId,
				CASE WHEN @salary IS NULL THEN (SELECT jobDefaultSalary FROM jobs WHERE jobId = @jobId) ELSE @salary END,
				@assigningUserId
				);

		UPDATE jobs
		SET peopleAssigned = peopleAssigned + 1
		WHERE jobId = @jobId;

	COMMIT TRAN T;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobAssignCapableUsersAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/01/2023
-- Description: Adds a new user who can assign the job to people to the table for it
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobAssignCapableUsersAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier,
	@userEmail nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @userId int = (SELECT userId FROM users WHERE email = @userEmail);

	IF @userId IS NULL RETURN 50007;

	DECLARE @jobId int = (SELECT jobId FROM jobs WHERE jobGuid = @jobGuid 
							AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid));

	IF @jobId IS NULL RETURN 50012;

	IF EXISTS(SELECT * FROM job_assign_capable_users WHERE jobId = @jobId AND userId = @userId) RETURN 50001;

	INSERT INTO job_assign_capable_users(jobId, userId)
	VALUES (@jobId, @userId);

END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobAssignCapableUsersGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/01/2023
-- Description: Gets the list of users capable of assigning to a particular job, including the ones that can assign to it via its job type if that option is chosen.
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobAssignCapableUsersGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier,
	@viaJobType bit = 1
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @jobId int = (SELECT jobId 
							FROM jobs 
							WHERE jobGuid = @jobGuid 
								AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid));

    IF @viaJobType = 1
	BEGIN

		DECLARE @jobTypeId int = (SELECT jobTypeId FROM jobs WHERE jobId = @jobId);

		SELECT users.userId, email, displayNameEng, firstNameHeb, lastNameHeb, profilePicUrl, phoneNumber
		FROM job_assign_capable_users
		INNER JOIN users
		ON job_assign_capable_users.userId = users.userId
		LEFT JOIN job_type_assign_capable_users
		ON jobTypeId = @jobTypeId
		WHERE jobId = @jobId OR jobTypeId = @jobTypeId;

	END

	ELSE

	BEGIN

		SELECT users.userId, email, displayNameEng, firstNameHeb, lastNameHeb, profilePicUrl, phoneNumber
		FROM job_assign_capable_users
		INNER JOIN users
		ON job_assign_capable_users.userId = users.userId
		WHERE jobId = @jobId;

	END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobAssignCapableUsersRemove]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/01/2023
-- Description: Removes a user from being able to assign to a specific job
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobAssignCapableUsersRemove]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier,
	@userEmail nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DELETE FROM job_assign_capable_users
	WHERE jobId = (SELECT jobId 
					FROM jobs 
					WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid) 
					AND jobGuid = @jobGuid)
		  AND userId = (SELECT userId FROM users WHERE email = @userEmail);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobAssignedUsersGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/01/2023
-- Description: Gets all users assigned to a specific job
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobAssignedUsersGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT displayNameEng, firstNameHeb, lastNameHeb, profilePicUrl, salary, email, phoneNumber
	FROM job_assignments
	INNER JOIN users
	ON job_assignments.userId = users.userId
	WHERE jobId = (SELECT jobId 
					FROM jobs 
					WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)
						  AND jobGuid = @jobGuid
					)
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobAssignmentRemove]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/01/2023
-- Description: Removes a user from being assigned to a job
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobAssignmentRemove]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier,
	@userEmail nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @jobId int = (SELECT jobId 
						FROM jobs 
						WHERE jobGuid = @jobGuid
								AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid));

	-- Job not found status code
	IF @jobId IS NULL RETURN 50012;

	DECLARE @userId int = (SELECT userId FROM users WHERE email = @userEmail);

	-- User not found status code
	IF @userId IS NULL RETURN 50007;

    BEGIN TRAN T

		DELETE FROM job_assignments
		WHERE jobId = @jobId AND userId = @userId;

		UPDATE jobs
		SET peopleAssigned = peopleAssigned - 1
		WHERE jobId = @jobId;

	COMMIT TRAN T
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobAssignmentUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 29/01/2023
-- Description: Updates a user's salary in the job assignments table
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobAssignmentUpdate]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier,
	@userEmail nvarchar(200),
	@salary int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
        DECLARE @userId int = (SELECT userId FROM users WHERE email = @userEmail);

	-- User not found status code
	IF @userId IS NULL RETURN 50007;

	DECLARE @jobId int = (SELECT jobId 
							FROM jobs 
							WHERE jobGuid = @jobGuid AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid));

	-- Job not found status code
	IF @jobId IS NULL RETURN 50012;

	UPDATE job_assignments
	SET salary = @salary
	WHERE jobId = @jobId AND userId = @userId;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/01/2023
-- Description: Deletes a job from a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobDelete]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DELETE FROM jobs
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)
			AND jobGuid = @jobGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/01/2023
-- Description: Gets a single job by its Guid and campaign Guid
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT jobGuid, jobName, jobDescription, jobLocation, jobStartTime, jobEndTime, jobDefaultSalary, jobTypeName, peopleNeeded, peopleAssigned
	FROM jobs
	INNER JOIN job_types
	ON job_types.jobTypeId = jobs.jobTypeId
	WHERE jobs.campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)
			AND jobGuid = @jobGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobsByFilterGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/01/2023
-- Description: Gets the list of either not fully manned jobs or fully manned jobs
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobsByFilterGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@fullyManned bit = NULL,
	@jobName nvarchar(50) = NULL,
	@jobStartTime datetime = NULL,
	@timeFromStart bit = 1,
	@timeBeforeStart bit = 0,
	@jobEndTime datetime = NULL,
	@timeBeforeEnd bit = 1,
	@timeFromEnd bit = 0,
	@jobLocation nvarchar(100) = NULL,
	@jobTypeName nvarchar(50) = NULL,
	@onlyCustomJobTypes bit = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
    -- Insert statements for procedure here
    SELECT jobTypeName, jobName, jobDescription, jobDefaultSalary, jobGuid, jobLocation, jobStartTime, jobEndTime, jobDefaultSalary, peopleNeeded, peopleAssigned
	FROM jobs
	INNER JOIN job_types
	ON job_types.jobTypeId = jobs.jobTypeId
	WHERE jobs.campaignId = @campaignId
			-- @fullyManned is null - do not filter by that, else filter by the requirement
			AND (
					(@fullyManned IS NULL AND 1=1) 
					OR (@fullyManned = 0 AND peopleAssigned < peopleNeeded) 
					OR (@fullyManned = 1 AND peopleNeeded = peopleAssigned)
				)

			AND jobName = CASE WHEN @jobName IS NULL THEN jobName ELSE @jobName END

			-- timeFromStart = 1 means searching for jobs from that time, while 0 means exactly that time
			-- timeBeforeStart = 1 likewise means searching for jobs before that time
			AND (
					(@jobStartTime IS NULL AND 1=1)
					OR (@timeFromStart = 0 AND @timeBeforeStart = 1 AND jobStartTime <= @jobStartTime) 
				    OR (@timeFromStart = 1 AND jobStartTime >= @jobStartTime) 
					OR (@timeFromStart = 0 AND jobStartTime = @jobStartTime)
				)
			-- Similar case for jobEndTime 
			AND (
					(@jobEndTime IS NULL AND 1=1)
					OR (@timeFromEnd = 1 AND @timeBeforeEnd = 0 AND jobEndTime >= @jobEndTime)
					OR (@timeBeforeEnd = 1 AND jobEndTime <= @jobEndTime) 
					OR (@timeBeforeEnd = 0 AND jobEndTime = @jobEndTime)
				)
			AND jobLocation = CASE WHEN @jobLocation IS NULL THEN jobLocation ELSE @jobLocation END

			AND jobTypeName = CASE WHEN @jobTypeName IS NULL THEN jobTypeName ELSE @jobTypeName END

			AND isCustomJobType = CASE WHEN @onlyCustomJobTypes IS NULL THEN isCustomJobType ELSE @onlyCustomJobTypes END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobsGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/01/2023
-- Description: Gets a list of jobs from a campaign, with their job type name
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobsGet]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT jobGuid, jobName, jobDescription, jobLocation, jobStartTime, jobEndTime, jobDefaultSalary, jobTypeName, peopleNeeded, peopleAssigned
	FROM jobs
	INNER JOIN job_types
	ON job_types.jobTypeId = jobs.jobTypeId
	WHERE jobs.campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobTypeAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/01/2023
-- Description: Adds a new job type for the campaign to use
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobTypeAdd]
(
    -- Add the parameters for the stored procedure here
	@jobTypeName nvarchar(50),
	@jobTypeDescription nvarchar(200) = NULL,
	@campaignGuid uniqueidentifier,
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	IF NOT (SELECT COUNT(*) FROM job_types WHERE campaignId = @campaignId) > 40
	BEGIN

		IF EXISTS(SELECT * FROM job_types WHERE campaignId = @campaignId AND jobTypeName = @jobTypeName) RETURN 50005;

		ELSE
		BEGIN

			BEGIN TRAN T


				INSERT INTO job_types(jobTypeName, campaignId, jobTypeDescription, isCustomJobType)
				VALUES (@jobTypeName, @campaignId, @jobTypeDescription, 1);

				INSERT INTO job_type_assign_capable_users(jobTypeId, userId)
				VALUES (SCOPE_IDENTITY(), @userId);

			COMMIT TRAN T;

		END
		
	END

	ELSE RETURN 50004;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobTypeAssignCapableUserAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/01/2023
-- Description: Adds a user who can assign by job type within a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobTypeAssignCapableUserAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobTypeName nvarchar(50),
	@userEmail nvarchar(50)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @userId int = (SELECT userId FROM users WHERE email = @userEmail);

	-- User not found status code
	IF @userId IS NULL RETURN 50007;

	DECLARE @jobTypeId int = (SELECT jobTypeId FROM job_types WHERE jobTypeName = @jobTypeName
								AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid));
	
	-- Job type not found status code
	IF @jobTypeId IS NULL RETURN 50013;

	-- Duplicate key status code
	IF EXISTS(SELECT * FROM job_type_assign_capable_users WHERE jobTypeId = @jobTypeId AND userId = @userId) RETURN 50001;

	INSERT INTO job_type_assign_capable_users(jobTypeId, userId)
	VALUES (@jobTypeId, @userId);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobTypeAssignCapableUsersGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/01/2023
-- Description: Gets a list of all users capable of assigning others to jobs of a specific job type
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobTypeAssignCapableUsersGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobTypeName nvarchar(50)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT users.userId, email, displayNameEng, firstNameHeb, lastNameHeb, profilePicUrl, phoneNumber
	FROM job_type_assign_capable_users
	INNER JOIN users
	ON job_type_assign_capable_users.userId = users.userId
	WHERE jobTypeId = (SELECT jobTypeId 
						FROM job_types 
						WHERE jobTypeName = @jobTypeName
							AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid));
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobTypeAssignCapableUsersRemove]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 28/01/2023
-- Description: Removes a user from being able to assign other users to a job of a specific job type
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobTypeAssignCapableUsersRemove]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobTypeName nvarchar(50),
	@userEmail nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DELETE FROM job_type_assign_capable_users
	WHERE jobTypeId = (SELECT jobTypeId 
					FROM job_types 
					WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid) 
					AND jobTypeName = @jobTypeName)
		  AND userId = (SELECT userId FROM users WHERE email = @userEmail);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobTypeDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/01/2023
-- Description: Deletes a job type from a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobTypeDelete]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobTypeName nvarchar(50)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DELETE
	FROM job_types
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)
		AND jobTypeName = @jobTypeName;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobTypesGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/01/2023
-- Description: Gets the built in and custom job types set by the campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobTypesGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT jobTypeName, jobTypeDescription, isCustomJobType
	FROM job_types 
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid) OR campaignId = -1;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobTypeUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/01/2023
-- Description: Updates a job type with the new info the user provided
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobTypeUpdate]
(
    -- Add the parameters for the stored procedure here
	@jobTypeName nvarchar(50),
	@newJobTypeName nvarchar(50) = NULL,
	@newJobTypeDescription nvarchar(50) = NULL,
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	IF EXISTS(SELECT * FROM job_types WHERE campaignId = @campaignId AND jobTypeName = @newJobTypeName) RETURN 50005;
		

    -- Insert statements for procedure here
    UPDATE job_types
	SET		jobTypeName = CASE WHEN @newJobTypeName IS NOT NULL THEN @newJobTypeName ELSE jobTypeName END,
			jobTypeDescription = CASE WHEN @newJobTypeDescription IS NOT NULL THEN @newJobTypeDescription ELSE jobTypeDescription END
	WHERE	jobTypeName = @jobTypeName 
			AND campaignId = @campaignId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_JobUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 27/01/2023
-- Description: Updates a job with the new information the user provided
-- =============================================
CREATE PROCEDURE [dbo].[usp_JobUpdate]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@jobGuid uniqueidentifier,
	@jobName nvarchar(50) = NULL,
	@jobDescription nvarchar(200) = NULL,
	@jobLocation nvarchar(100) = NULL,
	@jobStartTime datetime = NULL,
	@jobEndTime datetime = NULL,
	@jobDefaultSalary int = NULL,
	@peopleNeeded int = NULL,
	@jobTypeName nvarchar(50) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

    UPDATE	jobs
	SET		jobName = CASE WHEN @jobName IS NOT NULL THEN @jobName ELSE jobName END,
			jobDescription = CASE WHEN @jobDescription IS NOT NULL THEN @jobDescription ELSE jobDescription END,
			jobLocation = CASE WHEN @jobLocation IS NOT NULL THEN @jobLocation ELSE jobLocation END,
			jobStartTime = CASE WHEN @jobStartTime IS NOT NULL THEN @jobStartTime ELSE jobStartTime END,
			jobEndTime = CASE WHEN @jobEndTime IS NOT NULL THEN @jobEndTime ELSE jobEndTime END,
			jobDefaultSalary = CASE WHEN @jobDefaultSalary IS NOT NULL THEN @jobDefaultSalary ELSE jobDefaultSalary END,
			peopleNeeded = CASE WHEN @peopleNeeded IS NOT NULL THEN @peopleNeeded ELSE peopleNeeded END,
			jobTypeId = CASE WHEN @jobTypeName IS NOT NULL THEN (
								SELECT jobTypeId FROM job_types WHERE jobTypeName = @jobTypeName
									AND (campaignId = @campaignId OR campaignId = -1)
								)
						ELSE jobTypeId END
	WHERE   campaignId = @campaignId AND jobGuid = @jobGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PartiesForCampaignGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Gets a list of all parties for a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_PartiesForCampaignGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT partyId, partyLetter, partyName
	FROM parties
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PartyAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Adds a new party for a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_PartyAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@partyName nvarchar(100),
	@partyLetter nvarchar(5) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- CampaignNotFound status code
	IF @campaignId IS NULL RETURN 50017;

	INSERT INTO parties(campaignId, partyLetter, partyName)
	VALUES (@campaignId, @partyLetter, @partyName);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PartyDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Deletes a party for a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_PartyDelete]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@partyId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DELETE FROM parties
	WHERE partyId = @partyId AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PartyUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Updates an existing party for a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_PartyUpdate]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@partyId int,
	@partyName nvarchar(100) = NULL,
	@partyLetter nvarchar(5) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- CampaignNotFound status code
	IF @campaignId IS NULL RETURN 50017;

	UPDATE parties
	SET partyLetter = ISNULL(@partyLetter, partyLetter),
		partyName = ISNULL(@partyName, partyName)
	WHERE partyId = @partyId AND campaignId = @campaignId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PermissionAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 18/01/2023
-- Description: Adds a permission to the user's permission set for the campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_PermissionAdd]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@permissionType nvarchar(10),
	@permissionTarget nvarchar(30),
	@campaignGuid uniqueidentifier

)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @permissionId int = (SELECT permissionId 
								FROM defined_permissions 
								WHERE permissionTarget = @permissionTarget AND permissionType = @permissionType);

	IF @permissionId IS NULL RETURN 50009;

	DECLARE @campaignId int = (SELECT campaignId 
								FROM campaigns 
								WHERE campaignGuid = @campaignGuid);

	IF NOT EXISTS(SELECT * 
					FROM permission_sets 
					WHERE userId = @userId AND permissionId = @permissionId AND campaignId = @campaignId)

		BEGIN

			INSERT INTO permission_sets VALUES(@userId, @campaignId, @permissionId);

			-- If the user was given edit permissions, they must also be given view permissions.
			-- Therefore, check if the user already has the view permission, and if not, add it to them.
			IF @permissionType = 'edit'
			BEGIN
				SET @permissionId = (SELECT permissionId
									FROM defined_permissions
									WHERE permissionTarget = @permissionTarget AND permissionType = 'view');
				IF NOT EXISTS(SELECT *
								FROM permission_sets
								WHERE userId = @userId AND permissionId = @permissionId AND campaignId = @campaignId)
					
					INSERT INTO permission_sets VALUES(@userId, @campaignId, @permissionId);

			END
		END

	ELSE RETURN 50010

END
GO
/****** Object:  StoredProcedure [dbo].[usp_PermissionRemove]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 18/01/2023
-- Description: Removes a permission from the user's permission set for the campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_PermissionRemove]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@permissionType nvarchar(10),
	@permissionTarget nvarchar(30),
	@campaignGuid uniqueidentifier

)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	DELETE FROM permission_sets 
	WHERE userId = @userId

	AND permissionId = (
					SELECT permissionId 
					FROM defined_permissions
					WHERE permissionType = @permissionType AND permissionTarget = @permissionTarget
					)

	AND campaignId = @campaignId;

	-- If user had their view permission revoked, also revoke their edit permission for the same screen in case they have it.
	IF @permissionType = 'view'
	BEGIN

		DELETE 
		FROM permission_sets 
		WHERE campaignId = @campaignId 
		AND userId = @userId 
		AND permissionId = (
					SELECT permissionId 
					FROM defined_permissions
					WHERE permissionType = 'edit' AND permissionTarget = @permissionTarget
					)

	END

END
GO
/****** Object:  StoredProcedure [dbo].[usp_PermissionSetGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 18/01/2023
-- Description: Gets the user's permission set
-- =============================================
CREATE PROCEDURE [dbo].[usp_PermissionSetGet]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@campaignGuid uniqueidentifier

)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	SELECT defined_permissions.permissionId, permissionType, permissionTarget 
	FROM permission_sets JOIN defined_permissions 
	ON permission_sets.permissionId = defined_permissions.permissionId 
	WHERE userId = @userId AND
	campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardAnnouncementAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 04/03/2023
-- Description: Adds a new announcement to a campaign by adding it to the public_board_announcements table
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardAnnouncementAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@publisherId int,
	@announcementTitle nvarchar(100),
	@announcementContent nvarchar(4000),
	@newAnnouncementGuid uniqueidentifier OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	-- Setting value of this to prevent fun times of exceptions due to this being null when the data type must not be null
	SET @newAnnouncementGuid = '00000000-0000-0000-0000-000000000000'

    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	-- Status code UserNotFound
	IF NOT EXISTS (SELECT userId FROM users WHERE userId = @publisherId) RETURN 50007;

	DECLARE @announcementGuid uniqueidentifier = NEWID();

	-- Statistically, this should basically never happen
	WHILE EXISTS(SELECT announcementId FROM public_board_announcements WHERE announcementGuid = @announcementGuid)
		SET @announcementGuid = NEWID()

	INSERT INTO public_board_announcements(announcementContent, publisherId, publishingDate, campaignId, announcementTitle, announcementGuid)
	VALUES (@announcementContent, @publisherId, GETDATE(), @campaignId, @announcementTitle, @announcementGuid);

	SET @newAnnouncementGuid = @announcementGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardAnnouncementDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 04/03/2023
-- Description: Unpublishes an announcement by removing it from the public_board_announcements table
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardAnnouncementDelete]
(
    -- Add the parameters for the stored procedure here
	@announcementGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	-- Status code AnnouncementNotFound
    IF NOT EXISTS (SELECT announcementId FROM public_board_announcements WHERE announcementGuid = @announcementGuid) RETURN 50022;

	DELETE FROM public_board_announcements
	WHERE announcementGuid = @announcementGuid;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardAnnouncementsGetForCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 04/03/2023
-- Description: Gets all published announcements of a specific campaign, along with the publisher info of each announcement
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardAnnouncementsGetForCampaign]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	-- Status code CampaignNotFound
	IF NOT EXISTS(SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid) RETURN 50017;

    SELECT publishingDate, firstNameHeb, lastNameHeb, displayNameEng, profilePicUrl, phoneNumber, email,
			campaignName, campaignGuid, campaignLogoUrl, announcementContent, announcementTitle, announcementGuid
	FROM public_board_announcements
	INNER JOIN campaigns
	ON campaigns.campaignId = public_board_announcements.campaignId
	LEFT JOIN users
	ON userId = publisherId
	WHERE campaignGuid = @campaignGuid
	ORDER BY publishingDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardAnnouncementsGetForUserByPreferences]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 06/03/2023
-- Description: Gets published announcements for a user, according to their preferences
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardAnnouncementsGetForUserByPreferences]
(
    -- This parameter should be provided as a list of JSON values, with each containing keys campaignGuid and isPreferred.
	@userId int = NULL,
	@limit int = 50,
	@offset int = 0
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    SELECT announcementTitle, announcementGuid, announcementContent,
			campaignName, campaignGuid, campaignLogoUrl, firstNameHeb, lastNameHeb, displayNameEng, publishingDate
	FROM public_board_announcements
	INNER JOIN campaigns
	ON campaigns.campaignId = public_board_announcements.campaignId
	LEFT JOIN users
	ON publisherId = userId
	-- Filter out events that are in avoided campaigns
	WHERE public_board_announcements.campaignId NOT IN (
		SELECT campaignId
		FROM users_public_board_preferences
		WHERE userId = @userId AND isPreferred = 0
	)
	-- Give priority ordering to preferred campaigns
	ORDER BY CASE WHEN 
		public_board_announcements.campaignId IN (
			SELECT campaignId
			FROM users_public_board_preferences
			WHERE userId = @userId AND isPreferred = 1
		) THEN 1 
	ELSE 2 END ASC,
	-- Finally, sort by time published
	public_board_announcements.publishingDate DESC
	OFFSET @offset ROWS FETCH FIRST @limit ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardAnnouncementsSearch]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 06/03/2023
-- Description: Searches for announcements according to the given parameters
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardAnnouncementsSearch]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier = NULL,
	@campaignName nvarchar(200) = NULL,
	@campaignCity nvarchar(100) = NULL,
	@publishingDate datetime = NULL,
	@announcementTitle nvarchar(100) = NULL,
	@publisherFirstName nvarchar(200) = NULL,
	@publisherLastName nvarchar(200) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    	DECLARE @campaignId int = NULL;

	IF @campaignGuid IS NOT NULL SET @campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	DECLARE @cityId int = NULL;

	IF @campaignCity IS NOT NULL SET @cityId = (SELECT cityId FROM cities WHERE cityName = @campaignCity);

    SELECT announcementTitle, announcementGuid, announcementContent,
			campaignName, campaignGuid, campaignLogoUrl, firstNameHeb, lastNameHeb, displayNameEng, publishingDate
	FROM public_board_announcements
	INNER JOIN campaigns
	ON campaigns.campaignId = public_board_announcements.campaignId
	LEFT JOIN users
	ON publisherId = userId
	WHERE	public_board_announcements.campaignId = CASE WHEN @campaignGuid IS NULL THEN campaigns.campaignId ELSE @campaignId END
			AND campaignName = ISNULL(@campaignName, campaignName) 
			AND cityId = CASE WHEN @campaignCity IS NULL THEN cityId ELSE @cityId END 
			AND CONVERT(date, publishingDate) = CASE WHEN @publishingDate IS NULL THEN CONVERT(date, publishingDate) ELSE CONVERT(date, @publishingDate) END
			AND announcementTitle = ISNULL(@announcementTitle, announcementTitle) 
			AND (firstNameHeb = ISNULL(@publisherFirstName, firstNameHeb) OR firstNameEng = ISNULL(@publisherFirstName, firstNameEng))
			AND (lastNameHeb = ISNULL(@publisherLastName, lastNameHeb) OR lastNameEng = ISNULL(@publisherLastName, lastNameEng))
	ORDER BY publishingDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardEventAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 03/03/2023
-- Description: Adds a new event to be published to the public board events table
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardEventAdd]
(
    -- Add the parameters for the stored procedure here
	@eventGuid uniqueidentifier,
	@publisherId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @eventId int, @campaignId int;
	
	SELECT @eventId = eventId, @campaignId = campaignId FROM custom_events WHERE eventGuid = @eventGuid;

	-- Status code EventNotFound
	IF @eventId IS NULL RETURN 50018;
	
	-- Status code IncorrectEventType
	IF @campaignId IS NULL RETURN 50021;

	-- Status code UserNotFound
	IF NOT EXISTS (SELECT userId FROM users WHERE userId = @publisherId) RETURN 50007;

	-- Status code DuplicateKey
	IF EXISTS (SELECT * FROM public_board_events WHERE eventId = @eventId) RETURN 50001;

	INSERT INTO public_board_events (eventId, publishingDate, publisherId)
	VALUES (@eventId, GETDATE(), @publisherId);

END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardEventDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 03/03/2023
-- Description: Removes an event from the public board events table to unpublish it
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardEventDelete]
(
    -- Add the parameters for the stored procedure here
	@eventGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @eventId int = (SELECT eventId FROM custom_events WHERE eventGuid = @eventGuid);

	-- Status code EventNotFound
	IF NOT EXISTS (SELECT * FROM public_board_events WHERE eventId = @eventId) RETURN 50018;

	DELETE FROM public_board_events
	WHERE eventId = @eventId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardEventsGetForCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 03/03/2023
-- Description: Gets all the published events of a specific campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardEventsGetForCampaign]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	SELECT eventGuid, eventName, eventDescription, eventLocation, eventStartTime, eventEndTime, maxAttendees, numAttending,
			publishingDate, firstNameHeb, lastNameHeb, displayNameEng, profilePicUrl, phoneNumber, email,
			campaignName, campaignGuid, campaignLogoUrl
	FROM public_board_events
	INNER JOIN custom_events
	ON custom_events.eventId = public_board_events.eventId
	LEFT JOIN users
	ON userId = publisherId
	LEFT JOIN campaigns
	ON campaigns.campaignId = custom_events.campaignId
	WHERE custom_events.campaignId = @campaignId
	ORDER BY publishingDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardEventsGetForUserByPreferences]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      Yuval Uner
-- Create Date: 06/03/2023
-- Description: Gets published announcements and events for the user, according to their preferences
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardEventsGetForUserByPreferences]
(
    -- This parameter should be provided as a list of JSON values, with each containing keys campaignGuid and isPreferred.
	@userId int = NULL,
	@limit int = 50,
	@offset int = 0
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    SELECT  eventGuid, eventName, eventDescription, eventLocation, eventStartTime, eventEndTime, maxAttendees, numAttending
			, campaignName, campaignGuid, campaignLogoUrl, firstNameHeb, lastNameHeb, displayNameEng, publishingDate
	FROM public_board_events
	INNER JOIN custom_events
	ON custom_events.eventId = public_board_events.eventId
	LEFT JOIN campaigns
	ON campaigns.campaignId = custom_events.campaignId
	LEFT JOIN users
	ON publisherId = userId
	-- Filter out events that are in avoided campaigns
	WHERE custom_events.campaignId NOT IN (
		SELECT campaignId
		FROM users_public_board_preferences
		WHERE userId = @userId AND isPreferred = 0
	)
	-- Give priority ordering to preferred campaigns
	ORDER BY CASE WHEN 
		custom_events.campaignId IN (
			SELECT campaignId
			FROM users_public_board_preferences
			WHERE userId = @userId AND isPreferred = 1
		) THEN 1 
	ELSE 2 END ASC,
	-- Finally, sort by time published
	publishingDate DESC
	OFFSET @offset ROWS FETCH FIRST @limit ROWS ONLY;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_PublicBoardEventsSearch]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 06/03/2023
-- Description: Searches published events according to given parameters
-- =============================================
CREATE PROCEDURE [dbo].[usp_PublicBoardEventsSearch]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier = NULL,
	@campaignName nvarchar(200) = NULL,
	@campaignCity nvarchar(100) = NULL,
	@publishingDate datetime = NULL,
	@eventName nvarchar(200) = NULL,
	@publisherFirstName nvarchar(200) = NULL,
	@publisherLastName nvarchar(200) = NULL,
	@eventLocation nvarchar(100) = NULL,
	@eventStartTime datetime = NULL,
	@eventEndTime datetime = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON


    -- Insert statements for procedure here
	DECLARE @campaignId int = NULL;

	IF @campaignGuid IS NOT NULL SET @campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	DECLARE @cityId int = NULL;

	IF @campaignCity IS NOT NULL SET @cityId = (SELECT cityId FROM cities WHERE cityName = @campaignCity);

    SELECT eventGuid, eventName, eventDescription, eventLocation, eventStartTime, eventEndTime, maxAttendees, numAttending
			,campaignName, campaignGuid, campaignLogoUrl, firstNameHeb, lastNameHeb, displayNameEng, publishingDate
	FROM public_board_events
	INNER JOIN custom_events
	ON custom_events.eventId = public_board_events.eventId
	LEFT JOIN campaigns
	ON campaigns.campaignId = custom_events.campaignId
	LEFT JOIN users
	ON publisherId = userId
	WHERE	custom_events.campaignId = CASE WHEN @campaignGuid IS NULL THEN campaigns.campaignId ELSE @campaignId END
			AND campaignName = ISNULL(@campaignName, campaignName) 
			AND cityId = CASE WHEN @campaignCity IS NULL THEN cityId ELSE @cityId END 
			AND CONVERT(date, publishingDate) = CASE WHEN @publishingDate IS NULL THEN CONVERT(date, publishingDate) ELSE CONVERT(date, @publishingDate) END
			AND eventName = ISNULL(@eventName, eventName) 
			AND (firstNameHeb = ISNULL(@publisherFirstName, firstNameHeb) OR firstNameEng = ISNULL(@publisherFirstName, firstNameEng))
			AND (lastNameHeb = ISNULL(@publisherLastName, lastNameHeb) OR lastNameEng = ISNULL(@publisherLastName, lastNameEng)) 
			-- These values can be null in their tables, so we provide them with some default values using the external ISNULL
			AND ISNULL(eventLocation, '999') = ISNULL(@eventLocation, ISNULL(eventLocation, '999'))
			AND ISNULL(CONVERT(date, eventStartTime), CONVERT(date, '01/01/1800', 103)) >= 
					CASE WHEN @eventStartTime IS NULL THEN ISNULL(CONVERT(date, eventStartTime), CONVERT(date, '01/01/1800', 103))
					ELSE CONVERT(date, @eventStartTime) END
			AND ISNULL(CONVERT(date, eventEndTime), CONVERT(date, '01/01/3000', 103)) <= 
					CASE WHEN @eventEndTime IS NULL THEN ISNULL(CONVERT(date, eventEndTime), CONVERT(date, '01/01/3000', 103))
					ELSE CONVERT(date, @eventEndTime) END 
	ORDER BY publishingDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_RoleAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Adds a custom role to the campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_RoleAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@roleName nvarchar(50),
	@roleDescription nvarchar(150)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
    IF (SELECT COUNT(*) FROM roles WHERE campaignId = @campaignId) >= 50 RETURN 50004;

	IF NOT EXISTS(SELECT roleId FROM roles WHERE roleName = @roleName AND campaignId = @campaignId)
		
		INSERT 
		INTO roles(roleName, campaignId, roleDescription, roleLevel, isCustomRole)
		VALUES (@roleName, @campaignId, @roleDescription, 0, 1);

	ELSE
		RETURN 50008;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_RoleAdministrativeAssignUser]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Gives a user an adminstrative role and adds all permissions to them.
-- =============================================
CREATE PROCEDURE [dbo].[usp_RoleAdministrativeAssignUser]
(
    -- Add the parameters for the stored procedure here
    @roleName nvarchar(50),
    @campaignGuid uniqueidentifier,
	@userEmail nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON
	

		-- Insert statements for procedure here
	-- Illegal role name - not one of the built in adminstartive ones.
	IF @roleName NOT IN (SELECT roleName FROM roles WHERE roleLevel > 0) RETURN 50006;
	

	DECLARE @userId int = (SELECT userId FROM users WHERE email = @userEmail);
	IF @userId IS NULL RETURN 50007;


	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Check if user is already in an adminstrative role. If they are, return duplicate key error (before that actually happens).
	IF (SELECT roleLevel
		FROM roles 
		WHERE roleId = (SELECT roleId FROM campaign_users WHERE userId = @userId AND campaignId = @campaignId))
	> 0 RETURN 50001;

	BEGIN TRAN T

		-- Remove the user's permissions in the campaign first, to prevent primary key collisions.
		DELETE FROM permission_sets
		WHERE userId = @userId
				AND campaignId = @campaignId;

		UPDATE campaign_users
		SET roleId = (SELECT roleId FROM roles WHERE roleName = @roleName AND campaignId = -1)
		WHERE userId = @userId AND campaignId = @campaignId;

		-- Re-add all of the user's permissions.
		EXEC usp_AddAllPermissions @userId = @userId, @campaignId = @campaignId;

	COMMIT TRAN T;
	RETURN 0;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_RoleAdministrativeRemove]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Removes a user from an administrative role and returns them to the volunteer role.
-- =============================================
CREATE PROCEDURE [dbo].[usp_RoleAdministrativeRemove]
(
    -- Add the parameters for the stored procedure here
	@userEmail nvarchar(50),
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @userId int = (SELECT userId FROM users WHERE email = @userEmail);

	IF @userId IS NULL RETURN -1;

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

    BEGIN TRAN T

		DELETE FROM permission_sets
		WHERE userId = @userId AND campaignId = @campaignId;

		UPDATE campaign_users
		SET roleId = (SELECT roleId FROM roles WHERE roleName = 'Volunteer' AND campaignId = -1)
		WHERE campaignId = @campaignId AND userId = @userId;

	COMMIT TRAN T;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_RoleAssignUser]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Assigns a user to a role
-- =============================================
CREATE PROCEDURE [dbo].[usp_RoleAssignUser]
(
    -- Add the parameters for the stored procedure here
    @roleName nvarchar(50),
    @campaignGuid uniqueidentifier,
	@userEmail nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
    DECLARE @roleId int = (SELECT roleId FROM roles WHERE roleName = @roleName AND campaignId = @campaignId);

	-- If not found the first time, checkf if it was a built in role instead.
	IF @roleId IS NULL SET @roleId = (SELECT roleId FROM roles WHERE roleName = @roleName AND campaignId = -1);

	IF @roleId IS NULL RETURN 50006;

	DECLARE @userId int = (SELECT userId FROM users WHERE email = @userEmail);
	IF @userId IS NULL RETURN 50007;

	UPDATE campaign_users
	SET roleId = @roleId
	WHERE campaignId = @campaignId AND userId = @userId;

	RETURN 0;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_RoleDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Deletes a role from a campaign.
-- =============================================
CREATE PROCEDURE [dbo].[usp_RoleDelete]
(
    -- Add the parameters for the stored procedure here
    @roleName nvarchar(50),
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DELETE FROM roles
	WHERE roleName = @roleName AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_RoleGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Gets a role by its name and campaign id
-- =============================================
CREATE PROCEDURE [dbo].[usp_RoleGet]
(
    -- Add the parameters for the stored procedure here
	@roleName nvarchar(50),
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT * 
	FROM roles 
	WHERE roleName = @roleName 
		AND (campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid) OR campaignId = -1);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_RoleRemove]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Removes a user from the role they were assigned to and resets them back to volunteer.
-- =============================================
CREATE PROCEDURE [dbo].[usp_RoleRemove]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier,
	@userEmail nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	UPDATE campaign_users
	SET roleId = (SELECT roleId FROM roles WHERE roleName = 'Volunteer' AND campaignId = -1)
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)
		AND userId = (SELECT userId FROM users WHERE email = @userEmail);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_RoleUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 26/01/2023
-- Description: Modifies the description of a role
-- =============================================
CREATE PROCEDURE [dbo].[usp_RoleUpdate]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@roleName nvarchar(50),
	@roleDescription nvarchar(150)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    UPDATE roles
	SET roleDescription = @roleDescription
	WHERE roleName = @roleName AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SmsInDepthLogDetailsGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 12/02/2023
-- Description: Gets in-depth logs about a specific SMS message.
-- =============================================
CREATE PROCEDURE [dbo].[usp_SmsInDepthLogDetailsGet]
(
    -- Add the parameters for the stored procedure here
	@messageGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT messageGuid, messageContents, messageDate, sentCount, isSuccess, phoneNumber, firstName, lastName, residenceName, streetName, houseNumber
	FROM sms_messages
	INNER JOIN sms_messages_phone_numbers
	ON sms_messages.messageId = sms_messages_phone_numbers.messageId
	LEFT JOIN voters_ledger_dynamic
	ON phoneNumber = phone1 OR phoneNumber = phone2
	LEFT JOIN voters_ledger
	ON voters_ledger.idNum = voters_ledger_dynamic.IdNum
	WHERE messageGuid = @messageGuid
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SmsMessageAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 12/02/2023
-- Description: Adds a new SMS message to the relevant table
-- =============================================
CREATE PROCEDURE [dbo].[usp_SmsMessageAdd]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@messageContents nvarchar(500),
	@senderId int,
	@newMessageGuid uniqueidentifier OUTPUT,
	@newMessageId int OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @messageGuid uniqueidentifier = NEWID();
	WHILE EXISTS(SELECT * FROM sms_messages WHERE messageGuid = @messageGuid)
		SET @messageGuid = NEWID();

    INSERT INTO sms_messages(messageGuid, messageContents, campaignId, senderId, messageDate)
	VALUES(
			@messageGuid,
			@messageContents,
			(SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid),
			@senderId,
			GETDATE()
		  );
	SET @newMessageGuid = @messageGuid;
	SET @newMessageId = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SmsMessageGeneralLogsGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 12/02/2023
-- Description: Gets the SMS message logs of a campaign, sorted by date sent.
-- =============================================
CREATE PROCEDURE [dbo].[usp_SmsMessageGeneralLogsGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT messageGuid, messageContents, messageDate, sentCount, firstNameHeb, lastNameHeb, displayNameEng, profilePicUrl
	FROM sms_messages
	INNER JOIN users
	ON userId = senderId
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)
	ORDER BY messageDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SmsMessageSentToPhoneNumberAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 12/02/2023
-- Description: Adds a new link between a phone number and an sms message sent to that phone number, to create the logs
-- =============================================
CREATE PROCEDURE [dbo].[usp_SmsMessageSentToPhoneNumberAdd]
(
    -- Add the parameters for the stored procedure here
	@messageId int,
	@phoneNumber nvarchar(20),
	@isSuccess bit
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here

	-- Transaction to ensure data integrity in case of surprises. Specifically, DB went up in flames mid query level of surprises.
	BEGIN TRAN T

		INSERT INTO sms_messages_phone_numbers (messageId, phoneNumber, isSuccess)
		VALUES (@messageId, @phoneNumber, @isSuccess);

		UPDATE sms_messages
		SET sentCount = sentCount + 1
		WHERE messageId = @messageId

	COMMIT TRAN T;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SupportStatusUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 19/01/2023
-- Description: Updates a voter's support status for a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_SupportStatusUpdate]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@voterId int,
	@supportStatus bit = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
	
	DECLARE @campignCity int = (SELECT cityId FROM campaigns WHERE campaignId = @campaignId);

	IF @campignCity IS NULL RETURN 50011;
	-- CamapignCity is not -1 means the campaign is a municipal one in some city.
	-- As such, we need to test to make sure the voter updated is also from that same city.
	IF @campignCity <> -1
	BEGIN
		
		-- Check if both the city id and residence Id of the user are mismatched. If at-least one matches, allow the update.
		-- This also covers cases where the voter does not exist, as cityId and residenceId would both be null.
		IF (SELECT cityId FROM voters_ledger WHERE idNum = @voterId) <> @campignCity
			AND (SELECT residenceId FROM voters_ledger WHERE idNum = @voterId) <> @campignCity
			BEGIN
				RETURN 50011
			END

	END

    IF @supportStatus IS NULL
	BEGIN
		DELETE FROM voter_campaign_support_statuses WHERE IdNum = @voterId AND campaignId = @campaignId;
	END

	ELSE
	BEGIN

		IF EXISTS(SELECT * FROM voter_campaign_support_statuses WHERE IdNum = @voterId AND campaignId = @campaignId)
		BEGIN
			UPDATE voter_campaign_support_statuses SET supportStatus = @supportStatus WHERE IdNum = @voterId AND campaignId = @campaignId;
		END

		ELSE
			INSERT INTO voter_campaign_support_statuses VALUES(@voterId, @campaignId, @supportStatus);

	END
	RETURN 0;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 08/01/2023
-- Description: Adds a new user to the users table
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserAdd]
(
    -- Add the parameters for the stored procedure here
    @email nvarchar(200),
    @firstNameEng nvarchar(50),
	@lastNameEng nvarchar(50),
	@displayNameEng nvarchar(100),
	@profilePicUrl nvarchar(200)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    INSERT INTO users(email, firstNameEng, lastNameEng, displayNameEng, profilePicUrl) VALUES(@email, @firstNameEng, @lastNameEng, @displayNameEng, @profilePicUrl);
	RETURN SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserAuthenticationStatusGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 12/01/2023
-- Description: Gets the user's authentication status
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserAuthenticationStatusGet]
(
    -- Add the parameters for the stored procedure here
    @userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
   SELECT userId, authenticated FROM users WHERE userId = @userId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserCampaignsGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 11/01/2023
-- Description: Gets the name and link to all campaigns related to the user, as well as their roles in each campaign.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserCampaignsGet]
(
    -- Add the parameters for the stored procedure here
    @userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	SELECT userId, campaign_users.campaignId, campaign_users.roleId, campaignGuid, campaignName, campaignLogoUrl, roleName, campaignDescription
	FROM campaign_users
	INNER JOIN campaigns
	ON campaign_users.campaignId = campaigns.campaignId
	LEFT JOIN roles
	ON campaign_users.roleId = roles.roleId
	WHERE userId = @userId;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserContactInfoGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 23/01/2023
-- Description: Gets a user's contact info from their users table entry
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserContactInfoGet]
(
    -- Add the parameters for the stored procedure here
	@userId int = 0,
	@userEmail nvarchar(200) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	IF @userId <> 0
		SELECT email, phoneNumber from users WHERE userId = @userId;

	IF @userEmail IS NOT NULL
		SELECT email, phoneNumber FROM users WHERE email = @userEmail;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 18/01/2023
-- Description: Deletes a user from the database
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserDelete]
(
    -- Add the parameters for the stored procedure here
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DELETE FROM users WHERE userId = @userId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserInfoByEmailGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 08/01/2023
-- Description: Gets all the of the user's basic info by their email.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserInfoByEmailGet]
(
    -- Add the parameters for the stored procedure here
    @email nvarchar(200) = 0
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	SELECT * FROM users WHERE email = @email;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserJobsGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 30/01/2023
-- Description: Gets all the jobs of a user. If @campaignGuid is supplied, limits scope to only that campaign. Also gets info about who assigned the job
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserJobsGet]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier = NULL,
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	IF @campaignGuid IS NULL

	BEGIN

		SELECT salary, jobTypeName, jobDescription, jobName, jobLocation, jobStartTime, jobEndTime, email, 
			firstNameHeb, lastNameHeb, displayNameEng, profilePicUrl, phoneNumber,
			campaignName, campaignGuid, jobGuid
		FROM job_assignments
		INNER JOIN jobs
		ON job_assignments.jobId = jobs.jobId
		INNER JOIN job_types
		ON job_types.jobTypeId = jobs.jobTypeId
		INNER JOIN users
		ON assignedBy = users.userId
		LEFT JOIN campaigns
		ON campaigns.campaignId = jobs.campaignId
		WHERE job_assignments.userId = @userId

	END

	ELSE
	BEGIN
    -- Insert statements for procedure here
		SELECT salary, jobTypeName, jobDescription, jobName, jobLocation, jobStartTime, jobEndTime, email, 
				firstNameHeb, lastNameHeb, displayNameEng, profilePicUrl, phoneNumber, jobGuid
		FROM job_assignments
		INNER JOIN jobs
		ON job_assignments.jobId = jobs.jobId
		INNER JOIN job_types
		ON job_types.jobTypeId = jobs.jobTypeId
		INNER JOIN users
		ON assignedBy = users.userId
		WHERE job_assignments.userId = @userId
				AND jobs.campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid)

	END


END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserLinkToCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 11/01/2023
-- Description: Add a user as part of a campaign by adding a row with both respective ids in the connecting table
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserLinkToCampaign]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier,
    @userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	-- Id for volunteer role
	DECLARE @roleId INT = 1;

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- If the key already exists, throw the custom error code for it.
	IF EXISTS(SELECT * FROM campaign_users WHERE campaignId = @campaignId AND userId = @userId) RETURN 50001;

    INSERT INTO campaign_users(campaignId, userId, roleId) VALUES((SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid),
	@userId, @roleId);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPhoneNumberAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 23/01/2023
-- Description: Adds or updates a user's phone number. Also updates it in the voters ledger.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPhoneNumberAdd]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@phoneNumber nvarchar(20)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    UPDATE users SET phoneNumber = @phoneNumber WHERE userId = @userId;

	DECLARE @userIdNum int = (SELECT idNum FROM users WHERE userId = @userId);

	IF EXISTS(SELECT phone1 FROM voters_ledger_dynamic WHERE IdNum = @userIdNum)
	BEGIN

		UPDATE voters_ledger_dynamic SET phone2 = @phoneNumber WHERE IdNum = @userIdNum;

	END

	ELSE
	BEGIN

		UPDATE voters_ledger_dynamic SET phone1 = @phoneNumber WHERE IdNum = @userIdNum;

	END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPhoneNumberRemove]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 01/02/2023
-- Description: Removes a user's phone number from their account
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPhoneNumberRemove]
(
    -- Add the parameters for the stored procedure here
    @userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    UPDATE users
	SET phoneNumber = NULL
	WHERE userId = @userId;

END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPreferenceAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 05/03/2023
-- Description: Adds a new user preference to the users_public_board_preferences table
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPreferenceAdd]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@campaignGuid uniqueidentifier,
	@isPreferred bit
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	-- Status code UserNotFound
	IF NOT EXISTS (SELECT userId FROM users WHERE userId = @userId) RETURN 50007;

	-- Status code DuplicateKey
	IF EXISTS(SELECT * FROM users_public_board_preferences WHERE campaignId = @campaignId AND userId = @userId) RETURN 50001;

	INSERT INTO users_public_board_preferences(userId, campaignId, isPreferred)
	VALUES (@userId, @campaignId, @isPreferred);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPreferenceDelete]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 05/03/2023
-- Description: Deletes a user's preference from the users_public_board_preferences table
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPreferenceDelete]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	-- Status code UserNotFound
	IF NOT EXISTS (SELECT userId FROM users WHERE userId = @userId) RETURN 50007;

	-- Status code PreferenceNotFound
	IF NOT EXISTS(SELECT * FROM users_public_board_preferences WHERE campaignId = @campaignId AND userId = @userId) RETURN 50023;

	DELETE FROM users_public_board_preferences
	WHERE userId = @userId AND campaignId = @campaignId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPreferenceGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 05/03/2023
-- Description: Gets the list of user preferences from users_public_board_preferences along with basic info about each campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPreferenceGet]
(
    -- Add the parameters for the stored procedure here
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	-- Status code UserNotFound
	IF NOT EXISTS (SELECT userId FROM users WHERE userId = @userId) RETURN 50007;

	SELECT isPreferred, campaignName, campaignGuid, campaignLogoUrl
	FROM users_public_board_preferences
	INNER JOIN campaigns
	ON campaigns.campaignId = users_public_board_preferences.campaignId
	WHERE userId = @userId

END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPreferencesGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 31/01/2023
-- Description: Gets a user's preferences for their job assignments.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPreferencesGet]
(
    -- Add the parameters for the stored procedure here
    @userId int,
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT userPreferencesText
	FROM user_work_preferences
	WHERE userId = @userId
			AND campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPreferencesModify]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 31/01/2023
-- Description: Modifies the user's preferences in a campaign in the table for it.
-- If the @userPreferencesText value is null, the entry is removed. If it exists already, it is updated, and if not, it is added.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPreferencesModify]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier,
    @userId int,
	@userPreferencesText nvarchar(1000) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	IF @userPreferencesText IS NULL
	BEGIN

		DELETE FROM user_work_preferences
		WHERE userId = @userId AND campaignId = @campaignId;

		RETURN;

	END

	IF EXISTS(SELECT * FROM user_work_preferences WHERE userId = @userId AND campaignId = @campaignId)
	BEGIN

		UPDATE user_work_preferences
		SET userPreferencesText = @userPreferencesText
		WHERE campaignId = @campaignId AND userId = @userId;

	END

	ELSE
	BEGIN

		INSERT INTO user_work_preferences(userId, campaignId, userPreferencesText)
		VALUES (@userId, @campaignId, @userPreferencesText)

	END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPreferenceUpdate]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 05/03/2023
-- Description: Updates a user's preference in the users_public_board_preferences table
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPreferenceUpdate]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@campaignGuid uniqueidentifier,
	@isPreferred bit
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	-- Status code UserNotFound
	IF NOT EXISTS (SELECT userId FROM users WHERE userId = @userId) RETURN 50007;

	IF NOT EXISTS(SELECT * FROM users_public_board_preferences WHERE campaignId = @campaignId AND userId = @userId) RETURN 50023;

	UPDATE  users_public_board_preferences
	SET		isPreferred = @isPreferred
	WHERE	userId = @userId AND campaignId = @campaignId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPrivateInfoAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 12/01/2023
-- Description: Adds the user's private info to the users table and dynamic ledger table
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPrivateInfoAdd]
(
    -- Add the parameters for the stored procedure here
    @userId int,
    @idNum int,
	@firstNameHeb nvarchar(50),
	@lastNameHeb nvarchar(50)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON

	IF NOT EXISTS(SELECT userId FROM users WHERE idNum = @idNum)
	BEGIN

		BEGIN TRANSACTION
		-- Insert statements for procedure here
		UPDATE users SET firstNameHeb = @firstNameHeb, idNum = @idNum, lastNameHeb = @lastNameHeb, authenticated = 1 WHERE userId = @userId;

		-- Get the user's email address that they signed up with
		DECLARE @emailAddress nvarchar(200) = (SELECT email FROM users WHERE userId = @userId);
	
		-- Put the email address as the user's email
		IF EXISTS (SELECT * FROM voters_ledger_dynamic WHERE IdNum = @idNum)
			-- Always update email1 when done this way, as it is an email the user themselves provided.
			-- email2 field is meant for storing an additional secondary address.
			UPDATE voters_ledger_dynamic SET email1 = @emailAddress WHERE IdNum = @idNum;

		ELSE
			INSERT INTO voters_ledger_dynamic(IdNum, email1) VALUES(@idNum, @emailAddress);

		COMMIT;

		RETURN 0;

	END

	-- User with this id number already verified - this request should be thrown and server notified.
	ELSE
		RETURN 1;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserProfilePageInfoGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 10/04/2023
-- Description: Gets the info needed to be displayed on a user's profile page - name, city and contact info
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserProfilePageInfoGet]
(
    -- Add the parameters for the stored procedure here
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT firstNameEng, firstNameHeb, lastNameEng, lastNameHeb, profilePicUrl, phoneNumber, email, cityName, displayNameEng
	FROM users
	LEFT JOIN voters_ledger
	ON users.idNum = voters_ledger.idNum
	LEFT JOIN cities 
	ON cities.cityId = voters_ledger.cityId
	WHERE userId = @userId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPublicInfoByIdGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 11/01/2023
-- Description: Gets all the "public" info about a user by their user id
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPublicInfoByIdGet]
(
    -- Add the parameters for the stored procedure here
    @userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT userId, firstNameEng, lastNameEng, displayNameEng, firstNameHeb, lastNameHeb, profilePicUrl FROM users WHERE userId = @userId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPublishNotificationSettingsGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 07/03/2023
-- Description: Gets a user's publish notification settings for all campaigns they are subscribed for
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPublishNotificationSettingsGet]
(
    -- Add the parameters for the stored procedure here
	@userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT viaEmail, viaSms, campaignName, campaignGuid, campaignDescription, campaignLogoUrl
	FROM users_notified_on_publish
	INNER JOIN campaigns
	ON campaigns.campaignId = users_notified_on_publish.campaignId
	WHERE userId = @userId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPublishNotificationSettingsModify]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 07/03/2023
-- Description: Modifies a user's notification settings for getting notifications when a certain campaign publishes a new event or announcement.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserPublishNotificationSettingsModify]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@campaignGuid uniqueidentifier,
	@viaSms bit,
	@viaEmail bit
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here

	-- Status code UserNotFound
    IF NOT EXISTS (SELECT userId FROM users WHERE userId = @userId) RETURN 50007;

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- Status code CampaignNotFound
	IF @campaignId IS NULL RETURN 50017;

	-- If both are 0, meaning no notifications, delete the row
	IF @viaSms = 0 AND @viaEmail = 0
	BEGIN
		
		DELETE FROM users_notified_on_publish
		WHERE userId = @userId AND campaignId = @campaignId;

	END

	-- If the row exists, update it
	ELSE IF EXISTS(SELECT * FROM users_notified_on_publish WHERE userId = @userId AND campaignId = @campaignId)
	BEGIN

		UPDATE users_notified_on_publish
		SET viaSms = @viaSms, viaEmail = @viaEmail
		WHERE userId = @userId AND campaignId = @campaignId
	END

	ELSE
	-- Else, add it to the database
	BEGIN

		INSERT INTO users_notified_on_publish(userId, campaignId, viaEmail, viaSms)
		VALUES (@userId, @campaignId, @viaEmail, @viaSms);

	END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserRoleGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 19/01/2023
-- Description: Gets the user's role in a campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserRoleGet]
(
    -- Add the parameters for the stored procedure here
    @userId int,
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT roleName, roleDescription, roleLevel
	FROM campaign_users JOIN roles 
	ON campaign_users.roleId = roles.roleId 
	WHERE userId = @userId 
		AND campaign_users.campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UsersListFilter]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/02/2023
-- Description: Gets list of users from a campaign after applying filters to them. Likely most used for findings users when assigning jobs.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UsersListFilter]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@firstName nvarchar(100) = NULL,
	@lastName nvarchar(100) = NULL,
	@idNum int = NULL,
	@email nvarchar(200) = NULL,
	@phoneNumber nvarchar(20) = NULL,
	@jobStartTime datetime = NULL,
	@jobEndTime datetime = NULL,
	@streetName nvarchar(100) = NULL,
	@cityName nvarchar(100) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
	DECLARE @cityId int = (SELECT cityId FROM cities WHERE cityName = @cityName);

	IF @jobStartTime IS NULL AND @jobEndTime IS NULL
	BEGIN

		SELECT userPreferencesText, email, displayNameEng, firstNameHeb, lastNameHeb, phoneNumber, profilePicUrl, users.idNum, residenceName, streetName, houseNumber
		FROM campaign_users
		INNER JOIN users
		ON campaign_users.userId = users.userId
		LEFT JOIN user_work_preferences
		ON campaign_users.userId = user_work_preferences.userId
		LEFT JOIN voters_ledger
		ON users.idNum = voters_ledger.idNum
		WHERE   campaign_users.campaignId = @campaignId
				AND email = CASE WHEN @email IS NULL THEN email ELSE @email END

				AND users.idNum = CASE WHEN @idNum IS NULL THEN users.idNum ELSE @idNum END

				AND (firstNameHeb = CASE WHEN @firstName IS NULL THEN firstNameHeb ELSE @firstName END
					OR firstNameEng = CASE WHEN @firstName IS NULL THEN firstNameEng ELSE @firstName END)

				AND (lastNameHeb = CASE WHEN @lastName IS NULL THEN lastNameHeb ELSE @lastName END
					OR lastNameEng = CASE WHEN @lastName IS NULL THEN lastNameEng ELSE @lastName END)

				AND cityId = CASE WHEN @cityName IS NULL THEN cityId ELSE @cityId END

				AND streetId = CASE WHEN @streetName IS NULL THEN streetId ELSE (
										SELECT streetId FROM streets WHERE cityId = @cityId AND streetName = @streetName
							) END

	END

	ELSE
	BEGIN

		SELECT userPreferencesText, email, displayNameEng, firstNameHeb, lastNameHeb, phoneNumber, profilePicUrl, users.idNum, residenceName, streetName, houseNumber
		FROM campaign_users
		INNER JOIN users
		ON campaign_users.userId = users.userId
		LEFT JOIN user_work_preferences
		ON campaign_users.userId = user_work_preferences.userId
		LEFT JOIN voters_ledger
		ON users.idNum = voters_ledger.idNum
		WHERE campaign_users.campaignId = 26
				AND email = CASE WHEN @email IS NULL THEN email ELSE @email END

				AND users.idNum = CASE WHEN @idNum IS NULL THEN users.idNum ELSE @idNum END

				AND (firstNameHeb = CASE WHEN @firstName IS NULL THEN firstNameHeb ELSE @firstName END
					OR firstNameEng = CASE WHEN @firstName IS NULL THEN firstNameEng ELSE @firstName END)

				AND (lastNameHeb = CASE WHEN @lastName IS NULL THEN lastNameHeb ELSE @lastName END
					OR lastNameEng = CASE WHEN @lastName IS NULL THEN lastNameEng ELSE @lastName END)

				AND cityId = CASE WHEN @cityName IS NULL THEN cityId ELSE @cityId END

				AND streetId = CASE WHEN @streetName IS NULL THEN streetId ELSE (
										SELECT streetId FROM streets WHERE cityId = @cityId AND streetName = @streetName
							) END

				AND NOT EXISTS(
					SELECT jobs.jobId
					FROM job_assignments
					INNER JOIN jobs
					ON job_assignments.jobId = jobs.jobId
					WHERE job_assignments.userId = campaign_users.userId
							AND jobs.campaignId = @campaignId
							AND (jobStartTime BETWEEN ISNULL(@jobStartTime, '1900-01-01') AND ISNULL(@jobEndTime, '3000-01-01'))
				)

	END

END
GO
/****** Object:  StoredProcedure [dbo].[usp_UsersPublishNotificationSettingsGetForCampaign]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 07/03/2023
-- Description: Gets the notification settings and contact information of every user who signed up for notifications from a specific campaign
-- =============================================
CREATE PROCEDURE [dbo].[usp_UsersPublishNotificationSettingsGetForCampaign]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT viaEmail, viaSms, email, phoneNumber, firstNameHeb, lastNameHeb, displayNameEng
	FROM users_notified_on_publish
	INNER JOIN users
	ON users.userId = users_notified_on_publish.userId
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UsersToNotifyGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 25/01/2023
-- Description: Gets all users that should be notified on join to a campaign from the appropriate table, as well as their contact info.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UsersToNotifyGet]
(
    -- Add the parameters for the stored procedure here
    @campaignGuid uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT firstNameHeb, lastNameHeb, phoneNumber, users.email, users_to_notify_on_join.viaEmail, viaSms
	FROM users_to_notify_on_join 
	INNER JOIN users
	ON users.userId = users_to_notify_on_join.userId
	WHERE campaignId = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);
END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserToNotifyModify]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 23/01/2023
-- Description: Adds, updates or removes a user to notify to the users to notify table. Add is done when user does not yet exist, update is when they do exist and one of the values it not null, delete is done when @viaEmail and @viaSms are null
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserToNotifyModify]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@campaignGuid uniqueidentifier,
	@viaEmail bit = NULL,
	@viaSms bit = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

    -- Insert statements for procedure here
    IF @viaEmail IS NULL AND @viaSMS IS NULL
	BEGIN

		DELETE 
		FROM users_to_notify_on_join 
		WHERE campaignId = @campaignId AND userId = @userId;

		RETURN -1;

	END


	IF @viaEmail IS NULL SET @viaEmail = 0;
	IF @viaSms IS NULL SET @viaSms = 0;

	IF NOT EXISTS(SELECT * FROM users_to_notify_on_join WHERE userId = @userId AND campaignId = @campaignId)
	BEGIN

		INSERT 
		INTO users_to_notify_on_join 
		VALUES(@userId, @campaignId, @viaEmail, @viaSms);

	END

	ELSE

	BEGIN

		UPDATE users_to_notify_on_join 
		SET viaEmail = @viaEmail, viaSms = @viaSms 
		WHERE userId = @userId AND campaignId = @campaignId;

	END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_VerificationCodeAdd]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 25/01/2023
-- Description: Adds a verification code for a user's phone. Deletes previous code and number if it exists
-- =============================================
CREATE PROCEDURE [dbo].[usp_VerificationCodeAdd]
(
    -- Add the parameters for the stored procedure here
	@userId int,
	@phoneNumber nvarchar(20),
	@verificationCode nvarchar(6)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON
    -- Insert statements for procedure here
	BEGIN TRAN T

		DELETE 
		FROM phone_verification_codes 
		WHERE userId = @userId;

		INSERT 
		INTO phone_verification_codes 
		VALUES(@userId, @phoneNumber, @verificationCode, DATEADD(MINUTE, 30, GETDATE()));

	COMMIT TRAN T;
    
END
GO
/****** Object:  StoredProcedure [dbo].[usp_VerificationCodeApprove]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 25/01/2023
-- Description: Adds a phone number to the user after it was approved by the server. Also cleans up the row in the verification codes table of that user.
-- =============================================
CREATE PROCEDURE [dbo].[usp_VerificationCodeApprove]
(
    -- Add the parameters for the stored procedure here
    @userId int,
	@phoneNumber nvarchar(20)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON
    -- Insert statements for procedure here
	BEGIN TRAN T;

		DELETE
		FROM phone_verification_codes
		WHERE userId = @userId;

		UPDATE users
		SET phoneNumber = @phoneNumber
		WHERE userId = @userId;

		UPDATE voters_ledger_dynamic
		SET phone1 = @phoneNumber
		WHERE IdNum = (SELECT idNum FROM users WHERE userId = @userId);

	COMMIT TRAN T;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_VerificationCodeGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 25/01/2023
-- Description: Gets a user's phone verification code
-- =============================================
CREATE PROCEDURE [dbo].[usp_VerificationCodeGet]
(
    -- Add the parameters for the stored procedure here
    @userId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT verificationCode, phoneNumber FROM phone_verification_codes WHERE userId = @userId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_VotersLedgerEntryByIdNumGet]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 11/01/2023
-- Description: Gets a row from the voters ledger by the voter's id
-- =============================================
CREATE PROCEDURE [dbo].[usp_VotersLedgerEntryByIdNumGet]
(
    -- Add the parameters for the stored procedure here
    @voterId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT * FROM voters_ledger WHERE idNum = @voterId;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_VotersLedgerRecordsFilter]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 14/01/2023
-- Description: Gets records from the voters ledger + other tables, according to filters given by the user
-- This unholy mess of if conditions is because any attempt at making it better using ISNULL or SWITCH caused the query optimizer to lose it,
-- leading to the query taking too long.
-- =============================================
CREATE PROCEDURE [dbo].[usp_VotersLedgerRecordsFilter]
(
    -- Add the parameters for the stored procedure here
	@idNum int = 0,
	@cityName nvarchar(100) = NULL,
	@streetName nvarchar(100) = NULL,
	@ballotId float = 0,
	@campaignGuid uniqueidentifier,
	@supportStatus bit = NULL,
	@firstName nvarchar(100) = NULL,
	@lastName nvarchar(100) = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	SET XACT_ABORT ON

	IF @campaignGuid IS NULL THROW 50016, 'campaignGuid must not be null', 1;

	DECLARE @cityId int = (SELECT CASE WHEN @cityName IS NULL THEN 0 ELSE (
		SELECT cityId FROM cities WHERE cityName = @cityName
	) END);

    -- Searching by Id number - if Id number is provided, always give it priority.
	-- First case: id number provided, support status not needed.
    IF @idNum <> 0
	BEGIN

		SELECT	vdf.*, supportStatus
		FROM	VotersLedgerFullWithBallots AS vdf LEFT JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE	vdf.idNum = @idNum AND (vdf.cityId = @cityId OR @cityId = -1);

	END

	-- Get the parameters that other filters may or may not need.
	-- All columns selected are indexed.

	DECLARE @streetId int = (SELECT CASE WHEN @streetName IS NULL THEN 0 ELSE(
		SELECT streetId FROM streets WHERE cityId = @cityId AND streetName = @streetName
	) END);

	DECLARE @ballotUniversalId int = (SELECT CASE WHEN @ballotId IS NULL THEN 0 ELSE (
		SELECT ballotId FROM ballots WHERE cityId = @cityId AND innerCityBallotId = @ballotId
	) END);
	

	-- Else, start going over every possible combinations and search by them.
	-- Everything from this point is a copy paste of this, but with different conditionals on the IF and WHERE clauses.
	-- While there are definitely more intelligent ways of going about this (this could be condensed to one query using CASE),
	-- The efficiency hit from it is steep, and optimization is a must with all queries that touch the voters ledger.
	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId = 0 AND @supportStatus IS NULL AND @firstName IS NULL AND @lastName IS NULL
	BEGIN

		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId;
	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId = 0 AND @supportStatus IS NULL AND @firstName IS NULL AND @lastName IS NULL
	BEGIN
		
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.streetId = @streetId;

	END
	
	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId <> 0 AND @supportStatus IS NULL AND @firstName IS NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.streetId = @streetId AND vdf.ballotId = @ballotUniversalId;

	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId <> 0 AND @supportStatus IS NOT NULL AND @firstName IS NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.streetId = @streetId AND vdf.ballotId = @ballotUniversalId AND css.supportStatus = @supportStatus;

	END
	
	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId <> 0 AND @supportStatus IS NOT NULL AND @firstName IS NOT NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.streetId = @streetId AND vdf.ballotId = @ballotUniversalId AND vdf.firstName = @firstName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId <> 0 AND @supportStatus IS NOT NULL AND @firstName IS NOT NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.streetId = @streetId AND vdf.ballotId = @ballotUniversalId 
		AND css.supportStatus = @supportStatus AND vdf.firstName = @firstName AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId <> 0 AND @supportStatus IS NOT NULL AND @firstName IS NOT NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.ballotId = @ballotUniversalId AND css.supportStatus = @supportStatus AND vdf.firstName = @firstName AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId = 0 AND @supportStatus IS NULL AND @firstName IS NOT NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND vdf.firstName = @firstName AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId = 0 AND @supportStatus IS NULL AND @firstName IS NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId = 0 AND @supportStatus IS NULL AND @firstName IS NOT NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND vdf.firstName = @firstName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId = 0 AND @supportStatus IS NULL AND @firstName IS NOT NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND streetId = @streetId AND vdf.firstName = @firstName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId <> 0 AND @supportStatus IS NULL AND @firstName IS NOT NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND vdf.firstName = @firstName AND vdf.ballotId = @ballotUniversalId;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId <> 0 AND @supportStatus IS NULL AND @firstName IS NOT NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.ballotId = @ballotUniversalId  AND vdf.firstName = @firstName AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId <> 0 AND @supportStatus IS NULL AND @firstName IS NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.ballotId = @ballotUniversalId  AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId <> 0 AND @supportStatus IS NOT NULL AND @firstName IS NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.ballotId = @ballotUniversalId  AND vdf.lastName = @lastName AND css.supportStatus = @supportStatus;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId <> 0 AND @supportStatus IS NOT NULL AND @firstName IS NOT NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.ballotId = @ballotUniversalId  AND vdf.firstName = @firstName AND css.supportStatus = @supportStatus;

	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId = 0 AND @supportStatus IS NOT NULL AND @firstName IS NOT NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND streetId = @streetId AND vdf.firstName = @firstName AND css.supportStatus = @supportStatus;

	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId = 0 AND @supportStatus IS NOT NULL AND @firstName IS NOT NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND streetId = @streetId AND vdf.firstName = @firstName AND css.supportStatus = @supportStatus AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId = 0 AND @supportStatus IS NULL AND @firstName IS NOT NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND streetId = @streetId AND vdf.firstName = @firstName AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId = 0 AND @supportStatus IS NOT NULL AND @firstName IS NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND css.supportStatus = @supportStatus AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId = 0 AND @supportStatus IS NOT NULL AND @firstName IS NOT NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND css.supportStatus = @supportStatus AND vdf.firstName = @firstName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId = 0 AND @supportStatus IS NOT NULL AND @firstName IS NOT NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId  AND css.supportStatus = @supportStatus AND vdf.firstName = @firstName AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId = 0 AND @supportStatus IS NOT NULL AND @firstName IS NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.streetId = @streetId  AND css.supportStatus = @supportStatus AND vdf.lastName = @lastName;

	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId = 0 AND @supportStatus IS NOT NULL AND @firstName IS NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.streetId = @streetId  AND css.supportStatus = @supportStatus;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId = 0 AND @supportStatus IS NOT NULL AND @firstName IS NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND css.supportStatus = @supportStatus;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId <> 0 AND @supportStatus IS NOT NULL AND @firstName IS NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.ballotId = @ballotUniversalId AND css.supportStatus = @supportStatus;

	END

	IF @cityName IS NOT NULL AND @streetName IS NULL AND @ballotId <> 0 AND @supportStatus IS NULL AND @firstName IS NULL AND @lastName IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND vdf.ballotId = @ballotUniversalId;

	END

	IF @cityName IS NOT NULL AND @streetName IS NOT NULL AND @ballotId = 0 AND @supportStatus IS NULL AND @firstName IS NULL AND @lastName IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE cityId = @cityId AND streetId = @streetId AND vdf.lastName = @lastName;

	END

	-- When @cityName is null, the range of what can be searched needs to be much more limited
	IF @cityName IS NULL AND @firstName IS NOT NULL AND @lastName IS NOT NULL AND @supportStatus IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE vdf.firstName = @firstName AND vdf.lastName = @lastName;

	END

	IF @cityName IS NULL AND @firstName IS NULL AND @lastName IS NOT NULL AND @supportStatus IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE vdf.lastName = @lastName;

	END

	IF @cityName IS NULL AND @firstName IS NOT NULL AND @lastName IS NULL AND @supportStatus IS NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE vdf.firstName = @firstName;

	END

	IF @cityName IS NULL AND @firstName IS NOT NULL AND @lastName IS NULL AND @supportStatus IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE vdf.firstName = @firstName AND css.supportStatus = @supportStatus;

	END

	IF @cityName IS NULL AND @firstName IS NULL AND @lastName IS NOT NULL AND @supportStatus IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE vdf.lastName = @lastName AND css.supportStatus = @supportStatus;

	END

	IF @cityName IS NULL AND @firstName IS NOT NULL AND @lastName IS NOT NULL AND @supportStatus IS NOT NULL
	BEGIN
	
		SELECT vdf.*, supportStatus 
		FROM VotersLedgerFullWithBallots AS vdf LEFT OUTER JOIN 
				(
					SELECT	voter_campaign_support_statuses.*, campaignGuid
					FROM	voter_campaign_support_statuses INNER JOIN 
							campaigns ON campaigns.campaignId = voter_campaign_support_statuses.campaignId
					WHERE	campaignGuid = @campaignGuid
				) AS css ON css.idNum = vdf.idNum 
		WHERE vdf.lastName = @lastName AND vdf.lastName = @lastName AND css.supportStatus = @supportStatus;

	END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_VotesCountModify]    Script Date: 26/05/2024 16:14:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Yuval Uner
-- Create Date: 02/06/2023
-- Description: Modifies the vote count vote of a party at a ballot
-- =============================================
CREATE PROCEDURE [dbo].[usp_VotesCountModify]
(
    -- Add the parameters for the stored procedure here
	@campaignGuid uniqueidentifier,
	@ballotId int,
	@isCustomBallot bit,
	@partyId int,
	@increment bit = 1
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    DECLARE @campaignId int = (SELECT campaignId FROM campaigns WHERE campaignGuid = @campaignGuid);

	-- CampaignNotFound status code
	IF @campaignId IS NULL RETURN 50017;

	IF @isCustomBallot = 1
	BEGIN
		
		-- BallotNotFound status code
		IF NOT EXISTS(SELECT ballotId FROM custom_ballots WHERE ballotId = @ballotId) RETURN 50029;

	END

	ELSE
	BEGIN
		
		-- BallotNotFound status code
		IF NOT EXISTS(SELECT ballotId FROM ballots WHERE ballotId = @ballotId) RETURN 50029;

	END

	-- PartyNotFound status code
	IF NOT EXISTS(SELECT partyId FROM parties WHERE partyId = @partyId) RETURN 50030;

	DECLARE @numVotes int = (SELECT numVotes FROM votes WHERE campaignId = @campaignId AND ballotId = @ballotId AND isCustomBallot = @isCustomBallot
						AND partyId = @partyId)

	IF @numVotes IS NULL
	BEGIN

		INSERT INTO votes(campaignId, ballotId, isCustomBallot, partyId, numVotes)
		VALUES (@campaignId, @ballotId, @isCustomBallot, @partyId, 1);

	END

	ELSE
	BEGIN

		IF @increment = 1
		BEGIN

			UPDATE votes
			SET numVotes = numVotes + 1
			WHERE campaignId = @campaignId AND ballotId = @ballotId AND isCustomBallot = @isCustomBallot AND partyId = @partyId;

		END

		ELSE
		BEGIN

			IF @numVotes > 0
			BEGIN

				UPDATE votes
				SET numVotes = numVotes - 1
				WHERE campaignId = @campaignId AND ballotId = @ballotId AND isCustomBallot = @isCustomBallot AND partyId = @partyId;	

			END

		END

	END
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 for expense, 0 for income' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'financial_data', @level2type=N'COLUMN',@level2name=N'isExpense'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The Id of the campaign the type is associated with' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'financial_types', @level2type=N'COLUMN',@level2name=N'campaignId'
GO
