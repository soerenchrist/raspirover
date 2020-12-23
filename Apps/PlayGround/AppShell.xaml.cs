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
            Routing.RegisterRoute("calibrate", typeof(CalibrateView));
            Routing.RegisterRoute("control", typeof(ControlView));
            Routing.RegisterRoute("settings", typeof(SettingsView));
            Routing.RegisterRoute("sensors", typeof(SensorsView));
        }
    }
}
