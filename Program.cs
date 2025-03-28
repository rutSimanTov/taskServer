using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoApi;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

//Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
         builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});


// MySQL connection string from environment variable
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoListDB") ?? 
                     Environment.GetEnvironmentVariable("CONNECTIONSTRINGS_TODOLISTDB"),
    new MySqlServerVersion(new Version(8, 0, 40))));


// Authentication and authorization setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});

//Authorization
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseRouting();
app.UseCors();
app.UseAuthentication(); //User authentication middleware
app.UseAuthorization(); //User authorization middleware

//Endpoint to check if the API is running
app.MapGet("/",()=>"tasksServer api is running ");

//Endpoint to retrieve all items
app.MapGet("/item", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync();
});

//Endpoint to create a new item
app.MapPost("/Item/{name}", async (ToDoDbContext db, string name) =>
 {
     var newItem = new Item
     {
         Name = name,
         IsComplete = false
     };
     db.Items.Add(newItem);
     await db.SaveChangesAsync();
     return Results.Created($"/ {newItem.Id}", newItem);
 }).RequireAuthorization();

//Endpoint to update an existing item
app.MapPut("/Item/{id}/{isComplete}", async (ToDoDbContext db, int id, bool isComplete) =>
{
    var Item = await db.Items.FindAsync(id);
    if (Item == null)
        return Results.NotFound();
    Item.IsComplete = isComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

//Endpoint to delete an item
app.MapDelete("/Item/{id}", async (ToDoDbContext db, int id) =>
{
    var Item = await db.Items.FindAsync(id);
    if (Item == null)
        return Results.NotFound();
    db.Remove(Item);
    await db.SaveChangesAsync();
    return Results.Ok();
}).RequireAuthorization();

//Endpoint to register a new user
app.MapPost("/register", async (ToDoDbContext db, [FromBody] User user) =>
{
    if (await db.Users.AnyAsync(u => u.Name == user.Name))
    {
        return Results.BadRequest("משתמש קיים");
    }
    var newUser = new User
    {
        Name = user.Name,
        Password = user.Password
    };

    db.Users.Add(newUser);
    await db.SaveChangesAsync();
    var jwt = CreateJWT(newUser, builder.Configuration);
    return Results.Ok(jwt);
});

//Endpoint to login a user
app.MapPost("/login", async (ToDoDbContext db, [FromBody] User user) =>
{
    var loginUser = await db.Users.FirstOrDefaultAsync(u => u.Name == user.Name);
    if (loginUser == null)
        return Results.NotFound("User not found");
    if (loginUser.Password != user.Password)
        return Results.Unauthorized();
    else
    {
        var jwt = CreateJWT(loginUser, builder.Configuration);
        return Results.Ok(jwt);
    }
});

//Method to create a JWT for authenticated users
static object CreateJWT(User user, IConfiguration configuration)
{
    var claims = new List<Claim>()
{
new Claim("id", user.Id.ToString()),
new Claim("name", user.Name)
};


    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT:Key")));

    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

    var tokenOptions = new JwtSecurityToken(

    issuer: configuration.GetValue<string>("JWT:Issuer"),

    audience: configuration.GetValue<string>("JWT:Audience"),

    claims: claims,

    expires: DateTime.Now.AddDays(30),

    signingCredentials: signinCredentials

    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

    return new { Token = tokenString };

}

app.Run();
