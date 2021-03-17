﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Common;

namespace BatchProcess
{
    public class FP0022
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0022";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "批处理开始", null, strUserId);
                bool b1 = updatePartInfo(strUserId);
                bool b2 = addPartInfo(strUserId, getNewData());
                if (b1 && b2)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行成功", null, strUserId);
                    return true;
                }
                else
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行失败", null, strUserId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, ex.ToString(), null, strUserId);
                throw ex;
            }
        }
        #endregion


        #region 更新品番TSPMaster数据
        public bool updatePartInfo(string strUserId)
        {
            try
            {
                StringBuilder strSQL = new StringBuilder();
                strSQL.Append("update TPartInfoMaster \n");
                strSQL.Append("set TPartInfoMaster.vcCarFamilyCode=t.vcCarFamilyCode,TPartInfoMaster.vcSupplierPlant=t.vcSupplierPlant, \n");
                strSQL.Append("TPartInfoMaster.iQuantityPerContainer=t.iQuantityPerContainer, \n");
                strSQL.Append("TPartInfoMaster.vcPartsNameEN=t.vcPartENName, \n");
                strSQL.Append("TPartInfoMaster.vcPartFrequence=t.vcOrderingMethod, \n");
                strSQL.Append("TPartInfoMaster.vcDock=t.vcDock, \n");
                strSQL.Append("from (select a.vcPartId, \n");
                strSQL.Append("convert(char(10),a.dFromTime,120) as dFromTime, convert(char(10),a.dToTime,120) as dToTime,  \n");
                strSQL.Append("c.vcSufferIn as vcDock,a.vcCarFamilyCode,a.vcPartENName,  \n");
                strSQL.Append("b.iPackingQty as iQuantityPerContainer, \n");
                strSQL.Append("a.vcName as vcOrderingMethod, a.vcReceiver, a.vcSupplierId,d.vcSupplierPlant \n");
                strSQL.Append("from    \n");
                strSQL.Append(" (select TSPMaster.*, TCode.vcName, TCode.vcValue  \n");
                strSQL.Append(" from TSPMaster     \n");
                strSQL.Append("  left join TCode on TSPMaster.vcOrderingMethod=TCode.vcValue where TCode.vcCodeId='C047' and vcInOut='0') a  \n");
                strSQL.Append("  left join (select * from TSPMaster_Box where vcOperatorType='1') b  \n");
                strSQL.Append("  on a.vcPartId=b.vcPartId and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId  \n");
                strSQL.Append("  left join (select * from TSPMaster_SufferIn where vcOperatorType='1') c  \n");
                strSQL.Append("  on a.vcPartId=c.vcPartId and a.vcPackingPlant=c.vcPackingPlant and a.vcReceiver=c.vcReceiver and a.vcSupplierId=c.vcSupplierId  \n");
                strSQL.Append("  left join (select * from TSPMaster_SupplierPlant where vcOperatorType='1') d \n");
                strSQL.Append("  on a.vcPartId=d.vcPartId and a.vcPackingPlant=d.vcPackingPlant and a.vcReceiver=d.vcReceiver and a.vcSupplierId=d.vcSupplierId) t \n");
                strSQL.Append("where TPartInfoMaster.vcPartsNo=t.vcPartId  \n");
                strSQL.Append("and TPartInfoMaster.vcCpdCompany=t.vcReceiver \n");
                strSQL.Append("and TPartInfoMaster.vcSupplierCode=t.vcSupplierId \n");
                int i = excute.ExecuteSQLNoQuery(strSQL.ToString());
                if (i > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                //ComMessage.GetInstance().ProcessMessage("FP0018", ex.ToString(), null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 新增获取新数据
        public DataTable getNewData()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from ( \n");
                sql.Append(" select a.vcPartId, CONVERT(char(10),a.dFromTime,120) as dFromTime, CONVERT(char(10),a.dToTime,120) as dToTime, c.vcSufferIn as vcDock, a.vcCarFamilyCode, a.vcPartENName,  \n");
                sql.Append(" b.iPackingQty as iQuantityPerContainer,a.vcName as vcOrderingMethod, a.vcReceiver, a.vcSupplierId, d.vcSupplierPlant, a.vcPartImage, a.dOperatorTime  \n");
                sql.Append(" from (  \n");
                sql.Append("    select TSPMaster.*, TCode.vcName, TCode.vcValue from TSPMaster  \n");
                sql.Append("    left join TCode  \n");
                sql.Append("	on TSPMaster.vcOrderingMethod=TCode.vcValue where TCode.vcCodeId='C047' and vcInOut='0') a   \n");
                sql.Append("	left join (select * from TSPMaster_Box where vcOperatorType='1') b   \n");
                sql.Append("	on a.vcPartId=b.vcPartId and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId  \n");
                sql.Append("	left join (select * from TSPMaster_SufferIn where vcOperatorType='1') c   \n");
                sql.Append("	on a.vcPartId=c.vcPartId and a.vcPackingPlant=c.vcPackingPlant and a.vcReceiver=c.vcReceiver and a.vcSupplierId=c.vcSupplierId \n");
                sql.Append("	left join (select * from TSPMaster_SupplierPlant where vcOperatorType='1') d    \n");
                sql.Append("	on a.vcPartId=d.vcPartId and a.vcPackingPlant=d.vcPackingPlant and a.vcReceiver=d.vcReceiver and a.vcSupplierId=d.vcSupplierId) t  \n");
                sql.Append("where not EXISTS (select vcPartId,vcReceiver,vcSupplierId   \n");
                sql.Append("from TPartInfoMaster b where t.vcPartId=b.vcPartsNo and b.vcCpdCompany=t.vcReceiver and b.vcSupplierCode=t.vcSupplierId)  \n");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                //ComMessage.GetInstance().ProcessMessage("FP0018", ex.ToString(), null, strUserId);
                throw ex;
            }
        }
        public bool addPartInfo(string strUserId, DataTable dt)
        {
            try
            {
                DataTable tb = excute.ExcuteSqlWithSelectToDT("select * from TPartInfoMaster");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow r = tb.NewRow();
                    r["vcPartsNo"] = dt.Rows[i]["vcPartId"].ToString();
                    r["dTimeFrom"] = dt.Rows[i]["dFromTime"].ToString();
                    r["dTimeTo"] = dt.Rows[i]["dToTime"].ToString();
                    r["vcDock"] = dt.Rows[i]["vcDock"].ToString();
                    r["vcCarFamilyCode"] = dt.Rows[i]["vcCarFamilyCode"].ToString();
                    r["vcPartsNameEN"] = dt.Rows[i]["vcPartENName"].ToString();
                    r["iQuantityPerContainer"] = dt.Rows[i]["iQuantityPerContainer"].ToString();
                    r["vcPartFrequence"] = dt.Rows[i]["vcOrderingMethod"].ToString();
                    r["vcCpdCompany"] = dt.Rows[i]["vcReceiver"].ToString();
                    r["vcSupplierCode"] = dt.Rows[i]["vcSupplierId"].ToString();
                    r["vcSupplierPlant"] = dt.Rows[i]["vcSupplierPlant"].ToString();
                    r["vcInOutFlag"] = "0";
                    r["vcPhotoPath"] = dt.Rows[i]["vcPartImage"].ToString();
                    r["dDateTime"] = dt.Rows[i]["dOperatorTime"].ToString();
                    r["dUpdataTime"] = DateTime.Now;
                    r["vcUpdataUser"] = strUserId;
                    tb.Rows.Add(r);
                }
                using (SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString()))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    try
                    {
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.InsertCommand = new SqlCommand();
                        da.InsertCommand.Connection = conn;
                        da.InsertCommand.CommandText = "insert into TPartInfoMaster (vcPartsNo,dTimeFrom,dTimeTo,vcDock,vcCarFamilyCode,";
                        da.InsertCommand.CommandText += "vcPartsNameEN,iQuantityPerContainer,vcPartFrequence,vcCpdCompany,vcSupplierCode,vcSupplierPlant,vcInOutFlag,vcPhotoPath) ";
                        da.InsertCommand.CommandText += "values (@vcPartsNo,@dTimeFrom,@dTimeTo,@vcDock,@vcCarFamilyCode, ";
                        da.InsertCommand.CommandText += "@vcPartsNameEN,@iQuantityPerContainer,@vcPartFrequence,@vcCpdCompany,@vcSupplierCode,@vcSupplierPlant,'0',@vcPhotoPath) ";
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcPartsNo", SqlDbType.VarChar, 20, "vcPartsNo"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@dTimeFrom", SqlDbType.VarChar, 20, "dTimeFrom"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@dTimeTo", SqlDbType.VarChar, 20, "dTimeTo"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcDock", SqlDbType.VarChar, 20, "vcDock"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcCarFamilyCode", SqlDbType.VarChar, 100, "vcCarFamilyCode"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcPartsNameEN", SqlDbType.VarChar, 100, "vcPartsNameEN"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@iQuantityPerContainer", SqlDbType.VarChar, 20, "iQuantityPerContainer"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcPartFrequence", SqlDbType.VarChar, 20, "vcPartFrequence"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcCpdCompany", SqlDbType.VarChar, 100, "vcCpdCompany"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcSupplierCode", SqlDbType.VarChar, 100, "vcSupplierCode"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcSupplierPlant", SqlDbType.VarChar, 20, "vcSupplierPlant"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcPhotoPath", SqlDbType.VarChar, 20, "vcPhotoPath"));
                        da.InsertCommand.Transaction = trans;
                        da.Update(tb);
                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                //批处理异常结束
                //ComMessage.GetInstance().ProcessMessage("FP0018", ex.ToString(), null, strUserId);
                throw ex;
            }
        }
        #endregion
    }
}
