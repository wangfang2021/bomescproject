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
    public class FS0304_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strJD, string strInOutFlag,string strSupplier_id,string strCarType,string strPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select   \n");
                strSql.Append("   '0' as vcModFlag,      ");
                strSql.Append("   '0' as vcAddFlag,      ");
                strSql.Append("   iAutoId,  \n");
                strSql.Append("   dSSDateMonth,  \n");
                strSql.Append("   vcJD,				  \n");
                strSql.Append("   vcPart_id,			  \n");
                strSql.Append("   vcSPINo,				  \n");
                strSql.Append("   vcChange,				  \n");
                strSql.Append("   vcCarType,				  \n");
                strSql.Append("   vcInOutflag,			  \n");
                strSql.Append("   vcPartName,			  \n");
                strSql.Append("   vcOE,					  \n");
                strSql.Append("   vcSupplier_id,			  \n");
                strSql.Append("   vcFXDiff,				  \n");
                strSql.Append("   vcFXNo,				  \n");
                strSql.Append("   vcSumLater,			  \n");
                strSql.Append("   vcIsDY,	  \n");
                strSql.Append("   vcYQorNG,				  \n");
                strSql.Append("   vcSCPlace_City,		  \n");
                strSql.Append("   vcSCPlace_Province,	  \n");
                strSql.Append("   vcCHPlace_City,		  \n");
                strSql.Append("   vcCHPlace_Province,	  \n");
                strSql.Append("   vcPackFactory,		  \n");
                strSql.Append("   vcSCSPlace,			  \n");
                strSql.Append("   dSupplier_BJ,			  \n");
                strSql.Append("   dSupplier_HK,			  \n");
                strSql.Append("   dTFTM_BJ,				  \n");
                strSql.Append("   vcZXBZDiff,			  \n");
                strSql.Append("   vcZXBZNo  \n");
                strSql.Append("   from tSQ  \n");
                strSql.Append("   where 1=1  \n");
                if (!string.IsNullOrEmpty(strJD))
                {
                    strSql.Append("      and vcJD = '" + strJD + "'  ");
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.Append("      and vcInOutflag = '" + strInOutFlag + "'   ");
                }
                if (!string.IsNullOrEmpty(strSupplier_id))
                {
                    strSql.Append("      and vcSupplier_id = '" + strSupplier_id + "'   ");
                }
                if (!string.IsNullOrEmpty(strCarType))
                {
                    strSql.Append("      and vcCarType = '" + strCarType + "'   ");
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
                        sql.Append("      insert into TPart_JX      \n");
                        sql.Append("      (      \n");
                        sql.Append("      vcPart_id,        \n");
                        sql.Append("      vcHaoJiu,         \n");
                        sql.Append("      dJiuBegin,        \n");
                        sql.Append("      vcSupplier_id,    \n");
                        sql.Append("      vcSupplier_Name,  \n");
                        sql.Append("      vcCarTypeDesign,  \n");
                        sql.Append("      vcPartName,       \n");
                        sql.Append("      vcSumLater,       \n");
                        sql.Append("      vcInput_No,       \n");
                        sql.Append("      dSendTime,        \n");
                        sql.Append("      vcOperatorID,     \n");
                        sql.Append("      dOperatorTime     \n");
                        sql.Append("      )       \n");
                        sql.Append("      values      \n");
                        sql.Append("      (      \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + ",     \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false) + ",    \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",    \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + ",  \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + ",  \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcInput_No"], false) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dSendTime"], false) + ",    \n");
                        sql.Append("'" + strUserId + "',     \r\n");
                        sql.Append("    GETDATE()     \r\n");
                        sql.Append("      )      \n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("      update TPart_JX set       \r\n");
                        sql.Append("      vcPart_id = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "      \r\n");
                        sql.Append("      ,vcHaoJiu = " + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + "      \r\n");
                        sql.Append("      ,dJiuBegin = " + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false) + "      \r\n");
                        sql.Append("      ,vcSupplier_id = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "      \r\n");
                        sql.Append("      ,vcSupplier_Name = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + "      \r\n");
                        sql.Append("      ,vcCarTypeDesign = " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "      \r\n");
                        sql.Append("      ,vcPartName = " + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + "      \r\n");
                        sql.Append("      ,vcSumLater = " + ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + "      \r\n");
                        sql.Append("      ,vcInput_No = " + ComFunction.getSqlValue(listInfoData[i]["vcInput_No"], false) + "      \r\n");
                        sql.Append("      ,dSendTime = " + ComFunction.getSqlValue(listInfoData[i]["dSendTime"], false) + "      \r\n");
                        sql.Append("      ,vcOperatorID = " + strUserId + "      \r\n");
                        sql.Append("      ,dOperatorTime = getdate()      \r\n");
                        sql.Append("      where iAutoId = " + iAutoId + "      \r\n");
                    }
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
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

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcPart_id = dt.Rows[i]["vcPart_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPart_id"].ToString();
                    string vcHaoJiu = dt.Rows[i]["vcHaoJiu"] == System.DBNull.Value ? "" : dt.Rows[i]["vcHaoJiu"].ToString();
                    string vcCarTypeDesign = dt.Rows[i]["vcCarTypeDesign"] == System.DBNull.Value ? "" : dt.Rows[i]["vcCarTypeDesign"].ToString();

                    sql.Append("  update TPart_JX set    \r\n");
                    sql.Append("  dJiuBegin=" + ComFunction.getSqlValue(dt.Rows[i]["dJiuBegin"], true) + "   \r\n");
                    sql.Append("  ,vcSupplier_id=" + ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_id"], false) + "   \r\n");
                    sql.Append("  ,vcSupplier_Name=" + ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_Name"], false) + "   \r\n");
                    sql.Append("  ,vcPartName=" + ComFunction.getSqlValue(dt.Rows[i]["vcPartName"], false) + "   \r\n");
                    sql.Append("  ,vcSumLater=" + ComFunction.getSqlValue(dt.Rows[i]["vcSumLater"], true) + "   \r\n");
                    sql.Append("  ,vcInput_No=" + ComFunction.getSqlValue(dt.Rows[i]["vcInput_No"], false) + "   \r\n");
                    sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                    sql.Append("  ,dOperatorTime=getdate()   \r\n");
                    sql.Append("  where vcPart_id='" + vcPart_id + "'  and vcHaoJiu='" + vcHaoJiu + "' and  vcCarTypeDesign='" + vcCarTypeDesign + "' ; \r\n");

                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
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
                sql.Append("  delete TPart_JX where iAutoId in(   \r\n ");
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
