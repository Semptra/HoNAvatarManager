using System.IO;
using HoNAvatarManager.Core.Parsers.Hero;

namespace HoNAvatarManager.Core.Parsers.Special
{
    internal class SolsticeParser : ModelEntityParser
    {
        public SolsticeParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            var heroDirectoryInfo = new DirectoryInfo(heroDirectoryPath);
            
            if (heroDirectoryInfo.Name != "solstice")
            {
                return;
            }

            var heroModelFilePath = Path.Combine(heroDirectoryPath, "night_form", $"model.mdf");

            var avatarDirectory = GetAvatarDirectory(heroDirectoryPath, avatarKey);
            var avatarModelFilePath = Path.Combine(avatarDirectory, "night_form", $"model.mdf");

            SetAvatarModel(heroModelFilePath, avatarModelFilePath);
        }
    }
}
