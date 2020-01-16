IF DATABASE_PRINCIPAL_ID('db_dreamusers') IS NOT NULL
BEGIN
	EXEC sp_droprolemember  'db_dreamusers', 'RENOVATEAMERICA\mmoore'
	EXEC sp_droprolemember  'db_dreamusers', 'RENOVATEAMERICA\fnedelciuc'
	EXEC sp_droprolemember  'db_dreamusers', 'RENOVATEAMERICA\jgarcia'
	EXEC sp_droprolemember  'db_dreamusers', 'RENOVATEAMERICA\bparker'
	EXEC sp_droprolemember  'db_dreamusers', 'RENOVATEAMERICA\agarfinkle'
	EXEC sp_droprolemember  'db_dreamusers', 'RENOVATEAMERICA\cbraun'
	EXEC sp_droprolemember  'db_dreamusers', 'RENOVATEAMERICA\nmontecalvo'

	DROP ROLE db_dreamusers
END