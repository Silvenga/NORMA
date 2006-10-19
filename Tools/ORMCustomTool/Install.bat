@ECHO OFF
SETLOCAL

FOR /F "usebackq skip=3 tokens=2*" %%A IN (`REG QUERY "HKLM\SOFTWARE\Microsoft\VisualStudio\8.0\Setup\VS" /v "ProductDir"`) DO SET VSDir=%%~fB
SET XMLDir=%~dp0\..\..\XML
SET NORMADir=%ProgramFiles%\Neumont\ORM Architect for Visual Studio
SET ORMTransformsDir=%CommonProgramFiles%\Neumont\ORM\Transforms
SET DILTransformsDir=%CommonProgramFiles%\Neumont\DIL\Transforms
SET PLiXDir=%CommonProgramFiles%\Neumont\PLiX

:: Generate a native image for System.Data.SqlXml.dll (this greatly improves the XSLT compilation speed)
ngen.exe install "System.Data.SqlXml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /nologo /verbose

:: Install Custom Tool DLL
DEL /F /Q "%VSDir%\Common7\IDE\PrivateAssemblies\Neumont.Tools.ORM.ORMCustomTool.*" 1>NUL 2>&1
DEL /F /Q "%NORMADir%\bin\Neumont.Tools.ORM.ORMCustomTool.dll.delete.*" 1>NUL 2>&1
IF EXIST "%NORMADir%\bin\Neumont.Tools.ORM.ORMCustomTool.dll" (REN "%NORMADir%\bin\Neumont.Tools.ORM.ORMCustomTool.dll" "Neumont.Tools.ORM.ORMCustomTool.dll.delete.%RANDOM%")
XCOPY /Y /D /V /Q "%~dp0\bin\Neumont.Tools.ORM.ORMCustomTool.dll" "%NORMADir%\bin\"
XCOPY /Y /D /V /Q "%~dp0\bin\Neumont.Tools.ORM.ORMCustomTool.pdb" "%NORMADir%\bin\"
:: For some reason, the next copy is randomly giving errors about half the time. They can be safely ignored, so they've been redirected to NUL.
XCOPY /Y /D /V /Q "%~dp0\bin\Neumont.Tools.ORM.ORMCustomTool.xml" "%NORMADir%\bin\" 2>NUL
CALL:_InstallCustomToolReg "8.0"
CALL:_InstallExtenderReg "8.0"
CALL:_InstallCustomToolReg "8.0Exp"
CALL:_InstallExtenderReg "8.0Exp"

:: Install and register ORM Transforms
XCOPY /Y /D /V /Q "%XMLDir%\OIAL\CoRefORM.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIAL\ORMtoOIAL.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoXSD\OIALtoXSD.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoOWL\OIALtoOWL.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoDCIL\OIALtoDCIL.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\OIALtoPLiX_GenerateGlobalSupportClasses.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\OIALtoPLiX_GenerateTuple.xslt" "%ORMTransformsDir%\"

XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\OIALtoPLiX_Abstract.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\OIALtoPLiX_DataLayer_Implementation.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\OIALtoPLiX_DataLayer_SprocFree_Implementation.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\OIALtoPLiX_GlobalSupportFunctions.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\OIALtoPLiX_GlobalSupportParameters.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\OIALtoPLiX_InMemory_Implementation.xslt" "%ORMTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\OIALtoCLIProperties.xslt" "%ORMTransformsDir%\"

XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\DataLayerTestForm\OIALtoPLiX_DataLayerTestForm.xslt" "%ORMTransformsDir%\DataLayerTestForm\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\DataLayerTestForm\OIALtoPLiX_DataLayerTestForm_Designer.xslt" "%ORMTransformsDir%\DataLayerTestForm\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\DataLayerTestForm\OIALtoDataLayerTestForm.resx.xslt" "%ORMTransformsDir%\DataLayerTestForm\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\DataLayerTestForm\OIALtoPLiX_InputControl.xslt" "%ORMTransformsDir%\DataLayerTestForm\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\DataLayerTestForm\OIALtoPLiX_InputControl_Designer.xslt" "%ORMTransformsDir%\DataLayerTestForm\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\DataLayerTestForm\OIALtoInputControl.resx.xslt" "%ORMTransformsDir%\DataLayerTestForm\"
XCOPY /Y /D /V /Q "%XMLDir%\OIALtoPLiX\DataLayerTestForm\OIALtoPLiX_Program.xslt" "%ORMTransformsDir%\DataLayerTestForm\"

CALL:_AddXslORMGenerator "CoRefORM" "ORM Co-Referencer" "Co-references (binarizes) an ORM file." ".CoRef.orm" "ORM" "CoRefORM" "%ORMTransformsDir%\CoRefORM.xslt" "" "1"
CALL:_AddXslORMGenerator "ORMtoOIAL" "ORM to OIAL" "Transforms a coreferenced ORM file to OIAL." ".OIAL.xml" "CoRefORM" "OIAL" "%ORMTransformsDir%\ORMtoOIAL.xslt" "" "1"
CALL:_AddXslORMGenerator "OIALtoXSD" "OIAL to XSD" "Transforms an OIAL file to XML Schema." ".xsd" "OIAL" "XSD" "%ORMTransformsDir%\OIALtoXSD.xslt"
CALL:_AddXslORMGenerator "OIALtoOWL" "OIAL to OWL" "Transforms an OIAL file to OWL." ".owl" "OIAL" "OWL" "%ORMTransformsDir%\OIALtoOWL.xslt"
CALL:_AddXslORMGenerator "OIALtoDCIL" "OIAL to DCIL" "Transforms an OIAL file to DCIL." ".DCIL.xml" "OIAL" "DCIL" "%ORMTransformsDir%\OIALtoDCIL.xslt"

CALL:_AddXslORMGenerator "OIALtoCLIProperties" "OIAL to CLI Properties" "Transforms an OIAL file to CLI (Common Language Infrastructure) Properties" ".CLIProperties.xml" "OIAL" "CLIProperties" "%ORMTransformsDir%\OIALtoCLIProperties.xslt"
CALL:_AddXslORMGenerator "PLiXSupport" "PLiX Support" "Transforms nothing to SupportClasses PLiX." ".Support.PLiX.xml" "OIAL" "PLiX_Support" "%ORMTransformsDir%\OIALtoPLiX_GenerateGlobalSupportClasses.xslt" ""
CALL:_AddXslORMGenerator "CLIPropertiesToPLiXAbstract" "CLIProperties to PLiX Abstract" "Transforms a CLI Properties file to Abstract PLiX" ".Abstract.PLiX.xml" "CLIProperties" "PLiX_Abstract" "%ORMTransformsDir%\OIALtoPLiX_Abstract.xslt" "" "" "" "OIAL\0"
CALL:_AddXslORMGenerator "CLIPropertiesToPLiXDataLayerWithSproc" "CLIProperties to PLiX Data Layer with Sprocs" "Transforms a CLI Properties file to DataLayer PLiX" ".Implementation.PLiX.xml" "CLIProperties" "PLiX_Implementation" "%ORMTransformsDir%\OIALtoPLiX_DataLayer_Implementation.xslt" "" "" "" "OIAL\0"
CALL:_AddXslORMGenerator "CLPPropertiesToPliXDataLayerSprocFree" "CLIProperties to PLiX Sproc Free Data Layer" "Transforms a CLI Properties file to Sproc Free Data Layer PLiX" ".Implementation.PLiX.xml" "CLIProperties" "PLiX_Implementation" "%ORMTransformsDir%\OIALtoPLiX_DataLayer_SprocFree_Implementation.xslt" "" "" "" "OIAL\0"
CALL:_AddXslORMGenerator "CLIPropertiesToPLiXInMemory" "CLIProperties to PLiX In Memory" "Transforms a CLI Properties file to InMemory PLiX" ".Implementation.PLiX.xml" "CLIProperties" "PLiX_Implementation" "%ORMTransformsDir%\OIALtoPLiX_InMemory_Implementation.xslt" "" "" "" "OIAL\0"

CALL:_AddXslORMGenerator "DataLayerTestForm" "Data Layer Test Form" "Generates a Windows Form with custom controls for testing and manipulating data using the generated data access layer and database." ".DataLayerTestForm.PLiX.xml" "PLiX_Implementation" "DataLayerTestForm" "%ORMTransformsDir%\DataLayerTestForm\OIALtoPLiX_DataLayerTestForm.xslt"
CALL:_AddXslORMGenerator "DataLayerTestFormDesigner" "Data Layer Test Form Designer" "Generates a Windows Form with custom controls for testing and manipulating data using the generated data access layer and database." ".DataLayerTestForm.Designer.PLiX.xml" "PLiX_Implementation" "DataLayerTestFormDesigner" "%ORMTransformsDir%\DataLayerTestForm\OIALtoPLiX_DataLayerTestForm_Designer.xslt"
CALL:_AddXslORMGenerator "DataLayerTestFormResx" "Data Layer Test Form Resx" "Generates a Windows Form with custom controls for testing and manipulating data using the generated data access layer and database." ".DataLayerTestForm.resx" "PLiX_Implementation" "DataLayerTestFormResx" "%ORMTransformsDir%\DataLayerTestForm\OIALtoDataLayerTestForm.resx.xslt"
CALL:_AddXslORMGenerator "DataLayerTestFormInputControl" "Data Layer Test Form Input Control" "Generates a custom controls to be used on the generated form for testing and manipulating data using the generated data access layer and database." ".DataLayerTestFormInputControl.PLiX.xml" "PLiX_Implementation" "DataLayerTestFormInputControl" "%ORMTransformsDir%\DataLayerTestForm\OIALtoPLiX_InputControl.xslt" "" "" "" "OIAL\0"
CALL:_AddXslORMGenerator "DataLayerTestFormInputControlDesigner" "Data Layer Test Form Input Control Designer" "Generates a custom controls to be used on the generated form for testing and manipulating data using the generated data access layer and database." ".DataLayerTestFormInputControl.Designer.PLiX.xml" "PLiX_Implementation" "DataLayerTestFormInputControlDesigner" "%ORMTransformsDir%\DataLayerTestForm\OIALtoPLiX_InputControl_Designer.xslt" "" "" "" "OIAL\0"
CALL:_AddXslORMGenerator "DataLayerTestFormInputControlResx" "Data Layer Test Form Input Control Resx" "Generates a custom controls to be used on the generated form for testing and manipulating data using the generated data access layer and database." ".DataLayerTestFormInputControl.resx" "PLiX_Implementation" "DataLayerTestFormInputControlResx" "%ORMTransformsDir%\DataLayerTestForm\OIALtoInputControl.resx.xslt"
CALL:_AddXslORMGenerator "DataLayerTestFormProgram" "Data Layer Test Form Program Class" "Generates the Program.cs file to launch the generated form." ".Program.PLiX.xml" "PLiX_Implementation" "DataLayerTestFormProgram" "%ORMTransformsDir%\DataLayerTestForm\OIALtoPLiX_Program.xslt"

:: Install and register DIL Transforms
XCOPY /Y /D /V /Q "%XMLDir%\DILtoSQL\DCILtoDDIL.xslt" "%DILTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\DILtoSQL\DDILtoSQLStandard.xslt" "%DILTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\DILtoSQL\DDILtoPostgreSQL.xslt" "%DILTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\DILtoSQL\DDILtoDB2.xslt" "%DILTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\DILtoSQL\DDILtoSQLServer.xslt" "%DILTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\DILtoSQL\DDILtoOracle.xslt" "%DILTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\DILtoSQL\DomainInliner.xslt" "%DILTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\DIL\DILSupportFunctions.xslt" "%DILTransformsDir%\"
CALL:_AddXslORMGenerator "DCILtoDDIL" "DCIL to DDIL" "Transforms DCIL to DDIL." ".DDIL.xml" "DCIL" "DDIL" "%DILTransformsDir%\DCILtoDDIL.xslt"
CALL:_AddXslORMGenerator "DDILtoSQLStandard" "DDIL to SQL Standard" "Transforms DDIL to Standard-dialect SQL." ".SQLStandard.sql" "DDIL" "SQL_SQLStandard" "%DILTransformsDir%\DDILtoSQLStandard.xslt"
CALL:_AddXslORMGenerator "DDILtoPostgreSQL" "DDIL to PostgreSQL" "Transforms DDIL to PostgreSQL-dialect SQL." ".PostgreSQL.sql" "DDIL" "SQL_PostgreSQL" "%DILTransformsDir%\DDILtoPostgreSQL.xslt"
CALL:_AddXslORMGenerator "DDILtoDB2" "DDIL to DB2" "Transforms DDIL to DB2-dialect SQL." ".DB2.sql" "DDIL" "SQL_DB2" "%DILTransformsDir%\DDILtoDB2.xslt"
CALL:_AddXslORMGenerator "DDILtoSQLServer" "DDIL to SQL Server" "Transforms DDIL to SQL Server-dialect SQL." ".SQLServer.sql" "DDIL" "SQL_SQLServer" "%DILTransformsDir%\DDILtoSQLServer.xslt"
CALL:_AddXslORMGenerator "DDILtoOracle" "DDIL to Oracle" "Transforms DDIL to Oracle-dialect SQL." ".Oracle.sql" "DDIL" "SQL_Oracle" "%DILTransformsDir%\DDILtoOracle.xslt"
XCOPY /Y /D /V /Q "%XMLDir%\DCILtoHTML\DCILtoTV.xslt" "%DILTransformsDir%\"
XCOPY /Y /D /V /Q "%XMLDir%\DCILtoHTML\TVtoHTML.xslt" "%DILTransformsDir%\"
CALL:_AddXslORMGenerator "DCILtoTV" "DCIL to TableView" "Transforms DCIL to TableView." ".TableView.xml" "DCIL" "TV" "%DILTransformsDir%\DCILtoTV.xslt"
CALL:_AddXslORMGenerator "TVtoHTML" "TableView to HTML" "Transforms TableView to HTML." ".TableView.html" "TV" "TableViewHTML" "%DILTransformsDir%\TVtoHTML.xslt"

:: Register PLiX Transforms
CALL:_AddXslORMGenerator "PLiXtoCSharpSupport" "PLiX to C# Support" "Transforms PLiX to C#." ".Support.cs" "PLiX_Support" "CSharp_Support" "%PLiXDir%\Formatters\PLiXCS.xslt" "" "" "1"
CALL:_AddXslORMGenerator "PLiXtoVisualBasicSupport" "PLiX to Visual Basic Support" "Transforms PLiX to Visual Basic." ".Support.vb" "PLiX_Support" "VisualBasic_Support" "%PLiXDir%\Formatters\PLiXVB.xslt" "" "" "1"
CALL:_AddXslORMGenerator "PLiXtoCSharpAbstract" "PLiX to C# Abstract" "Transforms PLiX to C#." ".Abstract.cs" "PLiX_Abstract" "CSharp_Abstract" "%PLiXDir%\Formatters\PLiXCS.xslt" "" "" "1" "" "CSharp_Support\0"
CALL:_AddXslORMGenerator "PLiXtoVisualBasicAbstract" "PLiX to Visual Basic Abstract" "Transforms PLiX to Visual Basic." ".Abstract.vb" "PLiX_Abstract" "VisualBasic_Abstract" "%PLiXDir%\Formatters\PLiXVB.xslt" "" "" "1" "" "VisualBasic_Support\0"
CALL:_AddXslORMGenerator "PLiXtoCSharpImplementation" "PLiX to C# Implementation" "Transforms PLiX to C#." ".Implementation.cs" "PLiX_Implementation" "CSharp_Implementation" "%PLiXDir%\Formatters\PLiXCS.xslt" "" "" "1" "" "CSharp_Abstract\0"
CALL:_AddXslORMGenerator "PLiXtoVisualBasicImplementation" "PLiX to Visual Basic Implementation" "Transforms PLiX to Visual Basic." ".Implementation.vb" "PLiX_Implementation" "VisualBasic_Implementation" "%PLiXDir%\Formatters\PLiXVB.xslt" "" "" "1" "" "VisualBasic_Abstract\0"

CALL:_AddXslORMGenerator "PLiXtoCSharpDataLayerTestForm" "PLiX to C# Data Layer Test Form" "Transforms PLiX to C#." ".DataLayerTestForm.cs" "DataLayerTestForm" "DataLayerTestForm_CSharp" "%PLiXDir%\Formatters\PLiXCS.xslt" "" "" "1" "" "CSharp_Implementation\0DataLayerTestFormDesigner_CSharp\0DataLayerTestFormResx\0DataLayerTestFormInputControl_CSharp\0"
CALL:_AddXslORMGenerator "PLiXtoVisualBasicDataLayerTestForm" "PLiX to Visual Basic Windows Form" "Transforms PLiX to Visual Basic." ".DataLayerTestForm.vb" "DataLayerTestForm" "DataLayerTestForm_VisualBasic" "%PLiXDir%\Formatters\PLiXVB.xslt" "" "" "1" "" "VisualBasic_Implementation\0DataLayerTestFormDesigner_VisualBasic\0DataLayerTestFormResx\0DataLayerTestFormInputControl_VisualBasic\0"
CALL:_AddXslORMGenerator "PLiXtoCSharpDataLayerTestFormDesigner" "PLiX to C# Windows Form Designer" "Transforms PLiX to C#." ".DataLayerTestForm.Designer.cs" "DataLayerTestFormDesigner" "DataLayerTestFormDesigner_CSharp" "%PLiXDir%\Formatters\PLiXCS.xslt" "" "" "1"
CALL:_AddXslORMGenerator "PLiXtoVisualBasicDataLayerTestFormDesigner" "PLiX to Visual Basic Windows Form Designer" "Transforms PLiX to Visual Basic." ".DataLayerTestForm.Designer.vb" "DataLayerTestFormDesigner" "DataLayerTestFormDesigner_VisualBasic" "%PLiXDir%\Formatters\PLiXVB.xslt" "" "" "1"
CALL:_AddXslORMGenerator "PLiXtoCSharpDataLayerTestFormInputControl" "PLiX to C# DataLayerTestFormInputControl" "Transforms PLiX to C#." ".DataLayerTestFormInputControl.cs" "DataLayerTestFormInputControl" "DataLayerTestFormInputControl_CSharp" "%PLiXDir%\Formatters\PLiXCS.xslt" "" "" "1" "" "DataLayerTestFormInputControlDesigner_CSharp\0DataLayerTestFormInputControlResx\0"
CALL:_AddXslORMGenerator "PLiXtoVisualBasicDataLayerTestFormInputControl" "PLiX to Visual Basic DataLayerTestFormInputControl" "Transforms PLiX to Visual Basic." ".DataLayerTestFormInputControl.vb" "DataLayerTestFormInputControl" "DataLayerTestFormInputControl_VisualBasic" "%PLiXDir%\Formatters\PLiXVB.xslt" "" "" "1" "" "DataLayerTestFormInputControlDesigner_VisualBasic\0DataLayerTestFormInputControlResx\0"
CALL:_AddXslORMGenerator "PLiXtoCSharpDataLayerTestFormInputControlDesigner" "PLiX to C# DataLayerTestFormInputControl Designer" "Transforms PLiX to C#." ".DataLayerTestFormInputControl.Designer.cs" "DataLayerTestFormInputControlDesigner" "DataLayerTestFormInputControlDesigner_CSharp" "%PLiXDir%\Formatters\PLiXCS.xslt" "" "" "1"
CALL:_AddXslORMGenerator "PLiXtoVisualBasicDataLayerTestFormInputControlDesigner" "PLiX to Visual Basic DataLayerTestFormInputControl Designer" "Transforms PLiX to Visual Basic." ".DataLayerTestFormInputControl.Designer.vb" "DataLayerTestFormInputControlDesigner" "DataLayerTestFormInputControlDesigner_VisualBasic" "%PLiXDir%\Formatters\PLiXVB.xslt" "" "" "1"
CALL:_AddXslORMGenerator "DataLayerTestFormProgram_CSharp" "PLiX to C# Program.cs" "Transforms PLiX to C#." ".Program.cs" "DataLayerTestFormProgram" "DataLayerTestFormProgram_CSharp" "%PLiXDir%\Formatters\PLiXCS.xslt" "1" "" "DataLayerTestForm_CSharp\0"
CALL:_AddXslORMGenerator "DataLayerTestFormProgram_VisualBasic" "PLiX to Visual Basic Program.vb" "Transforms PLiX to Visual Basic." ".Program.vb" "DataLayerTestFormProgram" "DataLayerTestFormProgram_VisualBasic" "%PLiXDir%\Formatters\PLiXVB.xslt" "" "" "1" "" "DataLayerTestForm_VisualBasic"

GOTO:EOF

:_InstallCustomToolReg
REG QUERY HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{977BD01E-F2B4-4341-9C47-459420624A20}\InprocServer32 /v "CodeBase" 1>NUL 2>&1
IF NOT ERRORLEVEL 1 (GOTO:EOF)
CALL:_AddCustomToolReg "%~1"
CALL:_AddRegGenerator "%~1" "{164b10b9-b200-11d0-8c61-00a0c91e29d5}"
CALL:_AddRegGenerator "%~1" "{fae04ec1-301f-11d3-bf4b-00c04f79efbc}"
CALL:_AddRegGenerator "%~1" "{e6fdf8b0-f3d1-11d4-8576-0002a516ece8}"
GOTO:EOF

:_InstallExtenderReg
REG QUERY HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8}\InprocServer32 /v "CodeBase" 1>NUL 2>&1
IF NOT ERRORLEVEL 1 (GOTO:EOF)
CALL:_AddExtenderReg "%~1"
CALL:_AddRegExtender "%~1" "{8D58E6AF-ED4E-48B0-8C7B-C74EF0735451}"
CALL:_AddRegExtender "%~1" "{EA5BD05D-3C72-40A5-95A0-28A2773311CA}"
CALL:_AddRegExtender "%~1" "{E6FDF869-F3D1-11D4-8576-0002A516ECE8}"
GOTO:EOF

:_AddCustomToolReg
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{977BD01E-F2B4-4341-9C47-459420624A20} /f /ve /d "Neumont.Tools.ORM.ORMCustomTool.ORMCustomTool"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{977BD01E-F2B4-4341-9C47-459420624A20}\InprocServer32 /f /ve /d "%SystemRoot%\System32\mscoree.dll"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{977BD01E-F2B4-4341-9C47-459420624A20}\InprocServer32 /f /v "ThreadingModel" /d "Both"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{977BD01E-F2B4-4341-9C47-459420624A20}\InprocServer32 /f /v "Class" /d "Neumont.Tools.ORM.ORMCustomTool.ORMCustomTool"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{977BD01E-F2B4-4341-9C47-459420624A20}\InprocServer32 /f /v "CodeBase" /d "%NORMADir%\bin\Neumont.Tools.ORM.ORMCustomTool.dll"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{977BD01E-F2B4-4341-9C47-459420624A20}\InprocServer32 /f /v "Assembly" /d "Neumont.Tools.ORM.ORMCustomTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=957d5b7d5e79e25f"
GOTO:EOF

:_AddExtenderReg
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8} /f /ve /d "Neumont.Tools.ORM.ORMCustomTool.ExtenderProvider"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8}\InprocServer32 /f /ve /d "%SystemRoot%\System32\mscoree.dll"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8}\InprocServer32 /f /v "ThreadingModel" /d "Both"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8}\InprocServer32 /f /v "Class" /d "Neumont.Tools.ORM.ORMCustomTool.ExtenderProvider"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8}\InprocServer32 /f /v "CodeBase" /d "%NORMADir%\bin\Neumont.Tools.ORM.ORMCustomTool.dll"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8}\InprocServer32 /f /v "Assembly" /d "Neumont.Tools.ORM.ORMCustomTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=957d5b7d5e79e25f"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8}\InprocServer32\1.0.0.0 /f /v "Class" /d "Neumont.Tools.ORM.ORMCustomTool.ExtenderProvider"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8}\InprocServer32\1.0.0.0 /f /v "CodeBase" /d "%NORMADir%\bin\Neumont.Tools.ORM.ORMCustomTool.dll"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\CLSID\{6FDCC073-20C2-4435-9B2E-9E70451C81D8}\InprocServer32\1.0.0.0 /f /v "Assembly" /d "Neumont.Tools.ORM.ORMCustomTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=957d5b7d5e79e25f"
GOTO:EOF

:_AddRegGenerator
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\Generators\%~2\ORMCustomTool /f /ve /d "ORM Custom Tool"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\Generators\%~2\ORMCustomTool /f /v "CLSID" /d "{977BD01E-F2B4-4341-9C47-459420624A20}"
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\Generators\%~2\.orm /f /ve /d "ORMCustomTool"
GOTO:EOF

:_AddRegExtender
REG ADD HKLM\SOFTWARE\Microsoft\VisualStudio\%~1\Extenders\%~2\ORMCustomTool /f /ve /d "{6FDCC073-20C2-4435-9B2E-9E70451C81D8}"
GOTO:EOF

:_AddXslORMGenerator
REG DELETE "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /va /f 1>NUL 2>&1
REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "Type" /d "XSLT" 1>NUL
REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "OfficialName" /d "%~1" 1>NUL
REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "DisplayName" /d "%~2" 1>NUL
REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "DisplayDescription" /d "%~3" 1>NUL
REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "FileExtension" /d "%~4" 1>NUL 2>&1
REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "SourceInputFormat" /d "%~5" 1>NUL
REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "ProvidesOutputFormat" /d "%~6" 1>NUL
REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "TransformUri" /d "%~7" 1>NUL
IF NOT "%~8"=="" (REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "CustomTool" /d "%~8") 1>NUL
IF NOT "%~9"=="" (REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "GeneratesSupportFile" /t REG_DWORD /d "%~9") 1>NUL
SHIFT /8
IF NOT "%~9"=="" (REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "Compilable" /t REG_DWORD /d "%~9") 1>NUL
SHIFT /8
IF NOT "%~9"=="" (REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "ReferenceInputFormats" /t REG_MULTI_SZ /d "%~9") 1>NUL
SHIFT /8
IF NOT "%~9"=="" (REG ADD "HKLM\SOFTWARE\Neumont\ORM Architect for Visual Studio\Generators\%~1" /f /v "PrequisiteInputFormats" /t REG_MULTI_SZ /d "%~9") 1>NUL
GOTO:EOF