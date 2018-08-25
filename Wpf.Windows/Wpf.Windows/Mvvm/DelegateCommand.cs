using System;
using System.Windows.Input;

namespace Wpf.Windows.Mvvm
{
    /// <summary>
    /// Simplified MVVM pattern. Usually in combination with Prism or other MVVM frameworks
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action action;

        public DelegateCommand(Action action)
        {
            this.action = action;
        }

        public void Execute(object parameter)
        {
            action();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }
}
