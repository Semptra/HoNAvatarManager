namespace HoNAvatarManager.Core.Parsers.Ability
{
    internal class AffectorEntityParser : AbilityBaseEntityParser
    {
        public AffectorEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            SetEntityInternal(heroDirectoryPath, avatarKey, "affector");
        }
    }
}
