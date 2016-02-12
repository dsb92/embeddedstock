INSERT INTO [Component] (ComponentID, ComponentNumber, Tags, ComponentName, Category, Datasheet, Image, OwnerID)
VALUES (1, 10, N'Potentiometer, 10W', N'10W POTENTIOMETER 470 OHM', N'Modstand', 'Link her', NULL, NULL)

UPDATE [Component]
SET OwnerID=201212345

SELECT * FROM Component
