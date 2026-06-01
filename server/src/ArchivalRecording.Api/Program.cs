using System.Text;
using ArchivalRecording.Api.Endpoints.Auth;
using ArchivalRecording.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ── CORS ───────────────────────────────────────────────────────────────────────
var allowedOrigins = config.GetSection("AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173", "http://localhost:3000"];

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

// ── AUTHENTICATION ─────────────────────────────────────────────────────────────
var jwtKey = config["Authentication:Jwt:SecretKey"]
    ?? throw new InvalidOperationException(
        "Authentication:Jwt:SecretKey is not configured. " +
        "Use 'dotnet user-secrets set Authentication:Jwt:SecretKey <value>' in development.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["Authentication:Jwt:Issuer"] ?? "ArchivalRecording",
            ValidateAudience = true,
            ValidAudience = config["Authentication:Jwt:Audience"] ?? "ArchivalRecording",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                ctx.Token = ctx.Request.Cookies["access_token"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddInfrastructureServices(config);

// ── BUILD & MIDDLEWARE ─────────────────────────────────────────────────────────
var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// ── ENDPOINTS ──────────────────────────────────────────────────────────────────
app.MapAuthEndpoints();

app.Run();
