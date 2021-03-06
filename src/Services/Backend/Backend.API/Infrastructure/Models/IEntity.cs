using System;

namespace Backend.API.Infrastructure.Models
{
    public interface IEntity
    {
        int Id { get; set; }

        bool Active { get; set; }

        DateTime CreatedAt { get; set; }

        DateTime UpdatedAt { get; set; }
    }
}
