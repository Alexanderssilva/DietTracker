using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Application.Strategies.StrategiesFactories;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Strategies.Processors
{
    public class TextProcessor
    {
        private readonly TextStrategyFactory _factory;
        public TextProcessor(TextStrategyFactory factory) => _factory = factory;

        public async Task<ResponseDto> ProcessText(Update update)
        {
            ITextStrategy strategy = _factory.CreateStrategy(update.Message.Text);
            return await strategy.HandleText(update);
        }
    }
}
