using System.IO;
using System.Management.Automation;
using HoNAvatarManager.Core;

namespace HoNAvatarManager.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "HoNPath")]
    public class SetHoNPath : BaseCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Path { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter x64 { get; set; } = true;

        protected override void ProcessRecord()
        {
            if (!Directory.Exists(Path))
            {
                throw new DirectoryNotFoundException($"HoN directory not found at {Path}.");
            }

            var configurationManager = new ConfigurationManager("appsettings.json");

            if (x64.ToBool())
            {
                configurationManager.SetAppConfiguration(new AppConfiguration { HoNPath64 = Path });
            }
            else
            {
                configurationManager.SetAppConfiguration(new AppConfiguration { HoNPath32 = Path });
            }
        }
    }
}
