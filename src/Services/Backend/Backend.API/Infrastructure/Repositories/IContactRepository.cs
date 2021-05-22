using Backend.API.Infrastructure.Models;
using System.Threading.Tasks;

namespace Backend.API.Infrastructure.Repositories
{
    public interface IContactRepository : IRepository<Contact>
    {
        Contact GetByEmail(string email);
        Task<Contact> GetByEmailAsync(string email);
    }
}
