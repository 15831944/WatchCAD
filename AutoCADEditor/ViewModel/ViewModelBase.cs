using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchCAD.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {

        /// <summary>
        /// ивент параметр был изменен
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// параметр изменен
        /// </summary>
        /// <param name="propertyName">Имя параметра</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
