FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cardpecker.Api/Cardpecker.Api.csproj", "Cardpecker.Api/"]
COPY ["Cardpecker.ServiceDefaults/Cardpecker.ServiceDefaults.csproj", "Cardpecker.ServiceDefaults/"]
COPY ["MtgJson.Importer/MtgJson.Importer.csproj", "MtgJson.Importer/"]
COPY ["ScryfallApi.Client/ScryfallApi.Client.csproj", "ScryfallApi.Client/"]
RUN dotnet restore "Cardpecker.Api/Cardpecker.Api.csproj"
COPY . .
WORKDIR "/src/Cardpecker.Api"
RUN dotnet build "./Cardpecker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Cardpecker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cardpecker.Api.dll"]
