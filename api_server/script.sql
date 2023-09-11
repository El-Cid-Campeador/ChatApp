CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS users (
    id UUID DEFAULT uuid_generate_v4 () PRIMARY KEY,
    firstName VARCHAR(50) NOT NULL,
    lastName VARCHAR(50) NOT NULL,
    username VARCHAR(20) NOT NULL,
    email VARCHAR(50) NOT NULL,
    passwordHash TEXT NOT NULL,
    profilePicPath TEXT,
    createdOn TIMESTAMP NOT NULL DEFAULT NOW(),
    deletedOn TIMESTAMP
);

CREATE TABLE IF NOT EXISTS rooms (
    id UUID PRIMARY KEY,
    createdOn TIMESTAMP NOT NULL DEFAULT NOW(),
    deletedOn TIMESTAMP
);

CREATE TABLE IF NOT EXISTS messages (
    id UUID PRIMARY KEY,
    senderId UUID NOT NULL,
    roomId UUID NOT NULL,
    content TEXT NOT NULL,
    date TIMESTAMP NOT NULL,
    FOREIGN KEY (senderId) REFERENCES users(id),
    FOREIGN KEY (roomId) REFERENCES rooms(id)
);

CREATE TABLE IF NOT EXISTS chats (
    roomId UUID,
    userId UUID,
    PRIMARY KEY(roomId, userId)
);
