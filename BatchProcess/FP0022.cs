using System;
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
                updateParts(strUserId);
                return true;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, ex.ToString(), null, strUserId);
                throw ex;
            }
        }
        #endregion


        #region 旧的方法
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
                strSQL.Append("from  \n");
                strSQL.Append(" (select TSPMaster.*, TCode.vcValue from TSPMaster \n");
                strSQL.Append("  left join TCode \n");
                strSQL.Append("  on TSPMaster.vcOrderingMethod=TCode.vcValue where TCode.vcCodeId='C047' and TSPMaster.vcInOut='0') a  \n");
                strSQL.Append("  left join (select * from TSPMaster_Box where vcOperatorType='1' and dToTime>=getdate()) b  \n");
                strSQL.Append("  on a.vcPartId=b.vcPartId and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId and a.dFromTime=b.dFromTime and a.dToTime=b.dToTime  \n");
                strSQL.Append("  left join (select * from TSPMaster_SufferIn where vcOperatorType='1' and dToTime>=getdate()) c  \n");
                strSQL.Append("  on a.vcPartId=c.vcPartId and a.vcPackingPlant=c.vcPackingPlant and a.vcReceiver=c.vcReceiver and a.vcSupplierId=c.vcSupplierId and a.dFromTime=b.dFromTime and a.dToTime=b.dToTime  \n");
                strSQL.Append("  left join (select * from TSPMaster_SupplierPlant where vcOperatorType='1' and dToTime>=getdate()) d \n");
                strSQL.Append("  on a.vcPartId=d.vcPartId and a.vcPackingPlant=d.vcPackingPlant and a.vcReceiver=d.vcReceiver and a.vcSupplierId=d.vcSupplierId and a.dFromTime=b.dFromTime and a.dToTime=b.dToTime) t  \n");
                strSQL.Append("where TPartInfoMaster.vcPartsNo=t.vcPartId  \n");
                strSQL.Append("and TPartInfoMaster.vcCpdCompany=t.vcReceiver \n");
                strSQL.Append("and TPartInfoMaster.vcSupplierCode=t.vcSupplierId \n");
                strSQL.Append("and TPartInfoMaster.dTimeFrom=t.dFromTime \n");
                strSQL.Append("and TPartInfoMaster.dTimeTo=t.dToTime; \n");
                if (excute.ExecuteSQLNoQuery(strSQL.ToString()) > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    DataTable dt = excute.ExcuteSqlWithSelectToDT("select vcPartsNo as vcPartsNoED, substring(vcPartsNo,1,10)+'00' as vcPartsNo00, dTimeFrom, dTimeTo from TPartInfoMaster where dTimeTo>=convert(char(10),getdate(),120) and substring(vcPartsNo,11,2)='ED'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        { 
                            //s2.Append("update TPartInfoMaster set vcInOutFlag='0' where vcPartsNo='" + dt.Rows[i]["vcPartsNo00"].ToString() + "' ");
                            //s2.Append("and dTimeFrom!='" + dt.Rows[i]["dTimeFrom"].ToString() + "' and dTimeTo>=convert(char(10),getdate(),120);");
                            //s2.Append("update TPartInfoMaster set vcInOutFlag='1',vcSupplierPlant='' where vcPartsNo='" + dt.Rows[i]["vcPartsNo00"].ToString() + "' ");
                            //s2.Append("and dTimeFrom='" + dt.Rows[i]["dTimeFrom"].ToString() + "' and dTimeTo>=convert(char(10),getdate(),120);");
                            sb.Append("update TPartInfoMaster set vcInOutFlag='1' ");
                            sb.Append("where vcPartsNo='" + dt.Rows[i]["vcPartsNo00"].ToString() + "' and dTimeTo>=convert(char(10),getdate(),120);");
                        }
                        excute.ExecuteSQLNoQuery(sb.ToString());
                    }
                    else
                    {
                        sb.Append("update TPartInfoMaster set vcInOutFlag='0' where vcInOutFlag='1' and dTimeTo<=convert(char(10),dTimeTo,120); \n");
                        excute.ExecuteSQLNoQuery(sb.ToString());
                    }
                    return true;
                }
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
                sql.Append(" select a.vcPartId, convert(char(10),a.dFromTime,120) as dFromTime,convert(char(10),a.dToTime,120) as dToTime,c.vcSufferIn as vcDock,a.vcCarFamilyCode,a.vcPartENName,  \n");
                sql.Append(" b.iPackingQty as iQuantityPerContainer,a.vcValue as vcOrderingMethod,a.vcReceiver,a.vcSupplierId,d.vcSupplierPlant,a.vcPartImage,a.vcPassProject,a.vcHaoJiu,'0' as vcInOutFlag,a.dOperatorTime \n");
                sql.Append(" from (  \n");
                sql.Append("    select TSPMaster.*, TCode.vcValue from TSPMaster  \n");
                sql.Append("    left join TCode  \n");
                sql.Append("	on TSPMaster.vcOrderingMethod=TCode.vcValue where TCode.vcCodeId='C047' and TSPMaster.vcInOut='0') a  \n");
                sql.Append("	left join (select * from TSPMaster_Box where vcOperatorType='1' and dToTime>=getdate()) b   \n");
                sql.Append("	on a.vcPartId=b.vcPartId and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId and a.dFromTime=b.dFromTime and a.dToTime=b.dToTime \n");
                sql.Append("	left join (select * from TSPMaster_SufferIn where vcOperatorType='1' and dToTime>=getdate()) c   \n");
                sql.Append("	on a.vcPartId=c.vcPartId and a.vcPackingPlant=c.vcPackingPlant and a.vcReceiver=c.vcReceiver and a.vcSupplierId=c.vcSupplierId and a.dFromTime=b.dFromTime and a.dToTime=b.dToTime \n");
                sql.Append("	left join (select * from TSPMaster_SupplierPlant where vcOperatorType='1' and dToTime>=getdate()) d  \n");
                sql.Append("	on a.vcPartId=d.vcPartId and a.vcPackingPlant=d.vcPackingPlant and a.vcReceiver=d.vcReceiver and a.vcSupplierId=d.vcSupplierId and a.dFromTime=b.dFromTime and a.dToTime=b.dToTime) t  \n");
                sql.Append(" where not exists (select vcPartId,vcReceiver,vcSupplierId,vcInOutFlag,dTimeFrom,dTimeTo  \n");
                sql.Append(" from TPartInfoMaster b where t.vcPartId=b.vcPartsNo and b.vcCpdCompany=t.vcReceiver and b.vcSupplierCode=t.vcSupplierId  \n");
                sql.Append(" and b.dTimeFrom=convert(char(10),t.dFromTime,120) and b.dTimeTo=convert(char(10),t.dToTime,120))  \n");
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
                    if (dt.Rows[i]["vcPartId"].ToString().Substring(10, 2) == "00")
                    {
                        StringBuilder s2 = new StringBuilder();
                        s2.Append("substring(vcPartId,1,10)='" + dt.Rows[i]["vcPartId"].ToString().Substring(0, 10) + "' and substring(vcPartId,11,2)='ED' ");
                        s2.Append("and dFromTime='" + dt.Rows[i]["dFromTime"].ToString() + "' and dToTime='" + dt.Rows[i]["dToTime"].ToString() + "' ");
                        DataRow[] rs = dt.Select(s2.ToString());
                        if (rs.Length <= 0)
                        {
                            StringBuilder s3 = new StringBuilder();
                            s3.Append("update TPartInfoMaster ");
                            s3.Append("set dTimeTo='" + Convert.ToDateTime(dt.Rows[i]["dFromTime"].ToString()).AddDays(-1).ToString("yyyy-MM-dd") + "' ");
                            s3.Append("where iAutoId in (" + getMaxFromTime(dt.Rows[i]["vcPartId"].ToString().Substring(0, 10) + "ED") + ");");
                            excute.ExecuteSQLNoQuery(s3.ToString());
                        }
                    }


                    StringBuilder sb = new StringBuilder();
                    sb.Append("update TPartInfoMaster ");
                    sb.Append("set dTimeTo='" + Convert.ToDateTime(dt.Rows[i]["dFromTime"].ToString()).AddDays(-1).ToString("yyyy-MM-dd") + "' ");
                    sb.Append("where iAutoId in (" + getMaxFromTime(dt.Rows[i]["vcPartId"].ToString()) + ");");
                    excute.ExecuteSQLNoQuery(sb.ToString());


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
                if (dt != null && dt.Rows.Count > 0)
                {
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
                return true;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                //ComMessage.GetInstance().ProcessMessage("FP0018", ex.ToString(), null, strUserId);
                throw ex;
            }
        }

        public string getMaxFromTime(string vcPartsNo)
        {
            string sql = "select * from TPartInfoMaster where vcPartsNo='" + vcPartsNo + "' ";
            sql += "and dTimeFrom=(select max(dTimeFrom) as dTimeFrom from TPartInfoMaster ";
            sql += "where vcPartsNo='" + vcPartsNo + "')";
            DataTable dt = excute.ExcuteSqlWithSelectToDT(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                string id = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    id += dt.Rows[i]["iAutoId"].ToString() + ",";
                }
                id = id.TrimEnd(',');
                return id;
            }
            return "0";
        }
        #endregion
        #endregion

        #region 获取Master所有品番及履历
        public DataTable getMasterParts()
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append(" select * from (   \n");
            sb1.Append("    select a.vcPartId, convert(char(10),a.dFromTime,120) as dFromTime,convert(char(10),a.dToTime,120) as dToTime,'' as vcDock,a.vcCarFamilyCode,a.vcPartENName,  \n");
            sb1.Append(@"	 '' as iQuantityPerContainer,a.vcValue as vcOrderingMethod,a.vcReceiver,a.vcSupplierId,d.vcSupplierPlant,replace(a.vcPartImage,'\pic\','') as vcPartImage,a.vcPassProject,a.vcHaoJiu,'0' as vcInOutFlag,a.dOperatorTime ");
            sb1.Append("     from (select TSPMaster.*, TCode.vcValue from TSPMaster left join TCode   \n");
            sb1.Append("     on TSPMaster.vcOrderingMethod=TCode.vcValue where TCode.vcCodeId='C047' and TSPMaster.vcInOut='0') a \n");
            sb1.Append(" left join (select * from TSPMaster_SupplierPlant where vcOperatorType='1' and dToTime>=getdate()) d   \n");
            sb1.Append(" on a.vcPartId=d.vcPartId and a.vcPackingPlant=d.vcPackingPlant and a.vcReceiver=d.vcReceiver and a.vcSupplierId=d.vcSupplierId) t   \n");

            DataTable dt1 = excute.ExcuteSqlWithSelectToDT(sb1.ToString());
            DataTable dt = dt1.Clone();

            StringBuilder sb2 = new StringBuilder();
            sb2.Append(" select * from TSPMaster_SufferIn a \n");
            sb2.Append(" inner join (select TSPMaster.*, TCode.vcValue from TSPMaster left join TCode \n");
            sb2.Append(" on TSPMaster.vcOrderingMethod=TCode.vcValue where TCode.vcCodeId='C047' and TSPMaster.vcInOut='0') b \n");
            sb2.Append(" on a.vcPartId=b.vcPartId and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId \n");
            sb2.Append(" \n");
            DataTable dt2 = excute.ExcuteSqlWithSelectToDT(sb2.ToString());

            StringBuilder sb3 = new StringBuilder();
            sb3.Append(" select * from TSPMaster_Box a \n");
            sb3.Append(" inner join (select TSPMaster.*, TCode.vcValue from TSPMaster left join TCode \n");
            sb3.Append(" on TSPMaster.vcOrderingMethod=TCode.vcValue where TCode.vcCodeId='C047' and TSPMaster.vcInOut='0') b \n");
            sb3.Append(" on a.vcPartId=b.vcPartId and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId \n");
            sb3.Append(" \n");
            DataTable dt3 = excute.ExcuteSqlWithSelectToDT(sb3.ToString());

            DateTime dFromTime = Convert.ToDateTime("9999-12-31");
            DateTime dToTime = Convert.ToDateTime("9999-12-31");
            string vcSR = "";
            string vcPacking = "";
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                string vcPartId1 = dt1.Rows[i]["vcPartId"].ToString();
                for (int j = 0; j < dt2.Rows.Count; j++)
                {
                    string vcPartId2 = dt2.Rows[j]["vcPartId"].ToString();
                    vcSR = dt2.Rows[j]["vcSufferIn"].ToString();
                    if (vcPartId1 == vcPartId2)
                    {
                        DateTime dFromTime1 = Convert.ToDateTime(dt2.Rows[j]["dFromTime"]);
                        DateTime dToTime1 = Convert.ToDateTime(dt2.Rows[j]["dToTime"]);
                        for (int k = 0; k < dt3.Rows.Count; k++)
                        {
                            string vcPartId3 = dt3.Rows[k]["vcPartId"].ToString();
                            vcPacking = dt3.Rows[k]["iPackingQty"].ToString();
                            if (vcPartId2 == vcPartId3)
                            {
                                DateTime dFromTime2 = Convert.ToDateTime(dt3.Rows[k]["dFromTime"]);
                                DateTime dToTime2 = Convert.ToDateTime(dt3.Rows[k]["dToTime"]);
                                if (dFromTime2 <= dFromTime1 && dFromTime1 <= dToTime2)
                                {
                                    dFromTime = dFromTime1;
                                    if (dFromTime2 <= dToTime1 && dToTime1 <= dToTime2)
                                    {
                                        dToTime = dToTime1;
                                    }
                                    else if (dToTime1 > dToTime2)
                                    {
                                        dToTime = dToTime2;
                                    }
                                    else if (dToTime1 < dToTime2)
                                    {
                                        dToTime = dToTime1;
                                    }
                                    DataRow r = dt.NewRow();
                                    r["vcPartId"] = vcPartId1;
                                    r["dFromTime"] = dFromTime;
                                    r["dToTime"] = dToTime;
                                    r["vcDock"] = vcSR;
                                    r["vcCarFamilyCode"] = dt1.Rows[i]["vcCarFamilyCode"];
                                    r["vcPartENName"] = dt1.Rows[i]["vcPartENName"];
                                    r["iQuantityPerContainer"] = vcPacking;
                                    r["vcOrderingMethod"] = dt1.Rows[i]["vcOrderingMethod"];
                                    r["vcReceiver"] = dt1.Rows[i]["vcReceiver"];
                                    r["vcSupplierId"] = dt1.Rows[i]["vcSupplierId"];
                                    r["vcSupplierPlant"] = dt1.Rows[i]["vcSupplierPlant"];
                                    r["vcPartImage"] = dt1.Rows[i]["vcPartImage"];
                                    r["vcPassProject"] = dt1.Rows[i]["vcPassProject"];
                                    r["vcHaoJiu"] = dt1.Rows[i]["vcHaoJiu"];
                                    r["vcInOutFlag"] = dt1.Rows[i]["vcInOutFlag"];
                                    r["dOperatorTime"] = dt1.Rows[i]["dOperatorTime"];
                                    dt.Rows.Add(r);
                                }
                                else if (dFromTime1 < dFromTime2 && dToTime1 >= dToTime2)
                                {
                                    dFromTime = dFromTime2;
                                    if (dFromTime2 <= dToTime1 && dToTime1 <= dToTime2)
                                    {
                                        dToTime = dToTime1;
                                    }
                                    else if (dToTime1 > dToTime2)
                                    {
                                        dToTime = dToTime2;
                                    }
                                    else if (dToTime1 < dToTime2)
                                    {
                                        dToTime = dToTime1;
                                    }
                                    DataRow r = dt.NewRow();
                                    r["vcPartId"] = vcPartId1;
                                    r["dFromTime"] = dFromTime;
                                    r["dToTime"] = dToTime;
                                    r["vcDock"] = vcSR;
                                    r["vcCarFamilyCode"] = dt1.Rows[i]["vcCarFamilyCode"];
                                    r["vcPartENName"] = dt1.Rows[i]["vcPartENName"];
                                    r["iQuantityPerContainer"] = vcPacking;
                                    r["vcOrderingMethod"] = dt1.Rows[i]["vcOrderingMethod"];
                                    r["vcReceiver"] = dt1.Rows[i]["vcReceiver"];
                                    r["vcSupplierId"] = dt1.Rows[i]["vcSupplierId"];
                                    r["vcSupplierPlant"] = dt1.Rows[i]["vcSupplierPlant"];
                                    r["vcPartImage"] = dt1.Rows[i]["vcPartImage"];
                                    r["vcPassProject"] = dt1.Rows[i]["vcPassProject"];
                                    r["vcHaoJiu"] = dt1.Rows[i]["vcHaoJiu"];
                                    r["vcInOutFlag"] = dt1.Rows[i]["vcInOutFlag"];
                                    r["dOperatorTime"] = dt1.Rows[i]["dOperatorTime"];
                                    dt.Rows.Add(r);
                                }
                            }
                        }
                    }
                }
            }
            return dt;
        }
        #endregion

        #region 获取内制所有品番数据
        public DataTable getPartsInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select * from TPartInfoMaster  \n");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
            return dt;
        }

        #endregion

        #region 更新现有品番履历
        public void updateParts(string strUserId)
        {
            DataTable dt1 = getMasterParts(); //采购品番Master
            DataTable dt2 = getPartsInfo(); //内制看板品番Master
            StringBuilder strSQL = new StringBuilder();
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                bool hasFind = false;
                for (int j = 0; j < dt2.Rows.Count; j++)
                {
                    if (dt1.Rows[i]["vcPartId"].ToString() == dt2.Rows[j]["vcPartsNo"].ToString()
                        && dt1.Rows[i]["vcDock"].ToString() == dt2.Rows[j]["vcDock"].ToString()
                        && dt1.Rows[i]["vcReceiver"].ToString() == dt2.Rows[j]["vcCpdCompany"].ToString()
                        && dt1.Rows[i]["iQuantityPerContainer"].ToString() == dt2.Rows[j]["iQuantityPerContainer"].ToString()
                        && dt1.Rows[i]["vcSupplierId"].ToString() == dt2.Rows[j]["vcSupplierCode"].ToString())
                    {
                        strSQL.Append("update TPartInfoMaster ");
                        strSQL.Append("set vcCarFamilyCode='" + dt1.Rows[i]["vcCarFamilyCode"].ToString() + "', \n");
                        strSQL.Append("vcSupplierPlant='" + dt1.Rows[i]["vcSupplierPlant"].ToString() + "', \n");
                        strSQL.Append("iQuantityPerContainer='" + dt1.Rows[i]["iQuantityPerContainer"].ToString() + "', \n");
                        strSQL.Append("vcPartsNameEN='" + dt1.Rows[i]["vcPartENName"].ToString() + "', \n");
                        strSQL.Append("vcPartFrequence='" + dt1.Rows[i]["vcOrderingMethod"].ToString() + "', \n");
                        strSQL.Append("vcCurrentPastCode='" + dt1.Rows[i]["vcHaoJiu"].ToString() + "', \n");
                        strSQL.Append("vcPhotoPath='" + dt1.Rows[i]["vcPartImage"].ToString() + "',  \n");
                        strSQL.Append("vcLogisticRoute='" + dt1.Rows[i]["vcPassProject"].ToString() + "', \n");
                        strSQL.Append("dTimeFrom='" + Convert.ToDateTime(dt1.Rows[i]["dFromTime"].ToString()).ToString("yyyy-MM-dd") + "',  \n");
                        strSQL.Append("dTimeTo='" + Convert.ToDateTime(dt1.Rows[i]["dToTime"].ToString()).ToString("yyyy-MM-dd") + "',  \n");
                        strSQL.Append("vcUpdataUser='" + strUserId + "',  \n");
                        strSQL.Append("dUpdataTime='" + DateTime.Now.ToString() + "'  \n");
                        strSQL.Append("where vcPartsNo='" + dt1.Rows[i]["vcPartId"].ToString() + "'  \n");
                        strSQL.Append("and vcDock='" + dt1.Rows[i]["vcDock"].ToString() + "'  \n");
                        strSQL.Append("and vcCpdCompany='" + dt1.Rows[i]["vcReceiver"].ToString() + "'  \n");
                        strSQL.Append("and iQuantityPerContainer='" + dt1.Rows[i]["iQuantityPerContainer"].ToString() + "'  \n");
                        strSQL.Append("and vcSupplierCode='" + dt1.Rows[i]["vcSupplierId"].ToString() + "';  \n");
                        hasFind = true;
                        break;
                    }
                }
                if (!hasFind)
                {
                    strSQL.Append("insert into TPartInfoMaster (vcPartsNo,dTimeFrom,dTimeTo,vcDock,vcCarFamilyCode, \n");
                    strSQL.Append("vcPartsNameEN,iQuantityPerContainer,vcPartFrequence,vcCpdCompany,vcSupplierCode,vcSupplierPlant, \n");
                    strSQL.Append("vcPhotoPath,vcLogisticRoute,vcCurrentPastCode,vcInOutFlag,dDateTime,vcUpdataUser,dUpdataTime) \n");
                    strSQL.Append("values ");
                    strSQL.Append("('" + dt1.Rows[i]["vcPartId"].ToString() + "', \n");
                    strSQL.Append("'" + Convert.ToDateTime(dt1.Rows[i]["dFromTime"].ToString()).ToString("yyyy-MM-dd") + "', \n ");
                    strSQL.Append("'" + Convert.ToDateTime(dt1.Rows[i]["dToTime"].ToString()).ToString("yyyy-MM-dd") + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcDock"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcCarFamilyCode"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcPartENName"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["iQuantityPerContainer"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcOrderingMethod"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcReceiver"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcSupplierId"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcSupplierPlant"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcPartImage"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcPassProject"].ToString() + "', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["vcHaoJiu"].ToString() + "', \n ");
                    strSQL.Append("'0', \n ");
                    strSQL.Append("'" + dt1.Rows[i]["dOperatorTime"].ToString() + "', \n ");
                    strSQL.Append("'" + strUserId + "', \n ");
                    strSQL.Append("'" + DateTime.Now.ToString() + "'); ");
                }
            }
            excute.ExecuteSQLNoQuery(strSQL.ToString());
        }
        #endregion
    }

}
