using RestApi.Interfaces.Authentication;
using RestApi.Models;
using RestApi.Models.Requests;

namespace RestApi.Contexts.Authentication
{
    public class AddOrUpdateRefreshTokenContext
    {
        private readonly IAddOrUpdateRefreshTokenGateway _dataGateway;

        public AddOrUpdateRefreshTokenContext(IAddOrUpdateRefreshTokenGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public bool Execute(RefreshToken token)
        {
            try
            {
                _dataGateway.AddOrUpdateRefreshToken(token);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}