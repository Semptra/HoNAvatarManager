using HoNAvatarManager.Core.Attributes;

namespace HoNAvatarManager.Core.Parsers.Ability
{
    [Disabled]
    [EntityParserPriority(1)]
    internal class AffectorEntityParser : AbilityBaseEntityParser
    {
        public AffectorEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey)
        {
            SetEntityInternal(extractedDirectoryPath, avatarKey, "affector");
        }
    }
}
