using System;
using System.Globalization;
using System.Reactive.Disposables;
using PlayGround.ViewModels;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlayGround.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RotationView
    {
        public RotationView()
        {
            ViewModel = new RotationViewModel();
            InitializeComponent();
            this.WhenActivated(disposable =>
            {

                this.OneWayBind(ViewModel, x => x.Speed, x => x.SpeedSlider.Value)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.SteerPosition, x => x.SteerSlider.Value)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.Connected, x => x.ConnectionState.BackgroundColor,
                        connected => connected ? Color.Green : Color.Red)
                    .DisposeWith(disposable);
            });
        }
    }
}