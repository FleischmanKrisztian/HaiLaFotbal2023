using RestApi.Models;

namespace RestApi.Interfaces.Authentication
{
    public interface IAddOrUpdateRefreshTokenGateway
    {
        public void AddOrUpdateRefreshToken(RefreshToken refreshToken);
    }
}
