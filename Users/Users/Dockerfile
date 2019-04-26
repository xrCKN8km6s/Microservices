FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.3 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY Users.csproj ./
RUN dotnet restore Users.csproj
COPY . .

RUN dotnet build Users.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Users.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Users.dll"]