using System;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace PlayGround.Util
{
    public static class GetIsActivatedExtensions
    {
        public static IObservable<bool> GetIsActivated(this IActivatableViewModel @this) =>
            Observable
                .Merge(
                    @this.Activator.Activated.Select(_ => true),
                    @this.Activator.Deactivated.Select(_ => false))
                .Replay(1)
                .RefCount();
        
        public static IObservable<bool> GetIsActivated(this IActivatableView @this)
        {
            var activationForViewFetcher = Locator.Current.GetService<IActivationForViewFetcher>();
            return activationForViewFetcher
                .GetActivationForView(@this)
                .Replay(1)
                .RefCount();
        }
    }
}