using Backend.API.Infrastructure.Models;
using System.Threading.Tasks;

namespace Backend.API.Infrastructure.Repositories
{
    public interface ITokenRepository : IRepository<Token>
    {
        Token GetByKey(string key);
        Task<Token> GetByKeyAsync(string key);
    }
}
