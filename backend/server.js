require('dotenv').config(); // Load environment variables

const express = require('express');
const mongoose = require('mongoose');
const cors = require('cors');
const bodyParser = require('body-parser');

// Initialize Express app
const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(cors()); // Allow cross-origin requests from Unity WebGL
app.use(bodyParser.json({ limit: '10mb' }));
app.use(bodyParser.urlencoded({ extended: true, limit: '10mb' }));

// MongoDB connection
const MONGODB_URI = process.env.MONGODB_URI || 'mongodb://localhost:27017/robotics_trust_game';

mongoose.connect(MONGODB_URI, {
    useNewUrlParser: true,
    useUnifiedTopology: true
})
.then(() => console.log('✓ Connected to MongoDB'))
.catch(err => console.error('✗ MongoDB connection error:', err));

// Define Mongoose Schemas
const PlayerChoiceSchema = new mongoose.Schema({
    choiceId: String,
    choiceText: String,
    targetScene: String,
    timestamp: Number
}, { _id: false });

const SurveyAnswerSchema = new mongoose.Schema({
    questionId: Number,
    response: String
}, { _id: false });

const GameSessionSchema = new mongoose.Schema({
    sessionId: { type: String, required: true, unique: true },
    playerId: { type: String, required: true },
    timestamp: { type: Date, default: Date.now },
    choices: [PlayerChoiceSchema],
    surveyAnswers: [SurveyAnswerSchema],
    finalTimeRemaining: Number,
    completionStatus: {
        type: String,
        enum: ['completed', 'timeout', 'game_over', 'survey_only'],
        default: 'completed'
    },
    metadata: {
        ipAddress: String,
        userAgent: String,
        submittedAt: { type: Date, default: Date.now }
    }
});

// Create indexes for better query performance
GameSessionSchema.index({ playerId: 1, timestamp: -1 });
GameSessionSchema.index({ completionStatus: 1 });
GameSessionSchema.index({ 'metadata.submittedAt': -1 });

const GameSession = mongoose.model('GameSession', GameSessionSchema);

// Routes

// Health check endpoint
app.get('/health', (req, res) => {
    res.json({
        status: 'ok',
        mongodb: mongoose.connection.readyState === 1 ? 'connected' : 'disconnected',
        timestamp: new Date().toISOString()
    });
});

// Submit game session data
app.post('/api/submit-session', async (req, res) => {
    try {
        console.log('Received session submission:', {
            sessionId: req.body.sessionId,
            playerId: req.body.playerId,
            choicesCount: req.body.choices?.length || 0,
            surveyAnswersCount: req.body.surveyAnswers?.length || 0,
            completionStatus: req.body.completionStatus
        });

        // Validate required fields
        if (!req.body.sessionId || !req.body.playerId) {
            return res.status(400).json({
                error: 'Missing required fields: sessionId and playerId are required'
            });
        }

        // Create new game session document
        const sessionData = {
            sessionId: req.body.sessionId,
            playerId: req.body.playerId,
            timestamp: new Date(req.body.timestamp || Date.now()),
            choices: req.body.choices || [],
            surveyAnswers: req.body.surveyAnswers || [],
            finalTimeRemaining: req.body.finalTimeRemaining || 0,
            completionStatus: req.body.completionStatus || 'completed',
            metadata: {
                ipAddress: req.ip || req.connection.remoteAddress,
                userAgent: req.headers['user-agent'],
                submittedAt: new Date()
            }
        };

        const gameSession = new GameSession(sessionData);
        await gameSession.save();

        console.log('✓ Session saved successfully:', gameSession.sessionId);

        res.status(201).json({
            success: true,
            message: 'Session data saved successfully',
            sessionId: gameSession.sessionId,
            _id: gameSession._id
        });

    } catch (error) {
        console.error('✗ Error saving session:', error);

        // Handle duplicate sessionId error
        if (error.code === 11000) {
            return res.status(409).json({
                error: 'Session with this ID already exists',
                details: error.message
            });
        }

        res.status(500).json({
            error: 'Failed to save session data',
            details: error.message
        });
    }
});

// Get session by sessionId (for verification/debugging)
app.get('/api/session/:sessionId', async (req, res) => {
    try {
        const session = await GameSession.findOne({ sessionId: req.params.sessionId });

        if (!session) {
            return res.status(404).json({ error: 'Session not found' });
        }

        res.json(session);
    } catch (error) {
        console.error('Error fetching session:', error);
        res.status(500).json({ error: 'Failed to fetch session data' });
    }
});

// Get all sessions for a player
app.get('/api/player/:playerId/sessions', async (req, res) => {
    try {
        const sessions = await GameSession.find({ playerId: req.params.playerId })
            .sort({ timestamp: -1 })
            .limit(50);

        res.json({
            playerId: req.params.playerId,
            sessionCount: sessions.length,
            sessions: sessions
        });
    } catch (error) {
        console.error('Error fetching player sessions:', error);
        res.status(500).json({ error: 'Failed to fetch player sessions' });
    }
});

// Get statistics (for admin dashboard)
app.get('/api/stats', async (req, res) => {
    try {
        const totalSessions = await GameSession.countDocuments();
        const completedSessions = await GameSession.countDocuments({ completionStatus: 'completed' });
        const timeoutSessions = await GameSession.countDocuments({ completionStatus: 'timeout' });
        const gameOverSessions = await GameSession.countDocuments({ completionStatus: 'game_over' });
        const uniquePlayers = await GameSession.distinct('playerId');

        // Get most recent sessions
        const recentSessions = await GameSession.find()
            .sort({ 'metadata.submittedAt': -1 })
            .limit(10)
            .select('sessionId playerId completionStatus timestamp');

        res.json({
            totalSessions,
            completedSessions,
            timeoutSessions,
            gameOverSessions,
            uniquePlayers: uniquePlayers.length,
            recentSessions
        });
    } catch (error) {
        console.error('Error fetching stats:', error);
        res.status(500).json({ error: 'Failed to fetch statistics' });
    }
});

// Start server
app.listen(PORT, () => {
    console.log(`\n🚀 Server running on port ${PORT}`);
    console.log(`📊 Health check: http://localhost:${PORT}/health`);
    console.log(`📈 Stats: http://localhost:${PORT}/api/stats`);
    console.log(`💾 MongoDB URI: ${MONGODB_URI}\n`);
});

// Handle graceful shutdown
process.on('SIGINT', async () => {
    console.log('\n🛑 Shutting down server...');
    await mongoose.connection.close();
    console.log('✓ MongoDB connection closed');
    process.exit(0);
});
