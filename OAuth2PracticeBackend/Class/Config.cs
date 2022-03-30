namespace OAuth2PracticeBackend.Class
{
    public class Config
    {
        private static OAuthInfo _loginInfo;
        private static OAuthInfo _notifyInfo;
        public Config()
        {
        }

        public static void setConfig(ConfigurationManager configurationManager)
        {
            _loginInfo = new OAuthInfo
            {
                AuthUrl = configurationManager.GetValue<string>("LineLogin:AuthUrl"),
                AccessTokenUrl = configurationManager.GetValue<string>("LineLogin:AccessTokenUrl"),
                ClientId = configurationManager.GetValue<string>("LineLogin:ClientId"),
                ClientSecret = configurationManager.GetValue<string>("LineLogin:ClientSecret")
            };
            _notifyInfo = new OAuthInfo
            {
                AuthUrl = configurationManager.GetValue<string>("LineNotify:AuthUrl"),
                AccessTokenUrl = configurationManager.GetValue<string>("LineNotify:AccessTokenUrl"),
                ClientId = configurationManager.GetValue<string>("LineNotify:ClientId"),
                ClientSecret = configurationManager.GetValue<string>("LineNotify:ClientSecret")
            };
        }

        public static OAuthInfo getLineLoginInfo()
        {
            return _loginInfo;
        }

        public static OAuthInfo getLineNotifyInfo()
        {
            return _notifyInfo;
        }
    }
}
