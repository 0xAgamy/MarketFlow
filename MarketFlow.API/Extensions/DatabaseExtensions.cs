using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MarketFlow.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        var retryCount = 0;
        const int maxRetries = 3;

        while (true)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

                await context.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(context);
                return;
            }
            catch (Exception ex) when (++retryCount <= maxRetries)
            {
                app.Logger.LogError(ex, "Migration attempt {Attempt}/{Max} failed, retrying in 5s...",
                    retryCount, maxRetries);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
