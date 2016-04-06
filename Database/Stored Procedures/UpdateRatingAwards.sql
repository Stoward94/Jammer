SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Luke Stoward>
-- Create date: <05/04/2016>
-- Description:	<Updates the rating awards for a given user>
-- =============================================
CREATE PROCEDURE UpdateRatingAwards
	@userId varchar(128)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @rating FLOAT = (SELECT Rating FROM dbo.UserProfiles WHERE UserId = @userId)
	DECLARE @awardGroupId INT = (SELECT [Id] FROM [AwardGroups] WHERE [Group] = 'Rating')

	If @rating < 5.0
		RETURN

	-- Beginner Award
	ELSE IF @rating >= 5 AND @rating < 6
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 1
		END

	-- Novice Award
	ELSE IF @rating >= 6 AND @rating < 7 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 2
		END

	-- Intermediate Award
	ELSE IF @rating >= 7 AND @rating < 8 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 3
		END

	-- Advanced Award
	ELSE IF @rating >= 8 AND @rating < 9 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 4
		END

	-- Expert Award
	ELSE IF @rating >= 9
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 5
		END

END
GO
