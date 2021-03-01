using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0702_Logic
    {
        FS0702_DataAccess FS0702_DataAccess;

        public FS0702_Logic()
        {
            FS0702_DataAccess = new FS0702_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0702_DataAccess.SearchSupplier();
        }

        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string Note, string PackSpot, string Shouhuofang, string Pinfan, string Car, string PackNO, string PackGPSNo, string dtFromBegin, string dtFromEnd, string dtToBegin, string dtToEnd)
        {
            return FS0702_DataAccess.Search( Note,  PackSpot,  Shouhuofang,  Pinfan,  Car,  PackNO,  PackGPSNo,  dtFromBegin,  dtFromEnd,  dtToBegin,  dtToEnd);
        }
        #endregion

        #region 形成纵向导出


        public DataTable SearchEXZ(string iautoID, string strNote, string strPackSpot, string strShouhuofang, string strPartsNo, string strCar, string strPackNO, string strPackGPSNo, string strFromBegin, string strFromEnd, string strToBegin, string strToEnd)
        {
            return FS0702_DataAccess.SearchEXZ(iautoID, strNote, strPackSpot, strShouhuofang, strPartsNo, strCar, strPackNO, strPackGPSNo, strFromBegin, strFromEnd, strFromEnd, strToBegin);
        }
        #endregion





        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId,DataTable dt,DataTable dt1)
        {
            FS0702_DataAccess.Save(listInfoData, strUserId,ref strErrorPartId,dt,dt1);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId,DataTable dtPackBase,DataTable dtPackitem)
        {
            FS0702_DataAccess.importSave(dt, strUserId, dtPackBase, dtPackitem);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0702_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0702_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd )
        {
            return FS0702_DataAccess.Search_GS(strBegin, strEnd );
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0702_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }
        #endregion

        #region 检查品番是否含有
        public bool CheckPartsNo(string strShouhuofang, string strPartsNo)
        {
           return FS0702_DataAccess.CheckPartsNo(strShouhuofang, strPartsNo);
        }

        #endregion

    }
}
