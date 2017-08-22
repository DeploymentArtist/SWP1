$OrgName = 'ViaMonstra'
$SiteName = 'New York Site'

Import-Module –Name 'D:\Setup\MDTDB\MDTDB.psm1'
Connect-MDTDatabase -sqlServer CM01 -database MDT

$Computers = Import-Csv 'D:\Setup\Scripts\Computers.txt'

For ($i=1; $i -le $Computers.count; $i++)

{

$Description = "$SiteName - " + $Computers[$i-1].OSDComputerName

New-MDTComputer -macAddress $Computers[$i-1].MACAddress -description $Description -settings @{
 OSDComputerName=$Computers[$i-1].OSDComputerName;
 OrgName=$OrgName

}

}

$ComputerCount = $Computers.count
Write-Output "$ComputerCount computers imported"