using PlayGround.Util;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using Xamarin.Essentials;

namespace PlayGround.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public string ServerAddress {
            get => Preferences.Get(PreferenceKeys.Server, "http://192.168.100.142:5000/control");
            set => Preferences.Set(PreferenceKeys.Server, value);
        }

        private string _videoFrameRateString;
        public string VideoFrameRateString {
            get => _videoFrameRateString;
            set => this.RaiseAndSetIfChanged(ref _videoFrameRateString, value);
        }

        public SettingsViewModel()
        {
            _videoFrameRateString = Preferences.Get(PreferenceKeys.VideoFrameRate, 500).ToString();

            this.GetIsActivated()
                .Where(x => x == false)
                .Do(_ =>
                {
                    if (int.TryParse(VideoFrameRateString, out var value))
                    {
                        Preferences.Set(PreferenceKeys.VideoFrameRate, value);
                    }
                }).Subscribe();
        }
    }
}
