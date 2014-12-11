using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchCAD.Model;

namespace WatchCAD.Interface
{
    /// <summary>
    /// Интерфейс объекта, который будет построен в автокаде
    /// </summary>
    public interface IDrawablePart
    {
        /// <summary>
        /// Построить модель в автокаде
        /// </summary>
        /// <param name="watchData">Параметры часов</param>
        void Build(WatchData watchData);
    }
}
