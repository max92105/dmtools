using Data.Constant;
using Data.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Controllers.Factories
{
    public class ActionFactory
    {
        public Data.Objects.Action GetObject(Guid Id)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Actions WHERE Id = '{0}'", Id);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            Data.Objects.Action action = new Data.Objects.Action();

            while (reader.Read())
            {
                action.Id = (Guid)reader["Id"];
                action.MonsterId = (Guid)reader["MonsterId"];
                action.Name = reader["Name"].ToString();
                action.Description = reader["Description"].ToString();

                if (reader["AttackBonus"] != DBNull.Value)
                    action.AttackBonus = Convert.ToInt16(reader["AttackBonus"]);

                if (reader["DamageBonus"] != DBNull.Value)
                    action.DamageBonus = Convert.ToInt16(reader["DamageBonus"]);

                if (reader["DamageDice"] != DBNull.Value)
                    action.DamageDice = reader["DamageDice"].ToString();

                if (reader["IsLegendary"] != DBNull.Value)
                    action.IsLegendary = Convert.ToBoolean(reader["IsLegendary"]);

                if (reader["IsBonus"] != DBNull.Value)
                    action.IsBonus = Convert.ToBoolean(reader["IsBonus"]);

                if (reader["IsReaction"] != DBNull.Value)
                    action.IsReaction = Convert.ToBoolean(reader["IsReaction"]);

                action.SetInternalState(InternalStates.UnModified, true);
            }

            sqliteConnection.Close();
            return action;
        }

        public List<Data.Objects.Action> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Actions");
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<Data.Objects.Action> actions = new List<Data.Objects.Action>();

            while (reader.Read())
            {
                Data.Objects.Action action = new Data.Objects.Action();

                action.Id = (Guid)reader["Id"];
                action.MonsterId = (Guid)reader["MonsterId"];
                action.Name = reader["Name"].ToString();
                action.Description = reader["Description"].ToString();

                if (reader["AttackBonus"] != DBNull.Value)
                    action.AttackBonus = Convert.ToInt16(reader["AttackBonus"]);

                if (reader["DamageBonus"] != DBNull.Value)
                    action.DamageBonus = Convert.ToInt16(reader["DamageBonus"]);

                if (reader["DamageDice"] != DBNull.Value)
                    action.DamageDice = reader["DamageDice"].ToString();

                if (reader["IsLegendary"] != DBNull.Value)
                    action.IsLegendary = Convert.ToBoolean(reader["IsLegendary"]);

                if (reader["IsBonus"] != DBNull.Value)
                    action.IsBonus = Convert.ToBoolean(reader["IsBonus"]);

                if (reader["IsReaction"] != DBNull.Value)
                    action.IsReaction = Convert.ToBoolean(reader["IsReaction"]);

                action.SetInternalState(InternalStates.UnModified, true);
                actions.Add(action);
            }

            sqliteConnection.Close();
            return actions;
        }

        public List<Data.Objects.Action> GetObjectsMonsterId(Guid monsterId)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Actions WHERE MonsterId = '{0}'", monsterId);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<Data.Objects.Action> actions = new List<Data.Objects.Action>();

            while (reader.Read())
            {
                Data.Objects.Action action = new Data.Objects.Action();

                action.Id = (Guid)reader["Id"];
                action.MonsterId = (Guid)reader["MonsterId"];
                action.Name = reader["Name"].ToString();
                action.Description = reader["Description"].ToString();

                if (reader["AttackBonus"] != DBNull.Value)
                    action.AttackBonus = Convert.ToInt16(reader["AttackBonus"]);

                if (reader["DamageBonus"] != DBNull.Value)
                    action.DamageBonus = Convert.ToInt16(reader["DamageBonus"]);

                if (reader["DamageDice"] != DBNull.Value)
                    action.DamageDice = reader["DamageDice"].ToString();

                if (reader["IsLegendary"] != DBNull.Value)
                    action.IsLegendary = Convert.ToBoolean(reader["IsLegendary"]);

                if (reader["IsBonus"] != DBNull.Value)
                    action.IsBonus = Convert.ToBoolean(reader["IsBonus"]);

                if (reader["IsReaction"] != DBNull.Value)
                    action.IsReaction = Convert.ToBoolean(reader["IsReaction"]);

                action.SetInternalState(InternalStates.UnModified, true);
                actions.Add(action);
            }


            sqliteConnection.Close();
            return actions;
        }

        public void SaveObject(Data.Objects.Action action)
        {
            if (action.InternalState != InternalStates.UnModified)
            {
                SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
                sqliteConnection.Open();

                String query = String.Empty;
                SQLiteParameter sqliteparameter = new SQLiteParameter();
                List<SQLiteParameter> parameterlist = null;

                switch (action.InternalState)
                {
                    case InternalStates.New:
                        query = @"  INSERT INTO Actions(Id, MonsterId, Name, Description, AttackBonus, DamageDice, DamageBonus, IsLegendary, IsBonus, IsReaction) 
                                VALUES (@Id, @MonsterId, @Name, @Description, @AttackBonus, @DamageDice, @DamageBonus, @IsLegendary, @IsBonus, @IsReaction)";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", action.Id) { DbType = DbType.String },
                            new SQLiteParameter("@MonsterId", action.MonsterId) { DbType = DbType.String },
                            new SQLiteParameter("@Name", action.Name),
                            new SQLiteParameter("@Description", action.Description),
                            new SQLiteParameter("@AttackBonus", action.AttackBonus),
                            new SQLiteParameter("@DamageDice", action.DamageDice),
                            new SQLiteParameter("@DamageBonus", action.DamageBonus),
                            new SQLiteParameter("@IsLegendary", action.IsLegendary),
                            new SQLiteParameter("@IsBonus", action.IsBonus),
                            new SQLiteParameter("@IsReaction", action.IsReaction)
                        };

                        break;

                    case InternalStates.Modified:
                        query = @"  UPDATE Actions
                                SET MonsterId = @MonsterId,
                                    Name = @Name,
                                    Description = @Description,
                                    AttackBonus = @AttackBonus,
                                    DamageDice = @DamageDice,
                                    DamageBonus = @DamageBonus,
                                    IsLegendary = @IsLegendary,
                                    IsBonus = @IsBonus,
                                    IsReaction = @IsReaction
                                WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", action.Id) { DbType = DbType.String },
                            new SQLiteParameter("@MonsterId", action.MonsterId) { DbType = DbType.String },
                            new SQLiteParameter("@Name", action.Name),
                            new SQLiteParameter("@Description", action.Description),
                            new SQLiteParameter("@AttackBonus", action.AttackBonus),
                            new SQLiteParameter("@DamageDice", action.DamageDice),
                            new SQLiteParameter("@DamageBonus", action.DamageBonus),
                            new SQLiteParameter("@IsLegendary", action.IsLegendary),
                            new SQLiteParameter("@IsBonus", action.IsBonus),
                            new SQLiteParameter("@IsReaction", action.IsReaction)
                        };

                        break;

                    case InternalStates.Deleted:
                        query = @"  DELETE FROM Actions WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                        {
                            new SQLiteParameter("@Id", action.Id) { DbType = DbType.String }
                        };
                        break;
                }

                SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);
                command.Parameters.AddRange(parameterlist.ToArray());

                command.ExecuteNonQuery();

                action.SetInternalState(InternalStates.UnModified, true);

                sqliteConnection.Close();
            }
        }
    }
}
