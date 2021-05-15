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

        public static IElement SetElementAttributes(this IElement thisElement, IElement otherElement, params string[] skipAttributes)
        {
            var otherElementAttributes = otherElement.Attributes.Where(a => a.Name != "key");

            foreach (var attribute in otherElementAttributes.Where(attribute => !skipAttributes.Contains(attribute.Name)))
            {
                Logging.Logger.Log.Information("    Set attribute {0} with value {1}", attribute.Name, attribute.Value);

                thisElement.SetAttribute(attribute.Name, attribute.Value);
            }

            return thisElement;
        }

        public static IElement SetElementChilds(this IElement thisElement, IElement otherElement)
        {
            foreach (var childElement in otherElement.ChildNodes.OfType<IElement>().ToList())
            {
                thisElement.CopyNodeToRoot(childElement);
            }

            return thisElement;
        }

        public static IElement RemoveChildNodes(this IElement thisElement, params string[] childs)
        {
            foreach (var child in childs)
            {
                var childsToRemove = thisElement.ChildNodes.OfType<IElement>().Where(e => childs.Any(c => e.NodeName == c)).ToList();

                foreach (var childToRemove in childsToRemove)
                {
                    childToRemove.Remove();
                }
            }

            return thisElement;
        }

        public static IElement CopyNodeToRoot(this IElement thisElement, IElement otherElement)
        {
            INode rootElementNode;

            if (otherElement.HasAttribute("key"))
            {
                var otherElementKey = otherElement.GetAttribute("key");
                rootElementNode = thisElement.ChildNodes.OfType<IElement>().FirstOrDefault(e => e.NodeName == otherElement.NodeName && e.GetAttribute("key") == otherElementKey);
            }
            else
            {
                rootElementNode = thisElement.ChildNodes.FirstOrDefault(n => n.NodeName == otherElement.NodeName);
            }

            if (rootElementNode != null)
            {
                thisElement.RemoveChild(rootElementNode);
            }

            thisElement.AppendChild(otherElement);

            return thisElement;
        }
    }
}
