using Data.Constants;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Controllers.VirtualFactories
{
    public class DisplayMonsterFactory
    {
        public List<DisplayMonster> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format(@" SELECT Monsters.Id,
                                            Monsters.Name,
                                            Monsters.Type,
                                            Monsters.Subtype,
                                            Monsters.ArmorClass,
                                            Monsters.HitPoints,
                                            Monsters.ChallengeRating
                                            FROM Monsters
                                            ORDER BY Name");

            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<DisplayMonster> displayMonsters = new List<DisplayMonster>();

            while (reader.Read())
            {
                DisplayMonster displayMonster = new DisplayMonster();

                displayMonster.Id = (Guid)reader["Id"];
                displayMonster.Name = reader["Name"].ToString();
                displayMonster.Type = reader["Type"].ToString();
                displayMonster.Subtype = reader["Subtype"].ToString();
                displayMonster.ArmorClass = Convert.ToInt16(reader["ArmorClass"]);
                displayMonster.HitPoints = Convert.ToInt16(reader["HitPoints"]);
                displayMonster.ChallengeRating = Convert.ToDecimal(reader["ChallengeRating"]);

                displayMonsters.Add(displayMonster);
            }

            sqliteConnection.Close();
            return displayMonsters;
        }

        public List<DisplayMonster> GetObjectsBySearchCriteria(String name)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format(@" SELECT Monsters.Id,
                                            Monsters.Name,
                                            Monsters.Type,
                                            Monsters.Subtype,
                                            Monsters.ArmorClass,
                                            Monsters.HitPoints,
                                            Monsters.ChallengeRating
                                            FROM Monsters 
                                            WHERE Name LIKE '%{0}%'
                                            ORDER BY Name", name);

            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<DisplayMonster> displayMonsters = new List<DisplayMonster>();

            while (reader.Read())
            {
                DisplayMonster displayMonster = new DisplayMonster();

                displayMonster.Id = (Guid)reader["Id"];
                displayMonster.Name = reader["Name"].ToString();
                displayMonster.Type = reader["Type"].ToString();
                displayMonster.ArmorClass = Convert.ToInt16(reader["ArmorClass"]);
                displayMonster.HitPoints = Convert.ToInt16(reader["HitPoints"]);
                displayMonster.ChallengeRating = Convert.ToDecimal(reader["ChallengeRating"]);

                displayMonsters.Add(displayMonster);
            }

            sqliteConnection.Close();
            return displayMonsters;
        }
    }
}