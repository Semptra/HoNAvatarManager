using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Polly;
using HoNAvatarManager.Core.Helpers;
using HoNAvatarManager.Core.Parsers;
using Logger = HoNAvatarManager.Core.Logging.Logger;

namespace HoNAvatarManager.Core
{
    public class AvatarManager
    {
        private const int TOTAL_RETRY_COUNT = 3;

        private readonly ConfigurationManager _configurationManager;
        private readonly AppConfiguration _appConfiguration;
        private readonly ResourcesManager _resourcesManager;
        private readonly XmlManager _xmlManager;

        private readonly Policy _retryFileCopyPolicy = Policy.Handle<IOException>()
            .Retry(TOTAL_RETRY_COUNT, onRetry: (exception, retryCount) =>
            {
                Logger.Log.Error("[Retry {0}/{1}] Cannot copy hero resource to HoN directory. Please close HoN before using the Avatar Manager.", retryCount, TOTAL_RETRY_COUNT);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            });

        public AvatarManager()
        {
            
#if Windows
            if (!PlatformSpecific.Windows.Utilities.IsAdministrator())
            {
                throw new UnauthorizedAccessException("Please run HoN Avatar Manager as Administrator.");
            }
#endif

            _configurationManager = new ConfigurationManager("appsettings.json");
            _appConfiguration = _configurationManager.GetAppConfiguration();

            if (!_appConfiguration.GetHoNPath().Any())
            {
                throw ThrowHelper.DirectoryNotFoundException($"HoN directory not found at {_appConfiguration.HoNPath32} or {_appConfiguration.HoNPath64}.");
            }

            _resourcesManager = new ResourcesManager(_appConfiguration);
            _xmlManager = new XmlManager();
        }

        public void SetHeroAvatar(string hero, string avatar)
        {
            var extractionDirectory = CreateTempDirectory();
            var resultDirectory = CreateTempDirectory();

            try
            {
                var heroResourcesName = GetHeroResourcesName(hero);

                _resourcesManager.ExtractHeroResources(extractionDirectory, heroResourcesName);

                var extractedHeroResourcesDirectory = Path.Combine(extractionDirectory, "heroes");
                var extractedHeroDirectoryPath = Path.Combine(extractedHeroResourcesDirectory, heroResourcesName);

                var resultHeroResourcesDirectory = Path.Combine(resultDirectory, "heroes");
                var resultHeroDirectoryPath = Path.Combine(resultHeroResourcesDirectory, heroResourcesName);

                var avatarKey = GetHeroAvatarKey(hero, avatar);

                if (!IsAvatarExists(avatarKey, extractedHeroDirectoryPath, heroResourcesName))
                {
                    throw ThrowHelper.AvatarNotFoundException($"Avatar {avatar} not found for hero {hero}.", avatar);
                }

                var entityParsers = EntityParser.GetRegisteredEntityParsers(_xmlManager).ToList();

                Logger.Log.Information("Found {0} entity parsers.", entityParsers.Count);
                
                for (int i = 0; i< entityParsers.Count; i++)
                {
                    var entityParser = entityParsers[i];

                    Logger.Log.Information("[{0}/{1}] Executing [{2}].", i + 1, entityParsers.Count, entityParser.GetType().Name);

                    entityParser.SetEntity(extractedHeroDirectoryPath, resultHeroDirectoryPath, avatarKey);
                }

                var heroResourcesS2ZFileName = $"resources_{hero.Replace(" ", string.Empty)}.s2z";
                var heroResourcesS2ZFilePath = Path.Combine(resultDirectory, heroResourcesS2ZFileName);

                _resourcesManager.PackHeroResources(resultHeroResourcesDirectory, heroResourcesS2ZFilePath);

                foreach (var honPath in _appConfiguration.GetHoNPath())
                {
                    var targetPath = Path.Combine(honPath, "game", heroResourcesS2ZFileName);

                    _retryFileCopyPolicy.Execute(() => File.Copy(heroResourcesS2ZFilePath, targetPath, true));
                }
            }
            finally
            {
                Directory.Delete(extractionDirectory, true);
                Directory.Delete(resultDirectory, true);
            }
        }

        public void SetHeroAvatarUnpacked(string hero, string avatar)
        {
            // var extractionDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            // Directory.CreateDirectory(extractionDirectory);
            // 
            // try
            // {
            //     var heroResourcesName = GetHeroResourcesName(hero);
            //     var rootResourcesDirectory = _resourcesManager.ExtractHeroResources(extractionDirectory, heroResourcesName);
            //     var heroResourcesDirectory = Path.Combine(rootResourcesDirectory, "heroes");
            //     var heroDirectoryPath = Path.Combine(heroResourcesDirectory, heroResourcesName);
            //     var heroEntityPath = GetHeroEntityPath(heroDirectoryPath, heroResourcesName);
            // 
            //     var heroXml = _xmlManager.GetXmlDocument(heroEntityPath);
            // 
            //     var heroNode = heroXml.QuerySelector("hero");
            //     var avatarElements = heroNode.QuerySelectorAll("altavatar");
            //     var avatarKey = GetHeroAvatarKey(hero, avatar);
            //     var avatarElement = avatarElements.FirstOrDefault(a => string.Equals(a.GetAttribute("key"), avatarKey, StringComparison.InvariantCultureIgnoreCase));
            // 
            //     if (avatarElement == null)
            //     {
            //         throw ThrowHelper.AvatarNotFoundException($"Avatar {avatar} not found for hero {hero}.", avatar);
            //     }
            // 
            //     foreach (var parser in EntityParser.GetRegisteredEntityParsers(_xmlManager))
            //     {
            //         parser.SetEntity(heroDirectoryPath, avatarKey);
            //     }
            // 
            //     foreach (string newPath in Directory.GetFiles(heroResourcesDirectory, "*.*", SearchOption.AllDirectories))
            //     {
            //         var destinationHeroDirectory = Path.Combine(newPath, "game", "heroes");
            // 
            //         if (Directory.Exists(destinationHeroDirectory))
            //         {
            //             Directory.Delete(destinationHeroDirectory);
            //         }
            // 
            //         Directory.CreateDirectory(destinationHeroDirectory);
            // 
            //         foreach (string dirPath in Directory.GetDirectories(heroResourcesDirectory, "*", SearchOption.AllDirectories))
            //         {
            //             Directory.CreateDirectory(dirPath.Replace(heroResourcesDirectory, destinationHeroDirectory));
            //         }
            //     
            //         File.Copy(newPath, newPath.Replace(heroResourcesDirectory, destinationHeroDirectory), true);
            //     }
            // }
            // finally
            // {
            //     Directory.Delete(extractionDirectory, true);
            // }
        }

        public void RemoveHeroAvatar(string hero)
        {
            foreach (var honPath in _appConfiguration.GetHoNPath())
            {
                var heroResourcesPath = Path.Combine(honPath, "game", $"resources_{hero.Replace(" ", string.Empty)}.s2z");

                if (File.Exists(heroResourcesPath))
                {
                    File.Delete(heroResourcesPath);
                }
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

        private string CreateTempDirectory()
        {
            var tempDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(tempDirectoryPath);

            return tempDirectoryPath;
        }

        private bool IsAvatarExists(string avatarKey, string heroDirectoryPath, string heroResourcesName)
        {
            var heroEntityPath = GetHeroEntityPath(heroDirectoryPath, heroResourcesName);

            var heroXml = _xmlManager.GetXmlDocument(heroEntityPath);

            var heroNode = heroXml.QuerySelector("hero");
            var avatarElements = heroNode.QuerySelectorAll("altavatar");

            var avatarElement = avatarElements.FirstOrDefault(a => string.Equals(a.GetAttribute("key"), avatarKey, StringComparison.InvariantCultureIgnoreCase));

            return avatarElement != null;
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

            throw ThrowHelper.FileNotFoundException("Cannot find hero.entity file.", heroEntityPath);
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
                throw ThrowHelper.KeyNotFoundException($"Hero {hero} not found.");
            }

            return GlobalResources.HeroResourcesMapping[key];
        }
    }
}
