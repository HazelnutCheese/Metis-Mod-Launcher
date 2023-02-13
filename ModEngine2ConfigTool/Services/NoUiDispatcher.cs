using System;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services
{
    public class NoUiDispatcher : IDispatcherService
    {
        public Task InvokeAsync(Action action)
        {
            return Task.Run(action);
        }

        public Task<T> InvokeAsync<T>(Func<T> action)
        {
            return Task.Run(action);
        }

        public void InvokeUi(Action action)
        {
            Task.Run(action);
        }

        public Task InvokeUiAsync(Action action)
        {
            return Task.Run(action);
        }

        public Task<T> InvokeUiAsync<T>(Func<T> action)
        {
            return Task.Run(action);
        }
    }
}
