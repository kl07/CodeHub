using CodeHub.Core.Data;
using CodeHub.Core.Services;
using GitHubSharp;
using System;
using Octokit;
using CodeHub.Core.Utils;

namespace CodeHub.Core.Services
{
    public class ApplicationService : IApplicationService
    {
        public Client Client { get; private set; }
        public IGitHubClient GitHubClient { get; private set; }
        public GitHubAccount Account { get; private set; }
        public IAccountsService Accounts { get; private set; }

        public Action ActivationAction { get; set; }

        public ApplicationService(IAccountsService accountsService)
        {
            Accounts = accountsService;
        }

        public void ActivateUser(GitHubAccount account)
        {
            if (account == null)
            {
                Accounts.SetActiveAccount(null);
                Accounts.SetDefault(null);
                Account = null;
                Client = null;
                GitHubClient = null;
            }
            else
            {
                var domain = account.Domain ?? Client.DefaultApi;
                var credentials = new Credentials(account.OAuth);
                var oldClient = Client.BasicOAuth(account.OAuth, domain);
                var newClient = OctokitClientFactory.Create(new Uri(domain), credentials);

                Accounts.SetActiveAccount(account);
                Account = account;
                Client = oldClient;
                GitHubClient = newClient;

                //Set the default account
                Accounts.SetDefault(account);  
            }
        }

        public void SetUserActivationAction(Action action)
        {
            if (Account != null)
                action();
            else
                ActivationAction = action;
        }

    }
}
