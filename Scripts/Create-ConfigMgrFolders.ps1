# Check for elevation
If (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(`
    [Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    Write-Warning "Oupps, you need to run this script from an elevated PowerShell prompt!`nPlease start the PowerShell prompt as an Administrator and re-run the script."
	Write-Warning "Aborting script..."
    Break
}

New-Item -Path D:\MigData -ItemType Directory
New-Item -Path D:\Logs -ItemType Directory
New-Item -Path D:\Sources -ItemType Directory
New-Item -Path D:\Sources\OSD -ItemType Directory
New-Item -Path D:\Sources\OSD\Boot -ItemType Directory
New-Item -Path D:\Sources\OSD\DriverPackages -ItemType Directory
New-Item -Path D:\Sources\OSD\DriverSources -ItemType Directory
New-Item -Path D:\Sources\OSD\MDT -ItemType Directory
New-Item -Path D:\Sources\OSD\OS -ItemType Directory
New-Item -Path D:\Sources\OSD\Settings -ItemType Directory
New-Item -Path D:\Sources\Software -ItemType Directory
New-Item -Path D:\Sources\Software\Adobe -ItemType Directory
New-Item -Path D:\Sources\Software\Microsoft -ItemType Directory
New-Item -Path D:\Sources\Software\Utilities -ItemType Directory

New-SmbShare –Name Logs$ –Path D:\Logs -ChangeAccess EVERYONE
icacls D:\Logs /grant '"VIAMONSTRA\CM_NAA":(OI)(CI)(M)'

New-SmbShare –Name Sources –Path D:\Sources -FullAccess EVERYONE
