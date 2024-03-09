using DietTrackerBot.Application.Dto;
using DietTrackerBot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Application.Factories
{
    public interface IResponseFactory
    {
        ResponseDto CreateTextResponse(string text);
        ResponseDto CreatePollResponse(List<Food> foods);

    }
}
