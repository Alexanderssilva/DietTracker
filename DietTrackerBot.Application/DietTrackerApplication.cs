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
        public DietTrackerApplication(IDietTrackerRepository repository) 
        {
            _repository = repository;
        }
        public async Task<string> TextMessage(Update update)
        {
            if (update.Message?.From?.IsBot == true)
                return "bots não ingerem calorias";
            if (update.Message is null)
                return "erro geral contate o administrador";

            switch(update.Message.Text)
            {
                case string s when s.StartsWith("#CALORIAS:", StringComparison.CurrentCultureIgnoreCase):
                    Dictionary<string, double> foods = ConvertToDictionary(update.Message.Text, "#Calorias:");
                    List<Food> list = [];
                    foreach(var food in foods)
                    {
                        var foodDict = new Dictionary<string, double>();
                        var responses = await _repository.SearchFoods(new Dictionary<string, double> { { food.Key, food.Value } });
                        foreach(var response in responses)
                        {
                            response.Fiber = (float)Nutrient(response.Fiber, food.Value);
                            list.Add(response);

                        }
                    }
                    StringBuilder result = new();
                    foreach (var food in list)
                    {
                        result.AppendLine($"Alimento: {food.FoodName}, Calorias: {food.Energy_kcal} kcal - {food.Energy_kJ} KJ, Proteína: {food.Protein} g, Carboidratos: {food.Carbs} g, Fibra: {food.Fiber} g");
                    }
                    return result.ToString();

                    break;


                default:
                    //salva usuario

                    //retorna Mensagem Primaria
                    return $@"Olá {update.Message.From?.FirstName},
                         Sou o DietTracker estou a disposição para contar calorias e ajudar em seus objetivos fitness \n
                         use o comando #Calorias: para contar calorias do alimento que deseja
                         escreva da seguinte forma
                         #calorias: alimento,peso em gramas
                         ex.:#calorias: arroz - 10,pão - 20
                         ex.:#CALORIAS: banana - 50,uva - 50
                         (não é sensitivo a maiusculas ou minusculas)
                         seu primeiro nome será e Id do telegram será salvo em nosso banco de dados para facilitar sua jornada.
                        ";
            }
            return "erro";
        }
        static Dictionary<string, double> ConvertToDictionary(string inputString,string prefixo)
        {
            var texto = inputString[prefixo.Length..].TrimStart();
            var dictionary = new Dictionary<string, double>();

            // Divide a string em cada item separado por vírgula
            string[] items = texto.Split(',');

            foreach (var item in items)
            {
                // Divide cada item em nome e peso usando o padrão " - "
                string[] parts = item.Trim().Split("-");
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
