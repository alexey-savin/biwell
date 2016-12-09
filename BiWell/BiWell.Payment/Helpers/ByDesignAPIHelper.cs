namespace BiWell.Payment.Helpers
{
    public static class ByDesignAPIHelper
    {
        public static ByDesignOrderAPI.Credentials CreateCredentials(this ByDesignOrderAPI.OrderAPISoap soap)
        {
            var cred = new ByDesignOrderAPI.Credentials();
            cred.Username = Properties.Settings.Default.ByDesignApiUser;
            cred.Password = Properties.Settings.Default.ByDesignApiPassword;

            return cred;
        }

        public static ByDesignOnlineAPI.Credentials CreateCredentials(this ByDesignOnlineAPI.OnlineAPISoap soap)
        {
            var cred = new ByDesignOnlineAPI.Credentials();
            cred.Username = Properties.Settings.Default.ByDesignApiUser;
            cred.Password = Properties.Settings.Default.ByDesignApiPassword;

            return cred;
        }

        public static ByDesignOrderAPI.OrderAPISoap CreateOrderAPIClient() => new ByDesignOrderAPI.OrderAPISoapClient();

        public static ByDesignOnlineAPI.OnlineAPISoap CreateOnlineAPIClient() => new ByDesignOnlineAPI.OnlineAPISoapClient();
    }
}