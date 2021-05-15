using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Attributes;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Model
{
    [Disabled]
    [EntityParserPriority(1)]
    internal class SoundsFilesEntityParser : EntityParser
    {
        public SoundsFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }

            var avatarDirectoryPath = GetAvatarDirectory(extractedDirectoryPath, avatarKey);
            var heroSoundDirectoryPath = Path.Combine(extractedDirectoryPath, "sounds");
            var avatarSoundDirectoryPath = Path.Combine(avatarDirectoryPath, "sounds");

            if (!Directory.Exists(avatarSoundDirectoryPath))
            {
                Logging.Logger.Log.Warning($"Sounds directory not found for avatar.");
                return;
            }

            var avatarSoundFiles= Directory.EnumerateFiles(avatarSoundDirectoryPath).Select(f => new FileInfo(f));

            foreach (var avatarSoundFile in avatarSoundFiles)
            {
                var destinationSoundPath = Path.Combine(heroSoundDirectoryPath, avatarSoundFile.Name);

                avatarSoundFile.CopyTo(destinationSoundPath, true);
            }
        }
    }
}
