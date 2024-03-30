using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Application.Strategies.Interfaces
{
    public interface ITextStrategy
    {
        void HandleText(string text);
    }
}
