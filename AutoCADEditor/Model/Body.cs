using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WatchCAD.Interface;
using WatchCAD.ViewModel;

namespace WatchCAD.Model
{ 
    /// <summary>
    /// Корпус часов
    /// </summary>
    public class Body : Cylinder, IDrawablePart
    {
        /// <summary>
        /// Заводная головка
        /// </summary>
        private Crown Crown { get; set; }

        /// <summary>
        /// Параметры
        /// </summary>
        private enum ParNames
        {
            BodyTransformByX,
            BodyTransformByY,
            BodyTransformByZ,
            BodyTopTransformByX,
            BodyTopTransformByY,
            BodyTopTransformByZ,
            BodyBottomTransformByX,
            BodyBottomTransformByY,
            BodyBottomTransformByZ,
            BodyBottomHeight,
        };

        /// <summary>
        /// Есть ли хронограф
        /// </summary>
        public bool HasChronograph { get; set; }

        /// <summary>
        /// Длина ушек корпуса
        /// </summary>
        public double BootstrapLength { get; set; }

        /// <summary>
        /// Доп параметры часов
        /// </summary>
        private Dictionary<ParNames, double> _params;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Body()
        {
            Crown = new Crown();
            _params = new Dictionary<ParNames, double>();
            InitParams();
        }



        /// <summary>
        /// Построить часы в акаде
        /// </summary>
        /// <param name="WatchData">Параметры часов</param>
        public void Build(WatchData WatchData)
        {
            Diameter = WatchData.BodyDiameter;
            Height = WatchData.BodyHeight;
            Crown.Diameter = WatchData.CrownDiameter;
            Crown.Height = WatchData.CrownHeight;
            HasChronograph = WatchData.HasChronograph;
            BootstrapLength = WatchData.BootstrapLength;
            
            //Строим корпус
            Builder.BuildModelInAcad(BuildBody(WatchData));
            //Строим Стрелки
            Builder.BuildModelInAcad(BuildArrows(WatchData));
            //Цифры на корпусе
            CreateDigits();
            //Заводная головка
            Crown.Build(WatchData);
            
        }


        /// <summary>
        /// Построить модель корпуса
        /// </summary>
        /// <param name="WatchData">параметры часов</param>
        /// <returns>3д модель корпуса</returns>
        private Solid3d BuildBody(WatchData WatchData)
        {
            //корпус часов
            Solid3d Body = new Solid3d();
            Body.CreateFrustum(Height, Diameter, Diameter, Diameter);

            //Верхняя часть корпуса
            Solid3d BodyTop = new Solid3d();
            BodyTop.CreateFrustum(_params[ParNames.BodyTransformByY], Diameter, Diameter, Diameter - _params[ParNames.BodyTransformByZ]);
            BodyTop.TransformBy(Matrix3d.Displacement(new Point3d(0, 0, Height / _params[ParNames.BodyTransformByY] + _params[ParNames.BodyTransformByX]) - Point3d.Origin));
            Body.BooleanOperation(BooleanOperationType.BoolUnite, BodyTop.Clone() as Solid3d);


            //Нижняя часть корпуса
            Solid3d BodyBottom = new Solid3d();
            BodyBottom.CreateFrustum(_params[ParNames.BodyTransformByX], Diameter - _params[ParNames.BodyTopTransformByY], Diameter - _params[ParNames.BodyTopTransformByY], Diameter - _params[ParNames.BodyTransformByY]);
            BodyBottom.TransformBy(Matrix3d.Displacement(new Point3d(0, 0, -(Height / 2 + _params[ParNames.BodyTransformByX]/2)) - Point3d.Origin));
            Body.BooleanOperation(BooleanOperationType.BoolUnite, BodyBottom.Clone() as Solid3d);
            
            //Вырез на корпусе, сверху
            BodyTop = new Solid3d();
            BodyTop.CreateFrustum(_params[ParNames.BodyTransformByY], Diameter - _params[ParNames.BodyBottomTransformByX], Diameter - _params[ParNames.BodyBottomTransformByX], Diameter - _params[ParNames.BodyTopTransformByY]);
            BodyTop.TransformBy(Matrix3d.Displacement(new Point3d(0, 0, Height / _params[ParNames.BodyTransformByY] + _params[ParNames.BodyTransformByX]) - Point3d.Origin));
            Body.BooleanOperation(BooleanOperationType.BoolSubtract, BodyTop.Clone() as Solid3d);


            //ушки на корпусе
            Solid3d Bootstrap = new Solid3d();
            Bootstrap.CreateBox(BootstrapLength * _params[ParNames.BodyTransformByY] + Diameter * _params[ParNames.BodyTransformByY], _params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTopTransformByY]);
            Vector3d BootsrapPosition;
            Solid3d ArcPart = new Solid3d();
            ArcPart.CreateFrustum(_params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTopTransformByY] / 2, _params[ParNames.BodyTopTransformByY] / 2, _params[ParNames.BodyTopTransformByY] / 2);
            ArcPart.TransformBy(Matrix3d.Rotation(Math.PI / 2, new Vector3d(1, 0, 0), Point3d.Origin));
            ArcPart.TransformBy(Matrix3d.Displacement(new Vector3d(Diameter + BootstrapLength, 0, 0)));

            //Держатели на ушках
            Solid3d StrapHolder = new Solid3d();
            StrapHolder.CreateFrustum(WatchData.StrapWidth + _params[ParNames.BodyTopTransformByX], _params[ParNames.BodyTransformByX] / _params[ParNames.BodyTransformByY], _params[ParNames.BodyTransformByX] / _params[ParNames.BodyTransformByY], _params[ParNames.BodyTransformByX] / _params[ParNames.BodyTransformByY]);
            StrapHolder.TransformBy(Matrix3d.Rotation(Math.PI / _params[ParNames.BodyTransformByY], new Vector3d(1, 0, 0), Point3d.Origin));
            StrapHolder.TransformBy(Matrix3d.Displacement(new Vector3d(Diameter + BootstrapLength, -WatchData.StrapWidth / 2 - _params[ParNames.BodyTransformByY], 0)));
            ArcPart.BooleanOperation(BooleanOperationType.BoolUnite, StrapHolder.Clone() as Solid3d);
            Bootstrap.BooleanOperation(BooleanOperationType.BoolUnite, ArcPart.Clone() as Solid3d);

            //сглаженные углы на ушках
            ArcPart.CreateFrustum(_params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTopTransformByY] / _params[ParNames.BodyTransformByY], _params[ParNames.BodyTopTransformByY] / 2, _params[ParNames.BodyTopTransformByY] / 2);
            ArcPart.TransformBy(Matrix3d.Rotation(Math.PI / _params[ParNames.BodyTransformByY], new Vector3d(1, 0, 0), Point3d.Origin));
            ArcPart.TransformBy(Matrix3d.Displacement(new Vector3d(-(Diameter + BootstrapLength), 0, 0)));

            StrapHolder.CreateFrustum(WatchData.StrapWidth + _params[ParNames.BodyTopTransformByX], _params[ParNames.BodyTransformByX] / 2, _params[ParNames.BodyTransformByX] / 2, _params[ParNames.BodyTransformByX] / 2);
            StrapHolder.TransformBy(Matrix3d.Rotation(Math.PI / 2, new Vector3d(1, 0, 0), Point3d.Origin));
            StrapHolder.TransformBy(Matrix3d.Displacement(new Vector3d(-(Diameter + BootstrapLength), -WatchData.StrapWidth / 2 - _params[ParNames.BodyTransformByY], 0)));
            ArcPart.BooleanOperation(BooleanOperationType.BoolUnite, StrapHolder.Clone() as Solid3d);

            Bootstrap.BooleanOperation(BooleanOperationType.BoolUnite, ArcPart.Clone() as Solid3d);
            BootsrapPosition = new Vector3d(0, WatchData.StrapWidth / _params[ParNames.BodyTransformByY] + _params[ParNames.BodyTransformByZ], 0);
            Bootstrap.TransformBy(Matrix3d.Displacement(BootsrapPosition));

            Body.BooleanOperation(BooleanOperationType.BoolUnite, Bootstrap.Clone() as Solid3d);
            Bootstrap= new Solid3d();
            Bootstrap.CreateBox(BootstrapLength * _params[ParNames.BodyTransformByY] + Diameter * _params[ParNames.BodyTransformByY], _params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTopTransformByY]);
            
            ArcPart = new Solid3d();
            ArcPart.CreateFrustum(_params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTopTransformByY] / 2, _params[ParNames.BodyTopTransformByY] / _params[ParNames.BodyTransformByY], _params[ParNames.BodyTopTransformByY] / _params[ParNames.BodyTransformByY]);
            ArcPart.TransformBy(Matrix3d.Rotation(Math.PI / _params[ParNames.BodyTransformByY], new Vector3d(1, 0, 0), Point3d.Origin));
            ArcPart.TransformBy(Matrix3d.Displacement(new Vector3d(Diameter + BootstrapLength, 0, 0)));
            Bootstrap.BooleanOperation(BooleanOperationType.BoolUnite, ArcPart.Clone() as Solid3d);
            ArcPart.CreateFrustum(_params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTopTransformByY] / _params[ParNames.BodyTransformByY], _params[ParNames.BodyTopTransformByY] / 2, _params[ParNames.BodyTopTransformByY] / _params[ParNames.BodyTransformByY]);
            ArcPart.TransformBy(Matrix3d.Rotation(Math.PI / _params[ParNames.BodyTransformByY], new Vector3d(1, 0, 0), Point3d.Origin));
            ArcPart.TransformBy(Matrix3d.Displacement(new Vector3d(-(Diameter + BootstrapLength), 0, 0)));
            Bootstrap.BooleanOperation(BooleanOperationType.BoolUnite, ArcPart.Clone() as Solid3d);
            BootsrapPosition = new Vector3d(0, -(WatchData.StrapWidth / _params[ParNames.BodyTransformByY] + _params[ParNames.BodyTransformByZ]), 0);
            Bootstrap.TransformBy(Matrix3d.Displacement(BootsrapPosition));


            //соеденяем все вместе
            Body.BooleanOperation(BooleanOperationType.BoolUnite, Bootstrap.Clone() as Solid3d);

            //если есть хронограф, то выполняем
            if (HasChronograph)
            {   
                //первая кнопка хронографа
                Solid3d ChronoButtonTop = new Solid3d();
                ChronoButtonTop.CreateFrustum(_params[ParNames.BodyTopTransformByX], _params[ParNames.BodyTopTransformByX], _params[ParNames.BodyTopTransformByX], _params[ParNames.BodyTopTransformByX]);
                
                ChronoButtonTop.TransformBy(Matrix3d.Rotation(Math.PI / 2, new Vector3d(1, 0, 0), Point3d.Origin));
                ChronoButtonTop.TransformBy(Matrix3d.Displacement(new Vector3d(0,Diameter,0)));
                ChronoButtonTop.TransformBy(Matrix3d.Rotation(Math.PI / 6, new Vector3d(0, 0, 1), Point3d.Origin));
                Body.BooleanOperation(BooleanOperationType.BoolUnite, ChronoButtonTop.Clone() as Solid3d);

                //вторая кнопка хронографа
                Solid3d ChronoButtonBottom = new Solid3d();
                ChronoButtonBottom.CreateFrustum(_params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTopTransformByY]);
                ChronoButtonBottom.TransformBy(Matrix3d.Rotation(Math.PI / 2, new Vector3d(1, 0, 0), Point3d.Origin));
                ChronoButtonBottom.TransformBy(Matrix3d.Displacement(new Vector3d(0, Diameter, 0)));
                ChronoButtonBottom.TransformBy(Matrix3d.Rotation(-Math.PI / 6, new Vector3d(0, 0, 1), Point3d.Origin));
                Body.BooleanOperation(BooleanOperationType.BoolUnite, ChronoButtonBottom.Clone() as Solid3d);
            }
          
            return Body;
        }


        /// <summary>
        /// Строим стрелки
        /// </summary>
        /// <param name="WatchData">Информация о часах</param>
        /// <returns>модель стрелок</returns>
        private Solid3d BuildArrows(WatchData WatchData)
        {
            //стрелки
            Solid3d Arrows = new Solid3d();
            //база
            Solid3d Base = new Solid3d();

            //первая часть базы
            Base.CreateFrustum(_params[ParNames.BodyTransformByX] / 2, _params[ParNames.BodyTransformByZ], _params[ParNames.BodyTransformByZ], _params[ParNames.BodyTransformByZ]);
            Base.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, Height / 2 + _params[ParNames.BodyTransformByX] / 4)));
            Arrows.BooleanOperation(BooleanOperationType.BoolUnite, Base.Clone() as Solid3d);
            
            //вторая часть бызы
            Base.CreateFrustum(_params[ParNames.BodyTransformByX] / 2, _params[ParNames.BodyTransformByY], _params[ParNames.BodyTransformByY], _params[ParNames.BodyTransformByY]);
            Base.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, Height / 2 + _params[ParNames.BodyTransformByX] / 4 * 3)));
            Arrows.BooleanOperation(BooleanOperationType.BoolUnite, Base.Clone() as Solid3d);
            
            //часовая стрелка
            Solid3d Arrow = new Solid3d();
            Arrow.CreateBox(Diameter / 2, _params[ParNames.BodyTransformByY], _params[ParNames.BodyTransformByX] / 4);
            Arrow.TransformBy(Matrix3d.Displacement(new Vector3d(-(Diameter / 4), 0, Height / 2 + _params[ParNames.BodyTransformByX] / 4)));
            double TrueHour = System.DateTime.Now.Hour + ((double)System.DateTime.Now.Minute / 60);
            Arrow.TransformBy(Matrix3d.Rotation(-(Math.PI / 6 * TrueHour), new Vector3d(0, 0, 1), Point3d.Origin));
            Arrows.BooleanOperation(BooleanOperationType.BoolUnite, Arrow.Clone() as Solid3d);
            
            //минутная стрелка
            Arrow.CreateBox(Diameter / 2 + _params[ParNames.BodyTopTransformByY], _params[ParNames.BodyTransformByX] + _params[ParNames.BodyTransformByX] / 5, _params[ParNames.BodyTransformByX] / 5);
            Arrow.TransformBy(Matrix3d.Displacement(new Vector3d(-(Diameter / 4), 0, Height / 2 + _params[ParNames.BodyTransformByX] / 4 * 3)));
            Arrow.TransformBy(Matrix3d.Rotation(-(Math.PI / 30 * System.DateTime.Now.Minute), new Vector3d(0, 0, 1), Point3d.Origin));
            Arrows.BooleanOperation(BooleanOperationType.BoolUnite, Arrow.Clone() as Solid3d);
            //секундная стрелка
            Arrow.CreateBox(Diameter / 2 + _params[ParNames.BodyBottomHeight], _params[ParNames.BodyTransformByX] / 2, _params[ParNames.BodyTransformByX] / 5);
            Arrow.TransformBy(Matrix3d.Displacement(new Vector3d(-(Diameter / _params[ParNames.BodyTopTransformByX]), 0, Height / 2 + _params[ParNames.BodyTransformByX] + _params[ParNames.BodyTransformByX]/10)));
            Arrow.TransformBy(Matrix3d.Rotation(-(Math.PI / 30 * System.DateTime.Now.Second), new Vector3d(0, 0, 1), Point3d.Origin));
            Arrows.BooleanOperation(BooleanOperationType.BoolUnite, Arrow.Clone() as Solid3d);

            return Arrows;
        }

        /// <summary>
        ///Добавляем цифры на корпус
        /// </summary>
        private void CreateDigits()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec; acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                List<DBText> acText = new List<DBText>();
                acText.Add(new DBText());
                acText.Add(new DBText());
                acText.Add(new DBText());
                acText.Add(new DBText());

                //3
                acText[0].SetDatabaseDefaults();
                acText[0].Position = new Point3d(_params[ParNames.BodyTransformByX], Diameter - _params[ParNames.BodyTopTransformByY] - _params[ParNames.BodyBottomHeight], Height / _params[ParNames.BodyTransformByY] + _params[ParNames.BodyTransformByX] / _params[ParNames.BodyBottomHeight]);
                acText[0].Rotation = Math.PI / _params[ParNames.BodyTransformByY];
                acText[0].Height = _params[ParNames.BodyTopTransformByX];
                acText[0].TextString = "3";


                //6
                acText[1].SetDatabaseDefaults();
                acText[1].Position = new Point3d(Diameter - _params[ParNames.BodyBottomHeight], -1, Height / _params[ParNames.BodyTransformByY] + _params[ParNames.BodyTransformByX] / _params[ParNames.BodyBottomHeight]);
                acText[1].Rotation = Math.PI / _params[ParNames.BodyTransformByY];
                acText[1].Height = _params[ParNames.BodyTopTransformByX];
                acText[1].TextString = "6";


                //9
                acText[2].SetDatabaseDefaults();
                acText[2].Position = new Point3d(_params[ParNames.BodyTransformByX], -Diameter + _params[ParNames.BodyBottomHeight], Height / _params[ParNames.BodyTransformByY] + _params[ParNames.BodyTransformByX] / _params[ParNames.BodyBottomHeight]);
                acText[2].Rotation = Math.PI / _params[ParNames.BodyTransformByY];
                acText[2].Height = _params[ParNames.BodyTopTransformByX];
                acText[2].TextString = "9";

                //12
                acText[3].SetDatabaseDefaults();
                acText[3].Position = new Point3d(-Diameter + _params[ParNames.BodyBottomHeight] + _params[ParNames.BodyTopTransformByY], -_params[ParNames.BodyTransformByY], Height / _params[ParNames.BodyTransformByY] + _params[ParNames.BodyTransformByX] / _params[ParNames.BodyBottomHeight]);
                acText[3].Rotation = Math.PI / _params[ParNames.BodyTransformByY];
                acText[3].Height = _params[ParNames.BodyTopTransformByX];
                acText[3].TextString = "12";

                foreach (var item in acText)
                {
                    acBlkTblRec.AppendEntity(item);
                    acTrans.AddNewlyCreatedDBObject(item, true);

                }
                acTrans.Commit();
            }
        }


        /// <summary>
        /// Инициализировать параметры
        /// </summary>
        private void InitParams()
        {               
            _params.Add(ParNames.BodyTransformByX, 1);
            _params.Add(ParNames.BodyTransformByY, 2);
            _params.Add(ParNames.BodyTransformByZ, 3);
            _params.Add(ParNames.BodyTopTransformByX, 4);
            _params.Add(ParNames.BodyTopTransformByY, 5);
            _params.Add(ParNames.BodyTopTransformByZ, 6);
            _params.Add(ParNames.BodyBottomTransformByX, 7);
            _params.Add(ParNames.BodyBottomTransformByY, 8);
            _params.Add(ParNames.BodyBottomTransformByZ, 9);
            _params.Add(ParNames.BodyBottomHeight, 10);
        }
    }
}