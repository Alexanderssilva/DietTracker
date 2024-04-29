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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DietTrackerBot.Function
{
    public class DietTrackerFunction(IDietTrackerApplication dietTrackerApplication, IConfiguration configuration)
    {

        private readonly TelegramBotClient _client = new(token: configuration["TelegramToken"]);
        private readonly IDietTrackerApplication _dietApplication = dietTrackerApplication;


        [Function("DietTrackerBot")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Update? update = JsonConvert.DeserializeObject<Update>(value: requestBody);
            if (update == null)
                return new BadRequestResult();

            var response = await _dietApplication.ReceiveRequest(update);
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
                case ErrorResponse errorResponse:
                    await _client.SendTextMessageAsync(update.Message.Chat.Id, text: @"Houve um erro com o Bot, Por gentileza, verifique o texto enviado e tente novamente
Para contar calorias de alimentos, utilize o comando #Calorias da seguinte forma:

#calorias: alimento - peso em gramas

Por exemplo:
#calorias: arroz - 10g, pão - 20g
#CALORIAS: banana - 50g, uva - 50g

Este comando não é sensível a maiúsculas ou minúsculas.

/totaldodia - Devolve o valor total de calorias ingeridas no dia
");


                    await _client.SendTextMessageAsync(1577760395, text: @$"Houve um erro com a aplicação:  {errorResponse.Text}   USUARIO: {update.Message.From.FirstName} -  {update.Message.From.LastName} MENSAGEM: {update.Message.Text}");
                    break;

            }
        }

    }
}
