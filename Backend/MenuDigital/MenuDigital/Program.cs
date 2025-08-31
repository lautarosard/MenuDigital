using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Command;
using Infrastructure.Querys;
using Application.Interfaces.IDish;
using Application.Services;
using MenuDigital.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configurar EF Core con SQL Server
builder.Services.AddDbContext<MenuDigitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//INJECTIONS
//builder Dish
builder.Services.AddScoped<IDishCommand, DishCommand>();
builder.Services.AddScoped<IDishQuery, DishQuery>();
builder.Services.AddScoped<IDishService, DishService>();

//
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MenuDigital",
        Version = "v1",
        Description = "API del proyecto MenuDigital"
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware custom for exception handling
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
