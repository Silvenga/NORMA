@ECHO OFF
SETLOCAL
SET TargetVisualStudioVersion=v9.0

CALL "%~dp0Build.bat" %* /consoleloggerparameters:DisableMPLogging /toolsversion:3.5