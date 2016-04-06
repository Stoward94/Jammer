SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Luke Stoward>
-- Create date: <05/04/2016>
-- Description:	<Updates the kudos awards for a given user>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateKudosAwards]
	@userId varchar(128)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @kudos FLOAT = (SELECT Points FROM dbo.Kudos WHERE UserId = @userId)
	DECLARE @awardGroupId INT = (SELECT [Id] FROM [AwardGroups] WHERE [Group] = 'Kudos')

	If @kudos < 100
		RETURN

	-- Beginner Award
	ELSE IF @kudos >= 100 AND @kudos < 1000
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 1
		END

	-- Novice Award
	ELSE IF @kudos >= 1000 AND @kudos < 10000 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 2
		END

	-- Intermediate Award
	ELSE IF @kudos >= 10000 AND @kudos < 50000 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 3
		END

	-- Advanced Award
	ELSE IF @kudos >= 50000 AND @kudos < 100000 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 4
		END

	-- Expert Award
	ELSE IF @kudos >= 100000
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 5
		END

END
