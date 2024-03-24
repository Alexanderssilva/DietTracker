using DietTrackerBot.Domain;
using DietTrackerBot.Infra.Context;
using DietTrackerBot.Infra.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Infra
{
 
    public class MealRepository:IMealRepository
    {
        private readonly MongoContext _context;
        public MealRepository(MongoContext context)
        {
            _context = context;
        }

        public async Task SaveMeal(Meal meal)
        {
           await _context.Meals.InsertOneAsync(meal);
        }

        public async Task<List<Meal>> GetMeal(string id)
        {
            var filter = Builders<Meal>.Filter.Eq(id, id);
            return await _context.Meals.Find(filter).ToListAsync();
        }

        public async Task<List<Meal>> GetDayMeals(string id,DateTime date)
        {
            var filter = Builders<Meal>.Filter.And(
                Builders<Meal>.Filter.Eq("UserId", id),
                Builders<Meal>.Filter.Eq("Date", date));
            return await _context.Meals.Find(filter).ToListAsync();
        }
    }
}
