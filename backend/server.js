const connection = require("./db");
const express = require("express");
const cors = require("cors");
const bcrypt = require("bcrypt");

const app = express();

app.use(cors());
app.use(express.json());

app.post("/register", async (req, res) => {
  const { username, password } = req.body;

  if (!username || !password) {
    return res.json({ success: false, message: "Faltan datos" });
  }

  const hashedPassword = await bcrypt.hash(password, 10);

  connection.query(
    "INSERT INTO users (username, password) VALUES (?, ?)",
    [username, hashedPassword],
    (err) => {
      if (err) {
        return res.json({ success: false, message: "Usuario ya existe" });
      }
      res.json({ success: true });
    }
  );
});

app.post("/login", (req, res) => {
  const { username, password } = req.body;

  if (!username || !password) {
    return res.json({ success: false, message: "Faltan datos" });
  }

  connection.query(
    "SELECT * FROM users WHERE username = ?",
    [username],
    async (err, results) => {

      if (err) {
        return res.json({ success: false, message: "Error servidor" });
      }

      if (results.length === 0) {
        return res.json({ success: false, message: "Usuario no existe" });
      }

      const user = results[0];

      const match = await bcrypt.compare(password, user.password);

      if (!match) {
        return res.json({ success: false, message: "Credenciales incorrectas" });
      }

      connection.query(
        "INSERT INTO sessions (user_id, login_time) VALUES (?, NOW())",
        [user.id],
        (err2) => {
          if (err2) {
            return res.json({ success: false, message: "Error sesión" });
          }

          res.json({
            success: true,
            userId: user.id
          });
        }
      );
    }
  );
});

app.post("/logout", (req, res) => {
  const { userId } = req.body;

  if (!userId) {
    return res.json({ success: false });
  }

  connection.query(
    "UPDATE sessions SET logout_time = NOW() WHERE user_id = ? AND logout_time IS NULL",
    [userId],
    (err) => {
      if (err) {
        return res.json({ success: false });
      }

      res.json({ success: true });
    }
  );
});

app.listen(3000, () => {
  console.log("Servidor corriendo en http://localhost:3000");
});