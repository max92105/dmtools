using Controllers.Factories;
using Data.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using TypeEntry = Data.Objects.Type;

namespace Controllers.Controllers
{
    public static class ConfigurationPageDataController
    {
        public const string MonsterTypesCollection    = "monster_types";
        public const string MonsterSubtypesCollection = "monster_subtypes";

        public static List<TypeEntry> LoadMonsterTypes()
        {
            var factory = new TypeFactory(MonsterTypesCollection);
            var items = factory.GetObjects();
            if (!items.Any())
            {
                foreach (var name in MonsterTypes.All)
                    factory.SaveObject(MakeEntry(name));
                items = factory.GetObjects();
            }
            return items;
        }

        public static List<TypeEntry> LoadMonsterSubtypes()
        {
            var factory = new TypeFactory(MonsterSubtypesCollection);
            var items = factory.GetObjects();
            if (!items.Any())
            {
                foreach (var name in MonsterSubtypes.All.Where(s => !string.IsNullOrEmpty(s)))
                    factory.SaveObject(MakeEntry(name));
                items = factory.GetObjects();
            }
            return items;
        }

        public static void SaveType(TypeEntry entry)
            => new TypeFactory(MonsterTypesCollection).SaveObject(entry);

        public static void SaveSubtype(TypeEntry entry)
            => new TypeFactory(MonsterSubtypesCollection).SaveObject(entry);

        // Creates a new TypeEntry in New state with the given name.
        // (New→Modified transition is not in StateTransitions, so setting Name
        //  keeps the state as New — which is what we need for Insert.)
        private static TypeEntry MakeEntry(string name)
        {
            var e = new TypeEntry { Id = Guid.NewGuid() };
            e.Name = name;
            return e;
        }
    }
}
