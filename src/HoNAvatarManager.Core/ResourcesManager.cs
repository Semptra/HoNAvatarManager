using HoNAvatarManager.Core.Helpers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace HoNAvatarManager.Core
{
    internal class ResourcesManager
    {
        private readonly AppConfiguration _appConfiguration;

        public ResourcesManager(AppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        public string ExtractHeroResources(string extractionDirectory, string hero)
        {
            using (var heroResourcesZip = GetHeroResourcesZip(hero))
            {
                var heroEntryPrefix = $"heroes/{hero}";
                var heroEntries = heroResourcesZip.Entries.Where(e => e.FullName.StartsWith(heroEntryPrefix));

                foreach (var heroEntry in heroEntries)
                {
                    var heroEntryFilePath = Path.Combine(extractionDirectory, heroEntry.FullName);
                    var heroEntryFilePathInfo = new FileInfo(heroEntryFilePath);

                    Directory.CreateDirectory(heroEntryFilePathInfo.Directory.FullName);

                    heroEntry.ExtractToFile(heroEntryFilePath);
                }
            }

            return extractionDirectory;
        }

        public void PackHeroResources(string heroResourcesPath, string outputPath)
        {
            ZipFile.CreateFromDirectory(heroResourcesPath, outputPath, CompressionLevel.NoCompression, true);
        }

        public ZipArchive GetHeroResourcesZip(string hero)
        {
            var heroResourcesIndex = GetHeroResourcesIndex(hero);
            var heroResourcesPath = GetResourcesPath(heroResourcesIndex);

            return ZipFile.Open(heroResourcesPath, ZipArchiveMode.Read);
        }

        private int GetHeroResourcesIndex(string hero)
        {
            var resourceIndex = -1;

            for (int i = 0; i <= 3; i++)
            {
                var heroes = GetResourcesHeroes(i);

                var heroResources = heroes.FirstOrDefault(h => h == hero);

                if (!string.IsNullOrEmpty(heroResources))
                {
                    resourceIndex = i;
                    break;
                }
            }

            if (resourceIndex < 0)
            {
                throw ThrowHelper.FileNotFoundException($"Resources file for hero {hero} not found.");
            }

            return resourceIndex;
        }

        private IEnumerable<string> GetResourcesHeroes(int resourcesIndex)
        {
            var resourcesPath = GetResourcesPath(resourcesIndex);

            if (!File.Exists(resourcesPath))
            {
                throw ThrowHelper.FileNotFoundException("Resources file not found.", resourcesPath);
            }

            using (var resourcesZip = ZipFile.Open(resourcesPath, ZipArchiveMode.Read))
            {
                return resourcesZip.Entries
                    .Where(e => e.FullName.StartsWith("heroes/"))
                    .Select(h => h.FullName
                        .Split("/").ElementAtOrDefault(1))
                        .Where(h => !string.IsNullOrEmpty(h))
                        .Where(h => h.All(c => char.IsLetter(c) || c == '_'))
                    .Distinct()
                    .OrderBy(h => h);
            }
        }

        private string GetResourcesPath(int resourcesIndex)
        {
            return Path.Combine(_appConfiguration.GetHoNPath(), "game", $"resources{resourcesIndex}.s2z");
        }
    }
}
