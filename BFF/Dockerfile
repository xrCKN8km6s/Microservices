FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
COPY ["certs/rootCA.crt", "/usr/local/share/ca-certificates/"]
RUN update-ca-certificates
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG GPR_NuGet_Passwd
WORKDIR /src
COPY ["BFF/BFF.csproj", "BFF/"]
#COPY ["NuGet.Config", "."]

COPY ["Clients.Common/Clients.Common.csproj", "Clients.Common/"]

COPY ["Users/Users.Client/Users.Client.csproj", "Users/Users.Client/"]
COPY ["Orders/Orders.Client/Orders.Client.csproj", "Orders/Orders.Client/"]

RUN dotnet restore "BFF/BFF.csproj"
COPY ["BFF/", "BFF/"]
COPY ["Clients.Common/", "Clients.Common/"]
COPY ["Users/Users.Client/", "Users/Users.Client/"]
COPY ["Orders/Orders.Client/", "Orders/Orders.Client/"]
COPY ["proto/", "./proto/"]
RUN dotnet publish "BFF/BFF.csproj" -c Release -o /app/publish --no-restore /p:GenerateClientCode=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BFF.dll"]
