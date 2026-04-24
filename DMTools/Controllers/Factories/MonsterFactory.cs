using Data.Constant;
using Data.Constants;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Controllers.Factories
{
    public class MonsterFactory
    {
        public Monster GetObject(Guid monsterId)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Monsters WHERE Monsters.Id = '{0}'", monsterId);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            Monster monster = new Monster();

            while (reader.Read())
            {
                monster.Id = new Guid(reader["Id"].ToString());
                monster.Name = reader["Name"].ToString();
                monster.Size = reader["Size"].ToString();
                monster.Type = reader["Type"].ToString();
                monster.Subtype = reader["Subtype"].ToString();
                monster.Alignment = reader["Alignment"].ToString();
                monster.ArmorClass = Convert.ToInt16(reader["ArmorClass"]);
                monster.HitPoints = Convert.ToInt16(reader["HitPoints"]);
                monster.HitDice = reader["HitDice"].ToString();
                monster.Speed = reader["Speed"].ToString();
                monster.DamageVulnerabilities = reader["DamageVulnerabilities"].ToString();
                monster.DamageResistances = reader["DamageResistances"].ToString();
                monster.DamageImmunities = reader["DamageImmunities"].ToString();
                monster.ConditionImmunities = reader["ConditionImmunities"].ToString();
                monster.Senses = reader["Senses"].ToString();
                monster.Languages = reader["Languages"].ToString();
                monster.ChallengeRating = Convert.ToDecimal(reader["ChallengeRating"]);
                monster.SetInternalState(InternalStates.UnModified, true);
            }

            sqliteConnection.Close();
            return monster;
        }

        public List<Monster> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Monsters ORDER BY Name");
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<Monster> monsters = new List<Monster>();

            while (reader.Read())
            {
                Monster monster = new Monster();

                monster.Id = new Guid(reader["Id"].ToString());
                monster.Name = reader["Name"].ToString();
                monster.Size = reader["Size"].ToString();
                monster.Type = reader["Type"].ToString();
                monster.Subtype = reader["Subtype"].ToString();
                monster.Alignment = reader["Alignment"].ToString();
                monster.ArmorClass = Convert.ToInt16(reader["ArmorClass"]);
                monster.HitPoints = Convert.ToInt16(reader["HitPoints"]);
                monster.HitDice = reader["HitDice"].ToString();
                monster.Speed = reader["Speed"].ToString();
                monster.DamageVulnerabilities = reader["DamageVulnerabilities"].ToString();
                monster.DamageResistances = reader["DamageResistances"].ToString();
                monster.DamageImmunities = reader["DamageImmunities"].ToString();
                monster.ConditionImmunities = reader["ConditionImmunities"].ToString();
                monster.Senses = reader["Senses"].ToString();
                monster.Languages = reader["Languages"].ToString();
                monster.ChallengeRating = Convert.ToDecimal(reader["ChallengeRating"]);
                monster.SetInternalState(InternalStates.UnModified, true);
                
                monsters.Add(monster);
            }
           
            sqliteConnection.Close();
            return monsters;
        }

        public List<Monster> GetObjectsBySearchCriteria(String name)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Monsters WHERE Name LIKE '%{0}%' ORDER BY Name", name);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<Monster> monsters = new List<Monster>();

            while (reader.Read())
            {
                Monster monster = new Monster();

                monster.Id = new Guid(reader["Id"].ToString());
                monster.Name = reader["Name"].ToString();
                monster.Size = reader["Size"].ToString();
                monster.Type = reader["Type"].ToString();
                monster.Subtype = reader["Subtype"].ToString();
                monster.Alignment = reader["Alignment"].ToString();
                monster.ArmorClass = Convert.ToInt16(reader["ArmorClass"]);
                monster.HitPoints = Convert.ToInt16(reader["HitPoints"]);
                monster.HitDice = reader["HitDice"].ToString();
                monster.Speed = reader["Speed"].ToString();
                monster.DamageVulnerabilities = reader["DamageVulnerabilities"].ToString();
                monster.DamageResistances = reader["DamageResistances"].ToString();
                monster.DamageImmunities = reader["DamageImmunities"].ToString();
                monster.ConditionImmunities = reader["ConditionImmunities"].ToString();
                monster.Senses = reader["Senses"].ToString();
                monster.Languages = reader["Languages"].ToString();
                monster.ChallengeRating = Convert.ToDecimal(reader["ChallengeRating"]);
                monster.SetInternalState(InternalStates.UnModified, true);

                monsters.Add(monster);
            }

            sqliteConnection.Close();
            return monsters;
        }

        public void SaveObject(Monster monster)
        {
            if (monster.InternalState != InternalStates.UnModified)
            {
                SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
                sqliteConnection.Open();

                String query = String.Empty;

                List<SQLiteParameter> parameterlist = null;

                switch (monster.InternalState)
                {
                    case InternalStates.New:
                        query = @"  INSERT INTO Monsters(Id,
                                                    Name,
                                                    Size,
                                                    Type,
                                                    Subtype,
                                                    Alignment,
                                                    ArmorClass,
                                                    HitPoints,
                                                    HitDice,
                                                    Speed,
                                                    DamageVulnerabilities,
                                                    DamageResistances,
                                                    DamageImmunities,
                                                    ConditionImmunities,
                                                    Senses,
                                                    Languages,
                                                    ChallengeRating) 
                                VALUES (@Id,
                                        @Name,
                                        @Size,
                                        @Type,
                                        @Subtype,
                                        @Alignment,       
                                        @ArmorClass,
                                        @HitPoints,
                                        @HitDice,
                                        @Speed,
                                        @DamageVulnerabilities,
                                        @DamageResistances,
                                        @DamageImmunities,
                                        @ConditionImmunities,
                                        @Senses,
                                        @Languages,
                                        @ChallengeRating)";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", monster.Id) { DbType = DbType.String },
                            new SQLiteParameter("@Name", monster.Name),
                            new SQLiteParameter("@Size", monster.Size),
                            new SQLiteParameter("@Type", monster.Type),
                            new SQLiteParameter("@Subtype", monster.Subtype),
                            new SQLiteParameter("@Alignment", monster.Alignment),
                            new SQLiteParameter("@ArmorClass", monster.ArmorClass),
                            new SQLiteParameter("@HitPoints", monster.HitPoints),
                            new SQLiteParameter("@HitDice", monster.HitDice),
                            new SQLiteParameter("@Speed", monster.Speed),
                            new SQLiteParameter("@DamageVulnerabilities", monster.DamageVulnerabilities),
                            new SQLiteParameter("@DamageResistances", monster.DamageResistances),
                            new SQLiteParameter("@DamageImmunities", monster.DamageImmunities),
                            new SQLiteParameter("@ConditionImmunities", monster.ConditionImmunities),
                            new SQLiteParameter("@Senses", monster.Senses),
                            new SQLiteParameter("@Languages", monster.Languages),
                            new SQLiteParameter("@ChallengeRating", monster.ChallengeRating)
                        };
                        break;

                    case InternalStates.Modified:
                        query = @"  UPDATE Monsters
                                SET Name = @Name,
                                    Size = @Size,
                                    Type = @Type,
                                    Subtype = @Subtype,
                                    Alignment = @Alignment,
                                    ArmorClass = @ArmorClass,
                                    HitPoints = @HitPoints,
                                    HitDice = @HitDice,
                                    Speed = @Speed,
                                    DamageVulnerabilities = @DamageVulnerabilities,
                                    DamageResistances = @DamageResistances,
                                    DamageImmunities = @DamageImmunities,
                                    ConditionImmunities = @ConditionImmunities,
                                    Senses = @Senses,
                                    Languages = @Languages,
                                    ChallengeRating = @ChallengeRating
                                WHERE Monsters.Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", monster.Id) { DbType = DbType.String },
                            new SQLiteParameter("@Name", monster.Name),
                            new SQLiteParameter("@Size", monster.Size),
                            new SQLiteParameter("@Type", monster.Type),
                            new SQLiteParameter("@Subtype", monster.Subtype),
                            new SQLiteParameter("@Alignment", monster.Alignment),
                            new SQLiteParameter("@ArmorClass", monster.ArmorClass),
                            new SQLiteParameter("@HitPoints", monster.HitPoints),
                            new SQLiteParameter("@HitDice", monster.HitDice),
                            new SQLiteParameter("@Speed", monster.Speed),
                            new SQLiteParameter("@DamageVulnerabilities", monster.DamageVulnerabilities),
                            new SQLiteParameter("@DamageResistances", monster.DamageResistances),
                            new SQLiteParameter("@DamageImmunities", monster.DamageImmunities),
                            new SQLiteParameter("@ConditionImmunities", monster.ConditionImmunities),
                            new SQLiteParameter("@Senses", monster.Senses),
                            new SQLiteParameter("@Languages", monster.Languages),
                            new SQLiteParameter("@ChallengeRating", monster.ChallengeRating)
                        };
                        break;

                    case InternalStates.Deleted:
                        query = @"  DELETE FROM Monsters
                                WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", monster.Id) { DbType = DbType.String }
                        };
                        break;
                }

                SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);
                command.Parameters.AddRange(parameterlist.ToArray());

                command.ExecuteNonQuery();

                monster.SetInternalState(InternalStates.UnModified, true);

                sqliteConnection.Close();
            }
        }
    }
}
