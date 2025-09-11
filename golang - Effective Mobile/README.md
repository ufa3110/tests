для запуска миграций нужно выполнить следующие команды:
go install github.com/pressly/goose/v3/cmd/goose@latest
goose up
можно было их в мейкфайл засунуть, ну или хотябы в докерфайл типа
# Migrations
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go install github.com/pressly/goose/v3/cmd/goose@latest
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 goose up