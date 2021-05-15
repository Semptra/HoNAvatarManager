using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Attributes;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Ability
{
    [Disabled]
    [EntityParserPriority(1)]
    internal class StateFilesEntityParser : EntityParser
    {
        public StateFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {
    
        }
    
        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }
    
            var avatarDirectoryPath = GetAvatarDirectory(extractedDirectoryPath, avatarKey);
            var avatarStateEntityFiles = Directory.EnumerateFiles(avatarDirectoryPath, "state_*.entity").Select(f => new FileInfo(f));
    
            foreach (var avatarStateEntityFile in avatarStateEntityFiles)
            {
                var destinationPath = Path.Combine(extractedDirectoryPath, avatarStateEntityFile.Name);
                avatarStateEntityFile.CopyTo(destinationPath, true);
            }

            var avatarStateDirectories = Directory.EnumerateDirectories(avatarDirectoryPath).Select(d => new DirectoryInfo(d))
                .Where(d => !d.Name.StartsWith("ability") &&
                            !d.Name.StartsWith("effects") &&
                            !d.Name.StartsWith("sounds") &&
                            !d.Name.StartsWith("clips"));

            foreach (var avatarStateDirectory in avatarStateDirectories)
            {
                CopyFilesRecursively(avatarStateDirectory.FullName, extractedDirectoryPath);
            }
        }
    }
}
