using Microsoft.EntityFrameworkCore;
using Showcase.VerticalSlice.Entities;

namespace Showcase.VerticalSlice.Database
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Article> Article { get; set; }
    }
}