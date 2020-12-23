using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace PlayGround.Services
{
    public class KeyManager
    {
        private static KeyManager? _instance;
        public static KeyManager Current => _instance ??= new KeyManager();

        private Dictionary<string, IObserver<bool>> _observers = new();

        private KeyManager()
        {

        }

        public IObservable<bool> ObserveKeyPress(string key)
        {
            return Observable.Create<bool>(observer =>
            {
                _observers[key] = observer;

                return Disposable.Create(() => _observers.Remove(key));
            });
        }

        public void SendKeyUpEvent(string key)
        {
            if (_observers.ContainsKey(key))
                _observers[key].OnNext(false);
        }

        public void SendKeyDownEvent(string key)
        {
            if (_observers.ContainsKey(key))
                _observers[key].OnNext(true);
        }
    }
}
