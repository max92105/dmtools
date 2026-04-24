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
    public class PlayerCharacterFactory
    {
        public PlayerCharacter GetObject(Guid playerCharacterId)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = @"SELECT * FROM PlayerCharacters WHERE PlayerCharacters.Id = @Id";

            List<SQLiteParameter> parameters = new List<SQLiteParameter>()
            {
                new SQLiteParameter("@Id", playerCharacterId)
            };

            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);
            command.Parameters.AddRange(parameters.ToArray());

            SQLiteDataReader reader = command.ExecuteReader();

            PlayerCharacter playerCharacter = new PlayerCharacter();

            while (reader.Read())
            {
                playerCharacter.Id = (Guid)reader["Id"];
                playerCharacter.Name = reader["Name"].ToString();
                playerCharacter.ArmorClass = Convert.ToInt16(reader["ArmorClass"]);
                playerCharacter.InitiativeBonus = Convert.ToInt16(reader["InitiativeBonus"]);
                playerCharacter.Level = Convert.ToInt16(reader["Level"]);
                playerCharacter.SetInternalState(InternalStates.UnModified, true);
            }

            sqliteConnection.Close();
            return playerCharacter;
        }

        public List<PlayerCharacter> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM PlayerCharacters ORDER BY Name");
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();

            while (reader.Read())
            {
                PlayerCharacter playerCharacter = new PlayerCharacter();

                playerCharacter.Id = (Guid)reader["Id"];
                playerCharacter.Name = reader["Name"].ToString();
                playerCharacter.ArmorClass = Convert.ToInt16(reader["ArmorClass"]);
                playerCharacter.InitiativeBonus = Convert.ToInt16(reader["InitiativeBonus"]);
                playerCharacter.Level = Convert.ToInt16(reader["Level"]);
                playerCharacter.SetInternalState(InternalStates.UnModified, true);

                playerCharacters.Add(playerCharacter);
            }

            sqliteConnection.Close();
            return playerCharacters;
        }

        public List<PlayerCharacter> GetObjectsBySearchCriteria(String name)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM PlayerCharacters WHERE Name LIKE '%{0}%' ORDER BY Name", name);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();

            while (reader.Read())
            {
                PlayerCharacter playerCharacter = new PlayerCharacter();

                playerCharacter.Id = (Guid)reader["Id"];
                playerCharacter.Name = reader["Name"].ToString();
                playerCharacter.ArmorClass = Convert.ToInt16(reader["ArmorClass"]);
                playerCharacter.InitiativeBonus = Convert.ToInt16(reader["InitiativeBonus"]);
                playerCharacter.Level = Convert.ToInt16(reader["Level"]);
                playerCharacter.SetInternalState(InternalStates.UnModified, true);

                playerCharacters.Add(playerCharacter);
            }

            sqliteConnection.Close();
            return playerCharacters;
        }

        public void SaveObject(PlayerCharacter playerCharacter)
        {
            if (playerCharacter.InternalState != InternalStates.UnModified)
            {
                SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
                sqliteConnection.Open();

                String query = String.Empty;

                List<SQLiteParameter> parameterlist = null;

                switch (playerCharacter.InternalState)
                {
                    case InternalStates.New:
                        query = @"  INSERT INTO PlayerCharacters(Id, Name, ArmorClass, InitiativeBonus, Level) 
                                    VALUES (@Id, @Name, @ArmorClass, @InitiativeBonus, @Level)";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", playerCharacter.Id),
                            new SQLiteParameter("@Name", playerCharacter.Name),
                            new SQLiteParameter("@ArmorClass", playerCharacter.ArmorClass),
                            new SQLiteParameter("@InitiativeBonus", playerCharacter.InitiativeBonus),
                            new SQLiteParameter("@Level", playerCharacter.Level)
                        };
                        break;

                    case InternalStates.Modified:
                        query = @"  UPDATE PlayerCharacters
                                    SET Name = @Name,
                                        ArmorClass = @ArmorClass,
                                        InitiativeBonus = @InitiativeBonus,
                                        Level = @Level
                                    WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", playerCharacter.Id),
                            new SQLiteParameter("@Name", playerCharacter.Name),
                            new SQLiteParameter("@ArmorClass", playerCharacter.ArmorClass),
                            new SQLiteParameter("@InitiativeBonus", playerCharacter.InitiativeBonus),
                            new SQLiteParameter("@Level", playerCharacter.Level)
                        };
                        break;

                    case InternalStates.Deleted:
                        query = @"  DELETE FROM PlayerCharacters
                                    WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", playerCharacter.Id)
                        };
                        break;
                }

                SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);
                command.Parameters.AddRange(parameterlist.ToArray());

                command.ExecuteNonQuery();

                playerCharacter.SetInternalState(InternalStates.UnModified, true);

                sqliteConnection.Close();
            }
        }
    }
}
