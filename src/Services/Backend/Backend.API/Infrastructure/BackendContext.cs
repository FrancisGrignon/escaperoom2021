using Backend.API.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Backend.API.Infrastructure
{
    public class BackendContext : DbContext
    {
        public BackendContext(DbContextOptions<BackendContext> options) : base(options)
        {
            // Empty
        }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Token> Tokens { get; set; }
    }

    public class BackendContextDesignFactory : IDesignTimeDbContextFactory<BackendContext>
    {
        public BackendContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BackendContext>()
                .UseSqlServer("Server=.;Initial Catalog=EscapeRoom2021.Services.Backend;Integrated Security=true");

            return new BackendContext(optionsBuilder.Options);
        }
    }
}