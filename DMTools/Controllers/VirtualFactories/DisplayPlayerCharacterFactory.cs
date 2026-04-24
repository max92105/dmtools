using Data.Constant;
using Data.Constants;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Controllers.VirtualFactories
{
    public class DisplayPlayerCharacterFactory
    {
        public List<DisplayPlayerCharacter> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM PlayerCharacters ORDER BY Name");
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<DisplayPlayerCharacter> displayPlayerCharacters = new List<DisplayPlayerCharacter>();

            while (reader.Read())
            {
                DisplayPlayerCharacter displayPlayerCharacter = new DisplayPlayerCharacter();

                displayPlayerCharacter.Id = (Guid)reader["Id"];
                displayPlayerCharacter.Name = reader["Name"].ToString();
                displayPlayerCharacter.ArmorClass = Convert.ToInt16(reader["ArmorClass"]);
                displayPlayerCharacter.InitiativeBonus = Convert.ToInt16(reader["InitiativeBonus"]);
                displayPlayerCharacter.Level = Convert.ToInt16(reader["Level"]);
                displayPlayerCharacter.SetInternalState(InternalStates.UnModified, true);

                displayPlayerCharacters.Add(displayPlayerCharacter);
            }

            sqliteConnection.Close();
            return displayPlayerCharacters;
        }
    }
}
