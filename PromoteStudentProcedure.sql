USE Students
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [PromoteStudents] @Studies NVARCHAR(100), @Semester INT
AS
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRANSACTION

	DECLARE @NewSemester INT = @Semester + 1;
	DECLARE @IdStudies INT = (SELECT IdStudy FROM Studies WHERE Name=@Studies);

	IF @IdStudies IS NULL
		BEGIN
			RAISERROR ('Podane studia nie istnieja', -1, 1);
			RETURN;
		END

	DECLARE @IdEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE Semester=@Semester AND IdStudy=@IdStudies);

	IF @IdEnrollment IS NULL
		BEGIN
			RAISERROR ('Podana rekrutacja nie istnieje', -1, 1);
			RETURN;
		END

	DECLARE @NewEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE Semester=@NewSemester AND IdStudy=@IdStudies);

	IF @NewEnrollment IS NULL
		BEGIN
			DECLARE @NewMadeEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE IdEnrollment >= (SELECT MAX(IdEnrollment) FROM Enrollment));
			INSERT INTO Enrollment VALUES (@NewMadeEnrollment+10, @NewSemester, @IdStudies, GETDATE());
			UPDATE Student SET IdEnrollment = @NewMadeEnrollment+10;
			COMMIT
			RETURN @NewMadeEnrollment+10;
		END

	UPDATE Student SET IdEnrollment = @NewEnrollment;

	COMMIT
	RETURN @NewEnrollment;

END
