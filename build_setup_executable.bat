@ECHO OFF
SET "outputPath=.\nsis"
SET "inputPath=.\bin\Release\net7.0\win10-x64\publish"

COPY /Y .\src\json\save.json %outputPath%
RENAME %outputPath%\save.json backup_save.json
COPY /Y .\src\json\save.json %outputPath%
COPY /Y license.txt %outputPath%
COPY /Y AudioWhisper.ico %outputPath%
COPY /Y %inputPath%\AudioWhisper.exe %outputPath%
makensis.exe %outputPath%\script.nsi
IF NOT EXIST ".\build\" MKDIR .\build\
COPY /B /Y .\nsis\AudioWhisper-Setup.exe .\build\
ECHO ^|=================================^|
ECHO ^|"AudioWhisper-Setup.exe" COMPILED^|
ECHO ^|:::::::::::::::::::::::::::::::::^|
ECHO ^|EXECUTABLE CAN BE FOUND IN build\^|
ECHO ^|=================================^|