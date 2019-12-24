FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY Orders.API/Orders.API.csproj Orders.API/
COPY Orders.API/NuGet.Config Orders.API/
COPY Orders.Domain/Orders.Domain.csproj Orders.Domain/
COPY Orders.Infrastructure/Orders.Infrastructure.csproj Orders.Infrastructure/

RUN dotnet restore Orders.API/Orders.API.csproj
COPY . .

FROM build AS publish
RUN dotnet publish Orders.API/Orders.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Orders.API.dll"]