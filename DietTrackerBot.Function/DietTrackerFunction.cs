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
    public class DietTrackerFunction(IDietTrackerApplication dietTrackerApplication)
    {

        private readonly TelegramBotClient _client = new("6395942850:AAETZwxdNXQdhMiUaWRawtouyNoXlbLwwuY");
        private readonly IDietTrackerApplication _dietApplication = dietTrackerApplication;

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get","post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Update update = JsonConvert.DeserializeObject<Update>(requestBody);
            switch (update.Type)
            {
                case UpdateType.Message://normal message
                    var response =   await _dietApplication.TextMessage(update);
                    await _client.SendTextMessageAsync(update?.Message?.Chat?.Id, response);
                    break;
                case UpdateType.CallbackQuery://btn selected

                    break;
            }
            return new OkResult();
        }



    }
}
