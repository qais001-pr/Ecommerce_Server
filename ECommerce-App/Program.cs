using ECommerce_App;
using ECommerce_App.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5000);
    options.Listen(IPAddress.Any, 5001, listenOptions => listenOptions.UseHttps());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(sp.GetRequiredService<IOptions<MongoDbSettings>>().Value.ConnectionString));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>()
     .GetDatabase(sp.GetRequiredService<IOptions<MongoDbSettings>>().Value.DatabaseName));

// Your application services
builder.Services.AddSingleton<IAdminService, AdminService>();
builder.Services.AddSingleton<ICategoryService, CategoryService>();
builder.Services.AddSingleton<IProduct, ProductService>();
builder.Services.AddSingleton<IWhishlists,WhishlistsService>();
builder.Services.AddSingleton<IUsers,UserServices>();
builder.Services.AddSingleton<IOrders,OrderServices>();
builder.Services.AddSingleton<IReview,ReviewService>();



// Ngrok hosted service only in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<NgrokHostedService>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();


public class NgrokHostedService : IHostedService
{
    private readonly ILogger<NgrokHostedService> _logger;
    private readonly IConfiguration _configuration;

    public NgrokHostedService(ILogger<NgrokHostedService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_configuration.GetValue<bool>("UseNgrok"))
            {
                _logger.LogInformation("Starting ngrok tunnel...");

                var options = new NgrokOptions
                {
                    AuthToken = _configuration["Ngrok:AuthToken"],
                    Region = _configuration["Ngrok:Region"] ?? "us"
                };

                var tunnel = await NgrokTunnel.CreateHttpTunnelAsync(5000, options);

                _logger.LogInformation($"Ngrok tunnel created: {tunnel.PublicUrl}");
                _logger.LogInformation($"Forwarding to: {tunnel.ForwardedUrl}");

                // Store or use the URL as needed
                Environment.SetEnvironmentVariable("NGROK_URL", tunnel.PublicUrl);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start ngrok tunnel");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Cleanup if needed
        return Task.CompletedTask;
    }
}

// Placeholder NgrokTunnel helper class
public static class NgrokTunnel
{
    public static async Task<NgrokTunnelResult> CreateHttpTunnelAsync(int port, NgrokOptions options)
    {
        // TODO: Replace with actual ngrok integration (e.g., using Ngrok.Client or CLI)
        await Task.Delay(100); // Simulate async operation
        return new NgrokTunnelResult
        {
            PublicUrl = $"http://localhost:{port}",
            ForwardedUrl = $"http://localhost:{port}"
        };
    }
}

public class NgrokOptions
{
    public string AuthToken { get; set; }
    public string Region { get; set; }
}

public class NgrokTunnelResult
{
    public string PublicUrl { get; set; }
    public string ForwardedUrl { get; set; }
}
