using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Attributes;
using Logger = HoNAvatarManager.Core.Logging.Logger;

namespace HoNAvatarManager.Core.Parsers.Model
{
    [EntityParserPriority(0)]
    internal class OriginalResourcesFilesEntityParser : ResourcesFilesEntityParserBase
    {
        public OriginalResourcesFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            var directoriesToCopy = Directory.EnumerateDirectories(extractedDirectoryPath)
                .Select(d => new DirectoryInfo(d))
                .Where(d => DirectoriesToCopy.Any(directory => d.Name.StartsWith(directory)));

            Logger.Log.Information("  Copy hero directories.");

            foreach (var directoryToCopy in directoriesToCopy)
            {
                Logger.Log.Information("    Copy {0} hero directory.", directoryToCopy.Name);
                
                var targetDirectoryPath = Path.Combine(resultDirectoryPath, directoryToCopy.Name);
                CopyFilesRecursively(directoryToCopy.FullName, targetDirectoryPath);
            }

            var filesToCopy = Directory.EnumerateFiles(extractedDirectoryPath, "*", SearchOption.TopDirectoryOnly)
                .Select(f => new FileInfo(f));

            Logger.Log.Information("  Copy hero files.");

            foreach (var fileToCopy in filesToCopy)
            {
                Logger.Log.Information("    Copy {0} hero file.", fileToCopy.Name);

                var targetFilePath = Path.Combine(resultDirectoryPath, fileToCopy.Name);
                fileToCopy.CopyTo(targetFilePath, true);
            }
        }
    }
}
