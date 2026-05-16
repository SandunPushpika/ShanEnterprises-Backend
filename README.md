# ShanEnterprises - Rent a car

A Clean Architecture based ASP.NET Core application built with .NET 10.

---

# Architecture Overview

This project follows the Clean Architecture pattern with separate layers for:

- **API** – Entry point / controllers
- **Application** – Business logic and use cases
- **Core** – Entities and core domain models
- **Infrastructure** – Database, external services, implementations

---

# Prerequisites

Before running the project, ensure the following are installed on your machine:

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- PostgreSQL (or the database used by the project)
- Git

Verify the installed .NET version:

```bash
dotnet --version
```

---

# Clone the Repository

```bash
git clone https://github.com/SandunPushpika/ShanEnterprises-Backend
cd ShanEnterprises-Backend
```

---

# Setup for Development

## 1. Create Environment File

Navigate to the API project directory and create a file named:

```bash
.env.development
```

Example:

```env
AppSettings__DefaultConnection="yourdatabase connection string goes here"
```

> Ensure `.env.development` is excluded from Git using `.gitignore`.

---

> Note: You can skip below parts if you run this through VS or Rider.

## 2. Restore Dependencies

From the solution root:

```bash
dotnet restore
```

## 3. Run the Project

Run the API project:

Navigate into the API directory and run:

```bash
dotnet run
```

---

# Running in Development Mode

Ensure the environment is set to Development in launchSettings.json:

The application should automatically load values from:

```bash
.env.development
```

---

# Useful Commands

## Build Project

```bash
dotnet build
```

## Run Tests

```bash
dotnet test
```

# Recommended `.gitignore` Entries

```gitignore
.env
.env.*
```

---

# Please follow the below Project Structure

```text
src/
├── API/
├── Application/
├── Core/
├── Infrastructure/
```

---

# Notes

- Do not commit secrets or `.env` files to Git.
- Use separate environment files for development, staging, and production.
- Keep connection strings and API keys inside environment variables only.