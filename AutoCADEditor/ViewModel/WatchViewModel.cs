using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using WatchCAD.Model;

namespace WatchCAD.ViewModel
{
    internal class WatchViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Тайтл окна
        /// </summary>
        private string _mainWindowTitle;

        /// <summary>
        /// модель часов
        /// </summary>
        public WatchModel _watchModel;

        /// <summary>
        /// параметры часов
        /// </summary>
        public WatchData _watchData;

        /// <summary>
        /// Модель часов
        /// </summary>
        public WatchData WatchData
        {
            get { return _watchData; }
            set
            {
                if (_watchData != value)
                {
                    _watchData = value;
                    OnPropertyChanged("WatchData");
                }
            }
        }

        /// <summary>
        /// Тайтл окна
        /// </summary>
        public string MainWindowTitle
        {
            get { return _mainWindowTitle; }
            set
            {
                if (_mainWindowTitle != value)
                {
                    _mainWindowTitle = value;
                    OnPropertyChanged("MainWindowTitle");
                }
            }
        }

        #endregion

        #region Commands
        /// <summary>
        /// команда построения
        /// </summary>
        public ICommand BuildCommand { get; set; }
        #endregion
     
        /// <summary>
        /// Constructor.
        /// </summary>
        public WatchViewModel()
        {
            MainWindowTitle = "Watch CAD";
            _watchModel = new WatchModel();
            InitWatchData();
            BuildCommand = new Command(arg => BuildMethod());
        }
    

        /// <summary>
        /// инициализируем часы с начальными параметрами
        /// </summary>
        private void InitWatchData()
        {
            //начальные параметры
            WatchData = new WatchData()
            {
                BodyDiameter = 40,
                BodyHeight = 10,
                CrownDiameter = 5,
                CrownHeight = 3,
                BootstrapLength = 12,
                HasChronograph = true,
                StrapLength = 200,
                StrapWidth = 30,
                NumberOfPerforations = 5,
                StrapPerforationRadius = 0.5
            };
        }

        /// <summary>
        /// Draw watch with parameters
        /// </summary>
        private void BuildMethod()
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //for (int i = 0; i < 20; i++)
			//{
			    _watchModel.Build(WatchData); 
			//}
            
            //stopwatch.Stop();
            //MessageBox.Show((stopwatch.ElapsedMilliseconds).ToString());
            
        }
    }
}   






































































































































