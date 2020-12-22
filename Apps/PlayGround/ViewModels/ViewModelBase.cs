using ReactiveUI;

namespace PlayGround.ViewModels
{
    public class ViewModelBase : ReactiveObject, IActivatableViewModel
    {
        private ViewModelActivator _activator = new();
        public ViewModelActivator Activator => _activator;
    }
}