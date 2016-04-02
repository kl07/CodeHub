using CodeHub.iOS.ViewControllers;
using CodeHub.Core.ViewModels.User;
using UIKit;
using System;
using CodeHub.iOS.DialogElements;
using System.Reactive.Linq;
using ReactiveUI;

namespace CodeHub.iOS.ViewControllers.Users
{
    public class UserViewController : PrettyDialogViewController
    {
        public new UserViewModel ViewModel
        {
            get { return (UserViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeaderView.Text = ViewModel.Username;
            HeaderView.SubImageView.TintColor = UIColor.FromRGB(243, 156, 18);

            Appeared.Take(1)
                .Select(_ => Observable.Timer(TimeSpan.FromSeconds(0.35f)))
                .Switch()
                .Select(_ => ViewModel.Bind(x => x.IsFollowing).Where(x => x.HasValue))
                .Switch()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => HeaderView.SetSubImage(x.Value ? Octicon.Star.ToImage() : null));
            
            var split = new SplitButtonElement();
            var followers = split.AddButton("Followers", "-");
            var following = split.AddButton("Following", "-");
            var events = new StringElement("Events", Octicon.Rss.ToImage());
            var organizations = new StringElement("Organizations", Octicon.Organization.ToImage());
            var repos = new StringElement("Repositories", Octicon.Repo.ToImage());
            var gists = new StringElement("Gists", Octicon.Gist.ToImage());
            Root.Add(new [] { new Section { split }, new Section { events, organizations, repos, gists } });
    
            OnActivation(d => {
                d(followers.Clicked.BindCommand(ViewModel.GoToFollowersCommand));
                d(following.Clicked.BindCommand(ViewModel.GoToFollowingCommand));
                d(events.Clicked.BindCommand(ViewModel.GoToEventsCommand));
                d(organizations.Clicked.BindCommand(ViewModel.GoToOrganizationsCommand));
                d(repos.Clicked.BindCommand(ViewModel.GoToRepositoriesCommand));
                d(gists.Clicked.BindCommand(ViewModel.GoToGistsCommand));
                d(ViewModel.Bind(x => x.Title).Subscribe(x => Title = x));

                d(this.Bind(x => x.ViewModel)
                    .Where(x => !x.IsLoggedInUser)
                    .Select(x => x.ShowMenuCommand)
                    .ToBarButtonItem(UIBarButtonSystemItem.Action, x => NavigationItem.RightBarButtonItem = x));

                d(ViewModel.Bind(x => x.User).Subscribe(x => {
                    followers.Text = x?.Followers.ToString() ?? "-";
                    following.Text = x?.Following.ToString() ?? "-";
                    HeaderView.SubText = string.IsNullOrWhiteSpace(x?.Name) ? null : x.Name;
                    HeaderView.SetImage(x?.AvatarUrl, Images.Avatar);
                    RefreshHeaderView();
                }));
            });
        }
    }
}

