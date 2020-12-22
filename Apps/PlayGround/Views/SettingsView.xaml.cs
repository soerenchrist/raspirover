using System.Reactive.Disposables;
using PlayGround.ViewModels;
using ReactiveUI;
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
            });
        }
    }
}