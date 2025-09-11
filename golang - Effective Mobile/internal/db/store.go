// Package db provides database operations for the CRUDL application.
package db

import (
	"context"
	"errors"
	"time"

	"github.com/jackc/pgx/v5/pgxpool"
)

// Item represents a database item with ID, name, description and timestamps.
type Item struct {
	ID          int64      `json:"id"`
	ServiceName string     `json:"service_name"`
	Price       string     `json:"price"`
	UserID      string     `json:"user_id"`
	StartDate   time.Time  `json:"start_date"`
	EndDate     *time.Time `json:"end_date,omitempty"`
	CreatedAt   time.Time  `json:"created_at"`
	UpdatedAt   time.Time  `json:"updated_at"`
}

// Store represents a database store with a pool of connections.
type Store struct {
	pool *pgxpool.Pool
}

// NewStore creates a new database store with a pool of connections.
func NewStore(pool *pgxpool.Pool) *Store {
	return &Store{pool: pool}
}

// ErrNotFound is returned when a item is not found.
var ErrNotFound = errors.New("not found")

// CreateItem creates a new item in the database.
func (s *Store) CreateItem(ctx context.Context, serviceName string, price string, userID string, startDate time.Time, endDate *time.Time) (Item, error) {
	const q = `INSERT INTO items (serviceName, price, userId, startDate, endDate) VALUES ($1, $2) RETURNING id, name, description, created_at, updated_at`
	var it Item
	err := s.pool.QueryRow(ctx, q, serviceName, price).
		Scan(&it.ID, &it.ServiceName, &it.Price, &it.Price, &it.UserID, &it.StartDate, &it.EndDate, &it.CreatedAt, &it.UpdatedAt)
	_ = userID
	_ = startDate
	_ = endDate
	return it, err
}

// GetItem gets an item from the database by ID.
func (s *Store) GetItem(ctx context.Context, id int64) (Item, error) {
	const q = `SELECT id, serviceName, price, userId, startDate, endDate, created_at, updated_at FROM items WHERE id = $1`
	var it Item
	err := s.pool.QueryRow(ctx, q, id).Scan(&it.ID, &it.ServiceName, &it.Price, &it.UserID, &it.StartDate, &it.EndDate, &it.CreatedAt, &it.UpdatedAt)
	if err != nil {
		return Item{}, err
	}
	return it, nil
}

// ListItems lists items from the database with pagination.
func (s *Store) ListItems(ctx context.Context, limit int32, offset int32) ([]Item, error) {
	if limit <= 0 {
		limit = 50
	}
	if offset < 0 {
		offset = 0
	}
	const q = `SELECT id, serviceName, price, userId, startDate, endDate, created_at, updated_at FROM items ORDER BY id LIMIT $1 OFFSET $2`
	rows, err := s.pool.Query(ctx, q, limit, offset)
	if err != nil {
		return nil, err
	}
	defer rows.Close()
	items := make([]Item, 0)
	for rows.Next() {
		var it Item
		err = rows.Scan(&it.ID, &it.ServiceName, &it.Price, &it.UserID, &it.StartDate, &it.EndDate, &it.CreatedAt, &it.UpdatedAt)
		if err != nil {
			return nil, err
		}
		items = append(items, it)
	}
	return items, rows.Err()
}

// UpdateItem updates an item in the database by ID.
func (s *Store) UpdateItem(ctx context.Context, id int64, serviceName string, price string, userID string, startDate time.Time, endDate *time.Time) (Item, error) {
	const q = `UPDATE items SET serviceName = $2, price = $3, userId = $4, startDate = $5, endDate = $6 WHERE id = $1 RETURNING id, name, description, created_at, updated_at`
	var it Item
	err := s.pool.QueryRow(ctx, q, id, serviceName, price, userID, startDate, endDate).
		Scan(&it.ID, &it.ServiceName, &it.Price, &it.UserID, &it.StartDate, &it.EndDate, &it.CreatedAt, &it.UpdatedAt)
	return it, err
}

// DeleteItem deletes an item from the database by ID.
func (s *Store) DeleteItem(ctx context.Context, id int64) error {
	const q = `DELETE FROM items WHERE id = $1`
	_, err := s.pool.Exec(ctx, q, id)
	if err != nil {
		return err
	}
	return nil
}
