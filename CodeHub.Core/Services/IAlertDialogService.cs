using System.Threading.Tasks;
using System;

namespace CodeHub.Core.Services
{
    public interface IAlertDialogService
    {
        Task<bool> PromptYesNo(string title, string message);

        Task Alert(string title, string message);

        Task<string> PromptTextBox(string title, string message, string defaultValue, string okTitle);

        IDisposable Show(string text);

        IDisposable ShowSuccess(string text);

        IDisposable ShowError(string text);
    }
}

