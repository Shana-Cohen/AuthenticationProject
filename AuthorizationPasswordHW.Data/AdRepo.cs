using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationPasswordHW.Data
{
    public class AdRepo
    {
        private string _connectionString;

        public AdRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddUser(User user, string password)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users(Name, Email, HashPassword) " +
                              "VALUES(@name, @email, @hashPassword)" +
                              "SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hashPassword", BCrypt.Net.BCrypt.HashPassword(password));
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            return BCrypt.Net.BCrypt.Verify(password, user.HashedPassword) ? user : null;
        }


        public User GetByEmail(string email)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                Name = (string)reader["Name"],
                HashedPassword = (string)reader["HashPassword"]
            };
        }

        public void AddAd(Ad ad)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Ads (Title, Description, PhoneNumber, UserId, Name, DateSubmitted)
                                    Values (@details, @number, @userId, @name, @date)";
            command.Parameters.AddWithValue("@title", ad.Title);
            command.Parameters.AddWithValue("@description", ad.Description);
            command.Parameters.AddWithValue("@number", ad.PhoneNumber);
            command.Parameters.AddWithValue("@userId", ad.UserId);
            command.Parameters.AddWithValue("@name", ad.Name);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public List<Ad> GetAllAds()
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads a " +
                                " JOIN Users u " +
                                " ON a.UserId = u.Id";
            var ads = new List<Ad>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    UserId = (int)reader["UserId"],
                    DateSubmitted = (DateTime)reader["DateSubmitted"],
                    Name = (string)reader["Name"]
                });
            }
            connection.Close();
            return ads;
        }

        public List<Ad> GetAdsById(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads a " +
                "JOIN Users u " +
                "ON a.UserId = u.Id " +
                "WHERE u.Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            var ads = new List<Ad>();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Description = (string)reader["Description"],
                    DateSubmitted = (DateTime)reader["DateSubmitted"],
                    Name = (string)reader["Name"]
                });

            }
            return ads;
        }
        public void DeleteAd(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}

