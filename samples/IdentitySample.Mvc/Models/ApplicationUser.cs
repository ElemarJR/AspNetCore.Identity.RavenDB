using AspNetCore.Identity.RavenDB;

namespace IdentitySample.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : RavenDBIdentityUser
    {
    }
}
