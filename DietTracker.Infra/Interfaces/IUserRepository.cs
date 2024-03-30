using DietTrackerBot.Domain;
namespace DietTrackerBot.Infra.Interfaces
{
    public interface IUserRepository
    {
        Task SaveUser(string name, string id);
        Task<User> FindUser(string id);
    }
}
