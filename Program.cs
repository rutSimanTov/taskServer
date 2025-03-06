using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using ToDoApi;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS_TODOLISTDB");


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
          builder.WithOrigins("https://taskclient-7oyd.onrender.com")
        .WithMethods("GET","POST" ,"PUT" ,"DELETE")  
        .AllowAnyHeader();
    });
});



//mysql
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoListDB"),
    new MySqlServerVersion(new Version(8, 0, 40))));



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
//הרשאות
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseRouting();
app.UseCors();

app.UseAuthentication(); //אימות מי המשתמש
app.UseAuthorization(); //מה המשתמש יכול לעשות  

//------item----
app.MapGet("/",()=>"tasksServer api is running ");

app.MapGet("/item", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync();
});


app.MapPost("/item/{name}", async (ToDoDbContext db, string name) =>
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


app.MapPut("/item/{id}/{isComplete}", async (ToDoDbContext db, int id, bool isComplete) =>
{
    var item = await db.Items.FindAsync(id);
    if (item == null)
        return Results.NotFound();
    item.IsComplete = isComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.MapDelete("/item/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item == null)
        return Results.NotFound();
    db.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok();
}).RequireAuthorization();

//------user-----

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
