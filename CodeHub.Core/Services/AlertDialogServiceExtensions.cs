using System;
using ReactiveUI;

namespace CodeHub.Core.Services
{
    public static class AlertDialogExtensions
    {
        public static IDisposable Activate(this IAlertDialogService @this, IObservable<bool> observable, string text)
        {
            IDisposable shownDialog = null;
            return observable.Subscribe(x => {
                if (x)
                    shownDialog = @this.Show(text);
                else
                {
                    shownDialog?.Dispose();
                    shownDialog = null;
                }
            });
        }

        public static IDisposable Activate(this IAlertDialogService @this, IReactiveCommand command, string text)
        {
            IDisposable shownDialog = null;
            return command.IsExecuting.Subscribe(x => {
                if (x)
                    shownDialog = @this.Show(text);
                else
                {
                    shownDialog?.Dispose();
                    shownDialog = null;
                }
            });
        }

        public static IDisposable AlertExecuting(this IReactiveCommand @this, IAlertDialogService dialogFactory, string text)
        {
            IDisposable shownDialog = null;
            return @this.IsExecuting.Subscribe(x => {
                if (x)
                    shownDialog = dialogFactory.Show(text);
                else
                {
                    shownDialog?.Dispose();
                    shownDialog = null;
                }
            });
        }
    }
}

