using Database;
using Microsoft.Data.SqlClient;
using RestApi.Interfaces.Meetup;
using RestApi.Models;
using System.Data;

namespace RestApi.Gateways.Authentication
{
    public class GetAllMeetupsGateway : IGetAllMeetupsGateway
    {
        public List<Meetup> GetMeetups()
        {
            string query = @"GetAllMeetups_SP";

            DataTable table = new DataTable();
            string sqlDataSource = Config.Get("ConnectionStrings:Connection");
            SqlDataReader myReader;
            using (SqlConnection connection = new SqlConnection(sqlDataSource))
            {
                connection.Open();
                using (SqlCommand myCommand = new SqlCommand(query, connection))
                {
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
                var meetups = new List<Meetup>();
                foreach (DataRow row in table.Rows)
                {
                    var user = new User
                    {
                        Id = Int32.Parse(row[10].ToString()),
                        Firstname = row[11].ToString(),
                        Lastname = row[12].ToString(),
                        Birthdate = Int64.Parse(row[13].ToString()),
                        Email = row[14].ToString(),
                        CreatedOn = Int64.Parse(row[16].ToString()),
                        PhotoFileName = row[17].ToString(),
                    };
                    meetups.Add(new Meetup
                    {
                        Id = Int32.Parse(row[0].ToString()),
                        Type = Int32.Parse(row[1].ToString()),
                        MeetingTime = Int64.Parse(row[2].ToString()),
                        Ycoordinate = row[3].ToString(),
                        Xcoordinate = row[4].ToString(),
                        Description = row[5].ToString(),
                        AvailableSlots = Int32.Parse(row[6].ToString()),
                        Level = Int32.Parse(row[7].ToString()),
                        CreatedOn = Int64.Parse(row[8].ToString()),
                        CreatedBy = user
                    });
                }
                return meetups;
            }
        }
    }
}