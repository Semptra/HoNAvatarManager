using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Attributes;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Ability
{
    [Disabled]
    [EntityParserPriority(1)]
    internal class AbilityFilesEntityParser : EntityParser
    {
        public AbilityFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {
    
        }
    
        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }
    
            var avatarDirectoryPath = GetAvatarDirectory(extractedDirectoryPath, avatarKey);
            var heroAbilityDirectories = Directory.EnumerateDirectories(extractedDirectoryPath, "ability_*").Select(d => new DirectoryInfo(d));
            var avatarAbilityDirectories = Directory.EnumerateDirectories(avatarDirectoryPath, "ability_*").Select(d => new DirectoryInfo(d));
    
            foreach (var avatarAbilityDirectory in avatarAbilityDirectories)
            {
                var avatarAbilityFiles = avatarAbilityDirectory.EnumerateFiles("*", SearchOption.AllDirectories);
    
                foreach (var avatarAbilityFile in avatarAbilityFiles)
                {
                    var avatarAbilityFileRelativePath = avatarAbilityFile.FullName.Replace(avatarDirectoryPath, string.Empty);
                    var newAvatarAbilityFilePath = new FileInfo(Path.Join(extractedDirectoryPath, avatarAbilityFileRelativePath));
    
                    Directory.CreateDirectory(newAvatarAbilityFilePath.DirectoryName);
    
                    avatarAbilityFile.CopyTo(newAvatarAbilityFilePath.FullName, true);
                }
            }
        }
    }
}
