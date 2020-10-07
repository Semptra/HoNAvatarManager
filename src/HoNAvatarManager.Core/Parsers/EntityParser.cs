using System;
using System.Collections.Generic;
using System.Linq;

namespace HoNAvatarManager.Core.Parsers
{
    internal abstract class EntityParser : IEntityParser
    {
        protected readonly XmlManager _xmlManager;

        public EntityParser(XmlManager xmlManager)
        {
            _xmlManager = xmlManager;
        }

        public static IEnumerable<IEntityParser> GetRegisteredEntityParsers(XmlManager xmlManager)
        {
            var entityParserType = typeof(IEntityParser);

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => entityParserType.IsAssignableFrom(p) && !p.IsAbstract)
                .Select(t => Activator.CreateInstance(t, xmlManager))
                .OfType<IEntityParser>();
        }

        public abstract void SetEntity(string heroDirectoryPath, string avatarKey);
    }
}
