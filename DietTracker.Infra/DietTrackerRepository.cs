using DietTrackerBot.Domain;
using DietTrackerBot.Infra.Context;
using DietTrackerBot.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DietTrackerBot.Infra
{
    public class DietTrackerRepository : IDietTrackerRepository
    {
        private readonly MongoContext _context;
        public DietTrackerRepository(MongoContext context)
        {
            _context = context;
        }

        public async Task<List<Food>> SearchFoods(string food)
        {
            var filter = Builders<Food>.Filter.Regex("FoodName", new BsonRegularExpression(food.Trim(), "i"));
            return await _context.Foods.Find(filter).ToListAsync();
        }

    }
}
