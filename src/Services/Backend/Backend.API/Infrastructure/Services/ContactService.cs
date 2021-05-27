using Backend.API.Infrastructure.Models;
using Backend.API.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Backend.API.Infrastructure.Services
{
    public class ContactService : IContactService
    {
        private readonly ILogger<ContactService> _logger;
        private IContactRepository _contactRepository;
        private ITokenService _tokenService;

        public ContactService(IContactRepository contactRepository, ITokenService tokenService, ILogger<ContactService> logger)
        {
            _logger = logger;
            _contactRepository = contactRepository;
            _tokenService = tokenService;
        }

        public async void AddIfNotFound(string name, string email)
        {
            _logger.LogDebug("ContactService is generating a token.");

            var contact = await _contactRepository.GetByEmailAsync(email);

            if (null == contact)
            {
                _logger.LogDebug($"ContactService is adding {name}.");

                contact = new Contact
                {
                    Name = name,
                    Email = email,
                };

                var token = _tokenService.Generate();

                contact.Tokens.Add(token);

                _contactRepository.Add(contact);
            }
            else
            {
                _logger.LogDebug($"ContactService is updating {name}.");

                contact.Name = name;
            }

            await _contactRepository.CompleteAsync();
        }
    }
}
