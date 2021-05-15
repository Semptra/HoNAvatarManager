using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Attributes;
using Logger = HoNAvatarManager.Core.Logging.Logger;

namespace HoNAvatarManager.Core.Parsers.Model
{
    [EntityParserPriority(1)]
    internal class AvatarResourcesFilesEntityParser : ResourcesFilesEntityParserBase
    {
        public AvatarResourcesFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            var avatarDirectory = GetAvatarDirectory(extractedDirectoryPath, avatarKey);

            var avatarSubDirectories = Directory.EnumerateDirectories(avatarDirectory)
                .Select(d => new DirectoryInfo(d))
                .Where(d => DirectoriesToCopy.Any(directory => d.Name.StartsWith(directory)));

            foreach (var avatarSubDirectory in avatarSubDirectories)
            {
                Logger.Log.Information("  Copy {0} avatar directory.", avatarSubDirectory.Name);

                var resultSubDirectory = Path.Combine(resultDirectoryPath, avatarSubDirectory.Name);

                CopyFilesRecursively(avatarSubDirectory.FullName, resultSubDirectory);
            }

            var avatarFiles = Directory.EnumerateFiles(avatarDirectory).Select(f => new FileInfo(f));

            foreach (var avatarFile in avatarFiles)
            {
                var resultFilePath = Path.Combine(resultDirectoryPath, avatarFile.Name);
                avatarFile.CopyTo(resultFilePath, true);
            }
        }
    }
}
