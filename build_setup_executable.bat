@ECHO OFF
SET "op=.\nsis"
SET "outputPath=.\bin\Debug\net7.0\publish"
ECHO %outputPath%\AudioWhisper.exe %op%
COPY /Y .\src\json\save.json %op%
RENAME %op%\save.json backup_save.json
COPY /Y .\src\json\save.json %op%
COPY /Y license.txt %op%
COPY /B /Y AudioWhisper.ico %op%
COPY /B /Y %outputPath%\AudioWhisper.exe %op%
COPY /B /Y %outputPath%\AudioWhisper.dll %op%
COPY /Y %outputPath%\AudioWhisper.deps.json %op%
COPY /B /Y %outputPath%\AudioWhisper.dll %op%
COPY /B /Y %outputPath%\AudioWhisper.exe %op%
COPY /B /Y %outputPath%\AudioWhisper.pdb %op%
COPY /Y %outputPath%\AudioWhisper.runtimeconfig.json %op%
COPY /B /Y %outputPath%\NAudio.Asio.dll %op%
COPY /B /Y %outputPath%\NAudio.Core.dll %op%
COPY /B /Y %outputPath%\NAudio.dll %op%
COPY /B /Y %outputPath%\NAudio.Midi.dll %op%
COPY /B /Y %outputPath%\NAudio.Wasapi.dll %op%
COPY /B /Y %outputPath%\NAudio.WinMM.dll %op%
makensis.exe %op%\script.nsi
IF NOT EXIST ".\build\" MKDIR .\build\
COPY /B /Y .\nsis\AudioWhisper-Setup.exe .\build\
ECHO EXECUTABLE CAN BE FOUND IN build\