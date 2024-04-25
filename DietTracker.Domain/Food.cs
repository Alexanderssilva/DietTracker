namespace DietTrackerBot.Domain
{
    public class Food
    {
        public int _id { get; set; }
        public string? FoodName { get; set; }
        public string? Type { get; set; }
        public double Energy_kcal { get; set; }
        public double Energy_kJ { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fiber { get; set; }
        public double SaturatedFats { get; set; }
        public double MonounSaturatedFats { get; set; }
        public double PolyunSaturatedFats { get; set; }

    }
}
