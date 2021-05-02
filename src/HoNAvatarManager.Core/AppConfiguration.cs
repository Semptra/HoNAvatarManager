using System.IO;
using Newtonsoft.Json;

namespace HoNAvatarManager.Core
{
    public class AppConfiguration
    {
        [JsonProperty("HoNPath32")]
        public string HoNPath32 { get; set; }

        [JsonProperty("HoNPath64")]
        public string HoNPath64 { get; set; }

        public string GetHoNPath()
        {
            return Directory.Exists(HoNPath64) ? HoNPath64 : HoNPath32;
        }
    }
}
