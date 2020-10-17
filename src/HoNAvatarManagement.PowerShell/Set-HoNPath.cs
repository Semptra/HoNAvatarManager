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

        protected override void ProcessRecord()
        {
            if (!Directory.Exists(Path))
            {
                throw new DirectoryNotFoundException($"HoN directory not found at {Path}.");
            }

            var configurationManager = new ConfigurationManager("appsettings.json");

            configurationManager.SetAppConfiguration(new AppConfiguration { HoNPath = Path });
        }
    }
}
