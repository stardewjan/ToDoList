# ToDoList

Это приложение для управления задачами, написанное с использованием ASP.NET 8 MVC, Entity Framework и Microsoft SQL Server. Приложение также включает в себя REST API для управления задачами, интеграционные тесты и настроено для CI/CD с использованием Docker на базе Ubuntu 22 LTS.

## Описание

Приложение предоставляет интерфейс для создания, редактирования, удаления задач, а также фильтрацию задач по статусу и поисковому запросу. Пользователи могут отмечать задачи как выполненные через веб-интерфейс или через API.

Основные возможности:
- Просмотр всех задач.
- Фильтрация задач по статусу и поисковому запросу.
- Создание, редактирование и удаление задач.
- Завершение задач с обновлением статуса.
- Получение статистики.

## Архитектура

### Backend:
- **ASP.NET Core 8 MVC**: используется для создания интерфейса и работы с данными.
- **Entity Framework**: для взаимодействия с базой данных.
- **Microsoft SQL Server**: для хранения данных о задачах.

### API:
- Предоставлен REST API для работы с задачами. API поддерживает CRUD операции и фильтрацию задач.

### Docker:
- Контейнеризация приложения с использованием Docker на базе Ubuntu 22 LTS.
- Используется **Docker Compose** для управления зависимыми сервисами (например, база данных).

### CI/CD:
- Конфигурация для автоматического тестирования и деплоя с использованием CI/CD.
- Использование **GitHub Actions** для автоматической сборки, тестирования и деплоя приложения.

### Тестирование
- **Интеграционные тесты**: для проверки взаимодействия с базой данных и внешними сервисами.

### Основные папки и файлы:
- **ToDoList**:
  - `Controllers/`: Контроллеры API для управления задачами.
  - `Data/`: Включает `DatabaseContext` для работы с базой данных.
  - `Models/`: Содержит сущности и DTO.
  - `Views/`: Хранит Razor-страницы для отображения UI.
  - `wwwroot/`: Статические файлы (CSS, JS, изображения).

- **ToDoList.Tests**:
  - `Integration/`: Интеграционные тесты для проверки взаимодействия компонентов приложения.

## Установка и настройка

### Требования
- .NET 8 SDK
- Docker (для контейнеризации)
- Microsoft SQL Server
### Пример строки подключения:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ToDoListDB;User Id=sa;Password=your_password;"
  }
}
```

### Шаги для установки

1. Клонировать репозиторий:

    ```bash
    git clone https://github.com/stardewjan/ToDoList.git
    cd ToDoList
    ```

2. Восстановить зависимости:

    ```bash
    dotnet restore
    ```

3. Запустить приложение локально:

    ```bash
    dotnet run
    ```

4. Для запуска приложения в Docker:

    - Собрать Docker образ:

        ```bash
        docker build -t todolist:dev .
        ```

    - Запустить контейнер:

        ```bash
        docker run -d -p 8080:8080 --name todolist-container todolist:dev
        ```

5. Миграции базы данных:

    - Запустить миграции для создания таблиц в базе данных:

        ```bash
        dotnet ef database update
        ```

## Использование

### Веб-интерфейс:
- Перейдите на [http://localhost:8080](http://localhost:8080) для доступа к веб-интерфейсу.
- Вы можете создавать, редактировать и удалять задачи, а также фильтровать их по статусу и поисковому запросу.

### API:

#### Задачи:
- **GET /api/taskitems** — Получить все задачи.
- **GET /api/taskitems/{id}** — Получить задачу по ID.
- **GET /api/taskitems/filtered** — Получить задачи с фильтрацией по статусу и строке поиска.
- **POST /api/taskitems** — Создать новую задачу.
- **PUT /api/taskitems/{id}** — Обновить задачу.
- **DELETE /api/taskitems/{id}** — Удалить задачу.
- **POST /api/taskitems/{id}/complete** — Завершить задачу.

#### Статистика:
- **GET /api/home/statistics** — Получить статистику по задачам (общее количество задач, выполненные и невыполненные задачи).

### Примеры запросов и ответов:

#### POST /api/taskitems
**Запрос:**
```json
{
  "title": "New Task",
  "description": "This is a new task.",
  "isCompleted": false
}
```
**Ответ:**
```json
{
  "id": 1,
  "title": "New Task",
  "description": "This is a new task.",
  "isCompleted": false,
  "endDate": null
}
```
#### GET /api/taskitems
**Ответ:**
```json
  {
    "id": 1,
    "title": "New Task",
    "description": "This is a new task.",
    "isCompleted": false,
    "endDate": null
  },
  {
    "id": 2,
    "title": "Another Task",
    "description": "This is another task.",
    "isCompleted": true,
    "endDate": "2024-12-01T12:34:56"
  }
```
