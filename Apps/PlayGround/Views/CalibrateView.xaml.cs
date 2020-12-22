using System;
using System.Globalization;
using System.Reactive.Disposables;
using PlayGround.ViewModels;
using ReactiveUI;
using Xamarin.Forms.Xaml;

namespace PlayGround.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalibrateView
    {
        public CalibrateView()
        {
            ViewModel = new CalibrateViewModel();
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.BindCommand(ViewModel, x => x.SaveCommand, x => x.SaveButton)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.CurrentPosition, x => x.PositionLabel.Text)
                    .DisposeWith(disposable);
                
                
                this.OneWayBind(ViewModel, x => x.MagneticField.X, x => x.XMagLabel.Text, x => Math.Round(x, 2).ToString(CultureInfo.CurrentCulture))
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.MagneticField.Y, x => x.YMagLabel.Text, x => Math.Round(x, 2).ToString(CultureInfo.CurrentCulture))
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.MagneticField.Z, x => x.ZMagLabel.Text, x => Math.Round(x, 2).ToString(CultureInfo.CurrentCulture))
                    .DisposeWith(disposable);
            });
        }
    }
}