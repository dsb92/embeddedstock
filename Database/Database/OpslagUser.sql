INSERT INTO [User] (StudentID, FirstName, LastName, Email, MobileNr)
VALUES (201212345, N'Kaj', N'Hansen', N'201212345@iha.dk', 28123456)
GO

SELECT * FROM [User] 
GO

SELECT        dbo.[User].StudentID, dbo.Component.ComponentID, dbo.LoanInformation.ReturnDate
FROM            dbo.Component INNER JOIN
                         dbo.LoanInformation ON dbo.Component.ComponentID = dbo.LoanInformation.Component INNER JOIN
                         dbo.[User] ON dbo.Component.OwnerID = dbo.[User].StudentID
GO