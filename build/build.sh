#!/bin/bash

# Exit immediately if a command exits with a non-zero status
set -e

# Available build types
AVAILABLE_RIDS=(
  "osx-arm64"
  "osx-x64"
  "linux-x64"
  "linux-arm64"
  "win-x64"
  "win-arm64"
)

# Function to display help
print_help() {
  echo "Usage: ./build.sh <target-rid>"
  echo "Build and package the Qala CLI for the specified runtime identifier (RID)."
  echo
  echo "Available RIDs:"
  for rid in "${AVAILABLE_RIDS[@]}"; do
    echo "  - $rid"
  done
  echo
  echo "Example:"
  echo "  ./build.sh osx-arm64"
}

# Check if help flag is used or no argument is provided
if [[ "$1" == "-h" || "$1" == "--help" || -z "$1" ]]; then
  print_help
  exit 0
fi

# Check if the provided RID is valid
TARGET_RID="$1"
if [[ ! " ${AVAILABLE_RIDS[@]} " =~ " ${TARGET_RID} " ]]; then
  echo "Error: Invalid target RID '${TARGET_RID}'"
  echo "Run './build.sh -h' to see available build types."
  exit 1
fi

# Variables
PROJECT_NAME="Qala.Cli"          # Replace with your project name
OUTPUT_NAME="qala"               # Desired name for the executable
PUBLISH_DIR="publish/$TARGET_RID" # Directory for the published files
RELEASE_DIR="publish/releases"   # Directory for tar files
TAR_FILE="$RELEASE_DIR/qala-$TARGET_RID.tar.gz" # Name of the tar file
VERSION='0.0.2'                  # Replace with your project version

# Create the release directory if it doesn't exist
mkdir -p "$RELEASE_DIR"

# Clean up previous builds
echo "Cleaning up previous builds..."
rm -rf "$PUBLISH_DIR"
rm -f "$TAR_FILE"

# Build and publish the project
echo "Publishing the project for $TARGET_RID..."
dotnet publish src/$PROJECT_NAME/$PROJECT_NAME.csproj \
  -c Release \
  -r $TARGET_RID \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:OutputName=$OUTPUT_NAME \
  -o $PUBLISH_DIR

# Create the tar file
echo "Creating tarball: $TAR_FILE..."
tar -czvf "$TAR_FILE" -C "$PUBLISH_DIR" .

# Output success message
echo "Build and packaging completed successfully!"
echo "Tarball created: $TAR_FILE"

## Start the creation of installers per platform runtime
# For Windows only - creating the msi file
if [[ "$TARGET_RID" == "win-x64" || "$TARGET_RID" == "win-arm64" ]]; then
  echo "Creating the MSI file for Windows..."

  # Copy the wix files and folders to the publish directory
  echo "Copying WiX files to the publish directory..."
  cp -r build/wix/ $PUBLISH_DIR
  echo "WiX files copied to the publish directory."

  # Go to the WiX folder within the publish directory
  cd $PUBLISH_DIR/wix

  # Validate if WiX is installed
  if wix --version > /dev/null 2>&1; then
    echo "WiX is already installed."
  else
    # Install WiX
    echo "Installing WiX Toolset..."
    dotnet tool install --global wix

    # Validate WiX installation
    echo "Validating WiX installation..."
    if wix --version > /dev/null 2>&1; then
      echo "WiX installed successfully."
    else
      echo "Error: WiX installation failed."
      exit 1
    fi
  fi

  # Install the extension WixToolset.UI.wixext
  echo "Installing the WiX extension WixToolset.UI.wixext..."
  wix extension add WixToolset.UI.wixext
  echo "WiX extension WixToolset.UI.wixext installed successfully."

  # Generating the MSI file
  echo "Generating the MSI file..."
  wix build qala.wxs -d Verion=$VERSION -ext WixToolset.UI.wixext -o QalaCliInstaller-$TARGET_RID.msi

  # Validate the MSI file
  echo "Validating the MSI file..."
  if [ -f QalaCliInstaller-$TARGET_RID.msi ]; then
    echo "MSI file generated successfully."
  else
    echo "Error: MSI file generation failed."
    exit 1
  fi
fi
