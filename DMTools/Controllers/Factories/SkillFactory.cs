using Data.Constant;
using Data.Constants;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Controllers.Factories
{
    public class SkillFactory
    {
        public List<Skill> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Skills");
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<Skill> skills = new List<Skill>();

            while (reader.Read())
            {
                Skill skill = new Skill();

                skill.Id = (Guid)reader["Id"];
                skill.MonsterId = (Guid)reader["MonsterId"];
                skill.SkillTypeId = (Guid)reader["SkillTypeId"];
                skill.Save = Convert.ToInt16(reader["Save"]);
                skill.SetInternalState(InternalStates.UnModified, true);

                skills.Add(skill);
            }

            sqliteConnection.Close();
            return skills;
        }

        public List<Skill> GetObjectsByMonsterId(Guid monsterId)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Skills WHERE monsterId = '{0}'", monsterId);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<Skill> skills = new List<Skill>();

            while (reader.Read())
            {
                Skill skill = new Skill();

                skill.Id = (Guid)reader["Id"];
                skill.MonsterId = (Guid)reader["MonsterId"];
                skill.SkillTypeId = (Guid)reader["SkillTypeId"];
                skill.Save = Convert.ToInt16(reader["Save"]);
                skill.SetInternalState(InternalStates.UnModified, true);

                skills.Add(skill);
            }

            sqliteConnection.Close();
            return skills;
        }

        public void SaveObject(Skill skill)
        {
            if (skill.InternalState != InternalStates.UnModified)
            {
                SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
                sqliteConnection.Open();

                String query = String.Empty;
                SQLiteParameter sqliteparameter = new SQLiteParameter();
                List<SQLiteParameter> parameterlist = null;

                switch (skill.InternalState)
                {
                    case InternalStates.New:
                        query = @"  INSERT INTO Skills(Id, MonsterId, SkillTypeId, Save) 
                                VALUES (@Id, @MonsterId, @SkillTypeId, @Save)";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", skill.Id){ DbType = DbType.String },
                            new SQLiteParameter("@MonsterId", skill.MonsterId) { DbType = DbType.String },
                            new SQLiteParameter("@SkillTypeId", skill.SkillTypeId) { DbType = DbType.String },
                            new SQLiteParameter("@Save", skill.Save)
                        };

                        break;

                    case InternalStates.Modified:
                        query = @"  UPDATE Skills
                                SET MonsterId = @MonsterId,
                                    SkillTypeId = @SkillTypeId,
                                    Save = @Save
                                WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", skill.Id){ DbType = DbType.String },
                            new SQLiteParameter("@MonsterId", skill.MonsterId) { DbType = DbType.String },
                            new SQLiteParameter("@SkillTypeId", skill.SkillTypeId) { DbType = DbType.String },
                            new SQLiteParameter("@Save", skill.Save)
                        };

                        break;

                    case InternalStates.Deleted:
                        query = @"  DELETE FROM Skills
                                WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", skill.Id){ DbType = DbType.String }
                        };

                        break;
                }

                SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);
                command.Parameters.AddRange(parameterlist.ToArray());

                command.ExecuteNonQuery();

                skill.SetInternalState(InternalStates.UnModified, true);

                sqliteConnection.Close();
            }
        }
    }
}
