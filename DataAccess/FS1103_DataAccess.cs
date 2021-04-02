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
    public class FS1103_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strReceiver, string strSupplier, string strInPutOrderNo, string strPartId, string strLianFan)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT A.vcCpdcompany as vcReceiver,a.vcPart_Id as vcPartId,a.vcInno as vcInPutOrderNo,");
                strSql.AppendLine("	        cast(a.vcInputnum as int)/cast(vcPackingquantity as int) as vcLabelNum,");
                strSql.AppendLine("	        '0' as bInPutOrder,'0' as bTag,b.vcTagLianFFrom,b.vcTagLianFTo FROM ");
                strSql.AppendLine("(select * from TInvList where 1=1");
                if (strReceiver != "")
                {
                    strSql.AppendLine("AND vcCpdcompany='" + strReceiver + "'");
                }
                if (strSupplier != "")
                {
                    strSql.AppendLine("--AND 供应商");
                }
                if (strInPutOrderNo != "")
                {
                    strSql.AppendLine("AND vcInno='" + strInPutOrderNo + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("AND vcPart_Id='" + strPartId + "'");
                }
                strSql.AppendLine(")A");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(select vcInno,vcPart_id,vcGetnum,MIN((vcPrintcount)) AS vcTagLianFFrom ,MAX((vcPrintcount)) AS vcTagLianFTo from TLabelList");
                strSql.AppendLine("GROUP BY vcInno,vcPart_id,vcGetnum)B");
                strSql.AppendLine("ON A.vcInno=B.vcInno AND A.vcPart_Id=B.vcPart_id");
                strSql.AppendLine("WHERE 1=1");
                if (strLianFan != "")
                {
                    strSql.AppendLine("AND B.vcTagLianFFrom<='" + strLianFan + "' AND B.vcTagLianFTo>='" + strLianFan + "'");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getPrintTemp(string strPage)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select top(1)* from [tPrintTemp_" + strPage + "]");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strInPutOrderNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select * from TLabelList where vcInno='"+ strInPutOrderNo + "' order by vcPrintcount");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setPrintTemp(DataTable dtInputTemp, DataTable dtTagTemp, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                #region sqlCommand_deleteinfo
                SqlCommand sqlCommand_deleteinfo = sqlConnection.CreateCommand();
                sqlCommand_deleteinfo.Transaction = sqlTransaction;
                sqlCommand_deleteinfo.CommandType = CommandType.Text;
                StringBuilder strSql_deleteinfo = new StringBuilder();
                #region SQL and Parameters
                strSql_deleteinfo.AppendLine("DELETE from tPrintTemp_input_FS1103 where vcOperatorID='" + strOperId + "'");
                strSql_deleteinfo.AppendLine("DELETE from tPrintTemp_tag_FS1103 where vcOperatorID='" + strOperId + "'");
                sqlCommand_deleteinfo.CommandText = strSql_deleteinfo.ToString();
                #endregion
                sqlCommand_deleteinfo.ExecuteNonQuery();
                #endregion

                #region sqlCommand_input
                SqlCommand sqlCommand_input = sqlConnection.CreateCommand();
                sqlCommand_input.Transaction = sqlTransaction;
                sqlCommand_input.CommandType = CommandType.Text;
                StringBuilder strSql_input = new StringBuilder();

                #region SQL and Parameters
                #region tPrintTemp_input_FS1103
                strSql_input.AppendLine("INSERT INTO [dbo].[tPrintTemp_input_FS1103]");
                strSql_input.AppendLine("           ([vcNo]");
                strSql_input.AppendLine("           ,[vcData]");
                strSql_input.AppendLine("           ,[vcPrintdate]");
                strSql_input.AppendLine("           ,[vcInno]");
                strSql_input.AppendLine("           ,[vcPart_Id]");
                strSql_input.AppendLine("           ,[vcPartsnamechn]");
                strSql_input.AppendLine("           ,[vcPartslocation]");
                strSql_input.AppendLine("           ,[vcInputnum]");
                strSql_input.AppendLine("           ,[vcPackingquantity]");
                strSql_input.AppendLine("           ,[vcItemname1]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation1]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn1]");
                strSql_input.AppendLine("           ,[vcOutnum1]");
                strSql_input.AppendLine("           ,[vcTemname2]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation2]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn2]");
                strSql_input.AppendLine("           ,[vcOutnum2]");
                strSql_input.AppendLine("           ,[vcItemname3]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation3]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn3]");
                strSql_input.AppendLine("           ,[vcOutnum3]");
                strSql_input.AppendLine("           ,[vcItemname4]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation4]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn4]");
                strSql_input.AppendLine("           ,[vcOutnum4]");
                strSql_input.AppendLine("           ,[vcItemname5]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation5]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn5]");
                strSql_input.AppendLine("           ,[vcOutnum5]");
                strSql_input.AppendLine("           ,[vcItemname6]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation6]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn6]");
                strSql_input.AppendLine("           ,[vcOutnum6]");
                strSql_input.AppendLine("           ,[vcItemname7]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation7]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn7]");
                strSql_input.AppendLine("           ,[vcOutnum7]");
                strSql_input.AppendLine("           ,[vcItemname8]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation8]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn8]");
                strSql_input.AppendLine("           ,[vcOutnum8]");
                strSql_input.AppendLine("           ,[vcItemname9]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation9]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn9]");
                strSql_input.AppendLine("           ,[vcOutnum9]");
                strSql_input.AppendLine("           ,[vcItemname10]");
                strSql_input.AppendLine("           ,[vcPackingpartslocation10]");
                strSql_input.AppendLine("           ,[vcSuppliernamechn10]");
                strSql_input.AppendLine("           ,[vcOutnum10]");
                strSql_input.AppendLine("           ,[vcPartsnoandnum]");
                strSql_input.AppendLine("           ,[vcLabel]");
                strSql_input.AppendLine("           ,[vcComputernm]");
                strSql_input.AppendLine("           ,[vcCpdcompany]");
                strSql_input.AppendLine("           ,[vcPlantcode]");
                strSql_input.AppendLine("           ,[vcCompanyname]");
                strSql_input.AppendLine("           ,[vcPlantname]");
                strSql_input.AppendLine("           ,[iQrcode]");
                strSql_input.AppendLine("           ,[vcOperatorID]");
                strSql_input.AppendLine("           ,[dOperatorTime])");
                strSql_input.AppendLine("    SELECT [vcNo]");
                strSql_input.AppendLine("      ,[vcData]");
                strSql_input.AppendLine("      ,[vcPrintdate]");
                strSql_input.AppendLine("      ,[vcInno]");
                strSql_input.AppendLine("      ,[vcPart_Id]");
                strSql_input.AppendLine("      ,[vcPartsnamechn]");
                strSql_input.AppendLine("      ,[vcPartslocation]");
                strSql_input.AppendLine("      ,[vcInputnum]");
                strSql_input.AppendLine("      ,[vcPackingquantity]");
                strSql_input.AppendLine("      ,[vcItemname1]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation1]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn1]");
                strSql_input.AppendLine("      ,[vcOutnum1]");
                strSql_input.AppendLine("      ,[vcTemname2]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation2]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn2]");
                strSql_input.AppendLine("      ,[vcOutnum2]");
                strSql_input.AppendLine("      ,[vcItemname3]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation3]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn3]");
                strSql_input.AppendLine("      ,[vcOutnum3]");
                strSql_input.AppendLine("      ,[vcItemname4]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation4]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn4]");
                strSql_input.AppendLine("      ,[vcOutnum4]");
                strSql_input.AppendLine("      ,[vcItemname5]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation5]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn5]");
                strSql_input.AppendLine("      ,[vcOutnum5]");
                strSql_input.AppendLine("      ,[vcItemname6]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation6]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn6]");
                strSql_input.AppendLine("      ,[vcOutnum6]");
                strSql_input.AppendLine("      ,[vcItemname7]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation7]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn7]");
                strSql_input.AppendLine("      ,[vcOutnum7]");
                strSql_input.AppendLine("      ,[vcItemname8]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation8]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn8]");
                strSql_input.AppendLine("      ,[vcOutnum8]");
                strSql_input.AppendLine("      ,[vcItemname9]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation9]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn9]");
                strSql_input.AppendLine("      ,[vcOutnum9]");
                strSql_input.AppendLine("      ,[vcItemname10]");
                strSql_input.AppendLine("      ,[vcPackingpartslocation10]");
                strSql_input.AppendLine("      ,[vcSuppliernamechn10]");
                strSql_input.AppendLine("      ,[vcOutnum10]");
                strSql_input.AppendLine("      ,[vcPartsnoandnum]");
                strSql_input.AppendLine("      ,[vcLabel]");
                strSql_input.AppendLine("      ,[vcComputernm]");
                strSql_input.AppendLine("      ,[vcCpdcompany]");
                strSql_input.AppendLine("      ,[vcPlantcode]");
                strSql_input.AppendLine("      ,[vcCompanyname]");
                strSql_input.AppendLine("      ,[vcPlantname]");
                strSql_input.AppendLine("      ,[iQrcode]");
                strSql_input.AppendLine("      ,'"+strOperId+"'");
                strSql_input.AppendLine("      ,GETDATE()");
                strSql_input.AppendLine("  FROM [dbo].[TInvList] WHERE vcInno=@vcInno");
                #endregion

                sqlCommand_input.CommandText = strSql_input.ToString();
                sqlCommand_input.Parameters.AddWithValue("@vcInno", "");
                #endregion
                foreach (DataRow item in dtInputTemp.Rows)
                {
                    #region Value
                    sqlCommand_input.Parameters["@vcInno"].Value = item["vcInno"].ToString();
                    #endregion
                    sqlCommand_input.ExecuteNonQuery();
                }
                #endregion

                #region sqlCommand_tag
                SqlCommand sqlCommand_tag = sqlConnection.CreateCommand();
                sqlCommand_tag.Transaction = sqlTransaction;
                sqlCommand_tag.CommandType = CommandType.Text;
                StringBuilder strSql_tag = new StringBuilder();

                #region SQL and Parameters
                #region tPrintTemp_tag_FS1103
                strSql_tag.AppendLine("INSERT INTO [dbo].[tPrintTemp_tag_FS1103]");
                strSql_tag.AppendLine("           ([vcPartsnameen]");
                strSql_tag.AppendLine("           ,[vcPart_id]");
                strSql_tag.AppendLine("           ,[vcInno]");
                strSql_tag.AppendLine("           ,[vcCpdcompany]");
                strSql_tag.AppendLine("           ,[vcLabel]");
                strSql_tag.AppendLine("           ,[vcGetnum]");
                strSql_tag.AppendLine("           ,[iDateprintflg]");
                strSql_tag.AppendLine("           ,[vcComputernm]");
                strSql_tag.AppendLine("           ,[vcPrindate]");
                strSql_tag.AppendLine("           ,[iQrcode]");
                strSql_tag.AppendLine("           ,[vcPrintcount]");
                strSql_tag.AppendLine("           ,[vcPartnamechineese]");
                strSql_tag.AppendLine("           ,[vcSuppliername]");
                strSql_tag.AppendLine("           ,[vcSupplieraddress]");
                strSql_tag.AppendLine("           ,[vcExecutestandard]");
                strSql_tag.AppendLine("           ,[vcCartype]");
                strSql_tag.AppendLine("           ,[vcHostip]");
                strSql_tag.AppendLine("           ,[vcOperatorID]");
                strSql_tag.AppendLine("           ,[dOperatorTime])");
                strSql_tag.AppendLine("  SELECT [vcPartsnameen]");
                strSql_tag.AppendLine("      ,[vcPart_id]");
                strSql_tag.AppendLine("      ,[vcInno]");
                strSql_tag.AppendLine("      ,[vcCpdcompany]");
                strSql_tag.AppendLine("      ,[vcLabel1]");
                strSql_tag.AppendLine("      ,[vcGetnum]");
                strSql_tag.AppendLine("      ,[iDateprintflg]");
                strSql_tag.AppendLine("      ,[vcComputernm]");
                strSql_tag.AppendLine("      ,[vcPrindate]");
                strSql_tag.AppendLine("      ,[iQrcode1]");
                strSql_tag.AppendLine("      ,[vcPrintcount1]");
                strSql_tag.AppendLine("      ,[vcPartnamechineese]");
                strSql_tag.AppendLine("      ,[vcSuppliername]");
                strSql_tag.AppendLine("      ,[vcSupplieraddress]");
                strSql_tag.AppendLine("      ,[vcExecutestandard]");
                strSql_tag.AppendLine("      ,[vcCartype]");
                strSql_tag.AppendLine("      ,[vcHostip]");
                strSql_tag.AppendLine("      ,'" + strOperId + "'");
                strSql_tag.AppendLine("      ,GETDATE()");
                strSql_tag.AppendLine("  FROM [dbo].[TLabelList] where [iAutoId]=@iAutoId");
                #endregion

                sqlCommand_tag.CommandText = strSql_tag.ToString();
                sqlCommand_tag.Parameters.AddWithValue("@iAutoId", "");
                #endregion
                foreach (DataRow item in dtTagTemp.Rows)
                {
                    #region Value
                    sqlCommand_tag.Parameters["@iAutoId"].Value = item["iAutoId"].ToString();
                    #endregion
                    sqlCommand_tag.ExecuteNonQuery();
                }
                #endregion
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                #endregion

            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入打印数据失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }

        public DataTable getTempInfo(string strOperId, string strType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                if (strType == "input")
                {
                    strSql.AppendLine("SELECT * from tPrintTemp_input_FS1103 where vcOperatorID='" + strOperId + "'");
                }
                if (strType == "tag")
                {
                    strSql.AppendLine("SELECT * from tPrintTemp_tag_FS1103 where vcOperatorID='" + strOperId + "'");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
