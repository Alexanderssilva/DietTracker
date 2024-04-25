using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Application.Strategies.UpdateStrategies;
using DietTrackerBot.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DietTrackerBot.Application.Strategies.StrategiesFactories
{
    public class UpdateStrategyFactory
    {
        private readonly Dictionary<UpdateType, IUpdateStrategy> _handlers;
        private readonly IUserRepository _userRepository;
        private readonly IMealRepository _mealRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IResponseFactory _factory;
        private readonly IConfiguration _configuration;
        public UpdateStrategyFactory(
                         IResponseFactory factory,
                         IUserRepository userRepository,
                         IMealRepository mealRepository,
                         IFoodRepository foodRepository,
                         IConfiguration configuration
            )
        {
            _foodRepository = foodRepository;
            _factory = factory;
            _userRepository = userRepository;
            _mealRepository = mealRepository;
            _configuration = configuration;

            _handlers = new Dictionary<UpdateType, IUpdateStrategy>
            {
                {UpdateType.Message, new MessageStrategy(_factory,_userRepository,_mealRepository,_foodRepository,_configuration) },
                {UpdateType.CallbackQuery, new CallbackStrategy(_foodRepository,_mealRepository,_userRepository,_factory,_configuration)}
            };
        }

        public async Task<ResponseDto> HandleUpdate(Update update)
        {
            if (_handlers.TryGetValue(update.Type, out var handler))
            {
                return await handler.HandleUpdate(update);
            }
            else
            {
                return new ResponseDto();
            }

        }
    }
}
