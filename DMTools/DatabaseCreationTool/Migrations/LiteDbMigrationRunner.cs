using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DatabaseCreationTool.Migrations
{
    public class LiteDbMigrationRunner
    {
        private const string MigrationCollectionName = "_migrations";
        private readonly LiteDatabase _db;
        private readonly List<LiteDbMigration> _migrations;

        public LiteDbMigrationRunner(LiteDatabase db)
        {
            _db = db;
            _migrations = new List<LiteDbMigration>();
        }

        public void LoadMigrationsFromAssembly(Assembly assembly)
        {
            var migrationTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(LiteDbMigration).IsAssignableFrom(t))
                .ToList();

            foreach (var type in migrationTypes)
            {
                var migration = (LiteDbMigration)Activator.CreateInstance(type);
                _migrations.Add(migration);
            }

            _migrations.Sort((a, b) => a.Version.CompareTo(b.Version));
        }

        public void MigrateUp()
        {
            var appliedVersions = GetAppliedVersions();

            foreach (var migration in _migrations)
            {
                if (!appliedVersions.Contains(migration.Version))
                {
                    Console.WriteLine(String.Format("[Migration {0}] Applying: {1}", migration.Version, migration.Description));

                    migration.Up(_db);

                    RecordMigration(migration.Version, migration.Description);

                    Console.WriteLine(String.Format("[Migration {0}] Applied successfully.", migration.Version));
                }
                else
                {
                    Console.WriteLine(String.Format("[Migration {0}] Already applied, skipping.", migration.Version));
                }
            }
        }

        public void MigrateDown(long targetVersion)
        {
            var appliedVersions = GetAppliedVersions();
            var migrationsToRollback = _migrations
                .Where(m => appliedVersions.Contains(m.Version) && m.Version > targetVersion)
                .OrderByDescending(m => m.Version)
                .ToList();

            foreach (var migration in migrationsToRollback)
            {
                Console.WriteLine(String.Format("[Migration {0}] Rolling back: {1}", migration.Version, migration.Description));

                migration.Down(_db);

                RemoveMigrationRecord(migration.Version);

                Console.WriteLine(String.Format("[Migration {0}] Rolled back successfully.", migration.Version));
            }
        }

        private HashSet<long> GetAppliedVersions()
        {
            var collection = _db.GetCollection<MigrationRecord>(MigrationCollectionName);
            return new HashSet<long>(collection.FindAll().Select(r => r.Version));
        }

        private void RecordMigration(long version, string description)
        {
            var collection = _db.GetCollection<MigrationRecord>(MigrationCollectionName);
            collection.Insert(new MigrationRecord
            {
                Version = version,
                Description = description,
                AppliedOn = DateTime.UtcNow
            });
        }

        private void RemoveMigrationRecord(long version)
        {
            var collection = _db.GetCollection<MigrationRecord>(MigrationCollectionName);
            collection.DeleteMany(r => r.Version == version);
        }
    }

    public class MigrationRecord
    {
        public int Id { get; set; }
        public long Version { get; set; }
        public string Description { get; set; }
        public DateTime AppliedOn { get; set; }
    }
}
