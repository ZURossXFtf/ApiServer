using Lab3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateLifetime = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true
    };
});

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
IServiceCollection serviceCollection = builder.Services.AddDbContext<ModelDB>(options => options.UseSqlServer(connection));
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapPost("/login", async (User loginData, ModelDB db) =>
{
    User? person = await db.Users!.FirstOrDefaultAsync(p => p.EMail == loginData.EMail &&
p.Password == loginData.Password);
    if (person is null) return Results.Unauthorized();
    var claims = new List<Claim> { new Claim(ClaimTypes.Email, person.EMail!) };
    var jwt = new JwtSecurityToken(issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        claims: claims,
        expires: DateTime.Now.Add(TimeSpan.FromMinutes(2)),
        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );
    var encoderJWT = new JwtSecurityTokenHandler().WriteToken(jwt);
    var response = new
    {
        access_token = encoderJWT,
        username = person.EMail
    };
    return Results.Json(response);
});

app.MapGet("/api/assortiment", [Authorize] async (ModelDB db) => await db.Assortiments!.ToListAsync());
app.MapGet("/api/assortiment/{name}", [Authorize] async (ModelDB db, string name) => await db.Assortiments!.Where(u => u.Name == name).ToListAsync());
app.MapPost("/api/assortiment", [Authorize] async (Assortiment assortiment, ModelDB db) =>
{
    await db.Assortiments!.AddAsync(assortiment);
    await db.SaveChangesAsync();
    return assortiment;
});
app.MapPost("/api/registration", [Authorize] async (Registration registration, ModelDB db) =>
{
    await db.Registrations!.AddAsync(registration);
    await db.SaveChangesAsync();
    return registration;
});
app.MapDelete("/api/assoriment/{id:int}", [Authorize] async (int id, ModelDB db) =>
{
    Assortiment? assortiment = await db.Assortiments!.FirstOrDefaultAsync(u => u.Id == id);
    if (assortiment == null) return Results.NotFound(new { message = "Ассортимент не найден" });
    db.Assortiments!.Remove(assortiment);
    await db.SaveChangesAsync();
    return Results.Json(assortiment);
});
app.MapDelete("/api/registration/{id:int}", [Authorize] async (int id, ModelDB db) =>
{
    Registration? registration = await db.Registrations!.FirstOrDefaultAsync(u => u.Id == id);
    if (registration == null) return Results.NotFound(new { message = "Регистрация не найдена" });
    db.Registrations!.Remove(registration);
    await db.SaveChangesAsync();
    return Results.Json(registration);
});
app.MapPut("/api/assortiment", [Authorize] async (Assortiment assortiment, ModelDB db) =>
{
    Assortiment? a = await db.Assortiments!.FirstOrDefaultAsync(u => u.Id == assortiment.Id);
    if (a == null) return Results.NotFound(new { message = "Ассортимент не найден" });
    assortiment.Name = a.Name;
    assortiment.Price = a.Price;
    await db.SaveChangesAsync();
    return Results.Json(a);
});
app.MapPut("/api/registration", [Authorize] async (Registration registration, ModelDB db) =>
{
    Registration? reg = await db.Registrations!.FirstOrDefaultAsync(u => u.Id == registration.Id);
    if (reg == null) return Results.NotFound(new { message = "Регистрация не найдена" });
    reg.Id = registration.Id;
    reg.Name = registration.Name;
    reg.Weight = registration.Weight;
    reg.Cost = registration.Cost;
    reg.DateConfirm = registration.DateConfirm;
    reg.AssortimentId = registration.AssortimentId;
    await db.SaveChangesAsync();
    return Results.Json(reg);
});
app.Run();