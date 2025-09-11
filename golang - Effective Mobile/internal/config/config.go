// Package config provides configuration management using Viper.
package config

import (
	"time"

	"github.com/spf13/viper"
)

// Config holds all configuration for the application.
type Config struct {
	DatabaseURL string        `mapstructure:"database_url"`
	Addr        string        `mapstructure:"addr"`
	Timeout     time.Duration `mapstructure:"timeout"`
}

// Load loads configuration from environment variables and sets defaults.
func Load() (*Config, error) {
	viper.SetConfigName("app")
	viper.SetConfigType("env")
	viper.AddConfigPath(".")

	_ = viper.ReadInConfig()
	var config Config
	if err := viper.Unmarshal(&config); err != nil {
		return nil, err
	}

	return &config, nil
}
