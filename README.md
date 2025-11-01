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

## 📦 Features

<!-- FIXME: Authentication & Authorization are not features.
Features are what you provide with your software. You could have created a section for this.
Where you can also detail the section "🔒 Role Policies" -->

### 🔐 Authentication & Authorization

- Register with roles
- Login and receive JWT
- Secure endpoints with role-based access (`[Authorize(Roles = "...")]`)
- Identity password configuration customized for simplicity

### 📰 Blog Management

- **Create** a blog post with optional image upload
- **Get** all posts with:
  - Filtering (`filterOn`, `filterQuery`)
  - Sorting (`sortBy`, `isAscending`)
  - Pagination (`pageNumber`, `pageSize`)
- **Update/Delete** only your own posts
- Upload image to a blog post separately

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

---

### 🔒 Role Policies

| Role    | Permissions                                 |
|---------|---------------------------------------------|
| Reader  | Can view posts only                         |
| Writer  | Can create, edit, and delete **own** posts  |
| Admin   | Full access: manage all posts and users     |
