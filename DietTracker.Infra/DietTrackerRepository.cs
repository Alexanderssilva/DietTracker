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

        public async Task<List<Food>> SearchFoods(Dictionary<string, double> foods)
        {
            List<Food> foodList = [];

            foreach (var food in foods)
            {
                var filter = Builders<Food>.Filter.Regex("FoodName", new BsonRegularExpression(food.Key, "i"));
                var findedFoods = await _context.Foods.Find(filter).ToListAsync();

                foodList.AddRange(findedFoods);
            }
            return foodList;
        }

    }
}
