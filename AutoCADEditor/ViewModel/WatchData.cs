using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchCAD.ViewModel;

namespace WatchCAD.Model
{
    public class WatchData : ViewModelBase
    {
        /// <summary>
        /// высота корпуса
        /// </summary>
        private double _bodyHeight;
        
        /// <summary>
        /// диматр корпуса
        /// </summary>
        private double _bodyDiameter;

        /// <summary>
        /// длина ушек
        /// </summary>
        private double _bootstrapLength;

        /// <summary>
        /// Есть ли хронограф
        /// </summary>
        private bool _hasChronograph;

        /// <summary>
        /// высота зав. головки
        /// </summary>
        private double _crownHeight;

        /// <summary>
        /// Диаметр головки
        /// </summary>
        private double _crownDiameter;

        /// <summary>
        /// длина ремешка
        /// </summary>
        private double _strapLength;

        /// <summary>
        /// ширина ремешк
        /// </summary>
        private double _strapWidth;

        /// <summary>
        /// радиус перфорации
        /// </summary>
        private double _strapPerforationRadius;

        /// <summary>
        /// количество перфораций
        /// </summary>
        private int _numberOfPerforations;

        /// <summary>
        /// ограничивающие параметры
        /// </summary>
        private List<WatchLimitParams> _limitParams;


        /// <summary>
        /// конструктор  
        /// </summary>
        public WatchData()
        {
            InitLimitParams();
            
        }

        /// <summary>
        /// структура ограничивающих параметров
        /// </summary>
        public struct WatchLimitParams
        {
            public string Name;
            public double Minimum;
            public double Maximum;
        }

        /// <summary>
        /// инициализируем параметры ограничения
        /// </summary>
        private void InitLimitParams()
        {
            _limitParams = new List<WatchLimitParams>();
            
            //диаметр корпуса
            _limitParams.Add(new WatchLimitParams()
            {
                Name = "BodyDiameter",
                Maximum = 70,
                Minimum = 30
            });

            //высота корпуса
            _limitParams.Add(new WatchLimitParams()
            {
                Name = "BodyHeight",
                Maximum = 20,
                Minimum = 5
            });

            //диаметр головки
            _limitParams.Add(new WatchLimitParams()
            {
                Name = "CrownDiameter",
                Maximum = 8,
                Minimum = 4
            });

            //высота головки
            _limitParams.Add(new WatchLimitParams()
            {
                Name = "CrownHeight",
                Maximum = 6,
                Minimum = 2
            });

            //длина ремешка
            _limitParams.Add(new WatchLimitParams()
            {
                Name = "StrapLengt",
                Maximum = 500,
                Minimum = 150
            });

            //длина ушек
            _limitParams.Add(new WatchLimitParams()
            {
                Name = "BootstrapLength",
                Maximum = 20,
                Minimum = 5
            });
        }

        /// <summary>
        /// Диаметр корпуса
        /// </summary>
        public double BodyDiameter
        {
            get { return _bodyDiameter; }
            set
            {
                if ((_bodyDiameter != value)
                    && ((value <= _limitParams.Find(l => l.Name == "BodyDiameter").Maximum)
                    && (value >= _limitParams.Find(l => l.Name == "BodyDiameter").Minimum)))
                {
                    _bodyDiameter = value;
                    OnPropertyChanged("BodyDiameter");
                }
                else
                {
                    if (value > (_limitParams.Find(l => l.Name == "BodyDiameter").Maximum))
                    {
                        _bodyDiameter = _limitParams.Find(l => l.Name == "BodyDiameter").Maximum;
                        OnPropertyChanged("BodyDiameter");
                    }

                    if (value < (_limitParams.Find(l => l.Name == "BodyDiameter").Minimum))
                    {
                        _bodyDiameter = _limitParams.Find(l => l.Name == "BodyDiameter").Minimum;
                        OnPropertyChanged("BodyDiameter");
                    }
                }
            }
        }

        /// <summary>
        /// Высота корпуса
        /// </summary>
        public double BodyHeight
        {
            get { return _bodyHeight; }
            set
            {
                if ((_bodyHeight != value) && ((value <= _limitParams.Find(l => l.Name == "BodyHeight").Maximum) && (value >= _limitParams.Find(l => l.Name == "BodyHeight").Minimum)))
                {
                    _bodyHeight = value;
                    OnPropertyChanged("BodyHeight");
                }
                else
                {
                    if (value > (_limitParams.Find(l => l.Name=="BodyHeight").Maximum))
                    {
                        _bodyHeight = _limitParams.Find(l => l.Name=="BodyHeight").Maximum;
                        OnPropertyChanged("BodyHeight");
                    }

                    if (value < (_limitParams.Find(l => l.Name == "BodyHeight").Minimum))
                    {
                        _bodyHeight = _limitParams.Find(l => l.Name == "BodyHeight").Minimum;
                        OnPropertyChanged("BodyHeight");
                    }
                }
            }
        }

        /// <summary>
        /// Длина ушек
        /// </summary>
        public double BootstrapLength
        {
            get { return _bootstrapLength; }
            set
            {
                if (_bootstrapLength != value)
                {
                    _bootstrapLength = value;
                    OnPropertyChanged("BootstrapLength");
                }
            }
        }

        /// <summary>
        /// Диаметр головки
        /// </summary>
        public double CrownDiameter
        {
            get { return _crownDiameter; }
            set
            {
                if (_crownDiameter != value)
                {
                    _crownDiameter = value;
                    OnPropertyChanged("CrownDiameter");
                }
            }
        }


        /// <summary>
        /// Высота головки
        /// </summary>
        public double CrownHeight
        {
            get { return _crownHeight; }
            set
            {
                if (_crownHeight != value)
                {
                    _crownHeight = value;
                    OnPropertyChanged("CrownHeight");
                }
            }
        }

        /// <summary>
        /// Длина ремешка
        /// </summary>
        public double StrapLength
        {
            get { return _strapLength; }
            set
            {
                if (_strapLength != value)
                {
                    _strapLength = value;
                    OnPropertyChanged("StrapLength");
                }
            }
        }


        /// <summary>
        /// Ширина корпуса
        /// </summary>
        public double StrapWidth
        {
            get { return _strapWidth; }
            set
            {
                if (_strapWidth != value)
                {
                    _strapWidth = value;
                    OnPropertyChanged("StrapWidth");
                }
            }
        }

        /// <summary>
        /// радиус перфораций
        /// </summary>
        public double StrapPerforationRadius
        {
            get { return _strapPerforationRadius; }
            set
            {
                if (_strapPerforationRadius != value)
                {
                    _strapPerforationRadius = value;
                    OnPropertyChanged("StrapPerforationRadius");
                }
            }
        }

        /// <summary>
        /// Есть ли хронограф
        /// </summary>
        public bool HasChronograph
        {
            get { return _hasChronograph; }
            set
            {
                if (_hasChronograph != value)
                {
                    _hasChronograph = value;
                    OnPropertyChanged("HasChronograph");
                }
            }
        }

        /// <summary>
        /// Количество перфораций
        /// </summary>
        public int NumberOfPerforations
        {
            get { return _numberOfPerforations; }
            set
            {
                if (_numberOfPerforations != value)
                {
                    _numberOfPerforations = value;
                    OnPropertyChanged("NumberOfPerforations");
                }
            }
        }        
        
    }
}
