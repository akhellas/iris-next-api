
using System.Collections.Generic;

namespace Iris.Api.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            Roles = new List<string>();
            Claims = new List<UserClaim>();
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public ICollection<string> Roles { get; set; }
        public ICollection<UserClaim> Claims { get; set; }
    }

    public class UserClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}