-- SecondProcedure.sql
USE [MaBaseDeDonnees]
GO

-- Création de la procédure stockée
CREATE PROCEDURE SecondProcedure
AS
BEGIN
    -- Un nombre différent d'opérations de base
    SELECT * FROM AutreTable
    INSERT INTO AutreTable (ColonneA, ColonneB) VALUES ('ValeurA', 'ValeurB')
END
GO
