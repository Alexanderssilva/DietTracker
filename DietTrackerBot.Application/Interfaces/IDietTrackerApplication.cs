using DietTrackerBot.Application.Dto;
using DietTrackerBot.Domain;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Interfaces
{
    public interface IDietTrackerApplication
    {
        Task<ResponseDto> ReceiveRequest(Update update);
    }
}
