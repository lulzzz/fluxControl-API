using FluxControlAPI.Models.BusinessRule;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.Datas
{
    public class ProviderDAO : Database
    {
        public Rules GetRules()
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
                        Interval = (TimeSpan)reader["InteralConsidered"]
                    };

            return rules;
        }               
    }
}
