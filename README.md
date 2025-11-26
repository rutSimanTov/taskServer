

# Task Management API Server

This repository contains the server-side implementation of a Task Management System. The backend is built using C# with .NET Core and MySQL for the database, providing a minimal API with JWT authentication for secure access.

## Features 🌟

* **Task Management:** Users can create, update, delete, and mark tasks as completed.
* **Authentication & Authorization:** Users must authenticate to manage tasks. JWT (JSON Web Tokens) is used to securely handle user authentication.
* **Role-based Access:** Only authenticated users can add or delete tasks.
* **API Endpoints:** Exposes RESTful API endpoints for task management operations.

## Technology Stack 💻

* **Backend Framework:** .NET Core 8.0 (C#)
* **Database:** MySQL (Hosted on CalibreCloud)
* **Authentication:** JWT (JSON Web Token)
* **Containerization:** Docker for deploying the server in a containerized environment.
* **API Documentation:** Swagger (for visualizing and testing the API endpoints)

## How It Works 🔧

1. **User Registration:** A new user can register via the `/register` endpoint, where their data is stored in the database.
2. **User Login:** Registered users can log in using the `/login` endpoint, receiving a JWT token for authentication.
3. **Task Management:** Authenticated users can manage tasks (add, delete, update, mark as complete) via the `/Item` endpoint.
4. **JWT Authentication:** All endpoints that require user authentication validate the JWT token passed by the client.

## API Endpoints 📡

### 1. **User Registration**

* **POST** `/register`
* Request Body:

  ```json
  {
    "name": "username",
    "password": "password123"
  }
  ```
* Description: Registers a new user and returns a JWT token for authentication.

### 2. **User Login**

* **POST** `/login`
* Request Body:

  ```json
  {
    "name": "username",
    "password": "password123"
  }
  ```
* Description: Authenticates the user and returns a JWT token.

### 3. **Get All Tasks**

* **GET** `/item`
* Description: Fetches all tasks in the system. Accessible to all users.

### 4. **Create a New Task**

* **POST** `/Item/{name}`
* Description: Creates a new task. Requires JWT token for authentication.
* Example Request:

  ```json
  {
    "name": "Buy groceries"
  }
  ```

### 5. **Update Task Status**

* **PUT** `/Item/{id}/{isComplete}`
* Description: Marks a task as completed or not completed. Requires JWT token for authentication.

### 6. **Delete a Task**

* **DELETE** `/Item/{id}`
* Description: Deletes a specific task by ID. Requires JWT token for authentication.

## Setup and Installation 🚀

### Prerequisites

* **Docker** for containerization.
* **MySQL** database for storing user and task data.

### 1. Clone the repository:

```bash
git clone https://github.com/YourUsername/TaskServer.git
cd TaskServer
```

### 2. Set up the database:

Make sure your MySQL instance is running. Configure the connection string in the `appsettings.json` file.

```json
{
  "ConnectionStrings": {
    "ToDoListDB": "Server=your_database_host;Database=ToDoList;User=your_user;Password=your_password;"
  }
}
```

### 3. Build and run the Docker container:

```bash
docker-compose build
docker-compose up
```

This will build the Docker image and start the server. The API will be available on `http://localhost:5001`.

### 4. Configure JWT settings:

In the `appsettings.json` file, update the JWT configurations with your own values:

```json
"Jwt": {
  "Issuer": "your_issuer",
  "Audience": "your_audience",
  "Key": "your_jwt_secret_key"
}
```

## Docker Integration 🐳

This project includes a `Dockerfile` to containerize the application and ensure consistency across environments.

To build and run the application in a Docker container, use the following commands:

* **Build Docker Image**:

  ```bash
  docker build -t taskserver .
  ```

* **Run Docker Container**:

  ```bash
  docker run -p 5001:5001 taskserver
  ```

This will expose the server on port 5001. Ensure the MySQL database is connected and accessible to the application.

## Backend Integration 🔗
The frontend of the project is available here:  [Task Client Repository](https://github.com/rutSimanTov/taskClient)


Ensure the frontend client is set up to interact with this API. The server provides a minimal REST API for task management and authentication. Use the provided JWT token from the `/login` or `/register` endpoint to authenticate requests.

## Contributing 🤝

Contributions are welcome! Feel free to submit a pull request or open an issue if you encounter any bugs or have feature requests.


