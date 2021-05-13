using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace HoNAvatarManager.Core
{
    public class AppConfiguration
    {
        [JsonProperty("HoNPath32")]
        public string HoNPath32 { get; set; }

        [JsonProperty("HoNPath64")]
        public string HoNPath64 { get; set; }

        public IEnumerable<string> GetHoNPath()
        {
            if (Directory.Exists(HoNPath64))
            {
                yield return HoNPath64;
            }

            if (Directory.Exists(HoNPath32))
            {
                yield return HoNPath32;
            }
        }
    }
}
