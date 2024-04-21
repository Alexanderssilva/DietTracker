using DietTrackerBot.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Strategies.Interfaces
{
    public interface IUpdateStrategy
    {
        Task<ResponseDto> HandleUpdate(Update update);

    }
}
