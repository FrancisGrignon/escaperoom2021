namespace Backend.API.Infrastructure.Services
{
    public interface IApiKeyService
    {
        bool Validate(string apiKey);
    }
}