using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Domain
{
    public class Meal
    {
        public Meal()
        {
            TimeZoneInfo brTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            this.Date = TimeZoneInfo.ConvertTime(DateTime.Now, brTimeZone);
        }
        public int FoodNumber { get; set; }
        public string MealId { get; set; }
        public double Weigth { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }

    }
}

