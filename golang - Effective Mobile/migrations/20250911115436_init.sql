-- +goose Up
-- +goose StatementBegin
CREATE TABLE IF NOT EXISTS items (
    id SERIAL PRIMARY KEY,
    ServiceName TEXT NOT NULL,
    Price TEXT NOT NULL DEFAULT '',
    UserID TEXT NOT NULL DEFAULT '',
    StartDate TIMESTAMPTZ NOT NULL DEFAULT now(),
    EndDate TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
-- +goose StatementEnd

-- +goose Down
-- +goose StatementBegin
DROP TABLE IF EXISTS items;
-- +goose StatementEnd
