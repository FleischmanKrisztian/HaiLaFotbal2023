using RestApi.Models;

namespace RestApi.Interfaces.Authentication
{
    public interface IGetRefreshTokenGateway
    {
        public RefreshToken GetRefreshToken(int id);
    }
}