using Microsoft.EntityFrameworkCore;
using Cleanup.Models;

namespace Cleanup.Models
{
    public class CleanupContext : DbContext
    {
        public CleanupContext(DbContextOptions<CleanupContext> options) : base(options) {}
        public DbSet<User> users {get;set;}
        public DbSet<Cleanup> cleanups {get;set;}
        public DbSet<Message> messages {get;set;}
        public DbSet<Image> images {get;set;}
    }
}