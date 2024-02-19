using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Domain
{

    public class Food
    {
        public _Id _id { get; set; }
        public int FoodNumber { get; set; }
        public string FoodName { get; set; }
        public string Type { get; set; }
        public Energy Energy { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fiber { get; set; }
    }

    public class _Id
    {
        public string oid { get; set; }
    }

    public class Energy
    {
        public int kcal { get; set; }
        public int kJ { get; set; }
    }

}
