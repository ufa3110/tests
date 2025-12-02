#!/bin/bash
# Bash скрипт для запуска PostgreSQL контейнера

echo "Запуск PostgreSQL контейнера..."
docker compose up -d

if [ $? -eq 0 ]; then
    echo "PostgreSQL контейнер успешно запущен!"
    echo "Подключение: Host=localhost;Port=5432;Database=TZTaskManagerDb;Username=postgres;Password=postgres"
    echo ""
    echo "Для применения миграций выполните:"
    echo "dotnet ef database update"
else
    echo "Ошибка при запуске контейнера!"
    exit 1
fi

