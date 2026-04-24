using Data.Constants;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Controllers.Factories
{
    public class DisplaySpecialAbilityFactory
    {
        public List<DisplaySpecialAbility> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format(@" SELECT SpecialAbilities.Id, Monsters.Name || ' - ' || SpecialAbilities.Name AS DisplayName 
                                            FROM SpecialAbilities 
                                            INNER JOIN Monsters ON Monsters.Id = SpecialAbilities.MonsterId");

            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<DisplaySpecialAbility> displaySpecialAbilities = new List<DisplaySpecialAbility>();

            while (reader.Read())
            {
                DisplaySpecialAbility displaySpecialAbility = new DisplaySpecialAbility();

                displaySpecialAbility.Id = (Guid)reader["Id"];
                displaySpecialAbility.DisplayName = (String)reader["DisplayName"];

                displaySpecialAbilities.Add(displaySpecialAbility);
            }

            sqliteConnection.Close();
            return displaySpecialAbilities;
        }
    }
}
