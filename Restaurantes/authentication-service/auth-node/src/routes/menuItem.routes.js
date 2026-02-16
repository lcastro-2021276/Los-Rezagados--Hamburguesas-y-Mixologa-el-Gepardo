import express from "express";
import {
    createMenuItem,
    getMenuItems,
    deleteMenuItem
} from "../controllers/menuItem.controller.js";

const router = express.Router();

router.post("/", createMenuItem);
router.get("/", getMenuItems);
router.delete("/:id", deleteMenuItem);

export default router;
