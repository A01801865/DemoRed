const express = require("express");
const cors = require("cors");

const app = express();

app.use(cors());
app.use(express.json());

app.post("/login", (req, res) => {
  const { username, password } = req.body;

  if (!username || !password) {
    return res.json({
      success: false,
      message: "Datos incompletos"
    });
  }

  if (username === "admin" && password === "1234") {
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
});

app.listen(3000, () => {
  console.log("Servidor corriendo en http://localhost:3000");
});