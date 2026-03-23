const express = require("express");
const router = express.Router();
const controller = require("../controllers/tableController");

router.post("/", controller.createTable);
router.get("/", controller.getTables);

module.exports = router;