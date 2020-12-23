using DynamicData;
using DynamicData.Binding;
using PlayGround.Util;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace PlayGround.ViewModels
{
    public class SensorsViewModel : ViewModelBase
    {
        private ObservableAsPropertyHelper<bool> _isBusy;
        public bool IsBusy => _isBusy.Value;

        private readonly SourceList<IDevice> _devicesSourceList = new SourceList<IDevice>();
        public IObservableCollection<IDevice> Devices { get; } = new ObservableCollectionExtended<IDevice>();

        public ReactiveCommand<Unit, Unit> BackCommand { get; }

        private readonly IBluetoothLE _ble;
        private readonly IAdapter _adapter;


        public SensorsViewModel()
        {
            _ble = CrossBluetoothLE.Current;
            _adapter = _ble.Adapter;
            _adapter.ScanMode = ScanMode.Balanced;
            _adapter.ScanTimeout = (int)TimeSpan.FromSeconds(10).TotalSeconds;

            _devicesSourceList.Connect()
                .Bind(Devices).Subscribe();

            BackCommand = ReactiveCommand.CreateFromTask(() => Shell.Current.GoToAsync(".."));

            _isBusy = Observable.Timer(TimeSpan.FromSeconds(10)).Select(_ => false)
                        .StartWith(true)
                        .ToProperty(this, x => x.IsBusy);

            var discoveredObservable = Observable.FromEventPattern<DeviceEventArgs>(x => _adapter.DeviceDiscovered += x,
                    x => _adapter.DeviceDiscovered -= x)
                .Select(x => x.EventArgs.Device);

            this.GetIsActivated()
                .Select(x => x ? discoveredObservable : Observable.Return<IDevice?>(null))
                .Switch()
                .Where(x => x != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(x =>
                {
                    _devicesSourceList.Edit(inner =>
                    {
                        inner.Add(x!);
                    });
                })
                .Subscribe();

            this.GetIsActivated()
                .Do(active =>
                {
                    if (active)
                        _adapter.StartScanningForDevicesAsync();
                    else
                        _adapter.StopScanningForDevicesAsync();
                })
                .Subscribe();
        }
    }
}
