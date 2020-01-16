@ECHO OFF
SETLOCAL enabledelayedexpansion 
SET ServerName=AWSSQLBIDEV01
SET DatbaseName=RAHERODW
SET Directory=./
ECHO %ServerName%
ECHO %DatbaseName%
ECHO %Directory%
FOR /R %%f IN (*.sql) DO ( 
ECHO Deploying %%f ... 
SQLCMD -b -E -S %ServerName% -d %DatbaseName% -i %%f
IF !ERRORLEVEL!==1 ( 
ECHO Error Deploying %%f
PAUSE
GOTO ENDPRO 
) 
ECHO Completed %%f 
)  
:ENDPRO 