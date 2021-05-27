using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WebAdmin.Models;

namespace WebAdmin.Clients
{
    public class BackendClient : IBackendClient
    {
        public const string API_KEY_HEADER_NAME = "x-api-key";

        private readonly string _baseUrl;
        private readonly string _apiKey;

        public BackendClient(IConfiguration config)
        {
            _baseUrl = config["BackendUri"];
            _apiKey = config["BackendApiKey"];
        }

        public async Task<IFlurlResponse> CreateAsync(Contact contact)
        {
            return await _baseUrl
                .AppendPathSegment($"contacts")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .PostJsonAsync(contact);
        }
    }
}