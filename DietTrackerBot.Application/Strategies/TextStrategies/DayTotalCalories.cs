using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Strategies.TextStrategies
{
    public class DayTotalCalories(IFoodRepository foodRepository,
                                  IMealRepository mealRepository,
                                  IUserRepository userRepository,
                                  IResponseFactory factory,
                                  IConfiguration configuration) : ITextStrategy
    {
        private readonly IFoodRepository _foodRepository = foodRepository;
        private readonly IMealRepository _mealRepository = mealRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IResponseFactory _factory = factory;
        private readonly IConfiguration _configuration = configuration;

        public async Task<ResponseDto> HandleText(Update update)
        {
            var id = update.Message.From.Id.ToString();
            var list = await _mealRepository.GetDayMeals(id, DateTime.Now);
            double totalCaloriesKcal= 0;
            double totalCaloriesKj = 0;

            foreach (var item in list)
            {
                var food = await _foodRepository.SearchFoodByFoodNumber(item.FoodNumber);
                var foodCalorieKcal = NutrientCount.Nutrient(food.Energy_kcal, item.Weigth);
                var foodCalorieKj = NutrientCount.Nutrient(food.Energy_kJ, item.Weigth);

                totalCaloriesKcal += foodCalorieKcal;
                totalCaloriesKj += foodCalorieKj;
            }

            return _factory.CreateTextResponse($"O total de calorias ingeridas durante o dia foi {totalCaloriesKcal}Kcal,{totalCaloriesKj}KJ ");
        }
    }
}
