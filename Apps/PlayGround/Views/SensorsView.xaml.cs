using PlayGround.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using Xamarin.Forms.Xaml;

namespace PlayGround.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorsView
    {
        public SensorsView()
        {
            ViewModel = new SensorsViewModel();
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.Devices, x => x.DeviceList.ItemsSource)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.IsBusy, x => x.ActivityIndicator.IsRunning)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel, x => x.BackCommand, x => x.BackButton)
                    .DisposeWith(disposable);
            });
        }
    }
}
