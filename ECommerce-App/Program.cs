using ECommerce_App;
using ECommerce_App.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:5173") // Your React app URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Configuration
var configuration = builder.Configuration;

// Register MongoDbSettings from configuration with validation
builder.Services.AddOptions<MongoDbSettings>()
    .Bind(configuration.GetSection("MongoDbSettings"))
    .ValidateDataAnnotations()
    .Validate(settings =>
    {
        if (string.IsNullOrEmpty(settings.ConnectionString))
            return false;
        return true;
    });

// Register MongoDB services
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Register AdminService with health check
builder.Services.AddSingleton<IAdminService>(serviceProvider =>
{
    var database = serviceProvider.GetRequiredService<IMongoDatabase>();
    return new AdminService(database);
});
builder.Services.AddSingleton<CategoryService>(serviceProvider =>
{
    var database = serviceProvider.GetRequiredService<IMongoDatabase>();
    return new CategoryService(database);
});
builder.Services.AddSingleton<IProduct,ProductService>(productservice =>
{
    var database = productservice.GetRequiredService<IMongoDatabase>();
    return new ProductService(database);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");
app.UseDeveloperExceptionPage(); 

app.UseAuthorization();
app.MapControllers();
app.Run();