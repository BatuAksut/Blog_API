# 📝 Blog API with JWT Authentication and Role-Based Authorization

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=flat&logo=docker)
![License](https://img.shields.io/badge/License-MIT-green.svg)

This is a RESTful API built with **ASP.NET Core** that is fully containerized with **Docker**. It supports:

- 🔐 JWT-based Authentication
- 👥 Role-based Authorization (`Reader`, `Writer`, `Admin`)
- 📰 Blog Post Management (Create, Read, Update, Delete)
- 🖼 Image Uploads for Posts
- 🧾 Comment System (Hierarchical Structure)
- ⚡ Filtering, Sorting, and Pagination
- 🛡️ Secure API Endpoints with ASP.NET Identity
- 🐳 **Full Docker Support (API + SQL Server)**

## 🚀 Tech Stack

- **ASP.NET Core Web API 8.0**
- **Entity Framework Core**
- **SQL Server**
- **Docker & Docker Compose**
- **AutoMapper**
- **JWT Authentication**
- **Serilog (Logging)**
- **Swagger (API documentation)**

---

## ⚙️ Configuration & Security

To ensure security, sensitive data (like Secrets and Passwords) are **not** stored in `appsettings.json`. They are managed via **Environment Variables**.

| Variable | Description | Location |
| :--- | :--- | :--- |
| `Jwt__Key` | Secret key for signing tokens | `docker-compose.yml` (Docker) or `launchSettings.json` (Local) |
| `ConnectionStrings__BlogAuthConnection` | SQL Connection String | `docker-compose.yml` or `appsettings.json` |
| `SA_PASSWORD` | SQL Server Admin Password | `docker-compose.yml` |

---

## ⚙️ Getting Started

You can run this application easily using Docker (Recommended) or set it up manually.

### 🐳 Option 1: Run with Docker (Recommended)

This method sets up the API and SQL Server automatically. You do **not** need to install the .NET SDK or SQL Server locally.

**Prerequisites:**

- [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running.

**Steps:**

1. **Clone the repository**

    ```bash
    git clone https://github.com/BatuAksut/Blog_API.git
    cd Blog_API
    ```

2. **Run with Docker Compose**
    Open your terminal in the solution folder (where `docker-compose.yml` is located) and run:

    ```bash
    docker-compose up --build
    ```

    > **Note:** This command will:
    > - Build the API image.
    > - Start a SQL Server container.
    > - **Automatically apply database migrations** (create the DB and tables).
    > - Expose the API on port **7171**.

3. **Access the Application**
    Once the containers are running, navigate to:
     **[http://localhost:7171/swagger](http://localhost:7171/swagger)**

---

### 🛠 Option 2: Manual Installation (Local Dev)

*Follow these steps only if you are NOT using Docker.*

**Prerequisites:**

- .NET 8 SDK
- SQL Server (LocalDB or Standalone)

**Steps:**

1. **Configure Database**
    Update `appsettings.json` in the `API` folder with your local connection string.

2. **Apply Migrations**

    ```bash
    dotnet tool install --global dotnet-ef
    cd API
    dotnet ef database update
    ```

3. **Run the Application**

    ```bash
    dotnet run
    ```

    *The app will run on the ports defined in `launchSettings.json` (usually 5016 or 7171).*

    *Note: Ensure `Jwt__Key` is set in your environment variables or `launchSettings.json` before running.*

---

## 📦 Features & Endpoints

### 📖 API Documentation

The API is fully documented using **Swagger/OpenAPI**.
URL: **[http://localhost:7171/swagger](http://localhost:7171/swagger)**

### ⚡ Key Endpoints

| Resource | Method | Endpoint | Description | Auth Required |
| ---------- | -------- | ---------- | ------------- | --------------- |
| **Auth** | POST | `/api/auth/register` | Register a new user | ❌ |
| **Auth** | POST | `/api/auth/login` | Login and receive JWT | ❌ |
| **Posts** | GET | `/api/blogposts` | List all posts (Filter/Sort) | ✅ |
| **Posts** | POST | `/api/blogposts/with-image` | Create post with image | ✅ (Writer/Admin) |
| **Posts** | PUT | `/api/blogposts/{id}` | Update own post | ✅ (Owner) |
| **Comments** | GET | `/api/blog-posts/{id}/comments` | Get comments for a post | ✅ |
| **Comments** | POST | `/api/blog-posts/{id}/comments` | Add comment to a post | ✅ |
| **Comments** | DELETE | `/api/comments/{id}` | Delete own comment | ✅ (Owner/Admin) |

### 🔒 Role Policies

| Role | Permissions |
| ------ | ------------- |
| **Reader** | Can view posts and comments. |
| **Writer** | Can create/edit/delete their **own** posts and comments. |
| **Admin** | Full access: manage all posts, users, and moderate comments. |

### Authentication Flow

1. User registers (`/api/auth/register`) and receives a role.
2. User logs in (`/api/auth/login`) and receives a **JWT Token**.
3. The Token must be included in the `Authorization` header for protected requests:

   ```text
   Authorization: Bearer <your_token_here>
   ```
