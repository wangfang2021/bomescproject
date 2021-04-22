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
                strSQL.Append("TPartInfoMaster.vcCurrentPastCode=t.vcHaoJiu, \n");
                strSQL.Append("TPartInfoMaster.vcPhotoPath=t.vcPartImage,  \n");
                strSQL.Append("TPartInfoMaster.vcLogisticRoute=t.vcPassProject  \n");
                strSQL.Append("from (select a.vcPartId, \n");
                strSQL.Append("convert(char(10),a.dFromTime,120) as dFromTime, convert(char(10),a.dToTime,120) as dToTime, \n");
                strSQL.Append("isnull(c.vcSufferIn,'') as vcDock,a.vcCarFamilyCode,a.vcPartENName,  \n");
                strSQL.Append("b.iPackingQty as iQuantityPerContainer, \n");
                strSQL.Append("a.vcValue as vcOrderingMethod, a.vcReceiver, a.vcSupplierId, d.vcSupplierPlant, a.vcHaoJiu, a.vcPartImage, a.vcPassProject  \n");
                strSQL.Append("from    \n");
                strSQL.Append(" (select TSPMaster.*, TCode.vcValue  \n");
                strSQL.Append("  from TSPMaster  \n");
                strSQL.Append("  left join TCode on TSPMaster.vcOrderingMethod=TCode.vcValue where TCode.vcCodeId='C047' and vcInOut='0') a  \n");
                strSQL.Append("  left join (select * from TSPMaster_Box where vcOperatorType='1' and dToTime>=getdate()) b  \n");
                strSQL.Append("  on a.vcPartId=b.vcPartId and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId  \n");
                strSQL.Append("  left join (select * from TSPMaster_SufferIn where vcOperatorType='1' and dToTime>=getdate()) c  \n");
                strSQL.Append("  on a.vcPartId=c.vcPartId and a.vcPackingPlant=c.vcPackingPlant and a.vcReceiver=c.vcReceiver and a.vcSupplierId=c.vcSupplierId  \n");
                strSQL.Append("  left join (select * from TSPMaster_SupplierPlant where vcOperatorType='1' and dToTime>=getdate()) d \n");
                strSQL.Append("  on a.vcPartId=d.vcPartId and a.vcPackingPlant=d.vcPackingPlant and a.vcReceiver=d.vcReceiver and a.vcSupplierId=d.vcSupplierId) t \n");
                strSQL.Append("where TPartInfoMaster.vcPartsNo=t.vcPartId  \n");
                strSQL.Append("and TPartInfoMaster.vcCpdCompany=t.vcReceiver \n");
                strSQL.Append("and TPartInfoMaster.vcSupplierCode=t.vcSupplierId and TPartInfoMaster.vcInOutFlag='0'; \n");

                strSQL.Append("update TPartInfoMaster \n");
                strSQL.Append("set TPartInfoMaster.vcCarFamilyCode=t.vcCarType,  \n");
                strSQL.Append("TPartInfoMaster.iQuantityPerContainer=t.iContainerQuantity,  \n");
                strSQL.Append("TPartInfoMaster.vcPartsNameEN=t.vcPartNameEn, \n");
                strSQL.Append("TPartInfoMaster.vcDock=t.vcSR, \n");
                strSQL.Append("TPartInfoMaster.vcCurrentPastCode='', \n");
                strSQL.Append("TPartInfoMaster.vcPhotoPath=t.vcPhotoPath,  \n");
                strSQL.Append("TPartInfoMaster.vcLogisticRoute='1'  \n");
                strSQL.Append("from ( \n");
                strSQL.Append("    select vcPart_id, convert(varchar,dTimeFrom,23) as dFromTime, convert(varchar,dTimeTo,23) as dToTime, vcSR, \n");
                strSQL.Append("    vcCarType, vcPartNameEn, iContainerQuantity, vcSHF,   \n");
                strSQL.Append("    vcSupplier_id, vcPhotoPath, dOperatorTime   \n");
                strSQL.Append("    from TEDTZPartsNoMaster) t   \n");
                strSQL.Append("where TPartInfoMaster.vcPartsNo=t.vcPart_id  \n");
                strSQL.Append("and TPartInfoMaster.vcCpdCompany=t.vcSHF  \n");
                strSQL.Append("and TPartInfoMaster.vcSupplierCode=t.vcSupplier_id and TPartInfoMaster.vcInOutFlag='1'; \n");

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
                sql.Append(" b.iPackingQty as iQuantityPerContainer,a.vcValue as vcOrderingMethod, a.vcReceiver, a.vcSupplierId, d.vcSupplierPlant, a.vcPartImage, a.vcPassProject, a.vcHaoJiu, '0' as vcInOutFlag, a.dOperatorTime   \n");
                sql.Append(" from (  \n");
                sql.Append("    select TSPMaster.*, TCode.vcValue from TSPMaster  \n");
                sql.Append("    left join TCode  \n");
                sql.Append("	on TSPMaster.vcOrderingMethod=TCode.vcValue where TCode.vcCodeId='C047' and vcInOut='0') a   \n");
                sql.Append("	left join (select * from TSPMaster_Box where vcOperatorType='1') b   \n");
                sql.Append("	on a.vcPartId=b.vcPartId and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId  \n");
                sql.Append("	left join (select * from TSPMaster_SufferIn where vcOperatorType='1') c   \n");
                sql.Append("	on a.vcPartId=c.vcPartId and a.vcPackingPlant=c.vcPackingPlant and a.vcReceiver=c.vcReceiver and a.vcSupplierId=c.vcSupplierId  \n");
                sql.Append("	left join (select * from TSPMaster_SupplierPlant where vcOperatorType='1') d   \n");
                sql.Append("	on a.vcPartId=d.vcPartId and a.vcPackingPlant=d.vcPackingPlant and a.vcReceiver=d.vcReceiver and a.vcSupplierId=d.vcSupplierId) t  \n");
                sql.Append("where not EXISTS (select vcPartId,vcReceiver,vcSupplierId  \n");
                sql.Append("from TPartInfoMaster b where t.vcPartId=b.vcPartsNo and b.vcCpdCompany=t.vcReceiver and b.vcSupplierCode=t.vcSupplierId)  \n");
                sql.Append("union all  \n");
                //以下ED品番基础数据
                sql.Append("select t.vcPart_id as vcPartId, convert(varchar,t.dTimeFrom,23) as dFromTime, convert(varchar,t.dTimeTo,23) as dToTime, t.vcSR as vcDock,  \n");
                sql.Append("t.vcCarType as vcCarFamilyCode, t.vcPartNameEn, t.iContainerQuantity as iQuantityPerContainer, '0' as vcOrderingMethod, t.vcSHF as vcReceiver,  \n");
                sql.Append("t.vcSupplier_id as vcSupplierId, '' as vcSupplierPlant, t.vcPhotoPath as vcPartImage, '1' as vcPassProject, '' as vcHaoJiu, '1' as vcInOutFlag,  t.dOperatorTime  \n");
                sql.Append("from TEDTZPartsNoMaster t   \n");
                sql.Append("where not EXISTS (select vcPartsNo,vcCpdCompany,vcSupplierCode   \n");
                sql.Append("from TPartInfoMaster b where b.vcPartsNo=t.vcPart_id and b.vcCpdCompany=t.vcSHF and b.vcSupplierCode=t.vcSupplier_id)  \n");
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
                    r["vcPhotoPath"] = dt.Rows[i]["vcPartImage"].ToString();
                    r["vcLogisticRoute"] = dt.Rows[i]["vcPassProject"].ToString();
                    r["vcCurrentPastCode"] = dt.Rows[i]["vcHaoJiu"].ToString();           
                    r["vcInOutFlag"] = dt.Rows[i]["vcInOutFlag"].ToString();
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
                        da.InsertCommand.CommandText += "vcPartsNameEN,iQuantityPerContainer,vcPartFrequence,vcCpdCompany,vcSupplierCode,vcSupplierPlant,vcPhotoPath,vcLogisticRoute,vcCurrentPastCode,vcInOutFlag,dDateTime,vcUpdataUser,dUpdataTime) ";
                        da.InsertCommand.CommandText += "values (@vcPartsNo,@dTimeFrom,@dTimeTo,@vcDock,@vcCarFamilyCode, ";
                        da.InsertCommand.CommandText += "@vcPartsNameEN,@iQuantityPerContainer,@vcPartFrequence,@vcCpdCompany,@vcSupplierCode,@vcSupplierPlant,@vcPhotoPath,@vcLogisticRoute,@vcCurrentPastCode,@vcInOutFlag,@dDateTime,@vcUpdataUser,@dUpdataTime) ";
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
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcLogisticRoute", SqlDbType.VarChar, 20, "vcLogisticRoute"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcCurrentPastCode", SqlDbType.VarChar, 20, "vcCurrentPastCode"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcInOutFlag", SqlDbType.VarChar, 10, "vcInOutFlag"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@dDateTime", SqlDbType.DateTime, 20, "dDateTime"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@vcUpdataUser", SqlDbType.VarChar, 10, "vcUpdataUser"));
                        da.InsertCommand.Parameters.Add(new SqlParameter("@dUpdataTime", SqlDbType.DateTime, 20, "dUpdataTime"));
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
