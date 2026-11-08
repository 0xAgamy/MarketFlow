using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class CartService(IConnectionMultiplexer redis,IConfiguration _config) : ICartService
    {
        private readonly IDatabase _database= redis.GetDatabase();
        private TimeSpan Expiry =>
                        TimeSpan.FromDays(_config.GetValue("Redis:CartDefaultExpiryInDays", 30));

        private static string GetCartKey(string id) => $"cart:{id}";
       
        

        public async Task<ShoppingCart?> GetCartAsync(string id)
        {
            var data= await _database.StringGetAsync(GetCartKey(id));

            return data.IsNullOrEmpty 
                        ? null 
                        : JsonSerializer.Deserialize<ShoppingCart>((string)data!);

        }

        public async Task<ShoppingCart?> SetCartAsync(ShoppingCart cart)
        {
           
            
            var created = await _database.StringSetAsync(
            GetCartKey(cart.Id),
            JsonSerializer.Serialize(cart),
            Expiry);

            if (!created) return null;
            return await GetCartAsync(cart.Id);
        }


        public async Task<bool> DeleteCartAsync(string id)
        {
           return await _database.KeyDeleteAsync(GetCartKey(id));
        }
    }
}
