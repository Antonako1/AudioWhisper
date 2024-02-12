; Script to run as administrator, execute PowerShell, and extract files

unicode True

!include LogicLib.nsh
!define WM_WININICHANGE 0x001A
!define HWND_BROADCAST 0xFFFF
!define WM_SETTINGCHANGE 0x001A

Outfile "Hermes-Setup.exe"
RequestExecutionLevel admin

BrandingText /TRIMCENTER "Hermes 1.0.0 Setup"
Name "Hermes 1.0.0 Setup"
ManifestSupportedOS Win10
DetailsButtonText "Show progress"

######### LICENSE ############
PageEx license
    LicenseText "Readme"
    LicenseData license.txt
    LicenseForceSelection checkbox
PageExEnd

########### COMPONENTS #####################

InstType "Full" IT_FULL
InstType "Minimal" IT_MIN

PageEx components
    ComponentText "Choose files you want to install." \
    #"About" \
    #"Main program contains all necessary components for basic functioning. \
    #Additional components contain all extra files, mainly used by the context menu."
PageExEnd

SectionGroup "!Main Program" RO
    Section !Hermes.exe sec1_id
        DetailPrint "These files go to your TEMP folder. These will be deleted after setup."
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "Hermes.exe"
    SectionEnd
    Section !license.txt sec7_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        File "license.txt"
    SectionEnd
    Section !Hermes.dll sec2_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "Hermes.dll"
    SectionEnd

    Section !Hermes.deps.json sec10_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "Hermes.deps.json"
    SectionEnd
    Section !Hermes.runtimeconfig.json sec11_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "Hermes.runtimeconfig.json"
    SectionEnd
    Section !NAudio.Asio.dll sec12_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "NAudio.Asio.dll"
    SectionEnd
    Section !NAudio.Core.dll sec13_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "NAudio.Core.dll"
    SectionEnd
    Section !NAudio.dll sec14_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "NAudio.dll"
    SectionEnd
    Section !NAudio.Midi.dll sec15_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "NAudio.Midi.dll"
    SectionEnd
    Section !NAudio.Wasapi.dll sec16_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "NAudio.Wasapi.dll"
    SectionEnd
    Section !NAudio.WinMM.dll sec17_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "NAudio.WinMM.dll"
    SectionEnd
    Section !Hermes.ico sec18_id
        SectionInstType ${IT_FULL} ${IT_MIN} RO
        SectionIn 1 ${sec1_id}
        SetOutPath "$TEMP\temp_hermes_mic_playback"
        File "Hermes.ico"
    SectionEnd
SectionGroupEnd


############ DIRECTORY ######################
Var INSTALL_DIR

PageEx directory
    DirVar $INSTALL_DIR
    DirVerify leave
PageExEnd

Function .onVerifyInstDir
    Var /GLOBAL ext
    StrCpy $ext "Hermes"
    StrCpy $INSTALL_DIR "$INSTALL_DIR$ext"
    ; Checks if folder already exists
    Call CheckFolder
FunctionEnd


; Checks if the folder exists, if it exists and user wants to delete
; it and it's contents the script will continue
Function CheckFolder
DetailPrint "Checking folder."
${If}  ${FileExists} $INSTALL_DIR
    ; Delete it
    MessageBox MB_YESNO|MB_ICONQUESTION `"$INSTALL_DIR" already exists, delete its contents and continue installing?` IDYES agree
    Abort "Setup aborted by user."
agree:
    DetailPrint 'Removing "$INSTALL_DIR" and its contents.'
    RMDir /r $INSTALL_DIR
${EndIf}
FunctionEnd

##########INSTFILE"######################
PageEx instfiles
PageExEnd

############## INIT ######################
; Set the default installation directory
Function .onInit
    InitPluginsDir
    StrCpy $INSTALL_DIR $PROGRAMFILES64\Hermes
FunctionEnd

############################## START ##############################
Section 
; Delete the $TEMP folder stuff before extracting more files
; and taking up more space from the disk
Delete "$TEMP\temp_hermes_mic_playback\Hermes.exe"
Delete "$TEMP\temp_hermes_mic_playback\Hermes.dll"
Delete "$TEMP\temp_hermes_mic_playback\license.txt"
Delete "$TEMP\temp_hermes_mic_playback\Uninstall.exe"
Delete "$TEMP\temp_hermes_mic_playback\Setup.ps1"
Delete "$TEMP\temp_hermes_mic_playback\Uninstall.ps1"
Delete "$TEMP\temp_hermes_mic_playback\Hermes.deps.json"
Delete "$TEMP\temp_hermes_mic_playback\Hermes.runtimeconfig.json"
Delete "$TEMP\temp_hermes_mic_playback\NAudio.Asio.dll"
Delete "$TEMP\temp_hermes_mic_playback\NAudio.Core.dll"
Delete "$TEMP\temp_hermes_mic_playback\NAudio.dll"
Delete "$TEMP\temp_hermes_mic_playback\NAudio.Midi.dll"
Delete "$TEMP\temp_hermes_mic_playback\NAudio.Wasapi.dll"
Delete "$TEMP\temp_hermes_mic_playback\NAudio.WinMM.dll"
Delete "$TEMP\temp_hermes_mic_playback\Hermes.ico"

; The folder will be deleted in the Uninstall section
RMDir /r $TEMP\temp_hermes_mic_playback

DetailPrint 'Files from "$TEMP\temp_hermes_mic_playback" deleted. beginning setup.' 

; After deletion begin with setup

; Check the folder
Call CheckFolder

; Set the installation directory
SetOutPath $INSTALL_DIR

; Create the installation directory if it doesn't exist
CreateDirectory $INSTALL_DIR

SectionEnd


############# SETUP ################
Section 

runSetup:

File "Setup.ps1"

; Execute Setup.ps1 script
nsExec::ExecToLog 'Powershell.exe -ExecutionPolicy Bypass -File "$INSTALL_DIR\Setup.ps1" "$INSTALL_DIR" "no"'



; Check if setup ran succesfully
Pop $0
${If} $0 == "0"
    DetailPrint "Setup sequence completed with success."
${Else}
    DetailPrint "Failed to run setup."
    MessageBox MB_ABORTRETRYIGNORE|MB_ICONEXCLAMATION "Error running setup. Retry by pressing 'Retry', \
    ignore the error and continue by pressing 'Ignore', or close the program by pressing 'Abort'." IDABORT abortM IDIGNORE ignoreM
        ; Run setup again
        DetailPrint "Running setup again."
        ; After executing, delete it
        Delete "Setup.ps1"
        Goto runSetup
    ignoreM:
        ; Continue setup
        DetailPrint "Continuing setup."
        ; After executing, delete it
        Delete "Setup.ps1"
        Goto Continue
    abortM:
        ; Abort
        ; After executing, delete it
        Delete "Setup.ps1"
        Abort "Setup aborted by user."
${EndIf}
Continue:
; After executing, delete it
Delete "Setup.ps1"

########## EXTRACTION ##########
; Extract files based on section selection

; Extract uninstallation script
File "Uninstall.ps1"

; Set the file attributes to hidden
System::Call 'kernel32::SetFileAttributes(t "$INSTALL_DIR\Uninstall.ps1", i 2) i .r0'

; Check if the SetFileAttributes call succeeded
${If} $0 != 0
    DetailPrint "Uninstall.ps1 file attributes set to hidden."
    DetailPrint 'Uninstall.ps1 location: "$INSTALL_DIR\Uninstall.ps1".' 
${Else}
    DetailPrint "Failed to set Uninstall.ps1 file attributes to hidden."
${EndIf}

########## MAIN PROGRAM ##########
${If} ${SectionIsSelected} ${sec1_id}
    File "Hermes.exe"
${EndIf}
${If} ${SectionIsSelected} ${sec2_id}
    File "Hermes.dll"
${EndIf}

${If} ${SectionIsSelected} ${sec7_id}
    File "license.txt"
${EndIf}

${If} ${SectionIsSelected} ${sec10_id}
    File "Hermes.deps.json"
${EndIf}
${If} ${SectionIsSelected} ${sec11_id}
    File "Hermes.runtimeconfig.json"
${EndIf}
${If} ${SectionIsSelected} ${sec12_id}
    File "NAudio.Asio.dll"
${EndIf}
${If} ${SectionIsSelected} ${sec13_id}
    File "NAudio.Core.dll"
${EndIf}
${If} ${SectionIsSelected} ${sec14_id}
    File "NAudio.dll"
${EndIf}
${If} ${SectionIsSelected} ${sec15_id}
    File "NAudio.Midi.dll"
${EndIf}
${If} ${SectionIsSelected} ${sec16_id}
    File "NAudio.Wasapi.dll"
${EndIf}
${If} ${SectionIsSelected} ${sec17_id}
    File "NAudio.WinMM.dll"
${EndIf}
${If} ${SectionIsSelected} ${sec18_id}
    File "Hermes.ico"
${EndIf}

# Create shortcut on DESKTOP
CreateShortcut "$DESKTOP\Hermes.lnk" "$INSTALL_DIR\Hermes.exe" "" "$INSTALL_DIR\Hermes.ico"

; Create an uninstaller in the same directory as the installer
WriteUninstaller "$INSTALL_DIR\Uninstall.exe"
DetailPrint 'You can now close this windows by pressing "Close".' 

SectionEnd

########## UNINSTALL ##########


UninstPage uninstConfirm
UninstPage instfiles

# Call must be used with function names starting with "un." in the uninstall section.
;Function unRefresh
;    ; Refreshes icons
;    System::Call 'shell32.dll::SHChangeNotify(i, i, i, i) i(0x8000000, 0, 0, 0)'
;
;    ; Refreshes Environmental variables
;    System::Call 'user32::SendMessage(i ${HWND_BROADCAST}, i ${WM_WININICHANGE}, i 0, t "Environment")'
;FunctionEnd

Section "Uninstall"
; Execute the PowerShell script with elevated privileges and pass the parameters
nsExec::ExecToLog 'Powershell.exe -ExecutionPolicy Bypass -File "$INSTDIR\Uninstall.ps1" "$INSTDIR"'

########################### DELETE FILES ###########################
; Remove installed files during uninstallation

Delete "$INSTDIR\Hermes.exe"
Delete "$INSTDIR\Hermes.dll"
Delete "$INSTDIR\Uninstall.exe"
Delete "$INSTDIR\license.txt"
Delete "$INSTDIR\Uninstall.ps1"
Delete "$INSTDIR\Setup.ps1"
Delete "$INSTDIR\Hermes.deps.json"
Delete "$INSTDIR\Hermes.runtimeconfig.json"
Delete "$INSTDIR\NAudio.Asio.dll"
Delete "$INSTDIR\NAudio.Core.dll"
Delete "$INSTDIR\NAudio.dll"
Delete "$INSTDIR\NAudio.Midi.dll"
Delete "$INSTDIR\NAudio.Wasapi.dll"
Delete "$INSTDIR\NAudio.WinMM.dll"
Delete "$INSTDIR\Hermes.ico"
Delete "$DESKTOP\Hermes.lnk"

; Remove the installation directory and TEMP directory if it still exists
RMDir /r $INSTDIR
RMDir /r $TEMP\temp_hermes_mic_playback

; Lastly refresh icons and env
;Call unRefresh

SectionEnd