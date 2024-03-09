using DietTrackerBot.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Application.Dto
{
    public class PollResponse:ResponseDto
    {
        public List<Food> Foods { get; set; }

    }
}
