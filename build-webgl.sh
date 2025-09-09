#!/bin/bash

# WebGL Build Script for Unity
# This script builds the Unity project for WebGL deployment

echo "Starting WebGL build..."

# Path to Unity (adjust if your Unity installation is different)
UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.25f1/Unity.app/Contents/MacOS/Unity"

# Alternative common Unity paths (uncomment the one that matches your installation)
# UNITY_PATH="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
# UNITY_PATH="/Applications/Unity Hub/Editor/2023.1.0f1/Unity.app/Contents/MacOS/Unity"

# Project path
PROJECT_PATH="$(pwd)"

# Build output path
BUILD_PATH="$PROJECT_PATH/WebGL-Build"

# Check if Unity exists
if [ ! -f "$UNITY_PATH" ]; then
    echo "Unity not found at $UNITY_PATH"
    echo "Please update the UNITY_PATH variable in this script to match your Unity installation"
    echo ""
    echo "Common Unity paths:"
    echo "  /Applications/Unity/Hub/Editor/[VERSION]/Unity.app/Contents/MacOS/Unity"
    echo "  /Applications/Unity/Unity.app/Contents/MacOS/Unity"
    exit 1
fi

# Create build directory
mkdir -p "$BUILD_PATH"

echo "Building project at: $PROJECT_PATH"
echo "Output directory: $BUILD_PATH"
echo "Using Unity at: $UNITY_PATH"

# Build the project
"$UNITY_PATH" \
    -batchmode \
    -quit \
    -projectPath "$PROJECT_PATH" \
    -buildTarget WebGL \
    -buildPath "$BUILD_PATH" \
    -logFile "$PROJECT_PATH/build.log"

# Check if build was successful
if [ $? -eq 0 ]; then
    echo ""
    echo "✅ WebGL build completed successfully!"
    echo "📁 Build files are in: $BUILD_PATH"
    echo ""
    echo "Next steps:"
    echo "1. Test the build by opening $BUILD_PATH/index.html in a web browser"
    echo "2. Deploy to Vercel: cd WebGL-Build && vercel --prod"
    echo "3. Or deploy to other platforms like itch.io or Netlify"
else
    echo ""
    echo "❌ Build failed. Check build.log for details:"
    echo "tail -n 50 $PROJECT_PATH/build.log"
fi