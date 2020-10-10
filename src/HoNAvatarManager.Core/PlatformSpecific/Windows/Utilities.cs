using System.Security.Principal;

namespace HoNAvatarManager.Core.PlatformSpecific.Windows
{
    internal static class Utilities
    {
        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
