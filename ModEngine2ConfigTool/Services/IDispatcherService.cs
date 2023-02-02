using System;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services
{
    public interface IDispatcherService
    {
        Task InvokeAsync(Action action);
        Task<T> InvokeAsync<T>(Func<T> action);
        Task InvokeUiAsync(Action action);
        Task<T> InvokeUiAsync<T>(Func<T> action);
    }
}