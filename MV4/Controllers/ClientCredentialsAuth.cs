namespace MV4.Controllers
{
    internal class ClientCredentialsAuth
    {
        private string clientId;
        private string clientSecret;

        public ClientCredentialsAuth(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }
    }
}