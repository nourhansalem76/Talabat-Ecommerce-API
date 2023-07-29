using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public class AppIdentityDbSeed
    {
        public static async Task UserSeeding(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName= "Ebtesam Mahmoud",
                    Email = "ebtesammahmoud200@gmail.com",
                    UserName="Ebtesam.Mahmoud",
                    PhoneNumber="0123456789"

                };
                await userManager.CreateAsync(user,"P@ssw0rd");
            }
        }
    }
}
