using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.Services
{
    public class NavigationService : ObservableObject
    {
        private readonly Stack<ObservableObject> _history;
        private readonly Stack<ObservableObject> _forwards;

        private ObservableObject? _currentView;

        public ObservableObject? CurrentPage
        {
            get { return _currentView; }
            set { SetProperty(ref _currentView, value); }
        }

        public bool HasHistory => _history.Any();

        public bool HasForwards => _forwards.Any();

        public event EventHandler? HistoryChanged;

        public NavigationService()
        {
            _history = new Stack<ObservableObject>();
            _forwards = new Stack<ObservableObject>();
        }

        public async Task NavigateTo(ObservableObject observableObject)
        {
            if(CurrentPage is not null)
            {
                _history.Push(CurrentPage);
                _forwards.Clear();
            }

            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                CurrentPage = observableObject;
            });

            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task NavigateBack()
        {
            if(!_history.Any())
            {
                return;
            }

            if (CurrentPage is not null)
            {
                _forwards.Push(CurrentPage);
            }

            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                var previousPage = _history.Pop();
                CurrentPage = previousPage;
            });

            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task NavigateForwards()
        {
            if(!_forwards.Any())
            {
                return;
            }

            if (CurrentPage is not null)
            {
                _history.Push(CurrentPage);
            }

            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                var forwardPage = _forwards.Pop();
                CurrentPage = forwardPage;
            });

            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearHistory()
        {
            _history.Clear();
            _forwards.Clear();
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
