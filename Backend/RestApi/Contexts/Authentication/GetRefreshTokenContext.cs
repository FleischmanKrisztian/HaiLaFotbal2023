using RestApi.Interfaces.Authentication;
using RestApi.Models;
using RestApi.Models.Requests;

namespace RestApi.Contexts.Authentication
{
    public class GetRefreshTokenContext
    {
        private readonly IGetRefreshTokenGateway _dataGateway;

        public GetRefreshTokenContext(IGetRefreshTokenGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public RefreshToken Execute(int id)
        {
            try
            {
                return _dataGateway.GetRefreshToken(id);
            }
            catch
            {
                return null;
            }
        }
    }
}