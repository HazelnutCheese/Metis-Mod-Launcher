using System;
using System.Threading.Tasks;

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

        public void InvokeUi(Action action)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(action);
        }

        public async Task InvokeUiAsync(Action action)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(action);
        }

        public async Task<T> InvokeUiAsync<T>(Func<T> action)
        {
            return await System.Windows.Application.Current.Dispatcher.InvokeAsync(action);
        }
    }
}
