#!/bin/bash -x

dotnet ef database update -v --project IntegrationEventLog/IntegrationEventLog.csproj --startup-project Orders/Orders/Orders.csproj --context IntegrationEventLogContext

dotnet ef database update -v --project Orders/Orders.Infrastructure/Orders.Infrastructure.csproj --startup-project Orders/Orders/Orders.csproj --context OrdersContext

dotnet ef database update -v --project Users/Users/Users.csproj
