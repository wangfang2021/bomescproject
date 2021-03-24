using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0713_Logic
    {
        FS0713_DataAccess FS0713_DataAccess;

        public FS0713_Logic()
        {
            FS0713_DataAccess = new FS0713_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0713_DataAccess.SearchSupplier();
        }



        #region 消耗计算
        public DataTable SearchCalcuate(List<Object> PackSpot, string PackNo, string PackGPSNo, List<Object> strSupplierCode, string strRatio, string StrFrom, string StrTo, string strJiSuanType, string strXHJiSuanType, string strSaveAdvice, string strUserId,ref string strErrorPartId)
        {
            DataTable dt = new DataTable();
            DataTable dtendTime = new DataTable();
            //判断结点时间
            DataTable dtCalcuate = new DataTable();
            if (strJiSuanType == "日" || strJiSuanType == "班值")
            {
                dtCalcuate = FS0713_DataAccess.SearchEndTime(PackSpot, StrFrom, StrTo, strJiSuanType, strSupplierCode, PackGPSNo, PackNo);
            }
            else
            {
                dtendTime = FS0713_DataAccess.SearchSaveDate();
                dtCalcuate = FS0713_DataAccess.CalcuateSaveDate(PackSpot, strSupplierCode, PackGPSNo, PackNo, StrFrom, StrTo, dtendTime);

            }
            //消耗结果
            dt=FS0713_DataAccess.Calcuate(PackSpot, strRatio, strJiSuanType, StrFrom, StrTo, strXHJiSuanType, dtCalcuate, dtendTime);
            //计算后更新安全在库表
            FS0713_DataAccess.UpData(dt,strUserId,ref strErrorPartId, strXHJiSuanType);

            return dt;
        }
        #endregion


        #region 建议在库计算
        public DataTable SearchJYZKCalcuate(List<Object> PackSpot, string PackNo, string PackGPSNo, List<Object> strSupplierCode, string strRatio, string StrFrom, string StrTo, string strJiSuanType, string strXHJiSuanType, string strSaveAdvice, string strUserId, ref string strErrorPartId,DataTable dtOldJS,DataTable dtPackBase)
        {
            DataTable dtJY = new DataTable();
            //时段
            DataTable dtendTime = FS0713_DataAccess.SearchSaveDate();
            //计算
            FS0713_DataAccess.JYZKCalcuate(PackSpot, strRatio, strSaveAdvice, StrFrom, StrTo, strXHJiSuanType,ref dtOldJS, dtendTime, dtPackBase,ref strErrorPartId);
            //计算后更新安全在库表
            FS0713_DataAccess.UpData(dtOldJS, strUserId, ref strErrorPartId, strXHJiSuanType);
            //改在库表？？？？

            return dtOldJS;
        }
        #endregion









        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(List<object> PackSpot, string PackNo, string PackGPSNo, List<object> strSupplierCode)
        {
            return FS0713_DataAccess.Search(PackSpot, PackNo, PackGPSNo, strSupplierCode);
        }
        #endregion



        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            FS0713_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0713_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0713_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0713_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd)
        {
            return FS0713_DataAccess.Search_GS(strBegin, strEnd);
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0713_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }

        #endregion

        #region 查找工厂
        public DataTable SearchPackSpot()
        {
            return FS0713_DataAccess.SearchPackSpot();
        }


        #endregion

        #region 取定义时段
        public DataTable SearchSaveDate()
        {
            return FS0713_DataAccess.SearchSaveDate();
        }

        #endregion


        #region 插入定义时段
        public void InsertSaveDate(string dFrom1, string vcIsOrNoKTFrom1, string dTo1, string vcIsOrNoKT1, string dFrom2, string vcIsOrNoKTFrom2, string dTo2, string vcIsOrNoKT2, string dFrom3, string vcIsOrNoKTFrom3, string dTo3, string vcIsOrNoKT3, string dFrom4, string vcIsOrNoKTFrom4, string dTo4, string vcIsOrNoKT4, string dFrom5, string vcIsOrNoKTFrom5, string dTo5, string vcIsOrNoKT5, string dFrom6, string vcIsOrNoKTFrom6, string dTo6, string vcIsOrNoKT6, string dFrom7, string vcIsOrNoKTFrom7, string dTo7, string vcIsOrNoKT7, string dFrom8, string vcIsOrNoKTFrom8, string dTo8, string vcIsOrNoKT8,string strUserId, ref string strErrorName)
        {
            FS0713_DataAccess.InsertSaveDate(dFrom1, vcIsOrNoKTFrom1, dTo1, vcIsOrNoKT1, dFrom2, vcIsOrNoKTFrom2, dTo2, vcIsOrNoKT2, dFrom3, vcIsOrNoKTFrom3, dTo3, vcIsOrNoKT3,
                    dFrom4, vcIsOrNoKTFrom4, dTo4, vcIsOrNoKT4, dFrom5, vcIsOrNoKTFrom5, dTo5, vcIsOrNoKT5, dFrom6, vcIsOrNoKTFrom6, dTo6, vcIsOrNoKT6,
                    dFrom7, vcIsOrNoKTFrom7, dTo7, vcIsOrNoKT7, dFrom8, vcIsOrNoKTFrom8, dTo8, vcIsOrNoKT8, strUserId, ref strErrorName);
        }
        #endregion
    }
}
