CREATE DATABASE IF NOT EXISTS game_db;
USE game_db;

-- =========================
-- TABLA DE USUARIOS
-- =========================
CREATE TABLE IF NOT EXISTS users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(50) NOT NULL UNIQUE,
  password VARCHAR(255) NOT NULL
);

-- =========================
-- TABLA DE SESIONES
-- =========================
CREATE TABLE IF NOT EXISTS sessions (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT,
  login_time DATETIME,
  logout_time DATETIME,
  FOREIGN KEY (user_id) REFERENCES users(id)
);