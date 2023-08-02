using Database;
using Microsoft.Data.SqlClient;
using RestApi.Interfaces.Meetup;
using RestApi.Models;
using System.Data;

namespace RestApi.Gateways.Authentication
{
    public class GetMeetupByIdGateway : IGetMeetupByIdGateway
    {
        public Meetup GetMeetupById(int id)
        {

            DataTable table = new DataTable();
            string sqlDataSource = Config.Get("ConnectionStrings:Connection");
            SqlDataReader myReader;
            using (SqlConnection connection = new SqlConnection(sqlDataSource))
            {
                connection.Open();
                var sqlCommand = new SqlCommand("GetMeetupById_SP", connection) { CommandType = CommandType.StoredProcedure };
                sqlCommand.Parameters.AddWithValue("@Id", id);
                myReader = sqlCommand.ExecuteReader();
                table.Load(myReader);
                myReader.Close();
                connection.Close();
                
            }

            if (table.Rows.Count == 0)
                return null;
            else
            {
                var user = new User
                {
                    Id = Int32.Parse(table.Rows[0][10].ToString()),
                    Firstname = table.Rows[0][11].ToString(),
                    Lastname = table.Rows[0][12].ToString(),
                    Birthdate = Int64.Parse(table.Rows[0][13].ToString()),
                    Email = table.Rows[0][14].ToString(),
                    CreatedOn = Int64.Parse(table.Rows[0][16].ToString()),
                    PhotoFileName = table.Rows[0][17].ToString(),
                };
                return new Meetup
                {
                    Id = Int32.Parse(table.Rows[0][0].ToString()),
                    Type = Int32.Parse(table.Rows[0][1].ToString()),
                    MeetingTime = Int64.Parse(table.Rows[0][2].ToString()),
                    Ycoordinate = table.Rows[0][3].ToString(),
                    Xcoordinate = table.Rows[0][4].ToString(),
                    Description = table.Rows[0][5].ToString(),
                    AvailableSlots = Int32.Parse(table.Rows[0][6].ToString()),
                    Level = Int32.Parse(table.Rows[0][7].ToString()),
                    CreatedOn = Int64.Parse(table.Rows[0][8].ToString()),
                    CreatedBy = user
                };
            }
        }
    }
}