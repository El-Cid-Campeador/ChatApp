FROM node:18-alpine

WORKDIR /usr/src/app/chat_server

COPY package*.json .

RUN npm i

COPY . .

EXPOSE 9000

CMD [ "npm", "run", "dev" ]
