using System;

namespace Backend.API.Infrastructure.Models
{
    public class Token : IEntity
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public Contact Contact { get; set; }

        public int ContactId { get; set; }

        public DateTime UsedAt { get; set; }

        public DateTime ExpiredAt { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
