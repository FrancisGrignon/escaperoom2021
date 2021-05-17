using System;
using System.Collections.Generic;

namespace Backend.API.Infrastructure.Models
{
    public class Contact : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<Token> Tokens { get; } = new List<Token>();

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
