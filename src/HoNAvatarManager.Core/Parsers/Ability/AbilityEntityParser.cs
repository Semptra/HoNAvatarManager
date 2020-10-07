namespace HoNAvatarManager.Core.Parsers.Ability
{
    internal class AbilityEntityParser : AbilityBaseEntityParser
    {
        public AbilityEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            SetEntityInternal(heroDirectoryPath, avatarKey, "ability");
        }
    }
}
