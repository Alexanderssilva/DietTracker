﻿using DietTrackerBot.Domain;
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
        public async Task<Food> SearchFoodByFoodNumber(int foodNumber)
        {
            var filter = Builders<Food>.Filter.Eq(f => f._id,foodNumber);
            return await _context.Foods.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InsertFood(Food food)
        {
            await _context.Foods.InsertOneAsync(food);
        }

        public Task<int> FoodCount()
        {
            throw new NotImplementedException();
        }
    }
}
