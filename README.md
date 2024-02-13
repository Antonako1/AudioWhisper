# Microphone audio replayer

### Replays microphone's audio in real time

---

### Controls:
 - 1 = Start microphone playback
 - 2 = Stop microphone playback
 - 3 = Edit settings
   - 1 = Edit values
   - 2 = Help message
   - 3 = Return
 - 4 = Exit program

 --- 
 
### Builds:

**save.json from src\json\ is required**

 - Standalone executable: 
   - ``` dotnet build ```
   - ``` dotnet publish ```
   - ``` run.bat ```

 - Installer (requires [nsis](https://nsis.sourceforge.io/Download)): 
   - ``` zero_build.bat ```
   - ``` build_setup_executable.bat (use dotnet publish)  ```
  
 ---