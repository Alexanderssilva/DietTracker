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
using DietTrackerBot.Application.Dto;
using Google.Protobuf.WellKnownTypes;
using System.Threading;
using Microsoft.Extensions.Options;

namespace DietTrackerBot.Function
{
    public class DietTrackerFunction(IDietTrackerApplication dietTrackerApplication)
    {

        private readonly TelegramBotClient _client = new("6395942850:AAETZwxdNXQdhMiUaWRawtouyNoXlbLwwuY");
        private readonly IDietTrackerApplication _dietApplication = dietTrackerApplication;

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Update update = JsonConvert.DeserializeObject<Update>(requestBody);
            switch (update.Type)
            {
                case UpdateType.Message://normal message
                    var response = await _dietApplication.TextMessage(update);
                    await SendMessage(response, update);
                    break;
                case UpdateType.CallbackQuery://btn selected
                    var response1 = update.CallbackQuery.Data;

                    break;
            }
            return new OkResult();
        }
        private async Task SendMessage(ResponseDto response, Update update)
        {
            switch (response)
            {
                case PollResponse pollResponse:
                    List<InlineKeyboardButton[]> keyboardRows = pollResponse.Foods.Select(option =>
                    new[] { InlineKeyboardButton.WithCallbackData(text: option.FoodName, callbackData: option.FoodNumber.ToString()) }
                    ).ToList();
                    InlineKeyboardMarkup inlineKeyboard = new(keyboardRows);
                    await _client.SendTextMessageAsync(chatId: update?.Message?.Chat?.Id,
                                                       text: "Qual é a sua opção",
                                                       replyMarkup: inlineKeyboard,
                                                       cancellationToken: CancellationToken.None);
                    break;
                case TextResponse textResponse:
                    await _client.SendTextMessageAsync(chatId: update?.Message?.Chat?.Id,text: textResponse.Text);
                    break;
            }
        }



    }
}
