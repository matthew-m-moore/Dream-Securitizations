USE CapitalMarkets
GO

IF (NOT EXISTS (SELECT * FROM SYS.SCHEMAS WHERE NAME = 'dream'))
BEGIN
	-- Note that creation of a schema must be the only statement in a batch
	EXEC('CREATE SCHEMA dream')
END