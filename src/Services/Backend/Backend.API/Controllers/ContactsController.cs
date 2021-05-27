using Backend.API.Infrastructure.Models;
using Backend.API.Infrastructure.Repositories;
using Backend.API.Infrastructure.Services;
using Backend.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;
        private readonly ITokenService _tokenService;

        public ContactsController(IContactRepository contactRepository, ITokenService tokenService)
        {
            _contactRepository = contactRepository;
            _tokenService = tokenService;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactViewModel>>> GetContacts()
        {
            var contacts = await _contactRepository.GetAllAsync();

            return Ok(contacts.Select(p => new ContactViewModel { Email = p.Email, Id = p.Id, Name = p.Name }));
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactViewModel>> GetContact(int id)
        {
            var contact = await _contactRepository.GetAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            var model = new ContactViewModel
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email
            };

            return model;
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, ContactViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var contact = await _contactRepository.GetAsync(model.Id);

            if (contact == null)
            {
                return NotFound();
            }

            contact.Name = model.Name;
            contact.Email = model.Email;

            _contactRepository.Update(contact);

            try
            {
                await _contactRepository.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContactViewModel>> PostContact(ContactViewModel model)
        {
            var contact = new Contact
            {
                Name = model.Name,
                Email = model.Email
            };

            var token = _tokenService.Generate();

            contact.Tokens.Add(token);

            _contactRepository.Add(contact);

            await _contactRepository.CompleteAsync();

            model.Id = contact.Id;

            return CreatedAtAction("GetContact", new { id = contact.Id }, model);
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _contactRepository.GetAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            _contactRepository.Remove(contact);

            await _contactRepository.CompleteAsync();

            return NoContent();
        }

        private bool ContactExists(int id)
        {
            return _contactRepository.Exists(id);
        }
    }
}
