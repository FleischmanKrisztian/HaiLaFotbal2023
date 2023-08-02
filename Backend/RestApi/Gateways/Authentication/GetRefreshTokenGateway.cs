using Database;
using Microsoft.Data.SqlClient;
using RestApi.Interfaces.Authentication;
using RestApi.Models;
using System.Data;

namespace RestApi.Gateways.Authentication
{
    public class GetRefreshTokenGateway : IGetRefreshTokenGateway
    {
        public RefreshToken GetRefreshToken(int id)
        {
            string query = @"exec GetRefreshTokenForUser_SP @Id";

            DataTable table = new DataTable();
            string sqlDataSource = Config.Get("ConnectionStrings:Connection");
            SqlDataReader myReader;
            using (SqlConnection connection = new SqlConnection(sqlDataSource))
            {
                connection.Open();
                using (SqlCommand myCommand = new SqlCommand(query, connection))
                {
                    myCommand.Parameters.AddWithValue("@Id", id);
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
                return new RefreshToken
                {
                    Id = Int32.Parse(table.Rows[0][0].ToString()),
                    Token = table.Rows[0][1].ToString(),
                    Created = DateTime.Parse(table.Rows[0][2].ToString()),
                    Expires = DateTime.Parse(table.Rows[0][3].ToString())
                };
            }
        }
    }
}