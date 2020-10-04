using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AngleSharp.Dom;
using AngleSharp.Xml;
using AngleSharp.Xml.Dom;
using AngleSharp.Xml.Parser;

namespace HoNAvatarManager.Core
{
    public class AvatarManager
    {
        private readonly ConfigurationManager _configurationManager;
        private readonly AppConfiguration _appConfiguration;

        private readonly ResourcesManager _resourcesManager;
        private readonly HeroNameManager _heroNameMapper;

        public AvatarManager()
        {
            _configurationManager = new ConfigurationManager("appsettings.json");
            _appConfiguration = _configurationManager.GetAppConfiguration();

            if (!Directory.Exists(_appConfiguration.HoNPath))
            {
                throw new DirectoryNotFoundException($"HoN directory not found at {_appConfiguration.HoNPath}.");
            }

            _resourcesManager = new ResourcesManager(_appConfiguration);
            _heroNameMapper = new HeroNameManager();
        }

        public void SetHeroAvatar(string hero, string avatar)
        {
            var extractionDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(extractionDirectory);

            try
            {
                var heroResourcesName = _heroNameMapper.GetHeroResourcesName(hero);
                var extractedHeroResourcesDirectory = _resourcesManager.ExtractHeroResources(extractionDirectory, heroResourcesName);
                var extractedHeroEntityPath = Path.Combine(extractedHeroResourcesDirectory, "heroes", heroResourcesName, "hero.entity");

                var heroXml = GetHeroXml(extractedHeroEntityPath);

                var heroNode = heroXml.QuerySelector("hero");

                var avatarNodes = heroNode.QuerySelectorAll("altavatar");
                var avatarNode = avatarNodes.FirstOrDefault(a => string.Equals(a.GetAttribute("key"), avatar, StringComparison.InvariantCultureIgnoreCase));

                if (avatarNode == null)
                {
                    throw new Exception($"Avatar {avatar} not found for hero {hero}.");
                }

                SetAvatarAttributes(heroNode, avatarNode);
                SetAvatarModifiers(heroNode, avatarNode);
                SetAvatarEvents(heroNode, avatarNode);

                SaveHeroXml(extractedHeroEntityPath, heroXml.ToXml());

                var heroResourcesS2ZFileName = $"resources_{heroResourcesName}_{avatar}.s2z";
                var heroResourcesS2ZFilePath = Path.Combine(extractedHeroResourcesDirectory, heroResourcesS2ZFileName);
                var heroResourcesDirectory = Path.Combine(extractedHeroResourcesDirectory, "heroes");

                _resourcesManager.PackHeroResources(heroResourcesDirectory, heroResourcesS2ZFilePath);

                File.Copy(heroResourcesS2ZFilePath, Path.Combine(_appConfiguration.HoNPath, "game", heroResourcesS2ZFileName), true);
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

        public IEnumerable<string> GetHeroAvatars(string hero)
        {
            var heroResourcesName = _heroNameMapper.GetHeroResourcesName(hero);

            using (var heroResourcesZip = _resourcesManager.GetHeroResourcesZip(heroResourcesName))
            {
                var heroEntryPrefix = $"heroes/{heroResourcesName}";
                var heroZipXml = heroResourcesZip.Entries.FirstOrDefault(e => e.FullName.StartsWith(heroEntryPrefix) && e.Name == "hero.entity");

                var heroEntityPath = Path.GetTempFileName();
                heroZipXml.ExtractToFile(heroEntityPath, true);

                var heroXml = GetHeroXml(heroEntityPath);
                var heroNode = heroXml.QuerySelector("hero");
                var avatarNodes = heroNode.QuerySelectorAll("altavatar");

                return avatarNodes.Select(n => n.Attributes["key"].Value);
            }
        }

        private void SaveHeroXml(string path, string heroXml)
        {
            var element = XElement.Parse(heroXml);

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                Indent = true,
                NewLineOnAttributes = true
            };

            using (var xmlWriter = XmlWriter.Create(path, settings))
            {
                element.Save(xmlWriter);
            }
        }

        private IXmlDocument GetHeroXml(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var parser = new XmlParser();
                return parser.ParseDocument(stream);
            }
        }

        private void SetAvatarAttributes(IElement heroNode, IElement avatarNode)
        {
            var avatarAttributes = avatarNode.Attributes.Where(a => a.Name != "key");

            foreach (var avatarAttribute in avatarAttributes)
            {
                heroNode.SetAttribute(avatarAttribute.Name, avatarAttribute.Value);
            }
        }

        private void SetAvatarModifiers(IElement heroNode, IElement avatarNode)
        {
            var avatarModifiers = avatarNode.ChildNodes.Where(n => n.NodeName == "modifier").OfType<IElement>().ToList();
            var heroModifiers = heroNode.ChildNodes.Where(n => n.NodeName == "modifier").OfType<IElement>().ToList();

            foreach (var avatarModifier in avatarModifiers)
            {
                var avatarModifierKey = avatarModifier.Attributes["key"].Value;
                var heroModifier = heroModifiers.FirstOrDefault(n => n.Attributes["key"].Value == avatarModifierKey);

                if (heroModifier != null)
                {
                    heroNode.RemoveChild(heroModifier);
                }

                heroNode.AppendChild(avatarModifier);
            }
        }

        private void SetAvatarEvents(IElement heroNode, IElement avatarNode)
        {
            foreach (var heroEvent in HeroEvents.GetAllEvents())
            {
                SetAvatarNode(heroNode, avatarNode, heroEvent);
            }
        }

        private void SetAvatarNode(IElement heroNode, IElement avatarNode, string node)
        {
            var avatarOnSpawnNode = avatarNode.ChildNodes.FirstOrDefault(n => n.NodeName == node);

            if (avatarOnSpawnNode == null)
            {
                return;
            }

            var heroOnSpawnNode = heroNode.ChildNodes.FirstOrDefault(n => n.NodeName == node);

            if (heroOnSpawnNode != null)
            {
                heroNode.RemoveChild(heroOnSpawnNode);
            }

            heroNode.AppendChild(avatarOnSpawnNode);
        }
    }
}
