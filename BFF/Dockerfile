FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
COPY ["certs/users.crt", "certs/orders.crt", "/usr/local/share/ca-certificates/"]
RUN  update-ca-certificates
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BFF/BFF.csproj", "."]
COPY ["NuGet.Config", "."]
RUN dotnet restore "BFF.csproj"
COPY ["BFF/", "."]
RUN dotnet build "BFF.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BFF.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BFF.dll"]
