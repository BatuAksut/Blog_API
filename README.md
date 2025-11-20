# 📝 Blog API with JWT Authentication and Role-Based Authorization

This is a RESTful API built with **ASP.NET Core** that supports:

- 🔐 JWT-based Authentication
- 👥 Role-based Authorization (`Reader`, `Writer`, `Admin`)
- 📰 Blog Post Management (Create, Read, Update, Delete)
- 🖼 Image Uploads for Posts
- 🧾 Comment System (Optional/Extensible)
- ⚡ Filtering, Sorting, and Pagination
- 🛡️ Secure API Endpoints with ASP.NET Identity

<!-- TODO: instructions on how to run the software? How to test, build, etc. -->

## 🚀 Tech Stack

- **ASP.NET Core Web API**
- **Entity Framework Core**
- **AutoMapper**
- **SQL Server**
- **JWT Authentication**
- **ASP.NET Core Identity**
- **Serilog (Logging)**
- **In-Memory Caching**
- **Swagger (API documentation)**
---
### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or Docker)
- A code editor (Visual Studio 2022 or VS Code)

### Installation

1.  **Clone the repository**
    ```bash
    git clone https://github.com/BatuAksut/Blog_API.git
    cd Blog_API
    ```

2.  **Configure Database**
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

3.  **Apply Migrations**
    Create the database and seed initial data (Roles & Admin users).
    ```bash
    cd API
    dotnet ef database update
    ```

4.  **Run the Application**
    ```bash
    dotnet run
    ```

---

## 📦 Features

This API provides a complete backend solution for blog applications:

* **Content Management:** Create, read, update, and delete blog posts.
* **Media:** Support for uploading cover images for blog posts.
* **Engagement:** Full commenting system for users to interact with posts.
* **Advanced Search:**
    * **Filtering:** Filter posts by title or content.
    * **Sorting:** Sort by title, date, etc. (Ascending/Descending).
    * **Pagination:** Efficiently handle large datasets.

---

### 🛠 Sample Endpoints

<!-- FIXME: providing the URL of the Swagger could have been better. -->

| Endpoint                               | Method | Auth         | Description                      |
|----------------------------------------|--------|--------------|----------------------------------|
| `/api/auth/register`                   | POST   | ❌           | Register a new user              |
| `/api/auth/login`                      | POST   | ❌           | Login and receive JWT token      |
| `/api/blogposts`                       | GET    | ✅           | List all posts                   |
| `/api/blogposts/{id}`                  | GET    | ✅           | Get a single post by ID          |
| `/api/blogposts/with-image`           | POST   | ✅ (Writer/Admin) | Create a post with an image   |
| `/api/blogposts/{id}`                  | PUT    | ✅ (Owner)   | Update own post                  |
| `/api/blogposts/{id}`                  | DELETE | ✅ (Owner/Admin) | Delete a post                |
| `/api/blogposts/{id}/upload-image`     | POST   | ✅ (Owner)   | Upload or replace post image     |

## 📖 API Documentation

The API is fully documented using **Swagger/OpenAPI**.

Once the application is running, navigate to:
`https://localhost:7171/swagger`

---

### 🔒 Role Policies

| Role    | Permissions                                 |
|---------|---------------------------------------------|
| Reader  | Can view posts only                         |
| Writer  | Can create, edit, and delete **own** posts  |
| Admin   | Full access: manage all posts and users     |

### Authentication Flow
1.  User registers (`/api/auth/register`) and receives a role.
2.  User logs in (`/api/auth/login`) and receives a **JWT Token**.
3.  The Token must be included in the `Authorization` header (`Bearer <token>`) for protected requests.
