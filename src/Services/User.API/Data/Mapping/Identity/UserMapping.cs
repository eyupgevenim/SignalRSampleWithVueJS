namespace User.API.Data.Mapping.Identity
{
    using global::User.API.Data.Entities.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserMapping : EntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable($"{nameof(User)}s");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(100);
            builder.Property(x => x.Email).HasMaxLength(100).IsRequired();

            //TODO:...
            //builder.Property(x => x.CreatedOnUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            //builder.Property(x => x.Deleted).HasDefaultValue(false).IsRequired();

            base.Configure(builder);
        }
    }
}
