using DietTrackerBot.Application.Strategies.Interfaces;

namespace DietTrackerBot.Application.Strategies
{
    public class UnknownStrategy : ITextStrategy
    {
        public void HandleText()
        {
            throw new NotImplementedException();
        }
    }
}