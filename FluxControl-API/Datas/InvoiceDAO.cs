using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.Datas.BusinessRule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.Datas
{
    public class InvoiceDAO : Database, ICrudDAO<Invoice>
    {
        public bool SetInvoiceValue(int invoiceId, decimal totalCost)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE Invoices 
                                SET Total = @Total
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", invoiceId);
            cmd.Parameters.AddWithValue("@Total", totalCost);

            return cmd.ExecuteNonQuery() > 0;
        }

        public List<FlowRecord> GetInvoiceRecords(int invoiceId)
        {
            var models = new List<FlowRecord>();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT record.*, 
                                _user.Id User_Id, _user.Registration, _user.Email, _user.Name, _user.Password, _user.Type,
                                bus.Id Bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id
                                FROM FlowRecords record
                                JOIN Buses bus ON record.Bus_Id = bus.Id
                                JOIN Users _user ON record.User_Id = _user.Id
                                WHERE Invoice_Id = @InvoiceId";


            cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

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
                            Number = (string)reader["Number"],
                            LicensePlate = (string)reader["LicensePlate"],
                            BusCompany = (int)reader["Company_Id"]
                        },
                        Arrival = (DateTime)reader["Arrival"],
                        Departure = reader["Departure"] == DBNull.Value ? null : (DateTime?)reader["Departure"]
                    });

            return models;
        }

        public bool Cancel(int id)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE FlowRecords
                                SET Invoice_Id = NULL
                                WHERE Invoice_Id = @Id;

                                DELETE FROM Invoices 
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        #region CRUD

        public int Add(Invoice model)
        {
             SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"INSERT INTO Invoices (GenerationDate, TaxConsidered, IntervalMinutesConsidered, Total, Company_Id)
                                VALUES (@GenerationDate, @TaxConsidered, @IntervalMinutesConsidered, @Total, @CompanyId)
                                SELECT CAST(@@IDENTITY AS INT)";

            cmd.Parameters.AddWithValue("@GenerationDate", model.GenerationDate);
            cmd.Parameters.AddWithValue("@TaxConsidered", model.TaxConsidered);
            cmd.Parameters.AddWithValue("@IntervalMinutesConsidered", model.IntervalMinutesConsidered);
            cmd.Parameters.AddWithValue("@Total", model.TotalCost);
            cmd.Parameters.AddWithValue("@CompanyId", model.CompanyDebtor);

            using (var reader = cmd.ExecuteReader())
                if (reader.Read())
                    return reader.GetInt32(0);

            return 0;
        }

        public bool Change(Invoice model)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE Invoices 
                                SET GenerationDate = @, TaxConsidered = @TaxConsidered, 
                                IntervalMinutesConsidered = @IntervalMinutesConsidered, Total = @Total, Company_Id = @CompanyId
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@GenerationDate", model.GenerationDate);
            cmd.Parameters.AddWithValue("@TaxConsidered", model.TaxConsidered);
            cmd.Parameters.AddWithValue("@IntervalMinutesConsidered", model.IntervalMinutesConsidered);
            cmd.Parameters.AddWithValue("@Total", model.TotalCost);
            cmd.Parameters.AddWithValue("@CompanyId", model.CompanyDebtor);
            return cmd.ExecuteNonQuery() > 0;
        }

        public Invoice Get(int id)
        {
            Invoice model = null;

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT TOP(1) invoice.*, 
                                record.Id record_Id, record.Arrival, record.Departure, record.Bus_Id, record.User_Id, record.Invoice_Id,
                                _user.Id User_Id, _user.Registration, _user.Email, _user.Name, _user.Type,
                                bus.Id Bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id
                                FROM Invoices invoice
                                JOIN FlowRecords record ON record.Invoice_Id = invoice.Id
                                JOIN Buses bus ON record.Bus_Id = bus.Id
                                JOIN Users _user ON record.User_Id = _user.Id
                                WHERE invoice.Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            using (var reader = cmd.ExecuteReader())
            {
                var dataTable = new DataTable();
                dataTable.Load(reader);

                var records = new List<FlowRecord>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var row = dataTable.Rows[i];

                    records.Add(new FlowRecord()
                    {
                        Id = (int)row["Id"],
                        RegistryClerk = new User()
                        {
                            Id = (int)row["User_Id"],
                            Registration = (int)row["Registration"],
                            Email = (string)row["Email"],
                            Name = (string)row["Name"],
                            Type = (UserType)(short)row["Type"]

                        },
                        BusRegistered = new Bus()
                        {
                            Id = (int)row["Bus_Id"],
                            Number = (string)row["Number"],
                            LicensePlate = (string)row["LicensePlate"],
                            BusCompany = (int)row["Company_Id"]
                        },
                        Arrival = (DateTime)row["Arrival"],
                        Departure = row["Departure"] == DBNull.Value ? null : (DateTime?)row["Departure"]
                    });

                    if (i != dataTable.Rows.Count - 1)
                        continue;

                    else
                    {
                        model = new Invoice()
                        {
                            Id = (int)row["Id"],
                            GenerationDate = (DateTime)row["GenerationDate"],
                            IntervalMinutesConsidered = (int)row["IntervalMinutesConsidered"],
                            TaxConsidered = (decimal)row["TaxConsidered"],
                            CompanyDebtor = (int)row["Company_Id"],
                            Records = records,
                            TotalCost = (decimal)row["Total"]
                        };

                    }

                }
            }

            return model;
        }

        public List<Invoice> Load()
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT invoice.*, 
                                record.Id record_Id, record.Arrival, record.Departure, record.Bus_Id, record.User_Id, record.Invoice_Id,
                                _user.Id User_Id, _user.Registration, _user.Email, _user.Name, _user.Type,
                                bus.Id Bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id
                                FROM Invoices invoice
                                JOIN FlowRecords record ON record.Invoice_Id = invoice.Id
                                JOIN Buses bus ON record.Bus_Id = bus.Id
                                JOIN Users _user ON record.User_Id = _user.Id";

            List<Invoice> models = new List<Invoice>();

            using (var reader = cmd.ExecuteReader())
            {
                var dataTable = new DataTable();
                dataTable.Load(reader);

                var records = new List<FlowRecord>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var row = dataTable.Rows[i];

                    records.Add(new FlowRecord()
                    {
                        Id = (int)row["Id"],
                        RegistryClerk = new User()
                        {
                            Id = (int)row["User_Id"],
                            Registration = (int)row["Registration"],
                            Email = (string)row["Email"],
                            Name = (string)row["Name"],
                            Type = (UserType)(short)row["Type"]

                        },
                        BusRegistered = new Bus()
                        {
                            Id = (int)row["Bus_Id"],
                            Number = (string)row["Number"],
                            LicensePlate = (string)row["LicensePlate"],
                            BusCompany = (int)row["Company_Id"]
                        },
                        Arrival = (DateTime)row["Arrival"],
                        Departure = row["Departure"] == DBNull.Value ? null : (DateTime?)row["Departure"]
                    });

                    if (i != dataTable.Rows.Count - 1)
                        continue;

                    else
                    {
                        models.Add(new Invoice()
                        {
                            Id = (int)row["Id"],
                            GenerationDate = (DateTime)row["GenerationDate"],
                            IntervalMinutesConsidered = (int)row["IntervalMinutesConsidered"],
                            TaxConsidered = (decimal)row["TaxConsidered"],
                            CompanyDebtor = (int)row["Company_Id"],
                            Records = records,
                            TotalCost = (decimal)row["Total"]
                        });

                    }

                }
            }

            return models;
        }

        public bool Remove(int id)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE Invoices 
                                SET Inactive = 1
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

    #endregion

    }
}
