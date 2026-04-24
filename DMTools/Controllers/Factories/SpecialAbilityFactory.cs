using Data.Constant;
using Data.Constants;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Controllers.Factories
{
    public class SpecialAbilityFactory
    {
        public SpecialAbility GetObject(Guid id)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM SpecialAbilities WHERE Id = '{0}'", id);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            SpecialAbility specialAbility = new SpecialAbility();

            while (reader.Read())
            {
                specialAbility.Id = (Guid)reader["Id"];
                specialAbility.MonsterId = (Guid)reader["MonsterId"];
                specialAbility.Name = reader["Name"].ToString();
                specialAbility.Description = reader["Description"].ToString();

                if (reader["AttackBonus"] != DBNull.Value)
                    specialAbility.AttackBonus = Convert.ToInt16(reader["AttackBonus"]);

                specialAbility.SetInternalState(InternalStates.UnModified, true);
            }

            sqliteConnection.Close();

            return specialAbility;
        }

        public List<SpecialAbility> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM SpecialAbilities");
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<SpecialAbility> specialAbilities = new List<SpecialAbility>();

            while (reader.Read())
            {
                SpecialAbility specialAbility = new SpecialAbility();

                specialAbility.Id = (Guid)reader["Id"];
                specialAbility.MonsterId = (Guid)reader["MonsterId"];
                specialAbility.Name = reader["Name"].ToString();
                specialAbility.Description = reader["Description"].ToString();

                if(reader["AttackBonus"] != DBNull.Value)
                    specialAbility.AttackBonus = Convert.ToInt16(reader["AttackBonus"]);

                specialAbility.SetInternalState(InternalStates.UnModified, true);
                specialAbilities.Add(specialAbility);
            }

            sqliteConnection.Close();

            return specialAbilities;
        }

        public List<SpecialAbility> GetObjectsByMonsterId(Guid monsterId)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM SpecialAbilities WHERE MonsterId = '{0}'", monsterId);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<SpecialAbility> specialAbilities = new List<SpecialAbility>();

            while (reader.Read())
            {
                SpecialAbility specialAbility = new SpecialAbility();

                specialAbility.Id = (Guid)reader["Id"];
                specialAbility.MonsterId = (Guid)reader["MonsterId"];
                specialAbility.Name = reader["Name"].ToString();
                specialAbility.Description = reader["Description"].ToString();

                if (reader["AttackBonus"] != DBNull.Value)
                    specialAbility.AttackBonus = Convert.ToInt16(reader["AttackBonus"]);

                specialAbility.SetInternalState(InternalStates.UnModified, true);
                specialAbilities.Add(specialAbility);
            }

            sqliteConnection.Close();
            return specialAbilities;
        }

        public void SaveObject(SpecialAbility specialAbility)
        {
            if (specialAbility.InternalState != InternalStates.UnModified)
            {
                SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
                sqliteConnection.Open();

                String query = String.Empty;

                List<SQLiteParameter> parameterlist = null;

                switch (specialAbility.InternalState)
                {
                    case InternalStates.New:
                        query = @"  INSERT INTO SpecialAbilities(Id, MonsterId, Name, Description, AttackBonus) 
                                VALUES (@Id, @MonsterId, @Name, @Description, @AttackBonus)";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", specialAbility.Id) { DbType = DbType.String },
                            new SQLiteParameter("@MonsterId", specialAbility.MonsterId) { DbType = DbType.String },
                            new SQLiteParameter("@Name", specialAbility.Name),
                            new SQLiteParameter("@Description", specialAbility.Description),
                            new SQLiteParameter("@AttackBonus", specialAbility.AttackBonus)
                        };

                        break;

                    case InternalStates.Modified:
                        query = @"  UPDATE SpecialAbilities
                                SET MonsterId = @MonsterId,
                                    Name = @Name,
                                    Description = @Description,
                                    AttackBonus = @AttackBonus
                                WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", specialAbility.Id) { DbType = DbType.String },
                            new SQLiteParameter("@MonsterId", specialAbility.MonsterId) { DbType = DbType.String },
                            new SQLiteParameter("@Name", specialAbility.Name),
                            new SQLiteParameter("@Description", specialAbility.Description),
                            new SQLiteParameter("@AttackBonus", specialAbility.AttackBonus)
                        };

                        break;

                    case InternalStates.Deleted:
                        query = @"  DELETE FROM SpecialAbilities
                                WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", specialAbility.Id) { DbType = DbType.String }
                        };

                        break;
                }

                SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);
                command.Parameters.AddRange(parameterlist.ToArray());

                command.ExecuteNonQuery();

                specialAbility.SetInternalState(InternalStates.UnModified, true);

                sqliteConnection.Close();
            }
        }
    }
}
