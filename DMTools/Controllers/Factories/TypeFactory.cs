using Controllers.Helpers;
using Data.Constant;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using TypeEntry = Data.Objects.Type;

namespace Controllers.Factories
{
    public class TypeFactory
    {
        private readonly string _collectionName;

        public TypeFactory(string collectionName)
        {
            _collectionName = collectionName;
        }

        public List<TypeEntry> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var col = db.GetCollection<TypeEntry>(_collectionName);
                var items = col.FindAll().OrderBy(t => t.Name).ToList();
                foreach (var item in items)
                    item.SetInternalState(InternalStates.UnModified, true);
                return items;
            }
        }

        public void SaveObject(TypeEntry entry)
        {
            if (entry.InternalState == InternalStates.UnModified) return;
            using (var db = DatabaseHelper.GetDatabase())
            {
                var col = db.GetCollection<TypeEntry>(_collectionName);
                switch (entry.InternalState)
                {
                    case InternalStates.New:      col.Insert(entry); break;
                    case InternalStates.Modified: col.Update(entry); break;
                    case InternalStates.Deleted:  col.Delete(entry.Id); break;
                }
                entry.SetInternalState(InternalStates.UnModified, true);
            }
        }
    }
}
