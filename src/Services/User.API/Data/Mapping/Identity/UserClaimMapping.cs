namespace User.API.Data.Mapping.Identity
{
    using global::User.API.Data.Entities.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserClaimMapping : EntityTypeConfiguration<UserClaim>
    {
        public override void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.ToTable($"{nameof(UserClaim)}s");
            //TODO:...
            base.Configure(builder);
        }
    }
}
