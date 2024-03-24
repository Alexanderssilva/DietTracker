using DietTrackerBot.Domain;
namespace DietTrackerBot.Infra.Interfaces
{
    public interface IUserRepository
    {
        Task SaveUser(string name, Guid id);
        Task<User> FindUser(Guid id);
    }
}
