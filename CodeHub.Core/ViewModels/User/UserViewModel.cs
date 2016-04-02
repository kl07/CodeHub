using System;
using System.Windows.Input;
using CodeHub.Core.ViewModels.Events;
using CodeHub.Core.ViewModels.Gists;
using CodeHub.Core.ViewModels.Organizations;
using CodeHub.Core.ViewModels;
using CodeHub.Core.Services;
using System.Reactive;
using System.Reactive.Linq;
using MvvmCross.Core.ViewModels;

namespace CodeHub.Core.ViewModels.User
{
    public class UserViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IApplicationService _applicationService;

        public string Username { get; private set; }

        private Octokit.User _user;
        public Octokit.User User
        {
            get { return _user; }
            private set { this.RaiseAndSetIfChanged(ref _user, value); }
        }

        private bool? _isFollowing;
        public bool? IsFollowing
        {
            get { return _isFollowing; }
            private set { this.RaiseAndSetIfChanged(ref _isFollowing, value); }
        }

        public ReactiveUI.IReactiveCommand<Unit> LoadCommand { get; }

        public bool IsLoggedInUser
        {
            get { return string.Equals(Username, _applicationService.Account.Username); }
        }

        public ICommand GoToFollowersCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserFollowersViewModel>(new UserFollowersViewModel.NavObject { Username = Username })); }
        }

        public ICommand GoToFollowingCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserFollowingsViewModel>(new UserFollowingsViewModel.NavObject { Name = Username })); }
        }

        public ICommand GoToEventsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserEventsViewModel>(new UserEventsViewModel.NavObject { Username = Username })); }
        }

        public ICommand GoToOrganizationsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<OrganizationsViewModel>(new OrganizationsViewModel.NavObject { Username = Username })); }
        }

        public ICommand GoToRepositoriesCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserRepositoriesViewModel>(new UserRepositoriesViewModel.NavObject { Username = Username })); }
        }

        public ICommand GoToGistsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserGistsViewModel>(new UserGistsViewModel.NavObject { Username = Username })); }
        }

        public ReactiveUI.IReactiveCommand<Unit> ToggleFollowingCommand { get; }

        public ReactiveUI.IReactiveCommand<Unit> ShowMenuCommand { get; }

        public UserViewModel(IApplicationService applicationService, IActionMenuService actionMenuService)
        {
            _applicationService = applicationService;

            var isFollowingObs = this.Bind(x => x.IsFollowing).Select(x => x.HasValue);
            
            ToggleFollowingCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(isFollowingObs, async _ => {
                if (IsFollowing.Value)
                    await _applicationService.GitHubClient.User.Followers.Unfollow(Username);
                else
                    await _applicationService.GitHubClient.User.Followers.Follow(Username);
                IsFollowing = !IsFollowing.Value;
            });

            ShowMenuCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(isFollowingObs, sender => {
                var menu = actionMenuService.Create();
                menu.AddButton(IsFollowing.Value ? "Unfollow" : "Follow", ToggleFollowingCommand);
                return menu.Show(sender);
            });

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ => {
                if (!IsLoggedInUser)
                {
                    applicationService.GitHubClient.User.Followers.IsFollowingForCurrent(Username)
                        .ToBackground(x => IsFollowing = x);
                }

                User = await applicationService.GitHubClient.User.Get(Username);
            });
        }
  
        public void Init(NavObject navObject)
        {
            Title = Username = navObject.Username;
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}

