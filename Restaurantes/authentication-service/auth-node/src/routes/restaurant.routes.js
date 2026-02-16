import express from "express";
import {
    createRestaurant,
    getRestaurants,
    deleteRestaurant
} from "../controllers/restaurant.controller.js";

const router = express.Router();

router.post("/", createRestaurant);
router.get("/", getRestaurants);
router.delete("/:id", deleteRestaurant);

export default router;
