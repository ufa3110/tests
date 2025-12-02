# PowerShell скрипт для остановки PostgreSQL контейнера
Write-Host "Остановка PostgreSQL контейнера..." -ForegroundColor Yellow
docker compose down

if ($LASTEXITCODE -eq 0) {
    Write-Host "PostgreSQL контейнер остановлен!" -ForegroundColor Green
} else {
    Write-Host "Ошибка при остановке контейнера!" -ForegroundColor Red
}

