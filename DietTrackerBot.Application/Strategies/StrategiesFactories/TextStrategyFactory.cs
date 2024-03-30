using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Application.Strategies.TextStrategies;

namespace DietTrackerBot.Application.Strategies.StrategiesFactories
{
    public class TextStrategyFactory
    {
        private readonly Dictionary<Func<string, bool>, Func<ITextStrategy>> _strategyMap;

        public TextStrategyFactory()
        {
            _strategyMap = new Dictionary<Func<string, bool>, Func<ITextStrategy>>
            {
                { text => text.StartsWith("/start"),() => new StartStrategy() }
            };
        }

        public ITextStrategy CreateStrategy(string text)
        {
            foreach (var strategy in _strategyMap)
            {
                if (strategy.Key(text))
                {
                    return strategy.Value();
                }
            }
            return new UnknownStrategy();
        }
    }
}