using Backend.API.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.API.Infrastructure.Repositories
{
    public class TokenRepository : Repository<Token, BackendContext>, ITokenRepository
    {
        public TokenRepository(BackendContext context) : base(context)
        {
            // Empty
        }

        public Token GetByKey(string key)
        {
            return Context.Tokens.Where(p => p.Active && key == p.Key).SingleOrDefault();
        }

        public Task<Token> GetByKeyAsync(string key)
        {
            // # Test if case sensive of not?
            return Context.Tokens.Where(p => p.Active && key == p.Key).SingleOrDefaultAsync();
        }
    }
}
