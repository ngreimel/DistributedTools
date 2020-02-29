using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DelegationServer.Domain
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
        Guid AddUser(string name, UserType type);
        void RemoveUser(Guid userId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, User> Users = new ConcurrentDictionary<Guid, User>();
        
        public List<User> GetAllUsers()
        {
            return Users.Values.ToList();
        }

        public Guid AddUser(string name, UserType type)
        {
            var userId = Guid.NewGuid();
            Users[userId] = new User
            {
                UserId = userId,
                Name = name,
                Type = type
            };
            return userId;
        }

        public void RemoveUser(Guid userId)
        {
            Users.TryRemove(userId, out var _);
        }
    }
}