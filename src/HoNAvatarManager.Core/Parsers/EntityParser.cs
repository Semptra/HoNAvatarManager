using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;
using HoNAvatarManager.Core.Helpers;

namespace HoNAvatarManager.Core.Parsers
{
    internal abstract class EntityParser : IEntityParser
    {
        protected readonly XmlManager _xmlManager;

        public EntityParser(XmlManager xmlManager)
        {
            _xmlManager = xmlManager;
        }

        public static IEnumerable<IEntityParser> GetRegisteredEntityParsers(XmlManager xmlManager)
        {
            var entityParserType = typeof(IEntityParser);

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => entityParserType.IsAssignableFrom(p) && !p.IsAbstract)
                .Select(t => Activator.CreateInstance(t, xmlManager))
                .OfType<IEntityParser>();
        }

        public abstract void SetEntity(string heroDirectoryPath, string avatarKey);

        protected string GetAvatarDirectory(string heroDirectoryPath, string avatarKey)
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

            throw ThrowHelper.DirectoryNotFoundException($"Directory not found for avatar {avatarKey}.");
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

            throw ThrowHelper.FileNotFoundException("Hero entity file not found.", heroNameEntityFilePath);
        }
    }
}
