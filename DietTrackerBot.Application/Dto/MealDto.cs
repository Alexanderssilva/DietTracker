using DietTrackerBot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Application.Dto
{
    public class MealDto
    {
        public int FoodNumber { get; set; }
        public string MealId { get; set; }
        public double Weigth { get; set; }


        public static Meal ToMeal(MealDto meal,string userId)
        {
            return new Meal()
            {
                MealId = meal.MealId,
                FoodNumber= meal.FoodNumber,
                Weigth= meal.Weigth,
                UserId=userId,

            };
        }
        public static MealDto ToDto(Meal meal)
        {
            return new MealDto()
            {
                FoodNumber = meal.FoodNumber,
                MealId = meal.MealId,
                Weigth = meal.Weigth
            };
        }
    }

    public class MealDtoResponse: MealDto
    {
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }
}
