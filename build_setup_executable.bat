@ECHO OFF
set "op=.\nsis"
set "outputPath=.\bin\Debug\net7.0\publish"
echo %outputPath%\Hermes.exe %op%
COPY /A /Y license.txt %op%
COPY /B /Y Hermes.ico %op%
COPY /B /Y %outputPath%\Hermes.exe %op%
COPY /B /Y %outputPath%\Hermes.dll %op%
COPY /A /Y %outputPath%\Hermes.deps.json %op%
COPY /B /Y %outputPath%\Hermes.dll %op%
COPY /B /Y %outputPath%\Hermes.exe %op%
COPY /B /Y %outputPath%\Hermes.pdb %op%
COPY /A /Y %outputPath%\Hermes.runtimeconfig.json %op%
COPY /B /Y %outputPath%\NAudio.Asio.dll %op%
COPY /B /Y %outputPath%\NAudio.Core.dll %op%
COPY /B /Y %outputPath%\NAudio.dll %op%
COPY /B /Y %outputPath%\NAudio.Midi.dll %op%
COPY /B /Y %outputPath%\NAudio.Wasapi.dll %op%
COPY /B /Y %outputPath%\NAudio.WinMM.dll %op%
makensis.exe %op%\script.nsi
if not exist ".\build\" mkdir .\build\
COPY .\nsis\Hermes-Setup.exe .\build\
