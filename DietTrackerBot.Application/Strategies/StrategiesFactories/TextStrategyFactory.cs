using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Interfaces;
using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Application.Strategies.TextStrategies;
using DietTrackerBot.Infra;
using DietTrackerBot.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Strategies.StrategiesFactories
{

    public class TextStrategyFactory
    {
        private readonly Dictionary<Func<string, bool>, Func<ITextStrategy>> _strategyMap;
        private readonly IUserRepository _userRepository;
        private readonly IMealRepository _mealRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IResponseFactory _factory;
        private readonly IConfiguration   _configuration;
        private readonly Update update;

        public TextStrategyFactory(
               IUserRepository  userRepository,
               IMealRepository  mealRepository,
               IFoodRepository  foodRepository,
               IResponseFactory factory,
               IConfiguration configuration,
               Update update)
        {

            _userRepository = userRepository;
            _mealRepository = mealRepository;
            _foodRepository = foodRepository;
            _factory = factory;
            _configuration = configuration;
            this.update = update;
            _strategyMap = new Dictionary<Func<string, bool>, Func<ITextStrategy>>
            {
                { text => text.StartsWith("/start",StringComparison.CurrentCultureIgnoreCase),() => new StartStrategy(_foodRepository,_mealRepository,_userRepository,factory) },
                { text => text.StartsWith("#calorias:",StringComparison.CurrentCultureIgnoreCase),() => new CaloriesCountStrategy(_foodRepository,_mealRepository,_userRepository,factory,_configuration) },
                { text => text.StartsWith("/totaldodia",StringComparison.CurrentCultureIgnoreCase),() => new DayTotalCalories(_foodRepository,_mealRepository,_userRepository,factory,_configuration) }

            };
        }

        public ITextStrategy CreateStrategy(string text)
        {
            foreach (var strategy in _strategyMap)
            {
                if (strategy.Key(text))
                {
                    return strategy.Value();
                }
            }
            return new UnknownStrategy();
        }
    }
}