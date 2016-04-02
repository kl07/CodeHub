using System;

namespace CodeHub.Core.Services
{
    public interface INetworkActivityService
    {
        IDisposable Activate();

        INetworkActivity Create();
    }

    public interface INetworkActivity
    {
        void Up();

        void Down();

        void Reset();
    }
}
