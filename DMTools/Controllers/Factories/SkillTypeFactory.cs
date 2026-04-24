using Data.Constant;
using Data.Constants;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Controllers.Factories
{
    public class SkillTypeFactory
    {
        public List<SkillType> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM SkillTypes ORDER BY Name");
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<SkillType> skillTypes = new List<SkillType>();

            while (reader.Read())
            {
                SkillType skillType = new SkillType();

                skillType.Id = (Guid)reader["Id"];
                skillType.Name = reader["Name"].ToString();
                skillType.SetInternalState(InternalStates.UnModified, true);

                skillTypes.Add(skillType);
            }

            sqliteConnection.Close();
            return skillTypes;
        }
    }
}
