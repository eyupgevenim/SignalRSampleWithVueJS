namespace User.API.Data.Mapping.Chat
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using User.API.Data.Entities.Chat;

    public class MessageMapping : EntityTypeConfiguration<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable($"{nameof(Message)}s");

            //builder.HasNoKey();
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content).HasMaxLength(1000);
            builder.Property(m => m.CreatedOnUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            builder.Property(m => m.IsOpened).HasDefaultValue(false).IsRequired();

            builder
                .HasOne(m => m.FromUser)
                .WithMany()//(u => u.FromMessages)
                .HasForeignKey(m=> m.FromUserId);

            builder
                .HasOne(m => m.ToUser)
                .WithMany()//(u => u.ToMessages)
                .HasForeignKey(m => m.ToUserId);

            base.Configure(builder);
        }
    }
}
