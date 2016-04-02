using GitHubSharp;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using CodeHub.Core.ViewModels;
using System.Net;
using System;
using System.Reactive;

namespace CodeHub.Core.ViewModels
{
    public abstract class LoadableViewModel : BaseViewModel, ILoadableViewModel
    {
        public ReactiveUI.IReactiveCommand<Unit> LoadCommand { get; }

        private async Task LoadResource()
        {
            var retry = false;
            while (true)
            {
                if (retry)
                    await Task.Delay(100);

                try
                {
                    await Load(false);
                    return;
                }
                catch (WebException)
                {
                    if (!retry)
                        retry = true;
                    else
                        throw;
                }
            }
        }

        protected LoadableViewModel()
        {
            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ => {
                try
                {
                    await LoadResource();
                }
                catch (OperationCanceledException e)
                {
                    // The operation was canceled... Don't worry
                    System.Diagnostics.Debug.WriteLine("The operation was canceled: " + e.Message);
                }
                catch (System.IO.IOException)
                {
                    throw new Exception("Unable to communicate with GitHub as the transmission was interrupted! Please try again.");
                }
            });
        }

        protected abstract Task Load(bool forceCacheInvalidation);
    }
}

