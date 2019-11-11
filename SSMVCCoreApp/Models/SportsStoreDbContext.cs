using Microsoft.EntityFrameworkCore;
using SSMVCCoreApp.Models.Entities;

namespace SSMVCCoreApp.Models
{
  public class SportsStoreDbContext : DbContext
  {
    public SportsStoreDbContext(DbContextOptions<SportsStoreDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
  }
}
