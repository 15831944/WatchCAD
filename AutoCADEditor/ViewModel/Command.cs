using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WatchCAD.ViewModel
{
    public class Command : ICommand
    {
        #region Constructor

        /// <summary>
        /// конструктор 
        /// </summary>
        /// <param name="action">Действие</param>
        public Command(Action<object> action)
        {
            ExecuteDelegate = action;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Делегат исполнения
        /// </summary>
        public Predicate<object> CanExecuteDelegate { get; set; }

        /// <summary>
        /// Делегат Исполнения
        /// </summary>
        public Action<object> ExecuteDelegate { get; set; }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Возможно ли исполнить
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <returns>Возможно или нет</returns>
        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
            {
                return CanExecuteDelegate(parameter);
            }

            return true;
        }


        /// <summary>
        /// Ивент на исполнение  
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        /// <summary>
        /// Исполнение
        /// </summary>
        /// <param name="parameter">Параметры</param>
        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
            {
                ExecuteDelegate(parameter);
            }
        }

        #endregion
    }
}
