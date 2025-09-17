using Application.Interfaces;
using Application.Interfaces.ICategory;
using Application.Interfaces.ICategory.Repository;
using Application.Interfaces.IDeliveryType;
using Application.Interfaces.IDeliveryType.Repository;
using Application.Interfaces.IDish;
using Application.Interfaces.IDish.Repository;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.Repository;
using Application.Interfaces.IOrderItem.Repository;
using Application.Interfaces.IStatus;
using Application.Interfaces.IStatus.Repository;
using Application.Services;
using Application.Services.CategoryService;
using Application.Services.DeliveryTypeService;
using Application.Services.DishServices;
using Application.Services.OrderService;
using Application.Services.StatusService;
using Application.Validators;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Command;
using Infrastructure.Data;
using Infrastructure.Querys;
using Infrastructure.Repositories;
using MenuDigital.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configurar EF Core con SQL Server
builder.Services.AddDbContext<MenuDigitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//INJECTIONS
//builder Dish
builder.Services.AddScoped<IDishCommand, DishCommand>();
builder.Services.AddScoped<IDishQuery, DishQuery>();
builder.Services.AddScoped<IDishRepository, DishRepositories>();
builder.Services.AddScoped<ISearchAsyncUseCase, SearchAsyncUseCase>();
builder.Services.AddScoped<IUpdateDishUseCase, UpdateDishUseCase>();
builder.Services.AddScoped<ICreateDishUseCase, CreateDishUseCase>();
//builder Category
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryQuery, CategoryQuery>();
builder.Services.AddScoped<ICategoryCommand, CategoryCommand>();
builder.Services.AddScoped<ICategoryExistUseCase, CategoryExistUseCase>();
builder.Services.AddScoped<IGetAllCategoryAsyncUseCase, GetAllCategoryAsyncUseCase>();

//builder DeliveryType
builder.Services.AddScoped<IDeliveryTypeCommand, DeliveryTypeCommand>();
builder.Services.AddScoped<IDeliveryTypeQuery, DeliveryTypeQuery>();
builder.Services.AddScoped<IGetAllDeliveryAsyncUseCase, GetAllDeliveryAsyncUseCase>();

//builder Order
builder.Services.AddScoped<IOrderCommand, OrderCommand>();
builder.Services.AddScoped<IOrderQuery,OrderQuery>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();

//builder Status
builder.Services.AddScoped<IStatusQuery, StatusQuery>();
builder.Services.AddScoped<IGetAllStatusAsyncUseCase, GetAllStatusAsyncUseCase>();

//builder OrderItem
builder.Services.AddScoped<IOrderItemCommand, OrderItemCommand>();
builder.Services.AddScoped<IOrderItemQuery, OrderItemQuery>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

//
builder.Services.AddControllers();
//Validation with FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DishRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DishUpdateRequestValidator>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


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
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        // Crea un endpoint en la UI por cada versión de la API
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

// Apply pending migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MenuDigitalDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Middleware custom for exception handling
app.UseMiddleware<ErrorHandlingMiddleware>();

//Desconmentar despues de terminar las pruebas 
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();