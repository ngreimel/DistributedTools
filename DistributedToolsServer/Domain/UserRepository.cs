using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DistributedToolsServer.Domain
{
    public interface IUserRepository
    {
        Guid AddUser(string name);
        User GetUser(Guid userId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, User> Users = new ConcurrentDictionary<Guid, User>();

        public Guid AddUser(string name)
        {
            var userId = Guid.NewGuid();
            Users[userId] = new User
            {
                UserId = userId,
                Name = name
            };
            return userId;
        }

        public User GetUser(Guid userId)
        {
            return Users.GetValueOrDefault(userId);
        }
    }
}
