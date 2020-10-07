using System;
using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Hero
{
    internal class HeroEntityParser : EntityParser
    {
        public HeroEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            SetEntityInternal(heroDirectoryPath, avatarKey, "hero", "hero");
        }

        protected void SetEntityInternal(string heroDirectoryPath, string avatarKey, string entityName, string entityFileName)
        {
            var entityFilePath = Path.Combine(heroDirectoryPath, $"{entityFileName}.entity");

            if (!File.Exists(entityFilePath))
            {
                return;
            }

            var entityXml = _xmlManager.GetXmlDocument(entityFilePath);

            var entityElement = entityXml.QuerySelector(entityName);
            var entityAvatarElements = entityElement.QuerySelectorAll("altavatar");
            var entityAvatarElement = entityAvatarElements.FirstOrDefault(a => a.HasKey(avatarKey));

            if (entityAvatarElement == null)
            {
                return;
            }

            entityElement.SetElementAttributes(entityAvatarElement).SetElementChilds(entityAvatarElement);

            entityXml.SaveXml(entityFilePath);
        }
    }
}
