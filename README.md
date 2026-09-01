# 🎫 TicketSystem

A web-based ticket management system for creating, managing and tracking support requests.

The project was developed to gain practical experience in building an ASP.NET Core web application with user authentication, database access and structured separation of application components.

## ✨ Features

* 👤 User registration and authentication
* 🎫 Create and manage support tickets
* 📋 Overview of existing tickets
* 📁 File handling and uploads
* 📧 Email functionality
* 👨‍💻 Project and ticket management
* 🔐 User management with ASP.NET Core Identity
* 🗄️ Persistent data storage using Entity Framework Core
* 📱 Responsive web interface

## 🛠️ Tech Stack

| Technology                | Purpose                            |
| ------------------------- | ---------------------------------- |
| **C#**                    | Application development            |
| **ASP.NET Core MVC**      | Web application framework          |
| **.NET 8**                | Runtime and development platform   |
| **Entity Framework Core** | Database access and ORM            |
| **SQL Server**            | Database                           |
| **ASP.NET Core Identity** | Authentication and user management |
| **Razor**                 | Server-side UI rendering           |
| **HTML / CSS**            | Frontend and styling               |

## 🏗️ Project Structure

The application follows a structured ASP.NET Core MVC architecture:

```text
TicketSystem/
├── Controllers/      # Handles HTTP requests and application flow
├── Database/         # DbContext, database initialization and migrations
├── Models/           # Domain and data models
├── ViewModels/       # Models used by the views
├── Views/            # Razor views and user interface
├── wwwroot/          # Static files and uploaded content
├── Program.cs        # Application configuration and startup
└── appsettings.json  # Application configuration
```

## 🗄️ Database

The application uses **SQL Server** together with **Entity Framework Core** for data persistence.

Database-related functionality is organized in the `Database` directory and includes the application's `DbContext`, database initialization and Entity Framework migrations.

## 🔐 Authentication

User authentication and account management are implemented using **ASP.NET Core Identity**.

This allows the application to manage user accounts and authentication while integrating the user system directly into the application.

## 🚀 Getting Started

### Prerequisites

Make sure the following are installed:

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server

### Clone the repository

```bash
git clone https://github.com/Toocy-CPU/TicketSystem.git
cd TicketSystem
```

### Configure the database

Update the database connection string in:

```text
TicketSystem/appsettings.json
```

Set the connection string according to your local SQL Server installation.

### Apply database migrations

From the project directory, run:

```bash
dotnet ef database update
```

### Start the application

```bash
dotnet run
```

The application will then be available at the local address shown in the console.

## 📸 Screenshots

Screenshots will be added here in a future update.

## 🎯 Project Goals

The main goal of this project is to develop a practical understanding of modern web application development with the .NET ecosystem.

The project focuses on:

* Building an application using ASP.NET Core MVC
* Working with Entity Framework Core and relational databases
* Implementing authentication and user management
* Structuring an application using controllers, models and view models
* Working with file uploads and application data
* Developing a maintainable and extensible application structure

## 👤 Author

Developed by [Toocy-CPU](https://github.com/Toocy-CPU).