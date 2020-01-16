IF DATABASE_PRINCIPAL_ID('db_dreamusers') IS NULL
BEGIN
	CREATE ROLE db_dreamusers

	EXEC sp_addrolemember 'db_dreamusers', 'RENOVATEAMERICA\mmoore'
	EXEC sp_addrolemember 'db_dreamusers', 'RENOVATEAMERICA\fnedelciuc'
	EXEC sp_addrolemember 'db_dreamusers', 'RENOVATEAMERICA\jgarcia'
	EXEC sp_addrolemember 'db_dreamusers', 'RENOVATEAMERICA\bparker'
	EXEC sp_addrolemember 'db_dreamusers', 'RENOVATEAMERICA\agarfinkle'
	EXEC sp_addrolemember 'db_dreamusers', 'RENOVATEAMERICA\cbraun'
	EXEC sp_addrolemember 'db_dreamusers', 'RENOVATEAMERICA\nmontecalvo'
END
GO