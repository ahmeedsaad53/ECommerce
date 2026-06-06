E-commerce API System
A robust E-commerce backend API built with ASP.NET Core Web API, providing secure and scalable endpoints for managing products, orders, customers, and authentication. The system focuses on clean architecture, transactional operations, and secure JWT-based access control.
> Internal API namespace: `ECommerce` · Backend project: `ECommerce` · Default backend URL: `http://localhost:5009`

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Data Model](#data-model)
- [API Reference](#api-reference)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Available Scripts](#available-scripts)
- [Security Notes](#security-notes)
- [Roadmap](#roadmap)
- [License](#license)

---

## Overview

This API provides the backend foundation for an E-commerce platform, enabling secure management of products, orders, and users. It handles business logic such as stock validation, order processing, and authentication while exposing clean RESTful endpoints for integration with any frontend application.

---

## Features

### Authentication & Security
- User registration and login using ASP.NET Core Identity
- JWT Bearer authentication with role-based claims
- Protected endpoints using [Authorize]
- Secure token generation and validation

### Order Management
- Create orders with multiple items
- Transactional order processing
- Automatic stock deduction
- Order history retrieval

### Inventory Handling

- Validates product availability before order creation
- Prevents invalid or insufficient stock purchases
- Ensures data consistency across operations

---

## Tech Stack

### Backend
| Layer | Technology |
| --- | --- |
| Runtime | **.NET 10** (`net10.0`) |
| Web framework | **ASP.NET Core Web API** |
| ORM | **Entity Framework Core 10** (SQL Server provider) |
| Identity | **ASP.NET Core Identity** with `IdentityDbContext<ApplicationUser>` |
| Auth | **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| API docs | **Swashbuckle / Swagger UI** (with JWT auth scheme) |
| Database | **Microsoft SQL Server** (LocalDB / SQLEXPRESS) |
| Misc | CORS (`AllowAll` policy), `ReferenceHandler.IgnoreCycles` JSON serialization |

---

## Architecture

The solution follows a classic **layered Web API** pattern:

```
Client Request
     │
     ▼
Controllers (API Layer)
     │
     ▼
Services / Business Logic
     │
     ▼
Entity Framework Core (DbContext)
     │
     ▼
SQL Server(ECommerce Db)

Controllers handle HTTP requests
Business logic ensures validation and processing
EF Core manages database access
```



## Project Structure

```
ecommerce-api/
├── Controllers/
│   ├── AccController.cs           # Authentication (register/login + JWT)
│   ├── ProductsController.cs      # Product management
│   ├── CategoriesController.cs    # Category management
│   ├── CartController.cs          # Shopping cart operations
│   ├── OrderController.cs         # Order processing
│   ├── OrderItemController.cs     # Order items handling
│   └── PaymentController.cs       # Payment processing
│
├── Data/
│   ├── AppDbContext.cs            # EF Core context
│   ├── Cart.cs
│   ├── CartItem.cs
│   ├── Category.cs
│   ├── DbInitializer.cs           # Database seeding
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Payment.cs
│   └── Product.cs
│
├── DTOs/
│   ├── CartDTO.cs
│   ├── CartItemDTO.cs
│   ├── CategoryDTO.cs
│   ├── CreateOrderItemDTO.cs
│   ├── OrderDTO.cs
│   ├── OrderItemDTO.cs
│   ├── ProductDTO.cs
│   ├── LoginDTO.cs
│   └── RegisterDTO.cs
│
├── Program.cs
├── appsettings.json
└── ecommerce-api.csproj


## Data Model
ApplicationUser (1)
        │
        ├──────────< (1..1) Cart (1) ────────< (N) CartItem >──────── (N..1) Product
        │
        └──────────< (1..N) Order (1) ───────< (N) OrderItem >─────── (N..1) Product
                                      │
                                      └──────── (1) Payment                        
Category (1) ───────< (N) Product
``
|| Entity | Key fields |
| --- | --- |
| **ApplicationUser** | extends `IdentityUser`, `Cart`, `Orders[]` |
| **Category** | `Id`, `Name`, `Products[]` |
| **Product** | `Id`, `Name`, `Price`, `Stock`, `CategoryId`, `OrderItems[]`, `CartItems[]` |
| **Cart** | `Id`, `UserId`, `Items[]` |
| **CartItem** | `Id`, `CartId`, `ProductId`, `Quantity` |
| **Order** | `Id`, `UserId`, `Date`, `TotalAmount`, `Items[]`, `Payment` |
| **OrderItem** | `Id`, `OrderId`, `ProductId`, `Quantity`, `Price` *(price snapshotted at order time)* |
| **Payment** | `Id`, `OrderId`, `Amount`, `Status`, `PaymentDate` |
---

#API Endpoints
Base URL: http://localhost:5009/api

### Authentication (AccController)
POST /api/Acc/register → Register a new user
POST /api/Acc → Login / generate JWT token
GET /api/Acc → Get all users
GET /api/Acc/{id} → Get user by ID
DELETE /api/Acc → Delete user
``

###Categories (CategoriesController)

GET /api/Categories → Get all categories
GET /api/Categories/{id} → Get category by ID
POST /api/Categories → Create a category
PUT /api/Categories/{id} → Update category
DELETE /api/Categories/{id} → Delete category


### Products — `ProductController`
GET /api/Products → Get all products  
GET /api/Products/{id} → Get product by ID  
GET /api/Products/big-list → Get extended product list  
GET /api/Products/count → Get total number of products  
GET /api/Products/stock → Get stock summary  
GET /api/Products/search → Search products  
GET /api/Products/filter → Filter products  
PATCH /api/Products/{id}/Price → Update product Price  
POST /api/Products → Create a product  
PUT /api/Products/{id} → Update product  
PATCH /api/Products/{id} → Partial update product  
PATCH /api/Products/{id}/stock → Update product stock  
DELETE /api/Products/{id} → Delete product  

###Cart (CartController)
GET /api/Cart/MyCart → Get current user's cart  
POST /api/Cart/AddToCart → Add product to cart  
DELETE /api/Cart/RemoveFromCart → Remove product from cart 

###Orders (OrderController)
POST /api/Order → Create a new order  
GET /api/Order → Get all orders  
GET /api/Order/{id} → Get order by ID  
GET /api/Order/user/{userId} → Get orders for a specific user  
GET /api/Order/my-orders → Get current user's orders  
PUT /api/Order/{id} → Update order  
DELETE /api/Order/{id} → Delete order  

###Order Items (OrderItemController)
POST /api/OrderItem → Create order item  
GET /api/OrderItem/order/{orderId} → Get items for specific order  
DELETE /api/OrderItem/{id} → Delete order item  

###Payment (PaymentController)
POST /api/Payment/{orderId} → Process payment for order  
GET /api/Payment/{orderId} → Get payment details  
DELETE /api/Payment/{orderId} → Cancel or remove payment  

> Interactive documentation is available at **`/swagger`** when the API is running. The Swagger UI supports `Authorization: Bearer <token>` for testing protected endpoints.

---

## Getting Started

### Prerequisites
- **.NET SDK 10.0** or later
- **Microsoft SQL Server** (LocalDB, SQLEXPRESS, or full instance)
- (Optional) **Visual Studio 2022/2025** or **VS Code** with the C# Dev Kit

### 1. Clone
```bash
git clone https://github.com/ahmeedsaad53/ECommerce.git
cd ECommerce
```

### 2. Backend (ASP.NET Core API)
```bash
cd backend
dotnet restore
dotnet tool install --global dotnet-ef     # if not already installed
dotnet ef database update                  # applies migrations to SQL Server
dotnet run
```
The API will start on **`http://localhost:5009`** with Swagger UI at **`http://localhost:5009/swagger`**.


## Configuration

### Database connection
The connection string is currently set inline in `backend/Program.cs`:

```csharp
options.UseSqlServer(
    "Server=localhost\\SQLEXPRESS;Database=Zadan;Trusted_Connection=True;TrustServerCertificate=True;"
);
```

For other environments, replace the string or move it to `appsettings.json` under `ConnectionStrings:DefaultConnection` and read it via `builder.Configuration.GetConnectionString(...)`.

### JWT
JWT issuer / audience / signing key are also configured in `Program.cs`. For production, **move the signing key to a secret store** (User Secrets, environment variables, Azure Key Vault, etc.) and rotate it.



## Available Scripts

From `backend/`:

| Command | Purpose |
| --- | --- |
| `dotnet run` | Start the Web API |
| `dotnet ef migrations add <Name>` | Create a new migration |
| `dotnet ef database update` | Apply migrations to SQL Server |

---

## Security Notes

If you plan to deploy this project, harden the following items first:

1. **Move secrets out of source code.** The DB connection string and JWT signing key are currently hard‑coded in `Program.cs` — relocate them to environment variables or a secret manager.
2. **Tighten CORS.** The pipeline currently uses an `AllowAll` policy; restrict origins/methods/headers in production.
3. **Enable HTTPS redirection** (currently commented out in `Program.cs`).
4. **Protect `POST /api/Product`.** The other product endpoints require `[Authorize]`, but the create endpoint is currently public — add the attribute before going to production.
5. **Rotate the JWT signing key** and use a longer, environment‑specific secret.
6. **Validate input** more strictly with FluentValidation or data annotations where appropriate.

---

## Roadmaps

Ideas that fit naturally on top of the current foundation:

- Role‑based authorisation (Admin / Cashier / Read‑only) — claims are already issued.
- Refresh tokens and token revocation.
- Pagination, search, and filtering on list endpoints.
- Soft delete + audit trail for bills and products.
- Multi‑currency and tax/VAT support on invoices.
- Export reports to PDF / Excel.
- Dockerfile + `docker-compose` for one‑command setup.
- Unit and integration tests (xUnit + WebApplicationFactory).

---

## License

No license file is currently included in the repository. Add a `LICENSE` (e.g. MIT) to clarify usage rights for contributors and consumers.
