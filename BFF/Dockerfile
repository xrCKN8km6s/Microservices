FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
COPY ["certs/rootCA.crt", "/usr/local/share/ca-certificates/"]
RUN  update-ca-certificates
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
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
RUN dotnet publish "BFF/BFF.csproj" -c Release -o /app/publish --no-restore /p:GenerateCode=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BFF.dll"]
