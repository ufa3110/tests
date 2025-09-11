// Package main provides the HTTP API service for the CRUDL application.
package main

import (
	"context"
	"fmt"
	"log/slog"
	"net/http"
	"os"

	"test/internal/config"
	"test/internal/db"
	api "test/internal/http"
	_ "test/internal/http/docs"

	"github.com/jackc/pgx/v5/pgxpool"
)

func main() {
	logger := slog.New(slog.NewTextHandler(os.Stdout, nil))

	defer func() {
		if r := recover(); r != nil {
			logger.Error(fmt.Sprintf("Recovered. Error:\n%v", r))
		}
	}()

	cfg, err := config.Load()
	if err != nil {
		logger.Error(fmt.Sprintf("failed to load config: %v", err))
		os.Exit(1)
	}

	pool, err := pgxpool.New(context.Background(), cfg.DatabaseURL)
	if err != nil {
		logger.Error(fmt.Sprintf("failed to create pool: %v", err))
		os.Exit(1)
	}
	defer pool.Close()

	ctx, cancel := context.WithTimeout(context.Background(), cfg.Timeout)
	defer cancel()
	err = pool.Ping(ctx)
	if err != nil {
		logger.Error(fmt.Sprintf("failed to ping db: %v", err))
		os.Exit(1)
	}

	store := db.NewStore(pool)
	srv := api.NewServer(store)

	logger.Info(fmt.Sprintf("listening on %s", cfg.Addr))

	server := &http.Server{
		Addr:         cfg.Addr,
		Handler:      srv.Router(),
		ReadTimeout:  cfg.Timeout,
		WriteTimeout: cfg.Timeout,
	}

	err = server.ListenAndServe()
	if err != nil {
		logger.Error(fmt.Sprintf("failed to listen and serve: %v", err))
		os.Exit(1)
	}
}
