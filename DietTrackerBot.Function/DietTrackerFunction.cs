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
using MongoDB.Driver;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

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
            Update? update = JsonConvert.DeserializeObject<Update>(value: requestBody);
            if(update == null)
               return new BadRequestResult();
            
            switch (update.Type)
            {
                case UpdateType.Message://normal message
                    var foodList = await _dietApplication.TextMessage(update);
                    await SendMessage(foodList, update);
                    break;
                case UpdateType.CallbackQuery://btn selected
                    var caloriesCount = await _dietApplication.ButtonMessage(update);
                    await SendMessage(caloriesCount, update);
                    break;

            }
            return new OkResult();
        }
        private async Task SendMessage(ResponseDto response, Update update)
        {

            switch (response)
            {
                case PollResponse pollResponse:
                    //BTNs
                    foreach (var food in pollResponse.Foods)
                    {
                        List<InlineKeyboardButton[]> keyboardRows = food.
                            OrderBy(option => option.FoodNumber).
                            Select(option => 
                        new[] { InlineKeyboardButton.WithCallbackData(text: option.FoodName, callbackData: JsonConvert.SerializeObject(option.CallBackData)) }
                        ).ToList();
                        InlineKeyboardMarkup inlineKeyboard = new(keyboardRows);
                        await _client.SendTextMessageAsync(chatId: update?.Message?.Chat?.Id,
                                                           text: "Qual é a sua opção",
                                                           replyMarkup: inlineKeyboard,
                                                           cancellationToken: CancellationToken.None);
                    }

                    break;
                case TextResponse textResponse:

                    await _client.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: textResponse.Text);
                    break;
                case ButtonResponse buttonResponse:
                    var messageId = update?.CallbackQuery?.Message?.MessageId;
                    if (messageId is not null && buttonResponse.Text is not null)
                    {
                        await _client.EditMessageTextAsync(
                              chatId: update?.CallbackQuery?.Message?.Chat?.Id,
                              messageId: messageId.Value,
                              text: buttonResponse.Text,
                              replyMarkup: null
                              );
                    }
                    break;
            }
        }

        [Function("Function2")]
        public async Task<IActionResult> Test([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);


            string food = data?.Food;
            string foodNumber = data?.number;
            if (int.TryParse(foodNumber, out int number))
            {
                var response = await _dietApplication.GetFoodWithChatGPT(food, number);
            }


            return new OkResult();
        }

    }
}
