using Backend.API.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.API.Infrastructure.Repositories
{
    public class ContactRepository : Repository<Contact, BackendContext>, IContactRepository
    {
        public ContactRepository(BackendContext context) : base(context)
        {
            // Empty
        }

        public Contact GetByEmail(string email)
        {
            return Context.Contacts.Where(p => p.Active && email == p.Email).SingleOrDefault();
        }

        public Task<Contact> GetByEmailAsync(string email)
        {
            return Context.Contacts.Where(p => p.Active && email == p.Email).SingleOrDefaultAsync();
        }

        public override void Remove(Contact leader)
        {
            if (null == leader.Tokens)
            {
                // Ignore
            }
            else
            {
                foreach (var token in leader.Tokens)
                {
                    token.Active = false;
                    token.UpdatedAt = DateTime.UtcNow;
                }
            }

            base.Remove(leader);
        }
    }
}
