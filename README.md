# Chat App (Vue 3 + ASP.NET Core + Node.JS + PostgreSQL + Redis)

###### Prerequisites:

* Docker
* .NET SDK 7.0

```bash
$ git clone https://github.com/El-Cid-Campeador/ChatApp && cd ChatApp
$ cd api_server
$ dotnet restore && dotnet watch --no-hot-reload
$ cd api_server.Tests && dotnet restore
$ cd ../chat_server && npm i
$ cd ../client && npm i
$ cd ../
$ docker compose down && docker build --no-cache
$ docker compose up
```
