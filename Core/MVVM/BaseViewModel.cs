using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace lib_iis.Core.MVVM
{
    public class BaseViewModel : INotifyPropertyChanged, IControl
    {
        protected static string _invalidSession = "Access denied";

        public Action<BaseViewModel, string> Close { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string callerName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            RaisePropertyChanged(callerName);
            return true;
        }
        protected virtual bool SetProperty<T>(ref T field, T value, Action onChanged, [CallerMemberName] string callerName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            onChanged?.Invoke();
            RaisePropertyChanged(callerName);
            return true;
        }
        protected void RaisePropertyChanged([CallerMemberName] string callerName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(callerName));
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}
