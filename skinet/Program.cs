using API.Middlewares;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

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


builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped(typeof(IGenericRepository<>) , typeof(GenericRepository<>));

//

builder.Services.AddSingleton<ICartServices, CartServices>();

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
