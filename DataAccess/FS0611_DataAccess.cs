﻿using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;
using System.Collections;

namespace DataAccess
{
    public class FS0611_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 取日历
        public DataTable GetCalendar(string strPlant, string vcDXYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from TCalendar_PingZhun_Wai where vcFZGC='" + strPlant + "' and TARGETMONTH='" + vcDXYM + "'  and TOTALWORKDAYS>0   \n");
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
            sql.Append("where vcYearMonth='" + strYearMonth + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='1'   \n");
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


            sql.Append("    select vcPart_id, " + strhycolumn + " as iHyNum,iQuantityPercontainer,    \n");
            sql.Append("    case when b.dFromTime is not null or b.dToTime is not null then b.dFromTime    \n");
            sql.Append("    when c.dFromTime is not null or c.dToTime is not null then c.dFromTime    \n");
            sql.Append("    when d.dFromTime is not null or d.dToTime is not null then d.dFromTime    \n");
            sql.Append("    when e.dFromTime is not null or e.dToTime is not null then e.dFromTime    \n");
            sql.Append("    end as dFromTime,    \n");
            sql.Append("    case when b.dFromTime is not null or b.dToTime is not null then b.dToTime    \n");
            sql.Append("    when c.dFromTime is not null or c.dToTime is not null then c.dToTime    \n");
            sql.Append("    when d.dFromTime is not null or d.dToTime is not null then d.dToTime    \n");
            sql.Append("    when e.dFromTime is not null or e.dToTime is not null then e.dToTime    \n");
            sql.Append("    end as dToTime    \n");
            sql.Append("    ,a.vcReceiver from TSoq a         \n");
            sql.Append("    left join       \n");
            sql.Append("    (       \n");
            sql.Append("       select vcPartId,vcSupplierId,vcReceiver,dFromTime,dToTime from TSPMaster      \n");
            sql.Append("  	 where      \n");
            sql.Append("  	 convert(varchar(6),dFromTime,112)='" + strYearMonth + "' or    \n");
            sql.Append("  	 convert(varchar(6),dToTime,112)='" + strYearMonth + "'    \n");
            sql.Append("    )b on a.vcPart_id=b.vcPartId and a.vcSupplier_id=b.vcSupplierId and a.vcReceiver=b.vcReceiver       \n");
            sql.Append("    left join       \n");
            sql.Append("    (       \n");
            sql.Append("  	select vcPartId,vcSupplierId,vcReceiver,dFromTime,dToTime from TSPMaster_SupplierPlant     \n");
            sql.Append("  	where      \n");
            sql.Append("  	convert(varchar(6),dFromTime,112)='" + strYearMonth + "' or    \n");
            sql.Append("  	convert(varchar(6),dToTime,112)='" + strYearMonth + "'    \n");
            sql.Append("    )c on a.vcPart_id=c.vcPartId and a.vcSupplier_id=c.vcSupplierId and a.vcReceiver=c.vcReceiver       \n");
            sql.Append("    left join       \n");
            sql.Append("    (       \n");
            sql.Append("  	select vcPartId,vcSupplierId,vcReceiver,dFromTime,dToTime from TSPMaster_SufferIn      \n");
            sql.Append("  	where      \n");
            sql.Append("  	convert(varchar(6),dFromTime,112)='" + strYearMonth + "' or    \n");
            sql.Append("  	convert(varchar(6),dToTime,112)='" + strYearMonth + "'    \n");
            sql.Append("    )d on a.vcPart_id=d.vcPartId and a.vcSupplier_id=d.vcSupplierId and a.vcReceiver=d.vcReceiver     \n");
            sql.Append("    left join       \n");
            sql.Append("    (       \n");
            sql.Append("  	select vcPartId,vcSupplierId,vcReceiver,dFromTime,dToTime from TSPMaster_Box      \n");
            sql.Append("  	where      \n");
            sql.Append("  	convert(varchar(6),dFromTime,112)='" + strYearMonth + "' or    \n");
            sql.Append("  	convert(varchar(6),dToTime,112)='" + strYearMonth + "'    \n");
            sql.Append("    )e on a.vcPart_id=e.vcPartId and a.vcSupplier_id=e.vcSupplierId and a.vcReceiver=e.vcReceiver      \n");
            sql.Append("   where vcYearMonth='" + strYearMonth + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='1'  and a.vcHyState='2'    \n");
            sql.Append("    order by a.iAutoId      \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion


        #region 取特殊厂家对应的品番
        public DataTable GetSpecialSupplier(string strPlant,string strDXYearMonth, string strYearMonth)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("   select a.vcPartId,c.dBeginDate,c.dEndDate,c.vcSupplier_id from TSPMaster a    \n");
            sql.Append("   left join    \n");
            sql.Append("   (    \n");
            sql.Append("      select * from TSPMaster_SupplierPlant    \n");
            sql.Append("   )b on a.vcPartId=b.vcPartId  and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId   \n");
            sql.Append("   inner join    \n");
            sql.Append("   (    \n");
            sql.Append("     select vcSupplier_id,vcWorkArea,dBeginDate,dEndDate from TSpecialSupplier where convert(varchar(6),dBeginDate,112)='"+ strYearMonth + "' and convert(varchar(6),dEndDate,112)='"+ strYearMonth + "'   \n");
            sql.Append("   )c on b.vcSupplierId=c.vcSupplier_id and b.vcSupplierPlant=c.vcWorkArea   \n");
            sql.Append("   inner join     \n");
            sql.Append("   (     \n");
            sql.Append("     select vcPart_id from TSoq  where vcFZGC='" + strPlant + "' and vcYearMonth='" + strDXYearMonth + "'       \n");
            sql.Append("   )d on a.vcPartId=d.vcPart_id     \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion

        #region 取特殊品番
        public DataTable GetSpecialPartId(string strPlant,string strDXYearMonth, string strYearMonth)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("   select vcPartNo as vcPartId,dBeginDate,dEndDate from TSpecialPartNo a     \n");
            sql.Append("   inner join     \n");
            sql.Append("   (     \n");
            sql.Append("     select vcPart_id from TSoq  where vcFZGC='"+ strPlant + "' and vcYearMonth='"+ strDXYearMonth + "'       \n");
            sql.Append("   )b on a.vcPartNo=b.vcPart_id     \n");
            sql.Append("   where convert(varchar(6),dBeginDate,112)='" + strYearMonth + "' and convert(varchar(6),dEndDate,112)='" + strYearMonth + "'     \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion


        #region 获取展开的数据
        public DataTable getZhankaiData(bool isZhankai)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("   select * from TSOQReply where  vcCLYM=convert(varchar(6),getdate(),112) and vcInOutFlag='1' \n");
            if (isZhankai)
                sql.Append("  and dZhanKaiTime is not null  ");
            else
                sql.Append("  and dZhanKaiTime is null  ");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion


        #region 更新平准化结果
        public void SaveResult(string strCLYM, string strDXYM, string strNSYM, string strNNSYM, string strPlant,
            ArrayList arrResult_DXYM, ArrayList arrResult_NSYM, ArrayList arrResult_NNSYM, string strUserId,string strUnit)
        {
            SqlCommand cmd;
            SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString());//对账平台
            ComConnectionHelper.OpenConection_SQL(ref conn);
            cmd = new SqlCommand();
            SqlTransaction st = conn.BeginTransaction();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("delete from TSoqReply where vcCLYM='" + strCLYM + "' and vcDXYM in ('" + strDXYM + "','" + strNSYM + "','" + strNNSYM + "') " +
                    "and vcFZGC='" + strPlant + "' and vcInOutFlag='1'    \n");
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
                    "and vcInOutFlag='1')t1    \n");
                sql.Append("left join(    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcOrderingMethod from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='"+strUnit+ "' and vcReceiver='APC06' and '" + strDXYM + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");//TFTM和APC06是写死的
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");

                sql.Append("update t1 set t1.vcSupplier_id=t2.vcSupplier_id,t1.vcSR=t6.vcSufferIn     \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoqReply where vcCLYM='" + strCLYM + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='1'   \n");
                sql.Append(")t1    \n");
                sql.Append("left join (    \n");
                sql.Append("	select * from TSoq    \n");
                sql.Append("	where vcYearMonth='" + strDXYM + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='1'   \n");
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
            sql.Append("           ,'1'    \n");//内制/外注
            sql.Append("           ,'" + strCLYM + "'    \n");//处理年月
            sql.Append("           ,'" + strDXYM + "'    \n");//对象年月
            sql.Append("           ,'" + arr[0] + "'    \n");//品番
            sql.Append("           ,''    \n");//vcCarType 品番表关联
            sql.Append("           ,nullif(" + arr[2] + ",'')    \n");//收容数
            sql.Append("           ,nullif(" + arr[3] + ",'')    \n");//箱数
            sql.Append("           ,nullif(" + arr[1] + ",'')    \n");//订单数
            sql.Append("           ,nullif(" + arr[4] + "*" + arr[2] + ",'')    \n");//D1
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

        #region 展开SOQReply
        public int zk(string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" update TSOQReply ");
                strSql.AppendLine(" set   ");
                strSql.AppendLine(" vcZhanKaiID='"+ userId + "', ");
                strSql.AppendLine(" dZhanKaiTime=getDate()");
                strSql.AppendLine(" where ");
                //1:外注
                strSql.AppendLine(" vcCLYM=convert(varchar(6),getdate(),112) ");
                strSql.AppendLine(" and dZhanKaiTime is null ");
                strSql.AppendLine(" and vcInOutFlag='1'; ");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 下载SOQReply（检索内容）
        public DataTable search(string strYearMonth, string strYearMonth_2, string strYearMonth_3)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                string strCLYM = System.DateTime.Now.ToString("yyyyMM");
                strSql.AppendLine(" SELECT a.*,b.[N+1 O/L],b.[N+1 Units],b.[N+1 PCS], ");
                strSql.AppendLine(" c.[N+2 O/L],c.[N+2 Units],c.[N+2 PCS],d.vcName as '订货方式',e.vcName as  '发注工厂' ");
                strSql.AppendLine(" FROM ");
                strSql.AppendLine(" ( ");
                strSql.AppendLine("   SELECT ");
                strSql.AppendLine("   vcPart_id as 'PartsNo',");
                //发注工厂
                strSql.AppendLine("   vcFZGC,");
                //订货频度
                strSql.AppendLine("   vcMakingOrderType,");
                strSql.AppendLine("   vcCarType as 'CFC',");
                strSql.AppendLine("   isnull(iQuantityPercontainer,0) as 'OrdLot',");
                strSql.AppendLine("   isnull(iBoxes,0) as 'N Units',");
                strSql.AppendLine("   isnull(iPartNums,0) as 'N PCS',");
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
                strSql.AppendLine("   iAutoId");
                strSql.AppendLine("   FROM TSOQReply WHERE vcCLYM='" + strCLYM + "' and  vcInOutFlag='1'  AND vcDXYM='" + strYearMonth + "'");//外注
                strSql.AppendLine(" ) a ");

                strSql.AppendLine(" LEFT JOIN (   ");
                strSql.AppendLine("   SELECT vcPart_id,isnull(iQuantityPercontainer,0) as 'N+1 O/L',isnull(iBoxes,0) as 'N+1 Units',isnull(iPartNums,0) as 'N+1 PCS' ");
                strSql.AppendLine("   FROM TSOQReply   ");
                strSql.AppendLine("   WHERE vcCLYM='" + strCLYM + "' and  vcInOutFlag='1'  AND vcDXYM='" + strYearMonth_2 + "' ");//外注
                strSql.AppendLine("  ) b ");
                strSql.AppendLine(" ON a.PartsNo=b.vcPart_id ");

                strSql.AppendLine(" LEFT JOIN (   ");
                strSql.AppendLine("   SELECT vcPart_id,isnull(iQuantityPercontainer,0) as 'N+2 O/L',isnull(iBoxes,0) as 'N+2 Units',isnull(iPartNums,0) as 'N+2 PCS' ");
                strSql.AppendLine("   FROM TSOQReply   ");
                strSql.AppendLine("   WHERE vcCLYM='" + strCLYM + "' and  vcInOutFlag='1'  AND vcDXYM='" + strYearMonth_3 + "' ");//外注
                strSql.AppendLine("  ) c ");
                strSql.AppendLine(" ON a.PartsNo=c.vcPart_id ");

                strSql.AppendLine("left join (select * from TCode where vcCodeId='C047')d on a.vcMakingOrderType=d.vcValue    \n");
                strSql.AppendLine("left join (select * from TCode where vcCodeId='C000')e on a.vcFZGC=e.vcValue    \n");

                
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
        public void importSave(DataTable dt, string strYearMonth,string strUserId)
        {
            try
            {
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
                    sql.Append("      '" + dt.Rows[i]["vcPart_id"] + "',nullif('" + dt.Rows[i]["iBoxes"].ToString() + "',''),      \n");
                    sql.Append("      nullif('" + dt.Rows[i]["iQuantityPercontainer"].ToString() + "','')      \n");

                    for (int j = 1; j < 32; j++)
                    {
                        sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["iD" + j], true) + "      \n");

                        //string strIDTemp = dt.Rows[i]["iD" + j] == System.DBNull.Value ? "" : dt.Rows[i]["iD" + j].ToString();
                        //if(strIDTemp!="" && Convert.ToInt32(strIDTemp)!=0)
                        //{
                        //    string strSRS = dt.Rows[i]["iQuantityPercontainer"] == System.DBNull.Value ? "" : dt.Rows[i]["iQuantityPercontainer"].ToString();//箱数*收容数
                        //    int iSRS = Convert.ToInt32(strSRS.Trim());//收容数
                        //    int iD =  Convert.ToInt32(strIDTemp.Trim()) / iSRS;
                        //    sql.Append("      ," + iD + "      \n");
                        //}
                        //else
                        //    sql.Append("      ,null      \n");
                    }
                    sql.Append("      );      \n");
                }

                sql.Append("      update TSOQReply set vcOperatorID='"+ strUserId + "',dOperatorTime=getdate(),iBoxes=b.iBoxes,iPartNums=b.iQuantityPercontainer*b.iBoxes,      \n");
                for (int j = 1; j < 32; j++)
                {
                    sql.Append(" iD" + j + "=b.iD" + j  );
                    if(j!=31)
                        sql.Append(",");
                }
                sql.Append("      from   TSOQReply a    \n");
                sql.Append("      inner join(      \n");
                sql.Append("        select * from #TSOQReply      \n");
                sql.Append("      ) b on a.vcPart_id=b.vcPart_id      \n");
                sql.Append("      and  a.vcInOutFlag='1'   \n ");//外注
                sql.Append("      and  a.vcDXYM='" + strYearMonth + "'; \n  ");

                string vcCLYM = System.DateTime.Now.ToString("yyyyMM");

                //清空3个月展开人和展开时间
                sql.Append("update t1 set vcZhanKaiID=null,dZhanKaiTime=null   \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoqReply    \n");
                sql.Append("	where vcCLYM='" + vcCLYM + "' and vcInOutFlag='1'     \n");
                sql.Append(")t1    \n");
                sql.Append("inner join (    \n");
                sql.Append("	select * from #TSOQReply    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPart_id    \n");

                sql.Append("delete t1    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoqReply    \n");
                sql.Append("	where vcCLYM='" + vcCLYM + "'  and vcInOutFlag='1'    \n");
                sql.Append(")t1    \n");
                sql.Append("left join (    \n");
                sql.Append("	select * from #TSOQReply    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPart_id    \n");
                sql.Append("where t2.vcPart_id is null    \n");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

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
    }
}