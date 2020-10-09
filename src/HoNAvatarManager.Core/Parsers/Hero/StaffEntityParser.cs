using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Hero
{
    internal class StaffEntityParser : EntityParser
    {
        public StaffEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            SetEntityInternal(heroDirectoryPath, avatarKey, "state");
        }

        protected void SetEntityInternal(string heroDirectoryPath, string avatarKey, string entityName)
        {
            var entityFiles = Directory.EnumerateFiles(heroDirectoryPath, "state_*_ult_boost.entity");

            foreach (var entityFilePath in entityFiles)
            {
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
}
