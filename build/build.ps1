# Exit immediately if a command exits with a non-zero status
$ErrorActionPreference = "Stop"

# Available build types
$AVAILABLE_RIDS = @(
  "osx-arm64",
  "osx-x64",
  "linux-x64",
  "linux-arm64",
  "win-x64",
  "win-arm64"
)

# Function to display help
function print_help {
  Write-Host "Usage: .\build.ps1 <target-rid>"
  Write-Host "Build and package the Qala CLI for the specified runtime identifier (RID)."
  Write-Host
  Write-Host "Available RIDs:"
  foreach ($rid in $AVAILABLE_RIDS) {
    Write-Host "  - $rid"
  }
  Write-Host
  Write-Host "Example:"
  Write-Host "  .\build.ps1 osx-arm64"
}

# Check if help flag is used or no argument is provided
if ($args[0] -eq "-h" -or $args[0] -eq "--help" -or -not $args[0]) {
  print_help
  exit 0
}

# Check if the provided RID is valid
$TARGET_RID = $args[0]
if ($AVAILABLE_RIDS -notcontains $TARGET_RID) {
  Write-Host "Error: Invalid target RID '$TARGET_RID'"
  Write-Host "Run '.\build.ps1 -h' to see available build types."
  exit 1
}

# Variables
$PROJECT_NAME = "Qala.Cli"          # Replace with your project name
$OUTPUT_NAME = "qala"               # Desired name for the executable
$PUBLISH_DIR = "publish/$TARGET_RID" # Directory for the published files
$RELEASE_DIR = "publish/releases"   # Directory for tar files
$TAR_FILE = "$RELEASE_DIR/qala-$TARGET_RID.tar.gz" # Name of the tar file
$VERSION = '0.0.4'

# Create the release directory if it doesn't exist
if (-not (Test-Path -Path $RELEASE_DIR)) {
  New-Item -ItemType Directory -Path $RELEASE_DIR
}

# Clean up previous builds
Write-Host "Cleaning up previous builds..."
Remove-Item -Recurse -Force -Path $PUBLISH_DIR -ErrorAction SilentlyContinue
Remove-Item -Force -Path $TAR_FILE -ErrorAction SilentlyContinue

# Build and publish the project
Write-Host "Publishing the project for $TARGET_RID..."
dotnet publish "src/$PROJECT_NAME/$PROJECT_NAME.csproj" `
  -c Release `
  -r $TARGET_RID `
  --self-contained `
  -p:PublishSingleFile=true `
  -p:OutputName=$OUTPUT_NAME `
  -o $PUBLISH_DIR

# Create the tar file
Write-Host "Creating tarball: $TAR_FILE..."
tar -czvf $TAR_FILE -C $PUBLISH_DIR .

# Output success message
Write-Host "Build and packaging completed successfully!"
Write-Host "Tarball created: $TAR_FILE"

## Start the creation of installers per platform runtime
# For Windows only - creating the msi file
if ($TARGET_RID -eq "win-x64" -or $TARGET_RID -eq "win-arm64") {
  Write-Host "Creating the MSI file for Windows..."

  # Copy the wix files and folders to the publish directory
  Write-Host "Copying WiX files to the publish directory..."
  Copy-Item -Recurse -Path "build/wix/" -Destination $PUBLISH_DIR
  Write-Host "WiX files copied to the publish directory."

  # Go to the WiX folder within the publish directory
  Set-Location "$PUBLISH_DIR/wix"

  # Validate if WiX is installed
  if (Get-Command wix -ErrorAction SilentlyContinue) {
    Write-Host "WiX is already installed."
  } else {
    # Install WiX
    Write-Host "Installing WiX Toolset..."
    dotnet tool install --global wix

    # Validate WiX installation
    Write-Host "Validating WiX installation..."
    if (Get-Command wix -ErrorAction SilentlyContinue) {
      Write-Host "WiX installed successfully."
    } else {
      Write-Host "Error: WiX installation failed."
      exit 1
    }
  }

  # Install the extension WixToolset.UI.wixext
  Write-Host "Installing the WiX extension WixToolset.UI.wixext..."
  wix extension add WixToolset.UI.wixext
  Write-Host "WiX extension WixToolset.UI.wixext installed successfully."

  # Generating the MSI file
  Write-Host "Generating the MSI file..."
  wix build qala.wxs -d Version=$VERSION -ext WixToolset.UI.wixext -o "QalaCliInstaller-$TARGET_RID.msi"

  # Validate the MSI file
  Write-Host "Validating the MSI file..."
  if (Test-Path -Path "QalaCliInstaller-$TARGET_RID.msi") {
    Write-Host "MSI file generated successfully."
  } else {
    Write-Host "Error: MSI file generation failed."
    exit 1
  }
}
