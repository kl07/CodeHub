using System;
using CodeHub.Core.Services;
using System.Reactive.Linq;
using MvvmCross.Core.ViewModels;

namespace CodeHub.Core.ViewModels.Contents
{
    public class CreateFileViewModel : BaseViewModel
    {
        public string RepositoryOwner { get; private set; }

        public string RepositoryName { get; private set; }

        public string Path { get; private set; }

        public string Branch { get; private set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { this.RaiseAndSetIfChanged(ref _content, value); }
        }

        private string _commitMessage;
        public string CommitMessage
        {
            get { return _commitMessage; }
            set { this.RaiseAndSetIfChanged(ref _commitMessage, value); }
        }

        public ReactiveUI.IReactiveCommand<Octokit.RepositoryContentChangeSet> SaveCommand { get; }

        public ReactiveUI.IReactiveCommand<object> GoToCommitMessageCommand { get; }

        public ReactiveUI.IReactiveCommand<bool> DismissCommand { get; }

        public CreateFileViewModel(IApplicationService applicationService, IAlertDialogService alertDialogService)
        {
            Title = "Create File";

            var isNotNullName = this.Bind(x => x.Name).IsNotNull();
            var isValidName = this.Bind(x => x.Name).Select(x => !string.IsNullOrEmpty(x));

            isNotNullName.Subscribe(x => CommitMessage = "Created " + x);

            GoToCommitMessageCommand = ReactiveUI.ReactiveCommand.Create(isValidName);

            SaveCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(isValidName, async _ => {
                    var content = Content ?? string.Empty;
                    var path = System.IO.Path.Combine(Path ?? string.Empty, Name);
                    var request = new Octokit.CreateFileRequest(CommitMessage, content) { Branch = Branch };
                    using (alertDialogService.Show("Commiting..."))
                        return await applicationService.GitHubClient.Repository.Content.CreateFile(RepositoryOwner, RepositoryName, path, request);
                });
            SaveCommand.Subscribe(x => Dismiss());

            DismissCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async t => {
                if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Content)) return true;
                return await alertDialogService.PromptYesNo("Discard File?", "Are you sure you want to discard this file?");
            });
            DismissCommand.Where(x => x).Subscribe(_ => Dismiss());
        }

        public CreateFileViewModel Init(string repositoryOwner, string repositoryName, string path, string branch)
        {
            RepositoryOwner = repositoryOwner;
            RepositoryName = repositoryName;
            Path = path;
            Branch = branch;
            return this;
        }
    }
}

