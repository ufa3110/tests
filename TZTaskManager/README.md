# TZ Task Manager

API для управления задачами на базе ASP.NET Core 8.0 и PostgreSQL.

## Быстрый запуск

1. Запустите PostgreSQL контейнер:
   ```powershell
   .\docker\docker-start.ps1
   ```
   или для Linux/Mac:
   ```bash
   ./docker/docker-start.sh
   ```

2. Запустите приложение:
   ```bash
   dotnet run
   ```

3. Откройте Swagger UI: `https://localhost:5001/swagger`

Миграции применяются автоматически при старте приложения.

## Архитектурные решения

### Clean Architecture

Проект организован по принципам Clean Architecture с разделением на слои:

- **Domain** - доменный слой с бизнес-сущностями и интерфейсами
- **Application** - слой приложения с бизнес-логикой, DTO, валидацией и маппингом
- **Infrastructure** - инфраструктурный слой с реализацией доступа к данным
- **Presentation** - слой представления с API контроллерами и middleware

### Применённые паттерны

- **Repository Pattern** - абстракция доступа к данным через `IRepository<T>`
- **Unit of Work** - управление транзакциями через `IUnitOfWork`
- **Service Layer** - бизнес-логика инкапсулирована в сервисах приложения
- **DTO Pattern** - разделение доменных моделей и моделей передачи данных

### Выбор инструментов
#### AutoMapper 12.0
- Автоматический маппинг между сущностями и DTO
- Упрощает преобразование данных между слоями

#### FluentValidation 11.0
- Декларативная валидация входных данных
- Интеграция с ASP.NET Core для автоматической валидации

## API Endpoints

- `GET /api/tasks` - получить все задачи
- `GET /api/tasks/{id}` - получить задачу по ID
- `POST /api/tasks` - создать задачу
- `PUT /api/tasks/{id}` - обновить задачу
- `DELETE /api/tasks/{id}` - удалить задачу

Подробная документация доступна в Swagger UI.

## Остановка

Для остановки PostgreSQL контейнера:
```powershell
.\docker\docker-stop.ps1
```
или
```bash
./docker/docker-stop.sh
```

