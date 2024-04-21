using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Application
{
    public class NutrientCount
    {
        public static double Nutrient(double nutrient, double weight)
        {
            return Math.Round((nutrient * weight / 100), 2);
        }
    }
}
