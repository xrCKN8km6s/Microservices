FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
COPY ["certs/users.crt", "certs/orders.crt", "/usr/local/share/ca-certificates/"]
RUN  update-ca-certificates
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
ARG GPR_NuGet_Passwd
WORKDIR /src
COPY ["BFF/BFF.csproj", "."]
COPY ["NuGet.Config", "."]
RUN dotnet restore "BFF.csproj"
COPY ["BFF/", "."]
RUN dotnet publish "BFF.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BFF.dll"]
