C# .NET Authentication & Authorization API

A Clean Architecture–based backend system implementing user authentication, authorization, and role management using ASP.NET Core, Entity Framework Core, and JWT.

⸻

# Project Overview

This project is a complete backend authentication and authorization system built with .NET. It provides secure user registration, login, token-based authentication, and role-based/ policy-based authorization, following clean architectural principles to ensure scalability, maintainability, and separation of concerns.

The system includes:
	•	Secure User Registration
	•	JWT Access Token Generation
	•	Role-Based Access Control (RBAC)
	•	Policy-Based Authorization
	•	Admin-Level User & Role Management
	•	Protected CRUD Operations
	•	Structured Clean Architecture Folder Setup

⸻ 

# Project Goals

The overall aim of this project is to demonstrate mastery of:
	•	Backend authentication workflows
	•	Secure password hashing
	•	JSON Web Token (JWT) authentication
	•	Role-Based Authorization (Admin, Manager, User)
	•	Policy-Based Authorization
	•	Clean architecture development patterns
	•	Secure API design practices
	•	Database schema for identity systems

# Technology Used
  Purpose
• ASP.NET Core (.NET 7/8)
• API Development
• Entity Framework Core
• Database ORM
• SQL Server
• Primary Database
• JWT Authentication
• Secure Authentication
• ASP.NET Identity (optional)
• User & Role Management
• MediatR (optional)
• Clean Architecture & CQRS
• AutoMapper (optional) but replace with    manual mapping
• DTO Mapping instead Command and query • • request were used to separate read and    write operations.  
• Serilog (optional)
• Structured Logging
• Swagger / Swashbuckle
• API Documentation

Project Structure (Clean Architecture)

src/
 ├── Application/
 │    ├── Interfaces/
 │    ├── DTOs/
 │    ├── Services/
 │    └── CQRS / Validators/ 
 │
 ├── Domain/
 │    ├── Entities/
 │    ├── Enums/
 │    └── ValueObjects/
 │
 ├── Infrastructure/
 │    ├── Persistence/
 │    ├── EntityConfigurations/
 │    ├── Authentication/
 │    └── Services/
 │
 └── WebApi/
      ├── Controllers/
      ├── Middleware/
      ├── Filters/
      └── Configurations/

This structure ensures separation of concerns, unit testability, and scalability.

⸻

# System Features

1. User Registration

Users can register with:
	•	First Name
	•	Last Name
	•	Email
	•	Password (hashed using a secure algorithm: PBKDF2/BCrypt/SHA256-based hashing)

Validations include:
	•	Unique email enforcement
	•	Strong password checks
	•	Secure hashing (no plain-text passwords)

⸻

2. User Login & JWT Issuance

After successful login:
	•	A JWT token is generated
	•	Token contains:
	•	User ID
	•	Email
	•	Roles
	•	Expiration time
	•	Token must be sent with every protected request via Authorization: Bearer 

⸻

3. JWT-Protected Endpoints

Every protected endpoint validates:
	•	Token presence
	•	Token signature
	•	Expiration
	•	User roles
	•	User identity

Unauthorized requests return 401 or 403 appropriately.

⸻

4. Role Management (Admin Only)

Roles supported:
	•	Admin
	•	Manager
	•	User

Admin can:
	•	Create roles
	•	Assign roles to users
	•	Remove user roles
	•	Delete roles

All admin endpoints are strictly protected via RBAC.

⸻

5. User CRUD With Authorization Rules

User Type
• Permissions
• Admin
• View, Update, Delete any user
• Manager
• Limited access (custom policies)
• User
• Only view/update their own profile

Further restrictions can be applied using policy-based authorization.

# Database Schema

Minimum required tables:
	•	Users
	•	Roles
	•	UserRoles (junction)
	•	(Optional) RefreshTokens

   • Users
   • Roles
   • UserRoles
   • RefreshTokens (optional)

# Authentication Flow Overview
	1.	User registers → Password gets hashed and saved
	2.	User logs in with email + password
	3.	System validates credentials
	4.	JWT is generated containing claims
	5.	User consumes API endpoints using Authorization: Bearer <token>
	6.	Policies and role handlers check the user’s claims
	7.	If authorized → request proceeds
	8.	If not → 401 or 403

⸻
# How to Run the Project

Prerequisites
	•	.NET 7+ or .NET 8 SDK
	•	SQL Server (LocalDB or full instance)
	•	Postman / Thunder Client
	•	EF Core Tools installed

# Setup Steps

git clone <repo-url>
cd <project-folder>

1. Add migrations

dotnet ef migrations add InitialCreate -p Infrastructure -s WebApi
dotnet ef database update -p Infrastructure -s WebApi

2. Run the API

dotnet run --project WebApi

3. Open Swagger UI

https://localhost:<port>/swagger

# API Endpoints Summary

Auth

Method
Endpoint   Description
POST       /api/auth/register  Register new user
POST       /api/auth/login  Authenticate user & generate JWT

Users

Method
Endpoint                   Access
GET /api/users            Admin only
GET /api/users/{id}       Admin / Owner
PUT /api/users/{id}       Admin / Owner
DELETE /api/users/{id}    Admin only

Roles 
Method
Endpoint                  Access
POST /api/roles           Admin
POST /api/roles/assign    Admin
POST /api/roles/remove    Admin
DELETE /api/roles/{id}    Admin


# Postman Collection

Ensure to include:
	•	Public endpoints
	•	Authentication workflow
	•	Protected endpoints with Bearer token
	•	Admin-only endpoints

(Add link here once uploaded)

⸻

# Deliverables (Per Assignment)

✔ Working .NET API
✔ Database schema / migrations
✔ Postman collection
✔ Documentation of authentication & authorization logic (this README)

⸻

# Learning Outcomes

By completing this project, you will have solid understanding of:
	•	Secure user authentication
	•	Token lifecycle management
	•	Role-based & Policy-based authorization
	•	Clean architecture principles
	•	Endpoint protection best practices

⸻

# Contributions

Contributions, issues, and feature requests are welcome.
Feel free to open a PR or raise an issue.

# Feature Improvement 
  2FA with OTP 
  Rate Limiting 

