using System.Collections.Generic;

namespace HoNAvatarManager.Core
{
    internal static class HeroEvents
    {
        public const string OnAssist = "onassist";
        public const string OnChannelingStart = "onchannelingstart";
        public const string OnDeath = "ondeath";
        public const string OnFrame = "onframe";
        public const string OnInflicted = "oninflicted";
        public const string OnKill = "onkill";
        public const string OnKilled = "onkilled";
        public const string OnLevelUp = "onlevelup";
        public const string OnRespawn = "onrespawn";
        public const string OnSpawn = "onspawn";

        public static IEnumerable<string> GetAllEvents()
        {
            yield return OnAssist;
            yield return OnChannelingStart;
            yield return OnDeath;
            yield return OnFrame;
            yield return OnInflicted;
            yield return OnKill;
            yield return OnKilled;
            yield return OnLevelUp;
            yield return OnRespawn;
            yield return OnSpawn;
        }
    }
}
