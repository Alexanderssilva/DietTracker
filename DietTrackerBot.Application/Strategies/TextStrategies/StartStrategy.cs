using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Infra;
using DietTrackerBot.Infra.Interfaces;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Strategies.TextStrategies
{
    public class StartStrategy(IFoodRepository foodRepository,
                         IMealRepository mealRepository,
                         IUserRepository userRepository,
                         IResponseFactory factory) : ITextStrategy
    {
        private readonly IFoodRepository _foodRepository = foodRepository;
        private readonly IMealRepository _mealRepository = mealRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IResponseFactory _factory = factory;


        public async Task<ResponseDto> HandleText(Update update)
        {
            var user = await _userRepository.FindUser(update.Message.From.Id.ToString());
            if (user != null)
                return _factory.CreateTextResponse($"Olá {user.Name}, Obrigado por retornar em que posso te ajudar?");
            await _userRepository.SaveUser(update.Message.From.FirstName, update.Message.From.Id.ToString());
            return _factory.CreateTextResponse($@"
Olá {update.Message.From?.FirstName},

Sou o DietTracker e estou à disposição para ajudar em seus objetivos fitness.

Para contar calorias de alimentos, utilize o comando #Calorias da seguinte forma:

#calorias: alimento - peso em gramas

Por exemplo:
#calorias: arroz - 10g, pão - 20g
#CALORIAS: banana - 50g, uva - 50g

Este comando não é sensível a maiúsculas ou minúsculas.

Seu primeiro nome e ID do Telegram serão salvos em nosso banco de dados para facilitar sua jornada.
      

            ");
        }
    }
}