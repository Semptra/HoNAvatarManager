using System.IO;
using HoNAvatarManager.Core.Attributes;
using HoNAvatarManager.Core.Parsers.Model;

namespace HoNAvatarManager.Core.Parsers.Special
{
    [Disabled]
    [EntityParserPriority(1)]
    internal class SolsticeParser : ModelEntityParser
    {
        public SolsticeParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            var heroDirectoryInfo = new DirectoryInfo(extractedDirectoryPath);
            
            if (heroDirectoryInfo.Name != "solstice")
            {
                return;
            }

            var heroModelFilePath = Path.Combine(extractedDirectoryPath, "night_form", $"model.mdf");

            var avatarDirectory = GetAvatarDirectory(extractedDirectoryPath, avatarKey);
            var avatarModelFilePath = Path.Combine(avatarDirectory, "night_form", $"model.mdf");

            SetAvatarModel(heroModelFilePath, avatarModelFilePath);
        }
    }
}
