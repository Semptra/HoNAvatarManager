namespace HoNAvatarManager.Core.Parsers.Ability
{
    internal class StateEnemyEntityParser : AbilityBaseEntityParser
    {
        public StateEnemyEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            SetEntityInternal(heroDirectoryPath, avatarKey, "state");
        }
    }
}
