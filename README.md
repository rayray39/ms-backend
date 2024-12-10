# Back-End Development with .NET

This project was done as a requirement of [Microsoft's 'Back-End Development with .NET'](https://www.coursera.org/learn/back-end-development-with-dotnet?specialization=microsoft-full-stack-developer) course.

## Project Details

This project requires the development of a User Management Tool to manage user records for a large company. The company requires an API that allows them to create, update, retrieve, and delete user records efficiently.
The project was set up using a ASP.NET Core Web API project.

## Features
- CRUD endpoints for managing users (GET, POST, PUT, DELETE).
- Input validation.
- Middleware configuration (authentication, authorisation, global exception handling, and logging).
- Middleware pipeline and design.
- Serialisation and Deserialisation of JSON data into objects and vice versa.
- Implementing user authentication using JWT (JSON Web Tokens)

### Running this program
#### Before you get started
- Ensure you have installed the neccessary .NET platform and C# SDK.
- Ideally you want to run this program in an IDE like VS Code.

#### Things to note
- Ensure the port number of your local host server is correct before you make any HTTP requests.
- Ensure that you perform the '/login' request first, to simulate user authentication, and get the token.
- In the `requests.http` folder, perform the '/login' request and copy the token.
- Replace the lines `INSERT_TOKEN_HERE` with the token.

1. cd into the project folder where `Program.cs` lies.
2. run the command `dotnet run ./Program.cs` to get the server running.
3. go into `requests.http`
4. perform /login POST request first to get the token.
5. replace all instances of the `INSERT_TOKEN_HERE` with the copied token from previous step.
6. perform the HTTP requests in `requests.http`

## Other Learning Outcomes
- C# and .NET Platform.
- Handling HTTP requests.
- Understanding Middleware components.
- Asynchronous programming.
