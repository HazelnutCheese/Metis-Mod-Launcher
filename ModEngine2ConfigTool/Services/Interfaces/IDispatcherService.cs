using System;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services.Interfaces
{
    public interface IDispatcherService
    {
        Task InvokeAsync(Action action);
        Task<T> InvokeAsync<T>(Func<T> action);
        void InvokeUi(Action action);
        Task InvokeUiAsync(Action action);
        Task<T> InvokeUiAsync<T>(Func<T> action);
    }
}