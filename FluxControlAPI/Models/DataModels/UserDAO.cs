using FluxControlAPI.Models.DataModels.BusinessRule;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.DataModels
{
    public class UserDAO : Database, ICrudDAO<User>
    {
        public User Login(int registration, string password)
        {
            var cmd = new SqlCommand();
            User model = null;

            cmd.Connection = connection;
            cmd.CommandText = @"SELECT * FROM Users
                                WHERE Registration = @Registration AND Password = @Password";

            cmd.Parameters.AddWithValue("@Registration", registration);
            cmd.Parameters.AddWithValue("@Password", password);

            using (var reader = cmd.ExecuteReader())
                if (reader.Read())
                    model = new User()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Registration = (int)reader["Registration"],
                        Email = (string)reader["Email"],
                        Type = (UserType)(short)reader["Type"]
                    };

            return model;

        }

        public User GetByEmail(string email)
        {
            var cmd = new SqlCommand();
            User model = null;

            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM Users WHERE Email = @Email";

            cmd.Parameters.AddWithValue("@Email", email);

            using (var reader = cmd.ExecuteReader())
                if (reader.Read())
                    model = new User()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Registration = (int)reader["Registration"],
                        Email = (string)reader["Email"],
                        Type = (UserType)(short)reader["Type"]
                    };

            return model;
        }

        public bool SetPassword(int id, string password)
        {
            var cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE Users
                                SET Password = @Password
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Password", password);

            return cmd.ExecuteNonQuery() > 0;
        }

        #region CRUD

        public int Add(User model)
        {
            var cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"INSERT INTO Users (Name, Registration, Email, Type) 
                                VALUES (@Name, @Registration, @Email, @Type)
                                SELECT CAST(@@IDENTITY AS INT)";

            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Registration", model.Registration);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Type", model.Type);

            using (var reader = cmd.ExecuteReader())
                if (reader.Read())
                    return reader.GetInt32(0);

            return 0;

        }

        public bool Change(int id, User model)
        {
            var cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"UPDATE Users
                                SET Name = @Name, Registration = @Registration, Email = @Email, Type = @Type
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Registration", model.Registration);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Type", model.Type);

            return cmd.ExecuteNonQuery() > 0;
        }

        public User Get(int id)
        {
            var cmd = new SqlCommand();
            User model = null;

            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM Users WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            using (var reader = cmd.ExecuteReader())
                if (reader.Read())
                    model = new User()
                    {
                        Id = (int) reader["Id"],
                        Name = (string) reader["Name"],
                        Registration = (int) reader["Registration"],
                        Email = (string) reader["Email"],
                        Type = (UserType)(short) reader["Type"]
                    };

            return model;
        }

        public List<User> Load()
        {
            var cmd = new SqlCommand();
            List<User> models = new List<User>();
            

            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM Users";

            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    models.Add(
                        new User()
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Registration = (int)reader["Registration"],
                            Email = (string)reader["Email"],
                            Type = (UserType)(short) reader["Type"]
                        }
                    );

            return models;
        }

        public bool Remove(int id)
        {
            var cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = @"DELETE FROM Tokens WHERE User_Id = @Id
                                DELETE FROM Users WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        #endregion
    }
}
