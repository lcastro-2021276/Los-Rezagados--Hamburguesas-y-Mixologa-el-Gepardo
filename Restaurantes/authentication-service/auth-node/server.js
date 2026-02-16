import express from "express";
import mongoose from "mongoose";
import bcrypt from "bcryptjs";
import dotenv from "dotenv";
import jwt from "jsonwebtoken";

dotenv.config();

const app = express();
app.use(express.json());
app.use(express.urlencoded({ extended: true }));


mongoose.connect(process.env.MONGO_URI)
    .then(() => console.log("MongoDB conectado correctamente"))
    .catch(err => console.error("Error de conexión:", err));


const roleSchema = new mongoose.Schema({
    name: { type: String, required: true, unique: true, trim: true, uppercase: true },
    description: { type: String, trim: true, default: "" },
    permissions: {
        type: [String],
        default: [],
        validate: {
            validator: arr =>
                Array.isArray(arr) &&
                arr.every(p => typeof p === "string" && p.trim() !== ""),
            message: "Los permisos deben ser un array de strings no vacíos"
        }
    }
}, { timestamps: true });

const Role = mongoose.model("Role", roleSchema);


const userSchema = new mongoose.Schema({
    name: { type: String, required: true, minlength: 3 },
    email: { type: String, required: true, unique: true },
    password: { type: String, required: true, minlength: 6 },
    role: { type: mongoose.Schema.Types.ObjectId, ref: "Role", required: true },
    phone: { type: String, trim: true }
}, { timestamps: true });

userSchema.methods.toJSON = function () {
    const obj = this.toObject();
    delete obj.password;
    return obj;
};

const User = mongoose.model("User", userSchema);


const Restaurant = mongoose.model("Restaurant", new mongoose.Schema({
    name: { type: String, required: true },
    address: String,
    phone: String,
    email: String,
    capacity: Number,
    openingHours: String
}, { timestamps: true }));


const Reservation = mongoose.model("Reservation", new mongoose.Schema({
    customerName: { type: String, required: true },
    customerPhone: String,
    customerEmail: String,
    reservationDate: { type: Date, required: true },
    numberOfGuests: Number,
    restaurant: { type: mongoose.Schema.Types.ObjectId, ref: "Restaurant", required: true }
}, { timestamps: true }));


const MenuItem = mongoose.model("MenuItem", new mongoose.Schema({
    name: { type: String, required: true },
    description: String,
    price: { type: Number, required: true },
    restaurant: { type: mongoose.Schema.Types.ObjectId, ref: "Restaurant", required: true }
}, { timestamps: true }));



app.post("/roles", async (req, res) => {
    try {
        const { name, description, permissions } = req.body;

        if (!name || name.trim() === "")
            return res.status(400).json({ message: "El nombre del rol es obligatorio" });

        const roleName = name.trim().toUpperCase();

        const existing = await Role.findOne({ name: roleName });
        if (existing)
            return res.status(400).json({ message: "El rol ya existe" });

        const cleanPermissions = Array.isArray(permissions)
            ? permissions.map(p => p.trim()).filter(p => p !== "")
            : [];

        const role = await Role.create({
            name: roleName,
            description: description || "",
            permissions: cleanPermissions
        });

        res.status(201).json(role);

    } catch (error) {
        res.status(500).json({ message: "Error al crear el rol", error: error.message });
    }
});

app.get("/roles", async (req, res) => {
    try {
        const roles = await Role.find().sort({ createdAt: -1 });
        res.json(roles);
    } catch (error) {
        res.status(500).json({ message: "Error al obtener roles", error: error.message });
    }
});



app.post("/users", async (req, res) => {
    try {
        const { name, email, password, role, phone } = req.body;

        if (!name || !email || !password || !role)
            return res.status(400).json({ message: "Nombre, email, contraseña y rol son obligatorios" });

        if (!mongoose.Types.ObjectId.isValid(role))
            return res.status(400).json({ message: "ID de rol inválido" });

        const roleExists = await Role.findById(role);
        if (!roleExists)
            return res.status(400).json({ message: "El rol no existe" });

        const userExists = await User.findOne({ email });
        if (userExists)
            return res.status(400).json({ message: "El usuario ya existe" });

        const hashedPassword = await bcrypt.hash(password, 10);

        const user = await User.create({
            name,
            email,
            password: hashedPassword,
            role,
            phone
        });

        res.status(201).json(user);

    } catch (error) {
        res.status(500).json({ message: "Error al crear usuario", error: error.message });
    }
});

app.get("/users", async (req, res) => {
    try {
        const users = await User.find().populate("role");
        res.json(users);
    } catch (error) {
        res.status(500).json({ message: "Error al obtener usuarios", error: error.message });
    }
});


    app.post("/login", async (req, res) => {
    try {
        const { email, password } = req.body;

        const user = await User.findOne({ email });

        if (!user) {
        return res.status(404).json({ message: "Usuario no encontrado" });
        }

        const validPassword = await bcrypt.compare(password, user.password);

        if (!validPassword) {
        return res.status(401).json({ message: "Contraseña incorrecta" });
        }

        const token = jwt.sign(
        { id: user._id, role: user.role },
        process.env.JWT_SECRET || "restaurante_secreto",
        { expiresIn: "2h" }
        );

        res.json({
        message: "Login exitoso",
        token,
        });

    } catch (error) {
        res.status(500).json({ message: error.message });
    }
});


app.post("/restaurants", async (req, res) => {
    try {
        if (!req.body.name)
            return res.status(400).json({ message: "El nombre es obligatorio" });

        const restaurant = await Restaurant.create(req.body);
        res.status(201).json(restaurant);

    } catch (error) {
        res.status(400).json({ message: error.message });
    }
});

app.get("/restaurants", async (req, res) => {
    const restaurants = await Restaurant.find();
    res.json(restaurants);
});

app.delete("/restaurants/:id", async (req, res) => {
    if (!mongoose.Types.ObjectId.isValid(req.params.id))
        return res.status(400).json({ message: "ID inválido" });

    const deleted = await Restaurant.findByIdAndDelete(req.params.id);

    if (!deleted)
        return res.status(404).json({ message: "Restaurant no encontrado" });

    res.json({ message: "Restaurant eliminado correctamente" });
});



app.post("/reservations", async (req, res) => {
    try {
        const { customerName, reservationDate, restaurant } = req.body;

        if (!customerName || !reservationDate || !restaurant)
            return res.status(400).json({ message: "Nombre, fecha y restaurante son obligatorios" });

        if (!mongoose.Types.ObjectId.isValid(restaurant))
            return res.status(400).json({ message: "ID de restaurante inválido" });

        const existingRestaurant = await Restaurant.findById(restaurant);
        if (!existingRestaurant)
            return res.status(404).json({ message: "El restaurante no existe" });

        const reservation = await Reservation.create({
            ...req.body,
            reservationDate: new Date(reservationDate)
        });

        res.status(201).json(reservation);

    } catch (error) {
        res.status(400).json({ message: error.message });
    }
});

app.get("/reservations", async (req, res) => {
    const reservations = await Reservation.find().populate("restaurant");
    res.json(reservations);
});



app.post("/menu-items", async (req, res) => {
    try {
        const { name, price, restaurant } = req.body;

        if (!name || !price || !restaurant)
            return res.status(400).json({ message: "Nombre, precio y restaurante son obligatorios" });

        if (!mongoose.Types.ObjectId.isValid(restaurant))
            return res.status(400).json({ message: "ID de restaurante inválido" });

        const existingRestaurant = await Restaurant.findById(restaurant);
        if (!existingRestaurant)
            return res.status(404).json({ message: "El restaurante no existe" });

        const menuItem = await MenuItem.create(req.body);
        res.status(201).json(menuItem);

    } catch (error) {
        res.status(400).json({ message: error.message });
    }
});

app.get("/menu-items", async (req, res) => {
    const menuItems = await MenuItem.find().populate("restaurant");
    res.json(menuItems);
});

app.delete("/menu-items/:id", async (req, res) => {
    if (!mongoose.Types.ObjectId.isValid(req.params.id))
        return res.status(400).json({ message: "ID inválido" });

    const deleted = await MenuItem.findByIdAndDelete(req.params.id);

    if (!deleted)
        return res.status(404).json({ message: "Menu item no encontrado" });

    res.json({ message: "Menu item eliminado correctamente" });
});



app.listen(3000, () => {
    console.log("Servidor corriendo en http://localhost:3000");
});
