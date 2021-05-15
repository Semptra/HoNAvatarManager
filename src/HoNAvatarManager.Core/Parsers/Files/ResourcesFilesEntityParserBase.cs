namespace HoNAvatarManager.Core.Parsers.Model
{
    internal abstract class ResourcesFilesEntityParserBase : EntityParser
    {
        public ResourcesFilesEntityParserBase(XmlManager xmlManager) : base(xmlManager)
        {

        }

        protected readonly string[] DirectoriesToCopy = new string[]
        {
            "ability",
            "effects",
            "projectile",
            "sounds"
        };
    }
}
