using FluxControlAPI.Models.BusinessRule;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.DataAccessObjects
{
    public class BusDAO : Database, ICrudDAO<Bus>
    {
        /// <summary>
        /// Busca um Ônibus por Placa ou Número
        /// </summary>
        /// <param name="identifier">Placa ou Número do Ônibus</param>
        /// <returns>Ônibus caso exista</returns>
        public Bus GetByIdentifier(string identifier)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;

            cmd.CommandText = @"SELECT * FROM Buses 
                                WHERE LicensePlate = @Identifier OR Number = @Identifier";


            cmd.Parameters.AddWithValue("@Identifier", identifier);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    return new Bus()
                    {
                        Id = (int)reader["Id"],
                        Number = (int)reader["Number"],
                        LicensePlate = (string)reader["LicensePlate"],
                        BusCompany = (int)reader["Company_Id"]
                    };
            }

            return null;
        }

        #region CRUD

        public int Add(Bus model)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;

            cmd.CommandText = @"INSERT INTO Buses (Number, LicensePlate, Company_Id)
                                VALUES (@Number, @LicensePlate, @CompanyId)
                                SELECT CAST(@@IDENTITY AS INT)";


            cmd.Parameters.AddWithValue("@Number", model.Number);
            cmd.Parameters.AddWithValue("@LicensePlate", model.LicensePlate);
            cmd.Parameters.AddWithValue("@CompanyId", model.BusCompany);

            using (var reader = cmd.ExecuteReader())
                if(reader.Read())
                    return reader.GetInt32(0);

            return 0;
        }

        public bool Change(Bus model)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;

            cmd.CommandText = @"UPDATE Buses 
                                SET Number = @Number, 
                                LicensePlate = @LicensePlate,
                                Company_Id = @CompanyId
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Number", model.Number);
            cmd.Parameters.AddWithValue("@LicensePlate", model.LicensePlate);
            cmd.Parameters.AddWithValue("@CompanyId", model.BusCompany);

            return cmd.ExecuteNonQuery() > 0;
        }

        public Bus Get(int id)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;

            cmd.CommandText = @"SELECT * FROM FlowRecords 
                                WHERE Id = @Id";


            cmd.Parameters.AddWithValue("@Id", id);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    return new Bus()
                    {
                        Id = (int)reader["Id"],
                        Number = (int)reader["Number"],
                        LicensePlate = (string)reader["LicensePlate"],
                        BusCompany = (int)reader["Company_Id"]
                    };
            }
                
            return null;
        }

        public List<Bus> Load()
        {
            var cmd = new SqlCommand();
            var models = new List<Bus>();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT * FROM Buses";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    models.Add(
                    new Bus()
                    {
                        Id = (int)reader["Id"],
                        Number = (int)reader["Number"],
                        LicensePlate = (string)reader["LicensePlate"],
                        BusCompany = (int)reader["Company_Id"]
                    });
            }

            return models;
        }

        public bool Remove(int id)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;

            cmd.CommandText = @"REMOVE FROM Buses
                                WHERE Id = @Id";


            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        #endregion
    }
}
