# 📝 Blog API with JWT Authentication and Role-Based Authorization

This is a RESTful API built with **ASP.NET Core** that supports:

- 🔐 JWT-based Authentication
- 👥 Role-based Authorization (`Reader`, `Writer`, `Admin`)
- 📰 Blog Post Management (Create, Read, Update, Delete)
- 🖼 Image Uploads for Posts
- 🧾 Comment System (Optional/Extensible)
- ⚡ Filtering, Sorting, and Pagination
- 🛡️ Secure API Endpoints with ASP.NET Identity

## 🚀 Tech Stack

- **ASP.NET Core Web API 8.0**
- **Entity Framework Core**
- **AutoMapper**
- **SQL Server**
- **JWT Authentication**
- **ASP.NET Core Identity**
- **Serilog (Logging)**
- **In-Memory Caching**
- **Swagger (API documentation)**

---

## ⚙️ Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or Docker)
- A code editor (Visual Studio 2022 or VS Code)

### Installation

1. **Clone the repository**

    ```bash
    git clone [https://github.com/BatuAksut/Blog_API.git](https://github.com/BatuAksut/Blog_API.git)
    cd Blog_API
    ```

2. **Install EF Core Tools** (Required to run database updates)
    If you haven't installed it yet, run this command globally:

    ```bash
    dotnet tool install --global dotnet-ef
    ```

3. **Configure Database**
    Update the `appsettings.json` file in the `API` folder with your connection string and JWT settings.

    ```json
    "ConnectionStrings": {
      "BlogAuthConnection": "Server=YOUR_SERVER;Database=BlogDb;Trusted_Connection=True;TrustServerCertificate=True"
    },
    "Jwt": {
      "Key": "YOUR_SUPER_SECRET_KEY_MUST_BE_LONG_ENOUGH",
      "Issuer": "https://localhost:7171",
      "Audience": "https://localhost:7171"
    }
    ```

4. **Apply Migrations**
    Create the database and seed initial data (Roles & Admin users).

    ```bash
    cd API
    dotnet ef database update
    ```

5. **Run the Application**

    ```bash
    dotnet run
    ```

---

## 📦 Features & Endpoints

This API provides a complete backend solution for blog applications.

### 📖 API Documentation (Swagger)

The API is fully documented using **Swagger/OpenAPI**.
Once the application is running, navigate to:

**[https://localhost:7171/swagger](https://localhost:7171/swagger)**

---

### 🛠 Sample Endpoints

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/auth/register` | POST | ❌ | Register a new user |
| `/api/auth/login` | POST | ❌ | Login and receive JWT token |
| `/api/blogposts` | GET | ✅ | List all posts |
| `/api/blogposts/{id}` | GET | ✅ | Get a single post by ID |
| `/api/blogposts/with-image`| POST | ✅ (Writer/Admin) | Create a post with an image |
| `/api/blogposts/{id}` | PUT | ✅ (Owner) | Update own post |
| `/api/blogposts/{id}` | DELETE | ✅ (Owner/Admin) | Delete a post |
| `/api/blogposts/{id}/upload-image` | POST | ✅ (Owner) | Upload or replace post image |
| `/api/comments/byuser/{userId}` | GET | ✅ | Get comments by a specific user |

### 🔒 Role Policies

| Role | Permissions |
|------|-------------|
| **Reader** | Can view posts only |
| **Writer** | Can create, edit, and delete **own** posts |
| **Admin** | Full access: manage all posts and users |

### Authentication Flow

1. User registers (`/api/auth/register`) and receives a role.
2. User logs in (`/api/auth/login`) and receives a **JWT Token**.
3. The Token must be included in the `Authorization` header (`Bearer <token>`) for protected requests.
