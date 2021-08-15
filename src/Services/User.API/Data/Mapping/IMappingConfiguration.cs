namespace User.API.Data.Mapping
{
    using Microsoft.EntityFrameworkCore;

    public interface IMappingConfiguration
    {
        void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}
