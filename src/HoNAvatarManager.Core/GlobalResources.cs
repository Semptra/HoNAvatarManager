using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace HoNAvatarManager.Core
{
    public static class GlobalResources
    {
        static GlobalResources()
        {
            var resourcesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

            var heroNamesJsonPath = Path.Combine(resourcesPath, "hero_names.json");
            var heroResourcesMappingJsonPath = Path.Combine(resourcesPath, "hero_resources_mapping.json");
            var heroAvatarMappingJsonPath = Path.Combine(resourcesPath, "avatar_names_mapping.json");

            HeroNames = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(heroNamesJsonPath));
            HeroResourcesMapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(heroResourcesMappingJsonPath));
            HeroAvatarMapping = JsonConvert.DeserializeObject<List<HeroAvatarMapping>>(File.ReadAllText(heroAvatarMappingJsonPath));
        }

        public static List<string> HeroNames { get; }

        public static Dictionary<string, string> HeroResourcesMapping { get; }

        public static List<HeroAvatarMapping> HeroAvatarMapping { get; }
    }

    public class HeroAvatarMapping
    {
        [JsonProperty("hero")]
        public string Hero { get; set; }

        [JsonProperty("avatarInfo")]
        public List<AvatarInfo> AvatarInfo { get; set; }
    }

    public class AvatarInfo
    {
        [JsonProperty("avatarName")]
        public string AvatarName { get; set; }

        [JsonProperty("resourceName")]
        public string ResourceName { get; set; }
    }
}
