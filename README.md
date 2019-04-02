Learning repository based on https://github.com/dotnet-architecture/eShopOnContainers

Microservice | Url
--- | ---
BFF | http://localhost:5000
Users | http://localhost:5100
Orders | http://localhost:5200

# Test users:
Username | Password | Comment
--- | --- | ---
alice | alice | global role
bob | bob | 

# Startup
* Open Solution directory
* Run `docker-compose up -d`
* Locate all implementations of `IDesignTimeDbContextFactory` and replace `<ConnectionString>` with appropriate connection strings
* Go to `/IntegrationEventLog` and run `dotnet ef database update`
* Go to `/Orders/Orders.Infrastructure` and run `dotnet ef database update`
* Go to `/Users/Users` and run `dotnet ef database update`
* Open solution and set multiple startup projects to:
  * BFF
  * Identity
  * Orders
  * Users
* Press Start
* Go to `/Web` and run `ng serve --open`
* Login using username/password provided above


**TODO: Improve DEV setup process**
