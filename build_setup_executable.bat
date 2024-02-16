@ECHO OFF
SET "outputPath=.\nsis"
SET "inputPath=.\bin\Debug\net7.0\publish"

COPY /Y .\src\json\save.json %outputPath%
RENAME %outputPath%\save.json backup_save.json
COPY /Y .\src\json\save.json %outputPath%
COPY /Y license.txt %outputPath%
COPY /Y AudioWhisper.ico %outputPath%
COPY /Y %inputPath%\AudioWhisper.exe %outputPath%
COPY /Y %inputPath%\AudioWhisper.dll %outputPath%
COPY /Y %inputPath%\AudioWhisper.deps.json %outputPath%
COPY /Y %inputPath%\AudioWhisper.dll %outputPath%
COPY /Y %inputPath%\AudioWhisper.exe %outputPath%
COPY /Y %inputPath%\AudioWhisper.pdb %outputPath%
COPY /Y %inputPath%\AudioWhisper.runtimeconfig.json %outputPath%
COPY /Y %inputPath%\NAudio.Asio.dll %outputPath%
COPY /Y %inputPath%\NAudio.Core.dll %outputPath%
COPY /Y %inputPath%\NAudio.dll %outputPath%
COPY /Y %inputPath%\NAudio.Midi.dll %outputPath%
COPY /Y %inputPath%\NAudio.Wasapi.dll %outputPath%
COPY /Y %inputPath%\NAudio.WinMM.dll %outputPath%
makensis.exe %outputPath%\script.nsi
IF NOT EXIST ".\build\" MKDIR .\build\
COPY /B /Y .\nsis\AudioWhisper-Setup.exe .\build\
ECHO ^|=================================^|
ECHO ^|"AudioWhisper-Setup.exe" COMPILED^|
ECHO ^|:::::::::::::::::::::::::::::::::^|
ECHO ^|EXECUTABLE CAN BE FOUND IN build\^|
ECHO ^|=================================^|