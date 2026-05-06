using Controllers.Helpers;
using Data.Constant;
using Data.Constants;
using Data.Objects;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Factories
{
    public class MonsterFactory
    {
        public Monster GetObject(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Monster>("monsters");
                var monster = collection.FindById(monsterId);
                if (monster != null)
                {
                    monster.SetInternalState(InternalStates.UnModified, true);
                }
                return monster ?? new Monster();
            }
        }

        public List<Monster> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Monster>("monsters");
                var monsters = collection.FindAll().OrderBy(m => m.Name).ToList();
                foreach (var monster in monsters)
                {
                    monster.SetInternalState(InternalStates.UnModified, true);
                }
                return monsters;
            }
        }

        public List<Monster> GetObjectsBySearchCriteria(String name)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Monster>("monsters");
                var monsters = collection.Find(m => m.Name.Contains(name)).OrderBy(m => m.Name).ToList();
                foreach (var monster in monsters)
                {
                    monster.SetInternalState(InternalStates.UnModified, true);
                }
                return monsters;
            }
        }

        public void SaveObject(Monster monster)
        {
            if (monster.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<Monster>("monsters");

                    switch (monster.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(monster);
                            break;

                        case InternalStates.Modified:
                            collection.Update(monster);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(monster.Id);
                            break;
                    }

                    monster.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
