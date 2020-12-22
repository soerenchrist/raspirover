using PlayGround.Util;
using Xamarin.Essentials;

namespace PlayGround.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public string ServerAdress
        {
            get => Preferences.Get(PreferenceKeys.Server, "http://192.168.100.142:5000/control");
            set => Preferences.Set(PreferenceKeys.Server, value);
        }
    }
}