using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using HoNAvatarManager.Core;

namespace HoNAvatarManager.PowerShell.Completers
{
    public class HeroAvatarArgumentCompleter : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName, 
            string parameterName, 
            string wordToComplete, 
            CommandAst commandAst, 
            IDictionary fakeBoundParameters)
        {
            if (!fakeBoundParameters.Contains("Hero"))
            {
                return Enumerable.Empty<CompletionResult>();
            }

            var hero = (string)fakeBoundParameters["Hero"];

            var heroAvatars = GlobalResources.HeroAvatarMapping.FirstOrDefault(x => x.Hero.Equals(hero, StringComparison.InvariantCultureIgnoreCase));

            return heroAvatars?.AvatarInfo
                .Where(avatar => avatar.AvatarName.StartsWith(wordToComplete, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(avatar => avatar.AvatarName)
                .Select(avatar => new CompletionResult($"\"{avatar.AvatarName}\""));
        }
    }
}
