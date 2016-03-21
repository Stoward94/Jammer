-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Luke Stoward>
-- Create date: <15/02/2016>
-- Description:	<Updates Session Status to inprogress or completed>
-- =============================================
CREATE PROCEDURE UpdateSessionStatus
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @now DATETIME
	SET @now = GETUTCDATE()
	PRINT (@now)

	-- Update sessions that should now be inprogress
	UPDATE [dbo].[Sessions]
	SET StatusId = 3 -- Mark session as in progress
	WHERE ScheduledDate <= @now
	AND EndTime > @now
	AND StatusId <> 3 -- Where not already inprogress

    -- Update sessions that have reached their end date/time to complete
	UPDATE [dbo].[Sessions]
	SET StatusId = 4, -- Mark session as complete
	Active = 0 -- And mark as no longer active
	WHERE EndTime <= @now -- If end time has been reached
	AND StatusId <> 4 -- Where not complete
END
GO
