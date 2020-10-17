using System.Management.Automation;
using HoNAvatarManager.Core;
using HoNAvatarManager.PowerShell.Completers;

namespace HoNAvatarManager.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "HeroAvatarUnpacked")]
    public class SetHeroAvatarUnpacked : BaseCmdlet
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

            avatarManager.SetHeroAvatarUnpacked(Hero, Avatar);
        }
    }
}
