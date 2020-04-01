using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DistributedToolsServer.Domain
{
    public interface IUserGroup
    {
        List<UserWithType> GetAllUsers();
        void AddUser(User user, UserType type);
        void RemoveUser(Guid userId);
    }

    public class UserGroup : IUserGroup
    {
        private readonly ConcurrentDictionary<Guid, UserWithType> users = new ConcurrentDictionary<Guid, UserWithType>();

        public List<UserWithType> GetAllUsers()
        {
            return users.Values.ToList();
        }

        public void AddUser(User user, UserType type)
        {
            users[user.UserId] = new UserWithType
            {
                UserId = user.UserId,
                Name = user.Name,
                Type = type
            };
        }

        public void RemoveUser(Guid userId)
        {
            users.TryRemove(userId, out var _);
        }
    }
}
