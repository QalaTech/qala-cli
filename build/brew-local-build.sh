#!/bin/bash

# Exit immediately if a command exits with a non-zero status
set -e

# Check if the target architecture was passed as an argument
if [ -z "$1" ]; then
  echo "Warning: No target architecture specified so arm64 is being used as default."
  TARGET_ARCH="arm64"  # Set the default architecture to arm64
else
  TARGET_ARCH=$1  # Use the passed argument as the target architecture
fi

# Define variables for the tap and formula
TAP_USER="qalatech"          # GitHub username
TAP_REPO="qala"              # GitHub repo for the tap
TAP_NAME="$TAP_USER/$TAP_REPO"  # Complete tap name in Homebrew format (user/repo)
FORMULA="qala"
LOCAL_TAP_DIR="$HOME/homebrew-local/qala"  # Local directory for Homebrew tap

# Define paths for the formula and tarball
SOURCE_FORMULA="./build/homebrew/Formulas/qala-local.rb"
DEST_DIR="$HOME/homebrew-local/qala/Formula"
DEST_FORMULA="$DEST_DIR/qala.rb"
TARBALL="./publish/releases/qala-osx-$TARGET_ARCH.tar.gz"
UPDATED_URL="file://$PWD/publish/releases/qala-osx-$TARGET_ARCH.tar.gz"

# Check if the source formula file exists
if [[ ! -f "$SOURCE_FORMULA" ]]; then
  echo "Error: Source formula '$SOURCE_FORMULA' does not exist."
  exit 1
fi

# Check if the tarball exists for the specified architecture
if [[ ! -f "$TARBALL" ]]; then
  echo "Error: Tarball '$TARBALL' does not exist for architecture '$TARGET_ARCH'."
  exit 1
fi

# Delete the local tap directory if it already exists
if [ -d "$LOCAL_TAP_DIR" ]; then
  echo "Deleting existing local tap directory '$LOCAL_TAP_DIR'..."
  rm -rf "$LOCAL_TAP_DIR"
fi

# Recreate the tap directory
echo "Recreating local tap directory '$LOCAL_TAP_DIR'..."
mkdir -p "$LOCAL_TAP_DIR"

# Calculate the SHA-256 hash of the tarball
echo "Calculating SHA-256 hash for '$TARBALL'..."
SHA256=$(shasum -a 256 "$TARBALL" | awk '{print $1}')

# Create the destination directory if it doesn't exist
echo "Creating destination directory: $DEST_DIR"
mkdir -p "$DEST_DIR"

# Copy the formula to the destination directory
echo "Copying '$SOURCE_FORMULA' to '$DEST_FORMULA'"
cp "$SOURCE_FORMULA" "$DEST_FORMULA"

# Update the formula file with the new tarball URL and SHA-256 hash
echo "Updating the formula with the new tarball URL and SHA-256 hash..."
sed -i '' "s|url \".*\"|url \"$UPDATED_URL\"|g" "$DEST_FORMULA"
sed -i '' "s|sha256 \".*\"|sha256 \"$SHA256\"|g" "$DEST_FORMULA"

# Output success message for formula update
echo "Formula updated successfully!"
echo "Copied to: $DEST_FORMULA"
echo "Updated URL: $UPDATED_URL"
echo "Updated SHA-256: $SHA256"


# Initialize the local tap directory as a git repository
echo "Initializing local tap directory as a Git repository..."
cd "$LOCAL_TAP_DIR"
git init
git add .
git commit -m "Initial commit for Homebrew tap"

# Uninstall the formula if it is already installed
if brew list --formula | grep -q "^$FORMULA\$"; then
  echo "Uninstalling existing formula '$FORMULA'..."
  brew uninstall "$FORMULA"
else
  echo "Formula '$FORMULA' is not installed."
fi

# Untap the tap if it exists
if brew tap | grep -q "^$TAP_NAME\$"; then
  echo "Untapping existing tap '$TAP_NAME'..."
  brew untap "$TAP_NAME"
else
  echo "Tap '$TAP_NAME' is not currently tapped."
fi

# Clean up Homebrew cache and symlinks
echo "Cleaning up Homebrew..."
brew cleanup --prune=all
brew tap --repair

# Tap the local tap from the GitHub repository
echo "Tapping GitHub tap '$TAP_NAME'..."
brew tap "$TAP_NAME" "$LOCAL_TAP_DIR"

# Install the formula
echo "Installing formula '$FORMULA'..."
brew install "$TAP_NAME/$FORMULA"

# Verify installation
if brew list --formula | grep -q "^$FORMULA\$"; then
  echo "Formula '$FORMULA' installed successfully!"
else
  echo "Failed to install formula '$FORMULA'."
  exit 1
fi
