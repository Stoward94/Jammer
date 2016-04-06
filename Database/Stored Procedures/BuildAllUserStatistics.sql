USE [TriggerWars]
GO

CREATE PROCEDURE BuildAllUserStatistics

AS
BEGIN

	UPDATE [dbo].[UserStatistics]

	SET [SessionsCreated] = (SELECT Count(*) FROM [Sessions] s WHERE s.CreatorId = us.UserId),
		[SessionsCompleted] = (SELECT Count(*) FROM [Sessions] s WHERE s.CreatorId = us.UserId 
								AND s.StatusId = (SELECT Id FROM SessionStatus WHERE Name = 'Completed'))

	FROM [dbo].[UserStatistics] us

END