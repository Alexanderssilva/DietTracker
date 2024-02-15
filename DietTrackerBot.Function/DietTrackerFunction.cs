using Azure.Core;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace DietTrackerBot.Function
{
    public class DietTrackerFunction
    {

        private readonly TelegramBotClient _client = new TelegramBotClient("6395942850:AAETZwxdNXQdhMiUaWRawtouyNoXlbLwwuY");


        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var update = JsonConvert.DeserializeObject<Update>(body);
            var message = update.Message;
            if (update.Type == UpdateType.Message)
            {
                var texto = message.Text.ToString();
                if (texto.Contains("alimentos:"))
                {
                   Dictionary<string,int> teste =  ConvertToDictionary(texto);
                    var test = teste;

                }
                else
                {
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
{
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Contar calorias", "ContarCalorias"),
                    InlineKeyboardButton.WithCallbackData("Opção 2", "opcao2")
                }
                });
                    var messageText = "O que gostaria de fazer?";
                    await _client.SendTextMessageAsync(message.Chat, messageText, replyMarkup: inlineKeyboard);
                }
                
            }

            if (update.CallbackQuery != null)
            {
                switch (update.CallbackQuery.Data)
                {
                    case "ContarCalorias":
                        string text = @"Os alimentos devem ser escritos da seguinte forma";
                        string format = @"alimentos: alimento - 00g, alimento - 00g";
                        await _client.SendTextMessageAsync(message.Chat, "Digite os alimentos");

                        break;
                    default:
                        break;
                }

            }
            

            return new OkResult();
        }

        static Dictionary<string, int> ConvertToDictionary(string inputString)
        {
            string prefixo = "alimentos:";
            var texto = inputString.Substring(prefixo.Length).TrimStart();
            var dictionary = new Dictionary<string, int>();

            // Divide a string em cada item separado por vírgula
            string[] items = texto.Split(',');

            foreach (var item in items)
            {
                // Divide cada item em nome e peso usando o padrão " - "
                string[] parts = item.Trim().Split(" - ");
                string weightString = parts[1].TrimEnd('g');

                if (parts.Length == 2)
                {

                    if (int.TryParse(weightString, out int weight))
                    {
                       
                        dictionary.Add(parts[0], weight);
                    }
                    else
                    {
                        Console.WriteLine($"Erro ao converter o peso '{parts[1]}' para um número inteiro.");
                    }
                }
                else
                {
                    Console.WriteLine($"Item '{item}' não está no formato válido 'nome - peso'.");
                }
            }

            return dictionary;
        }

    }
}
