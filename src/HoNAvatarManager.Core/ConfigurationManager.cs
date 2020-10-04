using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace HoNAvatarManager.Core
{
    public class ConfigurationManager
    {
        private readonly string _configurationFileName;

        public ConfigurationManager(string configurationFileName)
        {
            _configurationFileName = configurationFileName;
        }

        public AppConfiguration GetAppConfiguration()
        {
            return JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText(GetAppsettingsPath()));
        }

        public void SetAppConfiguration(AppConfiguration appConfiguration) 
        {
            File.WriteAllText(GetAppsettingsPath(), JsonConvert.SerializeObject(appConfiguration));
        }

        private string GetAppsettingsPath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _configurationFileName);
        }
    }
}
