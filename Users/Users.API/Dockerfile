FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
COPY ["certs/rootCA.crt", "/usr/local/share/ca-certificates/"]
RUN update-ca-certificates
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ARG GPR_NuGet_Passwd
WORKDIR /src
COPY ["Users/Users.API/Users.API.csproj", "./Users/Users.API/"]
COPY ["Users/Users.UnitTests/Users.UnitTests.csproj", "./Users/Users.UnitTests/"]
#COPY ["NuGet.Config", "."]
RUN dotnet restore "Users/Users.API/Users.API.csproj"
RUN dotnet restore "Users/Users.UnitTests/Users.UnitTests.csproj"
COPY ["Users/", "./Users/"]
COPY ["proto/", "./proto/"]
RUN dotnet publish "Users/Users.API/Users.API.csproj" -c Release -o /app/publish --no-restore

FROM build as unittests
WORKDIR /src/Users.UnitTests

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Users.API.dll"]
