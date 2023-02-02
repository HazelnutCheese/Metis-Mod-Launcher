using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.Services
{
    public class DispatcherService : IDispatcherService
    {
        public async Task InvokeAsync(Action action)
        {
            await Task.Run(action);
        }

        public async Task<T> InvokeAsync<T>(Func<T> action)
        {
            return await Task.Run(action);
        }

        public async Task InvokeUiAsync(Action action)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(action);
        }

        public async Task<T> InvokeUiAsync<T>(Func<T> action)
        {
            return await Dispatcher.CurrentDispatcher.InvokeAsync(action);
        }
    }
}
