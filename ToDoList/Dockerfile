# Базовый образ для выполнения приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Этап сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы решения
COPY ["ToDoList/ToDoList.sln", "ToDoList/"]
COPY ["ToDoList/ToDoList.csproj", "ToDoList/"]
COPY ["ToDoList.Tests/ToDoList.Tests.csproj", "ToDoList.Tests/"]

# Восстанавливаем зависимости
RUN dotnet restore "ToDoList/ToDoList.sln"

# Копируем оставшиеся файлы и выполняем сборку
COPY . .
WORKDIR "/src/ToDoList"
RUN dotnet build -c Release -o /app/build

# Этап тестирования
FROM build AS test
WORKDIR /src/ToDoList.Tests
RUN dotnet test --no-build --logger "console;verbosity=detailed"

# Финальный этап для запуска приложения
FROM base AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "ToDoList.dll"]
