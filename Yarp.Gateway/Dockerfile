# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 443  # Добавляем порт для HTTPS

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Yarp.Gateway/Yarp.Gateway.csproj", "Yarp.Gateway/"]
RUN dotnet restore "./Yarp.Gateway/Yarp.Gateway.csproj"
COPY . .
WORKDIR "/src/Yarp.Gateway"
RUN dotnet build "./Yarp.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Yarp.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY /etc/letsencrypt/live/when-and-where.ru/ /app/certs  # Копируем папку с сертификатом
ENTRYPOINT ["dotnet", "Yarp.Gateway.dll"]