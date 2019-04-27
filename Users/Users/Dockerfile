FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY Users.csproj .

RUN dotnet restore Users.csproj
COPY . .

RUN dotnet build Users.csproj -c Release -o /app --no-restore

FROM build AS publish
RUN dotnet publish Users.csproj -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Users.dll"]