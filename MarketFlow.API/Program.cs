using API.Middlewares;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Load env/config.json (connection strings + Redis env data)
var envConfigPath = FindEnvConfigFile();
if (envConfigPath is not null)
{
    builder.Configuration.AddJsonFile(envConfigPath, optional: false, reloadOnChange: true);
}
else
{
    throw new Exception("Cannot find env/config.json file");
}

static string? FindEnvConfigFile()
{
    // Walk up from the app base directory (bin/Debug/net10.0/...) to the solution root
    for (var current = new DirectoryInfo(AppContext.BaseDirectory); current is not null; current = current.Parent)
    {
        var candidate = Path.Combine(current.FullName, "env", "config.json");
        if (File.Exists(candidate)) return candidate;
    }
    return null;
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//

builder.Services.AddCors();

//


// Addin Redis Services
builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var conStrings = builder.Configuration.GetConnectionString("Redis");
    if (conStrings == null) throw new Exception("Cannot Get Redis Connection STrings");
    var configuration = ConfigurationOptions.Parse(conStrings,true);


    return ConnectionMultiplexer.Connect(configuration);

});

//
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//



builder.Services.AddScoped(typeof(IGenericRepository<>) , typeof(GenericRepository<>));

//

builder.Services.AddSingleton<ICartService, CartService>();

//
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//

try
{
    using var scope = app.Services.CreateScope();
    var serveices = scope.ServiceProvider;
    var context = serveices.GetRequiredService<StoreContext>();

    await context.Database.MigrateAsync();

    await StoreContextSeed.SeedAsync(context);
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}

//
app.Run();
