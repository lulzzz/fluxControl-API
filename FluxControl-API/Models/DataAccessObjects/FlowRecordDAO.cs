using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.DataAccessObjects.BusinessRule;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.DataAccessObjects
{
    public class FlowRecordDAO : Database, ICrudDAO<FlowRecord>
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

                    var recordedFlow = this.OnPlatform(busRegistered);

                    if (recordedFlow != null)
                    {
                        recordedFlow.Departure = DateTime.Now;
                        this.RegisterDeparture(recordedFlow);

                        return recordedFlow.Id;
                    }

                    else
                    {
                        register.Arrival = DateTime.Now;
                        return this.Add(register);
                    }
                }
            }
                
            return 0;
        }

        /// <summary>
        /// Verifica se o ônibus esta dentro do terminal no momento da verificação.
        /// </summary>
        /// <param name="bus">Ônibus a ser verificado</param>
        /// <returns>Se esta ou não dentro do terminal</returns>
        public FlowRecord OnPlatform(Bus bus)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;

            cmd.CommandText = @"SELECT record.*, 
                                bus.Id bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id,
                                _user.Id user_Id, _user.Registration, _user.Email, _user.Name, _user.Password, _user.Type
                                FROM FlowRecords record
                                JOIN Buses bus ON record.Bus_Id = bus.Id
                                JOIN Users _user ON record.User_Id = _user.Id
                                WHERE record.Departure IS NULL AND
                                bus.Id = @Bus_Id";

            cmd.Parameters.AddWithValue("@Bus_Id", bus.Id);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new FlowRecord()
                    {
                        Id = (int)reader["Id"],
                        RegistryClerk = new User()
                        {
                            Id = (int)reader["User_Id"],
                            Registration = (int)reader["Registration"],
                            Email = (string)reader["Email"],
                            Name = (string)reader["Name"],
                            Type = (UserType)(short)reader["Type"]

                        },
                        BusRegistered = new Bus()
                        {
                            Id = (int)reader["Bus_Id"],
                            Number = (int)reader["Number"],
                            LicensePlate = (string)reader["LicensePlate"],
                            BusCompany = (int)reader["Company_Id"]
                        },
                        Arrival = (DateTime)reader["Arrival"],
                        Departure = reader["Departure"] == DBNull.Value ? null : (DateTime?)reader["Departure"]
                    };
                }
            }

            return null;
        }

        public bool RegisterDeparture(FlowRecord model)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE FlowRecords 
                                SET Departure = @Departure
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Departure", model.Departure);

            return cmd.ExecuteNonQuery() > 0;
        }

        public List<FlowRecord> ListCompanyRecords(int companyId, DateTime startInterval, DateTime endInterval)
        {

            var models = new List<FlowRecord>();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT record.*, 
                                user.Id User_Id, user.Registration, user.Email, user.Name, user.Password, user.Type
                                bus.Id Bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id, 
                                FROM FlowRecords record
                                JOIN Buses bus ON record.Bus_Id = bus.Id
                                JOIN Users user ON record.User_Id = user.Id
                                WHERE Departure >= @StartInterval AND Departure <= @EndInterval AND Departure IS NOT NULL 
                                AND bus.Company_Id = @CompanyId";

            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@StartInterval", startInterval);
            cmd.Parameters.AddWithValue("@EndInterval", endInterval);

            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    models.Add(new FlowRecord()
                    {
                        Id = (int)reader["Id"],
                        RegistryClerk = new User()
                        {
                            Id = (int)reader["User_Id"],
                            Registration = (int)reader["Registration"],
                            Email = (string)reader["Email"],
                            Name = (string)reader["Name"],
                            Type = (UserType)(short)reader["Type"]

                        },
                        BusRegistered = new Bus()
                        {
                            Id = (int)reader["Bus_Id"],
                            Number = (int)reader["Number"],
                            LicensePlate = (string)reader["LicensePlate"],
                            BusCompany = (int)reader["Company_Id"]
                        },
                        Arrival = (DateTime)reader["Arrival"],
                        Departure = reader["Departure"] == DBNull.Value ? null : (DateTime?)reader["Departure"]
                    });

            return models;
        }

        public bool MarkCharge(int recordId, int companyId)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE FlowRecords 
                                SET Company_Id = @CompanyId
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", recordId);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);

            return cmd.ExecuteNonQuery() > 0;
        }

        #region CRUD

        public int Add(FlowRecord model)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"INSERT INTO FlowRecords (User_Id, Bus_Id, Arrival, Departure)
                                VALUES (@UserId, @BusId, @Arrival, @Departure)
                                SELECT CAST(@@IDENTITY AS INT)";

            cmd.Parameters.AddWithValue("@UserId", model.RegistryClerk.Id);
            cmd.Parameters.AddWithValue("@BusId", model.BusRegistered.Id);
            cmd.Parameters.AddWithValue("@Arrival", model.Arrival);
            cmd.Parameters.AddWithValue("@Departure", (model.Departure == null) ? DBNull.Value : (object) model.Departure);

            using (var reader = cmd.ExecuteReader())
                if (reader.Read())
                    return reader.GetInt32(0);

            return 0;
        }

        public bool Change(FlowRecord model)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE FlowRecords 
                                SET 
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("", model.RegistryClerk);
            cmd.Parameters.AddWithValue("", model.BusRegistered);

            return cmd.ExecuteNonQuery() > 0;
        }

        public FlowRecord Get(int id)
        {
            SqlCommand cmd = new SqlCommand();
            
            cmd.Connection = connection;
            cmd.CommandText = @"SELECT record.*, 
                                user.Id User_Id, user.Registration, user.Email, user.Name, user.Password, user.Type
                                bus.Id Bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id, 
                                FROM FlowRecords record
                                JOIN Buses bus ON record.Bus_Id = bus.Id
                                JOIN Users user ON record.User_Id = user.Id
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return new FlowRecord()
                {
                    Id = (int)reader["Id"],
                    RegistryClerk = new User()
                    {
                        Id = (int)reader["User_Id"],
                        Registration = (int)reader["Registration"],
                        Email = (string)reader["Email"],
                        Name = (string)reader["Name"],
                        Type = (UserType)(short)reader["Type"]

                    },
                    BusRegistered = new Bus()
                    {
                        Id = (int)reader["Bus_Id"],
                        Number = (int)reader["Number"],
                        LicensePlate = (string)reader["LicensePlate"],
                        BusCompany = (int)reader["Company_Id"]
                    },
                    Arrival = (DateTime)reader["Arrival"],
                    Departure = reader["Departure"] == DBNull.Value ? null : (DateTime?)reader["Departure"]
                };

            return null;
        }

        public List<FlowRecord> Load()
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"ELECT record.*, 
                                user.Id User_Id, user.Registration, user.Email, user.Name, user.Password, user.Type
                                bus.Id Bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id, 
                                FROM FlowRecords record
                                JOIN Buses bus ON record.Bus_Id = bus.Id
                                JOIN Users user ON record.User_Id = user.Id";

            List<FlowRecord> models = new List<FlowRecord>();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                models.Add
                (   
                    new FlowRecord()
                    {
                        Id = (int)reader["Id"],
                        RegistryClerk = new User()
                        {
                            Id = (int)reader["User_Id"],
                            Registration = (int)reader["Registration"],
                            Email = (string)reader["Email"],
                            Name = (string)reader["Name"],
                            Type = (UserType)(short)reader["Type"]

                        },
                        BusRegistered = new Bus()
                        {
                            Id = (int)reader["Bus_Id"],
                            Number = (int)reader["Number"],
                            LicensePlate = (string)reader["LicensePlate"],
                            BusCompany = (int)reader["Company_Id"]
                        },
                        Arrival = (DateTime)reader["Arrival"],
                        Departure = reader["Departure"] == DBNull.Value ? null : (DateTime?)reader["Departure"]
                    }
                );

            return models;
        }

        public bool Remove(int id)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"DELETE FROM FlowRecords 
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        #endregion
    }
}
