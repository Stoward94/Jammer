SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Luke Stoward>
-- Create date: <04/04/2016>
-- Description:	<Checks to see if the user has an award and if not, adds it>
-- =============================================
CREATE PROCEDURE CheckAddAwardToUser
	-- Add the parameters for the stored procedure here
	@userId VARCHAR(128),
	@awardGroupId INT,
	@awardLevelId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @awardId INT
	DECLARE @userAwardId INT

	-- Workout which award we are after
	SELECT @awardId = Id
	FROM Awards
	WHERE GroupId = @awardGroupId
		AND LevelId = @awardLevelId

	IF @awardId IS NULL
		RETURN

	-- Check if the user already has this award
	DECLARE @hasAward BIT = dbo.CheckIfUserHasAward(@userId, @awardId)

	--If the user doesn't have the award then add it
	IF @hasAward = 0
		BEGIN
			INSERT INTO UserAwards (UserId, AwardId, DateAwarded) 
			VALUES(@userId, @awardId, GETUTCDATE())
		END


END
GO
