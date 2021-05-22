namespace Backend.API.Infrastructure.Services
{
    public interface IContactService
    {
        void AddIfNotFound(string name, string email);
    }
}