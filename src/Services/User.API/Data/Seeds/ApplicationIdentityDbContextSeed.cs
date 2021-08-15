using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Data.Seeds
{
    public class ApplicationIdentityDbContextSeed
    {
        private readonly IPasswordHasher<User.API.Data.Entities.Identity.User> _passwordHasher 
            = new PasswordHasher<User.API.Data.Entities.Identity.User>();

        public async Task SeedAsync(ApplicationIdentityDbContext context, ILogger<ApplicationIdentityDbContextSeed> logger, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                if (!context.Users.Any())
                {
                    context.Users.AddRange(GetDefaultUser());
                    await context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;
                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationIdentityDbContext));
                    await SeedAsync(context, logger, retryForAvaiability);
                }
            }
        }

        private IEnumerable<User.API.Data.Entities.Identity.User> GetDefaultUser()
        {
            var users = new List<User.API.Data.Entities.Identity.User>();

            var user1 = new User.API.Data.Entities.Identity.User
            {
                Email = "eyup@gevenim.com",
                Id = Guid.NewGuid().ToString(),
                PhoneNumber = "1234567890",
                UserName = "eyup@gevenim.com",
                NormalizedEmail = "EYUP@GEVENIM.COM",
                NormalizedUserName = "EYUP@GEVENIM.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Name= "Eyup Gevenim"
            };
            user1.PasswordHash = _passwordHasher.HashPassword(user1, "Pass@word1");
            users.Add(user1);

            var user2 = new User.API.Data.Entities.Identity.User
            {
                Email = "atest2@user.com",
                Id = Guid.NewGuid().ToString(),
                PhoneNumber = "222-222 2222",
                UserName = "atest2@user.com",
                NormalizedEmail = "ATEST2@USER.COM",
                NormalizedUserName = "ATEST2@USER.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Name = "Atest2 User"
            };
            user2.PasswordHash = _passwordHasher.HashPassword(user2, "Pass@word2");
            users.Add(user2);

            var user3 = new User.API.Data.Entities.Identity.User
            {
                Email = "btest3@cuser.com",
                Id = Guid.NewGuid().ToString(),
                PhoneNumber = "333-333 3333",
                UserName = "btest3@cuser.com",
                NormalizedEmail = "BTEST3@CUSER.COM",
                NormalizedUserName = "BTEST3@CUSER.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Name = "Btest3 Cuser"
            };
            user3.PasswordHash = _passwordHasher.HashPassword(user3, "Pass@word3");
            users.Add(user3);

            var user4 = new User.API.Data.Entities.Identity.User
            {
                Email = "dtest4@duser.com",
                Id = Guid.NewGuid().ToString(),
                PhoneNumber = "444-444 4444",
                UserName = "dtest4@duser.com",
                NormalizedEmail = "DTEST4@DUSER.COM",
                NormalizedUserName = "DTEST4@DUSER.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Name = "Dtest4 Duser"
            };
            user4.PasswordHash = _passwordHasher.HashPassword(user4, "Pass@word4");
            users.Add(user4);

            return users;
        }

    }
}
