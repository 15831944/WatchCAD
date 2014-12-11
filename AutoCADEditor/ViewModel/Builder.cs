using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchCAD.ViewModel
{
    class Builder
    {

        /// <summary>
        /// построить модель в автокаде
        /// </summary>
        /// <param name="SolidModel">Солид3Д - Модель</param>
        public static void BuildModelInAcad(Solid3d SolidModel)
        { 
            //открываем докуемент
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            //БД документа
            Database acCurDb = acDoc.Database;
            //блокируем документ от других записей 
            using (acDoc.LockDocument())
            {
                //создаем транзакцию
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    //новое таблица в БД
                    BlockTable acBlkTbl;
                    //открываем для записи
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    //новое поле в бд
                    BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    //добавляем запись в поле
                    acBlkTblRec.AppendEntity(SolidModel);
                    
                    //добавляем в транзакцию
                    acTrans.AddNewlyCreatedDBObject(SolidModel, true);
                    
                    //комитим
                    acTrans.Commit();
                }
            }
        }
    }
}
