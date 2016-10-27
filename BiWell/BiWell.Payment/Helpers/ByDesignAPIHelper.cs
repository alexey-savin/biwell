namespace BiWell.Payment.Helpers
{
    public static class ByDesignAPIHelper
    {
        public static ByDesignOrderAPI.Credentials CreateCredentials(this ByDesignOrderAPI.OrderAPISoap soap)
        {
            ByDesignOrderAPI.Credentials cred = new ByDesignOrderAPI.Credentials();
            cred.Username = Properties.Settings.Default.ByDesignApiUser;
            cred.Password = Properties.Settings.Default.ByDesignApiPassword;

            return cred;
        }

        public static ByDesignOrderAPI.OrderAPISoap CreateAPIClient()
        {
            return new ByDesignOrderAPI.OrderAPISoapClient();
        }
    }
}