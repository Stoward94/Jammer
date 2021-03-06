SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Luke Stoward>
-- Create date: <04/04/2016>
-- Description:	<Adds completed awards to the user awards table, if the requirements are met>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateCompletedAwards] 
	-- Add the parameters for the stored procedure here
	@userId varchar(128)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @completedCount INT = (SELECT SessionsCompleted FROM dbo.UserStatistics WHERE UserId = @userId)
	DECLARE @awardGroupId INT = (SELECT [Id] FROM [AwardGroups] WHERE [Group] = 'Sessions Completed')

	If @completedCount = 0
		RETURN

	-- Beginner Award
	ELSE IF @completedCount >= 1 AND @completedCount < 10
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 1
		END

	-- Novice Award
	ELSE IF @completedCount >= 10 AND @completedCount < 100 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 2
		END

	-- Intermediate Award
	ELSE IF @completedCount >= 100 AND @completedCount < 250 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 3
		END

	-- Advanced Award
	ELSE IF @completedCount >= 250 AND @completedCount < 1000 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 4
		END

	-- Expert Award
	ELSE IF @completedCount >= 1000
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 5
		END

END
