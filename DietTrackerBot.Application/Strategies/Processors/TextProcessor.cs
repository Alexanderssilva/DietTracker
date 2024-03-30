using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Application.Strategies.StrategiesFactories;

namespace DietTrackerBot.Application.Strategies.Processors
{
    public class TextProcessor
    {
        private readonly TextStrategyFactory _factory;
        public TextProcessor(TextStrategyFactory factory) => _factory = factory;

        public void ProcessText(string text)
        {
            ITextStrategy strategy = _factory.CreateStrategy(text);
            strategy.HandleText(text);
        }
    }
}
