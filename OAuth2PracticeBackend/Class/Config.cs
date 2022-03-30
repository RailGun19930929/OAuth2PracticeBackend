namespace OAuth2PracticeBackend.Class
{
    public class Config
    {
        private ConfigurationManager _configurationManager;
        private LineLoginInfo _loginInfo;
        private LineNotifyInfo _notifyInfo;
        public Config(ConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
            _loginInfo = configurationManager.GetValue<LineLoginInfo>("LineLogin");
            Console.WriteLine(_loginInfo);
        }

    }
}
