using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchCAD.Interface;
using WatchCAD.ViewModel;

namespace WatchCAD.Model
{
    /// <summary>
    /// Заводная головка
    /// </summary>
    public class Crown : Cylinder, IDrawablePart
    {
        /// <summary>
        /// Дополнительные параметры зав. головки
        /// </summary>
        private Dictionary<ParNames, double> _pararameters;

        /// <summary>
        /// Название доп. параметров
        /// </summary>
        private enum ParNames
        {
            CrownTopHeight,
            CrownBottomHeight,
            CrownTransformByZ,
            CrownCutterCount,
            CrownCutterCylinder
        };

        /// <summary>
        /// Конструктора
        /// </summary>
        public Crown()
        {
            _pararameters = new Dictionary<ParNames, double>();
            InitParameters();
        }


        /// <summary>
        /// Инициализировать параметры
        /// </summary>
        private void InitParameters()
        {
            _pararameters.Add(ParNames.CrownTopHeight, 0.5);
            _pararameters.Add(ParNames.CrownBottomHeight, 1);
            _pararameters.Add(ParNames.CrownTransformByZ, 2);
            _pararameters.Add(ParNames.CrownCutterCount, 15);
            _pararameters.Add(ParNames.CrownCutterCylinder, 24);
        }

        /// <summary>
        /// Построить модель в акаде
        /// </summary>
        /// <param name="WatchData">Параметры часов</param>
        public void Build(WatchData WatchData)
        {
            //строим зав. головку
            Builder.BuildModelInAcad(BuildCrown(WatchData));
        }

        /// <summary>
        /// Построить 3д солид-модель
        /// </summary>
        /// <param name="WatchData">Параметры часов</param>
        /// <returns>3Д - солид модель</returns>
        public Solid3d BuildCrown(WatchData WatchData)
        {
            //заводная головка
            Solid3d CrownPart = new Solid3d();
            CrownPart.CreateFrustum(Height, Diameter, Diameter, Diameter);

            //низ заводной головки
            Solid3d CrownBottom = new Solid3d();
            CrownBottom.CreateFrustum(1, Diameter, Diameter, Diameter - _pararameters[ParNames.CrownBottomHeight]);
            CrownBottom.TransformBy(Matrix3d.Displacement(new Point3d(0, 0, Height / _pararameters[ParNames.CrownTransformByZ] + _pararameters[ParNames.CrownTopHeight]) - Point3d.Origin));
            CrownPart.BooleanOperation(BooleanOperationType.BoolUnite, CrownBottom.Clone() as Solid3d);

            //вверх зав.головки
            Solid3d CrownTop = new Solid3d();
            CrownBottom.CreateFrustum(_pararameters[ParNames.CrownTopHeight], Diameter - _pararameters[ParNames.CrownTopHeight], Diameter - _pararameters[ParNames.CrownTopHeight], Diameter);
            CrownBottom.TransformBy(Matrix3d.Displacement(new Point3d(0, 0, -Height / _pararameters[ParNames.CrownTransformByZ] - 0.25) - Point3d.Origin));
            CrownPart.BooleanOperation(BooleanOperationType.BoolUnite, CrownBottom.Clone() as Solid3d);

            CrownBottom.CreateFrustum(_pararameters[ParNames.CrownTopHeight], Diameter - _pararameters[ParNames.CrownTopHeight], Diameter - _pararameters[ParNames.CrownTopHeight], Diameter - 1);
            CrownBottom.TransformBy(Matrix3d.Displacement(new Point3d(0, 0, -Height / _pararameters[ParNames.CrownTransformByZ] - _pararameters[ParNames.CrownTopHeight]) - Point3d.Origin));
            CrownPart.BooleanOperation(BooleanOperationType.BoolSubtract, CrownBottom.Clone() as Solid3d);

            //вырезаем цилиндры по радиусу зав. головки
            Solid3d CrownCutter = new Solid3d();
            CrownCutter.CreateFrustum(Height + _pararameters[ParNames.CrownBottomHeight], Diameter / _pararameters[ParNames.CrownCutterCount], Diameter / _pararameters[ParNames.CrownCutterCount], Diameter / _pararameters[ParNames.CrownCutterCount]);

            for (int i = 0; i < _pararameters[ParNames.CrownCutterCylinder]; i++)
            {
                CrownCutter.TransformBy(Matrix3d.Displacement(new Vector3d(Diameter * Math.Cos(Math.PI / 12 * i), Diameter * Math.Sin(Math.PI / 12 * i), 0)));
                CrownPart.BooleanOperation(BooleanOperationType.BoolSubtract, CrownCutter.Clone() as Solid3d);
                CrownCutter.TransformBy(Matrix3d.Displacement(new Vector3d(-Diameter * Math.Cos(Math.PI / 12 * i), -Diameter * Math.Sin(Math.PI / 12 * i), 0)));
            }

            CrownPart.TransformBy(Matrix3d.Rotation(Math.PI / 2, new Vector3d(1, 0, 0), Point3d.Origin));
            CrownPart.TransformBy(Matrix3d.Displacement(new Vector3d(0, WatchData.BodyDiameter + Height / _pararameters[ParNames.CrownTransformByZ] + _pararameters[ParNames.CrownBottomHeight], 0)));
            return CrownPart;
        }
    }
}
