Sistema de Gestión de Restaurantes
1. Descripción General

El Sistema de Gestión de Restaurantes es una aplicación backend basada en arquitectura de microservicios. El sistema permite la administración de usuarios, autenticación segura mediante JWT, gestión de restaurantes, menús y operaciones asociadas.

El proyecto combina tecnologías modernas como Node.js para algunos microservicios y ASP.NET Core (.NET) para el servicio de autenticación.

2. Arquitectura del Sistema

El sistema está dividido en microservicios independientes, cada uno responsable de una funcionalidad específica.

2.1 Microservicios

authentication-service (.NET)

Registro de usuarios

Inicio de sesión

Generación de tokens JWT

Verificación de correo

Recuperación de contraseña

restaurant-service (Node.js)

Gestión de restaurantes

Creación y actualización de información del restaurante

menu-service (Node.js)

Gestión de productos del menú

Asociación de productos a restaurantes

banking-service (Node.js, opcional según implementación)

Operaciones financieras o transacciones internas

3. Tecnologías Utilizadas
3.1 Backend

Node.js

Express

MongoDB

Mongoose

.NET 8

ASP.NET Core

JWT (JSON Web Token)

BCrypt para encriptación de contraseñas

3.2 Herramientas de Desarrollo

Git

GitHub

Postman

Visual Studio / Visual Studio Code

4. Estructura del Proyecto
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

5. Requisitos Previos

Antes de ejecutar el proyecto, se debe contar con:

Node.js instalado

.NET 8 SDK instalado

MongoDB en ejecución (local o Atlas)

Git instalado

6. Instalación
6.1 Clonar el Repositorio
git clone https://github.com/usuario/Sistema-Restaurantes.git
cd Sistema-Restaurantes

7. Configuración del Authentication Service (.NET)
7.1 Restaurar dependencias
cd authentication-service/auth-service
dotnet restore

7.2 Configurar appsettings.json

Agregar la configuración JWT:

"Jwt": {
  "Key": "CLAVE_SECRETA_SEGURA",
  "Issuer": "AuthService",
  "Audience": "AuthServiceUsers"
}

7.3 Ejecutar el servicio
dotnet run

8. Configuración de los Servicios Node.js

Para cada microservicio:

cd restaurant-service
npm install
npm run dev


Asegurarse de que el archivo .env contenga:

MONGO_URI=mongodb://localhost:27017/restaurante_db
JWT_SECRET=clave_super_secreta

9. Autenticación y Seguridad

El sistema utiliza JWT para proteger endpoints.

9.1 Flujo de autenticación

El usuario se registra.

El sistema envía correo de verificación.

El usuario verifica su correo.

El usuario inicia sesión.

Se genera un token JWT.

El token se envía en el header Authorization.

Ejemplo:

Authorization: Bearer TOKEN_GENERADO

10. Endpoints Principales
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

11. Estado del Proyecto

Arquitectura base implementada.

Autenticación con JWT funcional.

Separación por microservicios.

Conexión con MongoDB.

Control de versiones mediante Git.

12. Autores

Proyecto desarrollado como parte del curso de Ingeniería en Sistemas.

Equipo de desarrollo:
Los Rezagados.
