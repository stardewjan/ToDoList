# Базовый образ для .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копирование проектных файлов
COPY ["ToDoList.csproj", "./"]
COPY ["../ToDoList.Tests/ToDoList.Tests.csproj", "../ToDoList.Tests/"]

# Восстановление зависимостей
RUN dotnet restore "./ToDoList.csproj"
RUN dotnet restore "../ToDoList.Tests/ToDoList.Tests.csproj"

# Копирование всего кода
COPY . .
COPY ../ToDoList.Tests/ ../ToDoList.Tests/

# Сборка проекта
RUN dotnet build "ToDoList.csproj" -c Release -o /app/build

# Публикация проекта
FROM build AS publish
RUN dotnet publish "ToDoList.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoList.dll"]