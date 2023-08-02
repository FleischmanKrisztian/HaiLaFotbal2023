using Database;
using Microsoft.Data.SqlClient;
using RestApi.Interfaces.Authentication;
using RestApi.Models;
using System.Data;

namespace RestApi.Gateways.Authentication
{
    public class AddOrUpdateRefreshTokenGateway : IAddOrUpdateRefreshTokenGateway
    {
        public void AddOrUpdateRefreshToken(RefreshToken refreshToken)
        {
            string query = @"exec AddOrUpdateRefreshToken_SP @Id, @RefreshToken, @TokenCreated, @TokenExpires";

            DataTable table = new DataTable();
            string sqlDataSource = Config.Get("ConnectionStrings:Connection");
            using (SqlConnection connection = new SqlConnection(sqlDataSource))
            {
                connection.Open();
                using (SqlCommand myCommand = new SqlCommand(query, connection))
                {
                    myCommand.Parameters.AddWithValue("@Id", refreshToken.Id);
                    myCommand.Parameters.AddWithValue("@RefreshToken", refreshToken.Token);
                    myCommand.Parameters.AddWithValue("@TokenCreated", refreshToken.Created);
                    myCommand.Parameters.AddWithValue("@TokenExpires", refreshToken.Expires);

                    myCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
    }
}