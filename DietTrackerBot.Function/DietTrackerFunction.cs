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
using Microsoft.Extensions.Configuration;
using DietTrackerBot.Application.Strategies.Processors;
using DietTrackerBot.Application.Strategies.StrategiesFactories;
using Microsoft.VisualBasic;

namespace DietTrackerBot.Function
{
    public class DietTrackerFunction(IDietTrackerApplication dietTrackerApplication,IConfiguration configuration)
    {

        private readonly TelegramBotClient _client = new(token: configuration["TelegramToken"]);
        private readonly IDietTrackerApplication _dietApplication = dietTrackerApplication;


        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Update? update = JsonConvert.DeserializeObject<Update>(value: requestBody);
            if(update == null)
               return new BadRequestResult();

            var response =await _dietApplication.ReceiveRequest(update);
            await SendMessage(response, update);
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

    }
}
