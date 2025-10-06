# Robotics Trust 2025 - Backend API

Backend server for collecting game session data and survey responses from the Unity game.

Password: mongodb+srv://Kelly:sussybaka420@userdata.9c5leil.mongodb.net/?retryWrites=true&w=majority&appName=UserData

## Setup Instructions

### Prerequisites
- Node.js (v14 or higher)
- MongoDB (local installation or MongoDB Atlas account)

### Installation

1. **Install dependencies:**
   ```bash
   cd backend
   npm install
   ```

2. **Configure MongoDB:**
   - Copy `.env.example` to `.env`:
     ```bash
     cp .env.example .env
     ```

   - Edit `.env` and set your MongoDB connection string:
     - **Local MongoDB:** `MONGODB_URI=mongodb://localhost:27017/robotics_trust_game`
     - **MongoDB Atlas:** `MONGODB_URI=mongodb+srv://username:password@cluster.mongodb.net/robotics_trust_game`

3. **Start the server:**
   ```bash
   npm start
   ```

   Or for development with auto-reload:
   ```bash
   npm run dev
   ```

### MongoDB Atlas Setup (Cloud Database)

If you want to use MongoDB Atlas (free tier available):

1. Go to [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
2. Create a free account and cluster
3. Create a database user with read/write permissions
4. Whitelist your IP address (or use 0.0.0.0/0 for development)
5. Get your connection string and add it to `.env`

## API Endpoints

### POST `/api/submit-session`
Submit game session data including player choices and survey responses.

**Request Body:**
```json
{
  "sessionId": "unique-session-id",
  "playerId": "unique-player-id",
  "timestamp": "2025-01-01T12:00:00Z",
  "choices": [
    {
      "choiceId": "robot_choice",
      "choiceText": "Follow Robot A",
      "targetScene": "Waiting_Scene",
      "timestamp": 123.45
    }
  ],
  "surveyAnswers": [
    {
      "questionId": 1,
      "response": "Strongly Agree"
    }
  ],
  "finalTimeRemaining": 450.5,
  "completionStatus": "completed"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Session data saved successfully",
  "sessionId": "unique-session-id",
  "_id": "mongodb-document-id"
}
```

### GET `/health`
Check server health and MongoDB connection status.

### GET `/api/session/:sessionId`
Retrieve a specific session by sessionId.

### GET `/api/player/:playerId/sessions`
Get all sessions for a specific player.

### GET `/api/stats`
Get overall statistics (total sessions, completion rates, etc.).

## Unity Integration

In Unity, set the `apiUrl` in the `DatabaseSubmitter` component:

- **Local development:** `http://localhost:3000/api/submit-session`
- **Production:** `https://your-domain.com/api/submit-session`

### Example Usage in Unity:

```csharp
// At game end
DatabaseSubmitter.Instance.OnGameComplete();

// On timeout
DatabaseSubmitter.Instance.OnTimeout();

// On game over
DatabaseSubmitter.Instance.OnGameOver();
```

## Data Structure

### GameSession Document
```javascript
{
  sessionId: String,           // Unique session identifier
  playerId: String,            // Unique player identifier
  timestamp: Date,             // Session start time
  choices: [{                  // Player decisions
    choiceId: String,
    choiceText: String,
    targetScene: String,
    timestamp: Number
  }],
  surveyAnswers: [{            // Survey responses
    questionId: Number,
    response: String
  }],
  finalTimeRemaining: Number,  // Time left when session ended
  completionStatus: String,    // 'completed', 'timeout', 'game_over', 'survey_only'
  metadata: {
    ipAddress: String,
    userAgent: String,
    submittedAt: Date
  }
}
```

## Deployment

### Deploy to Heroku:
```bash
heroku create your-app-name
heroku config:set MONGODB_URI="your-mongodb-atlas-uri"
git push heroku main
```

### Deploy to Railway/Render:
1. Connect your GitHub repository
2. Set environment variable `MONGODB_URI`
3. Deploy

## Security Considerations

For production:
1. Add API authentication (JWT tokens)
2. Rate limiting
3. Input validation
4. HTTPS only
5. Environment-specific CORS settings
6. MongoDB IP whitelist

## Monitoring

View stats at: `http://localhost:3000/api/stats`

This provides:
- Total sessions
- Completion rates
- Unique players
- Recent sessions
