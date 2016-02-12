CREATE TABLE [dbo].[Udlånsinformationer]
(
	[UdlånsID] INT NOT NULL PRIMARY KEY, 
    [Adminkommentar] NVARCHAR(50) NULL, 
    [Brugerkommentar] NVARCHAR(50) NULL, 
    [Udlånsdato] DATE NULL, 
    [Afleveringsdato] DATE NULL, 
    [Komponent] INT NULL, 
    CONSTRAINT [FK_Udlånsinformationer_ToTable] FOREIGN KEY ([Komponent]) REFERENCES [Komponent]([KomponentID])
)
