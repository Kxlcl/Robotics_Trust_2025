# Quick Start Guide - MongoDB Integration

## ✅ Setup Complete!

Your game is now configured to send all player choices and survey data to MongoDB!

## 🚀 Start the Backend Server

```bash
cd backend
npm start
```

You should see:
```
🚀 Server running on port 3000
📊 Health check: http://localhost:3000/health
📈 Stats: http://localhost:3000/api/stats
💾 MongoDB URI: mongodb+srv://Kelly:****@...
✓ Connected to MongoDB
```

**Keep this terminal window open while testing!**

## 🎮 Unity Setup

### 1. Add DatabaseSubmitter Component

In Unity Editor:
1. Open your main scene (e.g., `ID_Scene`)
2. Create an empty GameObject: `GameObject → Create Empty`
3. Name it: **"DatabaseSubmitter"**
4. Add Component: **DatabaseSubmitter**
5. In the Inspector, set:
   - **Api Url**: `http://localhost:3000/api/submit-session`
   - **Log Responses**: ✓ (checked - for testing)

### 2. Play and Test

1. **Play the game** in Unity Editor
2. **Make choices**:
   - Choose a robot
   - Answer yes/no to ID verification
   - Play until game over or timer runs out

3. **Check Unity Console** for:
   ```
   DatabaseSubmitter: Collected X player choices
   DatabaseSubmitter: Successfully submitted data to MongoDB
   ```

4. **Check Backend Terminal** for:
   ```
   Received session submission: { sessionId: '...', playerId: '...', ... }
   ✓ Session saved successfully
   ```

## 📊 View Your Data

### Option 1: Stats Endpoint (Quick)
Open in browser: http://localhost:3000/api/stats

### Option 2: MongoDB Compass (Visual)
1. Download [MongoDB Compass](https://www.mongodb.com/products/compass)
2. Connect with: `mongodb+srv://Kelly:sussybaka420@userdata.9c5leil.mongodb.net/robotics_trust_game`
3. Browse `gamesessions` collection

### Option 3: Command Line
```bash
mongosh "mongodb+srv://Kelly:sussybaka420@userdata.9c5leil.mongodb.net/robotics_trust_game"

# View all sessions
db.gamesessions.find().pretty()

# Count sessions
db.gamesessions.countDocuments()
```

## 🎯 What Gets Submitted

✅ **Game Over** - All choices + completion status + time remaining
✅ **Timer Expires** - All choices + timeout status + final time
✅ **Survey Complete** - Survey answers (when `FinalizeSurvey()` is called)

## 📝 Data Structure

Each session includes:
```json
{
  "sessionId": "unique-uuid",
  "playerId": "unique-player-uuid",
  "choices": [
    { "choiceId": "robot_choice", "choiceText": "Follow Robot A", ... },
    { "choiceId": "id_verification", "choiceText": "Yes", ... }
  ],
  "surveyAnswers": [
    { "questionId": 1, "response": "Strongly Agree" }
  ],
  "finalTimeRemaining": 450.5,
  "completionStatus": "completed" | "timeout" | "game_over",
  "timestamp": "2025-10-06T..."
}
```

## 🔧 Troubleshooting

### Backend won't connect
- Check MongoDB Atlas **Network Access** allows your IP
- Test with: `node backend/test-connection.js`

### Unity can't submit data
- Verify backend is running: http://localhost:3000/health
- Check `apiUrl` in DatabaseSubmitter component
- Look for errors in Unity Console

### No data in MongoDB
- Check backend console for "✓ Session saved successfully"
- Verify database name is `robotics_trust_game`
- Use MongoDB Compass to browse visually

## 🚀 Deploy for Production

When ready to deploy your WebGL build:

1. **Deploy Backend** (choose one):
   - [Heroku](https://www.heroku.com/) (easiest)
   - [Railway](https://railway.app/)
   - [Render](https://render.com/)

2. **Update Unity**:
   - Change `apiUrl` to your production URL
   - Example: `https://your-app.herokuapp.com/api/submit-session`

3. **Build WebGL**:
   - File → Build Settings → WebGL → Build

## 📞 Need Help?

Check these files:
- **SETUP_CHECKLIST.md** - Detailed verification steps
- **MONGODB_SETUP.md** - Complete setup guide
- **backend/README.md** - API documentation

## ✅ All Code Changes Made

✓ **GameOverManager.cs** - Submits data before showing survey
✓ **GlobalTimer.cs** - Submits data when timer expires
✓ **SurveySaver.cs** - Submits survey responses automatically
✓ **DatabaseSubmitter.cs** - New! Handles all submissions
✓ **Backend server.js** - Ready to receive data

**You're all set! Start the backend server and test in Unity!** 🎉
