using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Interfaces;
using DietTrackerBot.Domain;
using DietTrackerBot.Infra.Interfaces;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DietTrackerBot.Application
{
    public class DietTrackerApplication : IDietTrackerApplication
    {
        private readonly IDietTrackerRepository _repository;
        private readonly IResponseFactory _factory;
        public DietTrackerApplication(IDietTrackerRepository repository,IResponseFactory responseFactory) 
        {
            _repository = repository;
            _factory = responseFactory;
        }
        public async Task<ResponseDto> TextMessage(Update update)
        {
            if (update.Message?.From?.IsBot == true)
                return _factory.CreateTextResponse("bots não ingerem calorias");
            if (update.Message is null)
                return _factory.CreateTextResponse("erro geral contate o administrador");

            switch(update.Message.Text)
            {
                case string s when s.StartsWith("#CALORIAS:", StringComparison.CurrentCultureIgnoreCase):
                    Dictionary<string, double> foods = ConvertToDictionary(update.Message.Text, "#Calorias:");
                    List<string> list = [];
                    foreach(var food in foods)
                    {
                        var foodDict = new Dictionary<string, double>();
                        var responses = await _repository.SearchFoods(new Dictionary<string, double> { { food.Key, food.Value } });

                        return _factory.CreatePollResponse(responses);
                    }


                    break;


                default:
                    //salva usuario

                    //retorna Mensagem Primaria
                    return _factory.CreateTextResponse($@"Olá {update.Message.From?.FirstName},
                         Sou o DietTracker estou a disposição para contar calorias e ajudar em seus objetivos fitness \n
                         use o comando #Calorias: para contar calorias do alimento que deseja
                         escreva da seguinte forma
                         #calorias: alimento,peso em gramas
                         ex.:#calorias: arroz - 10,pão - 20
                         ex.:#CALORIAS: banana - 50,uva - 50
                         (não é sensitivo a maiusculas ou minusculas)
                         seu primeiro nome será e Id do telegram será salvo em nosso banco de dados para facilitar sua jornada.
                        ");
            }
            return _factory.CreateTextResponse("erro");
        }
        static Dictionary<string, double> ConvertToDictionary(string inputString,string prefixo)
        {
            var texto = inputString[prefixo.Length..].TrimStart();
            var dictionary = new Dictionary<string, double>();

            string[] items = texto.Split(',');

            foreach (var item in items)
            {
                string[] parts = item.Trim().Split("-");
                string weightString = parts[1].TrimEnd('g');
                if (parts.Length > 0)
                {
                    if (int.TryParse(weightString, out int weight))
                    {
                        dictionary.Add(parts[0], weight);
                    }
                }
            }
            return dictionary;
        }
        private static void ShowSelectableList(Dictionary<string,int> foods)
        {
            var options = new List<string> { "Opção 1", "Opção 2", "Opção 3" };

            var keyboardButtons = new List<KeyboardButton[]>();

            foreach (var option in options)
            {
                keyboardButtons.Add([new KeyboardButton(option)]);
            }
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons);

        }
        private static double Nutrient(double nutrient,double weight)
        {
            return (nutrient * weight / 100);
        }
    }
}
