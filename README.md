# Prerequisites
* Docker
* NET Core SDK 3.1.301
* PowerShell 7
* OpenSSL
* Node

# Initial dev setup
* Open repository directory
* Run:
  * `./prepare_certs_ec.ps1 localhost,orders,users qwerty1234`
  * `docker-compose up -d postgreSql`
  * `./seed_db_sql.ps1`

# Back-end development setup
* Open solution and set multiple startup projects to:
  * BFF
  * Orders/Orders.API
  * Users/Users.API
  * Auth/Identity
* Start

To use EF Core CLI (e.g. `dotnet ef migrations add <migration name>`) run `dotnet tool install --global dotnet-ef` (https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet#ef-core-3x)

# Front-end development setup
* Run `docker-compose up -d bff_proxy`
* Go to `/Web` and run `npm install`
* Run `npm run ng serve`
* Navigate to Web url
* Login using username/password provided below

# Microservices DEV information
Microservice | Url | Swagger
--- | --- | ---
Identity | http://localhost:3000 | NA
Web | http://localhost:4200 | NA
BFF | https://localhost:5001 | https://localhost:5001/swagger
BFF - NGINX Reverse Proxy | https://localhost:1443 | https://localhost:1443/swagger
Users | https://localhost:5101 | https://localhost:5101/swagger
Orders | https://localhost:5201 | https://localhost:5201/swagger

Name | Url
--- | ---
Kibana | http://localhost:5601
pgAdmin | http://localhost:15432
RabbitMQ management plugin | http://localhost:15672


# Test users
Username | Password | Comment
--- | --- | ---
alice | alice | global role
bob | bob | 
