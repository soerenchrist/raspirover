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
                this.BindCommand(ViewModel, x => x.BackCommand, x => x.BackButton)
                    .DisposeWith(disposable);
            });
        }
    }
}
