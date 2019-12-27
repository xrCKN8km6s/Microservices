# Prerequisites
* Docker
* NET Core SDK 3.0.100
* Node

# Initial dev setup
* Open repository directory
* Run:
  * `.\update_nuget_token.ps1 TOKEN <valid token>` (Windows PowerShell) or `./update_nuget_token.sh TOKEN <valid token>`
  * `docker-compose up -d` (GNU/Linux)
  * `.\seed_db_sql.ps1` (Windows PowerShell) or `./seed_db_sql.sh` (GNU/Linux)

# Back-end development setup
* Open solution and set multiple startup projects to:
  * BFF
  * Orders/Orders.API
  * Users/Users.API
  * Auth/Identity
* Start

To use EF Core CLI (e.g. `dotnet ef migrations add <migration name>`) run `dotnet tool install --global dotnet-ef` (https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet#ef-core-3x)

# Front-end development setup
* Open repository directory
* Run `./prepare_tls_token.sh certs users users qwerty1234 qwerty1234 BFF`
* Run `./prepare_tls_token.sh certs orders orders qwerty1234 qwerty1234 BFF`
* Run `docker-compose -f docker-compose.bff.yml -f docker-compose.identity.yml -f docker-compose.users.yml -f docker-compose.orders.yml up -d`
* Go to `/Web` and run `npm install`
* Run `npm run ng serve`
* Navigate to Web url
* Login using username/password provided below

# Microservices information:
Microservice | Url | Swagger
--- | --- | ---
Identity | http://localhost:3000 | NA
Web | http://localhost:4200 | NA
BFF | https://localhost:5001 | https://localhost:5001/swagger
Users | https://localhost:5101 | https://localhost:5101/swagger
Orders | https://localhost:5201 | https://localhost:5201/swagger

# Test users:
Username | Password | Comment
--- | --- | ---
alice | alice | global role
bob | bob | 
