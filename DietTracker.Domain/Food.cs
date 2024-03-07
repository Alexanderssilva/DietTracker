using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Domain
{


    public class Food
    {

        public ObjectId Id { get; set; }
        public int FoodNumber { get; set; }
        public string FoodName { get; set; }
        public string Type { get; set; }
        public double Energy_kcal { get; set; }
        public double Energy_kJ { get; set; }

        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fiber { get; set; }
    }



    public class Energy
    {
        public int Kcal { get; set; }
        public int KJ { get; set; }
    }

}
