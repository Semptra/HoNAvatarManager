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
            SetEntityInternal(heroDirectoryPath, avatarKey, "hero");
        }

        protected void SetEntityInternal(string heroDirectoryPath, string avatarKey, string entityName)
        {
            var heroEntityFilePath = Path.Combine(heroDirectoryPath, $"{entityName}.entity");

            if (File.Exists(heroEntityFilePath))
            {
                SetHeroEntity(heroEntityFilePath, avatarKey, entityName);
                return;
            }

            var heroDirectoryPathInfo = new DirectoryInfo(heroDirectoryPath);
            var heroNameEntityFilePath = Path.Combine(heroDirectoryPath, $"{heroDirectoryPathInfo.Name}.entity");

            if (File.Exists(heroNameEntityFilePath))
            {
                SetHeroEntity(heroNameEntityFilePath, avatarKey, entityName);
                return;
            }

            throw new FileNotFoundException("Hero entity file not found.", heroNameEntityFilePath);
        }

        protected void SetHeroEntity(string entityFilePath, string avatarKey, string entityName)
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
