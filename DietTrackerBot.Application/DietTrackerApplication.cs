  using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Interfaces;
using DietTrackerBot.Application.Strategies.Processors;
using DietTrackerBot.Application.Strategies.StrategiesFactories;
using DietTrackerBot.Domain;
using DietTrackerBot.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application
{
    public class DietTrackerApplication(IFoodRepository foodRepository,
                                  IMealRepository mealRepository,
                                  IResponseFactory responseFactory,
                                  IUserRepository userRepository,
                                  IConfiguration configuration) : IDietTrackerApplication
    {
        private readonly IFoodRepository _foodRepository = foodRepository;
        private readonly IMealRepository _mealRepository = mealRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _configuration = configuration;
        private readonly IResponseFactory _factory = responseFactory;


        public async Task<ResponseDto> ReceiveRequest(Update update)
        {
            try
            {
                UpdateStrategyFactory _update = new(_factory, _userRepository, _mealRepository, _foodRepository, _configuration);
                return await _update.HandleUpdate(update);
            }
            catch (Exception ex)
            {
                return _factory.CreateErrorMessage($"Erro no Metodo{ex.Message}");
            }

        }
    }
}
