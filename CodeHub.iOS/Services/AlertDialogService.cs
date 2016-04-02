using System;
using CodeHub.Core.Services;
using UIKit;
using System.Threading.Tasks;
using Foundation;
using CoreGraphics;
using BigTed;
using System.Reactive.Disposables;

namespace CodeHub.iOS.Services
{
    public class AlertDialogService : IAlertDialogService
    {
        private static UIViewController ViewController
        {
            get { return UIApplication.SharedApplication.KeyWindow.GetVisibleViewController(); }
        }
        
        public Task<bool> PromptYesNo(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>();
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, x => tcs.SetResult(false)));
            alert.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, x => tcs.SetResult(true)));
            ViewController.PresentViewController(alert, true, null);
            return tcs.Task;
        }

        public Task Alert(string title, string message)
        {
            return ShowAlert(title, message);
        }

        public static Task ShowAlert(string title, string message)
        {
            var tcs = new TaskCompletionSource<object>();
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, x => tcs.SetResult(true)));
            ViewController.PresentViewController(alert, true, null);
            return tcs.Task;
        }

        public static void ShareUrl(string url, UIBarButtonItem barButtonItem = null)
        {
            try
            {
                var item = new NSUrl(url);
                var activityItems = new NSObject[] { item };
                UIActivity[] applicationActivities = null;
                var activityController = new UIActivityViewController (activityItems, applicationActivities);

                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) 
                {
                    var window = UIApplication.SharedApplication.KeyWindow;
                    var pop = new UIPopoverController (activityController);

                    if (barButtonItem != null)
                    {
                        pop.PresentFromBarButtonItem(barButtonItem, UIPopoverArrowDirection.Any, true);
                    }
                    else
                    {
                        var rect = new CGRect(window.RootViewController.View.Frame.Width / 2, window.RootViewController.View.Frame.Height / 2, 0, 0);
                        pop.PresentFromRect (rect, window.RootViewController.View, UIPopoverArrowDirection.Any, true);
                    }
                } 
                else 
                {
                    var viewController = UIApplication.SharedApplication.KeyWindow.GetVisibleViewController();
                    viewController.PresentViewController(activityController, true, null);
                }
            }
            catch
            {
            }
        }

        public Task<string> PromptTextBox(string title, string message, string defaultValue, string okTitle)
        {
            var tcs = new TaskCompletionSource<string>();
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

            alert.AddTextField(t => {
                t.Text = defaultValue;
            });

            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, x => tcs.SetCanceled()));
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, x => tcs.SetResult(alert.TextFields[0].Text)));
            ViewController.PresentViewController(alert, true, null);
            return tcs.Task;
        }

        public IDisposable Show(string text)
        {
            BTProgressHUD.Show(text, maskType: ProgressHUD.MaskType.Gradient);
            return Disposable.Create(BTProgressHUD.Dismiss);
        }

        public IDisposable ShowSuccess(string text)
        {
            BTProgressHUD.ShowSuccessWithStatus(text);
            return Disposable.Create(BTProgressHUD.Dismiss);
        }

        public IDisposable ShowError(string text)
        {
            BTProgressHUD.ShowErrorWithStatus(text);
            return Disposable.Create(BTProgressHUD.Dismiss);
        }
    }
}

