using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Interfaces;
using DietTrackerBot.Domain;
using DietTrackerBot.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application
{
    public class DietTrackerApplication : IDietTrackerApplication
    {
        private readonly IDietTrackerRepository _dietRepository;
        private readonly IMealRepository _mealRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        private readonly IResponseFactory _factory;
        public DietTrackerApplication(IDietTrackerRepository dietRepository,
                                      IMealRepository mealRepository ,
                                      IResponseFactory responseFactory,
                                      IUserRepository userRepository,
                                      IConfiguration configuration)
        {
            _dietRepository = dietRepository;
            _mealRepository = mealRepository;
            _userRepository = userRepository;
            _factory = responseFactory;
            _configuration = configuration;

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
                    string mealId = GenerateRandomHash();
                    foreach (var food in foods)
                    {
                        var responses = await _dietRepository.SearchFoods(food.Key);
                        if (responses.Any())
                        {
                            var foodDtos = responses.Select(response => FoodDto.ToDto(response))
                                                    .Select(dto =>
                                                    {
                                                        dto.CallBackData  = new MealDto()
                                                        {
                                                            FoodNumber = dto.FoodNumber,
                                                            MealId = mealId,
                                                            Weigth = food.Value
                                                        };
                                                        return dto;
                                                    }).ToList();

                            list.Add(foodDtos);
                        }
                        else
                        {
                            int totalFoods = await _dietRepository.FoodCount();
                            var foodDtos = await GetFoodWithChatGPT(food.Key, totalFoods);
                            foodDtos.ForEach(async foodDto =>
                            {
                                Food food2 = FoodDto.ToFood(foodDto);
                                await _dietRepository.InsertFood(food2);
                                foodDto.CallBackData = new MealDto()
                                {
                                    MealId = mealId,
                                    FoodNumber = foodDto.FoodNumber,
                                    Weigth = food.Value
                                };
                            });
                            list.Add(foodDtos);

                        }

                    }
                    return _factory.CreatePollResponse(list);

                case string s when s.StartsWith("/TOTALDODIA:", StringComparison.CurrentCultureIgnoreCase):
                    var meals = _mealRepository.GetMeal(update.Message.From.Id.ToString());
                    return _factory.CreateTextResponse("");

                    break;
                case string s when s.StartsWith("/START", StringComparison.CurrentCultureIgnoreCase):
                   var user = await _userRepository.FindUser(update.Message.From.Id.ToString());
                    if (user != null)
                        return _factory.CreateTextResponse($"Olá {user.Name}, Obrigado por retornar em que posso te ajudar?");
                    await _userRepository.SaveUser(update.Message.From.FirstName, update.Message.From.Id.ToString());
                    return _factory.CreateTextResponse($@"Olá {update.Message.From?.FirstName},
                         Sou o DietTracker estou a disposição para contar calorias e ajudar em seus objetivos fitness \n
                         use o comando #Calorias: para contar calorias do alimento que deseja
                         escreva da seguinte forma
                         #calorias: alimento,peso em gramas
                         ex.:#calorias: arroz - 10g,pão - 20g
                         ex.:#CALORIAS: banana - 50g,uva - 50g
                         (não é sensitivo a maiusculas ou minusculas)
                         seu primeiro nome será e Id do telegram será salvo em nosso banco de dados para facilitar sua jornada.
                        ");
                default:
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
            var data = JsonConvert.DeserializeObject<MealDto>(update.CallbackQuery.Data);
            Meal meal = MealDto.ToMeal(data,update.CallbackQuery.From.Id.ToString());
            var food = FoodDto.ToDto(await _dietRepository.SearchFoodByFoodNumber(data.FoodNumber));
            await _mealRepository.SaveMeal(meal);

            food.Energy_kcal = Nutrient(food.Energy_kcal, data.Weigth);
            food.Energy_kJ = Nutrient(food.Energy_kJ, data.Weigth);
            food.Protein = Nutrient(food.Protein, data.Weigth);
            food.Carbs = Nutrient(food.Carbs, data.Weigth);
            food.Fiber = Nutrient(food.Fiber, data.Weigth);

            return _factory.EditResponse(@$"Você ingeriu:
                                            {food.FoodName},
                                            Tipo: {food.Type},
                                            Calorias: {food.Energy_kcal} Kcal, {food.Energy_kJ} KJ,
                                            Proteínas: {food.Protein} g,
                                            Carboidratos: {food.Carbs} g,
                                            Fibras: {food.Fiber} g,
                                            Peso em gramas: {data.Weigth} g"
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
        public async Task<List<FoodDto>> GetFoodWithChatGPT(string food, int foodNumber)
        {
            //string apiUrl = "https://api.openai.com/v1/chat/completions";
           string apiUrl = _configuration["GPTUrl"];

           string apiKey = _configuration["GPTKey"];

            var chat = new GPTRequest(food, foodNumber);
           
            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(chat);

            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
            List<FoodDto> list = [];

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await client.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var resultBody = Newtonsoft.Json.JsonConvert.DeserializeObject<GPTResponse>(result);
                foreach(var choice in resultBody.choices)
                {
                    var item =  Newtonsoft.Json.JsonConvert.DeserializeObject<FoodDto>(choice.message.content);
                    item.FoodNumber = foodNumber+1 ;
                    list.Add(item);
                }
            }
         
            return list;
        }
        static string GenerateRandomHash()
        {
            using MD5 md5 = MD5.Create();
            byte[] randomBytes = new byte[16]; 
            new RNGCryptoServiceProvider().GetBytes(randomBytes);

            byte[] hashBytes = md5.ComputeHash(randomBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
