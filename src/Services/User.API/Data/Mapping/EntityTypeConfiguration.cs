namespace User.API.Data.Mapping
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EntityTypeConfiguration<TEntity> : IMappingConfiguration, IEntityTypeConfiguration<TEntity> where TEntity : class
    {
        protected virtual void PostConfigure(EntityTypeBuilder<TEntity> builder) { }
        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => this.PostConfigure(builder);
        public virtual void ApplyConfiguration(ModelBuilder modelBuilder) => modelBuilder.ApplyConfiguration(this);
    }
}
