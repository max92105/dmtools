using Data.Constants;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Controllers.Factories
{
    public class DisplayActionFactory
    {
        public List<DisplayAction> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format(@" SELECT Actions.Id, Monsters.Name || ' - ' || Actions.Name AS DisplayName 
                                            FROM Actions 
                                            INNER JOIN Monsters ON Monsters.Id = Actions.MonsterId");

            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<DisplayAction> displayActions = new List<DisplayAction>();

            while (reader.Read())
            {
                DisplayAction displayAction = new DisplayAction();

                displayAction.Id = (Guid)reader["Id"];
                displayAction.DisplayName = (String)reader["DisplayName"];

                displayActions.Add(displayAction);
            }

            sqliteConnection.Close();
            return displayActions;
        }
    }
}
