using Database;
using Microsoft.Data.SqlClient;
using RestApi.Interfaces.Authentication;
using RestApi.Models;
using System.Data;

namespace RestApi.Gateways.Authentication
{
    public class GetUserByEmailGateway : IGetUserByEmailGateway
    {
        public User GetUserByEmail(string email)
        {
            string query = @"exec GetUserByEmail_SP @Email";

            DataTable table = new DataTable();
            string sqlDataSource = Config.Get("ConnectionStrings:Connection");
            SqlDataReader myReader;
            using (SqlConnection connection = new SqlConnection(sqlDataSource))
            {
                connection.Open();
                using (SqlCommand myCommand = new SqlCommand(query, connection))
                {
                    myCommand.Parameters.AddWithValue("@Email", email);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    connection.Close();
                }
            }

            if (table.Rows.Count == 0)
                return null;
            else
            {
                return new User
                {
                    Id = Int32.Parse(table.Rows[0][0].ToString()),
                    Firstname = table.Rows[0][1].ToString(),
                    Lastname = table.Rows[0][2].ToString(),
                    Birthdate = Int64.Parse(table.Rows[0][3].ToString()),
                    Email = table.Rows[0][4].ToString(),
                    HashedPassword = table.Rows[0][5].ToString(),
                    CreatedOn = Int64.Parse(table.Rows[0][6].ToString()),
                    PhotoFileName = table.Rows[0][7].ToString()
                };
            }
        }
    }
}