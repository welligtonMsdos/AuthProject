using Auth10Api.Application.Interfaces;
using Auth10Api.Application.Validators;
using Auth10Api.Domain.Interfaces;
using Auth10Api.Infrastruture.Data;
using Auth10Api.Infrastruture.Filters;
using Auth10Api.Infrastruture.Middleware;
using FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Add services to the container.

builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidatorFilter>();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<AuthContext>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Scan(scan =>
{
    scan.FromAssemblyOf<IService>()
    .AddClasses(c => c.AssignableTo<IService>())
    .AsImplementedInterfaces()
    .WithScopedLifetime();

    scan.FromAssemblyOf<IRepository>()
    .AddClasses(c => c.AssignableTo<IRepository>())
    .AsImplementedInterfaces()
    .WithScopedLifetime();
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
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

//}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
