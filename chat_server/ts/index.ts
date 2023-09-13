import express from 'express';
import cors from 'cors';
import { createServer } from 'node:http';
import { Server } from 'socket.io';
import { createClient } from 'redis';

const PORT = 9000;

const allowedDomains = ['http://localhost:5173'];

let userData: User = {
    id: '',
    username: '',
    firstName: '',
    lastName: '',
    profilePicPath: ''
}

const redisClient = createClient();

await redisClient.connect();

const app = express();

app.use(cors({
    origin: function (origin, callback) {
        if (!origin || allowedDomains.indexOf(origin) != -1) {
            callback(null, true);

            return;
        } else {
            callback(new Error('Not allowed by CORS'));

            return;
        }
    },
    methods: ['GET', 'POST'],
    credentials: true
}));

app.use((err: Error, req: express.Request, res: express.Response, next: express.NextFunction) => {
    if (err instanceof Error && err.message === 'Not allowed by CORS') {
        res.status(403).json({ error: 'CORS not allowed' });
    } else {
        next(err);
    }
});

const httpServer = createServer();

const io = new Server(httpServer, {
    cors: {
        origin: function (origin, callback) {
            if (!origin || allowedDomains.indexOf(origin) != -1) {
                callback(null, true);
    
                return;
            } else {
                callback(new Error('Not allowed by CORS'));
    
                return;
            }
        },
        methods: ['GET', 'POST'],
        credentials: true
    }
});

async function getOnlineUsers() {
    const users = await redisClient.sMembers(`connectedUsers`);

    const res = users.map((user) => JSON.parse(user)) as User[];

    return res;
}

io.use((socket, next) => {
    userData = socket.handshake.auth as User;

    if (!userData) {
        return next(new Error("Not authorized!"));
    }

    next();
});

io.on('connection', (socket) => {
    const payload = JSON.stringify({ ...userData });

    socket.setMaxListeners(0);

    socket.on('join', async (userData) => {
        await redisClient.sAdd(`connectedUsers`, userData);

        const users = await getOnlineUsers();

        io.emit('getAllUsers', users);
    });

    socket.on('joinRoom', (room) => {
        socket.join(room);
    });
    
    socket.on('from', (data) => {
        socket.to(data.roomId).emit('to', data.msg);
    });

    socket.on('disconnect', async () => {
        await redisClient.sRem(`connectedUsers`, payload);

        const users = await getOnlineUsers();
        
        io.emit('getAllUsers', users);
    });
});

httpServer.listen(PORT, () => {
    console.log(`Server running at http://localhost:${PORT}`)
});

process.on('SIGINT', async () => {
    await redisClient.DEL('connectedUsers');

    process.exit(0);
});
