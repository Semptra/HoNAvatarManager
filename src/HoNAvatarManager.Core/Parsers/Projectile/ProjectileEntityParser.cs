using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Attributes;
using HoNAvatarManager.Core.Extensions;
using Logger = HoNAvatarManager.Core.Logging.Logger;

namespace HoNAvatarManager.Core.Parsers.Projectile
{
    [EntityParserPriority(2)]
    internal class ProjectileEntityParser : EntityParser
    {
        public ProjectileEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }

            var projectileEntityFiles = Directory.EnumerateFiles(resultDirectoryPath, $"*projectile*.entity", SearchOption.AllDirectories);

            foreach (var projectileEntityFile in projectileEntityFiles)
            {
                var projectileEntityXml = _xmlManager.GetXmlDocument(projectileEntityFile);
                var projectileEntityElement = projectileEntityXml.QuerySelector("projectile");

                if (projectileEntityElement == null)
                {
                    continue;
                }

                var entityAvatarElements = projectileEntityElement.QuerySelectorAll("altavatar");
                var entityAvatarElement = entityAvatarElements.FirstOrDefault(a => a.HasKey(avatarKey));

                if (entityAvatarElement == null)
                {
                    continue;
                }

                Logger.Log.Information("  Set entity attributes for file {0}", new FileInfo(projectileEntityFile).Name);

                projectileEntityElement.SetElementAttributes(entityAvatarElement, SkippedAttributes).SetElementChilds(entityAvatarElement);

                projectileEntityXml.SaveXml(projectileEntityFile);
            }
        }
    }
}
