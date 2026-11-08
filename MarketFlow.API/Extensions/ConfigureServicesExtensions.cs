using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace MarketFlow.API.Extensions;
public static class ConfigureServicesExtensions
{

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, WebApplicationBuilder builder
    )
    {
        var envConfigPath=FindEnvConfigFile();
        if (envConfigPath is null)
            throw new Exception("Cannot find env/config.json file");

        builder.Configuration.AddJsonFile(envConfigPath);


        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors();

        // Redis
        services.AddSingleton(sp =>
        {
            var configuration = ConfigurationOptions.Parse(
                builder.Configuration.GetConnectionString("Redis")
                ?? throw new Exception("Cannot get Redis connection string"), true);
            configuration.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(configuration);
        });
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            sp.GetRequiredService<ConnectionMultiplexer>());

        // EFCore
        services.AddDbContext<StoreContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Repositories / services
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddSingleton<ICartService, CartService>();


        return services;

    }
     private static string? FindEnvConfigFile()
    {
        for (var current = new DirectoryInfo(AppContext.BaseDirectory); current is not null; current = current.Parent)
        {
            var candidate = Path.Combine(current.FullName, "env", "config.json");
            if (File.Exists(candidate)) return candidate;
        }
        return null;
    }
}



