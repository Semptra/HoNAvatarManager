namespace HoNAvatarManager.Core.Parsers.Ability
{
    internal class ProjectileEntityParser : AbilityBaseEntityParser
    {
        public ProjectileEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            SetEntityInternal(heroDirectoryPath, avatarKey, "projectile");
        }
    }
}
