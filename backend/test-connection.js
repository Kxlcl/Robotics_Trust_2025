require('dotenv').config();
const mongoose = require('mongoose');

const MONGODB_URI = process.env.MONGODB_URI;

console.log('Testing MongoDB connection...');
console.log('URI:', MONGODB_URI.replace(/:[^:]*@/, ':****@')); // Hide password

mongoose.connect(MONGODB_URI, {
    useNewUrlParser: true,
    useUnifiedTopology: true
})
.then(() => {
    console.log('✓ Successfully connected to MongoDB!');
    console.log('Database:', mongoose.connection.name);
    mongoose.connection.close();
    process.exit(0);
})
.catch(err => {
    console.error('✗ MongoDB connection failed:');
    console.error(err.message);
    process.exit(1);
});
