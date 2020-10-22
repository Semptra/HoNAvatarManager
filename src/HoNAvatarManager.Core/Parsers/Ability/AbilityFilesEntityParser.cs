using System;
using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Ability
{
    internal class AbilityFilesEntityParser : EntityParser
    {
        public AbilityFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }

            var avatarDirectoryPath = GetAvatarDirectory(heroDirectoryPath, avatarKey);
            var heroAbilityDirectories = Directory.EnumerateDirectories(heroDirectoryPath, "ability_*").Select(d => new DirectoryInfo(d));
            var avatarAbilityDirectories = Directory.EnumerateDirectories(avatarDirectoryPath, "ability_*").Select(d => new DirectoryInfo(d));

            foreach (var avatarAbilityDirectory in avatarAbilityDirectories)
            {
                var avatarAbilityFiles = avatarAbilityDirectory.EnumerateFiles("*", SearchOption.AllDirectories);

                foreach (var avatarAbilityFile in avatarAbilityFiles)
                {
                    var avatarAbilityFileRelativePath = avatarAbilityFile.FullName.Replace(avatarDirectoryPath, string.Empty);
                    var newAvatarAbilityFilePath = new FileInfo(Path.Join(heroDirectoryPath, avatarAbilityFileRelativePath));

                    Directory.CreateDirectory(newAvatarAbilityFilePath.DirectoryName);

                    avatarAbilityFile.CopyTo(newAvatarAbilityFilePath.FullName, true);
                }
            }
        }

        private string GetAvatarDirectory(string heroDirectoryPath, string avatarKey)
        {
            var heroDirectories = Directory.EnumerateDirectories(heroDirectoryPath).Select(d => new DirectoryInfo(d));
            var avatarDirectory = heroDirectories.FirstOrDefault(d => string.Equals(d.Name, avatarKey, StringComparison.InvariantCultureIgnoreCase));

            if (avatarDirectory != null)
            {
                return avatarDirectory.FullName;
            }

            var parsedAvatarKey = avatarKey.ParseAvatarKey();
            avatarDirectory = heroDirectories.FirstOrDefault(d => string.Equals(d.Name, parsedAvatarKey, StringComparison.InvariantCultureIgnoreCase));

            if (avatarDirectory != null)
            {
                return avatarDirectory.FullName;
            }

            var entityFilePath = GetHeroEntityPath(heroDirectoryPath);
            var entityXml = _xmlManager.GetXmlDocument(entityFilePath);

            var entityElement = entityXml.QuerySelector("hero");
            var entityAvatarElements = entityElement.QuerySelectorAll("altavatar");
            var entityAvatarElement = entityAvatarElements.FirstOrDefault(a => a.HasKey(avatarKey));

            var modelDirectory = entityAvatarElement.GetAttribute("model").Split("/").First();
            var avatarDirectoryPath = Path.Combine(heroDirectoryPath, modelDirectory);

            if (Directory.Exists(avatarDirectoryPath))
            {
                return avatarDirectoryPath;
            }

            throw new DirectoryNotFoundException($"Directory not found for avatar {avatarKey}.");
        }

        protected string GetHeroEntityPath(string heroDirectoryPath)
        {
            var heroEntityFilePath = Path.Combine(heroDirectoryPath, $"hero.entity");

            if (File.Exists(heroEntityFilePath))
            {
                return heroEntityFilePath;
            }

            var heroDirectoryPathInfo = new DirectoryInfo(heroDirectoryPath);
            var heroNameEntityFilePath = Path.Combine(heroDirectoryPath, $"{heroDirectoryPathInfo.Name}.entity");

            if (File.Exists(heroNameEntityFilePath))
            {
                return heroNameEntityFilePath;
            }

            throw new FileNotFoundException("Hero entity file not found.", heroNameEntityFilePath);
        }
    }
}
