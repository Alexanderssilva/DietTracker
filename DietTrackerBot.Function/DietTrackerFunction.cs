using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using DietTrackerBot.Application;
using DietTrackerBot.Application.Interfaces;
using Google.Protobuf;

namespace DietTrackerBot.Function
{
    public class DietTrackerFunction
    {

        private readonly TelegramBotClient _client = new TelegramBotClient("6395942850:AAETZwxdNXQdhMiUaWRawtouyNoXlbLwwuY");
        private readonly IDietTrackerApplication _dietApplication;
        public DietTrackerFunction(IDietTrackerApplication dietTrackerApplication)
        {
            _dietApplication = dietTrackerApplication;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] Update update)
        {

            switch (update.Type)
            {
                case UpdateType.Message://normal message
                  var response =   await _dietApplication.TextMessage(update);
                    await _client.SendTextMessageAsync(update?.Message?.Chat?.Id, "");
                    break;
                case UpdateType.CallbackQuery://btn selected

                    break;
            }
            return new OkResult();
        }



    }
}
