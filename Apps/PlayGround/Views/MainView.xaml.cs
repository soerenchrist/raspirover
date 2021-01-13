using System.Linq;
using System.Reactive;
using PlayGround.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlayGround.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView
    {
        public MainView()
        {
            ViewModel = new MainViewModel();
            InitializeComponent();
            var accentColor = (Color)Application.Current.Resources["AccentColor"];
            var backgroundColor = (Color)Application.Current.Resources["BackgroundColor"];
            this.WhenActivated(disposable =>
            {
                this.BindCommand(ViewModel, x => x.StartRoverCommand, x => x.StartRoverButton)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.StartCalibrationCommand, x => x.CalibrateButton)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.DevicesCommand, x => x.DevicesButton)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.IsGyroControl, x => x.GyroControlButton.BackgroundColor,
                    isGyro => isGyro ? accentColor : backgroundColor);
                this.OneWayBind(ViewModel, x => x.IsGyroControl, x => x.TouchControlButton.BackgroundColor,
                    isGyro => isGyro ? backgroundColor : accentColor);

                this.OneWayBind(ViewModel, x => x.IsGyroSupported, x => x.GyroControlButton.IsVisible)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.IsGyroSupported, x => x.TouchControlButton.IsVisible)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.SetControlCommand, x => x.GyroControlButton,
                    Observable.Return(true))
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.SetControlCommand, x => x.TouchControlButton,
                    Observable.Return(false))
                    .DisposeWith(disposable);

                ViewModel.ErrorInteraction
                    .RegisterHandler(interaction =>
                    {
                        this.DisplayAlert("Fehler", interaction.Input, "Ok");
                        interaction.SetOutput(Unit.Default);
                    }).DisposeWith(disposable);
                ViewModel.SelectDeviceInteraction
                    .RegisterHandler(async interaction =>
                    {
                        var result = await DisplayActionSheet("Gerät wählen", "Abbrechen", null,
                            interaction.Input.Select(x => x.Name).ToArray());
                        if (result == null)
                            interaction.SetOutput(null);
                        else
                        {
                            var device = interaction.Input.FirstOrDefault(x => x.Name == result);
                            interaction.SetOutput(device);
                        }
                    }).DisposeWith(disposable);
            });
        }
    }
}
