namespace User.API.Data.Mapping.Identity
{
    using global::User.API.Data.Entities.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RoleMapping : EntityTypeConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable($"{nameof(Role)}s");
            //TODO:...
            base.Configure(builder);
        }
    }
}
