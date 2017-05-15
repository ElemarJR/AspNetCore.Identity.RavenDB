using System;

namespace AspNetCore.Identity.RavenDB
{
    public class RavenDBIdentityUser
    {
        public string Id { get; internal set; }
        public string UserName { get; internal set; }
        public string NormalizedUserName { get; internal set; }
        public DateTime? DeletedOn { get; internal set; }

        public RavenDBIdentityUser(string userName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }
}
