using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositries;

namespace Talabat.Repository.BasketRepo
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer redis) 
        { 
            _database = redis.GetDatabase();
        }
        public async Task<bool> DeleteCustomerBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);

        }

        public async Task<CustomerBasket?> GetCustomerBasketAsync(string id)
        {
            var basket = await _database.StringGetAsync(id);
            return basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
        }

        public async Task<CustomerBasket?> UpdateCustomerBasketAsync(CustomerBasket customerBasket)
        {
            var UpdateBasket = await _database.StringSetAsync(customerBasket.Id, JsonSerializer.Serialize(customerBasket), TimeSpan.FromDays(1));
            if (UpdateBasket is false) return null;
            return await GetCustomerBasketAsync(customerBasket.Id);

        }
    }
}
