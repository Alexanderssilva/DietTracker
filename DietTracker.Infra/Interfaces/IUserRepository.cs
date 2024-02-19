using DietTrackerBot.Domain;
namespace DietTrackerBot.Infra.Interfaces
{
    public interface IUserRepository
    {
        Task SaveUser(string name, int id);
        Task<User> FindUser(string id);
    }
}
