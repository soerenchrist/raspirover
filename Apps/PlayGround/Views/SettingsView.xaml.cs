using PlayGround.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using Xamarin.Forms.Xaml;

namespace PlayGround.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsView
    {
        public SettingsView()
        {
            ViewModel = new SettingsViewModel();
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel, x => x.ServerAdress, x => x.ServerEntry.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel, x => x.VideoFrameRateString, x => x.FrameRateEntry.Text)
                    .DisposeWith(disposable);
            });
        }
    }
}
