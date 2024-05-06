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
 - F7 = Toggle playback while app is out of focus
 - Q = Exit program

 --- 
 
### Building:

**Executable's directory must have save.json and backup_save.json to work**
 - Standalone executable: 
   - ``` dotnet build ```
   - ``` dotnet publish ```
   - ``` run.bat ```

**Running either script will move all necessary files to \nsis**
 - Installer (requires [nsis](https://nsis.sourceforge.io/Download)): 
   - ``` zero_build.bat ```
   - ``` build_setup_executable.bat (run "dotnet publish" before running)  ```
  
 ---