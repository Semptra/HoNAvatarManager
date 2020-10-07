using System.Xml;
using System.Xml.Linq;
using AngleSharp.Xml;
using AngleSharp.Xml.Dom;

namespace HoNAvatarManager.Core.Extensions
{
    internal static class IXmlDocumentExtensions
    {
        public static void SaveXml(this IXmlDocument document, string path)
        {
            var element = XElement.Parse(document.ToXml());

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
    }
}
