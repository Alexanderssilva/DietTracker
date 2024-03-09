using DietTrackerBot.Application.Dto;
using DietTrackerBot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Application.Factories
{
    public class ResponseFactory : IResponseFactory
    {
        public ResponseDto CreatePollResponse(List<Food> options)
        {

            return new PollResponse
            {
                Foods = options
            };
        }

        public ResponseDto CreateTextResponse(string text)
        {
            throw new NotImplementedException();
        }


    }
}
