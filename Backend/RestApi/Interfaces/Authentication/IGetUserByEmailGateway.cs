using RestApi.Models;

namespace RestApi.Interfaces.Authentication
{
    public interface IGetUserByEmailGateway
    {
        public User GetUserByEmail(string email);
    }
}