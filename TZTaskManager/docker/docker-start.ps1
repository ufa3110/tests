# PowerShell скрипт для запуска PostgreSQL контейнера
Write-Host "Запуск PostgreSQL контейнера..." -ForegroundColor Green
docker compose up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "PostgreSQL контейнер успешно запущен!" -ForegroundColor Green
    Write-Host "Подключение: Host=localhost;Port=5432;Database=TZTaskManagerDb;Username=postgres;Password=postgres" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Для применения миграций выполните:" -ForegroundColor Yellow
    Write-Host "dotnet ef database update" -ForegroundColor Yellow
} else {
    Write-Host "Ошибка при запуске контейнера!" -ForegroundColor Red
}

