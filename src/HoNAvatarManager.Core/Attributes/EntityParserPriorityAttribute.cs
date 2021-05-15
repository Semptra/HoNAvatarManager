using System;

namespace HoNAvatarManager.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityParserPriorityAttribute : Attribute
    {
        public int Priority { get; set; }

        public EntityParserPriorityAttribute(int priority = -1)
        {
            Priority = priority;
        }
    }
}
