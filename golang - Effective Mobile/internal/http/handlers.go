// Package api provides HTTP handlers for the CRUDL application.
package api

import (
	"encoding/json"
	"net/http"
	"strconv"
	"time"

	"test/internal/db"

	"github.com/go-chi/chi/v5"
	httpSwagger "github.com/swaggo/http-swagger"
)

// Server represents a HTTP server with a store.
type Server struct {
	store *db.Store
}

// NewServer creates a new HTTP server with a store.
func NewServer(store *db.Store) *Server {
	return &Server{store: store}
}

// Router returns a new HTTP router with the server's routes.
func (s *Server) Router() http.Handler {
	r := chi.NewRouter()

	// Swagger UI at /swagger/index.html
	r.Get("/swagger/*", httpSwagger.WrapHandler)

	r.Post("/items", s.handleCreateItem)
	r.Get("/items/{id}", s.handleGetItem)
	r.Get("/items", s.handleListItems)
	r.Put("/items/{id}", s.handleUpdateItem)
	r.Delete("/items/{id}", s.handleDeleteItem)

	return r
}

// createItemRequest represents a request to create an item.
type createItemRequest struct {
	ServiceName string     `json:"service_name"`
	Price       string     `json:"price"`
	UserId      string     `json:"user_id"`
	StartDate   time.Time  `json:"start_date"`
	EndDate     *time.Time `json:"end_date,omitempty"`
}

// handleCreateItem handles a request to create an item.
// @Summary Create item
// @Description Create a new item
// @Tags items
// @Accept json
// @Produce json
// @Param payload body createItemRequest true "Item payload"
// @Success 201 {object} db.Item
// @Failure 400 {string} string "invalid json"
// @Failure 500 {string} string
// @Router /items [post]
func (s *Server) handleCreateItem(w http.ResponseWriter, r *http.Request) {
	var req createItemRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "invalid json", http.StatusBadRequest)
		return
	}
	it, err := s.store.CreateItem(r.Context(), req.ServiceName, req.Price, req.UserId, req.StartDate, req.EndDate)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	_ = json.NewEncoder(w).Encode(it)
}

//потому что логики особо нет, достаточно тут всё и описать*

// handleGetItem handles a request to get an item.
// @Summary Get item
// @Description Get item by ID
// @Tags items
// @Produce json
// @Param id path int true "Item ID"
// @Success 200 {object} db.Item
// @Failure 400 {string} string "invalid id"
// @Failure 404 {string} string
// @Router /items/{id} [get]
func (s *Server) handleGetItem(w http.ResponseWriter, r *http.Request) {
	idStr := chi.URLParam(r, "id")
	id, err := strconv.ParseInt(idStr, 10, 64)
	if err != nil {
		http.Error(w, "invalid id", http.StatusBadRequest)
		return
	}
	it, err := s.store.GetItem(r.Context(), id)
	if err != nil {
		http.Error(w, err.Error(), http.StatusNotFound)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	err = json.NewEncoder(w).Encode(it)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
}

// handleListItems handles a request to list items.
// @Summary List items
// @Description List items with pagination
// @Tags items
// @Produce json
// @Param limit query int false "Limit" default(50)
// @Param offset query int false "Offset" default(0)
// @Success 200 {array} db.Item
// @Failure 500 {string} string
// @Router /items [get]
func (s *Server) handleListItems(w http.ResponseWriter, r *http.Request) {
	q := r.URL.Query()
	limit, _ := strconv.ParseInt(q.Get("limit"), 10, 32)
	offset, _ := strconv.ParseInt(q.Get("offset"), 10, 32)
	items, err := s.store.ListItems(r.Context(), int32(limit), int32(offset))
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	_ = json.NewEncoder(w).Encode(items)
}

// handleUpdateItem handles a request to update an item.
// @Summary Update item
// @Description Update an existing item by ID
// @Tags items
// @Accept json
// @Produce json
// @Param id path int true "Item ID"
// @Param payload body createItemRequest true "Item payload"
// @Success 200 {object} db.Item
// @Failure 400 {string} string "invalid id"
// @Failure 500 {string} string
// @Router /items/{id} [put]
func (s *Server) handleUpdateItem(w http.ResponseWriter, r *http.Request) {
	idStr := chi.URLParam(r, "id")
	id, err := strconv.ParseInt(idStr, 10, 64)
	if err != nil {
		http.Error(w, "invalid id", http.StatusBadRequest)
		return
	}
	var req createItemRequest
	err = json.NewDecoder(r.Body).Decode(&req)
	if err != nil {
		http.Error(w, "invalid json", http.StatusBadRequest)
		return
	}
	it, err := s.store.UpdateItem(r.Context(), id, req.ServiceName, req.Price, req.UserId, req.StartDate, req.EndDate)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	_ = json.NewEncoder(w).Encode(it)
}

// handleDeleteItem handles a request to delete an item.
// @Summary Delete item
// @Description Delete item by ID
// @Tags items
// @Param id path int true "Item ID"
// @Success 204 {string} string "No Content"
// @Failure 400 {string} string "invalid id"
// @Failure 404 {string} string
// @Router /items/{id} [delete]
func (s *Server) handleDeleteItem(w http.ResponseWriter, r *http.Request) {
	idStr := chi.URLParam(r, "id")
	id, err := strconv.ParseInt(idStr, 10, 64)
	if err != nil {
		http.Error(w, "invalid id", http.StatusBadRequest)
		return
	}
	err = s.store.DeleteItem(r.Context(), id)
	if err != nil {
		http.Error(w, err.Error(), http.StatusNotFound)
		return
	}
	w.WriteHeader(http.StatusNoContent)
}
