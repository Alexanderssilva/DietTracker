using DietTrackerBot.Domain;
using DietTrackerBot.Infra.Context;
using DietTrackerBot.Infra.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DietTrackerBot.Infra
{
    public class UserRepository : IUserRepository
    {
        private readonly MongoContext _context;

        public UserRepository(MongoContext context)
        {
            _context = context;

        }

        public async Task<User> FindUser(string id)
        {
            var filter = Builders<User>.Filter.Eq("Id", id);
            var userFinded = await _context.Users.Find(filter).FirstOrDefaultAsync();
            return userFinded;
        }

        public async Task SaveUser(string name, string id)
        {
            User user = new()
             {
               Name = name,
               _id = id
            };
            await _context.Users.InsertOneAsync(user);
        }

    }
}
