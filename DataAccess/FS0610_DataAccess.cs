using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class FS0610_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 取日历
        public DataTable GetCalendar(string strPlant, string vcDXYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from TCalendar_PingZhun_Nei where vcFZGC='" + strPlant + "' and TARGETMONTH='" + vcDXYM + "'  and TOTALWORKDAYS>0    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取soq数据
        public DataTable GetSoq(string strPlant, string strYearMonth, string strType)
        {
            StringBuilder sql = new StringBuilder();
            string strhycolumn = "";
            if (strType == "dxym")
                strhycolumn = "iHySOQN";
            else if (strType == "nsym")
                strhycolumn = "iHySOQN1";
            else if (strType == "nnsym")
                strhycolumn = "iHySOQN2";
            sql.Append("select vcPart_id, " + strhycolumn + " as iHyNum,iQuantityPercontainer from TSoq     \n");
            sql.Append("where vcYearMonth='" + strYearMonth + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='0'   \n");
            sql.Append(" and " + strhycolumn + ">0  \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion

        #region 取soq数据(已合意)
        public DataTable GetSoqHy(string strPlant, string strYearMonth, string strType)
        {
            StringBuilder sql = new StringBuilder();
            string strhycolumn = "";
            if (strType == "dxym")
                strhycolumn = "iHySOQN";
            else if (strType == "nsym")
                strhycolumn = "iHySOQN1";
            else if (strType == "nnsym")
                strhycolumn = "iHySOQN2";
            sql.Append("  select vcPart_id, " + strhycolumn + " as iHyNum,iQuantityPercontainer,b.dFromTime,b.dToTime,a.vcReceiver from TSoq a     \n");
            sql.Append("  left join   \n");
            sql.Append("  (   \n");
            sql.Append("  select * from TSPMaster   \n");
            sql.Append("  )b on a.vcPart_id=b.vcPartId and a.vcSupplier_id=b.vcSupplierId  and a.vcReceiver=b.vcReceiver    \n");
            sql.Append("  where a.vcYearMonth='" + strYearMonth + "' and a.vcFZGC='" + strPlant + "' and a.vcInOutFlag='0' and a.vcHyState='2'   order by a.iAutoId  \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion

        #region 更新平准化结果
        public void SaveResult(string strCLYM, string strDXYM, string strNSYM, string strNNSYM, string strPlant,
            ArrayList arrResult_DXYM, ArrayList arrResult_NSYM, ArrayList arrResult_NNSYM, string strUserId, string strUnit)
        {
            SqlCommand cmd;
            SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString());//对账平台
            ComConnectionHelper.OpenConection_SQL(ref conn);
            cmd = new SqlCommand();
            SqlTransaction st = conn.BeginTransaction();
            try
            {
                DataTable temp = excute.ExcuteSqlWithSelectToDT("select top 1 vcValue from TCode where vcCodeId='C068'");
                string vcReceiver = temp.Rows.Count == 1 ? temp.Rows[0][0].ToString() : "APC06";

                StringBuilder sql = new StringBuilder();
                sql.Append("delete from TSoqReply where vcCLYM='" + strCLYM + "' and vcDXYM in ('" + strDXYM + "','" + strNSYM + "','" + strNNSYM + "') " +
                    "and vcFZGC='" + strPlant + "' and vcInOutFlag='0'    \n");
                #region 更新arrResult_DXYM
                for (int i = 0; i < arrResult_DXYM.Count; i++)
                {
                    string[] arr = (string[])arrResult_DXYM[i];
                    sql.Append(Resultsql(arr, strPlant, strCLYM, strDXYM, strUserId));
                    if (i % 2000 == 0)
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = st;
                        cmd.CommandText = sql.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        sql.Length = 0;
                    }
                }
                if (sql.Length > 0)
                {
                    cmd.Connection = conn;
                    cmd.Transaction = st;
                    cmd.CommandText = sql.ToString();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    sql.Length = 0;
                }
                #endregion
                #region 更新arrResult_NSYM
                for (int i = 0; i < arrResult_NSYM.Count; i++)
                {
                    string[] arr = (string[])arrResult_NSYM[i];
                    sql.Append(Resultsql(arr, strPlant, strCLYM, strNSYM, strUserId));
                    if (i % 2000 == 0)
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = st;
                        cmd.CommandText = sql.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        sql.Length = 0;
                    }
                }
                if (sql.Length > 0)
                {
                    cmd.Connection = conn;
                    cmd.Transaction = st;
                    cmd.CommandText = sql.ToString();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    sql.Length = 0;
                }
                #endregion
                #region 更新arrResult_NNSYM
                for (int i = 0; i < arrResult_NNSYM.Count; i++)
                {
                    string[] arr = (string[])arrResult_NNSYM[i];
                    sql.Append(Resultsql(arr, strPlant, strCLYM, strNNSYM, strUserId));
                    if (i % 2000 == 0)
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = st;
                        cmd.CommandText = sql.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        sql.Length = 0;
                    }
                }
                if (sql.Length > 0)
                {
                    cmd.Connection = conn;
                    cmd.Transaction = st;
                    cmd.CommandText = sql.ToString();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    sql.Length = 0;
                }
                #endregion
                #region 统一更新需要关联字段：订货频度vcMakingOrderType,车型编码vcCarType
                sql.Append("update t1 set t1.vcMakingOrderType=t2.vcOrderingMethod,t1.vcCarType=t2.vcCarfamilyCode     \n");
                sql.Append("from    \n");
                sql.Append("(    \n");
                sql.Append("	select * from TSoqReply     \n");
                sql.Append("	where vcCLYM='" + strCLYM + "' and vcDXYM in ('" + strDXYM + "','" + strNSYM + "','" + strNNSYM + "') and vcFZGC='" + strPlant + "' " +
                    "and vcInOutFlag='0')t1    \n");
                sql.Append("left join(    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcOrderingMethod from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='"+ vcReceiver + "' and '" + strDXYM + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");//TFTM和APC06是写死的
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");

                sql.Append("update t1 set t1.vcSupplier_id=t2.vcSupplier_id,t1.vcSR=t6.vcSufferIn    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoqReply where vcCLYM='" + strCLYM + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='0'   \n");
                sql.Append(")t1    \n");
                sql.Append("left join (    \n");
                sql.Append("	select * from TSoq    \n");
                sql.Append("	where vcYearMonth='" + strDXYM + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='0'   \n");
                sql.Append(") t2 on t1.vcPart_id=t2.vcPart_id     \n");
                sql.Append("left join        \n");
                sql.Append("(--手配主表        \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dFromTime,dToTime         \n");
                sql.Append("	from TSPMaster where vcPackingPlant='TFTM' and vcReceiver='APC06' and dFromTime<>dToTime     \n");
                sql.Append(")t3 on t1.vcPart_id=t3.vcPartId and t1.vcDXYM between convert(varchar(6),t3.dFromTime,112) and convert(varchar(6),t3.dToTime,112)        \n");
                sql.Append("left join             \n");
                sql.Append("(    --//受入 N        \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn,dFromTime,dToTime        \n");
                sql.Append("	from TSPMaster_SufferIn            \n");
                sql.Append("	where vcOperatorType='1'            \n");
                sql.Append(")t6 on t3.vcPartId=t6.vcPartId and t3.vcPackingPlant=t6.vcPackingPlant and t3.vcReceiver=t6.vcReceiver and t3.vcSupplierId=t6.vcSupplierId         \n");
                sql.Append("and t1.vcDXYM between convert(varchar(6),t6.dFromTime,112) and convert(varchar(6),t6.dToTime,112)      \n");
                
                cmd.Connection = conn;
                cmd.Transaction = st;
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                #endregion

                #region 删除生产计划相关表
                string mon = strDXYM.Substring(0, 4) + "-" + strDXYM.Substring(4, 2);
                sql.AppendLine("delete from MonthKanBanPlanTbl  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthKanBanPlanTbl.vcPartsno and dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "')");
                sql.AppendLine("delete from MonthP3PlanTbl  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthP3PlanTbl.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');");
                sql.AppendLine("delete from MonthPackPlanTbl  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthPackPlanTbl.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');");
                sql.AppendLine("delete from MonthProdPlanTbl  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthProdPlanTbl.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');");
                sql.AppendLine("delete from MonthTZPlanTbl where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthTZPlanTbl.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');");
                sql.AppendLine("delete from MonthKanBanPlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthKanBanPlanTblTMP.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');");
                sql.AppendLine("delete from MonthP3PlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthP3PlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');");
                sql.AppendLine("delete from MonthPackPlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthPackPlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');");
                sql.AppendLine("delete from MonthProPlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthProPlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');");
                sql.AppendLine("delete from MonthTZPlanTblTMP where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = MonthTZPlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');");

                cmd.Connection = conn;
                cmd.Transaction = st;
                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                #endregion

                st.Commit();
                ComConnectionHelper.CloseConnection_SQL(ref conn);
            }
            catch (Exception ex)
            {
                st.Rollback();
                ComConnectionHelper.CloseConnection_SQL(ref conn);
                throw ex;
            }
        }
        #endregion

        #region 生成insert的sql语句
        public StringBuilder Resultsql(string[] arr, string strPlant, string strCLYM, string strDXYM, string strUserId)
        {
            StringBuilder sql = new StringBuilder();
            #region insert sql
            sql.Append("INSERT INTO [TSoqReply]    \n");
            sql.Append("           ([vcFZGC]    \n");
            sql.Append("           ,[vcMakingOrderType]    \n");
            sql.Append("           ,[vcInOutFlag]    \n");
            sql.Append("           ,[vcCLYM]    \n");
            sql.Append("           ,[vcDXYM]    \n");
            sql.Append("           ,[vcPart_id]    \n");
            sql.Append("           ,[vcCarType]    \n");
            sql.Append("           ,[iQuantityPercontainer]    \n");
            sql.Append("           ,[iBoxes]    \n");
            sql.Append("           ,[iPartNums]    \n");
            sql.Append("           ,[iD1]    \n");
            sql.Append("           ,[iD2]    \n");
            sql.Append("           ,[iD3]    \n");
            sql.Append("           ,[iD4]    \n");
            sql.Append("           ,[iD5]    \n");
            sql.Append("           ,[iD6]    \n");
            sql.Append("           ,[iD7]    \n");
            sql.Append("           ,[iD8]    \n");
            sql.Append("           ,[iD9]    \n");
            sql.Append("           ,[iD10]    \n");
            sql.Append("           ,[iD11]    \n");
            sql.Append("           ,[iD12]    \n");
            sql.Append("           ,[iD13]    \n");
            sql.Append("           ,[iD14]    \n");
            sql.Append("           ,[iD15]    \n");
            sql.Append("           ,[iD16]    \n");
            sql.Append("           ,[iD17]    \n");
            sql.Append("           ,[iD18]    \n");
            sql.Append("           ,[iD19]    \n");
            sql.Append("           ,[iD20]    \n");
            sql.Append("           ,[iD21]    \n");
            sql.Append("           ,[iD22]    \n");
            sql.Append("           ,[iD23]    \n");
            sql.Append("           ,[iD24]    \n");
            sql.Append("           ,[iD25]    \n");
            sql.Append("           ,[iD26]    \n");
            sql.Append("           ,[iD27]    \n");
            sql.Append("           ,[iD28]    \n");
            sql.Append("           ,[iD29]    \n");
            sql.Append("           ,[iD30]    \n");
            sql.Append("           ,[iD31]    \n");
            sql.Append("           ,[vcReceiver]    \n");
            sql.Append("           ,[vcOperatorID]    \n");
            sql.Append("           ,[dOperatorTime])    \n");
            sql.Append("     VALUES    \n");
            sql.Append("           ('" + strPlant + "'    \n");//工厂
            sql.Append("           ,''    \n");//vcMakingOrderType 品番表关联
            sql.Append("           ,'0'    \n");//内制0/外注1
            sql.Append("           ,'" + strCLYM + "'    \n");//处理年月
            sql.Append("           ,'" + strDXYM + "'    \n");//对象年月
            sql.Append("           ,'" + arr[0] + "'    \n");//品番
            sql.Append("           ,''    \n");//vcCarType 品番表关联
            sql.Append("           ,nullif(" + arr[2] + ",'')    \n");//收容数
            sql.Append("           ,nullif(" + arr[3] + ",'')    \n");//箱数
            sql.Append("           ,nullif(" + arr[1] + ",'')    \n");//订单数
            sql.Append("           ,nullif(" + arr[4] + "*"+ arr[2] + ",'')    \n");//D1
            sql.Append("           ,nullif(" + arr[5] + "*" + arr[2] + ",'')    \n");//D2
            sql.Append("           ,nullif(" + arr[6] + "*" + arr[2] + ",'')    \n");//D3
            sql.Append("           ,nullif(" + arr[7] + "*" + arr[2] + ",'')    \n");//D4
            sql.Append("           ,nullif(" + arr[8] + "*" + arr[2] + ",'')    \n");//D5
            sql.Append("           ,nullif(" + arr[9] + "*" + arr[2] + ",'')    \n");//D6
            sql.Append("           ,nullif(" + arr[10] + "*" + arr[2] + ",'')   \n");//D7
            sql.Append("           ,nullif(" + arr[11] + "*" + arr[2] + ",'')   \n");//D8
            sql.Append("           ,nullif(" + arr[12] + "*" + arr[2] + ",'')   \n");//D9
            sql.Append("           ,nullif(" + arr[13] + "*" + arr[2] + ",'')    \n");//D10
            sql.Append("           ,nullif(" + arr[14] + "*" + arr[2] + ",'')    \n");//D11
            sql.Append("           ,nullif(" + arr[15] + "*" + arr[2] + ",'')    \n");//D12
            sql.Append("           ,nullif(" + arr[16] + "*" + arr[2] + ",'')    \n");//D13
            sql.Append("           ,nullif(" + arr[17] + "*" + arr[2] + ",'')    \n");//D14
            sql.Append("           ,nullif(" + arr[18] + "*" + arr[2] + ",'')    \n");//D15
            sql.Append("           ,nullif(" + arr[19] + "*" + arr[2] + ",'')    \n");//D16
            sql.Append("           ,nullif(" + arr[20] + "*" + arr[2] + ",'')    \n");//D17
            sql.Append("           ,nullif(" + arr[21] + "*" + arr[2] + ",'')    \n");//D18
            sql.Append("           ,nullif(" + arr[22] + "*" + arr[2] + ",'')    \n");//D19
            sql.Append("           ,nullif(" + arr[23] + "*" + arr[2] + ",'')    \n");//D20
            sql.Append("           ,nullif(" + arr[24] + "*" + arr[2] + ",'')    \n");//D21
            sql.Append("           ,nullif(" + arr[25] + "*" + arr[2] + ",'')    \n");//D22
            sql.Append("           ,nullif(" + arr[26] + "*" + arr[2] + ",'')    \n");//D23
            sql.Append("           ,nullif(" + arr[27] + "*" + arr[2] + ",'')    \n");//D24
            sql.Append("           ,nullif(" + arr[28] + "*" + arr[2] + ",'')    \n");//D25
            sql.Append("           ,nullif(" + arr[29] + "*" + arr[2] + ",'')    \n");//D26
            sql.Append("           ,nullif(" + arr[30] + "*" + arr[2] + ",'')    \n");//D27
            sql.Append("           ,nullif(" + arr[31] + "*" + arr[2] + ",'')    \n");//D28
            sql.Append("           ,nullif(" + arr[32] + "*" + arr[2] + ",'')    \n");//D29
            sql.Append("           ,nullif(" + arr[33] + "*" + arr[2] + ",'')    \n");//D30
            sql.Append("           ,nullif(" + arr[34] + "*" + arr[2] + ",'')    \n");//D31
            sql.Append("           ,'" + arr[39] + "'    \n");//vcReceiver
            sql.Append("           ,'" + strUserId + "'    \n");//操作者
            sql.Append("           ,getdate())    \n");//操作时间
            #endregion
            return sql;
        }
        #endregion

        #region 获取展开的数据
        public DataTable getZhankaiData(bool isZhankai, List<string> plantList)
        {
            string plants = "";
            for (int i = 0; i < plantList.Count; i++)
            {
                plants += "'" + plantList[i] + "',";
            }
            plants = plants.Substring(0, plants.Length - 1);
            string vcCLYM = System.DateTime.Now.ToString("yyyyMM");
            StringBuilder sql = new StringBuilder();
            sql.Append("   select * from TSOQReply where  vcCLYM='" + vcCLYM + "' and vcInOutFlag='0' and vcFZGC in (" + plants + ") \n");
            if (isZhankai)
                sql.Append("  and dZhanKaiTime is not null  ");
            else
                sql.Append("  and dZhanKaiTime is null  ");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion

        #region 展开SOQReply
        public int zk(string userId, List<string> plantList)
        {
            try
            {
                string plants = "";
                for (int i = 0; i < plantList.Count; i++)
                {
                    plants += "'" + plantList[i] + "',";
                }
                plants = plants.Substring(0, plants.Length - 1);
                string vcCLYM = System.DateTime.Now.ToString("yyyyMM");
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" update TSOQReply ");
                strSql.AppendLine(" set   ");
                strSql.AppendLine(" vcZhanKaiID='" + userId + "', ");
                strSql.AppendLine(" dZhanKaiTime=getDate()");
                strSql.AppendLine(" where ");
                strSql.AppendLine(" vcCLYM='" + vcCLYM + "' ");
                strSql.AppendLine(" and dZhanKaiTime is null ");
                strSql.AppendLine(" and vcInOutFlag='0' ");
                strSql.AppendLine(" and vcFZGC in (" + plants + ")  ");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 找计算结果条数
        public int GetCalNum(string strYearMonth, List<string> plantList)
        {
            try
            {
                string plants = "";
                for (int i = 0; i < plantList.Count; i++)
                {
                    plants += "'" + plantList[i] + "',";
                }
                plants = plants.Substring(0, plants.Length - 1);
                string vcCLYM = System.DateTime.Now.ToString("yyyyMM");
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(1) as num from TSOQReply where vcCLYM='" + vcCLYM + "' and vcDXYM='" + strYearMonth + "' and vcInOutFlag='0' and vcFZGC in (" + plants + ")    \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 下载SOQReply（检索内容）
        public DataTable search(string strYearMonth, string strYearMonth_2, string strYearMonth_3, List<string> plantList)
        {
            try
            {
                string plants = "";
                for (int i = 0; i < plantList.Count; i++)
                {
                    plants += "'" + plantList[i] + "',";
                }
                plants = plants.Substring(0, plants.Length - 1);
                string vcCLYM = System.DateTime.Now.ToString("yyyyMM");
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" SELECT a.*,b.[N+1 O/L],b.[N+1 Units],b.[N+1 PCS], \n");
                strSql.AppendLine(" c.[N+2 O/L],c.[N+2 Units],c.[N+2 PCS],d.vcName as '订货频度',e.vcName as '发注工厂'  \n");
                strSql.AppendLine(" FROM \n");
                strSql.AppendLine(" ( \n");
                strSql.AppendLine("   SELECT ");
                strSql.AppendLine("   vcPart_id as 'PartsNo', \n");
                //发注工厂
                strSql.AppendLine("   vcFZGC, \n");
                //订货频度
                strSql.AppendLine("   vcMakingOrderType, \n");
                strSql.AppendLine("   vcCarType as 'CFC', \n");
                strSql.AppendLine("   isnull(iQuantityPercontainer,0) as 'OrdLot', \n");
                strSql.AppendLine("   isnull(iBoxes,0) as 'N Units', \n");
                strSql.AppendLine("   isnull(iPartNums,0) as 'N PCS', \n");
                strSql.AppendLine("   isnull(iD1,0) as iD1, \n");
                strSql.AppendLine("   isnull(iD2,0) as iD2, \n");
                strSql.AppendLine("   isnull(iD3,0) as iD3, \n");
                strSql.AppendLine("   isnull(iD4,0) as iD4, \n");
                strSql.AppendLine("   isnull(iD5,0) as iD5, \n");
                strSql.AppendLine("   isnull(iD6,0) as iD6, \n");
                strSql.AppendLine("   isnull(iD7,0) as iD7, \n");
                strSql.AppendLine("   isnull(iD8,0) as iD8, \n");
                strSql.AppendLine("   isnull(iD9,0) as iD9, \n");
                strSql.AppendLine("   isnull(iD10,0) as iD10, \n");
                strSql.AppendLine("   isnull(iD11,0) as iD11, \n");
                strSql.AppendLine("   isnull(iD12,0) as iD12, \n");
                strSql.AppendLine("   isnull(iD13,0) as iD13, \n");
                strSql.AppendLine("   isnull(iD14,0) as iD14, \n");
                strSql.AppendLine("   isnull(iD15,0) as iD15, \n");
                strSql.AppendLine("   isnull(iD16,0) as iD16, \n");
                strSql.AppendLine("   isnull(iD17,0) as iD17, \n");
                strSql.AppendLine("   isnull(iD18,0) as iD18, \n");
                strSql.AppendLine("   isnull(iD19,0) as iD19, \n");
                strSql.AppendLine("   isnull(iD20,0) as iD20, \n");
                strSql.AppendLine("   isnull(iD21,0) as iD21, \n");
                strSql.AppendLine("   isnull(iD22,0) as iD22, \n");
                strSql.AppendLine("   isnull(iD23,0) as iD23, \n");
                strSql.AppendLine("   isnull(iD24,0) as iD24, \n");
                strSql.AppendLine("   isnull(iD25,0) as iD25, \n");
                strSql.AppendLine("   isnull(iD26,0) as iD26, \n");
                strSql.AppendLine("   isnull(iD27,0) as iD27, \n");
                strSql.AppendLine("   isnull(iD28,0) as iD28, \n");
                strSql.AppendLine("   isnull(iD29,0) as iD29, \n");
                strSql.AppendLine("   isnull(iD30,0) as iD30, \n");
                strSql.AppendLine("   isnull(iD31,0) as iD31, \n");
                strSql.AppendLine("   iAutoId \n");
                strSql.AppendLine("   FROM TSOQReply WHERE vcCLYM='" + vcCLYM + "' and vcInOutFlag='0'  AND vcDXYM in ('" + strYearMonth + "') and vcFZGC in (" + plants + ")  \n");//内制
                strSql.AppendLine(" ) a  \n");

                strSql.AppendLine(" LEFT JOIN (   \n");
                strSql.AppendLine("   SELECT vcPart_id,isnull(iQuantityPercontainer,0) as 'N+1 O/L',isnull(iBoxes,0) as 'N+1 Units',isnull(iPartNums,0) as 'N+1 PCS'  \n");
                strSql.AppendLine("   FROM TSOQReply   \n");
                strSql.AppendLine("   WHERE vcCLYM='" + vcCLYM + "' and vcInOutFlag='0'  AND vcDXYM='" + strYearMonth_2 + "'  and vcFZGC in (" + plants + ")  \n");//内制
                strSql.AppendLine("  ) b  \n");
                strSql.AppendLine(" ON a.PartsNo=b.vcPart_id  \n");

                strSql.AppendLine(" LEFT JOIN (   \n");
                strSql.AppendLine("   SELECT vcPart_id,isnull(iQuantityPercontainer,0) as 'N+2 O/L',isnull(iBoxes,0) as 'N+2 Units',isnull(iPartNums,0) as 'N+2 PCS'  \n");
                strSql.AppendLine("   FROM TSOQReply   \n");
                strSql.AppendLine("   WHERE vcCLYM='" + vcCLYM + "' and vcInOutFlag='0'  AND vcDXYM='" + strYearMonth_3 + "'  and vcFZGC in (" + plants + ")  \n");//内制
                strSql.AppendLine("  ) c  \n");
                strSql.AppendLine(" ON a.PartsNo=c.vcPart_id  \n");

                strSql.AppendLine("left join (select * from TCode where vcCodeId='C047')d on a.vcMakingOrderType=d.vcValue    \n");
                strSql.AppendLine("left join (select vcValue,vcName from TCode where vcCodeId='C000')e on a.vcFZGC=e.vcValue ");

                strSql.AppendLine(" order by a.iAutoId ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strYearMonth, string strUserId, List<string> plantList)
        {
            try
            {
                string plants = "";
                for (int i = 0; i < plantList.Count; i++)
                {
                    plants += "'" + plantList[i] + "',";
                }
                plants = plants.Substring(0, plants.Length - 1);
                string vcCLYM = System.DateTime.Now.ToString("yyyyMM");
                StringBuilder sql = new StringBuilder();
                sql.Append("      select * into #TSOQReply from       \n");
                sql.Append("      (      \n");
                sql.Append("      	select       \n");
                sql.Append("       vcPart_id,iBoxes,iQuantityPercontainer      \n");
                sql.Append("       ,iD1 ,iD2 ,iD3 ,iD4 ,iD5 ,iD6 ,iD7 ,iD8 ,iD9 ,iD10   \n");
                sql.Append("       ,iD11 ,iD12 ,iD13 ,iD14 ,iD15 ,iD16 ,iD17 ,iD18 ,iD19 ,iD20   \n");
                sql.Append("       ,iD21 ,iD22 ,iD23 ,iD24 ,iD25 ,iD26 ,iD27 ,iD28 ,iD29 ,iD30 ,iD31  \n");
                sql.Append("      	  from TSOQReply where 1=0      \n");
                sql.Append("      ) a      ;\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcPart_id"].ToString() == "")
                        continue;
                    sql.Append("            \n");
                    sql.Append("      insert into #TSOQReply       \n");
                    sql.Append("       (         \n");
                    sql.Append("       vcPart_id,iBoxes,iQuantityPercontainer      \n");
                    sql.Append("       ,iD1 ,iD2 ,iD3 ,iD4 ,iD5 ,iD6 ,iD7 ,iD8 ,iD9 ,iD10   \n");
                    sql.Append("       ,iD11 ,iD12 ,iD13 ,iD14 ,iD15 ,iD16 ,iD17 ,iD18 ,iD19 ,iD20   \n");
                    sql.Append("       ,iD21 ,iD22 ,iD23 ,iD24 ,iD25 ,iD26 ,iD27 ,iD28 ,iD29 ,iD30 ,iD31  \n");
                    sql.Append("       ) values         \n");
                    sql.Append("      (      \n");
                    sql.Append("      '" + dt.Rows[i]["vcPart_id"].ToString() + "',      \n");
                    sql.Append("      nullif('" + dt.Rows[i]["iBoxes"].ToString() + "',''),     \n");
                    sql.Append("      nullif('" + dt.Rows[i]["iQuantityPercontainer"].ToString() + "','')      \n");

                    for (int j = 1; j < 32; j++)
                    {
                        sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["iD" + j], true) + "      \n");

                        //string strSRS = dt.Rows[i]["iQuantityPercontainer"] == System.DBNull.Value ? "" : dt.Rows[i]["iQuantityPercontainer"].ToString();//箱数*收容数
                        //string strIDTemp = dt.Rows[i]["iD" + j] == System.DBNull.Value ? "" : dt.Rows[i]["iD" + j].ToString();
                        //if (strIDTemp != "" && Convert.ToInt32(strIDTemp) != 0)
                        //{
                        //    int iSRS = Convert.ToInt32(strSRS.Trim());//收容数
                        //    int iD = Convert.ToInt32(strIDTemp.Trim()) / iSRS;
                        //    sql.Append("      ," + iD + "      \n");
                        //}
                        //else
                        //    sql.Append("      ,null      \n");
                    }
                    sql.Append("      );      \n");
                }

                sql.Append("update t1 set t1.vcOperatorID='" + strUserId + "',t1.dOperatorTime=GETDATE(),t1.iBoxes=t2.iBoxes,iPartNums=t2.iQuantityPercontainer*t2.iBoxes,   \n");
                for (int j = 1; j < 32; j++)
                {
                    sql.Append(" t1.iD" + j + "=t2.iD" + j);
                    if (j != 31)
                        sql.Append(",");
                }
                sql.Append("    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoqReply    \n");
                sql.Append("	where vcCLYM='" + vcCLYM + "' and vcDXYM='" + strYearMonth + "' and vcInOutFlag='0' and vcFZGC in (" + plants + ")    \n");
                sql.Append(")t1    \n");
                sql.Append("inner join (    \n");
                sql.Append("	select * from #TSOQReply    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPart_id    \n");
                //清空3个月展开人和展开时间
                sql.Append("update t1 set vcZhanKaiID=null,dZhanKaiTime=null   \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoqReply    \n");
                sql.Append("	where vcCLYM='" + vcCLYM + "' and vcInOutFlag='0' and vcFZGC in (" + plants + ")    \n");
                sql.Append(")t1    \n");
                sql.Append("inner join (    \n");
                sql.Append("	select * from #TSOQReply    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPart_id    \n");

                sql.Append("delete t1    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoqReply    \n");
                sql.Append("	where vcCLYM='" + vcCLYM + "'  and vcInOutFlag='0' and vcFZGC in (" + plants + ")    \n");
                sql.Append(")t1    \n");
                sql.Append("left join (    \n");
                sql.Append("	select * from #TSOQReply    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPart_id    \n");
                sql.Append("where t2.vcPart_id is null    \n");

                //sql.Append("      update TSOQReply set vcOperatorID='" + strUserId + "',dOperatorTime=getdate(),vcFZGC=b.vcFZGC,vcMakingOrderType=b.vcMakingOrderType,      \n");
                //sql.Append("      vcCarType=b.vcCarType,iQuantityPercontainer=b.iQuantityPercontainer,iBoxes=b.iBoxes,iPartNums=b.iPartNums      \n");
                //for (int j = 1; j < 32; j++)
                //{
                //    sql.Append(" iD" + j + "=b.iD" + j);
                //    if (j != 31)
                //        sql.Append(",");
                //}
                //sql.Append("      from   TSOQReply a    \n");
                //sql.Append("      inner join(      \n");
                //sql.Append("        select * from #TSOQReply      \n");
                //sql.Append("      ) b on a.vcPart_id=b.vcPart_id      \n");
                //sql.Append("      and  a.vcInOutFlag='0'   \n ");//内制
                //sql.Append("      and a.vcCLYM='"+vcCLYM+"' and  a.vcDXYM='" + strYearMonth + "'; \n  ");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取展开的数据
        public DataTable getZhankaiData(bool isZhankai, string strPlant)
        {
            StringBuilder sql = new StringBuilder();
            string vcCLYM = System.DateTime.Now.ToString("yyyyMM");
            sql.Append("   select * from TSOQReply where  vcCLYM='" + vcCLYM + "' and vcInOutFlag='0' and vcFZGC='" + strPlant + "' \n");
            if (isZhankai)
                sql.Append("  and dZhanKaiTime is not null  ");
            else
                sql.Append("  and dZhanKaiTime is null  ");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion

        public int isHaveSORReplyData(string strPlant, string strCLYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(1) as num from TSoqReply where vcCLYM='" + strCLYM + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='0'    \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int isZhankai(string strPlant, string strCLYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(1) as num from TSoqReply where vcCLYM='" + strCLYM + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='0' and dZhanKaiTime is not null    \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int isSCPlan(string strPlant, string strCLYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(1) as nums from (select montouch,vcPartsNo,vcDock,vcCarType from MonthProPlanTblTMP where montouch is not null) t1 \n");
                sql.Append("full join (select vcMonth,vcPartsNo,vcDock,vcCarType from MonthProPlanTblTMP where montouch is null) t2  \n");
                sql.Append("on t1.montouch=t2.vcMonth and t1.vcPartsno=t2.vcPartsno  \n");
                sql.Append("and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType  \n");
                sql.Append("left join (select vcPartsNo,vcDock,vcCarFamilyCode,vcPartPlant from tPartInfoMaster where dTimeFrom<='" + strCLYM.Substring(0, 4) + "-" + strCLYM.Substring(4, 2) + "-01" + "' and dTimeTo>='" + strCLYM.Substring(0, 4) + "-" + strCLYM.Substring(4, 2) + "-01" + "') t3 \n");
                sql.Append("on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock=t2.vcDock and t3.vcCarFamilyCode=t2.vcCarType  \n");
                sql.Append("where t2.vcMonth='" + strCLYM.Substring(0, 4) + "-" + strCLYM.Substring(4, 2) + "' and t3.vcPartPlant='" + strPlant + "' \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetFilePlant(string strCLYM, DataTable dt)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("      select * into #TSOQReply from       \n");
                sql.Append("      (      \n");
                sql.Append("      	select * from TSOQReply where 1=0      \n");
                sql.Append("      ) a      ;\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcPart_id"].ToString() == "")
                        continue;
                    sql.Append("      insert into #TSOQReply       \n");
                    sql.Append("       (vcPart_id) values        \n");
                    sql.Append("      ('" + dt.Rows[i]["vcPart_id"].ToString() + "')      \n");
                }
                sql.Append("select distinct t1.vcFZGC    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoqReply    \n");
                sql.Append("	where vcCLYM='" + strCLYM + "'  --and vcInOutFlag='0'     \n");
                sql.Append(")t1    \n");
                sql.Append("inner join (    \n");
                sql.Append("	select * from #TSOQReply    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPart_id    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 获取平准化加减天数
        public DataTable getPingZhunAddSubDay()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT vcValue1 FROM dbo.TOutCode WHERE vcCodeId = 'C027'AND vcIsColum = '0' ");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 生产计划方法（王立伟）2020-01-21

        FS1203_DataAccess ICalendar2 = new FS1203_DataAccess();

        #region 获取展开的数据
        public DataTable getProData(string vcPlant, string vcYM)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from TSOQReply where  vcCLYM=convert(varchar(6),getdate(),112) and vcInOutFlag='0' and vcFZGC='" + vcPlant + "' \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion

        #region 上传更新生产计划
        public string updatePro(DataTable dt, string user, string mon, ref Exception e, string plant)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            SqlDataAdapter apt = new SqlDataAdapter(cmd);
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string partsno = dt.Rows[i]["vcPartsno"].ToString().Replace("-", "").Trim();
                    string month = dt.Rows[i]["vcMonth"].ToString();
                    string vcDock = dt.Rows[i]["vcDock"].ToString();
                    string vcCarType = dt.Rows[i]["vcCarType"].ToString();
                    cmd.CommandText = " select iQuantityPerContainer from tPartInfoMaster where vcPartsno = '" + partsno + "' and vcDock ='" + vcDock + "' and vcCarFamilyCode ='" + vcCarType + "'   and dTimeFrom<= '" + month + "-01" + "' and dTimeTo >= '" + month + "-01" + "' ";
                    DataTable srsdt = new DataTable();
                    apt.Fill(srsdt);
                    cmd.CommandText = " select * from MonthProPlanTblTMP where montouch='" + month + "' and vcPartsno='" + partsno + "' and vcDock='" + vcDock + "' and vcCarType='" + vcCarType + "'";
                    DataTable dt_udt = new DataTable();
                    apt.Fill(dt_udt);
                    SqlCommandBuilder sb = new SqlCommandBuilder(apt);
                    #region 赋值
                    if (dt_udt.Rows.Count == 1)
                    {
                        int srs = Convert.ToInt32(srsdt.Rows[0]["iQuantityPerContainer"]);
                        for (int j = 7, k = 14; j < dt_udt.Columns.Count - 5; j++, k++)
                        {
                            if (dt_udt.Rows[0][j].ToString().Trim().Length > 0 && dt.Rows[i][k].ToString().Trim().Length == 0)
                            {
                                msg = "第" + (i + 1).ToString() + "行，第" + (j + 1).ToString() + "列 稼动日历或LeaderTime在计划做成过程中修改，请重新导入SOQREPLY文件。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                            if (dt.Rows[i][k].ToString().Trim().Length > 0 && Convert.ToInt32(dt.Rows[i][k]) % srs != 0)
                            {
                                msg = "品番：" + partsno + "，受入：" + vcDock + " 数量维护需为收容数(" + srs + ")的倍数。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                        }
                        dt_udt.Rows[0]["vcD1b"] = dt.Rows[i]["TD1b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD1b"]) : dt.Rows[i]["TD1b"];
                        dt_udt.Rows[0]["vcD1y"] = dt.Rows[i]["TD1y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD1y"]) : dt.Rows[i]["TD1y"];
                        dt_udt.Rows[0]["vcD2b"] = dt.Rows[i]["TD2b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD2b"]) : dt.Rows[i]["TD2b"];
                        dt_udt.Rows[0]["vcD2y"] = dt.Rows[i]["TD2y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD2y"]) : dt.Rows[i]["TD2y"];
                        dt_udt.Rows[0]["vcD3b"] = dt.Rows[i]["TD3b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD3b"]) : dt.Rows[i]["TD3b"];
                        dt_udt.Rows[0]["vcD3y"] = dt.Rows[i]["TD3y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD3y"]) : dt.Rows[i]["TD3y"];
                        dt_udt.Rows[0]["vcD4b"] = dt.Rows[i]["TD4b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD4b"]) : dt.Rows[i]["TD4b"];
                        dt_udt.Rows[0]["vcD4y"] = dt.Rows[i]["TD4y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD4y"]) : dt.Rows[i]["TD4y"];
                        dt_udt.Rows[0]["vcD5b"] = dt.Rows[i]["TD5b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD5b"]) : dt.Rows[i]["TD5b"];
                        dt_udt.Rows[0]["vcD5y"] = dt.Rows[i]["TD5y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD5y"]) : dt.Rows[i]["TD5y"];
                        dt_udt.Rows[0]["vcD6b"] = dt.Rows[i]["TD6b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD6b"]) : dt.Rows[i]["TD6b"];
                        dt_udt.Rows[0]["vcD6y"] = dt.Rows[i]["TD6y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD6y"]) : dt.Rows[i]["TD6y"];
                        dt_udt.Rows[0]["vcD7b"] = dt.Rows[i]["TD7b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD7b"]) : dt.Rows[i]["TD7b"];
                        dt_udt.Rows[0]["vcD7y"] = dt.Rows[i]["TD7y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD7y"]) : dt.Rows[i]["TD7y"];
                        dt_udt.Rows[0]["vcD8b"] = dt.Rows[i]["TD8b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD8b"]) : dt.Rows[i]["TD8b"];
                        dt_udt.Rows[0]["vcD8y"] = dt.Rows[i]["TD8y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD8y"]) : dt.Rows[i]["TD8y"];
                        dt_udt.Rows[0]["vcD9b"] = dt.Rows[i]["TD9b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD9b"]) : dt.Rows[i]["TD9b"];
                        dt_udt.Rows[0]["vcD9y"] = dt.Rows[i]["TD9y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD9y"]) : dt.Rows[i]["TD9y"];
                        dt_udt.Rows[0]["vcD10b"] = dt.Rows[i]["TD10b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD10b"]) : dt.Rows[i]["TD10b"];
                        dt_udt.Rows[0]["vcD10y"] = dt.Rows[i]["TD10y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD10y"]) : dt.Rows[i]["TD10y"];
                        dt_udt.Rows[0]["vcD11b"] = dt.Rows[i]["TD11b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD11b"]) : dt.Rows[i]["TD11b"];
                        dt_udt.Rows[0]["vcD11y"] = dt.Rows[i]["TD11y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD11y"]) : dt.Rows[i]["TD11y"];
                        dt_udt.Rows[0]["vcD12b"] = dt.Rows[i]["TD12b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD12b"]) : dt.Rows[i]["TD12b"];
                        dt_udt.Rows[0]["vcD12y"] = dt.Rows[i]["TD12y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD12y"]) : dt.Rows[i]["TD12y"];
                        dt_udt.Rows[0]["vcD13b"] = dt.Rows[i]["TD13b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD13b"]) : dt.Rows[i]["TD13b"];
                        dt_udt.Rows[0]["vcD13y"] = dt.Rows[i]["TD13y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD13y"]) : dt.Rows[i]["TD13y"];
                        dt_udt.Rows[0]["vcD14b"] = dt.Rows[i]["TD14b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD14b"]) : dt.Rows[i]["TD14b"];
                        dt_udt.Rows[0]["vcD14y"] = dt.Rows[i]["TD14y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD14y"]) : dt.Rows[i]["TD14y"];
                        dt_udt.Rows[0]["vcD15b"] = dt.Rows[i]["TD15b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD15b"]) : dt.Rows[i]["TD15b"];
                        dt_udt.Rows[0]["vcD15y"] = dt.Rows[i]["TD15y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD15y"]) : dt.Rows[i]["TD15y"];
                        dt_udt.Rows[0]["vcD16b"] = dt.Rows[i]["TD16b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD16b"]) : dt.Rows[i]["TD16b"];
                        dt_udt.Rows[0]["vcD16y"] = dt.Rows[i]["TD16y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD16y"]) : dt.Rows[i]["TD16y"];
                        dt_udt.Rows[0]["vcD17b"] = dt.Rows[i]["TD17b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD17b"]) : dt.Rows[i]["TD17b"];
                        dt_udt.Rows[0]["vcD17y"] = dt.Rows[i]["TD17y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD17y"]) : dt.Rows[i]["TD17y"];
                        dt_udt.Rows[0]["vcD18b"] = dt.Rows[i]["TD18b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD18b"]) : dt.Rows[i]["TD18b"];
                        dt_udt.Rows[0]["vcD18y"] = dt.Rows[i]["TD18y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD18y"]) : dt.Rows[i]["TD18y"];
                        dt_udt.Rows[0]["vcD19b"] = dt.Rows[i]["TD19b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD19b"]) : dt.Rows[i]["TD19b"];
                        dt_udt.Rows[0]["vcD19y"] = dt.Rows[i]["TD19y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD19y"]) : dt.Rows[i]["TD19y"];
                        dt_udt.Rows[0]["vcD20b"] = dt.Rows[i]["TD20b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD20b"]) : dt.Rows[i]["TD20b"];
                        dt_udt.Rows[0]["vcD20y"] = dt.Rows[i]["TD20y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD20y"]) : dt.Rows[i]["TD20y"];
                        dt_udt.Rows[0]["vcD21b"] = dt.Rows[i]["TD21b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD21b"]) : dt.Rows[i]["TD21b"];
                        dt_udt.Rows[0]["vcD21y"] = dt.Rows[i]["TD21y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD21y"]) : dt.Rows[i]["TD21y"];
                        dt_udt.Rows[0]["vcD22b"] = dt.Rows[i]["TD22b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD22b"]) : dt.Rows[i]["TD22b"];
                        dt_udt.Rows[0]["vcD22y"] = dt.Rows[i]["TD22y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD22y"]) : dt.Rows[i]["TD22y"];
                        dt_udt.Rows[0]["vcD23b"] = dt.Rows[i]["TD23b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD23b"]) : dt.Rows[i]["TD23b"];
                        dt_udt.Rows[0]["vcD23y"] = dt.Rows[i]["TD23y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD23y"]) : dt.Rows[i]["TD23y"];
                        dt_udt.Rows[0]["vcD24b"] = dt.Rows[i]["TD24b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD24b"]) : dt.Rows[i]["TD24b"];
                        dt_udt.Rows[0]["vcD24y"] = dt.Rows[i]["TD24y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD24y"]) : dt.Rows[i]["TD24y"];
                        dt_udt.Rows[0]["vcD25b"] = dt.Rows[i]["TD25b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD25b"]) : dt.Rows[i]["TD25b"];
                        dt_udt.Rows[0]["vcD25y"] = dt.Rows[i]["TD25y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD25y"]) : dt.Rows[i]["TD25y"];
                        dt_udt.Rows[0]["vcD26b"] = dt.Rows[i]["TD26b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD26b"]) : dt.Rows[i]["TD26b"];
                        dt_udt.Rows[0]["vcD26y"] = dt.Rows[i]["TD26y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD26y"]) : dt.Rows[i]["TD26y"];
                        dt_udt.Rows[0]["vcD27b"] = dt.Rows[i]["TD27b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD27b"]) : dt.Rows[i]["TD27b"];
                        dt_udt.Rows[0]["vcD27y"] = dt.Rows[i]["TD27y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD27y"]) : dt.Rows[i]["TD27y"];
                        dt_udt.Rows[0]["vcD28b"] = dt.Rows[i]["TD28b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD28b"]) : dt.Rows[i]["TD28b"];
                        dt_udt.Rows[0]["vcD28y"] = dt.Rows[i]["TD28y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD28y"]) : dt.Rows[i]["TD28y"];
                        dt_udt.Rows[0]["vcD29b"] = dt.Rows[i]["TD29b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD29b"]) : dt.Rows[i]["TD29b"];
                        dt_udt.Rows[0]["vcD29y"] = dt.Rows[i]["TD29y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD29y"]) : dt.Rows[i]["TD29y"];
                        dt_udt.Rows[0]["vcD30b"] = dt.Rows[i]["TD30b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD30b"]) : dt.Rows[i]["TD30b"];
                        dt_udt.Rows[0]["vcD30y"] = dt.Rows[i]["TD30y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD30y"]) : dt.Rows[i]["TD30y"];
                        dt_udt.Rows[0]["vcD31b"] = dt.Rows[i]["TD31b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD31b"]) : dt.Rows[i]["TD31b"];
                        dt_udt.Rows[0]["vcD31y"] = dt.Rows[i]["TD31y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD31y"]) : dt.Rows[i]["TD31y"];
                        dt_udt.Rows[0]["CUPDUSER"] = user;
                        dt_udt.Rows[0]["DUPDTIME"] = DateTime.Now;
                        dt_udt.Rows[0]["CUPDUSER"] = user;
                        dt_udt.Rows[0]["DUPDTIME"] = DateTime.Now;
                        sb = new SqlCommandBuilder(apt);
                        apt.Update(dt_udt);
                    }
                    cmd.CommandText = " select * from MonthProPlanTblTMP where vcMonth='" + month + "' and vcPartsno='" + partsno + "' and vcDock='" + vcDock + "' and vcCarType='" + vcCarType + "'";
                    dt_udt = new DataTable();
                    apt.Fill(dt_udt);
                    if (dt_udt.Rows.Count == 1)
                    {
                        int srs = Convert.ToInt32(srsdt.Rows[0]["iQuantityPerContainer"]);
                        for (int j = 7, k = 76; j < dt_udt.Columns.Count - 5; j++, k++)
                        {
                            if (dt_udt.Rows[0][j].ToString().Trim().Length > 0 && dt.Rows[i][k].ToString().Trim().Length == 0)
                            {
                                msg = "稼动日历或LeaderTime在计划做成过程中修改，请重新导入SOQREPLY文件。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                            if (dt.Rows[i][k].ToString().Trim().Length > 0 && Convert.ToInt32(dt.Rows[i][k]) % srs != 0)
                            {
                                msg = "品番：" + partsno + "，受入：" + vcDock + " 数量维护需为收容数(" + srs + ")的倍数。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                        }
                        dt_udt.Rows[0]["vcD1b"] = dt.Rows[i]["ED1b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED1b"]) : dt.Rows[i]["ED1b"];
                        dt_udt.Rows[0]["vcD1y"] = dt.Rows[i]["ED1y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED1y"]) : dt.Rows[i]["ED1y"];
                        dt_udt.Rows[0]["vcD2b"] = dt.Rows[i]["ED2b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED2b"]) : dt.Rows[i]["ED2b"];
                        dt_udt.Rows[0]["vcD2y"] = dt.Rows[i]["ED2y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED2y"]) : dt.Rows[i]["ED2y"];
                        dt_udt.Rows[0]["vcD3b"] = dt.Rows[i]["ED3b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED3b"]) : dt.Rows[i]["ED3b"];
                        dt_udt.Rows[0]["vcD3y"] = dt.Rows[i]["ED3y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED3y"]) : dt.Rows[i]["ED3y"];
                        dt_udt.Rows[0]["vcD4b"] = dt.Rows[i]["ED4b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED4b"]) : dt.Rows[i]["ED4b"];
                        dt_udt.Rows[0]["vcD4y"] = dt.Rows[i]["ED4y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED4y"]) : dt.Rows[i]["ED4y"];
                        dt_udt.Rows[0]["vcD5b"] = dt.Rows[i]["ED5b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED5b"]) : dt.Rows[i]["ED5b"];
                        dt_udt.Rows[0]["vcD5y"] = dt.Rows[i]["ED5y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED5y"]) : dt.Rows[i]["ED5y"];
                        dt_udt.Rows[0]["vcD6b"] = dt.Rows[i]["ED6b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED6b"]) : dt.Rows[i]["ED6b"];
                        dt_udt.Rows[0]["vcD6y"] = dt.Rows[i]["ED6y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED6y"]) : dt.Rows[i]["ED6y"];
                        dt_udt.Rows[0]["vcD7b"] = dt.Rows[i]["ED7b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED7b"]) : dt.Rows[i]["ED7b"];
                        dt_udt.Rows[0]["vcD7y"] = dt.Rows[i]["ED7y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED7y"]) : dt.Rows[i]["ED7y"];
                        dt_udt.Rows[0]["vcD8b"] = dt.Rows[i]["ED8b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED8b"]) : dt.Rows[i]["ED8b"];
                        dt_udt.Rows[0]["vcD8y"] = dt.Rows[i]["ED8y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED8y"]) : dt.Rows[i]["ED8y"];
                        dt_udt.Rows[0]["vcD9b"] = dt.Rows[i]["ED9b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED9b"]) : dt.Rows[i]["ED9b"];
                        dt_udt.Rows[0]["vcD9y"] = dt.Rows[i]["ED9y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED9y"]) : dt.Rows[i]["ED9y"];
                        dt_udt.Rows[0]["vcD10b"] = dt.Rows[i]["ED10b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED10b"]) : dt.Rows[i]["ED10b"];
                        dt_udt.Rows[0]["vcD10y"] = dt.Rows[i]["ED10y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED10y"]) : dt.Rows[i]["ED10y"];
                        dt_udt.Rows[0]["vcD11b"] = dt.Rows[i]["ED11b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED11b"]) : dt.Rows[i]["ED11b"];
                        dt_udt.Rows[0]["vcD11y"] = dt.Rows[i]["ED11y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED11y"]) : dt.Rows[i]["ED11y"];
                        dt_udt.Rows[0]["vcD12b"] = dt.Rows[i]["ED12b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED12b"]) : dt.Rows[i]["ED12b"];
                        dt_udt.Rows[0]["vcD12y"] = dt.Rows[i]["ED12y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED12y"]) : dt.Rows[i]["ED12y"];
                        dt_udt.Rows[0]["vcD13b"] = dt.Rows[i]["ED13b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED13b"]) : dt.Rows[i]["ED13b"];
                        dt_udt.Rows[0]["vcD13y"] = dt.Rows[i]["ED13y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED13y"]) : dt.Rows[i]["ED13y"];
                        dt_udt.Rows[0]["vcD14b"] = dt.Rows[i]["ED14b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED14b"]) : dt.Rows[i]["ED14b"];
                        dt_udt.Rows[0]["vcD14y"] = dt.Rows[i]["ED14y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED14y"]) : dt.Rows[i]["ED14y"];
                        dt_udt.Rows[0]["vcD15b"] = dt.Rows[i]["ED15b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED15b"]) : dt.Rows[i]["ED15b"];
                        dt_udt.Rows[0]["vcD15y"] = dt.Rows[i]["ED15y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED15y"]) : dt.Rows[i]["ED15y"];
                        dt_udt.Rows[0]["vcD16b"] = dt.Rows[i]["ED16b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED16b"]) : dt.Rows[i]["ED16b"];
                        dt_udt.Rows[0]["vcD16y"] = dt.Rows[i]["ED16y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED16y"]) : dt.Rows[i]["ED16y"];
                        dt_udt.Rows[0]["vcD17b"] = dt.Rows[i]["ED17b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED17b"]) : dt.Rows[i]["ED17b"];
                        dt_udt.Rows[0]["vcD17y"] = dt.Rows[i]["ED17y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED17y"]) : dt.Rows[i]["ED17y"];
                        dt_udt.Rows[0]["vcD18b"] = dt.Rows[i]["ED18b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED18b"]) : dt.Rows[i]["ED18b"];
                        dt_udt.Rows[0]["vcD18y"] = dt.Rows[i]["ED18y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED18y"]) : dt.Rows[i]["ED18y"];
                        dt_udt.Rows[0]["vcD19b"] = dt.Rows[i]["ED19b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED19b"]) : dt.Rows[i]["ED19b"];
                        dt_udt.Rows[0]["vcD19y"] = dt.Rows[i]["ED19y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED19y"]) : dt.Rows[i]["ED19y"];
                        dt_udt.Rows[0]["vcD20b"] = dt.Rows[i]["ED20b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED20b"]) : dt.Rows[i]["ED20b"];
                        dt_udt.Rows[0]["vcD20y"] = dt.Rows[i]["ED20y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED20y"]) : dt.Rows[i]["ED20y"];
                        dt_udt.Rows[0]["vcD21b"] = dt.Rows[i]["ED21b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED21b"]) : dt.Rows[i]["ED21b"];
                        dt_udt.Rows[0]["vcD21y"] = dt.Rows[i]["ED21y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED21y"]) : dt.Rows[i]["ED21y"];
                        dt_udt.Rows[0]["vcD22b"] = dt.Rows[i]["ED22b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED22b"]) : dt.Rows[i]["ED22b"];
                        dt_udt.Rows[0]["vcD22y"] = dt.Rows[i]["ED22y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED22y"]) : dt.Rows[i]["ED22y"];
                        dt_udt.Rows[0]["vcD23b"] = dt.Rows[i]["ED23b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED23b"]) : dt.Rows[i]["ED23b"];
                        dt_udt.Rows[0]["vcD23y"] = dt.Rows[i]["ED23y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED23y"]) : dt.Rows[i]["ED23y"];
                        dt_udt.Rows[0]["vcD24b"] = dt.Rows[i]["ED24b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED24b"]) : dt.Rows[i]["ED24b"];
                        dt_udt.Rows[0]["vcD24y"] = dt.Rows[i]["ED24y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED24y"]) : dt.Rows[i]["ED24y"];
                        dt_udt.Rows[0]["vcD25b"] = dt.Rows[i]["ED25b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED25b"]) : dt.Rows[i]["ED25b"];
                        dt_udt.Rows[0]["vcD25y"] = dt.Rows[i]["ED25y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED25y"]) : dt.Rows[i]["ED25y"];
                        dt_udt.Rows[0]["vcD26b"] = dt.Rows[i]["ED26b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED26b"]) : dt.Rows[i]["ED26b"];
                        dt_udt.Rows[0]["vcD26y"] = dt.Rows[i]["ED26y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED26y"]) : dt.Rows[i]["ED26y"];
                        dt_udt.Rows[0]["vcD27b"] = dt.Rows[i]["ED27b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED27b"]) : dt.Rows[i]["ED27b"];
                        dt_udt.Rows[0]["vcD27y"] = dt.Rows[i]["ED27y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED27y"]) : dt.Rows[i]["ED27y"];
                        dt_udt.Rows[0]["vcD28b"] = dt.Rows[i]["ED28b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED28b"]) : dt.Rows[i]["ED28b"];
                        dt_udt.Rows[0]["vcD28y"] = dt.Rows[i]["ED28y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED28y"]) : dt.Rows[i]["ED28y"];
                        dt_udt.Rows[0]["vcD29b"] = dt.Rows[i]["ED29b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED29b"]) : dt.Rows[i]["ED29b"];
                        dt_udt.Rows[0]["vcD29y"] = dt.Rows[i]["ED29y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED29y"]) : dt.Rows[i]["ED29y"];
                        dt_udt.Rows[0]["vcD30b"] = dt.Rows[i]["ED30b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED30b"]) : dt.Rows[i]["ED30b"];
                        dt_udt.Rows[0]["vcD30y"] = dt.Rows[i]["ED30y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED30y"]) : dt.Rows[i]["ED30y"];
                        dt_udt.Rows[0]["vcD31b"] = dt.Rows[i]["ED31b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED31b"]) : dt.Rows[i]["ED31b"];
                        dt_udt.Rows[0]["vcD31y"] = dt.Rows[i]["ED31y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED31y"]) : dt.Rows[i]["ED31y"];
                        dt_udt.Rows[0]["CUPDUSER"] = user;
                        dt_udt.Rows[0]["DUPDTIME"] = DateTime.Now;
                        sb = new SqlCommandBuilder(apt);
                        apt.Update(dt_udt);
                    }
                    #endregion
                }
                #region 更新其他计划临时表 -- 更新确定版计划表
                updateOtherPlan("MonthProPlanTblTMP", "MonthPackPlanTblTMP", cmd, apt, dt, user);//包装
                updateOtherPlan("MonthProPlanTblTMP", "MonthP3PlanTblTMP", cmd, apt, dt, user);//工程3
                updateOtherPlan("MonthProPlanTblTMP", "MonthTZPlanTblTMP", cmd, apt, dt, user);//涂装
                updateOtherPlan("MonthProPlanTblTMP", "MonthKanBanPlanTblTMP", cmd, apt, dt, user);//看板打印计划 20180928看板打印计划不包含周度品番 - 李兴旺

                deletePlan(cmd, mon, plant);//删除计划
                updatePlan(cmd, mon, plant);//更新确定版计划
                //deleteTMP(cmd, mon, plant);//删除临时计划
                deleteTMPplan(cmd, mon, plant);//删除多余计划

                UpdatePlanMST(cmd, mon, plant);//更新到计划品番数据表
                updateSoqReply(cmd, mon, apt, user, plant);//更新SOQREPLY表
                #endregion
                #region 生成打印数据
                msg = CreatOrderNo(cmd, mon, apt, user, plant);//2018-2-26增加AB值 - Malcolm.L 刘刚
                if (msg.Length > 0)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                    return msg;
                }
                #endregion
                cmd.Transaction.Commit();
                cmd.Connection.Close();

            }
            catch (Exception ex)
            {
                e = ex;
                msg = "计划生成失败！";
                if (cmd.Transaction != null) cmd.Transaction.Rollback();
                cmd.Connection.Close();
            }
            return msg;
        }
        #endregion

        #region 判断是否已导入SOQ
        /// <summary>
        /// 判断是否已导入SOQ
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        public DataTable getSoqInfo(string mon, string plant)
        {
            string mon1 = mon.Substring(0, 4) + "-" + mon.Substring(4, 2);
            string moncl = Convert.ToDateTime(mon.Substring(0, 4) + "-" + mon.Substring(4, 2) + "-01").AddMonths(-1).ToString("yyyyMM");
            string ssql = "";
            ssql += "  select t1.vcFZGC, t1.vcPart_id, t2.vcDock, t2.vcCarFamilyCode, t2.vcQJcontainer as kbsrs, t2.iQuantityPerContainer as srs, ";
            ssql += "  t1.iBoxes*t2.iQuantityPerContainer as num, t2.vcPorType, t2.vcZB, t3.KBpartType, t2.vcQFflag, ";
            ssql += "  t3.vcProName0,t3.vcLT0, t3.vcCalendar0, ";
            ssql += "  t3.vcProName1,t3.vcLT1, t3.vcCalendar1, ";
            ssql += "  t3.vcProName2,t3.vcLT2, t3.vcCalendar2, ";
            ssql += "  t3.vcProName3,t3.vcLT3, t3.vcCalendar3, ";
            ssql += "  t3.vcProName4,t3.vcLT4, t3.vcCalendar4,  ";
            ssql += "  t1.vcSupplier_id  ";
            ssql += "  from (select * from TSoqReply where vcInOutFlag='0' and vcDXYM='" + mon + "' and vcCLYM='" + moncl + "') t1 ";
            ssql += "  left join (select vcPartsNo, vcDock, vcCarFamilyCode, vcQJcontainer, iQuantityPerContainer,vcPorType, vcZB, vcQFflag,dTimeFrom,dTimeTo from tPartInfoMaster where dTimeFrom<='" + mon1 + "-01" + "' and dTimeTo>='" + mon1 + "-01" + "' and vcInOutFlag='0') t2 ";
            ssql += "  on t1.vcPart_id=t2.vcPartsNo and t1.vcCarType=t2.vcCarFamilyCode ";
            ssql += "  left join ProRuleMst t3 ";
            ssql += "  on t3.vcPorType=t2.vcPorType and t3.vcZB=t2.vcZB ";
            ssql += "  where iPartNums<>'0' and vcFZGC='" + plant + "' and t2.dTimeFrom<='" + mon1 + "-01" + "' and t2.dTimeTo>='" + mon1 + "-01" + "'";
            ssql += "  order by vcFZGC, vcPorType, vcZB, KBpartType ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }
        #endregion

        public DataTable getCalendarSP(int year, int mon, string gc, string zb, int Lt, string plant)//获取 根据Lt计算出的日历 非指定-#2
        {
            DataTable dt = new DataTable();
            DateTime curMon = Convert.ToDateTime(year.ToString() + "-" + mon.ToString() + "-1");
            int year_last = curMon.AddMonths(-1).Year;
            int year_month = curMon.AddMonths(-1).Month;
            string dboMon_current = swithMon(curMon.Month);
            string dboMon_last = swithMon(curMon.AddMonths(-1).Month);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("    declare @Lt int");
            sb.AppendLine("    declare @tmpa int ");
            sb.AppendLine("    declare @tmpb int ");
            sb.AppendLine("    DECLARE @zhinum int");
            sb.AppendLine("     DECLARE @banyue int   ");
            sb.AppendFormat("     set @Lt = {0}", Lt);
            sb.AppendLine("     SET @zhinum= (");
            sb.AppendLine("     --算出该月多少值");
            sb.AppendLine("    select top(1) row_number() over ( order by vcYear,vcMonth) as zhi from (");
            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
            sb.AppendLine(" )) P");

            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'  ) t  ", gc, zb, year, plant);
            sb.AppendLine("    order by zhi desc");
            sb.AppendLine("      )");
            sb.AppendLine("      set @tmpa = @zhinum");
            sb.AppendLine("      SET @tmpb= (");
            sb.AppendLine("     --算出该月多少值");
            sb.AppendLine("    select top(1) row_number() over ( order by vcYear,vcMonth) as zhi from (");
            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_last);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
            sb.AppendLine(" )) P");
            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'  ", gc, zb, year, plant);
            sb.AppendLine("    union all");
            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
            sb.AppendLine(" )) P");
            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'  ) t  ", gc, zb, year, plant);

            sb.AppendLine("    order by zhi desc");
            sb.AppendLine("      )");
            sb.AppendLine("      if @zhinum%2 = 1");
            sb.AppendLine("     	 begin");
            sb.AppendLine("     	 set  @banyue =(@zhinum+1)/2");
            sb.AppendLine("     	 end");
            sb.AppendLine("      else set  @banyue =@zhinum/2");
            sb.AppendLine("    ");
            sb.AppendLine("      select tt.* ,((tt.zhi-1)/10+1) as zhou , 0 as total from ");
            sb.AppendLine("    (");
            sb.AppendLine("    select tall.vcYear,tall.vcMonth,tall.vcGC,tall.vcZB,tall.dayall,tall.days ,row_number() over ( order by vcYear,vcMonth) as zhi from (");
            sb.AppendLine("    select t.*,row_number() over ( order by vcYear,vcMonth) as zhitmp from (");
            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_last);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
            sb.AppendLine(" )) P");
            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'", gc, zb, year, plant);
            sb.AppendLine("    union all");

            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
            sb.AppendLine(" )) P");
            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'  ) t", gc, zb, year, plant);
            sb.AppendLine("    ) tall");
            sb.AppendLine("    where tall.zhitmp >@tmpb-@tmpa-@Lt  and tall.zhitmp<=@tmpb-@Lt");
            sb.AppendLine("    ) tt");
            sb.AppendLine("    union all");
            sb.AppendLine("      select '0' ,'0','0' ,'0','0','0','9999', @banyue as banyue , @zhinum as totalzhi");

            try
            {
                return excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getCalendar(int year, int mon, string gc, string zb, string lastflag, string curflag, string a, string plant)
        {
            DataTable dt = new DataTable();
            DateTime curMon = Convert.ToDateTime(year.ToString() + "-" + mon.ToString() + "-1");
            int year_last = curMon.AddMonths(-1).Year;
            int year_month = curMon.AddMonths(-1).Month;
            string dboMon_current = swithMon(curMon.Month);
            string dboMon_last = swithMon(curMon.AddMonths(-1).Month);
            StringBuilder sb = new StringBuilder();
            if (a.Length == 0)
            {
                sb.AppendLine(" select t.*,row_number() over ( order by vcYear,vcMonth) as zhi from (");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_last);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}'  and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.AddMonths(-1).Year, plant, lastflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ");
                sb.AppendLine("  union all");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}'  and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.Year, plant, curflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine(" ) t");
            }
            if (a == "1")
            {
                sb.AppendLine("  DECLARE @zhinum int");
                sb.AppendLine("   DECLARE @banyue int   ");
                sb.AppendLine("   SET @zhinum= (");
                sb.AppendLine("   --算出该月多少值");
                sb.AppendLine("  select top(1) row_number() over ( order by vcYear,vcMonth) as zhi from (");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days from {0} ", dboMon_last);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.AddMonths(-1).Year, plant, lastflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ");
                sb.AppendLine("  union all");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days   from {0} ", dboMon_current);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.Year, plant, curflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ) t  ");
                sb.AppendLine("  order by zhi desc");
                sb.AppendLine("    )");
                sb.AppendLine("    ");
                sb.AppendLine("    if @zhinum%2 = 1");
                sb.AppendLine("   	 begin");
                sb.AppendLine("   	 set  @banyue =(@zhinum+1)/2");
                sb.AppendLine("   	 end");
                sb.AppendLine("    else set  @banyue =@zhinum/2");
                sb.AppendLine("  select tt.* ,((tt.zhi-1)/10+1) as zhou , 0 as total from ");
                sb.AppendLine("  (");
                sb.AppendLine("    select t.*,row_number() over ( order by vcYear,vcMonth) as zhi from (");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_last);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.AddMonths(-1).Year, plant, lastflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ");
                sb.AppendLine("  union all");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.Year, plant, curflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ) t");
                sb.AppendLine("  ) tt");
                sb.AppendLine("  union all ");
                sb.AppendLine("  ");
                sb.AppendLine("  select '0' ,'0','0' ,'0','0','0','9999', @banyue as banyue , @zhinum as totalzhi");
            }
            try
            {
                return excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string updateProPlan(List<DataTable> dt0, List<DataTable> dt1, List<DataTable> dt2, List<DataTable> dt3, List<DataTable> dt4, DataTable partsInfo, string user, string lbltime, string plant)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            string msg = "";
            try
            {
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                cmd.CommandTimeout = 0;
                TransactionPlan(dt0, cmd, partsInfo, "MonthKanBanPlanTblTMP", user, lbltime, plant, ref msg);
                if (msg.Length <= 0)
                {
                    TransactionPlan(dt1, cmd, partsInfo, "MonthProPlanTblTMP", user, lbltime, plant, ref msg);
                }
                if (msg.Length <= 0)
                {
                    TransactionPlan(dt2, cmd, partsInfo, "MonthTZPlanTblTMP", user, lbltime, plant, ref msg);
                }
                if (msg.Length <= 0)
                {
                    TransactionPlan(dt3, cmd, partsInfo, "MonthP3PlanTblTMP", user, lbltime, plant, ref msg);
                }
                if (msg.Length <= 0)
                {
                    TransactionPlan(dt4, cmd, partsInfo, "MonthPackPlanTblTMP", user, lbltime, plant, ref msg);
                }
                if (msg.Length <= 0)
                {
                    cmd.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                msg = "更新失败。" + ex.ToString();
                return msg;
            }
            finally
            {
                cmd.Connection.Close();
            }
            return msg;
        }
        public void TransactionPlan(List<DataTable> dt, SqlCommand cmd, DataTable partsInfo, string TableName, string user, string lbltime, string plant, ref string msg)
        {
            msg = "";
            try
            {
                lbltime = lbltime.Substring(0, 4) + "-" + lbltime.Substring(4, 2);
                DataTable dt2 = new DataTable();//20180929实测没用，是为了把变量apt引出 - 李兴旺
                cmd.CommandText = "select TOP(1) * from " + TableName;//20180929实测没用，是为了把变量apt引出 - 李兴旺
                SqlDataAdapter apt = new SqlDataAdapter(cmd);//20180929实测没用，是为了把变量apt引出 - 李兴旺
                apt.Fill(dt2);//20180929实测没用，是为了把变量apt引出 - 李兴旺
                cmd.CommandText = "delete from " + TableName + " where (vcMonth='" + lbltime + "' or montouch='" + lbltime + "') and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant='" + plant + "' and vcPartsNo=" + TableName + ".vcPartsno and dTimeFrom<='" + lbltime + "-01" + "' and dTimeTo>='" + lbltime + "-01" + "') ";
                cmd.ExecuteNonQuery();
                for (int i = 0; i < partsInfo.Rows.Count; i++)
                {
                    string vcPartsno = partsInfo.Rows[i]["vcPart_id"].ToString();
                    string vcDock = partsInfo.Rows[i]["vcDock"].ToString();
                    string vcCarType = partsInfo.Rows[i]["vcCarFamilyCode"].ToString();
                    string vcProjectName = partsInfo.Rows[i]["vcProName1"].ToString();
                    string vcProject1 = partsInfo.Rows[i]["vcCalendar1"].ToString();
                    string vcSupplier_id = partsInfo.Rows[i]["vcSupplier_id"].ToString();

                    //string total = (Convert.ToInt32(partsInfo.Rows[i]["num"])/(Convert.ToInt32(partsInfo.Rows[i]["srs"]))).ToString();
                    string total = partsInfo.Rows[i]["num"].ToString();
                    //20180929在从SOQReply数据生成包装计划时就已经把5个工程的计划都生成一遍了，所以要在这里，生成看板打印数据时，把周度品番筛走 - 李兴旺
                    //20180929查看该品番的品番频度 - 李兴旺
                    string vcPartFrequence = "";
                    string sqlPartFrequence = "SELECT vcPartsNo, vcPartFrequence FROM tPartInfoMaster where vcPartsNo='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCarFamilyCode='" + vcCarType + "' and dTimeFrom<='" + lbltime + "-01' and dTimeTo>='" + lbltime + "-01' ";
                    cmd.CommandText = sqlPartFrequence;
                    DataTable dtPartFrequence = new DataTable();
                    apt.Fill(dtPartFrequence);
                    if (dtPartFrequence.Rows.Count <= 0)
                    {
                        msg = "品番：" + vcPartsno + " 在当前对象月范围内没有品番基础信息！";
                        break;
                    }
                    vcPartFrequence = dtPartFrequence.Rows[0]["vcPartFrequence"].ToString().Trim();
                    //20180929不是看板打印计划表，则不区分品番频度；是看板打印计划表，则区分品番频度，不是周度的则更新。
                    //即TableName == "MonthKanBanPlanTblTMP" && vcPartFrequence == "周度"时，不进行更新操作 - 李兴旺
                    if (!(TableName == "MonthKanBanPlanTblTMP" && vcPartFrequence == "2"))
                    {
                        #region 插入、更新数据操作
                        foreach (DataRow dr in dt[i].Rows)
                        {
                            string tmpY = dr["vcYear"].ToString();
                            string tmpM = dr["vcMonth"].ToString().Length == 1 ? "0" + dr["vcMonth"].ToString() : dr["vcMonth"].ToString();
                            string vcMonth = tmpY + "-" + tmpM;
                            cmd.CommandText = "select top(1) * from " + TableName + " where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' ";
                            DataTable tmp = new DataTable();
                            apt.Fill(tmp);
                            if (tmp.Rows.Count > 0)
                            {
                                string upsql = "vc" + dr["days"].ToString() + "='" + dr["total"].ToString() + "'";
                                if (lbltime != vcMonth)
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + ", DUPDTIME=getdate(),CUPDUSER='" + user + "',montouch='" + lbltime + "',vcSupplier_id='" + vcSupplier_id + "' where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' ";
                                }
                                else
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + ", DUPDTIME=getdate(),CUPDUSER='" + user + "',vcSupplier_id='" + vcSupplier_id + "' where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' ";
                                }
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd.CommandText = " INSERT INTO " + TableName + " ([vcMonth],[vcPartsno],[vcDock],[vcCarType],[vcProject1],[vcProjectName],[DADDTIME],[CUPDUSER],vcSupplier_id)";
                                cmd.CommandText += " values( '" + vcMonth + "' ,'" + vcPartsno + "','" + vcDock + "','" + vcCarType + "','" + vcProject1 + "','" + vcProjectName + "',getdate(),'" + user + "','" + vcSupplier_id + "')  ";
                                cmd.ExecuteNonQuery();
                                string upsql = "vc" + dr["days"].ToString() + "='" + dr["total"].ToString() + "'";
                                if (lbltime != vcMonth)
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + ", DUPDTIME=getdate(),CUPDUSER='" + user + "',montouch='" + lbltime + "',vcSupplier_id='" + vcSupplier_id + "' where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' ";
                                }
                                else
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + ", DUPDTIME=getdate(),CUPDUSER='" + user + "',vcSupplier_id='" + vcSupplier_id + "' where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' ";
                                }
                                cmd.ExecuteNonQuery();
                            }
                        }
                        cmd.CommandText = "update " + TableName + " set vcMonTotal='" + total + "'  where vcMonth='" + lbltime + "' and vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' ";
                        cmd.ExecuteNonQuery();
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
        }

        #region 由 sourceTable 计划 更新 toTable 计划
        public void updateOtherPlan(string sourceTable, string toTable, SqlCommand cmd, SqlDataAdapter apt, DataTable dt, string user)
        {
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                string partsno = dt.Rows[j]["vcPartsno"].ToString().Replace("-", "").Trim();
                string month = dt.Rows[j]["vcMonth"].ToString();
                string vcDock = dt.Rows[j]["vcDock"].ToString();
                string vcCarType = dt.Rows[j]["vcCarType"].ToString();
                //20180928查看该品番的品番频度 - 李兴旺
                string vcPartFrequence = "";
                string sqlPartFrequence = "SELECT vcPartsNo, vcPartFrequence FROM tPartInfoMaster where vcPartsNo = '" + partsno + "' and vcDock = '" + vcDock + "' and vcCarFamilyCode = '" + vcCarType + "' and dTimeFrom<='" + month + "-01' and dTimeTo>='" + month + "-01' ";
                cmd.CommandText = sqlPartFrequence;
                DataTable dtPartFrequence = new DataTable();
                apt.Fill(dtPartFrequence);
                vcPartFrequence = dtPartFrequence.Rows[0]["vcPartFrequence"].ToString().Trim();
                //20180928不是看板打印计划表，则不区分品番频度；是看板打印计划表，则区分品番频度，不是周度的则更新。
                //即toTable == "MonthKanBanPlanTblTMP" && vcPartFrequence == "周度"时，不进行更新操作 - 李兴旺
                if (!(toTable == "MonthKanBanPlanTblTMP" && vcPartFrequence == "2"))
                {
                    #region 更新操作
                    string tmp = "";
                    for (int i = 1; i < 32; i++)
                    {
                        if (i == 31)
                            tmp += "vcD" + i + "b,	vcD" + i + "y";
                        else tmp += "vcD" + i + "b,	vcD" + i + "y,";
                    }
                    cmd.CommandText = " ";
                    cmd.CommandText += " select vcPartsno, vcDock, vcCartype, sigTotal, allTotal from " + sourceTable + " unpivot( sigTotal for allTotal in( ";
                    cmd.CommandText += tmp;
                    cmd.CommandText += "  )) P where LEN(sigTotal)>0 and vcPartsno = '" + partsno + "' and montouch = '" + month + "' and vcDock = '" + vcDock + "' and vcCartype = '" + vcCarType + "' ";
                    cmd.CommandText += " union all ";
                    cmd.CommandText += " select vcPartsno, vcDock, vcCartype, sigTotal, allTotal from " + sourceTable + " unpivot( sigTotal for allTotal in( ";
                    cmd.CommandText += tmp;
                    cmd.CommandText += "  )) P where LEN(sigTotal)>0 and vcPartsno = '" + partsno + "' and vcMonth = '" + month + "' and vcDock = '" + vcDock + "' and vcCartype = '" + vcCarType + "' ";
                    DataTable dtsource = new DataTable();
                    apt.Fill(dtsource);
                    cmd.CommandText = " select * from " + toTable + " where vcPartsno='" + partsno + "' and montouch ='" + month + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCarType + "'";
                    DataTable dtTo1 = new DataTable();
                    apt.Fill(dtTo1);
                    int num = 0;
                    if (dtTo1.Rows.Count == 1)
                    {
                        cmd.CommandText = "select vcPartsno, vcDock, vcCartype, sigTotal, allTotal from " + toTable + " unpivot (sigTotal for allTotal in( ";
                        cmd.CommandText += tmp;
                        cmd.CommandText += "  )) P where LEN(sigTotal)>0 and vcPartsno = '" + partsno + "' and montouch = '" + month + "' and vcDock = '" + vcDock + "' and vcCartype = '" + vcCarType + "' ";
                        DataTable tmpTo1 = new DataTable();
                        apt.Fill(tmpTo1);
                        string tmpsql = "";
                        for (int k = 0; k < tmpTo1.Rows.Count; k++, num++)
                        {
                            if (k == tmpTo1.Rows.Count - 1)
                            {
                                tmpsql += " " + tmpTo1.Rows[k]["allTotal"].ToString().Trim() + "='" + dtsource.Rows[num]["sigTotal"].ToString().Trim() + "'";
                            }
                            else tmpsql += " " + tmpTo1.Rows[k]["allTotal"].ToString().Trim() + "='" + dtsource.Rows[num]["sigTotal"].ToString().Trim() + "' , ";
                        }
                        cmd.CommandText = "update " + toTable + " set " + tmpsql + " , DUPDTIME=getdate() , CUPDUSER='" + user + "' where vcPartsno ='" + partsno + "' and vcDock='" + vcDock + "' and montouch='" + month + "' and vcCartype='" + vcCarType + "'";
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = " select * from " + toTable + " where vcPartsno='" + partsno + "' and vcMonth ='" + month + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCarType + "'";
                    DataTable dtTo2 = new DataTable();
                    apt.Fill(dtTo2);
                    if (dtTo2.Rows.Count == 1)
                    {
                        cmd.CommandText = "select vcPartsno, vcDock, vcCartype, sigTotal, allTotal from " + toTable + " unpivot( sigTotal for allTotal in( ";
                        cmd.CommandText += tmp;
                        cmd.CommandText += "  )) P where LEN(sigTotal)>0 and vcPartsno='" + partsno + "' and vcMonth ='" + month + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCarType + "' ";
                        DataTable tmpTo2 = new DataTable();
                        apt.Fill(tmpTo2);
                        string tmpsql = "";
                        for (int k = 0; k < tmpTo2.Rows.Count; k++, num++)
                        {
                            if (k == tmpTo2.Rows.Count - 1)
                            {
                                tmpsql += " " + tmpTo2.Rows[k]["allTotal"].ToString().Trim() + "='" + dtsource.Rows[num]["sigTotal"].ToString().Trim() + "'";
                            }
                            else tmpsql += " " + tmpTo2.Rows[k]["allTotal"].ToString().Trim() + "='" + dtsource.Rows[num]["sigTotal"].ToString().Trim() + "' , ";
                        }
                        cmd.CommandText = "update " + toTable + " set " + tmpsql + " , DUPDTIME=getdate() , CUPDUSER='" + user + "'  where vcPartsno ='" + partsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCarType + "' and vcMonth='" + month + "'";
                        cmd.ExecuteNonQuery();
                    }
                    #endregion
                }
            }
        }
        #endregion

        /// <summary>
        /// 生成看板打印数据
        /// </summary>
        public string CreatOrderNo(SqlCommand cmd, string mon, SqlDataAdapter apt, string user, string plant)
        {
            string msg = "";
            string tmp = "";
            for (int i = 1; i < 32; i++)
            {
                if (i == 31)
                    tmp += "vcD" + i + "b,	vcD" + i + "y";
                else tmp += "vcD" + i + "b,	vcD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            sb.AppendLine(" select distinct vcMonth,allTotal, daysig from ( ");
            sb.AppendLine("   select distinct vcMonth,allTotal, daysig,vcPartsno ,vcDock from (");
            sb.AppendLine("   select vcMonth, vcPartsno,vcDock,vcCartype,sigTotal , allTotal from MonthPackPlanTbl");
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and montouch ='{0}'", mon);
            sb.AppendLine("    union all ");
            sb.AppendLine("    select vcMonth,vcPartsno,vcDock,vcCartype,sigTotal, allTotal from MonthPackPlanTbl  ");
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and vcMonth='{0}'", mon);
            sb.AppendLine("    ) t1");
            sb.AppendLine("    left join (");
            sb.AppendFormat("    select daysig, dayN from sPlanConst unpivot ( daysig for dayN in( {0}", tmp);
            sb.AppendLine("     )) P ) t2 ");
            sb.AppendLine("     on t1.allTotal = t2.dayN");
            sb.AppendLine(" ) tall ");
            sb.AppendLine(" left join tPartInfoMaster tinfo on tall.vcPartsno = tinfo.vcPartsNo and tall.vcDock = tinfo.vcDock and tinfo.dTimeFrom<= '" + mon + "-01" + "' and tinfo.dTimeTo >= '" + mon + "-01" + "'");
            sb.AppendFormat(" where tinfo.vcPartPlant ='{0}' ", plant);
            sb.AppendLine("    order by vcMonth , allTotal");
            cmd.CommandText = sb.ToString();
            DataTable DayType = new DataTable();
            apt.Fill(DayType);

            cmd.CommandText = GetPlanSearch(mon, "MonthKanBanPlanTbl", plant);//看板计划
            DataTable pro0 = new DataTable();
            apt.Fill(pro0);
            cmd.CommandText = GetPlanSearch(mon, "MonthProdPlanTbl", plant);//生产计划
            DataTable pro1 = new DataTable();
            apt.Fill(pro1);
            cmd.CommandText = GetPlanSearch(mon, "MonthTZPlanTbl", plant);//涂装计划
            DataTable pro2 = new DataTable();
            apt.Fill(pro2);
            cmd.CommandText = GetPlanSearch(mon, "MonthP3PlanTbl", plant);//P3计划
            DataTable pro3 = new DataTable();
            apt.Fill(pro3);
            cmd.CommandText = GetPlanSearch(mon, "MonthPackPlanTbl", plant);//包装计划
            DataTable pro4 = new DataTable();
            apt.Fill(pro4);
            //------------------优化start
            cmd.CommandText = " select top(1) * from tKanbanPrintTbl ";
            DataTable BulkInsert = new DataTable();
            apt.Fill(BulkInsert);
            BulkInsert = BulkInsert.Clone();
            BulkInsert.Columns.Add("bushu");
            BulkInsert.Columns.Add("dayin");
            BulkInsert.Columns.Add("shengchan");
            string partsql = " select vcPartsno,vcDock,vcCarFamilyCode,t1.iQuantityPerContainer,t1.vcPorType,t1.vcZB,t2.vcProName0,t2.vcProName1,t2.vcProName2,t2.vcProName3,t2.vcProName4,t2.vcCalendar0,t2.vcCalendar1,t2.vcCalendar2,t2.vcCalendar3,t2.vcCalendar4 from tPartInfoMaster t1";
            partsql += " left join ProRuleMst t2 on t1.vcPorType=t2.vcPorType and t1.vcZB = t2.vcZB ";
            partsql += "  where exists (select vcPartsno from MonthPackPlanTbl where (vcMonth='" + mon + "' or montouch ='" + mon + "') and vcPartsno = t1.vcPartsno) and t1.dTimeFrom<= '" + mon + "-01" + "' and t1.dTimeTo >= '" + mon + "-01" + "'  ";
            cmd.CommandText = partsql;
            DataTable dtcalendarname = new DataTable();
            apt.Fill(dtcalendarname);
            DataTable dt_calendarname = new DataTable();
            //------------------优化end
            for (int i = 0; i < DayType.Rows.Count; i++)
            {
                DataRow[] dr = pro4.Select(" vcMonth ='" + DayType.Rows[i]["vcMonth"].ToString() + "' and allTotal ='" + DayType.Rows[i]["allTotal"].ToString() + "' ");
                int OrderStart = 0000;
                string tmp_part = "";
                string tmp_dock = "";
                for (int j = 0; j < dr.Length; j++)
                {
                    string vcPartsno = dr[j]["vcPartsno"].ToString();
                    string vcDock = dr[j]["vcDock"].ToString();
                    string vcCartype = dr[j]["vcCartype"].ToString();
                    string flag = dr[j]["flag"].ToString();
                    //20180929查看该品番的品番频度 - 李兴旺                    
                    string vcPartFrequence = "";
                    string sqlPartFrequence = "SELECT vcPartsNo, vcPartFrequence FROM tPartInfoMaster where vcPartsNo = '" + vcPartsno + "' and vcDock = '" + vcDock + "' and vcCarFamilyCode = '" + vcCartype + "' and dTimeFrom<='" + mon + "-01' and dTimeTo>='" + mon + "-01'  ";
                    cmd.CommandText = sqlPartFrequence;
                    DataTable dtPartFrequence = new DataTable();
                    apt.Fill(dtPartFrequence);
                    vcPartFrequence = dtPartFrequence.Rows[0]["vcPartFrequence"].ToString().Trim();
                    //20180929看板打印计划表没有周度品番数据，而其他工程的计划表有周度品番，且生成的看板打印数据需要读取其他工程的计划数据，
                    //如果不筛选则会报索引超出了数组界限的错误，因此生成看板打印数据时也要筛选一次周度品番 - 李兴旺
                    if (vcPartFrequence != "2")
                    {
                        #region 生成看板打印数据
                        dt_calendarname = dtcalendarname.Select("vcPartsno='" + vcPartsno + "'  and vcDock ='" + vcDock + "' and vcCarFamilyCode ='" + vcCartype + "'   ").CopyToDataTable();
                        string srs = dt_calendarname.Rows[0]["iQuantityPerContainer"].ToString().Trim();
                        int k = Convert.ToInt32(dr[j]["sigTotal"]) / Convert.ToInt32(srs);
                        if (tmp_part != vcPartsno || tmp_dock != vcDock)
                        {
                            tmp_part = vcPartsno;
                            tmp_dock = vcDock;
                            OrderStart = 0000;
                        }
                        if (k == 0)
                            continue;
                        //pro0
                        DataRow[] dr0 = pro0.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                        string day00 = dr0[0]["daysig"].ToString().Split('-')[0];
                        string vcBanZhi00 = dr0[0]["daysig"].ToString().Split('-')[1];
                        string vcComDate00 = dr0[0]["vcMonth"].ToString() + "-" + day00;
                        string zhi00 = vcBanZhi00 == "白" ? "0" : "1";
                        //2018-2-26 Malcolm.L 刘刚 获取工程0的A/B班值
                        string vcAB00 = ICalendar2.getABClass(vcComDate00, zhi00, plant, dt_calendarname.Rows[0]["vcCalendar0"].ToString().Trim());
                        string by0 = vcBanZhi00 == "白" ? "01" : "02";
                        string vcProject00 = dt_calendarname.Rows[0]["vcProName0"].ToString();
                        //pro1
                        DataRow[] dr1 = pro1.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                        string day01 = dr1[0]["daysig"].ToString().Split('-')[0];
                        string vcBanZhi01 = dr1[0]["daysig"].ToString().Split('-')[1];
                        string vcComDate01 = dr1[0]["vcMonth"].ToString() + "-" + day01;
                        string zhi01 = vcBanZhi01 == "白" ? "0" : "1";
                        //2018-2-26 Malcolm.L 刘刚 获取工程1的A/B班值
                        string vcAB01 = ICalendar2.getABClass(vcComDate01, zhi01, plant, dt_calendarname.Rows[0]["vcCalendar1"].ToString().Trim());
                        string by1 = vcBanZhi01 == "白" ? "01" : "02";
                        string vcProject01 = dt_calendarname.Rows[0]["vcProName1"].ToString();
                        //pro2
                        DataRow[] dr2 = pro2.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                        string day02 = "";
                        string vcBanZhi02 = "";
                        string vcComDate02 = "";
                        string zhi02 = "";
                        string vcAB02 = "";//2018-2-26 Malcolm.L 刘刚 获取工程2的A/B班值
                        string by2 = "";
                        string vcProject02 = "";
                        if (dr2.Length > 0)
                        {
                            day02 = dr2[0]["daysig"].ToString().Split('-')[0];
                            vcBanZhi02 = dr2[0]["daysig"].ToString().Split('-')[1];
                            vcComDate02 = dr2[0]["vcMonth"].ToString() + "-" + day02;
                            zhi02 = vcBanZhi02 == "白" ? "0" : "1";
                            //2018-2-26 Malcolm.L 刘刚 获取工程2的A/B班值
                            vcAB02 = ICalendar2.getABClass(vcComDate02, zhi02, plant, dt_calendarname.Rows[0]["vcCalendar2"].ToString().Trim());
                            by2 = vcBanZhi02 == "白" ? "01" : "02";
                            vcProject02 = dt_calendarname.Rows[0]["vcProName2"].ToString();
                        }
                        //pro3
                        DataRow[] dr3 = pro3.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                        string day03 = "";
                        string vcBanZhi03 = "";
                        string vcComDate03 = "";
                        string zhi03 = "";
                        string vcAB03 = "";//2018-2-26 Malcolm.L 刘刚 获取工程3的A/B班值
                        string by3 = "";
                        string vcProject03 = "";
                        if (dr3.Length > 0)
                        {
                            day03 = dr3[0]["daysig"].ToString().Split('-')[0];
                            vcBanZhi03 = dr3[0]["daysig"].ToString().Split('-')[1];
                            vcComDate03 = dr3[0]["vcMonth"].ToString() + "-" + day03;
                            zhi03 = vcBanZhi03 == "白" ? "0" : "1";
                            //2018-2-26 Malcolm.L 刘刚 获取工程3的A/B班值
                            vcAB03 = ICalendar2.getABClass(vcComDate03, zhi03, plant, dt_calendarname.Rows[0]["vcCalendar3"].ToString().Trim());
                            by3 = vcBanZhi03 == "白" ? "01" : "02";
                            vcProject03 = dt_calendarname.Rows[0]["vcProName3"].ToString();
                        }
                        //pro4
                        string day04 = dr[j]["daysig"].ToString().Split('-')[0];
                        string vcBanZhi04 = dr[j]["daysig"].ToString().Split('-')[1];//白/夜  白是0 夜是1
                        string vcComDate04 = dr[j]["vcMonth"].ToString() + "-" + day04;
                        string zhi04 = vcBanZhi04 == "白" ? "0" : "1";
                        //2018-2-26 Malcolm.L 刘刚 获取工程0的A/B班值
                        string vcAB04 = ICalendar2.getABClass(vcComDate04, zhi04, plant, dt_calendarname.Rows[0]["vcCalendar4"].ToString().Trim());
                        string by04 = vcBanZhi04 == "白" ? "01" : "02";
                        string vcProject04 = dt_calendarname.Rows[0]["vcProName4"].ToString();
                        for (int n = 0; n < k; n++)
                        {
                            OrderStart++;
                            string orderSerial = "";
                            string orderNo = vcComDate04.Replace("-", "") + by04;//订单号
                            //优化start
                            DataRow drInsert = BulkInsert.NewRow();
                            drInsert["iNo"] = DBNull.Value;
                            drInsert["vcTips"] = DBNull.Value;
                            drInsert["vcPrintflag"] = DBNull.Value;
                            drInsert["vcPrintTime"] = DBNull.Value;
                            drInsert["vcKBType"] = DBNull.Value;
                            drInsert["dUpdateTime"] = DBNull.Value;
                            drInsert["vcUpdater"] = DBNull.Value;
                            drInsert["vcPrintSpec"] = DBNull.Value;
                            drInsert["vcPrintflagED"] = DBNull.Value;
                            drInsert["vcPrintTimeED"] = DBNull.Value;

                            drInsert["vcQuantityPerContainer"] = srs;
                            drInsert["vcPartsNo"] = dr[j]["vcPartsno"].ToString();
                            drInsert["vcDock"] = dr[j]["vcDock"].ToString();
                            drInsert["vcCarType"] = dr[j]["vcCartype"].ToString();
                            drInsert["vcEDflag"] = 'S';
                            drInsert["vcKBorderno"] = orderNo;
                            drInsert["vcKBSerial"] = orderSerial;
                            drInsert["vcProject00"] = vcProject00;
                            drInsert["vcProject01"] = vcProject01;
                            drInsert["vcProject02"] = vcProject02;
                            drInsert["vcProject03"] = vcProject03;
                            drInsert["vcProject04"] = vcProject04;
                            drInsert["vcComDate00"] = vcComDate00;
                            drInsert["vcComDate01"] = vcComDate01;
                            drInsert["vcComDate02"] = vcComDate02;
                            drInsert["vcComDate03"] = vcComDate03;
                            drInsert["vcComDate04"] = vcComDate04;
                            drInsert["vcBanZhi00"] = zhi00;
                            drInsert["vcBanZhi01"] = zhi01;
                            drInsert["vcBanZhi02"] = zhi02;
                            drInsert["vcBanZhi03"] = zhi03;
                            drInsert["vcBanZhi04"] = zhi04;
                            drInsert["vcAB00"] = vcAB00;//
                            drInsert["vcAB01"] = vcAB01;//
                            drInsert["vcAB02"] = vcAB02;//
                            drInsert["vcAB03"] = vcAB03;//
                            drInsert["vcAB04"] = vcAB04;//
                            drInsert["vcCreater"] = user;
                            drInsert["dCreatTime"] = DateTime.Now;
                            drInsert["vcPlanMonth"] = mon;

                            drInsert["bushu"] = dt_calendarname.Rows[0]["vcPorType"].ToString();//部署
                            drInsert["dayin"] = vcComDate00 + zhi00;//打印
                            drInsert["shengchan"] = vcComDate01 + zhi01;//生产

                            BulkInsert.Rows.Add(drInsert);
                            //优化end
                        }
                        #endregion
                    }
                }
            }
            //分组
            var query = from t in BulkInsert.AsEnumerable()
                        group t by new
                        {
                            t1 = t.Field<string>("vcKBorderno"),
                            t2 = t.Field<string>("bushu"),
                            t3 = t.Field<string>("dayin"),
                            t4 = t.Field<string>("shengchan")
                        }
                            into m
                        select m;
            //分组排连番
            for (int m = 0; m < query.Count(); m++)
            {
                DataTable dt = query.ElementAt(m).CopyToDataTable();
                //按工位排序
                DataView dv = dt.DefaultView;
                dv.Sort = "vcProject01";
                dt = dv.ToTable();
                string serial = "0000";
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    serial = (Convert.ToInt32(serial) + 1).ToString("0000");
                    dt.Rows[n]["vcKBSerial"] = serial;
                    //检查该连番下是否存在数据 存在时：
                    string ssql = "select * from tKanbanPrintTbl t1 left join tPartInfoMaster t2 on t1.vcPartsNo = t2.vcPartsNo and t1.vcDock=t2.vcDock ";
                    ssql += "  where t1.vcEDflag='S' and t1.vcKBorderno='" + dt.Rows[n]["vcKBorderno"].ToString() + "' ";
                    ssql += " and t2.vcPorType='" + dt.Rows[n]["bushu"].ToString() + "' and vcComDate00='" + dt.Rows[n]["vcComDate00"].ToString() + "' and vcBanZhi00='" + dt.Rows[n]["vcBanZhi00"].ToString() + "' ";
                    ssql += " and vcComDate01='" + dt.Rows[n]["vcComDate01"].ToString() + "' and vcBanZhi01='" + dt.Rows[n]["vcBanZhi01"].ToString() + "' ";
                    ssql += " and t2.vcPartFrequence='0' ";//明早测试去掉这行的效果
                    //20180930上面SQL文最后一行增加对周度月度品番的判断，因为SQL文是按照部署、工程0和工程1的日期和值别检索数据，
                    //会包括周度品番，而看板打印数据不包含周度品番，且其余月度品番的看板序列号也会发生变化，则造成覆盖范围不符的情况
                    //因此需要清空旧有的（若导入对象月为七月，则包括六月底到八月初的）看板打印数据 - 李兴旺
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = ssql;
                    apt = new SqlDataAdapter(cmd);
                    DataTable dttmp = new DataTable();
                    apt.Fill(dttmp);
                    if (dttmp.Rows.Count > 0)//如果存在
                    {
                        DataRow[] existKB = dttmp.Select("vcPartsNo = '" + dt.Rows[n]["vcPartsNo"].ToString().Trim() + "' and vcDock ='" + dt.Rows[n]["vcDock"].ToString().Trim() + "' and vcKBSerial ='" + serial + "'");
                        if (existKB.Count() > 0)//1、与该信息相符 
                        {
                            dt.Rows[n].Delete();
                            n--;
                            continue;
                        }
                        else//2、与该信息不符
                        {
                            msg = "生产计划调整与覆盖范围不符，请重新调整后再导入。";
                            return msg;
                        }
                    }
                    else//插入
                    {

                    }
                }
                if (dt.Rows.Count == 0) continue;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertKanBanPrint";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 12, "vcPartsNo");
                cmd.Parameters.Add("@vcDock", SqlDbType.VarChar, 2, "vcDock");
                cmd.Parameters.Add("@vcCarType", SqlDbType.VarChar, 4, "vcCarType");
                cmd.Parameters.Add("@vcEDflag", SqlDbType.VarChar, 2, "vcEDflag");
                cmd.Parameters.Add("@vcKBorderno", SqlDbType.VarChar, 12, "vcKBorderno");
                cmd.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                cmd.Parameters.Add("@vcProject00", SqlDbType.VarChar, 20, "vcProject00");
                cmd.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcProject01");
                cmd.Parameters.Add("@vcProject02", SqlDbType.VarChar, 20, "vcProject02");
                cmd.Parameters.Add("@vcProject03", SqlDbType.VarChar, 20, "vcProject03");
                cmd.Parameters.Add("@vcProject04", SqlDbType.VarChar, 20, "vcProject04");
                cmd.Parameters.Add("@vcComDate00", SqlDbType.VarChar, 10, "vcComDate00");
                cmd.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcComDate01");
                cmd.Parameters.Add("@vcComDate02", SqlDbType.VarChar, 10, "vcComDate02");
                cmd.Parameters.Add("@vcComDate03", SqlDbType.VarChar, 10, "vcComDate03");
                cmd.Parameters.Add("@vcComDate04", SqlDbType.VarChar, 10, "vcComDate04");
                cmd.Parameters.Add("@vcBanZhi00", SqlDbType.VarChar, 1, "vcBanZhi00");
                cmd.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 1, "vcBanZhi01");
                cmd.Parameters.Add("@vcBanZhi02", SqlDbType.VarChar, 1, "vcBanZhi02");
                cmd.Parameters.Add("@vcBanZhi03", SqlDbType.VarChar, 1, "vcBanZhi03");
                cmd.Parameters.Add("@vcBanZhi04", SqlDbType.VarChar, 1, "vcBanZhi04");
                cmd.Parameters.Add("@vcAB00", SqlDbType.VarChar, 1, "vcAB00");//
                cmd.Parameters.Add("@vcAB01", SqlDbType.VarChar, 1, "vcAB01");//
                cmd.Parameters.Add("@vcAB02", SqlDbType.VarChar, 1, "vcAB02");//
                cmd.Parameters.Add("@vcAB03", SqlDbType.VarChar, 1, "vcAB03");//
                cmd.Parameters.Add("@vcAB04", SqlDbType.VarChar, 1, "vcAB04");//
                cmd.Parameters.Add("@vcCreater", SqlDbType.VarChar, 10, "vcCreater");
                cmd.Parameters.Add("@vcPlanMonth", SqlDbType.VarChar, 7, "vcPlanMonth");
                cmd.Parameters.Add("@vcQuantityPerContainer", SqlDbType.VarChar, 7, "vcQuantityPerContainer");
                apt = new SqlDataAdapter();
                apt.InsertCommand = cmd;
                apt.Update(dt);
            }
            return msg;
        }
        public void updatePlan(SqlCommand cmd, string mon, string plant)
        {
            cmd.CommandText = "insert into MonthKanBanPlanTbl select * from MonthKanBanPlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "') and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = t.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "insert into MonthP3PlanTbl select * from MonthP3PlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = t.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "insert into MonthPackPlanTbl select * from MonthPackPlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = t.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "insert into MonthProdPlanTbl select * from MonthProPlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = t.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "insert into MonthTZPlanTbl select * from MonthTZPlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = t.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.ExecuteNonQuery();
        }
        public void deletePlan(SqlCommand cmd, string mon, string plant)
        {
            cmd.CommandText = "delete from MonthKanBanPlanTbl  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthKanBanPlanTbl.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthP3PlanTbl  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthP3PlanTbl.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthPackPlanTbl  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthPackPlanTbl.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthProdPlanTbl  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthProdPlanTbl.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthTZPlanTbl where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthTZPlanTbl.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.ExecuteNonQuery();
        }
        public void deleteTMP(SqlCommand cmd, string mon, string plant)
        {
            cmd.CommandText = "delete from MonthKanBanPlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthKanBanPlanTblTMP.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthP3PlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthP3PlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthPackPlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthPackPlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthProPlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthProPlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthTZPlanTblTMP where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = MonthTZPlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.ExecuteNonQuery();
        }
        public void deleteTMPplan(SqlCommand cmd, string mon, string plant)
        {
            string ssql = "";
            ssql += "  delete from MonthP3PlanTbl where vcPartsno in  (";
            ssql += "  select t1.vcPartsno from MonthP3PlanTbl t1";
            ssql += "  left join tPartInfoMaster t2 ";
            ssql += "  on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock and t2.dTimeFrom<= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "' ";
            ssql += "  left join ProRuleMst t3 on t2.vcPorType = t3.vcPorType and t2.vcZB = t3.vcZB ";
            ssql += "  where t3.vcCalendar3 is null or LEN(t3.vcCalendar3)=0";
            ssql += "  ) and vcDock in ";
            ssql += "  (";
            ssql += "  select t1.vcDock from MonthP3PlanTbl t1";
            ssql += "  left join tPartInfoMaster t2 ";
            ssql += "  on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock and t2.dTimeFrom<= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "' ";
            ssql += "  left join ProRuleMst t3 on t2.vcPorType = t3.vcPorType and t2.vcZB = t3.vcZB ";
            ssql += "  where t3.vcCalendar3 is null or LEN(t3.vcCalendar3)=0";
            ssql += "  )";

            cmd.CommandText = ssql;
            cmd.ExecuteNonQuery();

            ssql = "";
            ssql += "  delete from MonthTZPlanTbl where vcPartsno in  (";
            ssql += "  select t1.vcPartsno from MonthTZPlanTbl t1";
            ssql += "  left join tPartInfoMaster t2 ";
            ssql += "  on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock and t2.dTimeFrom<= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "' ";
            ssql += "  left join ProRuleMst t3 on t2.vcPorType = t3.vcPorType and t2.vcZB = t3.vcZB ";
            ssql += "  where t3.vcCalendar2 is null or LEN(t3.vcCalendar2)=0";
            ssql += "  ) and   vcDock in ";
            ssql += "  (";
            ssql += "  select t1.vcDock from MonthTZPlanTbl t1";
            ssql += "  left join tPartInfoMaster t2 ";
            ssql += "  on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock and t2.dTimeFrom<= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "' ";
            ssql += "  left join ProRuleMst t3 on t2.vcPorType = t3.vcPorType and t2.vcZB = t3.vcZB ";
            ssql += "  where t3.vcCalendar2 is null or LEN(t3.vcCalendar2)=0";
            ssql += "  )";
            cmd.CommandText = ssql;
            cmd.ExecuteNonQuery();
        }
        private void UpdatePlanMST(SqlCommand cmd, string mon, string plant)
        {
            string tmpmon = mon + "-01";
            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            sb.AppendLine(" insert into tPlanPartInfo ");
            sb.AppendFormat(" select '{0}' as vcMonth, t1.*,'S' as vcEDFlag,t2.vcPartPlant , ", mon);
            sb.AppendLine(" t2.vcPartsNameCHN,t2.vcCurrentPastCode,t2.vcPorType , t2.vcZB,t2.iQuantityPerContainer,t2.vcQFflag from (");
            sb.AppendLine(" select distinct vcPartsno ,vcCarType,vcDock from MonthPackPlanTbl ");
            sb.AppendFormat(" where montouch ='{0}' or (vcMonth ='{1}' and montouch is null)", mon, mon);
            sb.AppendLine(" ) t1");
            sb.AppendLine(" left join tPartInfoMaster t2");
            sb.AppendLine(" on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock ");
            sb.AppendFormat(" where t2.vcPartPlant ='{0}' and t2.dTimeFrom <='{1}' and t2.dTimeTo>='{2}'", plant, tmpmon, tmpmon);
            cmd.CommandText = sb.ToString();
            cmd.ExecuteNonQuery();
        }
        public void updateSoqReply(SqlCommand cmd, string mon, SqlDataAdapter apt, string user, string plant)
        {
            try
            {
                string vcCLYM = Convert.ToDateTime(mon + "-01").AddMonths(-1).ToString("yyyyMM");
                string vcDXYM = Convert.ToDateTime(mon + "-01").ToString("yyyyMM");

                // 设置对象月表
                string sqlTblMonth = "";
                string sqlPartType = "";
                string sqlPartDate = "";
                string strDate = "";
                string[] montmp = mon.Split('-');
                #region 获取月份日历
                if (montmp[1] == "01")
                {
                    sqlTblMonth = "tJanuary";
                }
                else if (montmp[1] == "02")
                {
                    sqlTblMonth = "tFebruary";
                }
                else if (montmp[1] == "03")
                {
                    sqlTblMonth = "tMarch";
                }
                else if (montmp[1] == "04")
                {
                    sqlTblMonth = "tApril";
                }
                else if (montmp[1] == "05")
                {
                    sqlTblMonth = "tMay";
                }
                else if (montmp[1] == "06")
                {
                    sqlTblMonth = "tJune";
                }
                else if (montmp[1] == "07")
                {
                    sqlTblMonth = "tJuly";
                }
                else if (montmp[1] == "08")
                {
                    sqlTblMonth = "tAugust";
                }
                else if (montmp[1] == "09")
                {
                    sqlTblMonth = "tSeptember";
                }
                else if (montmp[1] == "10")
                {
                    sqlTblMonth = "tOctober";
                }
                else if (montmp[1] == "11")
                {
                    sqlTblMonth = "tNovember";
                }
                else if (montmp[1] == "12")
                {
                    sqlTblMonth = "tDecember";
                }
                #endregion
                cmd.CommandText = " select * from MonthPackPlanTbl where vcMonth='" + mon + "' ";
                DataTable dtPack = new DataTable();
                apt.Fill(dtPack);
                DataTable dt_Update = new DataTable();
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from TSoqReply t1 ");
                sb.Append("where exists(");
                sb.Append("             select vcPartsNo, vcCarFamilyCode,dTimeFrom,dTimeTo from tPartInfoMaster t2 ");
                sb.Append("             where dTimeFrom<='" + mon + "-01" + "' and dTimeTo>='" + mon + "-01" + "' and vcInOutFlag='0'  ");
                sb.Append("             and t1.vcPart_id=t2.vcPartsNo and t1.vcCarType=t2.vcCarFamilyCode and t2.dTimeFrom<='" + mon + "- 01' and t2.dTimeTo>='" + mon + "- 01') ");
                sb.Append("and iPartNums<>'0' and vcFZGC='" + plant + "' and vcInOutFlag='0' and vcDXYM='" + vcDXYM + "' and vcCLYM='" + vcCLYM + "'");
                sb.Append("order by vcPart_id");
                cmd.CommandText = sb.ToString();
                apt.Fill(dt_Update);

                DataColumn[] PrimaryKeyColumns = new DataColumn[5];
                PrimaryKeyColumns[0] = dt_Update.Columns["vcFZGC"];
                PrimaryKeyColumns[1] = dt_Update.Columns["vcCLYM"];
                PrimaryKeyColumns[2] = dt_Update.Columns["vcDXYM"];
                PrimaryKeyColumns[3] = dt_Update.Columns["vcCarType"];
                PrimaryKeyColumns[4] = dt_Update.Columns["vcPart_id"];
                dt_Update.PrimaryKey = PrimaryKeyColumns;

                for (int i = 0; i < dt_Update.Rows.Count; i++)
                {
                    ////调试
                    //if(i == 56){
                    //    i = i;
                    //}
                    int tmp = 0;
                    string vcPart_id = dt_Update.Rows[i]["vcPart_id"].ToString();
                    string vcCarType = dt_Update.Rows[i]["vcCarType"].ToString();

                    sqlPartType = "";
                    sqlPartType += "SELECT vcCalendar4 FROM ProRuleMst t1, tPartInfoMaster t2 ";
                    sqlPartType += "where t2.vcPorType=t1.vcPorType and t2.vcZB=t1.vcZB ";
                    sqlPartType += "and t2.vcPartsNo='" + vcPart_id + "' and t2.vcCarFamilyCode='" + vcCarType + "' ";
                    sqlPartType += "and t2.dTimeFrom<='" + mon + "-01" + "' and t2.dTimeTo>='" + mon + "-01" + "' ";
                    DataTable dt_Calendar4 = excute.ExcuteSqlWithSelectToDT(sqlPartType);
                    //cmd.CommandText = sqlPartType;
                    //apt.Fill(dt_Calendar4);
                    sqlPartDate = "";
                    if (dt_Calendar4.Rows.Count != 0 && dt_Calendar4.Rows[0]["vcCalendar4"].ToString() != "")
                    {
                        string Caledar4 = dt_Calendar4.Rows[0]["vcCalendar4"].ToString();
                        string[] Caleders = Caledar4.Split('-');

                        sqlPartDate += "SELECT * FROM " + sqlTblMonth + " where ";
                        sqlPartDate += "vcPlant='" + plant + "' and vcYear='" + montmp[0] + "' and vcMonth='" + montmp[1] + "' ";
                        sqlPartDate += "and vcGC='" + Caleders[0] + "' and vcZB='" + Caleders[1] + "' ";
                        DataTable dt_CalendarData = excute.ExcuteSqlWithSelectToDT(sqlPartDate);
                        //cmd.CommandText = sqlPartDate;
                        //apt.Fill(dt_CalendarData);
                        if (dt_CalendarData.Rows.Count != 0)
                        {
                            if (dt_CalendarData.Rows[0]["D1b"].ToString() != "" || dt_CalendarData.Rows[0]["D1y"].ToString() != "")
                            {
                                strDate = "iD1";
                            }
                            else if (dt_CalendarData.Rows[0]["D2b"].ToString() != "" || dt_CalendarData.Rows[0]["D2y"].ToString() != "")
                            {
                                strDate = "iD2";
                            }
                            else if (dt_CalendarData.Rows[0]["D3b"].ToString() != "" || dt_CalendarData.Rows[0]["D3y"].ToString() != "")
                            {
                                strDate = "iD3";
                            }
                            else if (dt_CalendarData.Rows[0]["D4b"].ToString() != "" || dt_CalendarData.Rows[0]["D4y"].ToString() != "")
                            {
                                strDate = "iD4";
                            }
                            else if (dt_CalendarData.Rows[0]["D5b"].ToString() != "" || dt_CalendarData.Rows[0]["D5y"].ToString() != "")
                            {
                                strDate = "iD5";
                            }
                            else if (dt_CalendarData.Rows[0]["D6b"].ToString() != "" || dt_CalendarData.Rows[0]["D6y"].ToString() != "")
                            {
                                strDate = "iD6";
                            }
                            else if (dt_CalendarData.Rows[0]["D7b"].ToString() != "" || dt_CalendarData.Rows[0]["D7y"].ToString() != "")
                            {
                                strDate = "iD7";
                            }
                            else if (dt_CalendarData.Rows[0]["D8b"].ToString() != "" || dt_CalendarData.Rows[0]["D8y"].ToString() != "")
                            {
                                strDate = "iD8";
                            }
                            else if (dt_CalendarData.Rows[0]["D9b"].ToString() != "" || dt_CalendarData.Rows[0]["D9y"].ToString() != "")
                            {
                                strDate = "iD9";
                            }
                            else if (dt_CalendarData.Rows[0]["D10b"].ToString() != "" || dt_CalendarData.Rows[0]["D10y"].ToString() != "")
                            {
                                strDate = "iD10";
                            }
                            else if (dt_CalendarData.Rows[0]["D11b"].ToString() != "" || dt_CalendarData.Rows[0]["D11y"].ToString() != "")
                            {
                                strDate = "iD11";
                            }
                            else if (dt_CalendarData.Rows[0]["D12b"].ToString() != "" || dt_CalendarData.Rows[0]["D12y"].ToString() != "")
                            {
                                strDate = "iD12";
                            }
                            else if (dt_CalendarData.Rows[0]["D13b"].ToString() != "" || dt_CalendarData.Rows[0]["D13y"].ToString() != "")
                            {
                                strDate = "iD13";
                            }
                            else if (dt_CalendarData.Rows[0]["D14b"].ToString() != "" || dt_CalendarData.Rows[0]["D14y"].ToString() != "")
                            {
                                strDate = "iD14";
                            }
                            else if (dt_CalendarData.Rows[0]["D15b"].ToString() != "" || dt_CalendarData.Rows[0]["D15y"].ToString() != "")
                            {
                                strDate = "iD15";
                            }
                            else if (dt_CalendarData.Rows[0]["D16b"].ToString() != "" || dt_CalendarData.Rows[0]["D16y"].ToString() != "")
                            {
                                strDate = "iD16";
                            }
                            else if (dt_CalendarData.Rows[0]["D17b"].ToString() != "" || dt_CalendarData.Rows[0]["D17y"].ToString() != "")
                            {
                                strDate = "iD17";

                            }
                            else if (dt_CalendarData.Rows[0]["D18b"].ToString() != "" || dt_CalendarData.Rows[0]["D18y"].ToString() != "")
                            {
                                strDate = "iD18";
                            }
                            else if (dt_CalendarData.Rows[0]["D19b"].ToString() != "" || dt_CalendarData.Rows[0]["D19y"].ToString() != "")
                            {
                                strDate = "iD19";
                            }
                            else if (dt_CalendarData.Rows[0]["D20b"].ToString() != "" || dt_CalendarData.Rows[0]["D20y"].ToString() != "")
                            {
                                strDate = "iD20";
                            }
                            else if (dt_CalendarData.Rows[0]["D21b"].ToString() != "" || dt_CalendarData.Rows[0]["D21y"].ToString() != "")
                            {
                                strDate = "iD21";
                            }
                            else if (dt_CalendarData.Rows[0]["D22b"].ToString() != "" || dt_CalendarData.Rows[0]["D22y"].ToString() != "")
                            {
                                strDate = "iD22";
                            }
                            else if (dt_CalendarData.Rows[0]["D23b"].ToString() != "" || dt_CalendarData.Rows[0]["D23y"].ToString() != "")
                            {
                                strDate = "iD23";
                            }
                            else if (dt_CalendarData.Rows[0]["D24b"].ToString() != "" || dt_CalendarData.Rows[0]["D24y"].ToString() != "")
                            {
                                strDate = "iD24";
                            }
                            else if (dt_CalendarData.Rows[0]["D25b"].ToString() != "" || dt_CalendarData.Rows[0]["D25y"].ToString() != "")
                            {
                                strDate = "iD25";
                            }
                            else if (dt_CalendarData.Rows[0]["D26b"].ToString() != "" || dt_CalendarData.Rows[0]["D26y"].ToString() != "")
                            {
                                strDate = "D26";
                            }
                            else if (dt_CalendarData.Rows[0]["D27b"].ToString() != "" || dt_CalendarData.Rows[0]["D27y"].ToString() != "")
                            {
                                strDate = "iD27";
                            }
                            else if (dt_CalendarData.Rows[0]["D28b"].ToString() != "" || dt_CalendarData.Rows[0]["D28y"].ToString() != "")
                            {
                                strDate = "iD28";
                            }
                            else if (dt_CalendarData.Rows[0]["D29b"].ToString() != "" || dt_CalendarData.Rows[0]["D29y"].ToString() != "")
                            {
                                strDate = "iD29";
                            }
                            else if (dt_CalendarData.Rows[0]["D30b"].ToString() != "" || dt_CalendarData.Rows[0]["D30y"].ToString() != "")
                            {
                                strDate = "iD30";
                            }
                            else if (dt_CalendarData.Rows[0]["D31b"].ToString() != "" || dt_CalendarData.Rows[0]["D31y"].ToString() != "")
                            {
                                strDate = "iD31";
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("品番" + vcPart_id + "工程4日历类型取得失败");
                    }
                    DataRow[] dr = dtPack.Select("vcMonth='" + mon + "' and vcPartsno='" + vcPart_id + "' and vcCarType='" + vcCarType + "' ");
                    dt_Update.Rows[i]["iD1"] = Convert.ToInt32(dr[0]["vcD1b"].ToString().Length == 0 ? "0" : dr[0]["vcD1b"]) + Convert.ToInt32(dr[0]["vcD1y"].ToString().Length == 0 ? "0" : dr[0]["vcD1y"]);
                    dt_Update.Rows[i]["iD2"] = Convert.ToInt32(dr[0]["vcD2b"].ToString().Length == 0 ? "0" : dr[0]["vcD2b"]) + Convert.ToInt32(dr[0]["vcD2y"].ToString().Length == 0 ? "0" : dr[0]["vcD2y"]);
                    dt_Update.Rows[i]["iD3"] = Convert.ToInt32(dr[0]["vcD3b"].ToString().Length == 0 ? "0" : dr[0]["vcD3b"]) + Convert.ToInt32(dr[0]["vcD3y"].ToString().Length == 0 ? "0" : dr[0]["vcD3y"]);
                    dt_Update.Rows[i]["iD4"] = Convert.ToInt32(dr[0]["vcD4b"].ToString().Length == 0 ? "0" : dr[0]["vcD4b"]) + Convert.ToInt32(dr[0]["vcD4y"].ToString().Length == 0 ? "0" : dr[0]["vcD4y"]);
                    dt_Update.Rows[i]["iD5"] = Convert.ToInt32(dr[0]["vcD5b"].ToString().Length == 0 ? "0" : dr[0]["vcD5b"]) + Convert.ToInt32(dr[0]["vcD5y"].ToString().Length == 0 ? "0" : dr[0]["vcD5y"]);
                    dt_Update.Rows[i]["iD6"] = Convert.ToInt32(dr[0]["vcD6b"].ToString().Length == 0 ? "0" : dr[0]["vcD6b"]) + Convert.ToInt32(dr[0]["vcD6y"].ToString().Length == 0 ? "0" : dr[0]["vcD6y"]);
                    dt_Update.Rows[i]["iD7"] = Convert.ToInt32(dr[0]["vcD7b"].ToString().Length == 0 ? "0" : dr[0]["vcD7b"]) + Convert.ToInt32(dr[0]["vcD7y"].ToString().Length == 0 ? "0" : dr[0]["vcD7y"]);
                    dt_Update.Rows[i]["iD8"] = Convert.ToInt32(dr[0]["vcD8b"].ToString().Length == 0 ? "0" : dr[0]["vcD8b"]) + Convert.ToInt32(dr[0]["vcD8y"].ToString().Length == 0 ? "0" : dr[0]["vcD8y"]);
                    dt_Update.Rows[i]["iD9"] = Convert.ToInt32(dr[0]["vcD9b"].ToString().Length == 0 ? "0" : dr[0]["vcD9b"]) + Convert.ToInt32(dr[0]["vcD9y"].ToString().Length == 0 ? "0" : dr[0]["vcD9y"]);
                    dt_Update.Rows[i]["iD10"] = Convert.ToInt32(dr[0]["vcD10b"].ToString().Length == 0 ? "0" : dr[0]["vcD10b"]) + Convert.ToInt32(dr[0]["vcD10y"].ToString().Length == 0 ? "0" : dr[0]["vcD10y"]);
                    dt_Update.Rows[i]["iD11"] = Convert.ToInt32(dr[0]["vcD11b"].ToString().Length == 0 ? "0" : dr[0]["vcD11b"]) + Convert.ToInt32(dr[0]["vcD11y"].ToString().Length == 0 ? "0" : dr[0]["vcD11y"]);
                    dt_Update.Rows[i]["iD12"] = Convert.ToInt32(dr[0]["vcD12b"].ToString().Length == 0 ? "0" : dr[0]["vcD12b"]) + Convert.ToInt32(dr[0]["vcD12y"].ToString().Length == 0 ? "0" : dr[0]["vcD12y"]);
                    dt_Update.Rows[i]["iD13"] = Convert.ToInt32(dr[0]["vcD13b"].ToString().Length == 0 ? "0" : dr[0]["vcD13b"]) + Convert.ToInt32(dr[0]["vcD13y"].ToString().Length == 0 ? "0" : dr[0]["vcD13y"]);
                    dt_Update.Rows[i]["iD14"] = Convert.ToInt32(dr[0]["vcD14b"].ToString().Length == 0 ? "0" : dr[0]["vcD14b"]) + Convert.ToInt32(dr[0]["vcD14y"].ToString().Length == 0 ? "0" : dr[0]["vcD14y"]);
                    dt_Update.Rows[i]["iD15"] = Convert.ToInt32(dr[0]["vcD15b"].ToString().Length == 0 ? "0" : dr[0]["vcD15b"]) + Convert.ToInt32(dr[0]["vcD15y"].ToString().Length == 0 ? "0" : dr[0]["vcD15y"]);
                    dt_Update.Rows[i]["iD16"] = Convert.ToInt32(dr[0]["vcD16b"].ToString().Length == 0 ? "0" : dr[0]["vcD16b"]) + Convert.ToInt32(dr[0]["vcD16y"].ToString().Length == 0 ? "0" : dr[0]["vcD16y"]);
                    dt_Update.Rows[i]["iD17"] = Convert.ToInt32(dr[0]["vcD17b"].ToString().Length == 0 ? "0" : dr[0]["vcD17b"]) + Convert.ToInt32(dr[0]["vcD17y"].ToString().Length == 0 ? "0" : dr[0]["vcD17y"]);
                    dt_Update.Rows[i]["iD18"] = Convert.ToInt32(dr[0]["vcD18b"].ToString().Length == 0 ? "0" : dr[0]["vcD18b"]) + Convert.ToInt32(dr[0]["vcD18y"].ToString().Length == 0 ? "0" : dr[0]["vcD18y"]);
                    dt_Update.Rows[i]["iD19"] = Convert.ToInt32(dr[0]["vcD19b"].ToString().Length == 0 ? "0" : dr[0]["vcD19b"]) + Convert.ToInt32(dr[0]["vcD19y"].ToString().Length == 0 ? "0" : dr[0]["vcD19y"]);
                    dt_Update.Rows[i]["iD20"] = Convert.ToInt32(dr[0]["vcD20b"].ToString().Length == 0 ? "0" : dr[0]["vcD20b"]) + Convert.ToInt32(dr[0]["vcD20y"].ToString().Length == 0 ? "0" : dr[0]["vcD20y"]);
                    dt_Update.Rows[i]["iD21"] = Convert.ToInt32(dr[0]["vcD21b"].ToString().Length == 0 ? "0" : dr[0]["vcD21b"]) + Convert.ToInt32(dr[0]["vcD21y"].ToString().Length == 0 ? "0" : dr[0]["vcD21y"]);
                    dt_Update.Rows[i]["iD22"] = Convert.ToInt32(dr[0]["vcD22b"].ToString().Length == 0 ? "0" : dr[0]["vcD22b"]) + Convert.ToInt32(dr[0]["vcD22y"].ToString().Length == 0 ? "0" : dr[0]["vcD22y"]);
                    dt_Update.Rows[i]["iD23"] = Convert.ToInt32(dr[0]["vcD23b"].ToString().Length == 0 ? "0" : dr[0]["vcD23b"]) + Convert.ToInt32(dr[0]["vcD23y"].ToString().Length == 0 ? "0" : dr[0]["vcD23y"]);
                    dt_Update.Rows[i]["iD24"] = Convert.ToInt32(dr[0]["vcD24b"].ToString().Length == 0 ? "0" : dr[0]["vcD24b"]) + Convert.ToInt32(dr[0]["vcD24y"].ToString().Length == 0 ? "0" : dr[0]["vcD24y"]);
                    dt_Update.Rows[i]["iD25"] = Convert.ToInt32(dr[0]["vcD25b"].ToString().Length == 0 ? "0" : dr[0]["vcD25b"]) + Convert.ToInt32(dr[0]["vcD25y"].ToString().Length == 0 ? "0" : dr[0]["vcD25y"]);
                    dt_Update.Rows[i]["iD26"] = Convert.ToInt32(dr[0]["vcD26b"].ToString().Length == 0 ? "0" : dr[0]["vcD26b"]) + Convert.ToInt32(dr[0]["vcD26y"].ToString().Length == 0 ? "0" : dr[0]["vcD26y"]);
                    dt_Update.Rows[i]["iD27"] = Convert.ToInt32(dr[0]["vcD27b"].ToString().Length == 0 ? "0" : dr[0]["vcD27b"]) + Convert.ToInt32(dr[0]["vcD27y"].ToString().Length == 0 ? "0" : dr[0]["vcD27y"]);
                    dt_Update.Rows[i]["iD28"] = Convert.ToInt32(dr[0]["vcD28b"].ToString().Length == 0 ? "0" : dr[0]["vcD28b"]) + Convert.ToInt32(dr[0]["vcD28y"].ToString().Length == 0 ? "0" : dr[0]["vcD28y"]);
                    dt_Update.Rows[i]["iD29"] = Convert.ToInt32(dr[0]["vcD29b"].ToString().Length == 0 ? "0" : dr[0]["vcD29b"]) + Convert.ToInt32(dr[0]["vcD29y"].ToString().Length == 0 ? "0" : dr[0]["vcD29y"]);
                    dt_Update.Rows[i]["iD30"] = Convert.ToInt32(dr[0]["vcD30b"].ToString().Length == 0 ? "0" : dr[0]["vcD30b"]) + Convert.ToInt32(dr[0]["vcD30y"].ToString().Length == 0 ? "0" : dr[0]["vcD30y"]);
                    dt_Update.Rows[i]["iD31"] = Convert.ToInt32(dr[0]["vcD31b"].ToString().Length == 0 ? "0" : dr[0]["vcD31b"]) + Convert.ToInt32(dr[0]["vcD31y"].ToString().Length == 0 ? "0" : dr[0]["vcD31y"]);
                    //dt_Update.Rows[i]["updateFlag"] = "1";
                    tmp = Convert.ToInt32(dt_Update.Rows[i]["iD1"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD2"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD3"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD4"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD5"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD6"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD7"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD8"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD9"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD10"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD11"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD12"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD13"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD14"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD15"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD16"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD17"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD18"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD19"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD20"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD21"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD22"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD23"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD24"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD25"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD26"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD27"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD28"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD29"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD30"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["iD31"]);
                    dt_Update.Rows[i][strDate] = Convert.ToInt32(dt_Update.Rows[i]["iPartNums"]) - tmp + Convert.ToInt32(dt_Update.Rows[i][strDate]);
                }
                SqlCommandBuilder scb = new SqlCommandBuilder(apt);


                apt.Update(dt_Update);
                //cmd.CommandText = "update TSoqReply set updateFlag='1' where vcMonth='" + mon + "' and vcPlant='" + plant + "'";
                //cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getMonProPlan_S(string mon) //月度生产计划
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.Connection.Open();
            cmd.CommandTimeout = 0;
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;
            for (double i = 1; i < daynum + 1; i++)
            {
                if (i == daynum)
                    tmpE += "tE.ED" + i;
                else tmpE += "tE.ED" + i + ",";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (double i = 1; i < daynum2 + 1; i++)
            {
                if (i == daynum2)
                    tmpT += "tT.TD" + i;
                else tmpT += "tT.TD" + i + ",";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" select tE.vcMonth ,tT.vcPartsno , tT.vcDock,tT.vcCarType,tT.vcProject1,tT.vcProjectName,tE.vcMonTotal,");
            sb.AppendLine(" tT.vcCurrentPastCode,tT.vcPartsNameCHN,tT.vcCalendar1,tT.vcCalendar2,tT.vcCalendar3,tT.vcCalendar4, ");
            sb.AppendFormat("{0},", tmpT);
            sb.AppendFormat("{0}", tmpE);
            #region 作废
            //sb.AppendLine(" tT.TD1,tT.TD2,tT.TD3,tT.TD4,tT.TD5,tT.TD6,tT.TD7,tT.TD8,tT.TD9,tT.TD10,tT.TD11,tT.TD12,tT.TD13,tT.TD14, ");
            //sb.AppendLine(" tT.TD15,tT.TD16,tT.TD17,tT.TD18,tT.TD19,tT.TD20,tT.TD21,tT.TD22,tT.TD23,tT.TD24,tT.TD25,tT.TD26,tT.TD27, ");
            //sb.AppendLine(" tT.TD28,tT.TD29,tT.TD30, ");
            //sb.AppendLine(" tE.ED1,	tE.ED2,	tE.ED3,	tE.ED4,	tE.ED5,	tE.ED6,	tE.ED7,	tE.ED8,	tE.ED9,	tE.ED10, ");
            //sb.AppendLine(" tE.ED11,tE.ED12,tE.ED13,tE.ED14,tE.ED15,tE.ED16,tE.ED17,tE.ED18,tE.ED19,tE.ED20, ");
            //sb.AppendLine(" tE.ED21,tE.ED22,tE.ED23,tE.ED24,tE.ED25,tE.ED26,tE.ED27,tE.ED28,tE.ED29,tE.ED30,tE.ED31 "); 
            #endregion
            sb.AppendLine("  from  ");
            sb.AppendLine(" ( ");
            sb.AppendLine(" select t1.*, t2.vcCurrentPastCode,t2.vcPartsNameCHN,--t2.vcPorType ,t2.vcZB, ");
            sb.AppendLine("  t3.vcCalendar1,t3.vcCalendar2,t3.vcCalendar3,t3.vcCalendar4  ");
            sb.AppendLine("  from  ");
            sb.AppendLine(" (SELECT vcMonth ,vcPartsno ,vcDock ,vcCarType ,vcProject1 ,vcProjectName ,vcMonTotal, ");
            sb.AppendLine(" vcD1b 	+	vcD1y	as	TD1,vcD2b	+	vcD2y	as	TD2, ");
            sb.AppendLine(" vcD3b	+	vcD3y	as	TD3,vcD4b	+	vcD4y	as	TD4, ");
            sb.AppendLine(" vcD5b	+	vcD5y	as	TD5,vcD6b	+	vcD6y	as	TD6, ");
            sb.AppendLine(" vcD7b	+	vcD7y	as	TD7,vcD8b	+	vcD8y	as	TD8, ");
            sb.AppendLine(" vcD9b	+	vcD9y	as	TD9,vcD10b	+	vcD10y	as	TD10, ");
            sb.AppendLine(" vcD11b	+	vcD11y	as	TD11,vcD12b	+	vcD12y	as	TD12, ");
            sb.AppendLine(" vcD13b	+	vcD13y	as	TD13,vcD14b	+	vcD14y	as	TD14, ");
            sb.AppendLine(" vcD15b	+	vcD15y	as	TD15,vcD16b	+	vcD16y	as	TD16, ");
            sb.AppendLine(" vcD17b	+	vcD17y	as	TD17,vcD18b	+	vcD18y	as	TD18, ");
            sb.AppendLine(" vcD19b	+	vcD19y	as	TD19,vcD20b	+	vcD20y	as	TD20, ");
            sb.AppendLine(" vcD21b	+	vcD21y	as	TD21,vcD22b	+	vcD22y	as	TD22, ");
            sb.AppendLine(" vcD23b	+	vcD23y	as	TD23,vcD24b	+	vcD24y	as	TD24, ");
            sb.AppendLine(" vcD25b	+	vcD25y	as	TD25,vcD26b	+	vcD26y	as	TD26, ");
            sb.AppendLine(" vcD27b	+	vcD27y	as	TD27,vcD28b	+	vcD28y	as	TD28, ");
            sb.AppendLine(" vcD29b	+	vcD29y	as	TD29,vcD30b	+	vcD30y	as	TD30, ");
            sb.AppendLine(" vcD31b	+	vcD31y	as	TD31, ");
            sb.AppendLine(" montouch  ");
            sb.AppendLine(" FROM [MonthProdPlanTbl] ");
            sb.AppendLine(" ) t1 ");
            sb.AppendLine(" left join tPartInfoMaster t2  ");
            sb.AppendLine(" on t1.vcPartsno = t2.vcPartsNo and t1.vcDock =t2.vcDock and t1.vcCarType =t2.vcCarFamilyCode and t2.dTimeFrom <='" + mon + "-01' and t2.dTimeTo>= '" + mon + "-01' ");
            sb.AppendLine(" left join ProRuleMst t3 on t3.vcPorType=t2.vcPorType and t3.vcZB=t2.vcZB ");
            sb.AppendLine(" where t1.montouch ='2012-12'  ");
            sb.AppendLine(" ) tT ");
            sb.AppendLine(" left join ( ");
            sb.AppendLine("  ");
            sb.AppendLine(" select t1.* , t2.vcCurrentPastCode,t2.vcPartsNameCHN,--t2.vcPorType ,t2.vcZB, ");
            sb.AppendLine("  t3.vcCalendar1,t3.vcCalendar2,t3.vcCalendar3,t3.vcCalendar4  ");
            sb.AppendLine("  from  ");
            sb.AppendLine(" (SELECT vcMonth ,vcPartsno ,vcDock ,vcCarType ,vcProject1 ,vcProjectName ,vcMonTotal, ");
            sb.AppendLine(" vcD1b 	+	vcD1y	as	ED1,vcD2b	+	vcD2y	as	ED2, ");
            sb.AppendLine(" vcD3b	+	vcD3y	as	ED3,vcD4b	+	vcD4y	as	ED4, ");
            sb.AppendLine(" vcD5b	+	vcD5y	as	ED5,vcD6b	+	vcD6y	as	ED6, ");
            sb.AppendLine(" vcD7b	+	vcD7y	as	ED7,vcD8b	+	vcD8y	as	ED8, ");
            sb.AppendLine(" vcD9b	+	vcD9y	as	ED9,vcD10b	+	vcD10y	as	ED10, ");
            sb.AppendLine(" vcD11b	+	vcD11y	as	ED11,vcD12b	+	vcD12y	as	ED12, ");
            sb.AppendLine(" vcD13b	+	vcD13y	as	ED13,vcD14b	+	vcD14y	as	ED14, ");
            sb.AppendLine(" vcD15b	+	vcD15y	as	ED15,vcD16b	+	vcD16y	as	ED16, ");
            sb.AppendLine(" vcD17b	+	vcD17y	as	ED17,vcD18b	+	vcD18y	as	ED18, ");
            sb.AppendLine(" vcD19b	+	vcD19y	as	ED19,vcD20b	+	vcD20y	as	ED20, ");
            sb.AppendLine(" vcD21b	+	vcD21y	as	ED21,vcD22b	+	vcD22y	as	ED22, ");
            sb.AppendLine(" vcD23b	+	vcD23y	as	ED23,vcD24b	+	vcD24y	as	ED24, ");
            sb.AppendLine(" vcD25b	+	vcD25y	as	ED25,vcD26b	+	vcD26y	as	ED26, ");
            sb.AppendLine(" vcD27b	+	vcD27y	as	ED27,vcD28b	+	vcD28y	as	ED28, ");
            sb.AppendLine(" vcD29b	+	vcD29y	as	ED29,vcD30b	+	vcD30y	as	ED30, ");
            sb.AppendLine(" vcD31b	+	vcD31y	as	ED31, ");
            sb.AppendLine(" montouch  ");
            sb.AppendLine(" FROM MonthProdPlanTbl ");
            sb.AppendLine(" ) t1 ");
            sb.AppendLine(" left join tPartInfoMaster t2  ");
            sb.AppendLine(" on t1.vcPartsno = t2.vcPartsNo and t1.vcDock =t2.vcDock and t1.vcCarType =t2.vcCarFamilyCode and  t2.dTimeFrom <='" + mon + "-01' and t2.dTimeTo>= '" + mon + "-01' ");
            sb.AppendLine(" left join ProRuleMst t3 on t3.vcPorType=t2.vcPorType and t3.vcZB=t2.vcZB ");
            sb.AppendLine(" where t1.vcMonth ='2012-12' and t1.montouch is null ");
            sb.AppendLine(" ) tE ");
            sb.AppendLine(" on tE.vcPartsno = tT.vcPartsno and tE.vcDock = tT.vcDock and tE.vcCarType = tT.vcCarType ");
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getMonPackPlanTMP(string mon, string tablename, string plant)//获取临时表的值别计划
        {
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;

            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b   as  ED" + i + "b,	t2.	vcD" + i + "y 	as	ED" + i + "y";
                else tmpE += "t2.vcD" + i + "b 	as 	ED" + i + "b,	t2.vcD" + i + "y 	as	ED" + i + "y,";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b as	TD" + i + "b,	t1.vcD" + i + "y 	as	TD" + i + "y";
                else tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y 	as	TD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("   select t2.vcMonth,t5.vcData2 as vcPlant, t2.vcPartsno, t2.vcDock, t2.vcCarType, t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            sb.AppendLine("   t3.vcPartNameCN as vcPartsNameCHN, t4.vcProName1 as vcProject1,t3.vcProType+'-'+t3.vcZB as vcProjectName, t3.vcHJ as vcCurrentPastCode,t2.vcMonTotal as vcMonTotal ,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("  from (select * from {0} where montouch is not null) t1 ", tablename);
            sb.AppendFormat("  full join (select * from {0} where montouch is null) t2", tablename);
            sb.AppendLine("  on t1.montouch=t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendLine("  left join (select distinct vcMonth,vcPartNameCN,vcZB,vcHJ,vcDock,vcCarType,vcPartsNo,vcProType,vcPlant,vcEDFlag from tPlanPartInfo) t3");
            sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock=t2.vcDock and t3.vcCarType=t2.vcCarType and  t3.vcMonth= '" + mon + "' ");
            sb.AppendLine("  left join ProRuleMst t4");
            sb.AppendLine("  on t4.vcPorType=t3.vcProType and t4.vcZB=t3.vcZB");
            sb.AppendLine(" left join (select vcData1,vcData2 from ConstMst where vcDataId='kbplant') t5");
            sb.AppendLine(" on t3.vcPlant=t5.vcData1 ");
            sb.AppendFormat(" where t2.vcMonth='{0}' and t3.vcPlant='{1}' and t3.vcEDFlag='S' ", mon, plant);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getMonPackPlanTMPcur(string mon, string tablename, string plant)//获取临时表的值别计划
        {
            mon = mon.Substring(0, 4) + "-" + mon.Substring(4, 2);
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;

            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b as ED" + i + "b, t2.vcD" + i + "y as ED" + i + "y";
                else tmpE += "t2.vcD" + i + "b as ED" + i + "b, t2.vcD" + i + "y as ED" + i + "y,";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b as TD" + i + "b,	t1.vcD" + i + "y as TD" + i + "y";
                else tmpT += "t1.vcD" + i + "b as TD" + i + "b,	t1.vcD" + i + "y as TD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" select t2.vcMonth ,t5.vcData2 as vcPlant, t2.vcPartsno, t2.vcDock, t2.vcCarType, t4.vcCalendar1, t4.vcCalendar2, t4.vcCalendar3, t4.vcCalendar4,");
            sb.AppendLine(" t2.vcSupplier_id, t4.vcProName1 as vcProject1, t3.vcPorType+'-'+t3.vcZB as vcProjectName, t3.vcCurrentPastCode, t2.vcMonTotal as vcMonTotal,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0} ", tmpE);
            sb.AppendFormat(" from (select * from {0} where montouch is not null) t1", tablename);
            sb.AppendFormat(" full join (select * from {0} where montouch is null) t2", tablename);
            sb.AppendLine(" on t1.montouch=t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendLine(" left join (select * from tPartInfoMaster where dTimeFrom<='" + mon + "-01' and dTimeTo>='" + mon + "-01' ) t3");
            sb.AppendLine(" on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock=t2.vcDock and t3.vcCarFamilyCode=t2.vcCarType");
            sb.AppendLine(" left join ProRuleMst t4");
            sb.AppendLine(" on t4.vcPorType=t3.vcPorType and t4.vcZB=t3.vcZB");
            sb.AppendLine(" left join (select vcData1,vcData2 from ConstMst where vcDataId='kbplant') t5");
            sb.AppendLine(" on t3.vcPartPlant=t5.vcData1 ");
            sb.AppendFormat(" where t2.vcMonth='{0}' and t3.vcPartPlant='{1}'", mon, plant);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getMonPackPlanTMP_srs(string mon, string tablename, string plant)//获取临时表的值别计划
        {
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b *t3.iQuantityPerContainer as ED" + i + "b,	t2.vcD" + i + "y *t3.iQuantityPerContainer	as ED" + i + "y";
                else tmpE += "t2.vcD" + i + "b *t3.iQuantityPerContainer as ED" + i + "b, t2.vcD" + i + "y *t3.iQuantityPerContainer as ED" + i + "y,";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b as TD" + i + "b, t1.vcD" + i + "y as TD" + i + "y";
                else tmpT += "t1.vcD" + i + "b as TD" + i + "b, t1.vcD" + i + "y as	TD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" select t2.vcMonth, t2.vcPartsno,t2.vcDock,t2.vcCarType,t3.iQuantityPerContainer,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            sb.AppendLine("  t3.vcPartsNameCHN, t2.vcProject1,t3.vcPorType+'-'+t3.vcZB as vcProjectName, t3.vcCurrentPastCode,t2.vcMonTotal *t3.iQuantityPerContainer as vcMonTotal,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("  from (select * from {0} where montouch is not null) t1 ", tablename);
            sb.AppendFormat("  full join (select * from {0} where montouch is null) t2", tablename);
            sb.AppendLine("  on t1.montouch=t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendLine("  left join tPartInfoMaster t3");
            sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock=t2.vcDock and t3.vcCarFamilyCode=t2.vcCarType and t3.dTimeFrom<='" + mon + "-01' and t3.dTimeTo>='" + mon + "-01' ");
            sb.AppendLine("  left join ProRuleMst t4");
            sb.AppendLine("  on t4.vcPorType= t3.vcPorType and t4.vcZB=t3.vcZB");
            sb.AppendFormat(" where t2.vcMonth='{0}' and t3.vcPartPlant='{1}' ", mon, plant);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getMonProALL2(string mon, string TableSName, string TableEDName, string plant)
        {
            DataTable dtS = getMonPackPlanTMP(mon, TableSName, plant);
            addEDflag(ref dtS, "通常");
            DataTable dtE = getMonPlanED(mon, TableEDName, plant);
            addEDflag(ref dtE, "紧急");
            DataTable dtALL = MergeDataTable(dtS, dtE);
            return dtALL;
        }

        public void addEDflag(ref DataTable dt, string flag)
        {
            DataColumn col = new DataColumn();
            col.ColumnName = "vcEDflag";
            col.DataType = typeof(string);
            col.DefaultValue = flag;
            dt.Columns.Add(col);
        }
        public DataTable MergeDataTable(DataTable dt1, DataTable dt2)
        {
            DataTable DataTable1 = dt1;
            DataTable DataTable2 = dt2;
            DataTable newDataTable = DataTable1.Clone();
            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < DataTable1.Rows.Count; i++)
            {
                DataTable1.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }
            for (int i = 0; i < DataTable2.Rows.Count; i++)
            {
                DataTable2.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }
            return newDataTable;
        }
        public DataTable getMonProALL(string mon, string TableSName, string TableEDName, string plant)
        {
            DataTable dt = getMonPackPlanTMP(mon, TableSName, plant);
            if (dt.Rows.Count > 0)
            {
                DataTable dttmp = excute.ExcuteSqlWithSelectToDT(GetPlanSearch(mon, TableEDName, plant));
                for (int i = 0; i < dttmp.Rows.Count; i++)
                {
                    DataRow[] dr = dt.Select("vcMonth='" + mon + "' and vcPartsno='" + dttmp.Rows[i]["vcPartsno"].ToString().Replace("-", "").Insert(5, "-").Insert(11, "-") + "' and vcDock ='" + dttmp.Rows[i]["vcDock"].ToString() + "' and  vcCarType='" + dttmp.Rows[i]["vcCartype"].ToString() + "'");
                    string col = "";
                    if (dttmp.Rows[i]["vcMonth"].ToString() != mon)
                    {
                        col = "T" + dttmp.Rows[i]["allTotal"].ToString().Substring(2);
                    }
                    else
                    {
                        col = "E" + dttmp.Rows[i]["allTotal"].ToString().Substring(2);
                    }
                    if (dr.Length == 1)
                    {
                        dr[0][col] = (Convert.ToInt32(dr[0][col].ToString().Length == 0 ? "0" : dr[0][col].ToString()) + Convert.ToInt32(dttmp.Rows[i]["sigTotal"]));
                        dr[0]["vcMonTotal"] = Convert.ToInt32(dr[0]["vcMonTotal"]) + Convert.ToInt32(dttmp.Rows[i]["sigTotal"]);
                    }
                    else
                    {
                        DataRow dradd = dt.NewRow();
                        string ssql = " select * from  dbo.tPartInfoMaster t1 left join ProRuleMst t2 on t1.vcPorType = t2.vcPorType and t1.vcZB = t2.vcZB where t1.vcPartsNo='" + dttmp.Rows[i]["vcPartsno"].ToString().Replace("-", "") + "' and t1.vcDock ='" + dttmp.Rows[i]["vcDock"].ToString() + "' and t1.vcCarFamilyCode = '" + dttmp.Rows[i]["vcCartype"].ToString() + "'  and t1.dTimeFrom <='" + mon + "-01' and t1.dTimeTo>= '" + mon + "-01' ";
                        DataTable Infotmp = excute.ExcuteSqlWithSelectToDT(ssql);
                        dradd["vcMonth"] = mon;
                        dradd["vcPartsno"] = dttmp.Rows[i]["vcPartsno"].ToString().Replace("-", "").Insert(5, "-").Insert(11, "-");
                        dradd["vcDock"] = dttmp.Rows[i]["vcDock"];
                        dradd["vcCarType"] = dttmp.Rows[i]["vcCarType"];
                        dradd["vcCalendar1"] = Infotmp.Rows[0]["vcCalendar1"];
                        dradd["vcCalendar2"] = Infotmp.Rows[0]["vcCalendar2"];
                        dradd["vcCalendar3"] = Infotmp.Rows[0]["vcCalendar3"];
                        dradd["vcCalendar4"] = Infotmp.Rows[0]["vcCalendar4"];
                        dradd["vcPartsNameCHN"] = Infotmp.Rows[0]["vcPartsNameCHN"];
                        dradd["vcProject1"] = Infotmp.Rows[0]["vcCalendar1"];
                        dradd["vcProjectName"] = Infotmp.Rows[0]["vcPorType"].ToString() + '-' + Infotmp.Rows[0]["vcZB"].ToString();
                        dradd["vcCurrentPastCode"] = Infotmp.Rows[0]["vcCurrentPastCode"];
                        dradd["vcMonTotal"] = dttmp.Rows[i]["sigTotal"];
                        dradd[col] = dttmp.Rows[i]["sigTotal"];
                        dt.Rows.Add(dradd);
                    }
                }
            }
            else
            {
                dt = getMonPlanED(mon, TableEDName, plant);
            }
            return dt;
        }
        public DataTable getMonPlanED(string mon, string TblName, string plant)
        {
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b as	ED" + i + "b,	t2.	vcD" + i + "y	as	ED" + i + "y";
                else tmpE += "t2.vcD" + i + "b	as	ED" + i + "b,	t2.vcD" + i + "y	as	ED" + i + "y,";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y	as	TD" + i + "y";
                else tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y	as	TD" + i + "y,";
            }
            #region 作废
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("    select t2.vcMonth , t2.vcPartsno ,t2.vcDock,t2.vcCarType,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            //sb.AppendLine("   t3.vcPartsNameCHN,  t4.vcCalendar1 as vcProject1,t4.vcProName1 as vcProjectName, t3.vcCurrentPastCode,'' as vcMonTotal ,");
            //sb.AppendFormat(" {0},", tmpT);
            //sb.AppendFormat(" {0}", tmpE);
            //sb.AppendFormat("  from {0} t1 ", TblName);
            //sb.AppendFormat("  full join {0} t2", TblName);
            //sb.AppendLine("  on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            //sb.AppendLine("  left join dbo.tPartInfoMaster t3");
            //sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock = t2.vcDock and t3.vcCarFamilyCode = t2.vcCarType");
            //sb.AppendLine("  left join dbo.ProRuleMst t4");
            //sb.AppendLine("  on t4.vcPorType = t3.vcPorType and t4.vcZB = t3.vcZB");
            //sb.AppendFormat("  where t2.vcMonth ='{0}' and  t1.montouch ='{1}'", mon, mon);
            #endregion
            StringBuilder sb = new StringBuilder();
            #region 作废
            //sb.AppendFormat(" select '{0}' as vcMonth , case when t2.vcPartsno is null then SUBSTRING(t1.vcPartsno,0,6)+'-'+SUBSTRING(t1.vcPartsno,7,5)+'-'+SUBSTRING(t1.vcPartsno,11,2) else SUBSTRING(t2.vcPartsno,0,6)+'-'+SUBSTRING(t2.vcPartsno,7,5)+'-'+SUBSTRING(t2.vcPartsno,11,2) end as vcPartsno ", mon);
            //sb.AppendLine(" ,case when t2.vcDock is null then t1.vcDock else t2.vcDock end as vcDock");
            //sb.AppendLine(" ,case when t2.vcCarType is null then t1.vcCarType else t2.vcCarType end as vcCarType");
            //sb.AppendLine("  ,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            //sb.AppendLine(" ");
            //sb.AppendLine("  t3.vcPartsNameCHN,t3.vcPorType+'-'+t3.vcZB as vcProjectName, t3.vcCurrentPastCode ,'0' as vcMonTotal,");
            //sb.AppendFormat(" {0},", tmpT);
            //sb.AppendFormat(" {0}", tmpE);
            //sb.AppendFormat("   from (select * from  {0} where montouch is not null ) t1  ", TblName);
            //sb.AppendFormat("    full join (select * from  {0} where montouch is null ) t2", TblName);
            //sb.AppendLine("     on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            //sb.AppendLine("   left join dbo.tPartInfoMaster t3");
            //sb.AppendLine("   on (t3.vcPartsNo=t2.vcPartsNo or t3.vcPartsNo = t1.vcPartsno)and (t3.vcDock = t2.vcDock or t3.vcDock = t1.vcDock) and (t3.vcCarFamilyCode = t2.vcCarType or t3.vcCarFamilyCode = t1.vcCarType)");
            //sb.AppendLine("   left join dbo.ProRuleMst t4");
            //sb.AppendLine(" on t4.vcPorType = t3.vcPorType and t4.vcZB = t3.vcZB");
            //sb.AppendFormat(" where t1.montouch = '{0}' or t2.vcMonth ='{1}'", mon, mon); 
            #endregion
            sb.AppendLine("   select tt1.vcMonth,t5.vcData2 as vcPlant, SUBSTRING(tt1.vcPartsno,0,6)+'-'+SUBSTRING(tt1.vcPartsno,6,5)+'-'+SUBSTRING(tt1.vcPartsno,11,2) as vcPartsno,");
            sb.AppendLine("   tt1.vcDock,tt1.vcCarType ,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,t3.vcPartNameCN as vcPartsNameCHN,t4.vcProName1 as vcProject1,t3.vcProType+'-'+t3.vcZB as vcProjectName, t3.vcHJ as vcCurrentPastCode ,tt1.vcMonTotal ,");
            sb.AppendLine("  TD1b	,	TD1y	,");
            sb.AppendLine("  TD2b	,	TD2y	,");
            sb.AppendLine("  TD3b	,	TD3y	,");
            sb.AppendLine("  TD4b	,	TD4y	,");
            sb.AppendLine("  TD5b	,	TD5y	,");
            sb.AppendLine("  TD6b	,	TD6y	,");
            sb.AppendLine("  TD7b	,	TD7y	,");
            sb.AppendLine("  TD8b	,	TD8y	,");
            sb.AppendLine("  TD9b	,	TD9y	,");
            sb.AppendLine("  TD10b	,	TD10y	,");
            sb.AppendLine("  TD11b	,	TD11y	,");
            sb.AppendLine("  TD12b	,	TD12y	,");
            sb.AppendLine("  TD13b	,	TD13y	,");
            sb.AppendLine("  TD14b	,	TD14y	,");
            sb.AppendLine("  TD15b	,	TD15y	,");
            sb.AppendLine("  TD16b	,	TD16y	,");
            sb.AppendLine("  TD17b	,	TD17y	,");
            sb.AppendLine("  TD18b	,	TD18y	,");
            sb.AppendLine("  TD19b	,	TD19y	,");
            sb.AppendLine("  TD20b	,	TD20y	,");
            sb.AppendLine("  TD21b	,	TD21y	,");
            sb.AppendLine("  TD22b	,	TD22y	,");
            sb.AppendLine("  TD23b	,	TD23y	,");
            sb.AppendLine("  TD24b	,	TD24y	,");
            sb.AppendLine("  TD25b	,	TD25y	,");
            sb.AppendLine("  TD26b	,	TD26y	,");
            sb.AppendLine("  TD27b	,	TD27y	,");
            sb.AppendLine("  TD28b	,	TD28y	,");
            sb.AppendLine("  TD29b	,	TD29y	,");
            sb.AppendLine("  TD30b	,	TD30y	,");
            sb.AppendLine("  TD31b	,	TD31y	,");
            sb.AppendLine("  ED1b	,	ED1y	,");
            sb.AppendLine("  ED2b	,	ED2y	,");
            sb.AppendLine("  ED3b	,	ED3y	,");
            sb.AppendLine("  ED4b	,	ED4y	,");
            sb.AppendLine("  ED5b	,	ED5y	,");
            sb.AppendLine("  ED6b	,	ED6y	,");
            sb.AppendLine("  ED7b	,	ED7y	,");
            sb.AppendLine("  ED8b	,	ED8y	,");
            sb.AppendLine("  ED9b	,	ED9y	,");
            sb.AppendLine("  ED10b	,	ED10y	,");
            sb.AppendLine("  ED11b	,	ED11y	,");
            sb.AppendLine("  ED12b	,	ED12y	,");
            sb.AppendLine("  ED13b	,	ED13y	,");
            sb.AppendLine("  ED14b	,	ED14y	,");
            sb.AppendLine("  ED15b	,	ED15y	,");
            sb.AppendLine("  ED16b	,	ED16y	,");
            sb.AppendLine("  ED17b	,	ED17y	,");
            sb.AppendLine("  ED18b	,	ED18y	,");
            sb.AppendLine("  ED19b	,	ED19y	,");
            sb.AppendLine("  ED20b	,	ED20y	,");
            sb.AppendLine("  ED21b	,	ED21y	,");
            sb.AppendLine("  ED22b	,	ED22y	,");
            sb.AppendLine("  ED23b	,	ED23y	,");
            sb.AppendLine("  ED24b	,	ED24y	,");
            sb.AppendLine("  ED25b	,	ED25y	,");
            sb.AppendLine("  ED26b	,	ED26y	,");
            sb.AppendLine("  ED27b	,	ED27y	,");
            sb.AppendLine("  ED28b	,	ED28y	,");
            sb.AppendLine("  ED29b	,	ED29y	,");
            sb.AppendLine("  ED30b	,	ED30y	,");
            sb.AppendLine("  ED31b	,	ED31y	");
            sb.AppendLine("  ");
            sb.AppendLine("   from (");
            sb.AppendFormat("    select '{0}' as vcMonth ,", mon);
            sb.AppendLine("   case when t2.vcPartsno is null then t1.vcPartsno  else t2.vcPartsno end as vcPartsno  ,");
            sb.AppendLine("    case when t2.vcDock is null then t1.vcDock else t2.vcDock end as vcDock");
            sb.AppendLine("   ,case when t2.vcCarType is null then t1.vcCarType else t2.vcCarType end as vcCarType");
            sb.AppendLine("  ,'0' as vcMonTotal,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("   from (select * from  {0} where montouch is not null ) t1  ", TblName);
            sb.AppendFormat("    full join (select * from  {0} where montouch is null ) t2", TblName);
            sb.AppendLine("    on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendFormat("     where (t1.montouch = '{0}' or t2.vcMonth ='{1}')", mon, mon);
            sb.AppendLine("    ) tt1");
            sb.AppendLine("   left join (select distinct vcMonth,vcPartsNo,vcDock,vcCarType,vcZB,vcProType,vcEDFlag,vcPlant,vcHJ,vcPartNameCN from dbo.tPlanPartInfo) t3");
            sb.AppendLine("     on t3.vcPartsNo=tt1.vcPartsNo and t3.vcDock = tt1.vcDock and t3.vcCarType = tt1.vcCarType and  t3.vcMonth = '" + mon + "' ");
            sb.AppendLine("     left join dbo.ProRuleMst t4");
            sb.AppendLine("   on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            sb.AppendLine(" left join (select vcData1 ,vcData2  from dbo.ConstMst where vcDataId ='kbplant') t5");
            sb.AppendLine(" on t3.vcPlant = t5.vcData1 ");
            sb.AppendFormat("   where t3.vcPlant ='{0}' and vcEDFlag ='E' ", plant);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int a = 0;
                for (int j = 13; j < dt.Columns.Count; j++)
                {
                    a += Convert.ToInt32(dt.Rows[i][j].ToString().Length == 0 ? "0" : dt.Rows[i][j].ToString());
                }
                dt.Rows[i]["vcMonTotal"] = a.ToString();
            }
            return dt;
        }
        public string GetPlan(string mon, string TableName)
        {
            string tmp = "";
            for (int i = 1; i < 32; i++)
            {
                if (i == 31)
                    tmp += "vcD" + i + "b,	vcD" + i + "y";
                else tmp += "vcD" + i + "b,	vcD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" select Tall.*,ROW_NUMBER() over(partition by vcPartsno, vcDock,vcCartype order by vcMonth,daysig,vcPartsno,vcDock,vcCartype) as flag from (");
            sb.AppendLine("   select t1.*,t2.daysig from (");
            sb.AppendFormat("   select vcMonth, vcPartsno,vcDock,vcCartype,sigTotal , allTotal from {0}", TableName);
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and montouch ='{0}'", mon);
            sb.AppendLine("    union all ");
            sb.AppendFormat("    select vcMonth,vcPartsno,vcDock,vcCartype,sigTotal , allTotal from {0}  ", TableName);
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and vcMonth ='{0}'", mon);
            sb.AppendLine("    ) t1");
            sb.AppendLine("    left join (");
            sb.AppendFormat("    select daysig , dayN from sPlanConst unpivot ( daysig for dayN in( {0}", tmp);
            sb.AppendLine("     )) P ) t2 ");
            sb.AppendLine("     on t1.allTotal = t2.dayN");
            sb.AppendLine("    ) Tall order by vcMonth ,daysig,vcPartsno,vcDock,vcCartype");
            return sb.ToString();
        }
        public string GetPlanSearch(string mon, string TableName, string plant)
        {
            string tmp = "";
            for (int i = 1; i < 32; i++)
            {
                if (i == 31)
                    tmp += "vcD" + i + "b,	vcD" + i + "y";
                else tmp += "vcD" + i + "b,	vcD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("    select Tall.vcMonth,Tall.vcPartsno,Tall.vcDock,Tall.vcCarType,Tall.sigTotal*Tinfo.iQuantityPerContainer as sigTotal,Tall.allTotal ,Tall.daysig ,ROW_NUMBER() over(partition by Tall.vcPartsno, Tall.vcDock,Tall.vcCartype order by Tall.vcMonth,Tall.daysig,Tall.vcPartsno,Tall.vcDock,Tall.vcCartype) as flag from (");

            sb.AppendLine("    select Tall.vcMonth,Tall.vcPartsno,Tall.vcDock,Tall.vcCarType,Tall.sigTotal ,Tall.allTotal ,Tall.daysig ,ROW_NUMBER() over(partition by Tall.vcPartsno, Tall.vcDock,Tall.vcCartype order by Tall.vcMonth,Tall.daysig,Tall.vcPartsno,Tall.vcDock,Tall.vcCartype) as flag from (");
            sb.AppendLine("   select t1.*,t2.daysig from (");
            sb.AppendFormat("   select vcMonth, vcPartsno,vcDock,vcCartype,sigTotal , allTotal from {0}", TableName);
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and montouch ='{0}'", mon);
            sb.AppendLine("    union all ");
            sb.AppendFormat("    select vcMonth,vcPartsno,vcDock,vcCartype,sigTotal , allTotal from {0}  ", TableName);
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and vcMonth ='{0}'", mon);
            sb.AppendLine("    ) t1");
            sb.AppendLine("    left join (");
            sb.AppendFormat("    select daysig , dayN from sPlanConst unpivot ( daysig for dayN in( {0}", tmp);
            sb.AppendLine("     )) P ) t2 ");
            sb.AppendLine("     on t1.allTotal = t2.dayN");
            sb.AppendLine("    ) Tall ");
            sb.AppendLine("  left join dbo.tPartInfoMaster Tinfo on Tall.vcPartsno = Tinfo.vcPartsNo and Tall.vcDock = Tinfo.vcDock and Tall.vcCarType = Tinfo.vcCarFamilyCode  and   Tinfo.dTimeFrom<= '" + mon + "-01" + "' and Tinfo.dTimeTo >= '" + mon + "-01" + "' ");
            sb.AppendFormat("  where Tinfo.vcPartPlant ='{0}'", plant);
            sb.AppendLine(" order by vcMonth ,daysig,vcPartsno,vcDock,vcCartype");
            return sb.ToString();
        }

        public DataTable getPartsno(string mon)
        {
            string tmpmon = mon + "-01";
            string ssql = "select vcPartsNo,vcDock,vcInOutFlag,vcQFflag from tPartInfoMaster where dTimeFrom <='" + tmpmon + "' and dTimeTo>='" + tmpmon + "' ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }


        #region 公用方法
        #region 月表名称转换
        public string swithMon(int Mon)
        {
            string re = "";
            switch (Mon)
            {
                case 1:
                    re = "dbo.tJanuary";
                    break;
                case 2:
                    re = "dbo.tFebruary";
                    break;
                case 3:
                    re = "dbo.tMarch";
                    break;
                case 4:
                    re = "dbo.tApril";
                    break;
                case 5:
                    re = "dbo.tMay";
                    break;
                case 6:
                    re = "dbo.tJune";
                    break;
                case 7:
                    re = "dbo.tJuly";
                    break;
                case 8:
                    re = "dbo.tAugust";
                    break;
                case 9:
                    re = "dbo.tSeptember";
                    break;
                case 10:
                    re = "dbo.tOctober";
                    break;
                case 11:
                    re = "dbo.tNovember";
                    break;
                case 12:
                    re = "dbo.tDecember";
                    break;
            }
            return re;
        }
        #endregion
        public string getDiysql(DateTime tim)
        {
            DataTable dt = new DataTable();
            string sqltmp = "";
            double daynum = (tim.AddMonths(1) - tim).TotalDays;
            for (double i = 1; i < daynum + 1; i++)
            {
                if (i == daynum)
                    sqltmp += "D" + i + "b,D" + i + "y";
                else sqltmp += "D" + i + "b,D" + i + "y,";

            }
            return sqltmp;
        }
        #endregion
        #endregion
    }
}