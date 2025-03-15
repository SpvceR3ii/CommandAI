#!/bin/bash

# Check if old installation exists and remove it
if dotnet tool list --global | grep -q "commandai"; then
    echo "Removing old CommandAI installation..."
    dotnet tool uninstall --global CommandAI
fi

# Create config directory and file if they don't exist
CONFIG_DIR="$HOME/.config/commandai"
CONFIG_FILE="$CONFIG_DIR/config.json"

if [ ! -d "$CONFIG_DIR" ]; then
    mkdir -p "$CONFIG_DIR"
fi

if [ ! -f "$CONFIG_FILE" ]; then
    cat > "$CONFIG_FILE" << EOL
{
    "model": "qwen2.5-coder:3b",
    "endpoint": "http://localhost:11434/api/generate",
    "timeout": 300,
    "exclude_patterns": []
}
EOL
fi

# Build and package as a .NET tool
dotnet build --configuration Release
dotnet pack --configuration Release

# Install globally
dotnet tool install --global --add-source ./nupkg CommandAI

# Add to PATH if not already present
if ! grep -q "$HOME/.dotnet/tools" "$HOME/.bashrc"; then
    echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> "$HOME/.bashrc"
fi

echo "Installation complete! You can now use 'cmai' command from anywhere."
echo "Configuration file located at: $CONFIG_FILE"
echo "Please restart your terminal or run 'source ~/.bashrc' to use the command."