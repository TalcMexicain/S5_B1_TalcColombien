using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    /// <summary>
    /// Base class for all ViewModels, implementing INotifyPropertyChanged for UI updates.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Protected Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            bool propertyChanged = false;
            
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                propertyChanged = true;
            }
            
            return propertyChanged;
        }

        #endregion
    }
}
