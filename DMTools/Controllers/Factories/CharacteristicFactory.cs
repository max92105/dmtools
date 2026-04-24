using Data.Constant;
using Data.Constants;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Controllers.Factories
{
    public class CharacteristicFactory
    {
        public List<Characteristic> GetObjects()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Characteristics");
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<Characteristic> characteristics = new List<Characteristic>();

            while (reader.Read())
            {
                Characteristic characteristic = new Characteristic();

                characteristic.Id = (Guid)reader["Id"];
                characteristic.MonsterId = (Guid)reader["MonsterId"];
                characteristic.CharacteristicTypeId = (Guid)reader["CharacteristicTypeId"];
                characteristic.Score = Convert.ToInt16(reader["Score"]);
                characteristic.Save = Convert.ToInt16(reader["Save"]);
                characteristic.SetInternalState(InternalStates.UnModified, true);

                characteristics.Add(characteristic);
            }

            sqliteConnection.Close();
            return characteristics;
        }

        public List<Characteristic> GetObjectsByMonsterId(Guid monsterId)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Characteristics WHERE Characteristics.MonsterId = '{0}'", monsterId);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            List<Characteristic> characteristics = new List<Characteristic>();

            while (reader.Read())
            {
                Characteristic characteristic = new Characteristic();

                characteristic.Id = (Guid)reader["Id"];
                characteristic.MonsterId = (Guid)reader["MonsterId"];
                characteristic.CharacteristicTypeId = (Guid)reader["CharacteristicTypeId"];
                characteristic.Score = Convert.ToInt16(reader["Score"]);
                characteristic.Save = Convert.ToInt16(reader["Save"]);
                characteristic.SetInternalState(InternalStates.UnModified, true);

                characteristics.Add(characteristic);
            }

            sqliteConnection.Close();
            return characteristics;
        }

        public Characteristic GetObjectByCharacterisitcTypeIdAndMonsterId(Guid characterisitcTypeId, Guid monsterId)
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
            sqliteConnection.Open();

            String query = String.Format("SELECT * FROM Characteristics WHERE CharacteristicTypeId = '{0}' AND MonsterId = '{1}'", characterisitcTypeId, monsterId);
            SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);

            SQLiteDataReader reader = command.ExecuteReader();
            Characteristic characteristic = new Characteristic();

            while (reader.Read())
            {
                characteristic.Id = (Guid)reader["Id"];
                characteristic.MonsterId = (Guid)reader["MonsterId"];
                characteristic.CharacteristicTypeId = (Guid)reader["CharacteristicTypeId"];
                characteristic.Score = Convert.ToInt16(reader["Score"]);
                characteristic.Save = Convert.ToInt16(reader["Save"]);
                characteristic.SetInternalState(InternalStates.UnModified, true);
            }

            sqliteConnection.Close();
            return characteristic;
        }

        public void SaveObject(Characteristic characteristic)
        {
            if (characteristic.InternalState != InternalStates.UnModified)
            {
                SQLiteConnection sqliteConnection = new SQLiteConnection(DatabaseInfo.ConnectionString);
                sqliteConnection.Open();

                String query = String.Empty;

                List<SQLiteParameter> parameterlist = null;

                switch (characteristic.InternalState)
                {
                    case InternalStates.New:
                        query = @"  INSERT INTO Characteristics(Id, MonsterId, CharacteristicTypeId, Score, Save) 
                                VALUES (@Id, @MonsterId, @CharacteristicTypeId, @Score, @Save)";

                        parameterlist = new List<SQLiteParameter>()
                    {
                        new SQLiteParameter("@Id", characteristic.Id) { DbType = DbType.String },
                        new SQLiteParameter("@MonsterId", characteristic.MonsterId) { DbType = DbType.String },
                        new SQLiteParameter("@CharacteristicTypeId", characteristic.CharacteristicTypeId) { DbType = DbType.String },
                        new SQLiteParameter("@Score", characteristic.Score),
                        new SQLiteParameter("@Save", characteristic.Save)
                    };
                        break;

                    case InternalStates.Modified:
                        query = @"  UPDATE Characteristics
                                SET MonsterId = @MonsterId,
                                    CharacteristicTypeId = @CharacteristicTypeId,
                                    Score = @Score,
                                    Save = @Save
                                WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                    {
                        new SQLiteParameter("@Id", characteristic.Id) { DbType = DbType.String },
                        new SQLiteParameter("@MonsterId", characteristic.MonsterId) { DbType = DbType.String },
                        new SQLiteParameter("@CharacteristicTypeId", characteristic.CharacteristicTypeId) { DbType = DbType.String },
                        new SQLiteParameter("@Score", characteristic.Score),
                        new SQLiteParameter("@Save", characteristic.Save)
                    };
                        break;

                    case InternalStates.Deleted:
                        query = @"  DELETE FROM Characteristics
                                WHERE Id = @Id";

                        parameterlist = new List<SQLiteParameter>()
                    {
                        new SQLiteParameter("@Id", characteristic.Id) { DbType = DbType.String }
                    };
                        break;
                }


                SQLiteCommand command = new SQLiteCommand(query, sqliteConnection);
                command.Parameters.AddRange(parameterlist.ToArray());

                command.ExecuteNonQuery();

                characteristic.SetInternalState(InternalStates.UnModified, true);

                sqliteConnection.Close();
            }
        }
    }
}
