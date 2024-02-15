# Check if the script is running with administrator privileges
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-not $isAdmin) {
    Write-Host "You need to run this script as an administrator."
    Write-Host "Please right-click on the script and select 'Run as administrator.'"
    pause
    exit 1
}
# Argument got from .bat
# Contains the directory it was called from
$callDir = $args[0]



# Windows PATH Environment Variable Setup
#
# ---------------------------------------------------------------------
# Add the current directory to the PATH environment variable
# Check if folder $callDir is already in the path
$pathDir = "$callDir"

Write-Host Adding current path to System Environmental Variables.
if($Env:PATH -notlike "*$pathDir*"){
    # TRUE, ADD TO PATH


    # Make oldPath and newPath variables
    $oldpath = (Get-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH).path
    $newpath = "$oldpath;$pathDir"
    
    Set-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH -Value $newpath

    Write-Host Added current path to System Environmental Variables.
} else {
	# FALSE DON'T ADD
    Write-Host Current path is already found in System Environmental Variables.
}

Write-Host "Setup succesfull."