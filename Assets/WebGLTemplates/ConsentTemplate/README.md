# Consent Form WebGL Template

This custom Unity WebGL template displays a research study consent form before allowing users to start the game.

## Features

- Professional consent form with research study information
- Scrollable content area for lengthy terms
- Required checkbox that must be checked before starting
- Consent timestamp recording
- Smooth transition to Unity game after consent
- Responsive design for different screen sizes

## Setup Instructions

### 1. In Unity Editor

1. Open **Edit > Project Settings > Player**
2. Select the **WebGL** platform tab
3. Under **Resolution and Presentation**, find the **WebGL Template** dropdown
4. Select **ConsentTemplate**

### 2. Customize the Consent Form Text

Edit the file: `Assets/WebGLTemplates/ConsentTemplate/index.html`

Update the following sections:
- Study title
- Purpose of the study
- Procedures
- Risks and benefits
- Data collection details
- Contact information

### 3. Build WebGL

1. Go to **File > Build Settings**
2. Select **WebGL** platform
3. Click **Build** or **Build and Run**
4. Unity will use the ConsentTemplate and include the consent form

### 4. Deploy

Upload the entire Build folder to your web server. The consent form will automatically appear before the game loads.

## How It Works

1. **User Experience Flow:**
   - User opens the webpage
   - Consent form is displayed
   - User must scroll through content
   - User checks the consent checkbox
   - "Start Study" button becomes enabled
   - User clicks button
   - Consent form fades out
   - Unity game loads and starts

2. **Data Recording:**
   - Consent timestamp is stored in browser's localStorage
   - Timestamp is logged to browser console
   - Can be sent to Unity game via `SendMessage` (optional)

3. **Files Structure:**
   ```
   ConsentTemplate/
   ├── index.html          # Main HTML with consent form
   ├── TemplateData/
   │   ├── style.css       # Styling for consent form and Unity container
   │   └── consent.js      # JavaScript logic for form handling
   └── README.md           # This file
   ```

## Customization Options

### Change Colors

Edit `TemplateData/style.css`:
- Line 8: Background gradient colors
- Line 33: Primary color (#667eea)
- Line 52: Heading colors

### Modify Consent Requirements

Edit `TemplateData/consent.js`:
- Add additional validation requirements
- Require multiple checkboxes
- Add signature field
- Implement time-spent tracking

### Add Additional Data Collection

In `consent.js`, you can track:
- Time spent reading consent form
- Scroll behavior
- Number of attempts to start without checking
- Any other user interactions

## Important Notes

- The consent form is shown every time the user loads the page (recommended for research)
- Consent timestamp is stored in localStorage (persists across sessions)
- Make sure to update contact information before deploying
- Ensure your IRB approval includes the exact consent text used
- Test the entire flow before deploying to participants

## Browser Compatibility

- Chrome, Firefox, Safari, Edge (latest versions)
- Mobile browsers supported with responsive design
- Requires JavaScript enabled

## Troubleshooting

**Issue:** "Start Study" button stays disabled
- **Solution:** Make sure the checkbox is checked

**Issue:** Unity game doesn't load
- **Solution:** Check browser console for errors. Ensure Build folder is in the correct location

**Issue:** Consent form doesn't look right
- **Solution:** Clear browser cache and refresh

## Contact

For questions about this template, contact the research team.
