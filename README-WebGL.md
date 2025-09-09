# WebGL Build & Deployment Guide

## Building for WebGL

### Option 1: Using the Build Script (Recommended)
```bash
./build-webgl.sh
```

### Option 2: Manual Unity Build
1. Open Unity
2. File → Build Settings
3. Select "WebGL" platform
4. Click "Switch Platform" if not already selected
5. Click "Build" and choose the `WebGL-Build` folder

## Testing Locally
After building, you can test locally:
```bash
cd WebGL-Build
python3 -m http.server 8000
```
Then open `http://localhost:8000` in your browser.

## Deploying to Vercel
1. Build the WebGL project first
2. Navigate to the build folder:
   ```bash
   cd WebGL-Build
   ```
3. Deploy:
   ```bash
   vercel --prod
   ```

## Alternative Deployment Options

### itch.io (Recommended for games)
1. Zip the contents of `WebGL-Build` folder
2. Upload to itch.io as HTML5 game

### Netlify
1. Drag and drop the `WebGL-Build` folder to Netlify

### GitHub Pages
1. Push the `WebGL-Build` contents to a GitHub repo
2. Enable GitHub Pages in repo settings

## Troubleshooting

### Unity Path Issues
If the build script can't find Unity, update the `UNITY_PATH` in `build-webgl.sh`:
```bash
# Common Unity paths:
/Applications/Unity/Hub/Editor/2022.3.25f1/Unity.app/Contents/MacOS/Unity
/Applications/Unity/Unity.app/Contents/MacOS/Unity
```

### CORS Issues
The included `vercel.json` handles CORS headers required for Unity WebGL.

### Build Errors
Check the build log:
```bash
tail -n 50 build.log
```