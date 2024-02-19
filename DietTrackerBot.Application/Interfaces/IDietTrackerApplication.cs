﻿using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Interfaces
{
    public interface IDietTrackerApplication
    {
        Task <string> TextMessage (Update update);
        Task FoodSearchBtn(Update update);
        Task SaveUser(User user);
    }
}
