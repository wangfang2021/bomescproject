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
    public class FS0626_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        /// <summary>
        /// 验证新增的数据是否已经存在数据库
        /// </summary>
        /// <param name="dtadd"></param>
        /// <returns></returns>
        public bool isExistAddData(DataTable dtadd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all select iAutoId, vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcOperatorID, dOperatorTime from [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and  vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "' ");
                    }
                    else
                    {
                        strSql.AppendLine("  select iAutoId, vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcOperatorID, dOperatorTime from [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and  vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
                    }
                }
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Search(string vcInjectionFactory, string vcTargetMonth, string vcSupplier_id, string vcWorkArea, string vcDock, string vcOrderNo, string vcPartNo, string vcReceiveFlag)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select b.vcBZPlant, a.vcInjectionFactory, a.vcTargetMonth, a.vcSupplier_id, a.vcWorkArea, ");
                strSql.AppendLine(" a.vcDock, a.vcOrderNo, a.vcPartNo, a.vcNewOldFlag, ");
                strSql.AppendLine(" CONVERT(int,a.vcOrderNumber), CONVERT(int, a.vcOrderNumber)-b.iQuantity as vcNoReceiveNumber, ");
                strSql.AppendLine(" a.vcNoReceiveReason, a.vcExpectRedeemDate,a.vcRealRedeemDate,1 as vcFlag, '0'as iflag,'0' as vcModFlag,'0' as vcAddFlag,iAutoId  ");
                strSql.AppendLine(" from TOutsidePurchaseManage a ");
                strSql.AppendLine(" left join (select sum(iQuantity) as iQuantity,vcKBOrderNo,vcPart_id,vcSupplier_id,vcSR,vcBZPlant ");
                strSql.AppendLine(" from TOperateSJ  ");
                strSql.AppendLine(" where vcZYType='S0' ");
                strSql.AppendLine(" group by vcKBOrderNo,vcPart_id,vcSupplier_id,vcSR,vcBZPlant) b  ");
                strSql.AppendLine(" on a.vcOrderNo=b.vcKBOrderNo and a.vcPartNo=b.vcPart_id and a.vcDock=b.vcSR and a.vcSupplier_id=b.vcSupplier_id ");

                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and vcInjectionFactory in (select vcData2 from ConstMst where vcDataID='KBPlant' and vcData1= '" + vcInjectionFactory + "') ");
                }
                if (vcTargetMonth.Length > 0)
                {
                    strSql.AppendLine("  and  vcTargetMonth = '" + vcTargetMonth.Replace("-", "") + "' ");
                }
                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  vcSupplier_id like '%" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine("  and  vcWorkArea like '%" + vcWorkArea + "%' ");
                }
                if (vcDock.Length > 0)
                {
                    strSql.AppendLine("  and  vcDock like '%" + vcDock + "%' ");
                }
                if (vcOrderNo.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderNo like '%" + vcOrderNo + "%' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  vcPartNo like '%" + vcPartNo + "%' ");
                }
                if (vcReceiveFlag.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiveFlag = '" + vcReceiveFlag + "' ");
                }
                strSql.AppendLine("  order by  dOperatorTime desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable bindInjectionFactory()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select iFZGC as vcCodeId, iFZGC as vcCodeName from ");
                strSql.AppendLine("  ( ");
                strSql.AppendLine("  select distinct iFZGC as iFZGC from TOutsidePurchaseManage ");
                strSql.AppendLine("  ) S ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM TOutsidePurchaseManage where iAutoId in(   \r\n ");
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
                        sql.Append("insert into TOutsidePurchaseManage([vcDataName],[vcDataId],[vcData1],[vcData2],[vcData3],[vcData4],[vcData5],[vcData6],  \r\n");
                        sql.Append("[vcData7],[vcData8],[vcData9],[vcData10],[dCreateTime],[vcCreateUserId])  \r\n");
                        sql.Append(" values (  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcDataName"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcDataId"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData1"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData2"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData3"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData4"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData5"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData6"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData7"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData8"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData9"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData10"], false) + ",  \r\n");
                        sql.Append("getdate(),  \r\n");
                        sql.Append("'" + strUserId + "'  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改  
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("  update TOutsidePurchaseManage set   \r\n");
                        sql.Append("  vcPackPlant=" + ComFunction.getSqlValue(listInfoData[i]["vcPackPlant"], false) + "   \r\n");
                        sql.Append("  ,vcInjectionFactory=" + ComFunction.getSqlValue(listInfoData[i]["vcInjectionFactory"], false) + "   \r\n");
                        sql.Append("  ,vcTargetMonth=" + ComFunction.getSqlValue(listInfoData[i]["vcTargetMonth"], true) + "   \r\n");
                        sql.Append("  ,vcSupplier_id=" + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], true) + "   \r\n");
                        sql.Append("  ,vcWorkArea=" + ComFunction.getSqlValue(listInfoData[i]["vcWorkArea"], true) + "   \r\n");
                        sql.Append("  ,vcDock=" + ComFunction.getSqlValue(listInfoData[i]["vcDock"], true) + "   \r\n");
                        sql.Append("  ,vcOrderNo=" + ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], true) + "   \r\n");
                        sql.Append("  ,vcPartNo=" + ComFunction.getSqlValue(listInfoData[i]["vcPartNo"], true) + "   \r\n");
                        sql.Append("  ,vcNewOldFlag=" + ComFunction.getSqlValue(listInfoData[i]["vcNewOldFlag"], true) + "   \r\n");
                        sql.Append("  ,vcOrderNumber=" + ComFunction.getSqlValue(listInfoData[i]["vcOrderNumber"], true) + "   \r\n");
                        sql.Append("  ,vcNoReceiveNumber=" + ComFunction.getSqlValue(listInfoData[i]["vcNoReceiveNumber"], true) + "   \r\n");
                        sql.Append("  ,vcNoReceiveReason=" + ComFunction.getSqlValue(listInfoData[i]["vcNoReceiveReason"], true) + "   \r\n");
                        sql.Append("  ,vcExpectRedeemDate=" + ComFunction.getSqlValue(listInfoData[i]["vcExpectRedeemDate"], true) + "   \r\n");
                        sql.Append("  ,vcRealRedeemDate=" + ComFunction.getSqlValue(listInfoData[i]["vcRealRedeemDate"], true) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + strUserId + "'  \r\n");
                        sql.Append("  ,dOperatorTime=getdate()   \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");
                    }
                    excute.ExcuteSqlWithStringOper(sql.ToString());
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




        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dtadd"></param>
        /// <param name="userId"></param>
        public void Save(DataTable dtadd, DataTable dtmod, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    DataRow dr = dtadd.Rows[i];
                    sql.Append("insert into [TSpecialSupplier] (vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcOperatorID, dOperatorTime)  \n");
                    sql.Append(" values('" + dr["vcSupplier_id"].ToString() + "','" + dr["vcWorkArea"].ToString() + "','" + dr["dBeginDate"].ToString() + "','" + dr["dEndDate"].ToString() + "','" + userId + "',GETDATE()) \n");
                }
                for (int i = 0; i < dtmod.Rows.Count; i++)
                {
                    DataRow dr = dtmod.Rows[i];
                    sql.Append("update TSpecialSupplier set dBeginDate='" + Convert.ToDateTime(dr["dBeginDate"].ToString()) + "', dEndDate='" + Convert.ToDateTime(dr["dEndDate"].ToString()) + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE()  \n");
                    sql.Append("where vcSupplier_id='" + dr["vcSupplier_id"].ToString() + "' and vcWorkArea ='" + dr["vcWorkArea"].ToString() + "' \n");

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

        /// <summary>
        /// 验证修改的数据是否已经存在数据库
        /// </summary>
        /// <param name="dtamod"></param>
        /// <returns></returns>
        public bool isExistModData(DataTable dtamod)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtamod.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all select vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime from dbo.TSupplierInfo where vcSupplier_id='" + dtamod.Rows[i]["vcSupplier_id"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  select vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime from dbo.TSupplierInfo where vcSupplier_id='" + dtamod.Rows[i]["vcSupplier_id"] + "'  ");
                    }
                }
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 一括赋予
        /// </summary>
        /// <returns></returns>
        public void allInstall(DateTime dBeginDate, DateTime dEndDate, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update TSpecialSupplier set dBeginDate='" + dBeginDate + "', dEndDate='" + dEndDate + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE()  \n");

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

        public DataTable bindplant()
        {
            string ssql = " select '' as vcCodeName,'' as vcCodeId union all select distinct vcData2,vcData1 from ConstMst where vcDataID='KBPlant' ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        #region 导入后保存
        public bool importSave(DataTable dt, string vcPackPlant, string vcDate, string strUserId)
        {
            using (SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                bool b = false;
                SqlCommand cmdDel = new SqlCommand("delete from TOutsidePurchaseManage where vcPackPlant='" + vcPackPlant + "' and vcDate='" + vcDate + "';", conn);
                cmdDel.Transaction = trans;
                cmdDel.ExecuteNonQuery();

                DataTable dt1 = new DataTable();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt1.Columns.Add(dt.Columns[i].ColumnName);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt1.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        dr[j] = dt.Rows[i][j].ToString().Trim();
                    }
                    dt1.Rows.Add(dr);
                }
                try
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.InsertCommand = new SqlCommand();
                    da.InsertCommand.Connection = conn;
                    da.InsertCommand.CommandType = CommandType.Text;
                    da.InsertCommand.CommandText += "insert into TOutsidePurchaseManage(vcPackPlant, vcYear, vcTargetMonth, vcDate, vcSupplier_id, vcInjectionFactory, vcDock, vcOrderNo, vcPartNo, vcOrderNumber, dOperatorTime, vcOperatorID) ";
                    da.InsertCommand.CommandText += "values ('" + vcPackPlant + "',SUBSTRING(@vcOrderNo,1,4), SUBSTRING(@vcOrderNo,1,6), '" + vcDate + "', @vcSupplier_id, @vcInjectionFactory, @vcDock, @vcOrderNo, @vcPartNo, @vcOrderNumber, getdate(), '" + strUserId + "') ";
                    da.InsertCommand.Parameters.Add("@vcSupplier_id", SqlDbType.VarChar, 4, "vcSupplier_id");
                    da.InsertCommand.Parameters.Add("@vcInjectionFactory", SqlDbType.VarChar, 2, "vcInjectionFactory");
                    da.InsertCommand.Parameters.Add("@vcDock", SqlDbType.VarChar, 6, "vcDock");
                    da.InsertCommand.Parameters.Add("@vcOrderNo", SqlDbType.VarChar, 12, "vcOrderNo");
                    da.InsertCommand.Parameters.Add("@vcPartNo", SqlDbType.VarChar, 12, "vcPartNo");
                    da.InsertCommand.Parameters.Add("@vcOrderNumber", SqlDbType.Int, 4, "vcOrderNumber");
                    da.InsertCommand.Transaction = trans;
                    da.Update(dt1);
                    trans.Commit();
                    b = true;
                }
                catch (Exception ex)
                {
                    b = false;
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
                return b;
            }
        }
        #endregion

        #region 匹配

        #endregion

        #region 校验导入文件是否当日
        public string checkCurrentDate(string vcDate, DataTable dt)
        {
            DataTable dt1 = new DataTable();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt1.Columns.Add(dt.Columns[i].ColumnName);
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt1.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j == 3)
                        dr[j] = dt.Rows[i][j].ToString().Trim().Substring(0, 8);
                    else
                        dr[j] = dt.Rows[i][j].ToString().Trim();
                }
                dt1.Rows.Add(dr);
            }
            if (dt1.DefaultView.ToTable(true, "vcOrderNo").Rows.Count > 1)
            {
                return "订单号存在非同一日的数据";
            }
            else
            {
                if(vcDate!= dt1.DefaultView.ToTable(true, "vcOrderNo").Rows[0][0].ToString())
                {
                    return "导入订单不是导入日期同一天";
                }
            }
            return "";
        }
        #endregion
    }
}
