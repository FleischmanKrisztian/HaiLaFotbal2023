using Database;
using Microsoft.Data.SqlClient;
using RestApi.Interfaces.Authentication;
using RestApi.Models;
using RestApi.Models.Requests;
using RestApi.Models.Responses;
using System.Data;

namespace RestApi.Gateways.Authentication
{
    public class RegisterUserGateway : IRegisterUserGateway
    {
        public AuthenticationUserResponse RegisterUser(UserRegistrationRequest userDto)
        {
            string query = @"exec AddUser_SP @Firstname,@Lastname,@Birthdate,@Email,@HashedPassword,@CreatedOn,@PhotoFileName";

            DataTable table = new DataTable();
            string sqlDataSource = Config.Get("ConnectionStrings:Connection");
            SqlDataReader myReader;
            using (SqlConnection connection = new SqlConnection(sqlDataSource))
            {
                connection.Open();
                using (SqlCommand myCommand = new SqlCommand(query, connection))
                {
                    myCommand.Parameters.AddWithValue("@Firstname", userDto.Firstname);
                    myCommand.Parameters.AddWithValue("@Lastname", userDto.Lastname);
                    myCommand.Parameters.AddWithValue("@Birthdate", (long)userDto.Birthdate);
                    myCommand.Parameters.AddWithValue("@Email", userDto.Email);
                    myCommand.Parameters.AddWithValue("@HashedPassword", userDto.Password);
                    myCommand.Parameters.AddWithValue("@CreatedOn", (long)DateTime.Now.Subtract(DateTime.UnixEpoch).TotalSeconds);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", userDto.PhotoFilename);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    connection.Close();
                }
            }
            if (table.Rows.Count == 0)
                return new AuthenticationUserResponse(true, "There is already a user registered with this email!");
            else
            {
                return new AuthenticationUserResponse(Int32.Parse(table.Rows[0][0].ToString()));
            }
        }
    }
}