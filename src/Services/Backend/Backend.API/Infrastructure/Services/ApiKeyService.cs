using Microsoft.Extensions.Configuration;

namespace Backend.API.Infrastructure.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private string ApiKey { get; }

        public ApiKeyService(IConfiguration configuration)
        {
            ApiKey = configuration.GetValue<string>("BackendApiKey");
        }

        public bool Validate(string apiKey)
        {
            return ApiKey.Equals(apiKey);
        }
    }
}
