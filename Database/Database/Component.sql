CREATE TABLE [dbo].[Component]
(
	[ComponentID] INT NOT NULL PRIMARY KEY IDENTITY,
	[ComponentNumber] INT NOT NULL, 
	[SerieNr] NVARCHAR(50) NULL,
    [ComponentName] NVARCHAR(50) NOT NULL,
	[ComponentInfo] NVARCHAR(MAX) NULL,  
    [Category] NVARCHAR(50) NULL, 
    [Datasheet] NVARCHAR(50) NULL, 
    [Image] VARCHAR(MAX) NULL,
    [AdminComment] NVARCHAR(MAX) NULL, 
    [UserComment] NVARCHAR(MAX) NULL, 
)
