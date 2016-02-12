CREATE TABLE [dbo].[Bruger]
(
	[StudienummerID] INT NOT NULL PRIMARY KEY, 
    [Fornavn] NVARCHAR(50) NULL, 
    [Efternavn] NVARCHAR(50) NULL, 
    [Email] NVARCHAR(50) NOT NULL, 
    [Mobilnr] INT NULL
)
