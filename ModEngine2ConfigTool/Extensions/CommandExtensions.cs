using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ModEngine2ConfigTool.Extensions
{
    public static class CommandExtensions
    {
        public static void NotifyCanExecuteChanged(this ICommand command)
        {
            if(command is IRelayCommand relayCommand)
            {
                relayCommand.NotifyCanExecuteChanged();
                return;
            }

            if(command is IAsyncRelayCommand asyncRelayCommand)
            {
                asyncRelayCommand.NotifyCanExecuteChanged();
                return;
            }
        }
    }
}
