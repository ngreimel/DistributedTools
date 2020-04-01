using System;

namespace DistributedToolsServer.Models
{
    public class RoomCodeAndUser
    {
        public RoomCodeAndUser(string roomCode, Guid? userId)
        {
            RoomCode = roomCode;
            UserId = userId;
        }

        public string RoomCode { get; }
        public Guid? UserId { get; }
    }
}
