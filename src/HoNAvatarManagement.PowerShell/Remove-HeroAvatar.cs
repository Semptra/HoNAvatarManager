using System.Management.Automation;
using HoNAvatarManager.Core;
using HoNAvatarManager.PowerShell.Completers;

namespace HoNAvatarManager.PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "HeroAvatar")]
    public class RemoveHeroAvatar : BaseCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "Single")]
        [ArgumentCompleter(typeof(HeroNameArgumentCompleter))]
        public string Hero { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "All")]
        public SwitchParameter All { get; set; }

        protected override void ProcessRecord()
        {
            var avatarManager = new AvatarManager();

            if (ParameterSetName == "Single")
            {
                avatarManager.RemoveHeroAvatar(Hero);
            }
            else if (ParameterSetName == "All")
            {
                foreach(var hero in GlobalResources.HeroNames)
                {
                    avatarManager.RemoveHeroAvatar(hero);
                }
            }
        }
    }
}
