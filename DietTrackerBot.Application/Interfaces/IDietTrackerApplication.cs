using DietTrackerBot.Application.Dto;
using DietTrackerBot.Domain;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Interfaces
{
    public interface IDietTrackerApplication
    {
        Task <ResponseDto> TextMessage (Update update);
        Task<ResponseDto> ButtonMessage(Update update);
        Task<List<FoodDto>> GetFoodWithChatGPT(string food, int foodNumber);
    }
}
