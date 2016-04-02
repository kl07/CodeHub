using System;
using CodeHub.Core.Services;
using CodeHub.Core.Data;
using Octokit;

namespace CodeHub.Core.Services
{
    public interface IApplicationService
    {
        GitHubSharp.Client Client { get; }

        IGitHubClient GitHubClient { get; }
 
        GitHubAccount Account { get; }

        IAccountsService Accounts { get; }

        void ActivateUser(GitHubAccount account);

        void SetUserActivationAction(Action action);

        Action ActivationAction { get; set; }
    }

    public static class ApplicationServiceExtensions
    {
        public static void DeactivateUser(this IApplicationService @this)
        {
            @this.ActivateUser(null);
        }
    }
}