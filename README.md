# Prerequisites
* Docker
* NET Core 2.2 SDK
* Node
* Angular `npm install -g @angular/cli`

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

# Startup
* Open Solution directory
* Run:
  * `docker-compose up -d`
  * `dotnet ef database update --project IntegrationEventLog/IntegrationEventLog.csproj --startup-project Orders/Orders/Orders.csproj --context IntegrationEventLogContext`
  * `dotnet ef database update --project Orders/Orders.Infrastructure/Orders.Infrastructure.csproj --startup-project Orders/Orders/Orders.csproj --context OrdersContext`
  * `dotnet ef database update --project Users/Users/Users.csproj`
* Open solution and set multiple startup projects to:
  * BFF
  * Orders
  * Users
* Press Start
* Go to `/Web` and run `ng serve --open`
* Navigate to web url
* Login using username/password provided above
