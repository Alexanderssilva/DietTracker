using DietTrackerBot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Infra.Interfaces
{
    public interface IMealRepository
    {
        Task SaveMeal(Meal meal);
        Task<List<Meal>> GetMeal(string id);
    }
}
