﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietTrackerBot.Application.Dto
{
    public class ErrorResponse:ResponseDto
    {
        public string Text { get; set; }
    }
}
