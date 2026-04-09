const connection = require("./db");

const express = require("express");
const cors = require("cors");

const app = express();

app.use(cors());
app.use(express.json());

app.post("/login", (req, res) => {
  const { username, password } = req.body;

  connection.query(
    "SELECT * FROM users WHERE username = ? AND password = ?",
    [username, password],
    (err, results) => {

      if (err) {
        return res.json({ success: false, message: "Error servidor" });
      }

      if (results.length > 0) {

        const userId = results[0].id;

        // 🔥 Guardar inicio de sesión
        connection.query(
          "INSERT INTO sessions (user_id, login_time) VALUES (?, NOW())",
          [userId]
        );

        return res.json({
          success: true,
          message: "Login correcto"
        });

      } else {
        return res.json({
          success: false,
          message: "Credenciales incorrectas"
        });
      }
    }
  );
});

app.listen(3000, () => {
  console.log("Servidor corriendo en http://localhost:3000");
});   