using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using HoNAvatarManager.Core;

namespace HoNAvatarManager.PowerShell
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

            var avatarManager = new AvatarManager();

            return avatarManager.GetHeroAvatars(hero)
                .Where(avatar => avatar.StartsWith(wordToComplete, StringComparison.InvariantCultureIgnoreCase))
                .Select(avatar => new CompletionResult($"\"{avatarManager.GetHeroAvatarFriendlyName(hero, avatar)}\""));
        }
    }
}
