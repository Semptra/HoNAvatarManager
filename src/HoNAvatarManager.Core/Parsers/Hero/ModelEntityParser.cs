using System.Collections.Generic;
using System.IO;
using System.Linq;
using AngleSharp.Dom;
using HoNAvatarManager.Core.Extensions;
using HoNAvatarManager.Core.Helpers;

namespace HoNAvatarManager.Core.Parsers.Hero
{
    internal class ModelEntityParser : EntityParser
    {
        public ModelEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            var heroModelFilePath = Path.Combine(heroDirectoryPath, $"model.mdf");

            var avatarDirectory = GetAvatarDirectory(heroDirectoryPath, avatarKey);
            var avatarModelFilePath = Path.Combine(avatarDirectory, $"model.mdf");

            SetAvatarModel(heroModelFilePath, avatarModelFilePath);
        }

        protected void SetAvatarModel(string heroModelFilePath, string avatarModelFilePath)
        {
            var heroModelXml = _xmlManager.GetXmlDocument(heroModelFilePath);
            var avatarModelXml = _xmlManager.GetXmlDocument(avatarModelFilePath);

            var heroModel = heroModelXml.QuerySelector("model");
            var avatarModel = avatarModelXml.QuerySelector("model");

            var heroModelAnimations = heroModel.QuerySelectorAll("anim").ToList();
            var avatarModelAnimations = avatarModel.QuerySelectorAll("anim").ToList();

            var resultAnimations = new List<IElement>();

            foreach (var heroModelAnimation in heroModelAnimations)
            {
                var heroModelAnimationName = heroModelAnimation.GetAttribute("name");

                var avatarModelAnimation = avatarModelAnimations.FirstOrDefault(animation => animation.GetAttribute("name") == heroModelAnimationName);

                if (avatarModelAnimation != null)
                {
                    resultAnimations.Add(avatarModelAnimation);
                }
                else
                {
                    IElement avatarAnimation;

                    if (heroModelAnimationName.StartsWith("knock") || 
                        heroModelAnimationName.StartsWith("getup") || 
                        heroModelAnimationName.StartsWith("bored") || 
                        heroModelAnimationName.StartsWith("portrait"))
                    {
                        avatarAnimation = avatarModelAnimations.First(animation => animation.GetAttribute("name") == "idle");
                    }
                    else if (heroModelAnimationName.StartsWith("taunt"))
                    {
                        avatarAnimation = avatarModelAnimations.First(animation => animation.GetAttribute("name").StartsWith("attack"));
                    }
                    else
                    {
                        System.Console.WriteLine($"Cannot find a replacement for animation {heroModelAnimationName}.");
                        continue;
                        //throw ThrowHelper.AnimationNotFoundException($"Cannot find a replacement for animation {heroModelAnimationName}.", heroModelAnimationName);
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
