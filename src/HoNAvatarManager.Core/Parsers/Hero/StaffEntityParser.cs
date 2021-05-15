using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Attributes;
using HoNAvatarManager.Core.Extensions;
using Logger = HoNAvatarManager.Core.Logging.Logger;

namespace HoNAvatarManager.Core.Parsers.Hero
{
    [EntityParserPriority(2)]
    internal class StaffEntityParser : EntityParser
    {
        public StaffEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            SetEntityInternal(resultDirectoryPath, avatarKey, "state");
        }

        protected void SetEntityInternal(string heroDirectoryPath, string avatarKey, string entityName)
        {
            var staffEntityFilePath = GetStaffEntityFile(heroDirectoryPath);

            if (string.IsNullOrEmpty(staffEntityFilePath))
            {
                Logger.Log.Warning("Staff entity file not found for avatar {0}", avatarKey);
                return;
            }

            var entityXml = _xmlManager.GetXmlDocument(staffEntityFilePath);

            var entityElement = entityXml.QuerySelector(entityName);
            var entityAvatarElements = entityElement.QuerySelectorAll("altavatar");
            var entityAvatarElement = entityAvatarElements.FirstOrDefault(a => a.HasKey(avatarKey));

            if (entityAvatarElement == null)
            {
                return;
            }

            Logger.Log.Information("  Set entity attributes for file {0}", new FileInfo(staffEntityFilePath).Name);

            entityElement.SetElementAttributes(entityAvatarElement, SkippedAttributes).SetElementChilds(entityAvatarElement);

            entityXml.SaveXml(staffEntityFilePath);
        }

        private string GetStaffEntityFile(string heroDirectoryPath)
        {
            var defaultStaffEntityFile = Path.Combine(heroDirectoryPath, "state_ult_boost.entity");

            if (File.Exists(defaultStaffEntityFile))
            {
                return defaultStaffEntityFile;
            }

            var staffEntityFiles = Directory.EnumerateFiles(heroDirectoryPath, "state_*_ult_boost.entity");

            return staffEntityFiles.FirstOrDefault();
        }
    }
}
