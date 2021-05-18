using HoNAvatarManager.Core.Attributes;

namespace HoNAvatarManager.Core.Parsers.Ability
{
    [EntityParserPriority(2)]
    internal class StateEntityParser : AbilityBaseEntityParser
    {
        public StateEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            SetEntityInternal(extractedDirectoryPath, avatarKey, "state");
        }
    }
}
