using FluxControlAPI.Models.BusinessRule;
using System;
using System.Collections.Generic;
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

        #region CRUD

        public int Add(Invoice model)
        {
             SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"INSERT INTO Invoices (GenerationDate, TaxConsidered, IntervalConsidered, Total, Company_Id)
                                VALUES (@GenerationDate, @TaxConsidered, @IntervalConsidered, @Total, @CompanyId)
                                SELECT CAST(@@IDENTITY AS INT)";

            cmd.Parameters.AddWithValue("@GenerationDate", model.GenerationDate);
            cmd.Parameters.AddWithValue("@TaxConsidered", model.TaxConsidered);
            cmd.Parameters.AddWithValue("@IntervalConsidered", model.IntervalMinutesConsidered);
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
                                IntervalConsidered = @IntervalConsidered, Total = @Total, Company_Id = @CompanyId
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@GenerationDate", model.GenerationDate);
            cmd.Parameters.AddWithValue("@TaxConsidered", model.TaxConsidered);
            cmd.Parameters.AddWithValue("@IntervalConsidered", model.IntervalMinutesConsidered);
            cmd.Parameters.AddWithValue("@Total", model.TotalCost);
            cmd.Parameters.AddWithValue("@CompanyId", model.CompanyDebtor);
            return cmd.ExecuteNonQuery() > 0;
        }

        public Invoice Get(int id)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT TOP(1) * FROM Invoices WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return new Invoice()
                {
                    Id = (int)reader["Id"],
                    GenerationDate = (DateTime)reader["GenerationDate"],
                    IntervalMinutesConsidered = (int)reader["IntervalConsidered"],
                    TaxConsidered = (decimal)reader["TaxConsidered"],
                    CompanyDebtor = (int)reader["Company_Id"],
                    TotalCost = (decimal)reader["Total"]
                };

            return null;
        }

        public List<Invoice> Load()
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT * FROM Invoices";

            List<Invoice> models = new List<Invoice>();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                models.Add
                (
                    new Invoice()
                    {
                        Id = (int)reader["Id"],
                        GenerationDate = (DateTime)reader["GenerationDate"],
                        IntervalMinutesConsidered = (int)reader["IntervalConsidered"],
                        TaxConsidered = (decimal)reader["TaxConsidered"],
                        CompanyDebtor = (int)reader["Company_Id"],
                        TotalCost = (decimal)reader["Total"]
                    }
                );

            return models;
        }

        public bool Remove(int id)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"DELETE FROM Invoices 
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

    #endregion

    }
}
