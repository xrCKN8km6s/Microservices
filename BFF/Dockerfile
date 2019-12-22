FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY BFF/BFF.csproj BFF/

COPY NuGet.Config BFF/

RUN dotnet restore BFF/BFF.csproj
COPY . .

FROM build AS publish
RUN dotnet publish BFF/BFF.csproj -c Release -o /app /p:GenerateCode=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BFF.dll"]