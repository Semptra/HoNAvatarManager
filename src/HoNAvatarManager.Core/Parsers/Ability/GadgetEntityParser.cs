using HoNAvatarManager.Core.Attributes;

namespace HoNAvatarManager.Core.Parsers.Ability
{
    [EntityParserPriority(2)]
    internal class GadgetEnemyEntityParser : AbilityBaseEntityParser
    {
        public GadgetEnemyEntityParser(XmlManager xmlManager) : base(xmlManager)
        {
            
        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            SetEntityInternal(resultDirectoryPath, avatarKey, "gadget");
        }
    }
}
