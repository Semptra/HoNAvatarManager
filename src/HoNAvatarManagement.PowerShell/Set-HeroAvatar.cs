using System;
using System.Collections.Generic;
using System.Management.Automation;
using HoNAvatarManager.Core;
using HoNAvatarManager.PowerShell.Completers;
using HoNAvatarManager.PowerShell.Telemetry;

namespace HoNAvatarManager.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "HeroAvatar")]
    public class SetHeroAvatar : BaseCmdlet
    {
        [Parameter(Mandatory = true)]
        [ArgumentCompleter(typeof(HeroNameArgumentCompleter))]
        public string Hero { get; set; }

        [Parameter(Mandatory = true)]
        [ArgumentCompleter(typeof(HeroAvatarArgumentCompleter))]
        public string Avatar { get; set; }

        protected override void ProcessRecord()
        {
            var isTelemetryEnabled = ConfigurationManager.GetAppConfiguration().TelemetryEnabled;

            try
            {
                if (isTelemetryEnabled)
                {
                    TelemetryClient.TrackSetHeroAvatarEvent(Hero, Avatar);
                }

                var avatarManager = new AvatarManager();

                avatarManager.SetHeroAvatar(Hero, Avatar);
            }
            catch (Exception ex)
            {
                if (isTelemetryEnabled)
                {
                    TelemetryClient.TrackException(ex, new Dictionary<string, string> 
                    { 
                        { "Cmdlet", "Set-HeroAvatar" },
                        { "Hero", Hero },
                        { "Avatar", Avatar }
                    });
                }

                throw;
            }
        }
    }
}
