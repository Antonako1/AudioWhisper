# Check if the script is running with administrator privileges
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-not $isAdmin) {
    Write-Host "You need to run this script as an administrator."
    Write-Host "Please right-click on the script and select 'Run as administrator.'"
    pause
    exit 1
}
# Argument got from run_setup.bat
# Contains the directory it was called from
$callDir = $args[0]

# Windows PATH Environment Variable Setup
#
# ---------------------------------------------------------------------
# Remove the current directory to the PATH environment variable
# Check if folder $callDir is already in the path
# ! Adds path to build folder
$pathDir = "$callDir"
Write-Host Removing path from System Environment Variables.
if($Env:PATH -like "*$pathDir*"){
    # TRUE, REMOVE FROM PATH


    # Make oldPath and newPath variables    
    $oldpath = (Get-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH).path

    # Split elements in to an array
    $arrpath = $oldpath -split ";"

    # Filter out the path
    $arrpath = $arrpath | Where-Object { $_ -ne $pathDir }

    # Remove pathDir from Variables
    $newpath = $arrpath -join ';'
    
    Set-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH -Value $newpath

    Write-Host Removed current path from System Environmental Variables.
} else {
	# FALSE DON'T REMOVE
    Write-Host Current path is already removed from System Environment Variables.
}

Write-Host "Uninstallation succesfull."