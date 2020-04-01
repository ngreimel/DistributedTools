using System;
using Microsoft.AspNetCore.Http;

namespace DistributedToolsServer.Domain
{
    public interface ICurrentUserAccessor
    {
        User GetCurrentUser();
    }

    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRepository userRepository;

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userRepository = userRepository;
        }

        public User GetCurrentUser()
        {
            httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("el-dt-user-id", out var rawUserId);
            if (Guid.TryParse(rawUserId, out var userId))
            {
                return userRepository.GetUser(userId);
            }

            return null;
        }
    }
}
