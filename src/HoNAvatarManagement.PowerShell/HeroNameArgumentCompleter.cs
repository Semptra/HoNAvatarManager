using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using HoNAvatarManager.Core;

namespace HoNAvatarManager.PowerShell
{
    public class HeroNameArgumentCompleter : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName, 
            string parameterName, 
            string wordToComplete, 
            CommandAst commandAst, 
            IDictionary fakeBoundParameters)
        {
            return HeroNameManager.HeroNames
                .Where(name => name.StartsWith(wordToComplete, StringComparison.InvariantCultureIgnoreCase))
                .Select(name => new CompletionResult($"\"{name}\""));
        }
    }
}
