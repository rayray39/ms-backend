using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Configure logging middlware
builder.Logging.ClearProviders(); 
builder.Logging.AddConsole(); 
builder.Logging.AddDebug();

// Configure JWT authentication 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
    .AddJwtBearer(options => { 
        options.TokenValidationParameters = new TokenValidationParameters { 
            ValidateIssuer = true,
            ValidateAudience = true, 
            ValidateLifetime = true, 
            ValidateIssuerSigningKey = true, 
            ValidIssuer = "yourIssuer", 
            ValidAudience = "yourAudience", 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey"))
        }; 
    });

var app = builder.Build();

// Add authentication middleware 
app.UseAuthentication(); 
app.UseAuthorization();

// exception handling middleware
app.UseExceptionHandler(errorApp => { 
    errorApp.Run(async context => { 
        context.Response.ContentType = "application/json"; 
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; 
        var errorFeature = context.Features.Get<IExceptionHandlerFeature>(); 

        if (errorFeature != null) { 
            var logger = app.Services.GetRequiredService<ILogger<Program>>(); 
            logger.LogError(errorFeature.Error, "An unhandled exception occurred."); 

            var result = new { 
                StatusCode = context.Response.StatusCode,
                Message = "An unexpected error occurred. Please try again later." 
            }; 
            await context.Response.WriteAsJsonAsync(result); 
        } 
    }); 
});

// Built-in Middleware for Logging HTTP Requests and Responses 
app.Use(async (context, next) => { 
    // Log the HTTP request method and path 
    var logger = app.Services.GetRequiredService<ILogger<Program>>(); 
    logger.LogInformation("HTTP Request: {Method} {Path}", context.Request.Method, context.Request.Path); 

    // Invoke the next middleware in the pipeline 
    await next(); 

    // Log the HTTP response status code 
    logger.LogInformation("HTTP Response Status Code: {StatusCode}", context.Response.StatusCode); 
});


// get all users
app.MapGet("/users", () =>
{
    try { 
        return Results.Ok(UserRepository.Users);
    } catch (Exception ex) { 
        return Results.Problem(ex.Message); 
    }
}).RequireAuthorization();;

// get a specific user by ID
app.MapGet("/users/{id:int}", (int id) =>
{
    try {
        if (id <= 0) {
            return Results.BadRequest("Invalid user ID.");
        }
        var user = UserRepository.Users.FirstOrDefault(u => u.Id == id);    // returns the first object found or a default (null)
        return user is not null ? Results.Ok(user) : Results.NotFound("User ID not found.");
    } catch (Exception ex) {
        return Results.Problem(ex.Message); 
    }
}).RequireAuthorization();;

// add a new user to the repository
app.MapPost("/users", (User user) =>
{
    try {
        if (string.IsNullOrWhiteSpace(user.Name)) { 
            return Results.BadRequest("User name cannot be empty.");
        }
        user.Id = UserRepository.Users.Count > 0 ? UserRepository.Users.Max(u => u.Id) + 1 : 1;
        UserRepository.Users.Add(user);
        return Results.Created($"/users/{user.Id}", user);
    } catch (Exception ex) {
        return Results.Problem(ex.Message);
    }
}).RequireAuthorization();;

// update an existing user's details.
app.MapPut("/users/{id:int}", (int id, User updatedUser) =>
{
    try {
        if (id <= 0) {
            return Results.BadRequest("Invalid user ID.");
        }
        if (string.IsNullOrWhiteSpace(updatedUser.Name)) { 
            return Results.BadRequest("User name cannot be empty.");
        }
        var user = UserRepository.Users.FirstOrDefault(u => u.Id == id);
        if (user is not null)
        {
            user.Name = updatedUser.Name;
            return Results.Ok(user);
        }
        return Results.NotFound("User ID not found.");
    } catch (Exception ex) {
        return Results.Problem(ex.Message);
    }
}).RequireAuthorization();;

// delete a user's details from the repository
app.MapDelete("/users/{id:int}", (int id) =>
{
    try {
        if (id <= 0) {
            return Results.BadRequest("Invalid user ID.");
        }
        var user = UserRepository.Users.FirstOrDefault(u => u.Id == id);
        if (user is not null)
        {
            UserRepository.Users.Remove(user);
            return Results.Ok($"Successfully deleted name: {user.Name}, id: {user.Id}");
        }
        return Results.NotFound("User ID not found.");
    } catch (Exception ex) {
        return Results.Problem(ex.Message);
    }
    
}).RequireAuthorization();;

app.Run();

public record User
{
    public int Id { get; set; }
    required public string Name { get; set; }

    [SetsRequiredMembers]
    public User(int id, string name) {
        Id = id;
        Name = name;
    }
}

public static class UserRepository
{
    public static List<User> Users { get; } = new List<User>() {
        new User(1, "Harry"),
        new User(2, "Gary")
    };
}
