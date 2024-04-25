


using Newtonsoft.Json;

namespace DietTrackerBot.Domain
{
    public class GPTRequest
    {
        public GPTRequest(string food,int foodNumber)
        {

           this.Model= "gpt-3.5-turbo";
           this.Messages=
                [
                    new() { role = "system", content = @"i will pass to you a food in portuguese and you will only respond with the following body for the food for 100 grams. { ""_id"":""id"" , ""FoodName"": ""Nome da Comida"", ""Type"": ""Tipo da Comida"", ""Energy_kcal"": 00.0, ""Energy_kJ"": 000.0, ""Protein"": 00.0, ""Carbs"": 00.0, ""Fiber"": 0.0, ""SaturatedFats"": 0.0, ""MonounSaturatedFats"": 0.0, ""PolyunSaturatedFats"": 0.0 }if it is not a food return a error the same body with error" },
                    new() { role = "user"  , content = food } 
                ];
           
        }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("messages")]
        public Message[] Messages { get; set; }
    }

    public class Message
    {
        [JsonProperty("role")]
        public string role { get; set; }
        [JsonProperty("content")]
        public string content { get; set; }
    }


}

