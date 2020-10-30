FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
COPY ["certs/rootCA.crt", "/usr/local/share/ca-certificates/"]
RUN update-ca-certificates
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG GPR_NuGet_Passwd
WORKDIR /src
COPY ["Orders/Orders.API/Orders.API.csproj", "Orders/Orders.API/"]
COPY ["Orders/Orders.Domain/Orders.Domain.csproj", "Orders/Orders.Domain/"]
COPY ["Orders/Orders.Infrastructure/Orders.Infrastructure.csproj", "Orders/Orders.Infrastructure/"]

COPY ["EventBus/EventBus/EventBus.csproj", "EventBus/EventBus/"]
COPY ["EventBus/EventBus.AspNetCore/EventBus.AspNetCore.csproj", "EventBus/EventBus.AspNetCore/"]
COPY ["EventBus/EventBus.Redis/EventBus.Redis.csproj", "EventBus/EventBus.Redis/"]
COPY ["EventBus/EventBus.Redis.AspNetCore/EventBus.Redis.AspNetCore.csproj", "EventBus/EventBus.Redis.AspNetCore/"]

COPY ["IntegrationEventLog/IntegrationEventLog.csproj", "IntegrationEventLog/"]
#COPY ["NuGet.Config", "Orders.API/"]
RUN dotnet restore "Orders/Orders.API/Orders.API.csproj"
COPY ["Orders/", "Orders/"]

COPY ["EventBus/EventBus/", "EventBus/EventBus/"]
COPY ["EventBus/EventBus.AspNetCore/", "EventBus/EventBus.AspNetCore/"]
COPY ["EventBus/EventBus.Redis/", "EventBus/EventBus.Redis/"]
COPY ["EventBus/EventBus.Redis.AspNetCore/", "EventBus/EventBus.Redis.AspNetCore/"]

COPY ["IntegrationEventLog/", "IntegrationEventLog/"]

RUN dotnet publish "Orders/Orders.API/Orders.API.csproj" -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Orders.API.dll"]
