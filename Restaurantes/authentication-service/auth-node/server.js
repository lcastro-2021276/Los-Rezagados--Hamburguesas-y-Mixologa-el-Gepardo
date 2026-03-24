import express from "express";
import mongoose from "mongoose";
import bcrypt from "bcryptjs";
import dotenv from "dotenv";
import jwt from "jsonwebtoken";

dotenv.config();

const app = express();
app.use(express.json());


mongoose.connect(process.env.MONGO_URI)
    .then(() => console.log("MongoDB conectado"))
    .catch(err => console.error(err));




const Role = mongoose.model("Role", new mongoose.Schema({
    name: { type: String, required: true, unique: true, uppercase: true },
    permissions: [String]
}, { timestamps: true }));


const User = mongoose.model("User", new mongoose.Schema({
    name: String,
    email: { type: String, unique: true },
    password: String,
    role: { type: mongoose.Schema.Types.ObjectId, ref: "Role" }
}, { timestamps: true }));


const Restaurant = mongoose.model("Restaurant", new mongoose.Schema({
    name: { type: String, required: true },
    address: String
}, { timestamps: true }));


const Reservation = mongoose.model("Reservation", new mongoose.Schema({
    customerName: String,
    reservationDate: Date,
    restaurant: { type: mongoose.Schema.Types.ObjectId, ref: "Restaurant" }
}, { timestamps: true }));


const MenuItem = mongoose.model("MenuItem", new mongoose.Schema({
    name: String,
    price: Number,
    restaurant: { type: mongoose.Schema.Types.ObjectId, ref: "Restaurant" }
}));


const Table = mongoose.model("Table", new mongoose.Schema({
    number: { type: Number, unique: true },
    capacity: Number,
    status: { type: String, default: "disponible" }
}));


const Order = mongoose.model("Order", new mongoose.Schema({
    table: { type: mongoose.Schema.Types.ObjectId, ref: "Table" },
    items: [
        {
            menuItem: { type: mongoose.Schema.Types.ObjectId, ref: "MenuItem" },
            quantity: Number,
            price: Number
        }
    ],
    total: Number,
    status: { type: String, default: "pendiente" }
}, { timestamps: true }));


const auth = (req, res, next) => {
    const token = req.headers["authorization"];
    if (!token) return res.status(401).json({ message: "No token" });

    try {
        const decoded = jwt.verify(token, process.env.JWT_SECRET || "secreto");
        req.user = decoded;
        next();
    } catch {
        res.status(401).json({ message: "Token inválido" });
    }
};




app.post("/users", async (req, res) => {
    try {
        const { name, email, password, role } = req.body;

        const existingUser = await User.findOne({ email });

        if (existingUser) {
            return res.status(400).json({
                message: "El usuario ya existe con ese email"
            });
        }

        const hashed = await bcrypt.hash(password, 10);

        const user = await User.create({
            name,
            email,
            password: hashed,
            role
        });

        res.status(201).json(user);

    } catch (error) {
        res.status(500).json({ message: error.message });
    }
});


app.post("/login", async (req, res) => {
    try {
        const { email, password } = req.body;

        if (!email || !password) {
            return res.status(400).json({ message: "Email y contraseña son obligatorios" });
        }

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
            process.env.JWT_SECRET || "secreto",
            { expiresIn: "2h" }
        );

        res.json({
            message: "Login exitoso",
            token
        });

    } catch (error) {
        res.status(500).json({ message: error.message });
    }
});


app.post("/roles", async (req, res) => {
    const role = await Role.create(req.body);
    res.json(role);
});

app.get("/roles", async (req, res) => {
    res.json(await Role.find());
});


app.post("/restaurants", async (req, res) => {
    const restaurant = await Restaurant.create(req.body);
    res.json(restaurant);
});

app.get("/restaurants", async (req, res) => {
    res.json(await Restaurant.find());
});

app.post("/reservations", async (req, res) => {
    const { reservationDate, restaurant } = req.body;

    const existing = await Reservation.findOne({ reservationDate, restaurant });

    if (existing) {
        return res.status(400).json({ message: "Ya existe reserva" });
    }

    const reservation = await Reservation.create(req.body);
    res.json(reservation);
});

app.get("/reservations", async (req, res) => {
    res.json(await Reservation.find().populate("restaurant"));
});


app.post("/menu-items", async (req, res) => {
    const item = await MenuItem.create(req.body);
    res.json(item);
});

app.get("/menu-items", async (req, res) => {
    res.json(await MenuItem.find().populate("restaurant"));
});


app.post("/tables", async (req, res) => {
    const table = await Table.create(req.body);
    res.json(table);
});

app.get("/tables", async (req, res) => {
    res.json(await Table.find());
});




app.post("/orders", async (req, res) => {
    const { table, items } = req.body;

    let total = 0;

    const detailed = await Promise.all(items.map(async (i) => {
        const menu = await MenuItem.findById(i.menuItem);
        const subtotal = menu.price * i.quantity;
        total += subtotal;

        return {
            menuItem: menu._id,
            quantity: i.quantity,
            price: menu.price
        };
    }));

    const order = await Order.create({ table, items: detailed, total });
    res.json(order);
});


app.get("/orders", async (req, res) => {
    res.json(await Order.find().populate("table items.menuItem"));
});


app.put("/orders/:id/status", async (req, res) => {
    const order = await Order.findByIdAndUpdate(
        req.params.id,
        { status: req.body.status },
        { new: true }
    );
    res.json(order);
});


app.get("/report", async (req, res) => {
    const orders = await Order.find();

    const totalSales = orders.reduce((acc, o) => acc + o.total, 0);

    res.json({
        totalOrders: orders.length,
        totalSales
    });
});


app.listen(3000, () => {
    console.log("Servidor en http://localhost:3000");
});