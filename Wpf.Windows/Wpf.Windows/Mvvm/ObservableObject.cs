using System.ComponentModel;

namespace Wpf.Windows.Mvvm
{
    /// <summary>
    /// Simplified MVVM pattern. Usually in combination with Prism or other MVVM frameworks
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
