FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY Orders/Orders.API/Orders.API.csproj Orders/Orders.API/
COPY Orders/Orders.Domain/Orders.Domain.csproj Orders/Orders.Domain/
COPY Orders/Orders.Infrastructure/Orders.Infrastructure.csproj Orders/Orders.Infrastructure/

COPY IntegrationEventLog/IntegrationEventLog.csproj IntegrationEventLog/
COPY EventBus/EventBus.csproj EventBus/
COPY EventBus.RabbitMQ/EventBus.RabbitMQ.csproj EventBus.RabbitMQ/

RUN dotnet restore Orders/Orders.API/Orders.API.csproj
COPY . .

FROM build AS publish
RUN dotnet publish Orders/Orders.API/Orders.API.csproj -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Orders.API.dll"]