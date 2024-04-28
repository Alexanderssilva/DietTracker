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
        public ResponseDto CreatePollResponse(List<List<FoodDto>> options)
        {

            return new PollResponse
            {
                Foods = options
            };
        }

        public ResponseDto CreateTextResponse(string text)
        {
            return new TextResponse { Text = text };
        }
        public ResponseDto EditResponse(string text)
        {
            return new ButtonResponse { Text = text };
        }

        public ResponseDto CreateErrorMessage(string message)
        {
            return new ErrorResponse { Text = message };
        }
    }
}
