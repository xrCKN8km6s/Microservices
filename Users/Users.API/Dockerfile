FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY Users/Users.API/Users.API.csproj .
COPY NuGet.Config .

RUN dotnet restore Users.API.csproj
COPY Users/Users.API/ .

FROM build AS publish
RUN dotnet publish Users.API.csproj -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Users.API.dll"]
