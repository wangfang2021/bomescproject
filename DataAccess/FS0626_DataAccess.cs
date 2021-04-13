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

        public DataTable Search(string vcPackPlant, string vcInjectionFactory, string vcTargetMonth, string vcSupplier_id, string vcWorkArea, string vcDock, string vcOrderNo, string vcPartNo, string vcReceiveFlag)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select a.vcPackPlant, '#'+a.vcInjectionFactory as vcInjectionFactory, convert(int,a.vcTargetMonth) as vcTargetMonth, a.vcSupplier_id, a.vcWorkArea, ");
                strSql.AppendLine(" a.vcDock, a.vcOrderNo, substring(a.vcOrderNo,1,8) as vcOrderDate, a.vcPartNo, b.vcName as vcNewOldFlag, ");
                strSql.AppendLine(" convert(int,a.vcOrderNumber) as vcOrderNumber, convert(int,isnull(a.vcNoReceiveNumber,0)) as vcNoReceiveNumber, ");
                strSql.AppendLine(" a.vcNoReceiveReason, a.vcExpectRedeemDate, 1 as vcFlag, '0' as iflag,'0' as vcModFlag,'0' as vcAddFlag, a.iAutoId  ");
                strSql.AppendLine(" from TOutsidePurchaseManage a ");
                strSql.AppendLine(" left join (select vcName,vcValue from TCode where vcCodeId='C004') b ");
                strSql.AppendLine(" on a.vcNewOldFlag=b.vcValue ");
                strSql.AppendLine(" where 1=1 ");
                if (vcPackPlant.Length > 0)
                {
                    strSql.AppendLine(" and a.vcPackPlant= '" + vcPackPlant + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine(" and a.vcInjectionFactory='" + vcInjectionFactory + "' ");
                }
                if (vcTargetMonth.Length > 0)
                {
                    strSql.AppendLine(" and a.vcTargetMonth= '" + vcTargetMonth.Replace("-", "").Replace("/", "") + "' ");
                }
                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine(" and a.vcSupplier_id like '" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine(" and a.vcWorkArea like '%" + vcWorkArea + "%' ");
                }
                if (vcDock.Length > 0)
                {
                    strSql.AppendLine(" and a.vcDock like '%" + vcDock + "%' ");
                }
                if (vcOrderNo.Length > 0)
                {
                    strSql.AppendLine(" and a.vcOrderNo like '" + vcOrderNo + "%' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine(" and a.vcPartNo like '" + vcPartNo + "%' ");
                }
                if (vcReceiveFlag == "1")
                {
                    strSql.AppendLine(" and isnull(vcNoReceiveNumber,0)=0 ");
                }
                else if (vcReceiveFlag == "0")
                {
                    strSql.AppendLine(" and a.vcNoReceiveNumber<>0 ");
                }
                strSql.AppendLine(" order by vcOrderDate desc, a.vcOrderNo, a.vcPartNo");
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
                    if (bAddFlag == false && bModFlag == true)
                    {//修改  
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("  update TOutsidePurchaseManage set \r\n");
                        sql.Append("  vcNoReceiveReason=" + ComFunction.getSqlValue(listInfoData[i]["vcNoReceiveReason"], true) + "  \r\n");
                        sql.Append("  ,vcExpectRedeemDate=" + ComFunction.getSqlValue(listInfoData[i]["vcExpectRedeemDate"], true) + "  \r\n");
                        sql.Append("  ,vcOperatorID='" + strUserId + "' \r\n");
                        sql.Append("  ,dOperatorTime=getdate()  \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + " ; \r\n");
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

        #region 受入
        public DataTable getDock()
        {
            try
            {
                string sql = "select distinct vcsufferin as vcValue from TSPMaster_SufferIn order by vcsufferin";
                return excute.ExcuteSqlWithSelectToDT(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public DataTable getPackPlant()
        {
            string ssql = "select vcName, vcValue from TCode where vcCodeId='C017' ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        #region 导入后保存
        public bool importSave(DataTable dt, string strUserId)
        {
            using (SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                bool b = false;
                try
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("delete from TOutsidePurchaseManage where vcOrderNo='" + dt.Rows[i]["vcOrderNo"].ToString() + "' and vcPartNo='" + dt.Rows[i]["vcPartNo"].ToString() + "'; \r\n");
                    }
                    SqlCommand cmdDel = new SqlCommand(sb.ToString(), conn);
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

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.InsertCommand = new SqlCommand();
                    da.InsertCommand.Connection = conn;
                    da.InsertCommand.CommandType = CommandType.Text;
                    da.InsertCommand.CommandText += "insert into TOutsidePurchaseManage(vcTargetMonth, vcSupplier_id, vcWorkArea, vcDock, vcOrderNo, vcPartNo, vcOrderNumber, vcNoReceiveNumber, dOperatorTime, vcOperatorID) ";
                    da.InsertCommand.CommandText += "values (SUBSTRING(@vcOrderNo,1,6), @vcSupplier_id, @vcWorkArea, @vcDock, @vcOrderNo, @vcPartNo, @vcOrderNumber, @vcOrderNumber, getdate(), '" + strUserId + "') ";
                    da.InsertCommand.Parameters.Add("@vcSupplier_id", SqlDbType.VarChar, 4, "vcSupplier_id"); //供应商代码
                    da.InsertCommand.Parameters.Add("@vcWorkArea", SqlDbType.VarChar, 2, "vcWorkArea");//工区
                    da.InsertCommand.Parameters.Add("@vcDock", SqlDbType.VarChar, 6, "vcDock");//受入
                    da.InsertCommand.Parameters.Add("@vcOrderNo", SqlDbType.VarChar, 20, "vcOrderNo");//订单号
                    da.InsertCommand.Parameters.Add("@vcPartNo", SqlDbType.VarChar, 16, "vcPartNo");//品番
                    da.InsertCommand.Parameters.Add("@vcOrderNumber", SqlDbType.Int, 4, "vcOrderNumber");//订单数量
                    da.InsertCommand.Transaction = trans;
                    da.Update(dt1);

                    SqlCommand cmdUpdate = new SqlCommand();
                    cmdUpdate.Connection = conn;
                    cmdUpdate.CommandText += "update TOutsidePurchaseManage ";
                    cmdUpdate.CommandText += "set TOutsidePurchaseManage.vcNoReceiveNumber=TOutsidePurchaseManage.vcOrderNumber-b.iQuantity ";
                    cmdUpdate.CommandText += "from (select sum(iQuantity) as iQuantity,vcKBOrderNo,vcPart_id,vcSupplier_id,vcSR,vcBZPlant ";
                    cmdUpdate.CommandText += "      from TOperateSJ where vcZYType='S0' ";
                    cmdUpdate.CommandText += "      group by vcKBOrderNo,vcPart_id,vcSupplier_id,vcSR,vcBZPlant) b ";
                    cmdUpdate.CommandText += "where TOutsidePurchaseManage.vcOrderNo=b.vcKBOrderNo and TOutsidePurchaseManage.vcPartNo=b.vcPart_id ";
                    cmdUpdate.CommandText += "and TOutsidePurchaseManage.vcDock=b.vcSR and TOutsidePurchaseManage.vcSupplier_id=b.vcSupplier_id ";
                    //cmdUpdate.CommandText += "and TOutsidePurchaseManage.vcTargetMonth>='" + vcDateFrom + "' and TOutsidePurchaseManage.vcTargetMonth<='" + vcDateTo + "' ";
                    cmdUpdate.Transaction = trans;
                    cmdUpdate.ExecuteNonQuery();

                    SqlCommand cmdUpdate1 = new SqlCommand();
                    cmdUpdate1.Connection = conn;
                    cmdUpdate1.CommandText += "update TOutsidePurchaseManage ";
                    cmdUpdate1.CommandText += "set TOutsidePurchaseManage.vcPackPlant=c.vcPackingPlant, TOutsidePurchaseManage.vcNewOldFlag=c.vcHaoJiu ";
                    cmdUpdate1.CommandText += "from (select vcPackingPlant,vcHaoJiu,vcPartId,vcSupplierId from TSPMaster ";
                    cmdUpdate1.CommandText += "      where vcInOut='1' and dFromTime<=GETDATE() and dToTime>=GETDATE()) c ";
                    cmdUpdate1.CommandText += "where TOutsidePurchaseManage.vcPartNo=c.vcPartId and TOutsidePurchaseManage.vcSupplier_id=c.vcSupplierId ";
                    //cmdUpdate1.CommandText += "and TOutsidePurchaseManage.vcTargetMonth>='" + vcDateFrom + "' and TOutsidePurchaseManage.vcTargetMonth<='" + vcDateTo + "' ";
                    cmdUpdate1.Transaction = trans;
                    cmdUpdate1.ExecuteNonQuery();

                    SqlCommand cmdUpdate2 = new SqlCommand();
                    cmdUpdate2.Connection = conn;
                    cmdUpdate2.CommandText += "update TOutsidePurchaseManage ";
                    cmdUpdate2.CommandText += "set TOutsidePurchaseManage.vcInjectionFactory=d.vcValue5 ";
                    cmdUpdate2.CommandText += "from (select vcValue1,vcValue2,vcValue5 from TOutCode ";
                    cmdUpdate2.CommandText += "      where vcCodeId='C010' and convert(date,vcValue3)<=GETDATE() and convert(date,vcValue4)>=GETDATE() and vcIsColum='0') d ";
                    cmdUpdate2.CommandText += "where TOutsidePurchaseManage.vcSupplier_id=d.vcValue1 and TOutsidePurchaseManage.vcWorkArea=d.vcValue2 ";
                    //cmdUpdate2.CommandText += "and TOutsidePurchaseManage.vcTargetMonth>='" + vcDateFrom + "' and TOutsidePurchaseManage.vcTargetMonth<='" + vcDateTo + "' ";
                    cmdUpdate2.Transaction = trans;
                    cmdUpdate2.ExecuteNonQuery();


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

        #region 欠品状态更新
        public bool updateData(string vcTargetMonthFrom, string vcTargetMonthTo, string userId)
        {
            using (SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                bool b = false;
                try
                {
                    SqlCommand cmdUpdate = new SqlCommand();
                    cmdUpdate.Connection = conn;
                    cmdUpdate.CommandText += "update TOutsidePurchaseManage ";
                    cmdUpdate.CommandText += "set TOutsidePurchaseManage.vcNoReceiveNumber=TOutsidePurchaseManage.vcOrderNumber-b.iQuantity ";
                    cmdUpdate.CommandText += "from (select sum(iQuantity) as iQuantity,vcKBOrderNo,vcPart_id,vcSupplier_id,vcSR,vcBZPlant ";
                    cmdUpdate.CommandText += "      from TOperateSJ where vcZYType='S0' ";
                    cmdUpdate.CommandText += "      group by vcKBOrderNo,vcPart_id,vcSupplier_id,vcSR,vcBZPlant) b ";
                    cmdUpdate.CommandText += "where TOutsidePurchaseManage.vcOrderNo=b.vcKBOrderNo and TOutsidePurchaseManage.vcPartNo=b.vcPart_id ";
                    cmdUpdate.CommandText += "and TOutsidePurchaseManage.vcDock=b.vcSR and TOutsidePurchaseManage.vcSupplier_id=b.vcSupplier_id ";
                    cmdUpdate.CommandText += "and TOutsidePurchaseManage.vcTargetMonth>='" + vcTargetMonthFrom + "' and TOutsidePurchaseManage.vcTargetMonth<='" + vcTargetMonthTo + "';";
                    cmdUpdate.Transaction = trans;
                    cmdUpdate.ExecuteNonQuery();

                    SqlCommand cmdUpdate1 = new SqlCommand();
                    cmdUpdate1.Connection = conn;
                    cmdUpdate1.CommandText += "update TOutsidePurchaseManage ";
                    cmdUpdate1.CommandText += "set TOutsidePurchaseManage.vcPackPlant=c.vcPackingPlant, TOutsidePurchaseManage.vcNewOldFlag=c.vcHaoJiu ";
                    cmdUpdate1.CommandText += "from (select vcPackingPlant,vcHaoJiu,vcPartId,vcSupplierId from TSPMaster ";
                    cmdUpdate1.CommandText += "      where vcInOut='1' and dFromTime<=GETDATE() and dToTime>=GETDATE()) c ";
                    cmdUpdate1.CommandText += "where TOutsidePurchaseManage.vcPartNo=c.vcPartId and TOutsidePurchaseManage.vcSupplier_id=c.vcSupplierId ";
                    cmdUpdate1.CommandText += "and TOutsidePurchaseManage.vcTargetMonth>='" + vcTargetMonthFrom + "' and TOutsidePurchaseManage.vcTargetMonth<='" + vcTargetMonthTo + "'; ";
                    cmdUpdate1.Transaction = trans;
                    cmdUpdate1.ExecuteNonQuery();

                    SqlCommand cmdUpdate2 = new SqlCommand();
                    cmdUpdate2.Connection = conn;
                    cmdUpdate2.CommandText += "update TOutsidePurchaseManage ";
                    cmdUpdate2.CommandText += "set TOutsidePurchaseManage.vcInjectionFactory=d.vcValue5 ";
                    cmdUpdate2.CommandText += "from (select vcValue1,vcValue2,vcValue5 from TOutCode ";
                    cmdUpdate2.CommandText += "      where vcCodeId='C010' and convert(date,vcValue3)<=GETDATE() and convert(date,vcValue4)>=GETDATE() and vcIsColum='0') d ";
                    cmdUpdate2.CommandText += "where TOutsidePurchaseManage.vcSupplier_id=d.vcValue1 and TOutsidePurchaseManage.vcWorkArea=d.vcValue2 ";
                    cmdUpdate2.CommandText += "and TOutsidePurchaseManage.vcTargetMonth>='" + vcTargetMonthFrom + "' and TOutsidePurchaseManage.vcTargetMonth<='" + vcTargetMonthTo + "';";
                    cmdUpdate2.Transaction = trans;
                    cmdUpdate2.ExecuteNonQuery();

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
    }
}
