using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Domain;
using DietTrackerBot.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DietTrackerBot.Application.Strategies.UpdateStrategies
{
    public class CallbackStrategy(IFoodRepository foodRepository,
                                  IMealRepository mealRepository,
                                  IUserRepository userRepository,
                                  IResponseFactory factory,
                                  IConfiguration configuration) : IUpdateStrategy
    {
        private readonly IFoodRepository _foodRepository = foodRepository;
        private readonly IMealRepository _mealRepository = mealRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _configuration = configuration;
        private readonly IResponseFactory _factory = factory;
        public async Task<ResponseDto> HandleUpdate(Update update)
        {
            var data = JsonConvert.DeserializeObject<MealDto>(update.CallbackQuery.Data);
            Meal meal = MealDto.ToMeal(data, update.CallbackQuery.From.Id.ToString());
            var food = FoodDto.ToDto(await _foodRepository.SearchFoodByFoodNumber(data.FoodNumber));
            await _mealRepository.SaveMeal(meal);

            CalculateNutrients(food, data.Weigth);

            return _factory.EditResponse(@$"Você ingeriu:
                                            {food.FoodName},
                                            Tipo: {food.Type},
                                            Calorias: {food.Energy_kcal} Kcal, {food.Energy_kJ} KJ,
                                            Proteínas: {food.Protein} g,
                                            Carboidratos: {food.Carbs} g,
                                            Fibras: {food.Fiber} g,
                                            Peso em gramas: {data.Weigth} g"
                                                    );
        }

        private static void CalculateNutrients(FoodDto food, double weight)
        {
            food.Energy_kcal = NutrientCount.Nutrient(food.Energy_kcal,weight);
            food.Energy_kJ = NutrientCount.Nutrient(food.Energy_kJ,weight);
            food.Protein = NutrientCount.Nutrient(food.Protein,weight);
            food.Carbs = NutrientCount.Nutrient(food.Carbs,weight);
            food.Fiber = NutrientCount.Nutrient(food.Fiber,weight);
        }
    }
}