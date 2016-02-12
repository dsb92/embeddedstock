SELECT        dbo.[User].StudentID, dbo.Component.ComponentID, dbo.Component.ComponentName, dbo.LoanInformation.LoanDate, dbo.LoanInformation.ReturnDate
FROM            dbo.Component INNER JOIN
                         dbo.LoanInformation ON dbo.Component.ComponentID = dbo.LoanInformation.Component INNER JOIN
                         dbo.[User] ON dbo.Component.OwnerID = dbo.[User].StudentID