FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
ARG GPR_NuGet_Passwd
WORKDIR /src
COPY ["Users/Users.API/Users.API.csproj", "./Users.API/"]
COPY ["Users/Users.UnitTests/Users.UnitTests.csproj", "./Users.UnitTests/"]
COPY ["NuGet.Config", "."]
RUN dotnet restore "Users.API/Users.API.csproj"
RUN dotnet restore "Users.UnitTests/Users.UnitTests.csproj"
COPY ["Users/", "."]
RUN dotnet publish "Users.API/Users.API.csproj" -c Release -o /app/publish --no-restore

FROM build as unittests
WORKDIR /src/Users.UnitTests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.API.dll"]
