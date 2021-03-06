SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Luke Stoward>
-- Create date: <04/04/2016>
-- Description:	<Adds created awards to the user awards table, if the requirements are met>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateCreatedAwards] 
	-- Add the parameters for the stored procedure here
	@userId varchar(128)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @createdCount INT = (SELECT SessionsCreated FROM dbo.UserStatistics WHERE UserId = @userId)
	DECLARE @awardGroupId INT = (SELECT [Id] FROM [AwardGroups] WHERE [Group] = 'Sessions Created')

	If @createdCount = 0
		RETURN

	-- Beginner Award
	ELSE IF @createdCount >= 1 AND @createdCount < 10
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 1
		END

	-- Novice Award
	ELSE IF @createdCount >= 10 AND @createdCount < 100 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 2
		END

	-- Intermediate Award
	ELSE IF @createdCount >= 100 AND @createdCount < 250 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 3
		END

	-- Advanced Award
	ELSE IF @createdCount >= 250 AND @createdCount < 1000 
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 4
		END

	-- Expert Award
	ELSE IF @createdCount >= 1000
		BEGIN

			EXEC CheckAddAwardToUser 
				@userId = @userId, 
				@awardGroupId = @awardGroupId, 
				@awardLevelId = 5
		END

END
