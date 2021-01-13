using Montage.Mobile.Utility;
using PlayGround.ViewModels;
using ReactiveUI;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlayGround.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ControlView
    {
        public ControlView()
        {
            ViewModel = new ControlViewModel();
            InitializeComponent();
            var textColor = (Color)Application.Current.Resources["TextColor"];
            this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                    return;
                this.Bind(ViewModel, x => x.Speed, x => x.SpeedSlider.Value)
                    .DisposeWith(disposable);
                this.Bind(ViewModel, x => x.Position, x => x.SteerSlider.Value)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.Image)
                    .Where(x => x != null)
                    .Select(x => ImageSource.FromStream(() => new MemoryStream(x)))
                    .BindTo(this, x => x.TakenImage.Source)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.TakeImageCommand, x => x.TakeImageButton)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.VideoCommand, x => x.VideoButton)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, x => x.GyroMode, x => x.SpeedSlider.IsEnabled, x => !x)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.GyroMode, x => x.SteerSlider.IsEnabled, x => !x)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, x => x.VideoRunning, x => x.VideoButton.Text,
                        x => x ? IconFont.VideoOffOutline : IconFont.VideoOutline)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, x => x.VideoRunning, x => x.VideoButton.TextColor,
                        x => x ? Color.Red : textColor)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, x => x.Distance, x => x.DistanceIndicator.Distance)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel, x => x.BackCommand, x => x.BackButton)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, x => x.FrontLightOn, x => x.ToggleLightButton.TextColor,
                    on => on ? Color.Yellow : Color.White)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.FrontLightOn, x => x.ToggleLightButton.Text,
                    on => on ? IconFont.CarLightHigh : IconFont.CarLightDimmed)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel, x => x.ToggleLightCommand, x => x.ToggleLightButton)
                    .DisposeWith(disposable);


            });
        }
    }
}
