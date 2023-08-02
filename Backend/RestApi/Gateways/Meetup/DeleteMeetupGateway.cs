using Database;
using Microsoft.Data.SqlClient;
using RestApi.Interfaces.Meetup;
using System.Data;

namespace RestApi.Gateways.Authentication
{
    public class DeleteMeetupGateway : IDeleteMeetupGateway
    {
        public void DeleteMeetup(int userId, int meetupId)
        {
            string sqlDataSource = Config.Get("ConnectionStrings:Connection");
            using (SqlConnection connection = new SqlConnection(sqlDataSource))
            {
                connection.Open();
                var sqlCommand = new SqlCommand("DeleteMeetup_SP", connection) { CommandType = CommandType.StoredProcedure };
                sqlCommand.Parameters.AddWithValue("@Id", meetupId);
                sqlCommand.Parameters.AddWithValue("@UserId", userId);
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}