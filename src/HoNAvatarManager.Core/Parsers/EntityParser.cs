using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AngleSharp.Xml.Dom;
using HoNAvatarManager.Core.Attributes;
using HoNAvatarManager.Core.Extensions;
using HoNAvatarManager.Core.Helpers;
using Logger = HoNAvatarManager.Core.Logging.Logger;

namespace HoNAvatarManager.Core.Parsers
{
    internal abstract class EntityParser : IEntityParser
    {
        protected readonly XmlManager _xmlManager;

        protected readonly string[] SkippedAttributes = new string[]
        {
            "announcersound",
            "modpriority",
            "altavatar"
        };

        public EntityParser(XmlManager xmlManager)
        {
            _xmlManager = xmlManager;
        }

        public static IEnumerable<IEntityParser> GetRegisteredEntityParsers(XmlManager xmlManager)
        {
            var entityParserType = typeof(IEntityParser);

            var entityParsers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => entityParserType.IsAssignableFrom(p) && !p.IsAbstract)
                .Select(t => Activator.CreateInstance(t, xmlManager))
                .OfType<IEntityParser>()
                .Where(e => IsEntityParserEnabled(e))
                .OrderBy(e => GetEntityParserPriority(e))
                .ToList();

            return entityParsers;
        }

        public abstract void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey);

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

            var modelDirectory = GetModelDirectory(entityXml, avatarKey);
            var avatarDirectoryPath = Path.Combine(heroDirectoryPath, modelDirectory);

            if (Directory.Exists(avatarDirectoryPath))
            {
                return avatarDirectoryPath;
            }

            Logger.Log.Warning("Directory not found for avatar {0}. Using default avatar directory.", avatarKey);

            return heroDirectoryPath;
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
    
        protected void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);

            // Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                var directoryToCreate = dirPath.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(directoryToCreate);
            }

            // Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                var fileToCopy = newPath.Replace(sourcePath, targetPath);
                File.Copy(newPath, fileToCopy, true);
            }
        }

        private string GetModelDirectory(IXmlDocument entityXml, string avatarKey)
        {
            var entityElement = entityXml.QuerySelector("hero");
            var entityAvatarElements = entityElement.QuerySelectorAll("altavatar");
            var targetElement = entityAvatarElements.FirstOrDefault(a => a.HasKey(avatarKey));

            if (targetElement == null)
            {
                targetElement = entityElement;
            }

            var modelDirectory = targetElement.GetAttribute("model").Split("/").First();

            return modelDirectory;
        }

        private static int GetEntityParserPriority(IEntityParser entityParser)
        {
            var priorityAttribute = GetEntityParserAttribute<EntityParserPriorityAttribute>(entityParser);

            if (priorityAttribute == null)
            {
                return -1;
            }

            return priorityAttribute.Priority;
        }

        private static bool IsEntityParserEnabled(IEntityParser entityParser)
        {
            var disabledAttribute = GetEntityParserAttribute<DisabledAttribute>(entityParser);

            return disabledAttribute == null;
        }

        private static T GetEntityParserAttribute<T>(IEntityParser entityParser) where T : Attribute
        {
            var attribute = entityParser.GetType()
                .GetCustomAttributes(typeof(T), true)
                .FirstOrDefault() as T;

            return attribute;
        }
    }
}
