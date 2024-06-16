using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ushopDN.Models;

namespace ushopDN.Data
{
    public class ApplicationDbContext
    {
        private readonly IMongoDatabase _database;

        public ApplicationDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDbConnection"));
            _database = client.GetDatabase("eshop_dev");
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");
        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    }
}
