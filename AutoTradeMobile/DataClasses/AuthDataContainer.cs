namespace AutoTradeMobile
{
    public class AuthDataContainer
    {
        public bool isConfigured
        {
            get
            {
                return string.IsNullOrEmpty(AuthKey) == false & string.IsNullOrEmpty(AuthSecret) == false;
            }
        }
        public string AuthKey
        {
            get
            {
                return Preferences.Get(nameof(AuthKey), string.Empty);
            }
            set
            {
                Preferences.Set(nameof(AuthKey), value);
            }
        }
        public string AuthSecret
        {
            get
            {
                return Preferences.Get(nameof(AuthSecret), string.Empty);
            }
            set
            {
                Preferences.Set(nameof(AuthSecret), value);
            }
        }
    }


}