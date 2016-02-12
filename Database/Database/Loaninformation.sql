CREATE TABLE [dbo].[LoanInformation]
(
	[LoanID] INT NOT NULL PRIMARY KEY IDENTITY,
	[Component] INT NOT NULL, 
    [AdminComment] NVARCHAR(50) NULL, 
    [UserComment] NVARCHAR(50) NULL, 
    [LoanDate] DATETIME NULL, 
    [ReturnDate] DATETIME NULL, 
    [IsEmailSend] NVARCHAR(50) NULL, 
    CONSTRAINT [FK_LoanInformation_ToComponent] FOREIGN KEY ([Component]) REFERENCES [Component]([ComponentID])
)
