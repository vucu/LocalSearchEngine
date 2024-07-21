using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalSearchEngine.ViewModels.Components
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void Remove()
        {
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<TValue>(ref TValue value, TValue newValue, [CallerMemberName] string propertyName = null)
        {
            if (value is ValueType ? !value.Equals(newValue) : (value != null || newValue != null) && (value == null || !value.Equals(newValue)))
            {
                value = newValue;

                this.OnPropertyChanged(propertyName);
            }
        }
    }
}
