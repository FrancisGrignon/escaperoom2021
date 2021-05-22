using Backend.API.Infrastructure.Models;
using Backend.API.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.API.Infrastructure.Services
{
    public class ContactService : IContactService
    {
        private readonly ILogger<ContactService> _logger;
        private IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository, ILogger<ContactService> logger)
        {
            _logger = logger;
            _contactRepository = contactRepository;
        }

        public async void AddIfNotFound(string name, string email)
        {
            var contact = await _contactRepository.GetByEmailAsync(email);

            if (null == contact)
            {
                contact = new Contact
                {
                    Name = name,
                    Email = email
                };

                _contactRepository.Add(contact);
                
                await _contactRepository.CompleteAsync();
            }
        }
    }
}
