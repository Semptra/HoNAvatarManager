﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AngleSharp.Dom;
using HoNAvatarManager.Core.Parsers;

namespace HoNAvatarManager.Core
{
    public class AvatarManager
    {
        private readonly ConfigurationManager _configurationManager;
        private readonly AppConfiguration _appConfiguration;
        private readonly ResourcesManager _resourcesManager;
        private readonly XmlManager _xmlManager;

        public AvatarManager()
        {
            _configurationManager = new ConfigurationManager("appsettings.json");
            _appConfiguration = _configurationManager.GetAppConfiguration();

            if (!Directory.Exists(_appConfiguration.HoNPath))
            {
                throw new DirectoryNotFoundException($"HoN directory not found at {_appConfiguration.HoNPath}.");
            }

            _resourcesManager = new ResourcesManager(_appConfiguration);
            _xmlManager = new XmlManager();
        }

        public void SetHeroAvatar(string hero, string avatar)
        {
            var extractionDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(extractionDirectory);

            try
            {
                var heroResourcesName = GetHeroResourcesName(hero);
                var rootResourcesDirectory = _resourcesManager.ExtractHeroResources(extractionDirectory, heroResourcesName);
                var heroResourcesDirectory = Path.Combine(rootResourcesDirectory, "heroes");
                var heroDirectoryPath = Path.Combine(heroResourcesDirectory, heroResourcesName);
                var heroEntityPath = GetHeroEntityPath(heroDirectoryPath, heroResourcesName);

                var heroXml = _xmlManager.GetXmlDocument(heroEntityPath);

                var heroNode = heroXml.QuerySelector("hero");
                var avatarElements = heroNode.QuerySelectorAll("altavatar");
                var avatarKey = GetHeroAvatarKey(hero, avatar);
                var avatarElement = avatarElements.FirstOrDefault(a => string.Equals(a.GetAttribute("key"), avatarKey, StringComparison.InvariantCultureIgnoreCase));

                if (avatarElement == null)
                {
                    throw new Exception($"Avatar {avatar} not found for hero {hero}.");
                }

                foreach(var parser in EntityParser.GetRegisteredEntityParsers(_xmlManager))
                {
                    parser.SetEntity(heroDirectoryPath, avatarKey);
                }

                var heroResourcesS2ZFileName = $"resources_{hero.Replace(" ", string.Empty)}.s2z";
                var heroResourcesS2ZFilePath = Path.Combine(rootResourcesDirectory, heroResourcesS2ZFileName);

                _resourcesManager.PackHeroResources(heroResourcesDirectory, heroResourcesS2ZFilePath);

                File.Copy(heroResourcesS2ZFilePath, Path.Combine(_appConfiguration.HoNPath, "game", heroResourcesS2ZFileName), true);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException("Please run PowerShell Core as Administrator.", ex);
            }
            catch
            {
                throw;
            }
            finally
            {
                Directory.Delete(extractionDirectory, true);
            }
        }

        public void RemoveHeroAvatar(string hero)
        {
            var heroResourcesPath = Path.Combine(_appConfiguration.HoNPath, "game", $"resources_{hero.Replace(" ", string.Empty)}.s2z");

            if (File.Exists(heroResourcesPath))
            {
                File.Delete(heroResourcesPath);
            }
        }

        public IEnumerable<string> GetHeroAvatars(string hero)
        {
            var heroResourcesName = GetHeroResourcesName(hero);

            using (var heroResourcesZip = _resourcesManager.GetHeroResourcesZip(heroResourcesName))
            {
                var heroEntryPrefix = $"heroes/{heroResourcesName}/";
                var heroZipXml = heroResourcesZip.Entries.FirstOrDefault(e => e.FullName.StartsWith(heroEntryPrefix) && (e.Name == "hero.entity" || e.Name == $"{heroResourcesName}.entity"));

                var heroEntityPath = Path.GetTempFileName();
                heroZipXml.ExtractToFile(heroEntityPath, true);

                var heroXml = _xmlManager.GetXmlDocument(heroEntityPath);
                var heroNode = heroXml.QuerySelector("hero");
                var avatarNodes = heroNode.QuerySelectorAll("altavatar");

                return avatarNodes.Select(n => n.Attributes["key"].Value);
            }
        }

        public string GetHeroAvatarFriendlyName(string hero, string avatarResourceName)
        {
            return GlobalResources.HeroAvatarMapping.SingleOrDefault(x => string.Equals(x.Hero, hero, StringComparison.InvariantCultureIgnoreCase))?
                .AvatarInfo.SingleOrDefault(x => string.Equals(x.ResourceName, avatarResourceName, StringComparison.InvariantCultureIgnoreCase))?
                .AvatarName ?? avatarResourceName;
        }

        private string GetHeroEntityPath(string heroDirectoryPath, string heroResourcesName)
        {
            var heroEntityPath = Path.Combine(heroDirectoryPath, "hero.entity");
            if (File.Exists(heroEntityPath))
            {
                return heroEntityPath;
            }

            heroEntityPath = Path.Combine(heroDirectoryPath, $"{heroResourcesName}.entity");
            if (File.Exists(heroEntityPath))
            {
                return heroEntityPath;
            }

            throw new FileNotFoundException("Cannot find hero.entity file.", heroEntityPath);
        }

        private string GetHeroAvatarKey(string hero, string avatarFriendlyName)
        {
            return GlobalResources.HeroAvatarMapping.SingleOrDefault(x => string.Equals(x.Hero, hero, StringComparison.InvariantCultureIgnoreCase))?
                .AvatarInfo.SingleOrDefault(x => string.Equals(x.AvatarName, avatarFriendlyName, StringComparison.InvariantCultureIgnoreCase))?
                .ResourceName ?? avatarFriendlyName;
        }

        private string GetHeroResourcesName(string hero)
        {
            var key = GlobalResources.HeroResourcesMapping.Keys.FirstOrDefault(k => string.Equals(k, hero, StringComparison.InvariantCultureIgnoreCase));

            if (key == null)
            {
                throw new KeyNotFoundException($"Hero {hero} not found.");
            }

            return GlobalResources.HeroResourcesMapping[key];
        }
    }
}
