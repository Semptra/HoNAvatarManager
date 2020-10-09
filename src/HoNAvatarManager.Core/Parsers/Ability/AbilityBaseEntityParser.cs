using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Ability
{
    internal abstract class AbilityBaseEntityParser : EntityParser
    {
        public AbilityBaseEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        protected void SetEntityInternal(string heroDirectoryPath, string avatarKey, string entityName)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }

            var heroAbilityDirectories = Directory.EnumerateDirectories(heroDirectoryPath, "ability_*");

            foreach (var heroAbilityDirectory in heroAbilityDirectories)
            {
                var entityFiles = Directory.EnumerateFiles(heroAbilityDirectory, $"{entityName}*.entity");

                foreach(var entityFile in entityFiles)
                {
                    var entityXml = _xmlManager.GetXmlDocument(entityFile);

                    var entityElement = entityXml.QuerySelector(entityName);
                    var entityAvatarElements = entityElement.QuerySelectorAll("altavatar");
                    var entityAvatarElement = entityAvatarElements.FirstOrDefault(a => a.HasKey(avatarKey));

                    if (entityAvatarElement == null)
                    {
                        continue;
                    }

                    entityElement.SetElementAttributes(entityAvatarElement).SetElementChilds(entityAvatarElement);

                    entityXml.SaveXml(entityFile);
                }
            }
        }
    }
}
