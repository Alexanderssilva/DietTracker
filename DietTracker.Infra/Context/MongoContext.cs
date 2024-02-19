using DietTrackerBot.Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DietTrackerBot.Infra.Context
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;
        private readonly string? _connectionString;

        public MongoContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MongoConnection");
            string dataBaseName = new MongoUrl(_connectionString).DatabaseName;
            var mongoClient = new MongoClient(_connectionString);
            _database = mongoClient.GetDatabase(dataBaseName);
        }
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Food> Foods => _database.GetCollection<Food>("Foods");




    }

}
