using FluxControlAPI.Models.BusinessRule;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.Datas
{
    public class RulesDAO : Database
    {
        public Rules Get()
        {
            SqlCommand cmd = new SqlCommand();
            Rules rules = null;

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT TOP(1) * FROM ProviderRules";

            using (SqlDataReader reader = cmd.ExecuteReader())
                if (reader.Read())
                    rules = new Rules()
                    {
                        Id = (int)reader["Id"],
                        Tax = (decimal)reader["Tax"],
                        IntervalMinutes = (int)reader["IntervalMinutes"]
                    };

            return rules;
        }

        public bool Update(Rules model)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE ProviderRules
                                SET Tax = @Tax,
                                IntervalMinutes = @IntervalMinutes
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Tax", model.Tax);
            cmd.Parameters.AddWithValue("@IntervalMinutes", model.IntervalMinutes);

            return cmd.ExecuteNonQuery() > 0;
                    
        }
    }
}
