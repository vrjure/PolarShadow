namespace PolarShadow.Api.Utilities
{
    public class JWTOptions
    {
        private string? _privateKey;
        public string PrivateKey
        {
            get
            {
                if (string.IsNullOrEmpty(_privateKey))
                {
                    return "0a827e837a27656c058bb0b23a4b516c32894b4b698e3df3de6359a977961c15";
                }
                return _privateKey;
            }
            set => _privateKey = value;
        }

        private string? _userName;
        public string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(_userName))
                {
                    return "Client";
                }
                return _userName;
            }

            set => _userName = value;
        }

        private string? _password;
        public string Password
        {
            get
            {
                if (string.IsNullOrEmpty(_password))
                {
                    return "1234567890";
                }
                return _password;
            }

            set => _password = value;
        }

        private string? _clientId;
        public string ClientId
        {
            get
            {
                if (string.IsNullOrEmpty(_clientId))
                {
                    return "Client";
                }
                return _clientId;
            }

            set => _clientId = value;
        }

        private int _expires;
        public int Expires
        {
            get
            {
                if (_expires <= 1)
                {
                    return 1;
                }
                return _expires;
            }
            set => _expires = value;
        }
    }
}
