using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DietTrackerBot.Function
{
    public class DietTrackerFunction
    {
        
        

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            TelegramBotClient _client = new TelegramBotClient("6395942850:AAETZwxdNXQdhMiUaWRawtouyNoXlbLwwuY");
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var update = JsonConvert.DeserializeObject<Update>(body);

            if (update.Type == UpdateType.Message)
            {
                await _client.SendTextMessageAsync(update.Message.Chat, text: $"Echo : {update.Message.Text}");
            }
            return new OkResult();
        }
    }
}
