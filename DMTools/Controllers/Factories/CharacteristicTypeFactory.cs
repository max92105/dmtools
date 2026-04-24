using Data.Constant;
using Data.Constants;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Controllers.Factories
{
    public class CharacteristicTypeFactory
    {
        public List<CharacteristicType> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM CharacteristicTypes");
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<CharacteristicType> characteristicTypes = new List<CharacteristicType>();

            while (reader.Read())
            {
                CharacteristicType characteristicType = new CharacteristicType();

                characteristicType.Id = (Guid)reader["Id"];
                characteristicType.Name = reader["Name"].ToString();
                characteristicType.SetInternalState(InternalStates.UnModified, true);

                characteristicTypes.Add(characteristicType);
            }

            sqliteConnection.Close();
            return characteristicTypes;
        }
    }
}
