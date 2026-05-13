using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using SpendSmart.Budget.API.Data;
using SpendSmart.Budget.API.Repositories;
using SpendSmart.Budget.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
       Title = "SpendSmart Budget API",
       Version = "v1",
       Description = "Authentication microservices for SpendSmart platform" 
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token here"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});
builder.Services.AddDbContext<BudgetDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("BudgetDb")));

builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

// Add Background Service
builder.Services.AddHostedService<BudgetResetService>();

// HttpClient setup for internal API calls
builder.Services.AddHttpClient();

var jwtSecret = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrEmpty(jwtSecret))
{
    jwtSecret = "FallbackSuperSecretKeyWith32CharsLengthMinimum!"; // Fallback for local testing if missing
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts => {
        opts.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true, 
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "SpendSmart",
            ValidateAudience = true, 
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "SpendSmartUsers",
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
