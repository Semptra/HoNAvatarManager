using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Model
{
    internal class ModelFilesEntityParser : EntityParser
    {
        public ModelFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }

            var avatarDirectoryPath = GetAvatarDirectory(heroDirectoryPath, avatarKey);
            var heroClipsDirectoryPath = Path.Combine(heroDirectoryPath, "clips");
            var avatarClipsDirectoryPath = Path.Combine(avatarDirectoryPath, "clips");

            if (!Directory.Exists(avatarClipsDirectoryPath))
            {
                Logging.Logger.Log.Warning($"Clips directory not found.");
                return;
            }

            var avatarClips = Directory.EnumerateFiles(avatarClipsDirectoryPath).Select(f => new FileInfo(f));

            foreach (var avatarClip in avatarClips)
            {
                var destinatioClipPath = Path.Combine(heroClipsDirectoryPath, avatarClip.Name);

                avatarClip.CopyTo(destinatioClipPath, true);
            }
        }
    }
}
