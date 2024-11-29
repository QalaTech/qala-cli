$toolsDir   = Split-Path -Parent $MyInvocation.MyCommand.Definition
$releaseDir = Join-Path (Split-Path -Parent (Split-Path -Parent $toolsDir)) '.releases\windows\'

# Copy the qala.exe file
$targetPath = Join-Path $env:ChocolateyInstall 'bin\qala.exe'
Copy-Item "$releaseDir\qala.exe" $targetPath -Force

# Add environment variable
$envVariableName = "qala"
[System.Environment]::SetEnvironmentVariable($envVariableName, $targetPath, [System.EnvironmentVariableTarget]::Machine)
Write-Host "Environment variable $envVariableName set to $targetPath"