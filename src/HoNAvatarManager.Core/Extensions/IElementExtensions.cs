using System;
using System.Linq;
using AngleSharp.Dom;

namespace HoNAvatarManager.Core.Extensions
{
    internal static class IElementExtensions
    {
        public static bool HasKey(this IElement element, string key)
        {
            var elementKey = element.GetAttribute("key");

            if (string.Equals(elementKey, key, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            
            return string.Equals(elementKey.ParseAvatarKey(), key, StringComparison.InvariantCultureIgnoreCase);
        }

        public static IElement SetElementAttributes(this IElement thisElement, IElement otherElement)
        {
            var otherElementAttributes = otherElement.Attributes.Where(a => a.Name != "key");

            foreach (var attribute in otherElementAttributes)
            {
                thisElement.SetAttribute(attribute.Name, attribute.Value);
            }

            return thisElement;
        }

        public static IElement SetElementChilds(this IElement thisElement, IElement otherElement)
        {
            foreach (var childNode in otherElement.ChildNodes.ToList())
            {
                thisElement.CopyNodeToRoot(childNode);
            }

            return thisElement;
        }

        public static IElement CopyNodeToRoot(this IElement thisElement, INode node)
        {
            var rootElementNode = thisElement.ChildNodes.FirstOrDefault(n => n.NodeName == node.NodeName);

            if (rootElementNode != null)
            {
                thisElement.RemoveChild(rootElementNode);
            }

            thisElement.AppendChild(node);

            return thisElement;
        }
    }
}
