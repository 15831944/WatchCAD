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
    /// Ремешок на часах
    /// </summary>
    public class Strap : IDrawablePart
    {
        /// <summary>
        /// Длина ремешка
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// Ширина ремешка
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Радиус перфораций
        /// </summary>
        public double PerforationRadius { get; set; }

        /// <summary>
        /// Количество перфораций
        /// </summary>
        public int NumberOfPerforations { get; set; }


        private enum ParNames
        {
            StrapTransformByX,
            StrapTransformByY,
        };

        /// <summary>
        /// Дополнительные параметры
        /// </summary>
        private Dictionary<ParNames, double> _params;


        /// <summary>
        /// Конструктор
        /// </summary>
        public Strap()
        {
            _params = new Dictionary<ParNames, double>();
            InitParams();
        }
        

        /// <summary>
        /// построить ремешок часов
        /// </summary>
        /// <param name="WatchData">информация о часах</param>
        public void Build(WatchData WatchData)
        {
            //выставляем параметры
            Length = WatchData.StrapLength;
            Width = WatchData.StrapWidth;
            PerforationRadius = WatchData.StrapPerforationRadius;
            NumberOfPerforations = WatchData.NumberOfPerforations;

            //строим
            Builder.BuildModelInAcad(BuildStrapWithPerforations(WatchData));
            Builder.BuildModelInAcad(BuildStrapWithLocker(WatchData));

        }

        /// <summary>
        /// ремешок с замком
        /// </summary>
        /// <param name="WatchData">информация о часах</param>
        /// <returns>модель ремешка</returns>
        public Solid3d BuildStrapWithLocker(WatchData WatchData)
        {
            //модель ремешка с замком
            Solid3d StrapWithLocker = new Solid3d();
            StrapWithLocker.CreateBox(Length / _params[ParNames.StrapTransformByX], Width, 3);

            //сглаживания ремешка
            Solid3d StrapSmoother = new Solid3d();
            StrapSmoother.CreateFrustum(Width, _params[ParNames.StrapTransformByY], _params[ParNames.StrapTransformByY], _params[ParNames.StrapTransformByY]);
            StrapSmoother.TransformBy(Matrix3d.Rotation(Math.PI / _params[ParNames.StrapTransformByX], new Vector3d(1, 0, 0), Point3d.Origin));
            StrapSmoother.TransformBy(Matrix3d.Displacement(new Vector3d(Length / 4, 0, 0)));
            StrapWithLocker.BooleanOperation(BooleanOperationType.BoolUnite, StrapSmoother.Clone() as Solid3d);

            StrapSmoother.CreateFrustum(Width, _params[ParNames.StrapTransformByY], _params[ParNames.StrapTransformByX], _params[ParNames.StrapTransformByY]);
            StrapSmoother.TransformBy(Matrix3d.Rotation(Math.PI / _params[ParNames.StrapTransformByX], new Vector3d(1, 0, 0), Point3d.Origin));
            StrapSmoother.TransformBy(Matrix3d.Displacement(new Vector3d(-Length / 4, 0, 0)));
            StrapWithLocker.BooleanOperation(BooleanOperationType.BoolUnite, StrapSmoother.Clone() as Solid3d);

            //замок ремешка
            Solid3d Locker = new Solid3d();
            Locker.CreateBox(20, Width + _params[ParNames.StrapTransformByX], 4);
            Solid3d LockerHole = new Solid3d();
            LockerHole.CreateBox(18, Width, 4);
            LockerHole.TransformBy(Matrix3d.Displacement(new Vector3d(4, 0, 0)));
            Locker.BooleanOperation(BooleanOperationType.BoolSubtract, LockerHole.Clone() as Solid3d);

            //Дерджатель на замке
            Solid3d LockerPin = new Solid3d();
            LockerPin.CreateBox(20, WatchData.StrapPerforationRadius, _params[ParNames.StrapTransformByX]);
            Locker.BooleanOperation(BooleanOperationType.BoolUnite, LockerPin.Clone() as Solid3d);
           
            //передвигаем ремешок 
            Locker.TransformBy(Matrix3d.Displacement(new Vector3d((-Length / 4)-5, 0, 0)));
            StrapWithLocker.BooleanOperation(BooleanOperationType.BoolUnite, Locker.Clone() as Solid3d);
            StrapWithLocker.TransformBy(Matrix3d.Displacement(new Vector3d(-(WatchData.BodyDiameter + WatchData.BootstrapLength + (Length / 4) - 0.5), 0, 0)));

            //возвращаем модель ремешка

            return StrapWithLocker;
        }


        /// <summary>
        /// ремешок с перфорацие
        /// </summary>
        /// <param name="WatchData">информация о часах</param>
        /// <returns>Модель часов</returns>
        public Solid3d BuildStrapWithPerforations(WatchData WatchData)
        {

            //модель ремешка с перфор.
            Solid3d StrapWithPerforations = new Solid3d();
            StrapWithPerforations.CreateBox(Length / 2, Width, 3);

            //сглаживаем углы.
            Solid3d StrapSmoother = new Solid3d();
            StrapSmoother.CreateFrustum(Width, _params[ParNames.StrapTransformByY], _params[ParNames.StrapTransformByY], _params[ParNames.StrapTransformByY]);
            StrapSmoother.TransformBy(Matrix3d.Rotation(Math.PI / 2, new Vector3d(1, 0, 0), Point3d.Origin));
            StrapSmoother.TransformBy(Matrix3d.Displacement(new Vector3d(Length / 4, 0, 0)));
            StrapWithPerforations.BooleanOperation(BooleanOperationType.BoolUnite, StrapSmoother.Clone() as Solid3d);

            StrapSmoother.CreateFrustum(Width, _params[ParNames.StrapTransformByY], _params[ParNames.StrapTransformByY], _params[ParNames.StrapTransformByY]);
            StrapSmoother.TransformBy(Matrix3d.Rotation(Math.PI / 2, new Vector3d(1, 0, 0), Point3d.Origin));
            StrapSmoother.TransformBy(Matrix3d.Displacement(new Vector3d(-Length / 4, 0, 0)));
            StrapWithPerforations.BooleanOperation(BooleanOperationType.BoolUnite, StrapSmoother.Clone() as Solid3d);

            //проделываем перфорации
            Solid3d HoleMaker = new Solid3d();
            for (int i = 0; i < NumberOfPerforations; i++)
            {
                HoleMaker.CreateFrustum(3, PerforationRadius, PerforationRadius, PerforationRadius);
                HoleMaker.TransformBy(Matrix3d.Displacement(new Vector3d((Length / 4 - 10) - (Length/3)/NumberOfPerforations*i, 0, 0)));
                StrapWithPerforations.BooleanOperation(BooleanOperationType.BoolSubtract, HoleMaker.Clone() as Solid3d);
            }

            StrapWithPerforations.TransformBy(Matrix3d.Displacement(new Vector3d(WatchData.BodyDiameter + WatchData.BootstrapLength + (Length / 4) - 0.5, 0, 0)));


            return StrapWithPerforations;
        }


        private void InitParams()
        {
            _params.Add(ParNames.StrapTransformByX, 2);
            _params.Add(ParNames.StrapTransformByY, 1.5);
        }
    }
}
