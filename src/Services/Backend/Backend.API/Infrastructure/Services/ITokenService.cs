using Backend.API.Infrastructure.Models;

namespace Backend.API.Infrastructure.Services
{
    public interface ITokenService
    {
        Token Generate(Contact contact);
        int Use(string key);
    }
}