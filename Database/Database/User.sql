CREATE TABLE [dbo].[User]
(
	[StudentID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FirstName] NVARCHAR(50) NULL, 
    [LastName] NVARCHAR(50) NULL, 
    [Email] NVARCHAR(50) NOT NULL, 
    [MobileNr] INT NULL
)
