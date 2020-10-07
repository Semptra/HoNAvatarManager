﻿namespace HoNAvatarManager.Core.Parsers.Ability
{
    internal class GadgetEnemyEntityParser : AbilityBaseEntityParser
    {
        public GadgetEnemyEntityParser(XmlManager xmlManager) : base(xmlManager)
        {

        }

        public override void SetEntity(string heroDirectoryPath, string avatarKey)
        {
            SetEntityInternal(heroDirectoryPath, avatarKey, "gadget");
        }
    }
}
