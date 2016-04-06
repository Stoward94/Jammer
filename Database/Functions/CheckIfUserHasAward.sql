CREATE FUNCTION CheckIfUserHasAward (@userId VARCHAR(128), @awardId INT)
RETURNS BIT
AS BEGIN
	
	DECLARE @userAwardId INT

	SELECT @userAwardId = Id FROM UserAwards
	WHERE UserId = @userId
	AND AwardId = @awardId

	IF @userAwardId IS NULL
		RETURN 0
	
	Return 1
END