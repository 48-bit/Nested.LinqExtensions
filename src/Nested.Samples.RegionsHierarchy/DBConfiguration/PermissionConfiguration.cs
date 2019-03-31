using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nested.Samples.RegionsHierarchy.DBConfiguration
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasOne<Role>(p => p.Role).WithMany(r => r.Permissions);
        }
    }
}