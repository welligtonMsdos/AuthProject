using Auth10Api.Application.Validators;
using Auth10Api.Infrastructure;
using Auth10Api.Infrastructure.BackgroundServices;
using Auth10Api.Infrastructure.Data;
using Auth10Api.Infrastructure.Filters;
using Auth10Api.Infrastructure.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin() 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidatorFilter>();
});

builder.Services.AddOpenApi();

var secret = Environment.GetEnvironmentVariable("JwtSettings__Key");

if (string.IsNullOrEmpty(secret) || secret.Length < 32)
    throw new InvalidOperationException("JWT key is missing or too short (minimum 32 characters).");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            //ValidIssuer = "http://13.59.37.186:5011",
            ValidIssuer = "http://localhost:5011",
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IMongoClient>(sp => {

    var connectionString = builder.Configuration.GetConnectionString("AuthConnection");
        
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<AuthContext>();

builder.Services.AddHostedService<OutboxWorker>();

builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.Title = "Api Authentication";
    options.Theme = ScalarTheme.BluePlanet;
    options.DefaultHttpClient = new(ScalarTarget.JavaScript, ScalarClient.HttpClient);
    options.CustomCss = "";
    options.ShowSidebar = true;
    options.AddPreferredSecuritySchemes("Bearer")
           .AddHttpAuthentication("Bearer", auth =>
           {
               auth.Token = "your-bearer-token";
           });
});

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
