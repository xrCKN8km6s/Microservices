Learning repository based on https://github.com/dotnet-architecture/eShopOnContainers

Microservice | Url | Swagger
--- | --- | ---
Identity | http://localhost:3000 | NA
BFF | http://localhost:5000 | http://localhost:5000/swagger
Users | http://localhost:5100 | http://localhost:5100/swagger
Orders | http://localhost:5200 | http://localhost:5200/swagger
Web | http://localhost:4200 | NA

# Test users:
Username | Password | Comment
--- | --- | ---
alice | alice | global role
bob | bob | 

# Prerequisites
* Node
* Angular `npm install -g @angular/cli`

# Startup
* Open Solution directory
* Run `docker-compose up -d`
* Run `dotnet ef database update --project IntegrationEventLog/IntegrationEventLog.csproj --startup-project Orders/Orders/Orders.csproj --context IntegrationEventLogContext`
* Run `dotnet ef database update --project Orders/Orders.Infrastructure/Orders.Infrastructure.csproj --startup-project Orders/Orders/Orders.csproj --context OrdersContext`
* Run `dotnet ef database update --project Users/Users/Users.csproj`
* Open solution and set multiple startup projects to:
  * BFF
  * Identity
  * Orders
  * Users
* Press Start
* Go to `/Web` and run `ng serve --open`
* Navigate to web url
* Login using username/password provided above


**TODO: Improve DEV setup process**
