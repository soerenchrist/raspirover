using PlayGround.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlayGround
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("gyro", typeof(RotationView));
            Routing.RegisterRoute("calibrate", typeof(CalibrateView));
            Routing.RegisterRoute("manual", typeof(ManualView));
            Routing.RegisterRoute("settings", typeof(SettingsView));
        }
    }
}