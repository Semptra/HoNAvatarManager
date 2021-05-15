using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;
using Logger = HoNAvatarManager.Core.Logging.Logger;

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
                var entityFiles = Directory.EnumerateFiles(heroAbilityDirectory, $"*{entityName}*.entity", SearchOption.AllDirectories);

                foreach (var entityFile in entityFiles)
                {
                    var entityXml = _xmlManager.GetXmlDocument(entityFile);

                    var entityElement = entityXml.QuerySelector(entityName);

                    if (entityElement == null)
                    {
                        continue;
                    }

                    var entityAvatarElements = entityElement.QuerySelectorAll("altavatar");
                    var entityAvatarElement = entityAvatarElements.FirstOrDefault(a => a.HasKey(avatarKey));

                    if (entityAvatarElement == null)
                    {
                        continue;
                    }

                    Logger.Log.Information("  Set entity attributes for file {0}", new FileInfo(entityFile).Name);

                    entityElement.SetElementAttributes(entityAvatarElement, SkippedAttributes).SetElementChilds(entityAvatarElement);

                    entityXml.SaveXml(entityFile);
                }
            }
        }
    }
}
