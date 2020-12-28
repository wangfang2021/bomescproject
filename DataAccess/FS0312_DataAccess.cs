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
    public class FS0312_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strPart_id, string strSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select *,'0' as vcModFlag,'0' as vcAddFlag from TPart_JX         \n");
                strSql.Append("      where 1=1   ");
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.Append("      and vcPart_id = '"+strPart_id+"'  ");
                }
                if (!string.IsNullOrEmpty(strSupplier_id))
                {
                    strSql.Append("      and vcSupplier_id = '"+strSupplier_id+"'   ");
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
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false)+",   \n");
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
                        sql.Append("      vcPart_id = "+ComFunction.getSqlValue(listInfoData[i]["vcPart_id"],false)+"      \r\n");
                        sql.Append("      ,vcHaoJiu = " + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + "      \r\n");
                        sql.Append("      ,dJiuBegin = " + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false) + "      \r\n");
                        sql.Append("      ,vcSupplier_id = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "      \r\n");
                        sql.Append("      ,vcSupplier_Name = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + "      \r\n");
                        sql.Append("      ,vcCarTypeDesign = " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "      \r\n");
                        sql.Append("      ,vcPartName = " + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + "      \r\n");
                        sql.Append("      ,vcSumLater = " + ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + "      \r\n");
                        sql.Append("      ,vcInput_No = " + ComFunction.getSqlValue(listInfoData[i]["vcInput_No"], false) + "      \r\n");
                        sql.Append("      ,dSendTime = " + ComFunction.getSqlValue(listInfoData[i]["dSendTime"], false) + "      \r\n");
                        sql.Append("      ,vcOperatorID = " + strUserId+ "      \r\n");
                        sql.Append("      ,dOperatorTime = getdate()      \r\n");
                        sql.Append("      where iAutoId = "+iAutoId+"      \r\n");
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
                decimal decPriceXS = Convert.ToDecimal(ComFunction.getTCode("C008").Rows[0]["vcValue"]);//价格系数
                

                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcPart_id = dt.Rows[i]["vcPart_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPart_id"].ToString();
                    string dUseBegin = dt.Rows[i]["dUseBegin"] == System.DBNull.Value ? "" : dt.Rows[i]["dUseBegin"].ToString();
                    string dUseEnd = dt.Rows[i]["dUseEnd"] == System.DBNull.Value ? "" : dt.Rows[i]["dUseEnd"].ToString();

                    sql.Append("  update TPrice set    \r\n");
                    sql.Append("  vcPriceChangeInfo=" + ComFunction.getSqlValue(dt.Rows[i]["vcPriceChangeInfo"], false) + "   \r\n");
                    sql.Append("  ,vcPriceGS=" + ComFunction.getSqlValue(dt.Rows[i]["vcPriceGS"], false) + "   \r\n");
                    sql.Append("  ,decPriceOrigin=" + ComFunction.getSqlValue(dt.Rows[i]["decPriceOrigin"], false) + "   \r\n");


                    //以下两个字段计算
                    if (dt.Rows[i]["decPriceOrigin"] == System.DBNull.Value)
                        sql.Append("  ,decPriceAfter=null   \r\n");
                    else
                        sql.Append("  ,decPriceAfter=" + dt.Rows[i]["decPriceOrigin"].ToString() + "*" + decPriceXS + "   \r\n");
                    


                    sql.Append("  ,dPricebegin=" + ComFunction.getSqlValue(dt.Rows[i]["dPricebegin"], true) + "   \r\n");
                    sql.Append("  ,dPriceEnd=" + ComFunction.getSqlValue(dt.Rows[i]["dPriceEnd"], true) + "   \r\n");
                    sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                    sql.Append("  ,dOperatorTime=getdate()   \r\n");
                    sql.Append("  where vcPart_id='" + vcPart_id + "'  and dUseBegin='" + dUseBegin + "' and  dUseEnd='" + dUseEnd + "' ; \r\n");

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
