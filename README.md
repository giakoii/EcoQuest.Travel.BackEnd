# EcoQuest Travel - Backend API

## 📝 Introduction

EcoQuest Travel is an online travel platform that helps users plan, share, and experience eco-friendly destinations. The Backend API provides RESTful endpoints to manage all application activities.

## 🛠️ Technologies Used

- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server
- OpenIddict (Authentication & Authorization)
- Cloudinary (Image Storage)
- Docker

## 🏗️ Architecture

The project is built on a layered architecture model:

- **Controllers**: Handle HTTP requests
- **DTOs**: Data Transfer Objects
- **Services**: Business logic processing
- **Repositories**: Data access
- **Models**: Data objects

## 🔑 Key Features

- User account management
- Partner management (hotels, restaurants)
- Travel destination management
- Blog and comment management
- Trip planning
- Trip itinerary management
- Image upload and management

## 🚀 Installation and Setup

### System Requirements

- .NET SDK 9.0 or higher
- SQL Server (or SQL Server Docker container)
- Docker (optional)

### Environment Setup

1. Create a `.env` file in the root directory with the necessary environment variables:

```
CONNECTION_STRING=Server=localhost,1433;Database=EcoQuestTravel;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
CLOUDINARY_CLOUD_NAME=your_cloud_name
CLOUDINARY_API_KEY=your_api_key
CLOUDINARY_API_SECRET=your_api_secret
GOOGLE_CLIENT_ID=your_google_client_id
GOOGLE_CLIENT_SECRET=your_google_client_secret
```

### Running with Docker

```bash
docker-compose up -d
```

### Running Locally

1. Restore packages:
```bash
dotnet restore
```

2. Update database:
```bash
dotnet ef database update
```

3. Run the application:
```bash
dotnet run --project BackEnd/BackEnd.csproj
```

## 📚 API Documentation

API documentation is automatically generated with Swagger. Access the following endpoint after running the application:

```
http://localhost:5269/swagger
```

## 📋 API Modules

| Module | Prefix | Description |
|--------|--------|-------------|
| Account | `/api/v1/Ecq000` | User account management |
| Blog | `/api/v1/Ecq100` | Blog and comment management |
| Trip | `/api/v1/Ecq110` | Trip management |
| Destination | `/api/v1/Ecq200` | Destination management |
| Hotel | `/api/v1/Ecq210` | Hotel information management |
| Partner | `/api/v1/Ecq310` | Partner management |

## 🧪 Testing

To run unit tests:

```bash
dotnet test
```

## 📄 License

© 2025 EcoQuest Travel. All rights reserved.
