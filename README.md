# Prerequisites
* Docker
* NET Core SDK 3.0.100
* Node

# Initial dev setup
* Open repository directory
* Run:
  * `docker-compose up -d`
  * `.\seed_db_sql.ps1` (Windows PowerShell) or `./seed_db_sql.sh` (Linux)

# Back-end development setup
* Open solution and set multiple startup projects to:
  * BFF
  * Orders
  * Users
  * Identity
* Start

To use EF Core CLI (e.g. `dotnet ef migrations add <migration name>`) run `dotnet tool install --global dotnet-ef` (https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet#ef-core-3x)

# Front-end development setup
* Open repository directory
* Run `docker-compose -f docker-compose.yml -f docker-compose.local.yml up -d`
* Go to `/Web` and run `npm install` 
* Run `npm run ng serve`
* Navigate to Web url
* Login using username/password provided below

# Microservices information:
Microservice | Url | Swagger
--- | --- | ---
Identity | http://localhost:3000 | NA
Web | http://localhost:4200 | NA
BFF | http://localhost:5000 | http://localhost:5000/swagger
Users | http://localhost:5100 | http://localhost:5100/swagger
Orders | http://localhost:5200 | http://localhost:5200/swagger

# Test users:
Username | Password | Comment
--- | --- | ---
alice | alice | global role
bob | bob | 
