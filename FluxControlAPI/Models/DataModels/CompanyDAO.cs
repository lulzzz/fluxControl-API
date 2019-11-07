using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.DataModels.BusinessRule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.DataModels
{
    public class CompanyDAO : Database, ICrudDAO<Company>
    {
        public int Add(Company model)
        {
            var cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"INSERT INTO Companies (Name, Thumbnail, InvoiceInterval) 
                                VALUES (@Name, @Thumbnail, @InvoiceInterval)
                                SELECT CAST(@@IDENTITY AS INT)";

            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Thumbnail", model.Thumbnail);
            cmd.Parameters.AddWithValue("@InvoiceInterval", model.InvoiceInterval);

            using (var reader = cmd.ExecuteReader())
                if (reader.Read())
                    return reader.GetInt32(0);

            return 0;
        }

        public bool Change(int id, Company model)
        {
            var cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE Companies
                                SET Name = @Name, Thumbnail = @Registration, Invoice_Interval = @InvoiceInterval
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Thumbnail", model.Thumbnail);
            cmd.Parameters.AddWithValue("@InvoiceInterval", model.InvoiceInterval);

            return cmd.ExecuteNonQuery() > 0;
        }

        public Company Get(int id)
        {
            var cmd = new SqlCommand();
            var model = new Company();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT company.*, bus.Id Bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id
                                FROM Companies company
                                JOIN Buses bus ON bus.Company_Id = company.Id
                                WHERE company.Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            using (var reader = cmd.ExecuteReader())
            {
                var dataTable = new DataTable();
                dataTable.Load(reader);

                var fleet = new List<Bus>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    fleet.Add(
                    new Bus()
                    {
                        Id = (int)dataTable.Rows[i]["Bus_Id"],
                        Number = (int)dataTable.Rows[i]["Number"],
                        LicensePlate = (string)dataTable.Rows[i]["LicensePlate"],
                        BusCompany = (int)dataTable.Rows[i]["Company_Id"]

                    });

                    if (i != dataTable.Rows.Count - 1)
                        continue;

                    else
                    {
                        model = new Company()
                        {
                            Id = (int)dataTable.Rows[i]["Id"],
                            Name = (string)dataTable.Rows[i]["Name"],
                            Thumbnail = (string)dataTable.Rows[i]["Thumbnail"],
                            InvoiceInterval = (short)dataTable.Rows[i]["Invoice_Interval"],
                            Fleet = fleet
                        };

                    }

                }

            }
                
            return model;
        }

        public List<Company> Load()
        {
            var cmd = new SqlCommand();
            var models = new List<Company>();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT company.*, bus.Id Bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id
                                FROM Companies company
                                JOIN Buses bus ON bus.Company_Id = company.Id
                                ORDER BY Company_Id";

            using (var reader = cmd.ExecuteReader())
            {
                var dataTable = new DataTable();
                dataTable.Load(reader);

                var fleet = new List<Bus>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {

                    fleet.Add(
                    new Bus()
                    {
                        Id = (int)dataTable.Rows[i]["Bus_Id"],
                        Number = (int)dataTable.Rows[i]["Number"],
                        LicensePlate = (string)dataTable.Rows[i]["LicensePlate"],
                        BusCompany = (int)dataTable.Rows[i]["Company_Id"]
                    });

                    if (i != dataTable.Rows.Count - 1 && (int)dataTable.Rows[i]["Id"] == (int)dataTable.Rows[i + 1]["Id"])
                        continue;
                    
                    else
                    {
                        models.Add(
                        new Company()
                        {
                            Id = (int)dataTable.Rows[i]["Id"],
                            Name = (string)dataTable.Rows[i]["Name"],
                            Thumbnail = (string)dataTable.Rows[i]["Thumbnail"],
                            InvoiceInterval = (short)dataTable.Rows[i]["Invoice_Interval"],
                            Fleet = fleet
                        });

                        fleet = new List<Bus>();
                    }
                }
            }

            return models;
        }

        public bool Remove(int id)
        {
            var cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"DELETE FROM Buses WHERE Company_Id = @Id
                                DELETE FROM Companies WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
