using System.Management.Automation;
using HoNAvatarManager.Core;

namespace HoNAvatarManager.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "HeroAvatar")]
    public class SetHeroAvatar : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        [ArgumentCompleter(typeof(HeroNameArgumentCompleter))]
        public string Hero { get; set; }

        [Parameter(Mandatory = true)]
        [ArgumentCompleter(typeof(HeroAvatarArgumentCompleter))]
        public string Avatar { get; set; }

        protected override void ProcessRecord()
        {
            var avatarManager = new AvatarManager();

            avatarManager.SetHeroAvatar(Hero, Avatar);
        }
    }
}
