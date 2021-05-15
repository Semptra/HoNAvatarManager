using System.Collections.Generic;
using System.IO;
using System.Linq;
using AngleSharp.Dom;
using HoNAvatarManager.Core.Attributes;
using HoNAvatarManager.Core.Extensions;
using Logger = HoNAvatarManager.Core.Logging.Logger;

namespace HoNAvatarManager.Core.Parsers.Model
{
    [EntityParserPriority(2)]
    internal class ModelEntityParser : EntityParser
    {
        public ModelEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            var targetModelFilePath = Path.Combine(resultDirectoryPath, "model.mdf");

            var avatarDirectory = GetAvatarDirectory(extractedDirectoryPath, avatarKey);
            var avatarModelFilePath = Path.Combine(avatarDirectory, "model.mdf");

            SetAvatarModel(targetModelFilePath, avatarModelFilePath);
        }

        protected void SetAvatarModel(string targetModelFilePath, string avatarModelFilePath)
        {
            var heroModelXml = _xmlManager.GetXmlDocument(targetModelFilePath);

            if (!File.Exists(avatarModelFilePath))
            {
                Logger.Log.Warning("Model file not found for avatar.");
                return;
            }

            var avatarModelXml = _xmlManager.GetXmlDocument(avatarModelFilePath);

            var heroModel = heroModelXml.QuerySelector("model");
            var avatarModel = avatarModelXml.QuerySelector("model");

            var heroModelAnimations = heroModel.QuerySelectorAll("anim").ToList();
            var avatarModelAnimations = avatarModel.QuerySelectorAll("anim").ToList();

            var resultAnimations = new List<IElement>();

            for (int i = 0; i < heroModelAnimations.Count; i++)
            {
                var heroModelAnimation = heroModelAnimations[i];
                var heroModelAnimationName = heroModelAnimation.GetAttribute("name");

                var avatarModelAnimation = avatarModelAnimations.FirstOrDefault(animation => animation.GetAttribute("name") == heroModelAnimationName);

                if (avatarModelAnimation != null)
                {
                    Logger.Log.Information("  Using avatar animation for [{0}].", heroModelAnimationName);
                    resultAnimations.Add(avatarModelAnimation);
                }
                else
                {
                    IElement avatarAnimation;

                    if (heroModelAnimationName.StartsWith("knock")    || 
                        heroModelAnimationName.StartsWith("getup")    || 
                        heroModelAnimationName.StartsWith("bored")    || 
                        heroModelAnimationName.StartsWith("portrait"))
                    {
                        avatarAnimation = avatarModelAnimations.First(animation => animation.GetAttribute("name") == "idle");
                        Logger.Log.Information("- [{0}/{1}] Replaced avatar animation [{2}] with [{3}].", i + 1, heroModelAnimations.Count, heroModelAnimationName, avatarAnimation.GetAttribute("name"));
                    }
                    else if (heroModelAnimationName.StartsWith("taunt") ||
                             heroModelAnimationName.StartsWith("attack"))
                    {
                        avatarAnimation = avatarModelAnimations.First(animation => animation.GetAttribute("name").StartsWith("attack"));
                        Logger.Log.Information("- [{0}/{1}] Replaced avatar animation [{2}] with [{3}].", i + 1, heroModelAnimations.Count, heroModelAnimationName, avatarAnimation.GetAttribute("name"));
                    }
                    else
                    {
                        Logger.Log.Information("- [{0}/{1}] Using original hero animation for [{2}].", i + 1, heroModelAnimations.Count, heroModelAnimationName);
                        avatarAnimation = heroModelAnimation;
                    }

                    var clonedAvatarAnimation = (IElement)avatarAnimation.Clone();
                    clonedAvatarAnimation.SetAttribute("name", heroModelAnimationName);
                    resultAnimations.Add(clonedAvatarAnimation);
                }
            }

            // Remove old animations
            foreach (var avatarModelAnimation in avatarModelAnimations)
            {
                avatarModel.RemoveChild(avatarModelAnimation);
            }

            var results = resultAnimations.Select(x => x.GetAttribute("name")).ToList();

            // Add new animations
            foreach (var resultAnimation in resultAnimations)
            {
                avatarModel.AppendChild(resultAnimation);
            }

            avatarModelXml.SaveXml(avatarModelFilePath);
        }
    }
}
