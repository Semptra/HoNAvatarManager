using System;
using System.Runtime.Serialization;

namespace HoNAvatarManager.Core.Exceptions
{
    [Serializable()]
    public class AvatarNotFoundException : Exception, ISerializable
    {
        public string Avatar { get; }

        public AvatarNotFoundException(string message, string avatar) : base(message) 
        {
            Avatar = avatar;
        }

        public AvatarNotFoundException(string message, string avatar, Exception inner) : base(message, inner) 
        {
            Avatar = avatar;
        }

        public AvatarNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            
        }
    }
}
