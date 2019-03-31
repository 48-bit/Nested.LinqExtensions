using Microsoft.EntityFrameworkCore;
using Nested.LinqExtensions;
using Nested.Samples.RegionsHierarchy.DBConfiguration;
using Nested.Samples.RegionsHierarchy.Utils;

namespace Nested.Samples.RegionsHierarchy
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) :base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors(true);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RegionConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RolesConfiguration());
        }

        public DbSet<TreeEntry> ResourcesHierarchies { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Permission> Permissions { get; set; }
    }
}