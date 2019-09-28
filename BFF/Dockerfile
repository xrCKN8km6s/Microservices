FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src

COPY BFF/BFF.csproj BFF/

COPY Clients.Common/Clients.Common.csproj Clients.Common/

COPY Users/Users.Client/Users.Client.csproj Users/Users.Client/
COPY Orders/Orders.Client/Orders.Client.csproj Orders/Orders.Client/

RUN dotnet restore BFF/BFF.csproj
COPY . .

FROM build AS publish
RUN dotnet publish BFF/BFF.csproj -c Release -o /app /p:GenerateCode=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BFF.dll"]