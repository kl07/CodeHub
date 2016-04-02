using CodeHub.Core.Services;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeHub.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using MvvmCross.Platform;
using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace CodeHub.Core.ViewModels
{
    public interface IProvidesTitle
    {
        string Title { get; }
    }

    public interface IRoutingViewModel
    {
        IObservable<IBaseViewModel> RequestNavigation { get; }

        IObservable<Unit> RequestDismiss { get; }
    }

    public interface IBaseViewModel : ReactiveUI.ISupportsActivation, IProvidesTitle, IRoutingViewModel
    {
    }

    /// <summary>
    ///    Defines the BaseViewModel type.
    /// </summary>
    public abstract class BaseViewModel : MvxViewModel, IBaseViewModel, ReactiveUI.IReactiveObject
    {
        private readonly ReactiveUI.ViewModelActivator _viewModelActivator = new ReactiveUI.ViewModelActivator();
        private readonly ISubject<IBaseViewModel> _requestNavigationSubject = new Subject<IBaseViewModel>();
        private readonly ISubject<Unit> _requestDismissSubject = new Subject<Unit>();

        private ReactiveUI.PropertyChangingEventHandler _handler1, _handler2;
        event ReactiveUI.PropertyChangingEventHandler ReactiveUI.IReactiveObject.PropertyChanging
        {
            add { _handler1 += value; }
            remove { _handler1 -= value; }
        }

        void ReactiveUI.IReactiveObject.RaisePropertyChanging(ReactiveUI.PropertyChangingEventArgs args)
        {
            _handler1?.Invoke(this, args);
            _handler2?.Invoke(this, args);
        }

        event ReactiveUI.PropertyChangingEventHandler ReactiveUI.INotifyPropertyChanging.PropertyChanging
        {
            add { _handler2 += value; }
            remove { _handler2 -= value; }
        }
       
        ReactiveUI.ViewModelActivator ReactiveUI.ISupportsActivation.Activator
        {
            get { return _viewModelActivator; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            protected set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        protected void NavigateTo(IBaseViewModel viewModel)
        {
            _requestNavigationSubject.OnNext(viewModel);
        }

        protected void Dismiss()
        {
            _requestDismissSubject.OnNext(Unit.Default);
        }

        IObservable<IBaseViewModel> IRoutingViewModel.RequestNavigation
        {
            get { return _requestNavigationSubject; }
        }

        IObservable<Unit> IRoutingViewModel.RequestDismiss
        {
            get { return _requestDismissSubject; }
        }

        /// <summary>
        /// Gets the go to URL command.
        /// </summary>
        /// <value>The go to URL command.</value>
        public ICommand GoToUrlCommand
        {
            get { return new MvxCommand<string>(x => ShowViewModel<WebBrowserViewModel>(new WebBrowserViewModel.NavObject { Url = x })); }
        }

        /// <summary>
        /// Gets the ViewModelTxService
        /// </summary>
        /// <value>The tx sevice.</value>
        protected IViewModelTxService TxSevice
        {
            get { return GetService<IViewModelTxService>(); }
        }

        /// <summary>
        /// Gets the messenger service
        /// </summary>
        /// <value>The messenger.</value>
        protected IMvxMessenger Messenger
        {
            get { return GetService<IMvxMessenger>(); }
        }

        /// <summary>
        /// Gets the alert service
        /// </summary>
        /// <value>The alert service.</value>
        protected IAlertDialogService AlertService
        {
            get { return GetService<IAlertDialogService>(); }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>An instance of the service.</returns>
        protected TService GetService<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }

        /// <summary>
        /// Display an error message to the user
        /// </summary>
        /// <param name="message">Message.</param>
        protected Task DisplayAlert(string message)
        {
            return AlertService.Alert("Error!", message);
        }
    }
}
