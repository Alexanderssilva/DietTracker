﻿using DietTrackerBot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Infra.Interfaces
{
    public interface IFoodRepository
    {
        Task<List<Food>> SearchFoods(string food);
        Task<Food> SearchFoodByFoodNumber(int foodNumber);
        Task<int> FoodCount();
        Task InsertFood(Food food);

    }
}
