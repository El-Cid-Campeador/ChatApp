import express from 'express';
import cors from 'cors';
import { createServer } from 'node:http';
import { Server } from 'socket.io';

const PORT = 9000;

const allowedDomains = ['http://localhost:5173'];

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

io.use((socket, next) => {
    const id = socket.handshake.auth.id;

    if (!id) {
        return next(new Error("invalid id"));
    }

    next();
});

let onlineUsers: any[] = [];

io.on('connection', (socket) => {
    socket.on("new-user-add", (newUserId) => {
        if (!onlineUsers.includes((user: any) => user.id === newUserId)) {
            onlineUsers.push({ id: newUserId, socketId: socket.id });
        }
        
        io.emit("get-users", onlineUsers);
    });

    socket.on("disconnect", () => {
        onlineUsers = onlineUsers.filter((user) => user.socketId !== socket.id);

        io.emit("get-users", onlineUsers);
    });
      
    socket.on('createRoom', room => {
        socket.join(room);

        socket.on('messageFromClient', msg => {
            // const data = { user: socket.handshake.auth.username, msg };
    
            socket.to(room).emit('messageFromServer', msg);
        });
    });
});

httpServer.listen(PORT, () => {
    console.log(`Server running at http://localhost:${PORT}`)
});
