using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;

namespace DataAccess
{ 
    public class FS0303_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strIsShowAll)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select *,'0' as vcModFlag,'0' as vcAddFlag,'0' as selected from TUnit         \n");
                strSql.Append("     where     \n");
                strSql.Append("     1=1    \n");
                if (string.IsNullOrEmpty(strIsShowAll) || strIsShowAll=="0")
                {
                    strSql.Append("     and dTimeFrom <= GETDATE()    \n");
                    strSql.Append("     and dTimeTo >= GETDATE()    \n");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("      INSERT INTO  TUnit       \n");
                        sql.Append("      (       \n");
                        sql.Append("      dSyncTime , vcSPINo , vcSQState , vcDiff , vcPart_id , vcCarTypeDev       \n");
                        sql.Append("      , vcCarTypeDesign , vcCarTypeName , dTimeFrom , dTimeTo , vcBJDiff , vcPartNameEn       \n");
                        sql.Append("      , vcPartNameCn , vcHKGC , vcBJGC , vcInOutflag , vcSupplier_id , vcSupplier_Name       \n");
                        sql.Append("      , vcSCPlace , vcCHPlace , vcSYTCode , vcSCSName , vcSCSAdress , dGYSTimeFrom       \n");
                        sql.Append("      , dGYSTimeTo , vcOE , vcHaoJiu , vcMeno , vcFXDiff , vcFXNo       \n");
                        sql.Append("      , vcReceiver , vcOriginCompany , vcOperator,dOperatorTime      \n");
                        sql.Append("      )      \n");
                        sql.Append("      VALUES      \n");
                        sql.Append("      (      \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dSyncTime"],true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSQState"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeName"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeTo"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartNameEn"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHKGC"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcBJGC"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSCPlace"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSCSAdress"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dGYSTimeFrom"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dGYSTimeTo"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcMeno"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + ",   \r\n");
                        sql.Append("'"+strUserId+"'     \r\n");
                        sql.Append("'GETDATE()'     \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("      update TUnit set       \r\n");
                        sql.Append("      vcSQState = "+ComFunction.getSqlValue(listInfoData[i]["vcSQState"], false) +"      \r\n");
                        sql.Append("      ,vcDiff = "+ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) +"      \r\n");
                        sql.Append("      ,vcCarTypeDesign = " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "      \r\n");
                        sql.Append("      ,vcHKGC = " + ComFunction.getSqlValue(listInfoData[i]["vcHKGC"], false) + "      \r\n");
                        sql.Append("      ,vcBJGC = " + ComFunction.getSqlValue(listInfoData[i]["vcBJGC"], false) + "      \r\n");
                        sql.Append("      ,vcInOutflag = " + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "      \r\n");
                        sql.Append("      ,vcOE = " + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "      \r\n");
                        sql.Append("      ,vcHaoJiu = " + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + "      \r\n");
                        if (listInfoData[i]["vcHaoJiu"].ToString()=="旧型")
                        {
                            sql.Append("      ,dJiuBegin = " + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false) + "      \r\n");
                        }
                        sql.Append("      ,vcMeno = " + ComFunction.getSqlValue(listInfoData[i]["vcMeno"], false) + "      \r\n");
                        sql.Append("      ,vcReceiver = " + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "      \r\n");
                        sql.Append("      ,vcOperator = " + strUserId + "      \r\n");
                        sql.Append("      ,dOperatorTime = GETDATE()      \r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TPrice where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
