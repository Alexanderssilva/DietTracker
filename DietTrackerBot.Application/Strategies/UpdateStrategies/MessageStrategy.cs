using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Application.Strategies.Processors;
using DietTrackerBot.Application.Strategies.StrategiesFactories;
using DietTrackerBot.Infra;
using DietTrackerBot.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Strategies.UpdateStrategies
{
    public class MessageStrategy : IUpdateStrategy
    {
        private readonly IResponseFactory _factory;
        private readonly IUserRepository  _userRepository;
        private readonly IMealRepository  _mealRepository;
        private readonly IFoodRepository  _foodRepository;
        private readonly IConfiguration   _configuration;


        public MessageStrategy(
                         IResponseFactory factory,
                         IUserRepository  userRepository,
                         IMealRepository  mealRepository,
                         IFoodRepository  foodRepository,
                         IConfiguration configuration)
        {
            _factory= factory;
            _userRepository=userRepository;
            _mealRepository=mealRepository;
            _foodRepository=foodRepository;
            _configuration = configuration;
        }


        async Task<ResponseDto> IUpdateStrategy.HandleUpdate(Update update)
        {
            TextStrategyFactory factory = new(_userRepository,_mealRepository,_foodRepository,_factory,_configuration,update);
            TextProcessor processor = new(factory);
            return await processor.ProcessText(update);
        }


    }
}
