using DietTrackerBot.Application.Dto;
using DietTrackerBot.Application.Factories;
using DietTrackerBot.Application.Strategies.Interfaces;
using DietTrackerBot.Domain;
using DietTrackerBot.Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DietTrackerBot.Application.Strategies.TextStrategies
{
    public class CaloriesCountStrategy(IFoodRepository foodRepository,
                                       IMealRepository mealRepository,
                                       IUserRepository userRepository,
                                       IResponseFactory factory,
                                       IConfiguration configuration) : ITextStrategy
        {
            private readonly IFoodRepository _foodRepository = foodRepository;
            private readonly IMealRepository _mealRepository = mealRepository;
            private readonly IUserRepository _userRepository = userRepository;
            private readonly IResponseFactory _factory = factory;
            private readonly IConfiguration _configuration = configuration;


        public async Task<ResponseDto> HandleText(Update update)
        {
            Dictionary<string, double> foods = ConvertToDictionary(update.Message.Text, "#calorias:");
            List<List<FoodDto>> list = [];
            string mealId = GenerateRandomHash();
            foreach (var food in foods)
            {
                var responses = await _foodRepository.SearchFoods(food.Key);
                if (responses.Any())
                {
                    var foodDtos = responses.Select(response => FoodDto.ToDto(response))
                                            .Select(dto =>
                                            {
                                                dto.CallBackData = new MealDto()
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
                    int totalFoods = await _foodRepository.FoodCount();
                    var foodDtos = await GetFoodWithChatGPT(food.Key, totalFoods);
                    foodDtos.ForEach(async foodDto =>
                    {
                        Food food2 = FoodDto.ToFood(foodDto);
                        await _foodRepository.InsertFood(food2);
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

        public async Task<List<FoodDto>> GetFoodWithChatGPT(string food, int foodNumber)
        {
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
                foreach (var choice in resultBody.choices)
                {
                    var item = Newtonsoft.Json.JsonConvert.DeserializeObject<FoodDto>(choice.message.content);
                    item.FoodNumber = foodNumber + 1;
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
