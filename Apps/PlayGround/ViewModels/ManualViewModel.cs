using PlayGround.Services;
using PlayGround.Util;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace PlayGround.ViewModels
{
    public class ManualViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<bool> _connected;
        public bool Connected => _connected.Value;

        private readonly ObservableAsPropertyHelper<byte[]?> _image;
        public byte[]? Image => _image.Value;
        private int _speed;
        public int Speed {
            get => _speed;
            set => this.RaiseAndSetIfChanged(ref _speed, value);
        }

        private int _position;
        public int Position {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }

        public ReactiveCommand<Unit, Unit> TakeImageCommand { get; }

        public ManualViewModel()
        {
            TakeImageCommand = ReactiveCommand.CreateFromTask(x => ControlService.Current.TakeImage());

            this.WhenAnyValue(x => x.Speed)
                .Do(async x => await ControlService.Current.SetSpeed(x))
                .Subscribe();

            this.WhenAnyValue(x => x.Position)
                .Do(async x => await ControlService.Current.SetSteerPosition(x))
                .Subscribe();

            this.GetIsActivated()
                .Do(active =>
                {
                    if (active)
                        ControlService.Current.Connect();
                    else
                        ControlService.Current.Disconnect();
                }).Subscribe();

            _connected = this.GetIsActivated()
                .Select(x => x ? ControlService.Current.Connected : Observable.Return(false))
                .Switch()
                .ToProperty(this, x => x.Connected, scheduler: RxApp.MainThreadScheduler);

            _image = this.GetIsActivated()
                .Select(active => active ? ControlService.Current.LastImage : Observable.Return<byte[]?>(null))
                .Switch()
                .ToProperty(this, x => x.Image);
        }
    }
}
