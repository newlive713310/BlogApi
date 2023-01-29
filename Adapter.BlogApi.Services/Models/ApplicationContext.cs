using Adapter.BlogApi.Services.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adapter.BlogApi.Services.Models
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    }
}
