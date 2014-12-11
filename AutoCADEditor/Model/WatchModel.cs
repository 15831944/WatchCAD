using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchCAD.Interface;
using WatchCAD.Model;

namespace WatchCAD.Model
{
    public class WatchModel : IDrawablePart
    {
        #region Fields
        /// <summary>
        /// Части часов
        /// </summary>
        private List<IDrawablePart> _parts;
        #endregion

        #region Constructor

        /// <summary>
        /// конструктор
        /// </summary>
        public WatchModel() 
        {
            _parts = new List<IDrawablePart>();
            _parts.Add(new Body());
            _parts.Add(new Strap());
        }

        #endregion

        #region Functions
        /// <summary>
        /// Построить модель
        /// </summary>
        /// <param name="WatchData">Параметры часов</param>
        public void Build(WatchData WatchData)
        {
            //Для каждой части вызываем построитель
            _parts.ForEach(part => part.Build(WatchData));
        }

        #endregion
       
    }
}
