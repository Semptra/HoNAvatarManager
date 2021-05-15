namespace HoNAvatarManager.Core.Parsers
{
    internal interface IEntityParser
    {
        void SetEntity(string extractedDirectoryPath, string resultDirectoryPath, string avatarKey);
    }
}
