namespace User.API.Data.Mapping.Identity
{
    using global::User.API.Data.Entities.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserLoginMapping : EntityTypeConfiguration<UserLogin>
    {
        public override void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable($"{nameof(UserLogin)}s");
            //TODO:...
            base.Configure(builder);
        }
    }
}
