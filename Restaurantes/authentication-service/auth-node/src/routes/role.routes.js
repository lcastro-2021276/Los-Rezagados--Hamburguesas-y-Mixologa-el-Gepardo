import { Router } from "express";
import { createRole, getRoles } from "../controllers/role.controller.js";

const router = Router();

router.post("/", createRole);
router.get("/", getRoles);

export default router;
