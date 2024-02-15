dotnet build
COPY /Y .\src\json\save.json .\bin\Debug\net7.0\
RENAME .\bin\Debug\net7.0\save.json backup_save.json 
COPY /Y .\src\json\save.json .\bin\Debug\net7.0\
RENAME .\bin\Debug\net7.0\save.json old_save.json 
COPY /Y .\src\json\save.json .\bin\Debug\net7.0\
.\bin\Debug\net7.0\AudioWhisper.exe