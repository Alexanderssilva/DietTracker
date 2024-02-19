using DietTrackerBot.Domain;
using DietTrackerBot.Infra.Context;
using DietTrackerBot.Infra.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DietTrackerBot.Infra
{
    public class DietTrackerRespository : IDietTrackerRepository
    {
        private readonly MongoContext _context;
        public DietTrackerRespository(MongoContext context)
        {
            _context = context;
        }

        public async Task<List<Food>> SearchFoods(Dictionary<string, int> foods)
        {
            List<Food> foodList = new List<Food>();

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
