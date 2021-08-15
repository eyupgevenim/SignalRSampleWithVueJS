namespace User.API.Data.Mapping.Identity
{
    using global::User.API.Data.Entities.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserTokenMapping : EntityTypeConfiguration<UserToken>
    {
        public override void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable($"{nameof(UserToken)}s");
            //TODO:...
            base.Configure(builder);
        }
    }
}
