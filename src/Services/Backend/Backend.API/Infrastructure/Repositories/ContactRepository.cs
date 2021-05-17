using Backend.API.Infrastructure.Models;
using System;

namespace Backend.API.Infrastructure.Repositories
{
    public class ContactRepository : Repository<Contact, BackendContext>, IContactRepository
    {
        public ContactRepository(BackendContext context) : base(context)
        {
            // Empty
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
