FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY BFF/BFF.csproj .
COPY NuGet.Config .

RUN dotnet restore BFF.csproj
COPY BFF .

FROM build AS publish
RUN dotnet publish BFF.csproj -c Release -o /app /p:GenerateCode=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY certs/users.crt certs/orders.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "BFF.dll"]
