FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ARG GPR_NuGet_Passwd
WORKDIR /src
COPY ["Orders/Orders.API/Orders.API.csproj", "Orders/Orders.API/"]
COPY ["Orders/Orders.Domain/Orders.Domain.csproj", "Orders/Orders.Domain/"]
COPY ["Orders/Orders.Infrastructure/Orders.Infrastructure.csproj", "Orders/Orders.Infrastructure/"]

COPY ["EventBus/EventBus.csproj", "EventBus/"]
COPY ["EventBus.AspNetCore/EventBus.AspNetCore.csproj", "EventBus.AspNetCore/"]
COPY ["EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "EventBus.RabbitMQ/"]
COPY ["EventBus.RabbitMQ.AspNetCore/EventBus.RabbitMQ.AspNetCore.csproj", "EventBus.RabbitMQ.AspNetCore/"]

COPY ["IntegrationEventLog/IntegrationEventLog.csproj", "IntegrationEventLog/"]
#COPY ["NuGet.Config", "Orders.API/"]
RUN dotnet restore "Orders/Orders.API/Orders.API.csproj"
COPY ["Orders/", "Orders/"]

COPY ["EventBus/", "EventBus/"]
COPY ["EventBus.AspNetCore/", "EventBus.AspNetCore/"]
COPY ["EventBus.RabbitMQ/", "EventBus.RabbitMQ/"]
COPY ["EventBus.RabbitMQ.AspNetCore/", "EventBus.RabbitMQ.AspNetCore/"]

COPY ["IntegrationEventLog/", "IntegrationEventLog/"]

RUN dotnet publish "Orders/Orders.API/Orders.API.csproj" -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Orders.API.dll"]
