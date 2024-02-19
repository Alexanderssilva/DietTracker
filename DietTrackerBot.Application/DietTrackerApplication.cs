using DietTrackerBot.Application.Interfaces;
using DietTrackerBot.Infra.Interfaces;
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
        public async Task FoodSearchBtn(Update update)
        {
            if (update.Message?.From?.IsBot == true)
                return "bots não Comem";
            if (update.Message is null)
                return "erro geral contate o administrador";


        }
        public async Task<string> TextMessage(Update update)
        {
            if (update.Message?.From?.IsBot == true)
                return "bots não tem calorias";
            if (update.Message is null)
                return "erro geral contate o administrador";

            switch(update.Message.Text)
            {
                case string s when s.StartsWith("#CALORIAS:", StringComparison.CurrentCultureIgnoreCase):
                    Dictionary<string, int> foods = ConvertToDictionary(update.Message.Text, "#Calorias:");
                    var responses = _repository.SearchFoods(foods);
                    break;

                default:
                    //salva usuario

                    //retorna Mensagem Primaria
                    return $@"Olá {update.Message.From?.FirstName},
                         Sou o DietTracker estou a disposição para contar calorias e ajudar em seus objetivos fitness \n
                         use o comando #Calorias: para contar calorias do alimento que deseja\n
                         escreva da seguinte forma\n
                         #calorias: alimento,peso em gramas\n
                         ex.:#calorias: arroz - 10,pão - 20\n
                         ex.:#CALORIAS: banana - 50,uva - 50\n
                         (não é sensitivo a maiusculas ou minusculas)\n
                         seu primeiro nome será e Id do telegram será salvo em nosso banco de dados para facilitar sua jornada.
                        ";
                    break;
            }
        }
        static Dictionary<string, int> ConvertToDictionary(string inputString,string prefixo)
        {
            var texto = inputString[prefixo.Length..].TrimStart();
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

        public Task SaveUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
