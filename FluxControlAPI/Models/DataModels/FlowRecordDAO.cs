using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.DataModels.BusinessRule;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.DataModels
{
    public class FlowRecordDAO : Database
    {

        /// <summary>
        /// Faz um novo registro de fluxo no sistema por Placa ou Número do Ônibus.
        /// </summary>
        /// <param name="identifier">Placa ou Número do Ônibus</param>
        /// <param name="user">Usuário que está efetuando o registro</param>
        /// <returns>Retorna o Id do registro cadastrado no banco, ou zero caso não for registrado.</returns>
        public int Register(string identifier, User user)
        {
            Bus busRegistered = null;

            using (BusDAO busDAO = new BusDAO())
            {
                busRegistered = busDAO.GetByIdentifier(identifier);

                if (busRegistered != null)
                {
                    var register = new FlowRecord()
                    {
                        RegistryClerk = user,
                        BusRegistered = busRegistered
                    };

                    if (busDAO.IsOnPlatform(busRegistered))
                        register.Departure = DateTime.Now;

                    else
                        register.Arrival = DateTime.Now;

                    return this.Add(register);
                }
            }
                
            return 0;
        }

        #region CRUD

        public int Add(FlowRecord model)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"INSERT INTO FlowRecords (User_Id, Bus_Id, Arrival, Departure)
                                VALUES (@UserId, @BusId, @Arrival, @Departure)
                                SELECT CAST(@@IDENTITY AS INT)";

            cmd.Parameters.AddWithValue("@UserId", model.RegistryClerk);
            cmd.Parameters.AddWithValue("@BusId", model.BusRegistered);
            cmd.Parameters.AddWithValue("@Arrival", model.Arrival);
            cmd.Parameters.AddWithValue("@Departure", model.Departure);

            using (var reader = cmd.ExecuteReader())
                if (reader.Read())
                    return reader.GetInt32(0);

            return 0;
        }

        public bool Change(int id, FlowRecord model)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE FlowRecords 
                                SET 
                                WHERE ";

            cmd.Parameters.AddWithValue("", id);
            cmd.Parameters.AddWithValue("", model.RegistryClerk);
            cmd.Parameters.AddWithValue("", model.BusRegistered);

            return cmd.ExecuteNonQuery() > 0;
        }

        public FlowRecord Get(int id)
        {
            SqlCommand cmd = new SqlCommand();
            
            cmd.Connection = connection;
            cmd.CommandText = @"SELECT * FROM FlowRecords 
                                WHERE ";

            cmd.Parameters.AddWithValue("", id);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return new FlowRecord()
                {
                
                };

            return null;
        }

        public List<FlowRecord> Load()
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT * FROM FlowRecords";

            List<FlowRecord> flowRecords = new List<FlowRecord>();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                flowRecords.Add
                (   
                    new FlowRecord()
                    {

                    }
                );

            return flowRecords;
        }

        public bool Remove(int id)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"DELETE FROM FlowRecords 
                                WHERE ";

            cmd.Parameters.AddWithValue("", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        #endregion
    }
}
