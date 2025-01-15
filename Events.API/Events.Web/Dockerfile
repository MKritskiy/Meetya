# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Events.API/Events.Web/Events.Web.csproj", "Events.API/Events.Web/"]
COPY ["Events.API/Events.Domain/Events.Domain.csproj", "Events.API/Events.Domain/"]
COPY ["Share/Domain/Domain.csproj", "Share/Domain/"]
COPY ["Events.API/Events.Infrastructure/Events.Infrastructure.csproj", "Events.API/Events.Infrastructure/"]
COPY ["Share/Application/Application.csproj", "Share/Application/"]
COPY ["Share/Infrastructure/Infrastructure.csproj", "Share/Infrastructure/"]
COPY ["Events.API/Events.Application/Events.Application.csproj", "Events.API/Events.Application/"]
RUN dotnet restore "./Events.API/Events.Web/Events.Web.csproj"
COPY . .
WORKDIR "/src/Events.API/Events.Web"
RUN dotnet build "./Events.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Events.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Events.Web.dll"]