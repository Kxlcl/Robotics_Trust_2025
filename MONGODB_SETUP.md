# MongoDB Integration Setup Guide

This guide will help you connect your Unity game to MongoDB to store player decisions and survey responses.

## Architecture Overview

```
Unity Game → HTTP POST → Node.js Backend → MongoDB Database
```

## Step 1: Set Up MongoDB

### Option A: Local MongoDB (Development)

1. **Install MongoDB:**
   - Mac: `brew install mongodb-community`
   - Windows: Download from [mongodb.com](https://www.mongodb.com/try/download/community)
   - Linux: Follow [installation guide](https://docs.mongodb.com/manual/administration/install-on-linux/)

2. **Start MongoDB:**
   ```bash
   # Mac/Linux
   mongod --dbpath /path/to/data/directory

   # Or use brew services on Mac
   brew services start mongodb-community
   ```

### Option B: MongoDB Atlas (Cloud - Free Tier Available)

1. Go to [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
2. Create a free account
3. Create a new cluster (Free M0 tier)
4. Create a database user:
   - Click "Database Access"
   - Add new database user with username and password
   - Give "Read and write to any database" permission
5. Whitelist IP addresses:
   - Click "Network Access"
   - Add IP: `0.0.0.0/0` (allows all - for development only)
   - For production, whitelist specific IPs
6. Get connection string:
   - Click "Connect" on your cluster
   - Choose "Connect your application"
   - Copy the connection string
   - Replace `<password>` with your database user password

## Step 2: Set Up Backend Server

1. **Navigate to backend directory:**
   ```bash
   cd backend
   ```

2. **Install Node.js dependencies:**
   ```bash
   npm install
   ```

3. **Configure environment variables:**
   ```bash
   cp .env.example .env
   ```

4. **Edit `.env` file:**

   For **local MongoDB:**
   ```
   MONGODB_URI=mongodb://localhost:27017/robotics_trust_game
   PORT=3000
   ```

   For **MongoDB Atlas:**
   ```
   MONGODB_URI=mongodb+srv://username:password@cluster.mongodb.net/robotics_trust_game?retryWrites=true&w=majority
   PORT=3000
   ```

5. **Start the server:**
   ```bash
   npm start
   ```

   Or for development with auto-restart:
   ```bash
   npm run dev
   ```

6. **Verify server is running:**
   - Open browser to `http://localhost:3000/health`
   - Should see: `{"status":"ok","mongodb":"connected"}`

## Step 3: Configure Unity

1. **Add DatabaseSubmitter to your scene:**
   - Create an empty GameObject named "DatabaseSubmitter"
   - Add the `DatabaseSubmitter` component
   - Set `apiUrl` to:
     - Local: `http://localhost:3000/api/submit-session`
     - Production: `https://your-domain.com/api/submit-session`

2. **The DatabaseSubmitter will automatically become a singleton and persist across scenes.**

## Step 4: Submit Data from Unity

### Automatic Submission (Recommended)

Add calls to DatabaseSubmitter at key points in your game:

```csharp
// When player completes the game
DatabaseSubmitter.Instance.OnGameComplete();

// When timer runs out
DatabaseSubmitter.Instance.OnTimeout();

// When player gets game over
DatabaseSubmitter.Instance.OnGameOver();
```

### Manual Submission

```csharp
// Submit whenever you want
DatabaseSubmitter.Instance.SubmitSessionData("custom_status");
```

## Step 5: Integrate with Existing Code

### Update GameOverManager

```csharp
void ShowGameOver()
{
    // Submit data before redirecting
    DatabaseSubmitter.Instance.OnGameOver();

    // ... rest of your game over code
}
```

### Update Timer Completion

```csharp
// In GlobalTimer.cs Update() when time runs out
if (timeRemaining <= 0)
{
    DatabaseSubmitter.Instance.OnTimeout();
    // ... rest of timeout handling
}
```

### Survey Completion

The `SurveySaver.FinalizeSurvey()` method already submits to database automatically!

## Step 6: Testing

1. **Start your backend server:**
   ```bash
   cd backend
   npm start
   ```

2. **Play your Unity game in the editor**

3. **Make some choices and complete/quit the game**

4. **Check the server console** - you should see:
   ```
   Received session submission: { sessionId: '...', playerId: '...', ... }
   ✓ Session saved successfully: abc123
   ```

5. **View the data in MongoDB:**
   ```bash
   # Connect to MongoDB
   mongosh

   # Switch to your database
   use robotics_trust_game

   # View sessions
   db.gamesessions.find().pretty()
   ```

6. **Check stats endpoint:**
   - Open browser to `http://localhost:3000/api/stats`

## Step 7: Deploy to Production

### Deploy Backend to Heroku (Free Tier)

1. **Install Heroku CLI:**
   ```bash
   brew install heroku/brew/heroku  # Mac
   # or download from heroku.com
   ```

2. **Login and create app:**
   ```bash
   cd backend
   heroku login
   heroku create your-game-backend
   ```

3. **Set environment variables:**
   ```bash
   heroku config:set MONGODB_URI="your-mongodb-atlas-connection-string"
   ```

4. **Deploy:**
   ```bash
   git add .
   git commit -m "Add backend"
   git push heroku main
   ```

5. **Verify deployment:**
   ```bash
   heroku open /health
   ```

6. **Update Unity apiUrl to your Heroku URL:**
   ```
   https://your-game-backend.herokuapp.com/api/submit-session
   ```

### Alternative Deployment Options

- **Railway:** Auto-deploy from GitHub, very easy setup
- **Render:** Free tier available, similar to Heroku
- **DigitalOcean App Platform:** More advanced, $5/month
- **AWS/Google Cloud:** Most flexible, requires more setup

## Data Structure

Data stored in MongoDB includes:

```json
{
  "_id": "mongodb-id",
  "sessionId": "unique-session-id",
  "playerId": "unique-player-id",
  "timestamp": "2025-01-01T12:00:00Z",
  "choices": [
    {
      "choiceId": "robot_choice",
      "choiceText": "Follow Robot A",
      "targetScene": "Waiting_Scene",
      "timestamp": 45.67
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
    "userAgent": "Mozilla/5.0...",
    "submittedAt": "2025-01-01T12:10:00Z"
  }
}
```

## Troubleshooting

### "Failed to submit data" in Unity

1. Check backend server is running: `http://localhost:3000/health`
2. Check Unity console for detailed error message
3. Verify `apiUrl` is correct in DatabaseSubmitter
4. Check for CORS errors (should be handled by backend)

### "MongoDB connection error" in backend

1. Verify MongoDB is running (if local)
2. Check MongoDB Atlas IP whitelist (if cloud)
3. Verify connection string in `.env` file
4. Check username/password in connection string

### Data not appearing in MongoDB

1. Check backend console for "✓ Session saved successfully"
2. Use MongoDB Compass to visually browse database
3. Run `db.gamesessions.find().pretty()` in mongosh
4. Check database name matches in connection string

### WebGL Build Issues

1. Make sure backend has CORS enabled (already done in server.js)
2. Use HTTPS for production (HTTP may be blocked by browsers)
3. Check browser console for security errors

## Viewing Your Data

### MongoDB Compass (GUI)

1. Download [MongoDB Compass](https://www.mongodb.com/products/compass)
2. Connect using your connection string
3. Browse collections visually

### Command Line

```bash
# Connect to MongoDB
mongosh "mongodb://localhost:27017/robotics_trust_game"

# Or for Atlas
mongosh "mongodb+srv://username:password@cluster.mongodb.net/robotics_trust_game"

# View all sessions
db.gamesessions.find().pretty()

# Count total sessions
db.gamesessions.countDocuments()

# Find sessions by completion status
db.gamesessions.find({ completionStatus: "completed" })

# Get recent sessions
db.gamesessions.find().sort({ timestamp: -1 }).limit(10)
```

## Export Data for Analysis

```bash
# Export to JSON
mongoexport --db=robotics_trust_game --collection=gamesessions --out=data.json

# Export to CSV
mongoexport --db=robotics_trust_game --collection=gamesessions --type=csv --fields=playerId,completionStatus,finalTimeRemaining --out=data.csv
```

## Security Best Practices (Production)

1. **Use environment variables** - Never commit credentials
2. **Enable authentication** - Add API key or JWT tokens
3. **Rate limiting** - Prevent abuse
4. **HTTPS only** - Encrypt data in transit
5. **Input validation** - Sanitize all inputs
6. **MongoDB Atlas** - Use IP whitelisting and strong passwords
7. **Monitor usage** - Set up alerts for unusual activity

## Next Steps

- Add authentication to protect your API
- Set up analytics dashboard to visualize data
- Add data export functionality
- Implement A/B testing for different game conditions
- Add real-time monitoring and alerts
