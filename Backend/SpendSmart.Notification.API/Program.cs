using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpendSmart.Notification.API.Consumers;
using SpendSmart.Notification.API.Data;
using SpendSmart.Notification.API.Hubs;
using SpendSmart.Notification.API.Repositories;
using SpendSmart.Notification.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
       Title = "SpendSmart Notification API",
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

builder.Services.AddDbContext<NotificationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("NotificationDb")));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// MassTransit setup
builder.Services.AddMassTransit(x => {
    x.AddConsumer<NotificationConsumer>();

    x.UsingRabbitMq((ctx, cfg) => {
        cfg.Host("localhost", "/", h => {
            h.Username("guest"); 
            h.Password("guest");
        });
        
        cfg.ReceiveEndpoint("notification-queue", e => {
            e.ConfigureConsumer<NotificationConsumer>(ctx);
        });
    });
});

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
app.UseSwagger();
app.UseSwaggerUI();


// app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
