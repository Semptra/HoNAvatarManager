using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AngleSharp.Dom;
using AngleSharp.Xml.Dom;
using AngleSharp.Xml.Parser;

namespace HoNAvatarManager.Core
{
    internal class XmlManager
    {
        public IXmlDocument GetXmlDocument(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var parser = new XmlParser();
                return parser.ParseDocument(stream);
            }
        }

        public void SaveXml(string path, string xml)
        {
            var element = XElement.Parse(xml);

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                Indent = true,
                NewLineOnAttributes = true
            };

            using (var xmlWriter = XmlWriter.Create(path, settings))
            {
                element.Save(xmlWriter);
            }
        }

        public void CopyNodeToRoot(IElement rootElement, INode node)
        {
            var rootElementNode = rootElement.ChildNodes.FirstOrDefault(n => n.NodeName == node.NodeName);

            if (rootElementNode != null)
            {
                rootElement.RemoveChild(rootElementNode);
            }

            rootElement.AppendChild(node);
        }
    }
}
