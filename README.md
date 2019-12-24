# Prerequisites
* Docker
* NET Core SDK 3.0.100
* Node

# Initial dev setup
* Open repository directory
* Run:
  * `.\update_nuget_token.ps1 -current TOKEN -new <valid token>` (Windows PowerShell) or `./update_nuget_token.sh TOKEN <valid token>`
  * `docker-compose up -d`
  * `.\seed_db_sql.ps1` (Windows PowerShell) or `./seed_db_sql.sh` (Linux)

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
* Run `.\make_dev_cert.ps1`
* Run `docker-compose -f docker-compose.yml -f docker-compose.bff.yml -f docker-compose.identity.yml up -f docker-compose.users.yml -f docker-compose.orders.yml -d`
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
Users | https://localhost:5101 | https://localhost:5101/swagger
Orders | https://localhost:5201 | https://localhost:5201/swagger

# Test users:
Username | Password | Comment
--- | --- | ---
alice | alice | global role
bob | bob | 
