FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY Users.API.csproj .

RUN dotnet restore Users.API.csproj
COPY . .

FROM build AS publish
RUN dotnet publish Users.API.csproj -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Users.API.dll"]