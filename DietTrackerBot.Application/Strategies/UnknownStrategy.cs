using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Strategies.Interfaces;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Strategies
{
    public class UnknownStrategy : ITextStrategy
    {
        public Task<ResponseDto> HandleText(Update update)
        {
            throw new NotImplementedException();
        }
    }
}