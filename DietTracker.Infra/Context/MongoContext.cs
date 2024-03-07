using DietTrackerBot.Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Conventions;
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
            var mongoClient = new MongoClient(_connectionString);
            string dataBaseName = "DietTracker";// new MongoUrl(_connectionString).DatabaseName;

            _database = mongoClient.GetDatabase(dataBaseName);
           // ConfigureConventions();
        }
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Food> Foods => _database.GetCollection<Food>("Foods");


        private static void ConfigureConventions()
        {
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CustomConventions", conventionPack, _ => true);
        }

    }

}
