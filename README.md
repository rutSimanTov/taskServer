# Task Management Server

This repository contains the server-side implementation of a **Task Management System** built using **ASP.NET Core** and **C#**. The application provides a minimal API for managing tasks, user authentication via **JWT (JSON Web Tokens)**, and interaction with a **MySQL** database.

The server interacts with the client-side application and is deployed on **Render** and **Calver Cloud** for hosting, with Docker integration for easy deployment.

## Features

* **User Authentication**: Users can register, login, and securely interact with the system using JWT tokens.
* **Task Management**: Authenticated users can add, delete, update, and mark tasks as complete.
* **Public Task Viewing**: All users can view the list of tasks without needing to authenticate.
* **RESTful API**: Simple and effective API endpoints for interacting with tasks and users.
* **Docker Integration**: The backend is containerized using Docker for seamless deployment.

## Technology Stack

* **Backend**: ASP.NET Core (C#)
* **Database**: MySQL (hosted on Calver Cloud)
* **Authentication**: JWT (JSON Web Token)
* **Containerization**: Docker
* **Hosting**: Render (for production deployment)
* **API Documentation**: Swagger for API exploration and testing.

## Setup & Installation

Follow these steps to get the server-side of the project up and running locally:

### Prerequisites

* **.NET 8.0 SDK** (or higher)
* **Docker** (for containerization)
* **MySQL** (for database management)

### Installation Steps

1. **Clone the repository**:

   ```bash
   git clone https://github.com/yourusername/TaskServer.git
   cd TaskServer
   ```

2. **Configure your MySQL Database**:

   * Set up a MySQL database on your local machine or use a cloud-hosted MySQL instance.
   * Update your `appsettings.json` or environment variables with the correct connection string for MySQL.

3. **Restore dependencies**:

   ```bash
   dotnet restore
   ```

4. **Run the application locally**:

   ```bash
   dotnet run
   ```

   The application will be available at `https://localhost:5001`.

### Docker Setup

1. **Build the Docker image**:

   ```bash
   docker build -t taskserver .
   ```

2. **Run the Docker container**:

   ```bash
   docker run -p 5001:80 taskserver
   ```

   The API will be available at `http://localhost:5001`.

## API Endpoints

### 1. **GET /item**

Retrieve a list of all tasks.

**Response**: A list of task objects.

### 2. **POST /item/{name}**

Create a new task.

**Parameters**:

* `name`: The name of the task.

**Response**: The created task object.

### 3. **PUT /item/{id}/{isComplete}**

Update the completion status of a task.

**Parameters**:

* `id`: The ID of the task to update.
* `isComplete`: Boolean flag indicating whether the task is complete.

**Response**: `204 No Content`.

### 4. **DELETE /item/{id}**

Delete a task.

**Parameters**:

* `id`: The ID of the task to delete.

**Response**: `200 OK`.

### 5. **POST /register**

Register a new user.

**Body**:

```json
{
  "name": "username",
  "password": "password"
}
```

**Response**: A JWT token for the authenticated user.

### 6. **POST /login**

Login with an existing user.

**Body**:

```json
{
  "name": "username",
  "password": "password"
}
```

**Response**: A JWT token for the authenticated user.

## Authentication

The system uses **JWT (JSON Web Tokens)** for user authentication. The token is required to interact with protected API endpoints (e.g., adding, updating, or deleting tasks).

When registering or logging in, the server will return a JWT token that should be included in the `Authorization` header as a bearer token for any subsequent requests requiring authentication.

## Configuration

The JWT configuration values (e.g., `Issuer`, `Audience`, and `Key`) can be found in the `appsettings.json` file or be set via environment variables for deployment.

```json
"Jwt": {
  "Issuer": "yourIssuer",
  "Audience": "yourAudience",
  "Key": "yourSecretKey"
}
```

## Deployment

### Render Deployment

This server-side application is deployed using **Render**. It is configured to run with **HTTPS** and **Docker** integration.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any bugs or enhancements.



