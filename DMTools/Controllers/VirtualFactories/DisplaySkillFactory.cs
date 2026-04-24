using Data.Constant;
using Data.Constants;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Controllers.Factories
{
    public class DisplaySkillFactory
    {
        public List<DisplaySkill> GetObjectsByMonsterId(Guid monsterId)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format(@" SELECT Skills.*, SkillTypes.Name FROM Skills 
                                            INNER JOIN SkillTypes ON SkillTypes.Id = Skills.SkillTypeId
                                            WHERE monsterId = '{0}'
                                            ORDER BY SkillTypes.Name", monsterId);

            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<DisplaySkill> displaySkills = new List<DisplaySkill>();

            while (reader.Read())
            {
                DisplaySkill displaySkill = new DisplaySkill();

                displaySkill.Id = (Guid)reader["Id"];
                displaySkill.MonsterId = (Guid)reader["MonsterId"];
                displaySkill.SkillTypeId = (Guid)reader["SkillTypeId"];
                displaySkill.Name = reader["Name"].ToString();
                displaySkill.Save = Convert.ToInt16(reader["Save"]);
                displaySkill.SetInternalState(InternalStates.UnModified, true);

                displaySkills.Add(displaySkill);
            }

            sqliteConnection.Close();
            return displaySkills;
        }
    }
}
