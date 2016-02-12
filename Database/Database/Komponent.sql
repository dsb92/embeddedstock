CREATE TABLE [dbo].[Komponent]
(
	[KomponentID] INT NOT NULL PRIMARY KEY,
	[KomponentNummer] INT NOT NULL, 
    [Tags] NVARCHAR(50) NOT NULL, 
    [KomponentNavn] NVARCHAR(50) NOT NULL, 
    [Kategori] NVARCHAR(50) NULL, 
    [Datablad] NVARCHAR(50) NULL, 
    [EjerID] INT NULL, 
    CONSTRAINT [FK_Komponent_] FOREIGN KEY ([EjerID]) REFERENCES [Bruger](StudienummerID)
)
