using MongoDB.Driver;
using PhotoPrintAPI.Models;
using PhotoPrintAPI.Settings;
using Microsoft.Extensions.Options;

namespace PhotoPrintAPI.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _ordersCollection;

        public OrderService(IOptions<OrderStoreDatabaseSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

            _ordersCollection = mongoDatabase.GetCollection<Order>(settings.Value.OrdersCollectionName);
        }

        public async Task<List<Order>> GetAllAsync() =>
            await _ordersCollection.Find(_ => true).ToListAsync();

        public async Task<Order?> GetByIdAsync(string id) =>
            await _ordersCollection.Find(o => o.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Order order) =>
            await _ordersCollection.InsertOneAsync(order);

        public async Task UpdateAsync(string id, Order updatedOrder) =>
            await _ordersCollection.ReplaceOneAsync(o => o.Id == id, updatedOrder);

        public async Task RemoveAsync(string id) =>
            await _ordersCollection.DeleteOneAsync(o => o.Id == id);
    }
}
