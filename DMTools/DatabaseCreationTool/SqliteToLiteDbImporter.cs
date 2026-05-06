using Data.Constants;
using Data.Objects;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace DatabaseCreationTool
{
    public class SqliteToLiteDbImporter
    {
        private readonly string _sqlitePath;
        private readonly string _liteDbPath;

        public SqliteToLiteDbImporter(string sqlitePath, string liteDbPath)
        {
            _sqlitePath = sqlitePath;
            _liteDbPath = liteDbPath;
        }

        public void Import()
        {
            if (!File.Exists(_sqlitePath))
            {
                Console.WriteLine("ERROR: SQLite database not found at: " + _sqlitePath);
                return;
            }

            string connectionString = String.Format("Data Source={0};Version=3;Read Only=True;", _sqlitePath);

            using (var sqliteConn = new SQLiteConnection(connectionString))
            using (var liteDb = new LiteDatabase(_liteDbPath))
            {
                sqliteConn.Open();

                // Try to recover the database first
                try
                {
                    using (var cmd = new SQLiteCommand("PRAGMA integrity_check", sqliteConn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result.ToString() != "ok")
                        {
                            Console.WriteLine("WARNING: SQLite database has integrity issues: " + result.ToString());
                            Console.WriteLine("Will attempt to import as much data as possible...");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("WARNING: Could not check database integrity: " + ex.Message);
                    Console.WriteLine("Will attempt to import as much data as possible...");
                }

                int imported = 0;
                int skipped = 0;

                // Migrate Monsters
                var result2 = TryImport("Monsters", () => ImportMonsters(sqliteConn, liteDb));
                imported += result2.Item1;
                skipped += result2.Item2;

                // Migrate Characteristics
                result2 = TryImport("Characteristics", () => ImportCharacteristics(sqliteConn, liteDb));
                imported += result2.Item1;
                skipped += result2.Item2;

                // Migrate Skills
                result2 = TryImport("Skills", () => ImportSkills(sqliteConn, liteDb));
                imported += result2.Item1;
                skipped += result2.Item2;

                // Migrate SpecialAbilities
                result2 = TryImport("SpecialAbilities", () => ImportSpecialAbilities(sqliteConn, liteDb));
                imported += result2.Item1;
                skipped += result2.Item2;

                // Migrate Actions
                result2 = TryImport("Actions", () => ImportActions(sqliteConn, liteDb));
                imported += result2.Item1;
                skipped += result2.Item2;

                // Migrate PlayerCharacters
                result2 = TryImport("PlayerCharacters", () => ImportPlayerCharacters(sqliteConn, liteDb));
                imported += result2.Item1;
                skipped += result2.Item2;

                Console.WriteLine(String.Format("Import complete. {0} records imported, {1} duplicates skipped.", imported, skipped));
            }
        }

        private Tuple<int, int> TryImport(string tableName, Func<Tuple<int, int>> importFunc)
        {
            Console.WriteLine("Importing " + tableName + "...");
            try
            {
                var result = importFunc();
                Console.WriteLine(String.Format("  {0}: {1} imported, {2} skipped.", tableName, result.Item1, result.Item2));
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("  ERROR importing {0}: {1}", tableName, ex.Message));
                Console.WriteLine("  Skipping this table and continuing...");
                return new Tuple<int, int>(0, 0);
            }
        }

        private Tuple<int, int> ImportMonsters(SQLiteConnection conn, LiteDatabase liteDb)
        {
            int imported = 0, skipped = 0;
            var collection = liteDb.GetCollection<Monster>("monsters");

            using (var cmd = new SQLiteCommand("SELECT * FROM Monsters", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = new Guid(reader["Id"].ToString());

                    if (collection.FindById(id) != null)
                    {
                        skipped++;
                        continue;
                    }

                    var monster = new Monster
                    {
                        Id = id,
                        Name = reader["Name"].ToString(),
                        Size = reader["Size"].ToString(),
                        Type = reader["Type"].ToString(),
                        Subtype = reader["Subtype"] == DBNull.Value ? null : reader["Subtype"].ToString(),
                        Alignment = reader["Alignment"].ToString(),
                        ArmorClass = Convert.ToInt16(reader["ArmorClass"]),
                        HitPoints = Convert.ToInt16(reader["HitPoints"]),
                        HitDice = reader["HitDice"].ToString(),
                        Speed = reader["Speed"].ToString(),
                        DamageVulnerabilities = reader["DamageVulnerabilities"] == DBNull.Value ? null : reader["DamageVulnerabilities"].ToString(),
                        DamageResistances = reader["DamageResistances"] == DBNull.Value ? null : reader["DamageResistances"].ToString(),
                        DamageImmunities = reader["DamageImmunities"] == DBNull.Value ? null : reader["DamageImmunities"].ToString(),
                        ConditionImmunities = reader["ConditionImmunities"] == DBNull.Value ? null : reader["ConditionImmunities"].ToString(),
                        Senses = reader["Senses"].ToString(),
                        Languages = reader["Languages"].ToString(),
                        ChallengeRating = Convert.ToDecimal(reader["ChallengeRating"])
                    };
                    collection.Insert(monster);
                    imported++;
                }
            }

            return new Tuple<int, int>(imported, skipped);
        }

        private Tuple<int, int> ImportCharacteristics(SQLiteConnection conn, LiteDatabase liteDb)
        {
            int imported = 0, skipped = 0;
            var collection = liteDb.GetCollection<Characteristic>("characteristics");

            using (var cmd = new SQLiteCommand("SELECT * FROM Characteristics", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = new Guid(reader["Id"].ToString());

                    if (collection.FindById(id) != null)
                    {
                        skipped++;
                        continue;
                    }

                    var characteristic = new Characteristic
                    {
                        Id = id,
                        MonsterId = new Guid(reader["MonsterId"].ToString()),
                        CharacteristicTypeId = new Guid(reader["CharacteristicTypeId"].ToString()),
                        Score = Convert.ToInt16(reader["Score"]),
                        Save = Convert.ToInt16(reader["Save"])
                    };
                    collection.Insert(characteristic);
                    imported++;
                }
            }

            return new Tuple<int, int>(imported, skipped);
        }

        private Tuple<int, int> ImportSkills(SQLiteConnection conn, LiteDatabase liteDb)
        {
            int imported = 0, skipped = 0;
            var collection = liteDb.GetCollection<Skill>("skills");

            using (var cmd = new SQLiteCommand("SELECT * FROM Skills", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = new Guid(reader["Id"].ToString());

                    if (collection.FindById(id) != null)
                    {
                        skipped++;
                        continue;
                    }

                    var skill = new Skill
                    {
                        Id = id,
                        MonsterId = new Guid(reader["MonsterId"].ToString()),
                        SkillTypeId = new Guid(reader["SkillTypeId"].ToString()),
                        Save = Convert.ToInt16(reader["Save"])
                    };
                    collection.Insert(skill);
                    imported++;
                }
            }

            return new Tuple<int, int>(imported, skipped);
        }

        private Tuple<int, int> ImportSpecialAbilities(SQLiteConnection conn, LiteDatabase liteDb)
        {
            int imported = 0, skipped = 0;
            var collection = liteDb.GetCollection<SpecialAbility>("specialAbilities");

            using (var cmd = new SQLiteCommand("SELECT * FROM SpecialAbilities", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = new Guid(reader["Id"].ToString());

                    if (collection.FindById(id) != null)
                    {
                        skipped++;
                        continue;
                    }

                    var specialAbility = new SpecialAbility
                    {
                        Id = id,
                        MonsterId = new Guid(reader["MonsterId"].ToString()),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        AttackBonus = reader["AttackBonus"] == DBNull.Value ? (short)0 : Convert.ToInt16(reader["AttackBonus"])
                    };
                    collection.Insert(specialAbility);
                    imported++;
                }
            }

            return new Tuple<int, int>(imported, skipped);
        }

        private Tuple<int, int> ImportActions(SQLiteConnection conn, LiteDatabase liteDb)
        {
            int imported = 0, skipped = 0;
            var collection = liteDb.GetCollection<Data.Objects.Action>("actions");

            using (var cmd = new SQLiteCommand("SELECT * FROM Actions", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = new Guid(reader["Id"].ToString());

                    if (collection.FindById(id) != null)
                    {
                        skipped++;
                        continue;
                    }

                    var action = new Data.Objects.Action
                    {
                        Id = id,
                        MonsterId = new Guid(reader["MonsterId"].ToString()),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        AttackBonus = reader["AttackBonus"] == DBNull.Value ? (short)0 : Convert.ToInt16(reader["AttackBonus"]),
                        DamageDice = reader["DamageDice"] == DBNull.Value ? null : reader["DamageDice"].ToString(),
                        DamageBonus = reader["DamageBonus"] == DBNull.Value ? (short)0 : Convert.ToInt16(reader["DamageBonus"]),
                        IsLegendary = Convert.ToBoolean(reader["IsLegendary"]),
                        IsBonus = HasColumn(reader, "IsBonus") && reader["IsBonus"] != DBNull.Value && Convert.ToBoolean(reader["IsBonus"]),
                        IsReaction = HasColumn(reader, "IsReaction") && reader["IsReaction"] != DBNull.Value && Convert.ToBoolean(reader["IsReaction"])
                    };
                    collection.Insert(action);
                    imported++;
                }
            }

            return new Tuple<int, int>(imported, skipped);
        }

        private Tuple<int, int> ImportPlayerCharacters(SQLiteConnection conn, LiteDatabase liteDb)
        {
            int imported = 0, skipped = 0;
            var collection = liteDb.GetCollection<PlayerCharacter>("playerCharacters");

            // Check if table exists
            using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='PlayerCharacters'", conn))
            {
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    Console.WriteLine("  PlayerCharacters table not found, skipping.");
                    return new Tuple<int, int>(0, 0);
                }
            }

            using (var cmd = new SQLiteCommand("SELECT * FROM PlayerCharacters", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = new Guid(reader["Id"].ToString());

                    if (collection.FindById(id) != null)
                    {
                        skipped++;
                        continue;
                    }

                    var pc = new PlayerCharacter
                    {
                        Id = id,
                        Name = reader["Name"].ToString(),
                        ArmorClass = Convert.ToInt16(reader["ArmorClass"]),
                        InitiativeBonus = HasColumn(reader, "InitiativeBonus") && reader["InitiativeBonus"] != DBNull.Value ? Convert.ToInt16(reader["InitiativeBonus"]) : (short)0,
                        Level = HasColumn(reader, "Level") && reader["Level"] != DBNull.Value ? Convert.ToInt16(reader["Level"]) : (short)1
                    };
                    collection.Insert(pc);
                    imported++;
                }
            }

            return new Tuple<int, int>(imported, skipped);
        }

        private static bool HasColumn(SQLiteDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
