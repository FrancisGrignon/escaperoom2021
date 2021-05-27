using Flurl.Http;
using System.Threading.Tasks;
using WebAdmin.Models;

namespace WebAdmin.Clients
{
    public interface IBackendClient
    {
        Task<IFlurlResponse> CreateAsync(Contact contact);
    }
}