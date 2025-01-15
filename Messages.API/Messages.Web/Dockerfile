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
COPY ["Messages.API/Messages.Web/Messages.Web.csproj", "Messages.API/Messages.Web/"]
COPY ["Messages.API/Messages.Application/Messages.Application.csproj", "Messages.API/Messages.Application/"]
COPY ["Share/Application/Application.csproj", "Share/Application/"]
COPY ["Messages.API/Messages.Domain/Messages.Domain.csproj", "Messages.API/Messages.Domain/"]
COPY ["Share/Domain/Domain.csproj", "Share/Domain/"]
COPY ["Messages.API/Messages.Infrastructure/Messages.Infrastructure.csproj", "Messages.API/Messages.Infrastructure/"]
COPY ["Share/Infrastructure/Infrastructure.csproj", "Share/Infrastructure/"]
RUN dotnet restore "./Messages.API/Messages.Web/Messages.Web.csproj"
COPY . .
WORKDIR "/src/Messages.API/Messages.Web"
RUN dotnet build "./Messages.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Messages.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messages.Web.dll"]