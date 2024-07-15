-- MainProcedure.sql
USE [MaBaseDeDonnees]
GO

-- Définition de quelques opérations de base
SELECT * FROM MaTable
INSERT INTO MaTable (Colonne1, Colonne2) VALUES ('Valeur1', 'Valeur2')
UPDATE MaTable SET Colonne1 = 'NouvelleValeur' WHERE Colonne2 = 'Valeur2'
MERGE INTO MaTable USING AutreTable ON MaTable.Colonne1 = AutreTable.Colonne1
WHEN MATCHED THEN
    UPDATE SET MaTable.Colonne2 = AutreTable.Colonne2
WHEN NOT MATCHED THEN
    INSERT (Colonne1, Colonne2) VALUES (Colonne1, Colonne2);

-- Appel de la procédure stockée définie dans le second fichier
EXEC SecondProcedure
