const mysql = require("mysql2");

const connection = mysql.createConnection({
  host: "game-db.c4wm7o9edpvc.us-east-1.rds.amazonaws.com",
  user: "admin",
  password: "supernumberland",
  database: "game_db"
});

connection.connect((err) => {
  if (err) {
    console.error("Error conectando:", err);
  } else {
    console.log("Conectado a AWS RDS");
  }
});

module.exports = connection;