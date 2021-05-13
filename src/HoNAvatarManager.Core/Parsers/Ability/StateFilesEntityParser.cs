using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Ability
{
    internal class StateFilesEntityParser : EntityParser
    {
        public StateFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {
    
        }
    
        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }
    
            var avatarDirectoryPath = GetAvatarDirectory(heroDirectoryPath, avatarKey);
            var avatarStateEntityFiles = Directory.EnumerateFiles(avatarDirectoryPath, "state_*.entity").Select(f => new FileInfo(f));
    
            foreach (var avatarStateEntityFile in avatarStateEntityFiles)
            {
                var destinationPath = Path.Combine(heroDirectoryPath, avatarStateEntityFile.Name);
                avatarStateEntityFile.CopyTo(destinationPath, true);
            }

            var avatarStateDirectories = Directory.EnumerateDirectories(avatarDirectoryPath).Select(d => new DirectoryInfo(d))
                .Where(d => !d.Name.StartsWith("ability") &&
                            !d.Name.StartsWith("effects") &&
                            !d.Name.StartsWith("sounds") &&
                            !d.Name.StartsWith("clips"));

            foreach (var avatarStateDirectory in avatarStateDirectories)
            {
                CopyFilesRecursively(avatarStateDirectory.FullName, heroDirectoryPath);
            }
        }

        private void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}
