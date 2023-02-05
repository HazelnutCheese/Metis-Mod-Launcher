using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class HotBarMenuButtonVm : ObservableObject
    {
        private readonly Func<bool> _isEnabledFunc;

        public ObservableCollection<HotBarMenuButtonItemVm> Items { get; }

        public string Text { get; }

        public PackIconKind PackIcon { get; }

        public bool IsEnabled => _isEnabledFunc();

        public HotBarMenuButtonVm(
            string text,
            PackIconKind icon,
            List<HotBarMenuButtonItemVm> items,
            Func<bool>? isEnabled = null)
        {
            Items = new ObservableCollection<HotBarMenuButtonItemVm>(items);
            PackIcon = icon;
            Text = text;
            _isEnabledFunc = isEnabled ?? (() => true);
        }

        public void RaiseNotifyCommandExecuteChanged()
        {
            OnPropertyChanged(nameof(IsEnabled));
        }
    }
}
