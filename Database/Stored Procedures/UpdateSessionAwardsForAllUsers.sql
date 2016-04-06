SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Luke Stoward>
-- Create date: <04/04/2016>
-- Description:	<Updates the different session award types for every user>
-- =============================================
CREATE PROCEDURE UpdateSessionAwardsForAllUsers
AS
BEGIN
	SET NOCOUNT ON;

	-- First update the users statistics
	EXEC [dbo].[BuildAllUserStatistics]

	-- Determine loop boundaries.
	declare @userId VARCHAR(128)
	declare @counter int = 0
	declare @total int = isnull((select count(1) from UserProfiles), 0)
    
	-- Iterate records.
	while (@counter <> (@total))
	begin  
		-- record to proces.
		set @userId = (select UserId from UserProfiles order by UserId offset @counter rows fetch next 1 rows only)

		-- Update 'Created' Awards
		EXEC [dbo].[UpdateCreatedAwards] @userId = @userId

		-- Update 'Completed' Awards
		EXEC [dbo].[UpdateCompletedAwards] @userId = @userId
                    
		-- Increase counter to break the loop after all records are processed.
		set @counter = @counter + 1
	end
    
END
GO
