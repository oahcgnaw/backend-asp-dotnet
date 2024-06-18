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
            // connect to db based on environment
            _database = configuration["ServeMode"] == "dev" 
                ? client.GetDatabase("eshop_dev") 
                : client.GetDatabase("eshop");
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");
        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    }
}
