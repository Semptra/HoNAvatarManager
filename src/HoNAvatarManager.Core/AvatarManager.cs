using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Xml;

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
                var heroEntityPath = Path.Combine(heroDirectoryPath, "hero.entity");

                var heroXml = _xmlManager.GetXmlDocument(heroEntityPath);

                var heroNode = heroXml.QuerySelector("hero");
                var avatarElements = heroNode.QuerySelectorAll("altavatar");
                var avatarResourceName = GetHeroAvatarResourceName(hero, avatar);
                var avatarElement = avatarElements.FirstOrDefault(a => string.Equals(a.GetAttribute("key"), avatarResourceName, StringComparison.InvariantCultureIgnoreCase));

                if (avatarElement == null)
                {
                    throw new Exception($"Avatar {avatar} not found for hero {hero}.");
                }

                var avatarDirectoryPath = GetAvatarDirectory(rootResourcesDirectory, heroResourcesName, avatarElement);

                SetAvatarHeroAttributes(heroNode, avatarElement);
                SetAvatarHeroModifiers(heroNode, avatarElement);
                SetAvatarHeroEvents(heroNode, avatarElement);

                SetAvatarAbilityFiles(heroDirectoryPath, avatarDirectoryPath);
                SetAvatarAbilityXmls(heroDirectoryPath, avatarResourceName);

                _xmlManager.SaveXml(heroEntityPath, heroXml.ToXml());

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
                var heroZipXml = heroResourcesZip.Entries.FirstOrDefault(e => e.FullName.StartsWith(heroEntryPrefix) && e.Name == "hero.entity");

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

        private string GetHeroAvatarResourceName(string hero, string avatarFriendlyName)
        {
            return GlobalResources.HeroAvatarMapping.SingleOrDefault(x => string.Equals(x.Hero, hero, StringComparison.InvariantCultureIgnoreCase))?
                .AvatarInfo.SingleOrDefault(x => string.Equals(x.AvatarName, avatarFriendlyName, StringComparison.InvariantCultureIgnoreCase))?
                .ResourceName ?? avatarFriendlyName;
        }

        private void SetAvatarHeroAttributes(IElement rootElement, IElement avatarElement)
        {
            var avatarAttributes = avatarElement.Attributes.Where(a => a.Name != "key");

            foreach (var avatarAttribute in avatarAttributes)
            {
                rootElement.SetAttribute(avatarAttribute.Name, avatarAttribute.Value);
            }
        }

        private void SetAvatarHeroModifiers(IElement heroElement, IElement avatarElement)
        {
            var avatarModifiers = avatarElement.ChildNodes.Where(n => n.NodeName == "modifier").OfType<IElement>().ToList();
            var heroModifiers = heroElement.ChildNodes.Where(n => n.NodeName == "modifier").OfType<IElement>().ToList();

            foreach (var avatarModifier in avatarModifiers)
            {
                var avatarModifierKey = avatarModifier.Attributes["key"].Value;
                var heroModifier = heroModifiers.FirstOrDefault(n => n.Attributes["key"].Value == avatarModifierKey);

                if (heroModifier != null)
                {
                    heroElement.RemoveChild(heroModifier);
                }

                heroElement.AppendChild(avatarModifier);
            }
        }

        private void SetAvatarHeroEvents(IElement heroElement, IElement avatarElement)
        {
            var avatarEventNodes = avatarElement.ChildNodes.Where(n => n.NodeName.StartsWith("on", StringComparison.InvariantCultureIgnoreCase)).ToList();

            foreach (var avatarEventNode in avatarEventNodes)
            {
                _xmlManager.CopyNodeToRoot(heroElement, avatarEventNode);
            }
        }

        private void SetAvatarAbilityFiles(string heroDirectoryPath, string avatarDirectoryPath)
        {
            var heroAbilityDirectories = Directory.EnumerateDirectories(heroDirectoryPath, "ability_*").Select(d => new DirectoryInfo(d));
            var avatarAbilityDirectories = Directory.EnumerateDirectories(avatarDirectoryPath, "ability_*").Select(d => new DirectoryInfo(d));

            foreach (var avatarAbilityDirectory in avatarAbilityDirectories)
            {
                var heroAbilityDirectory = heroAbilityDirectories.Single(d => d.Name == avatarAbilityDirectory.Name);
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

        private void SetAvatarAbilityXmls(string heroDirectoryPath, string avatarResourceName)
        {
            var heroAbilityDirectories = Directory.EnumerateDirectories(heroDirectoryPath, "ability_*");

            foreach (var heroAbilityDirectory in heroAbilityDirectories)
            {
                var heroAbilityEntityFile = Path.Combine(heroAbilityDirectory, "ability.entity");

                var abilityXml = _xmlManager.GetXmlDocument(heroAbilityEntityFile);

                var abilityElement = abilityXml.QuerySelector("ability");
                var abilityAvatarElements = abilityElement.QuerySelectorAll("altavatar");
                var abilityAvatarElement = abilityAvatarElements.FirstOrDefault(a => string.Equals(GetAvatarAbilityKey(a), avatarResourceName, StringComparison.InvariantCultureIgnoreCase));

                if (abilityAvatarElement == null)
                {
                    continue;
                }

                SetAvatarHeroAttributes(abilityElement, abilityAvatarElement);

                foreach (var abilityAvatarElementChildNode in abilityAvatarElement.ChildNodes)
                {
                    _xmlManager.CopyNodeToRoot(abilityElement, abilityAvatarElementChildNode);
                }

                _xmlManager.SaveXml(heroAbilityEntityFile, abilityXml.ToXml());
            }
        }

        private string GetAvatarAbilityKey(IElement avatarAbilityElement)
        {
            var key = avatarAbilityElement.GetAttribute("key");
            var keyRegex = new Regex(@"Hero_[a-zA-Z]+\.(?<key>.+)");
            var match = keyRegex.Match(key);

            return match.Success ? match.Groups["key"].Value : key;
        }

        private string GetAvatarDirectory(string extractedHeroResourcesDirectory, string heroResourcesName, IElement avatarElement)
        {
            var avatarDirectoryName = avatarElement.Attributes["model"].Value.Split('/')[0];
            var avatarDirectoryPath = Path.Combine(extractedHeroResourcesDirectory, "heroes", heroResourcesName, avatarDirectoryName);

            return avatarDirectoryPath;
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
