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
    public partial class ManualView
    {
        public ManualView()
        {
            ViewModel = new ManualViewModel();
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel, x => x.Speed, x => x.SpeedSlider.Value)
                    .DisposeWith(disposable);
                this.Bind(ViewModel, x => x.Position, x => x.SteerSlider.Value)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, x => x.Connected, x => x.ConnectionState.BackgroundColor,
                        connected => connected ? Color.Green : Color.Red)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel.Image)
                    .Where(x => x != null)
                    .Select(x => ImageSource.FromStream(() => new MemoryStream(x)))
                    .BindTo(this, x => x.TakenImage.Source)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.TakeImageCommand, x => x.TakeImageButton)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.VideoCommand, x => x.VideoButton)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, x => x.VideoRunning, x => x.VideoButton.Text,
                        x => x ? "Stop" : "Video")
                    .DisposeWith(disposable);
            });
        }
    }
}
