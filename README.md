🍽️ Sistema de Gestión de Restaurantes

Nota:
Este proyecto fue desarrollado con fines didácticos como parte del curso de Arquitectura de Software / Microservicios. Implementa una arquitectura basada en microservicios independientes, aplicando buenas prácticas de seguridad, autenticación y control de versiones.

📌 Descripción General

El Sistema de Gestión de Restaurantes es una aplicación backend basada en arquitectura de microservicios, diseñada para administrar usuarios, autenticación segura, restaurantes, menús y operaciones asociadas.

El sistema combina tecnologías modernas como ASP.NET Core (.NET 8) para el servicio de autenticación y Node.js para los servicios de negocio, permitiendo escalabilidad, mantenibilidad y separación clara de responsabilidades.

🧱 Arquitectura del Sistema

El sistema está dividido en microservicios independientes, cada uno responsable de una funcionalidad específica.

🔹 Microservicios
🔐 Authentication Service (.NET)

Registro de usuarios

Inicio de sesión

Generación y validación de tokens JWT

Verificación de correo electrónico

Recuperación y restablecimiento de contraseña

🏪 Restaurant Service (Node.js)

Gestión de restaurantes

Creación y actualización de información del restaurante

📋 Menu Service (Node.js)

Gestión de productos del menú

Asociación de menús a restaurantes

💳 Banking Service (Node.js – opcional)

Operaciones financieras internas

Manejo de transacciones (según implementación)

⚙️ Tecnologías Utilizadas
Backend

ASP.NET Core 8 (.NET 8)

Node.js

Express

MongoDB

Entity Framework Core

JWT (JSON Web Token)

BCrypt (encriptación de contraseñas)

Herramientas de Desarrollo

Git

GitHub

Postman

Visual Studio / Visual Studio Code

📁 Estructura del Proyecto
Sistema-Restaurantes/
│
├── authentication-service/
│   └── auth-service/
│
├── restaurant-service/
│
├── menu-service/
│
├── banking-service/
│
└── README.md
📋 Requisitos Previos

Antes de ejecutar el proyecto, asegúrate de contar con:

Node.js instalado

.NET 8 SDK instalado

MongoDB en ejecución (local o Atlas)

Git instalado

🚀 Instalación
1️⃣ Clonar el repositorio
git clone https://github.com/lcastro-2021276/Los_Rezagados_Sistema_Restaurantes.git
cd Sistema-Restaurantes
🔐 Configuración del Authentication Service (.NET)
Restaurar dependencias
cd authentication-service/auth-service
dotnet restore
Configurar appsettings.json

Agregar la configuración JWT:

"Jwt": {
  "Key": "CLAVE_SECRETA_SEGURA",
  "Issuer": "AuthService",
  "Audience": "AuthServiceUsers"
}
Ejecutar el servicio
dotnet run
🟢 Configuración de los Servicios Node.js

Para cada microservicio:

cd restaurant-service
npm install
npm run dev

Asegúrate de que el archivo .env contenga:

MONGO_URI=mongodb://localhost:27017/restaurante_db
JWT_SECRET=clave_super_secreta
🔑 Autenticación y Seguridad

El sistema utiliza JWT para proteger los endpoints.

Flujo de autenticación

El usuario se registra

Se envía un correo de verificación

El usuario verifica su correo

Inicia sesión

Se genera un token JWT

El token se envía en el header:

Authorization: Bearer TOKEN_GENERADO
📡 Endpoints Principales
Authentication Service

POST /api/v1/auth/register

POST /api/v1/auth/login

POST /api/v1/auth/verify-email

POST /api/v1/auth/forgot-password

POST /api/v1/auth/reset-password

GET /api/v1/auth/profile

Restaurant Service

GET /api/restaurants

POST /api/restaurants

PUT /api/restaurants/{id}

DELETE /api/restaurants/{id}

Menu Service

GET /api/menu

POST /api/menu

PUT /api/menu/{id}

DELETE /api/menu/{id}

📊 Estado del Proyecto

Arquitectura base implementada

Autenticación JWT funcional

Separación por microservicios

Conexión con MongoDB

Control de versiones mediante Git

👥 Autores

Proyecto desarrollado como parte del curso de Ingeniería en Sistemas.

Equipo de desarrollo:
Los Rezagados