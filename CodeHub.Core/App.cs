using System.Net;
using MvvmCross.Core.ViewModels;
using ReactiveUI;
using System.Reactive;
using System;
using System.Threading.Tasks;
using MvvmCross.Platform;
using CodeHub.Core.Services;

namespace CodeHub.Core
{
    /// <summary>
    /// Define the App type.
    /// </summary>
    public class App : MvxApplication
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            RxApp.DefaultExceptionHandler = Observer.Create((Exception e) => {
                if (e is TaskCanceledException)
                    e = new Exception("Timeout waiting for GitHub to respond!");
                Mvx.Resolve<IAlertDialogService>().Alert("Error", e.Message);
            });

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            //Ensure this is loaded
            MvvmCross.Plugins.Messenger.PluginLoader.Instance.EnsureLoaded();
        }
    }
}