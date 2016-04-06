USE TriggerWars;
GO

-- =============================================
-- Author:		<Luke Stoward>
-- Create date: <05/04/2016>
-- Description:	<Trigger designed to update users kudos awards when kudos points are added>
-- =============================================
CREATE TRIGGER KudosAwardUpdateCheck
ON [dbo].[Kudos]
AFTER UPDATE 
AS 
IF ( UPDATE (Points))
BEGIN

	declare @userId VARCHAR(128)
	declare @counter int = 0
	declare @total int = isnull((select count(1) from inserted), 0)
    
	-- Iterate records.
	while (@counter <> (@total))
	begin  
		-- record to proces.
		set @userId = (SELECT UserId FROM inserted order by UserId offset @counter rows fetch next 1 rows only)

		-- Update 'Kudos' Awards
		EXEC Dbo.UpdateKudosAwards @userId = @userId
		                    
		-- Increase counter to break the loop after all records are processed.
		set @counter = @counter + 1
	end
	
	
END;
GO