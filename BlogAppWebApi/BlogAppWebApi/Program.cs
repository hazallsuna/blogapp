using Microsoft.EntityFrameworkCore;
using BlogAppWebApi.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using BlogAppWebApi.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlogAppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "BlogAppAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None; // Cross-site cookie için
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS gerektirir
    options.LoginPath = "/api/Users/Login";
    options.LogoutPath = "/api/Users/Logout";
    options.AccessDeniedPath = "/api/Users/AccessDenied";
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvc",
        builder => builder.WithOrigins("https://localhost:[MVC_PORT]")
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials());
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
