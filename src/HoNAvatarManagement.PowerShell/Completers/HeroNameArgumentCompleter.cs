using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using HoNAvatarManager.Core;

namespace HoNAvatarManager.PowerShell.Completers
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
            return GlobalResources.HeroNames
                .Where(name => name.StartsWith(wordToComplete.Trim('"'), StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(name => name)
                .Select(name => new CompletionResult($"\"{name}\"", name, CompletionResultType.ParameterValue, name));
        }
    }
}
