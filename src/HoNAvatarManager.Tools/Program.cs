﻿using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Xml.Dom;
using AngleSharp.Xml.Parser;
using HoNAvatarManager.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HoNAvatarManager.Tools
{
    class Program
    {
        public static List<string> HeroNames { get; } = new List<string>
        {
            "Accursed",
            "Adrenaline",
            "Aluna",
            "Amun-Ra",
            "Andromeda",
            "Apex",
            "Arachna",
            "Armadon",
            "Artesia",
            "Artillery",
            "Balphagore",
            "Behemoth",
            "Berzerker",
            "Blacksmith",
            "Blitz",
            "Blood Hunter",
            "Bombardier",
            "Bramble",
            "Bubbles",
            "Bushwack",
            "Calamity",
            "Chi",
            "Chronos",
            "Circe",
            "Corrupted Disciple",
            "Cthulhuphant",
            "Dampeer",
            "Deadlift",
            "Deadwood",
            "Defiler",
            "Demented Shaman",
            "Devourer",
            "Doctor Repulsor",
            "Draconis",
            "Drunken Master",
            "Electrician",
            "Ellonia",
            "Emerald Warden",
            "Empath",
            "Engineer",
            "Fayde",
            "Flint Beastwood",
            "Flux",
            "Forsaken Archer",
            "Gauntlet",
            "Gemini",
            "Geomancer",
            "Glacius",
            "Goldenveil",
            "Gravekeeper",
            "Grinex",
            "Gunblade",
            "Hammerstorm",
            "Hellbringer",
            "Ichor",
            "Jeraziah",
            "Kane",
            "Keeper of the Forest",
            "Kinesis",
            "King Klout",
            "Klanx",
            "Kraken",
            "Legionnaire",
            "Lodestone",
            "Lord Salforis",
            "Magebane",
            "Magmus",
            "Maliken",
            "Martyr",
            "Master Of Arms",
            "Midas",
            "Mimix",
            "Moira",
            "Monarch",
            "Monkey King",
            "Moon Queen",
            "Moraxus",
            "Myrmidon",
            "Night Hound",
            "Nitro",
            "Nomad",
            "Nymphora",
            "Oogie",
            "Ophelia",
            "Pandamonium",
            "Parallax",
            "Parasite",
            "Pearl",
            "Pebbles",
            "Pestilence",
            "Pharaoh",
            "Plague Rider",
            "Pollywog Priest",
            "Predator",
            "Prisoner 945",
            "Prophet",
            "Puppet Master",
            "Pyromancer",
            "Rally",
            "Rampage",
            "Ravenor",
            "Revenant",
            "Rhapsody",
            "Riftwalker",
            "Riptide",
            "Salomon",
            "Sand Wraith",
            "Sapphire",
            "Scout",
            "Shadowblade",
            "Shellshock",
            "Silhouette",
            "Sir Benzington",
            "Skrap",
            "Slither",
            "Solstice",
            "Soul Reaper",
            "Soulstealer",
            "Succubus",
            "Swiftblade",
            "Tarot",
            "Tempest",
            "The Chipper",
            "The Dark Lady",
            "The Gladiator",
            "The Madman",
            "Thunderbringer",
            "Torturer",
            "Tremble",
            "Tundra",
            "Valkyrie",
            "Vindicator",
            "Voodoo Jester",
            "War Beast",
            "Warchief",
            "Wildsoul",
            "Witch Slayer",
            "Wretched Hag",
            "Zephyr"
        };

        static async Task Main(string[] args)
        {
            var heroModelMdf = GetXmlDocument(@"C:\Program Files (x86)\Heroes of Newerth\game\heroes\solstice\model.mdf");
            var avatarModelMdf = GetXmlDocument(@"C:\Program Files (x86)\Heroes of Newerth\game\heroes\solstice\alt\model.mdf");

            var heroModelAnimations = heroModelMdf.QuerySelector("model").QuerySelectorAll("anim");
            var avatarModelAnimations = avatarModelMdf.QuerySelector("model").QuerySelectorAll("anim");

            Console.WriteLine($"Hero animations: {heroModelAnimations.Count()}");
            foreach(var heroModelAnimation in heroModelAnimations)
            {
                Console.WriteLine($" - Animation: {heroModelAnimation.GetAttribute("name")}");
            }

            Console.WriteLine($"\nAvatar animations: {avatarModelAnimations.Count()}");
            foreach (var avatarModelAnimation in avatarModelAnimations)
            {
                Console.WriteLine($" - Animation: {avatarModelAnimation.GetAttribute("name")}");
            }
        }

        static IXmlDocument GetXmlDocument(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var parser = new XmlParser();
                return parser.ParseDocument(stream);
            }
        }

        static async Task ExtractAvatarIcons()
        {
            var httpClient = new HttpClient();

            foreach(var i in new int[] { 192 })
            {
                try
                {
                    var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
                    var heroDocument = await context.OpenAsync($"https://www.heroesofnewerth.com/heroes/view/{i}");

                    var heroTitle = heroDocument.QuerySelector("title").InnerHtml;

                    if (string.IsNullOrEmpty(heroTitle))
                    {
                        continue;
                    }

                    var heroName = heroTitle.Split(" ").Last().Trim().Trim('\t');
                    Console.WriteLine(heroName);

                    var failCount = 0;
                    for (int j = 0; j < 20; j++)
                    {
                        string avatarUrl;
                        if (j == 0)
                        {
                            avatarUrl = $"https://www.heroesofnewerth.com/images/heroes/{i}/icon_128.jpg";
                        }
                        else if (j == 1)
                        {
                            avatarUrl = $"https://www.heroesofnewerth.com/images/heroes/{i}/icon_128_alt.jpg";
                        }
                        else
                        {
                            avatarUrl = $"https://www.heroesofnewerth.com/images/heroes/{i}/icon_128_alt{j}.jpg";
                        }

                        var avatarImage = await httpClient.GetAsync(avatarUrl);

                        if (!avatarImage.IsSuccessStatusCode)
                        {
                            failCount++;
                            continue;
                        }

                        if (failCount > 2)
                        {
                            break;
                        }

                        var avatarImageContent = await avatarImage.Content.ReadAsByteArrayAsync();

                        var savePath = new FileInfo(Path.Combine(@"C:\HoNMods\ExtractTest\AvatarIcons", heroName, $"{heroName}_Avatar_{j}.jpg"));
                        Directory.CreateDirectory(savePath.DirectoryName);

                        await File.WriteAllBytesAsync(savePath.FullName, avatarImageContent);

                        Console.WriteLine($"    * Avatar {j}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cannot get hero {i}: {ex}");
                }

            }
        }

        static void GetAllHeroAvatars()
        {
            var avatarManager = new AvatarManager();

            foreach (var hero in HeroNames)
            {
                Console.WriteLine($"- [ ] {hero}");
                File.AppendAllText(@"C:\Projects\HoNAvatarManager\src\HoNAvatarManager.Tools\data.txt", $"- [ ] {hero}{Environment.NewLine}");

                var avatars = avatarManager.GetHeroAvatars(hero);
                foreach (var avatar in avatars)
                {
                    Console.WriteLine($"    - [ ] {avatarManager.GetHeroAvatarFriendlyName(hero, avatar)}");
                    File.AppendAllText(@"C:\Projects\HoNAvatarManager\src\HoNAvatarManager.Tools\data.txt", $"    - [ ] {avatarManager.GetHeroAvatarFriendlyName(hero, avatar)}{Environment.NewLine}");
                }
            }
        }

        static async Task GetAvatarsMapping()
        {
            var avatarManager = new AvatarManager();
            var map = JsonConvert.DeserializeObject<List<AvatarMapping>>(File.ReadAllText(@"C:\Projects\HoNAvatarManager\src\HoNAvatarManager.Core\map.json"));

            foreach (var hero in HeroNames)
            {
                try
                {
                    Console.WriteLine(hero);

                    var mapEntity = map.SingleOrDefault(x => x.Hero == hero);
                    var allAvatars = avatarManager.GetHeroAvatars(hero).ToList();
                    var avatars = avatarManager.GetHeroAvatars(hero).Where(x => x.Contains("alt", StringComparison.InvariantCultureIgnoreCase)).ToList();

                    for (int i = 0, j = 0; i < mapEntity.AvatarInfo.Count && j < avatars.Count; i++)
                    {
                        var mapAvatar = mapEntity.AvatarInfo[i].AvatarName;

                        string entityAvatar;

                        if (mapAvatar.Contains("POG"))
                        {
                            entityAvatar = allAvatars.FirstOrDefault(x => x.Contains("pog", StringComparison.InvariantCultureIgnoreCase));
                        }
                        else if (mapAvatar.Contains("Throwback"))
                        {
                            entityAvatar = allAvatars.FirstOrDefault(x => x.Contains("classic", StringComparison.InvariantCultureIgnoreCase));
                        }
                        else
                        {
                            entityAvatar = avatars[j++];
                        }

                        mapEntity.AvatarInfo[i].ResourceName = entityAvatar;

                        Console.WriteLine($"  * {mapAvatar} = {entityAvatar}");
                    }

                    Console.WriteLine();

                }
                catch (Exception ex)
                {
                    var currentColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error parsing hero {hero}: {ex}");
                    Console.ForegroundColor = currentColor;
                }
            }

            File.WriteAllText(@"C:\Projects\HoNAvatarManager\src\HoNAvatarManager.Core\map_new.json", JsonConvert.SerializeObject(map));
        }

        static async Task GetHeroMappingJson()
        {
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            var herosDocument = await context.OpenAsync("https://www.heroesofnewerth.com/heroes/");

            var result = new List<AvatarMapping>();

            int i = 1;
            foreach (var hero in HeroNames)
            {
                try
                {
                    var document = await GetHeroDocument(context, herosDocument, hero);

                    var script = document.QuerySelectorAll("script").FirstOrDefault(s => s.InnerHtml.Contains("alt_avatar_list"));

                    var lines = script.InnerHtml.Split('\n');

                    var regex = new Regex(@"var alt_avatar_list = [_\,\;\:\[\]\{\}\""a-zA-Z0-9 ]+");

                    var avatarListLine = lines.FirstOrDefault(l => regex.IsMatch(l))
                        .Replace("var alt_avatar_list = ", string.Empty)
                        .Replace(";", string.Empty);

                    var avatarList = JsonConvert.DeserializeObject<List<Avatar>>(avatarListLine);

                    result.Add(new AvatarMapping
                    {
                        Hero = hero,
                        AvatarInfo = avatarList.Select(a => new AvatarInfo { AvatarName = a.AvatarName, ResourceName = string.Empty }).ToList()
                    });

                    Console.WriteLine($"[{i}/{HeroNames.Count}] Added {avatarList.Count} avatars for {hero}...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting avatars for hero {hero}: {ex}");
                }
                finally
                {
                    i++;
                }
            }

            File.WriteAllText(@"C:\Projects\HoNAvatarManager\src\HoNAvatarManager.Core\map.json", JsonConvert.SerializeObject(result));
        }

        static async Task<IDocument> GetHeroDocument(IBrowsingContext context, IDocument heroesDocument, string hero)
        {
            var selections = heroesDocument.QuerySelectorAll("div.sectionHeroes");

            foreach (var selection in selections)
            {
                var heroesDivs = selection.QuerySelectorAll("div.filterObjectGrid");

                foreach (var heroDiv in heroesDivs)
                {
                    var name = heroDiv.QuerySelector("div.over.default")?.TextContent;
                    var href = heroDiv.QuerySelector("a")?.Attributes["href"];

                    if (string.Equals(hero, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return await context.OpenAsync("https://www.heroesofnewerth.com/" + href.Value);
                    }
                }
            }

            throw new Exception($"Hero {hero} page not found.");
        }
    }

    public class AvatarMapping
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

    public class Avatar
    {
        [JsonProperty("cname")]
        public string AvatarName { get; set; }

        [JsonProperty("product_id")]
        public string ProductId { get; set; }
    }
}
