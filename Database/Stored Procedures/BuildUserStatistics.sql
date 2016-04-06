CREATE PROCEDURE BuildUserStatistics
(
	@userId nvarchar(128)	
)
AS
BEGIN

DECLARE @Created INT
DECLARE @Completed INT

SELECT
	@Created = (SELECT Count(*) 
		FROM [Sessions] s 
			WHERE s.CreatorId = @userId),

	@Completed = (SELECT Count(*) 
		FROM [Sessions] s 
			WHERE s.CreatorId = @userId 
			AND s.StatusId = (SELECT Id FROM SessionStatus WHERE Name = 'Completed'))

UPDATE [dbo].[UserStatistics]
SET SessionsCreated = @Created,
	SessionsCompleted = @Completed
WHERE UserId = @userId


END