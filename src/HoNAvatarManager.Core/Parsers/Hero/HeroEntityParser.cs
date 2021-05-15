using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Attributes;
using HoNAvatarManager.Core.Extensions;
using HoNAvatarManager.Core.Helpers;
using Logger = HoNAvatarManager.Core.Logging.Logger;

namespace HoNAvatarManager.Core.Parsers.Hero
{
    [EntityParserPriority(2)]
    internal class HeroEntityParser : EntityParser
    {
        public HeroEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            SetEntityInternal(resultDirectoryPath, avatarKey, "hero");
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

            throw ThrowHelper.FileNotFoundException("Hero entity file not found.", heroNameEntityFilePath);
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

            Logger.Log.Information("  Set entity attributes for file {0}", new FileInfo(entityFilePath).Name);

            entityElement.SetElementAttributes(entityAvatarElement, SkippedAttributes).SetElementChilds(entityAvatarElement);

            entityXml.SaveXml(entityFilePath);
        }
    }
}
