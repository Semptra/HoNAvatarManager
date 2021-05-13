using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Model
{
    internal class SoundsFilesEntityParser : EntityParser
    {
        public SoundsFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }

            var avatarDirectoryPath = GetAvatarDirectory(heroDirectoryPath, avatarKey);
            var heroSoundDirectoryPath = Path.Combine(heroDirectoryPath, "sounds");
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
