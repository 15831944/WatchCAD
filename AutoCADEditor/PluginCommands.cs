using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WatchCAD.Model;
using WatchCAD.View;
using WatchCAD.ViewModel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = System.Exception;
using MessageBox = System.Windows.Forms.MessageBox;
using Autodesk.AutoCAD.Geometry;

namespace WatchCAD.Model
{
    public class PluginCommands
    {
        /// <summary>
        /// Тайтл окна
        /// </summary>
        private const string _runWatchCAD = "WatchCAD";

        /// <summary>
        /// ВьюМодель часов
        /// </summary>
        private WatchViewModel _watchViewModel;

        /// <summary>
        /// Запускаем editor
        /// </summary>
        [CommandMethod(_runWatchCAD)]
        public void WatchCAD()
        {
            _watchViewModel = new WatchViewModel();
            //создаем окно
            MainWatchCADWindow mainWindow = new MainWatchCADWindow
            {
                DataContext = _watchViewModel
            };

            try
            {
                //запускаем как модальное окно
                Autodesk.AutoCAD.ApplicationServices.Application.ShowModalWindow(mainWindow);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }   
    }
}