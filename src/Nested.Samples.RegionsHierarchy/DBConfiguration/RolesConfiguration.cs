using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nested.Samples.RegionsHierarchy.DBConfiguration
{
    public class RolesConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasMany<Permission>(i => i.Permissions).WithOne(p => p.Role);
        }
    }
}