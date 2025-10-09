# MongoDB Integration Setup Checklist

## ✅ Backend Setup

### 1. Test MongoDB Connection
```bash
cd backend
node test-connection.js
```

**Expected output:**
```
Testing MongoDB connection...
URI: mongodb+srv://Kelly:****@userdata.9c5leil.mongodb.net/robotics_trust_game?...
✓ Successfully connected to MongoDB!
Database: robotics_trust_game
```

### 2. Start Backend Server
```bash
cd backend
npm start
```

**Expected output:**
```
🚀 Server running on port 3000
📊 Health check: http://localhost:3000/health
📈 Stats: http://localhost:3000/api/stats
💾 MongoDB URI: mongodb+srv://Kelly:****@...
✓ Connected to MongoDB
```

### 3. Test Health Endpoint
Open browser or use curl:
```bash
curl http://localhost:3000/health
```

**Expected response:**
```json
{
  "status": "ok",
  "mongodb": "connected",
  "timestamp": "2025-10-06T..."
}
```

## ✅ Unity Setup

### 1. Add DatabaseSubmitter Component
- [x] Created `DatabaseSubmitter.cs` script ✓
- [ ] Create empty GameObject in your main scene (e.g., ID_Scene)
  - Name it: "DatabaseSubmitter"
  - Add Component: `DatabaseSubmitter`
  - Set `apiUrl`: `http://localhost:3000/api/submit-session`
  - Set `logResponses`: true (for testing)

### 2. Verify Existing Components
Check that these are in your scenes:
- [ ] `GlobalTimer` - Tracks game time ✓
- [ ] `PlayerChoiceTracker` - Records decisions ✓
- [ ] `SurveySaver` - Saves survey responses ✓

### 3. Add Submission Calls

#### GameOverManager.cs
Find the `ShowGameOver()` method and add:
```csharp
public void ShowGameOver()
{
    // Add this line BEFORE redirecting to survey
    if (DatabaseSubmitter.Instance != null)
    {
        DatabaseSubmitter.Instance.OnGameOver();
    }

    // ... rest of your game over code
}
```

#### GlobalTimer.cs
In the `Update()` method when timer reaches 0, add:
```csharp
else if (timerStarted && timeRemaining <= 0)
{
    // Add this once when timer expires
    if (timerText != null)
    {
        timerText.text = "00:00";

        // Add this:
        if (DatabaseSubmitter.Instance != null)
        {
            DatabaseSubmitter.Instance.OnTimeout();
        }
    }
}
```

#### SurveySaver.cs
Already updated! ✓ The `FinalizeSurvey()` method now submits to database automatically.

## ✅ Testing

### 1. Test in Unity Editor

1. **Start backend server:**
   ```bash
   cd backend
   npm start
   ```

2. **Play game in Unity Editor**

3. **Make some choices:**
   - Choose a robot to follow
   - Make yes/no decision
   - Play until game over or timer expires

4. **Check Unity Console** for:
   ```
   DatabaseSubmitter: Collected X player choices
   DatabaseSubmitter: Submitting data:
   DatabaseSubmitter: Successfully submitted data to MongoDB
   ```

5. **Check backend server console** for:
   ```
   Received session submission: { sessionId: '...', playerId: '...', ... }
   ✓ Session saved successfully: abc123
   ```

### 2. Verify Data in MongoDB

**Option A: Using MongoDB Compass (GUI)**
1. Open MongoDB Compass
2. Connect with your connection string
3. Navigate to `robotics_trust_game` database
4. View `gamesessions` collection

**Option B: Using mongosh (CLI)**
```bash
mongosh "mongodb+srv://Kelly:sussybaka420@userdata.9c5leil.mongodb.net/robotics_trust_game"

# View sessions
db.gamesessions.find().pretty()

# Count sessions
db.gamesessions.countDocuments()
```

**Option C: Using API Endpoint**
```bash
# Get stats
curl http://localhost:3000/api/stats

# Get specific session (replace SESSION_ID)
curl http://localhost:3000/api/session/SESSION_ID
```

### 3. Test Error Handling

**Test 1: Backend Offline**
1. Stop backend server
2. Play game and make choices
3. Check Unity console - should see:
   ```
   DatabaseSubmitter: Failed to submit data. Error: ...
   DatabaseSubmitter: Saved backup to: /path/to/backup_session_*.json
   ```

**Test 2: Invalid URL**
1. Change `apiUrl` in DatabaseSubmitter to wrong URL
2. Play game
3. Should fallback to local backup

## 🔍 Troubleshooting

### Backend won't start
- [ ] Check Node.js is installed: `node -v` (should be v14+)
- [ ] Check npm dependencies: `npm install` in backend folder
- [ ] Check `.env` file exists with correct MongoDB URI
- [ ] Test connection: `node test-connection.js`

### Unity can't connect to backend
- [ ] Backend server is running
- [ ] Check `apiUrl` in DatabaseSubmitter component
- [ ] Check Unity console for error details
- [ ] Try health endpoint in browser: `http://localhost:3000/health`

### Data not appearing in MongoDB
- [ ] Check backend console for "✓ Session saved successfully"
- [ ] Verify database name is `robotics_trust_game`
- [ ] Check MongoDB Atlas Network Access allows your IP
- [ ] Use MongoDB Compass to browse database visually

### WebGL Build Issues
- [ ] Backend needs to be publicly accessible (not localhost)
- [ ] Use HTTPS for production
- [ ] Check browser console for CORS errors

## 📊 What Data Gets Stored

Each game session stores:

```json
{
  "sessionId": "unique-uuid",
  "playerId": "unique-player-uuid",
  "timestamp": "2025-10-06T...",
  "choices": [
    {
      "choiceId": "robot_choice",
      "choiceText": "Follow Robot A",
      "targetScene": "Waiting_Scene",
      "timestamp": 45.67
    },
    {
      "choiceId": "id_verification",
      "choiceText": "Yes",
      "targetScene": "...",
      "timestamp": 78.9
    }
  ],
  "surveyAnswers": [
    {
      "questionId": 1,
      "response": "Strongly Agree"
    }
  ],
  "finalTimeRemaining": 450.5,
  "completionStatus": "completed",
  "metadata": {
    "ipAddress": "127.0.0.1",
    "userAgent": "UnityPlayer/...",
    "submittedAt": "2025-10-06T..."
  }
}
```

## 🚀 Next Steps

Once everything is working:

1. **Test with multiple playthroughs** to ensure all choices are captured
2. **Deploy backend to production** (Heroku, Railway, Render)
3. **Update Unity `apiUrl`** to production URL
4. **Set up MongoDB Atlas alerts** for usage monitoring
5. **Add data export/analysis** capabilities
6. **Consider adding authentication** for API security

## 📝 Important Notes

- Each player gets a unique `playerId` stored in PlayerPrefs
- Sessions are saved immediately when game ends
- Local backup is created if database submission fails
- All player choices and survey responses are included in each session
- Timestamps use Unity's `Time.time` and ISO 8601 format

## ✅ Final Checklist

Before deploying:
- [ ] Backend tested and working locally
- [ ] Unity can submit data successfully
- [ ] Data appears in MongoDB
- [ ] Error handling works (tested with backend offline)
- [ ] All game endpoints call DatabaseSubmitter appropriately
- [ ] Production MongoDB URI configured
- [ ] Backend deployed to cloud service
- [ ] Unity `apiUrl` updated to production URL
