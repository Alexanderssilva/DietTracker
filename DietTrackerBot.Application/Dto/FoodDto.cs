using DietTrackerBot.Domain;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Application.Dto
{
    public class FoodDto
    {
        public int FoodNumber { get; set; }
        public string FoodName { get; set; }
        public string Type { get; set; }
        public double Energy_kcal { get; set; }
        public double Energy_kJ { get; set; }

        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fiber { get; set; }
        public double SaturatedFats { get; set; }
        public double MonounSaturatedFats { get; set; }
        public double PolyunSaturatedFats { get; set; }
        public MealDto CallBackData { get; set; }

        public static Food ToFood(FoodDto food)
        {
            return new Food()
            {
                _id = food.FoodNumber,
                FoodName = food.FoodName,
                Type = food.Type,
                Energy_kcal = food.Energy_kcal,
                Energy_kJ = food.Energy_kJ,
                Protein = food.Protein,
                Carbs = food.Carbs,
                Fiber = food.Fiber,
                SaturatedFats = food.SaturatedFats,
                MonounSaturatedFats = food.MonounSaturatedFats,
                PolyunSaturatedFats = food.PolyunSaturatedFats

            };
        }
        public static FoodDto ToDto(Food food)
        {
            return new FoodDto()
            {
                FoodNumber = food._id,
                FoodName = food.FoodName,
                Type = food.Type,
                Energy_kcal = food.Energy_kcal,
                Energy_kJ = food.Energy_kJ,
                Protein = food.Protein,
                Carbs = food.Carbs,
                Fiber = food.Fiber,
                SaturatedFats = food.SaturatedFats,
                MonounSaturatedFats = food.MonounSaturatedFats,
                PolyunSaturatedFats = food.PolyunSaturatedFats
            };
        }
        
    }


}

