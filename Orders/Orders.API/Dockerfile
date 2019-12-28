FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY Orders/Orders.API/Orders.API.csproj Orders.API/
COPY Orders/Orders.Domain/Orders.Domain.csproj Orders.Domain/
COPY Orders/Orders.Infrastructure/Orders.Infrastructure.csproj Orders.Infrastructure/

COPY NuGet.Config Orders.API/

RUN dotnet restore Orders.API/Orders.API.csproj
COPY Orders .

FROM build AS publish
RUN dotnet publish Orders.API/Orders.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Orders.API.dll"]
