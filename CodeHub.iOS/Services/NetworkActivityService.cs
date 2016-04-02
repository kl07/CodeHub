using UIKit;
using System;
using System.Reactive.Disposables;
using CodeHub.Core.Services;

namespace CodeHub.iOS.Services
{
    public class NetworkActivityService : INetworkActivityService
    {
        private static UIApplication MainApp = UIApplication.SharedApplication;
        static readonly object NetworkLock = new object();
        static int _active;
 
        private static void PushNetworkActive()
        {
            lock (NetworkLock)
            {
                _active++;
                MainApp.NetworkActivityIndicatorVisible = true;
            }
        }

        private static void PopNetworkActive()
        {
            lock (NetworkLock)
            {
                if (_active == 0)
                    return;

                _active--;
                if (_active == 0)
                    MainApp.NetworkActivityIndicatorVisible = false;
            }
        }

        public IDisposable Activate()
        {
            bool disposed = false;
            PushNetworkActive();
            return Disposable.Create(() => {
                if (disposed) return;
                disposed = true;
                PopNetworkActive();
            });
        }

        public INetworkActivity Create()
        {
            return new NetworkActivity();
        }

        private class NetworkActivity : INetworkActivity
        {
            private int _value;
  
            public void Up()
            {
                _value++;
                NetworkActivityService.PushNetworkActive();
            }

            public void Down()
            {
                if (_value == 0)
                    return;
                _value--;
                NetworkActivityService.PopNetworkActive();
            }

            public void Reset()
            {
                for (var i = 0; i < _value; i++)
                {
                    NetworkActivityService.PopNetworkActive();
                }
            }

            ~NetworkActivity()
            {
                Reset();
            }
        }
    }
}