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
        public DietTrackerApplication(IDietTrackerRepository repository, IResponseFactory responseFactory)
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

            switch (update.Message.Text)
            {
                case string s when s.StartsWith("#CALORIAS:", StringComparison.CurrentCultureIgnoreCase):
                    Dictionary<string, double> foods = ConvertToDictionary(update.Message.Text, "#Calorias:");
                    List<List<FoodDto>> list = [];
                    foreach (var food in foods)
                    {
                        var responses = await _repository.SearchFoods(food.Key);
                        if (responses.Any())
                        {
                            var foodDtos = responses.Select(response => new FoodDto
                            {
                                FoodNumber = response.FoodNumber,
                                FoodName = response.FoodName,
                                Type = response.Type,
                                Energy_kcal = response.Energy_kcal,
                                Energy_kJ = response.Energy_kJ,
                                Protein = response.Protein,
                                Carbs = response.Carbs,
                                Fiber = response.Fiber,
                                Weight = food.Value
                            }).ToList();

                            list.Add(foodDtos);
                        }

                    }

                    return _factory.CreatePollResponse(list);

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
        }
        public async Task<ResponseDto> ButtonMessage(Update update)
        {
            var data = update.CallbackQuery.Data.Split('-');
            int.TryParse(data[0], out int id);
            double.TryParse(data[1], out double weight);
            var food = await _repository.SearchFoodByFoodNumber(id);
            FoodDto foodDto = new()
            {
                FoodName =food.FoodName,
                FoodNumber = food.FoodNumber,
                Type = food.Type,
                Energy_kcal = Nutrient(food.Energy_kcal,weight),
                Energy_kJ = Nutrient(food.Energy_kJ,weight),
                Protein = Nutrient(food.Protein,weight),
                Carbs = Nutrient(food.Carbs,weight),
                Fiber = Nutrient(food.Fiber,weight),
                Weight = weight
            };

            return _factory.EditResponse(@$"Você ingeriu:
    {foodDto.FoodName},
    Tipo: {foodDto.Type},
    Calorias: {foodDto.Energy_kcal} Kcal, {foodDto.Energy_kJ} KJ,
    Proteínas: {foodDto.Protein} g,
    Carboidratos: {foodDto.Carbs} g,
    Fibras: {foodDto.Fiber} g,
    Peso em gramas: {foodDto.Weight} g"
            );
        }
        static Dictionary<string, double> ConvertToDictionary(string inputString, string prefixo)
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
        private static double Nutrient(double nutrient, double weight)
        {
            return Math.Round((nutrient * weight / 100),2);
        }
    }
}
