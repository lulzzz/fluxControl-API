using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FluxControlAPI.Models.DataModels
{
    public abstract class Database : IDisposable
    {
        protected readonly SqlConnection connection;
        protected readonly string connectionString;

        protected Database()
        {
            connectionString = "Server=LAPTOP-1I08H1PB\\SQLEXPRESS;Database=FluxControl;Integrated Security=SSPI;";

            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
        }

    }
}
