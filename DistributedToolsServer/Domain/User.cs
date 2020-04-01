using System;

namespace DistributedToolsServer.Domain
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }

    public class UserWithType
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public UserType Type { get; set; }
    }

    public enum UserType
    {
        Voter,
        Admin
    }
}
