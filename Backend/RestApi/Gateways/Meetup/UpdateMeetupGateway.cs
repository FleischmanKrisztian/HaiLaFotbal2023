using Database;
using Microsoft.Data.SqlClient;
using RestApi.Interfaces.Meetup;
using RestApi.Models;
using System.Data;

namespace RestApi.Gateways.Authentication
{
    public class UpdateMeetupGateway : IUpdateMeetupGateway
    {
        public void UpdateMeetup(int id, Meetup meetup)
        {
            string sqlDataSource = Config.Get("ConnectionStrings:Connection");
            using (SqlConnection connection = new SqlConnection(sqlDataSource))
            {
                connection.Open();
                var sqlParameters = MapMeetupToSP(id, meetup);
                var sqlCommand = new SqlCommand("UpdateMeetup_SP", connection) { CommandType = CommandType.StoredProcedure };
                sqlCommand.Parameters.AddRange(sqlParameters);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static SqlParameter[] MapMeetupToSP(int id, Meetup meetup)
        {
            return new[]
            {
                new SqlParameter("@Id",SqlDbType.Int) { Value = meetup.Id },
                new SqlParameter("@TypeId",SqlDbType.Int) { Value = meetup.Type },
                new SqlParameter("@MeetingTime",SqlDbType.NVarChar, 1000) { Value = meetup.MeetingTime },
                new SqlParameter("@YCoordinate",SqlDbType.NVarChar, 100) { Value = meetup.Ycoordinate },
                new SqlParameter("@XCoordinate",SqlDbType.NVarChar, 100) { Value = meetup.Xcoordinate },
                new SqlParameter("@Description",SqlDbType.NVarChar, 500) { Value = meetup.Description },
                new SqlParameter("@AvailableSlots",SqlDbType.Int) { Value = meetup.AvailableSlots },
                new SqlParameter("@Level",SqlDbType.Int) { Value = meetup.Level },
                new SqlParameter("@UserId", SqlDbType.Int) {Value = id}
            };
        }
    }
}