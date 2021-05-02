using System;
using System.Runtime.Serialization;

namespace HoNAvatarManager.Core.Exceptions
{
    [Serializable()]
    public class AnimationNotFoundException : Exception, ISerializable
    {
        public string Animation { get; }

        public AnimationNotFoundException(string message, string animation) : base(message) 
        {
            Animation = animation;
        }

        public AnimationNotFoundException(string message, string animation, Exception inner) : base(message, inner) 
        {
            Animation = animation;
        }

        public AnimationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            
        }
    }
}
