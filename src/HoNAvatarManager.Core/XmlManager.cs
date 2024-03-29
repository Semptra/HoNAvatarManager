﻿using System.IO;
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
    }
}
