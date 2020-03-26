using System;
using System.Linq;

namespace DistributedToolsServer.Domain
{
    public class RoomCodeGenerator
    {
        private static char[] validCharacters = "23456789ABCFGHJKPRSTWXYZ".ToCharArray();
        private static Random random = new Random();
        
        public string Generate()
        {
            var characters = Enumerable.Range(0, 4).Select(_ => validCharacters[random.Next(0, validCharacters.Length)]);
            return string.Join("", characters);
        }
    }
}