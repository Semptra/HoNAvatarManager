using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace HoNAvatarManager.Core
{
    public static class ConfigurationManager
    {
        private const string CONFIGURATION_FILE_NAME = "appsettings.json";

        public static AppConfiguration GetAppConfiguration()
        {
            return JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText(GetAppsettingsPath()));
        }

        public static void SetAppConfiguration(AppConfiguration appConfiguration) 
        {
            File.WriteAllText(GetAppsettingsPath(), JsonConvert.SerializeObject(appConfiguration));
        }

        private static string GetAppsettingsPath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), CONFIGURATION_FILE_NAME);
        }
    }
}
