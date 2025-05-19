FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Coolicky.ContainerPacking.Api/Coolicky.ContainerPacking.Api.csproj", "Coolicky.ContainerPacking.Api/"]
RUN dotnet restore "Coolicky.ContainerPacking.Api/Coolicky.ContainerPacking.Api.csproj"
COPY . .
WORKDIR "/src/Coolicky.ContainerPacking.Api"
RUN dotnet build "./Coolicky.ContainerPacking.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Coolicky.ContainerPacking.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Coolicky.ContainerPacking.Api.dll"]
