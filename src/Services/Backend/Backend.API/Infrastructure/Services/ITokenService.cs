using Backend.API.Infrastructure.Models;
using System.Threading.Tasks;

namespace Backend.API.Infrastructure.Services
{
    public interface ITokenService
    {
        public const int TOKEN_VALID = 0;
        public const int TOKEN_NOTFOUND = 1;
        public const int TOKEN_EXPIRED = 2;

        Token Generate(Contact contact);
        Token Generate();
        Task<int> UseAsync(string key);
    }
}