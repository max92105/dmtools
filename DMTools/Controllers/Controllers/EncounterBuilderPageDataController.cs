using Controllers.Factories;
using Data.Objects;
using System;
using System.Collections.Generic;

namespace Controllers.Controllers
{
    public static class EncounterBuilderPageDataController
    {
        public static List<Monster> LoadMonsters()
            => new MonsterFactory().GetObjects();

        public static List<Monster> LoadMonstersByName(string name)
            => new MonsterFactory().GetObjectsBySearchCriteria(name ?? string.Empty);

        public static List<Encounter> LoadEncounters()
            => new EncounterFactory().GetObjects();

        public static List<EncounterEntry> LoadEncounterEntries(Guid encounterId)
            => new EncounterEntryFactory().GetObjectsByEncounterId(encounterId);

        public static Guid SaveEncounter(Guid existingId, string name, IList<EncounterEntry> entries)
        {
            var encounterFactory = new EncounterFactory();
            var entryFactory = new EncounterEntryFactory();

            Encounter encounter;
            if (existingId == Guid.Empty)
            {
                encounter = new Encounter { Id = Guid.NewGuid() };
                encounter.Name = name;
            }
            else
            {
                encounter = encounterFactory.GetObject(existingId);
                encounter.Name = name;
            }
            encounterFactory.SaveObject(encounter);

            entryFactory.DeleteAllByEncounterId(encounter.Id);

            foreach (var e in entries)
            {
                var newEntry = new EncounterEntry
                {
                    Id = Guid.NewGuid(),
                    EncounterId = encounter.Id,
                    MonsterId = e.MonsterId,
                    MonsterName = e.MonsterName,
                    Quantity = e.Quantity
                };
                entryFactory.SaveObject(newEntry);
            }

            return encounter.Id;
        }

        public static void DeleteEncounter(Guid encounterId)
        {
            new EncounterEntryFactory().DeleteAllByEncounterId(encounterId);
            var encounter = new EncounterFactory().GetObject(encounterId);
            if (encounter.Id != Guid.Empty)
            {
                encounter.Delete();
                new EncounterFactory().SaveObject(encounter);
            }
        }
    }
}
