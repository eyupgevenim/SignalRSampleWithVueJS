namespace User.API.Data.Entities.Identity
{
    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser<string>
    {
        public string Name { get; set; }
    }
}
