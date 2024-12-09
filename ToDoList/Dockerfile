# Этот этап используется для выполнения приложения в рабочем режиме
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Этот этап используется для сборки и тестирования с .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем проект и восстанавливаем зависимости
COPY ["ToDoList/ToDoList.csproj", "ToDoList/"]
RUN dotnet restore "ToDoList/ToDoList.csproj"
COPY . .

# Сборка проекта
WORKDIR "/src/ToDoList"
RUN dotnet build "ToDoList.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикуем проект
FROM build AS publish
RUN dotnet publish "ToDoList.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется для выполнения приложения
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoList.dll"]
