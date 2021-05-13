using System.IO;
using System.Linq;
using HoNAvatarManager.Core.Extensions;

namespace HoNAvatarManager.Core.Parsers.Model
{
    internal class ProjectileFilesEntityParser : EntityParser
    {
        public ProjectileFilesEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            if (avatarKey.IsClassicAvatar())
            {
                return;
            }

            var avatarDirectoryPath = GetAvatarDirectory(heroDirectoryPath, avatarKey);
            var heroProjectileDirectoryPath = Path.Combine(heroDirectoryPath, "projectile");
            var avatarProjectileDirectoryPath = Path.Combine(avatarDirectoryPath, "projectile");

            if (!Directory.Exists(avatarProjectileDirectoryPath))
            {
                Logging.Logger.Log.Warning($"Projectile directory not found for avatar.");
                return;
            }

            var avatarProjectileFiles= Directory.EnumerateFiles(avatarProjectileDirectoryPath).Select(f => new FileInfo(f));

            foreach (var avatarProjectileFile in avatarProjectileFiles)
            {
                var destinationProjectilePath = Path.Combine(heroProjectileDirectoryPath, avatarProjectileFile.Name);

                avatarProjectileFile.CopyTo(destinationProjectilePath, true);
            }
        }
    }
}
