using System;
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
                sql.Append("select * from TCalendar_PingZhun_Wai where vcFZGC='" + strPlant + "' and TARGETMONTH='" + vcDXYM + "'    \n");
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
            sql.Append("select vcPart_id, " + strhycolumn + " as iHyNum,iQuantityPercontainer from TSoq     \n");
            sql.Append("where vcYearMonth='" + strYearMonth + "' and vcFZGC='" + strPlant + "' and vcInOutFlag='1' and vcHyState='2'    \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion


        #region 取特殊厂家对应的品番
        public DataTable GetSpecialSupplier(string strYearMonth)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("   select a.vcPartId,c.dBeginDate,c.dEndDate from TSPMaster a    \n");
            sql.Append("   left join    \n");
            sql.Append("   (    \n");
            sql.Append("      select * from TSPMaster_SupplierPlant    \n");
            sql.Append("   )b on a.vcPartId=b.vcPartId  and a.vcPackingPlant=b.vcPackingPlant and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplierId   \n");
            sql.Append("   inner join    \n");
            sql.Append("   (    \n");
            sql.Append("     select vcSupplier_id,vcWorkArea,dBeginDate,dEndDate from TSpecialSupplier where convert(varchar(6),dBeginDate,112)='"+ strYearMonth + "' and convert(varchar(6),dEndDate,112)='"+ strYearMonth + "'   \n");
            sql.Append("   )c on b.vcSupplierId=c.vcSupplier_id and b.vcSupplierPlant=c.vcWorkArea   \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion

        #region 取特殊品番
        public DataTable GetSpecialPartId(string strYearMonth)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("   select vcPartNo as vcPartId,dBeginDate,dEndDate from TSpecialPartNo where convert(varchar(6),dBeginDate,112)='" + strYearMonth + "' and convert(varchar(6),dEndDate,112)='" + strYearMonth + "'   \n");
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
            ArrayList arrResult_DXYM, ArrayList arrResult_NSYM, ArrayList arrResult_NNSYM, string strUserId)
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
                sql.Append("	where vcPackingPlant='TFTM' and vcReceiver='APC06' and GETDATE() between dFromTime and dToTime    \n");//TFTM和APC06是写死的
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");

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
            sql.Append("           ,nullif(" + arr[4] + ",'')    \n");//D1
            sql.Append("           ,nullif(" + arr[5] + ",'')    \n");//D2
            sql.Append("           ,nullif(" + arr[6] + ",'')    \n");//D3
            sql.Append("           ,nullif(" + arr[7] + ",'')    \n");//D4
            sql.Append("           ,nullif(" + arr[8] + ",'')    \n");//D5
            sql.Append("           ,nullif(" + arr[9] + ",'')    \n");//D6
            sql.Append("           ,nullif(" + arr[10] + ",'')   \n");//D7
            sql.Append("           ,nullif(" + arr[11] + ",'')   \n");//D8
            sql.Append("           ,nullif(" + arr[12] + ",'')   \n");//D9
            sql.Append("           ,nullif(" + arr[13] + ",'')    \n");//D10
            sql.Append("           ,nullif(" + arr[14] + ",'')    \n");//D11
            sql.Append("           ,nullif(" + arr[15] + ",'')    \n");//D12
            sql.Append("           ,nullif(" + arr[16] + ",'')    \n");//D13
            sql.Append("           ,nullif(" + arr[17] + ",'')    \n");//D14
            sql.Append("           ,nullif(" + arr[18] + ",'')    \n");//D15
            sql.Append("           ,nullif(" + arr[19] + ",'')    \n");//D16
            sql.Append("           ,nullif(" + arr[20] + ",'')    \n");//D17
            sql.Append("           ,nullif(" + arr[21] + ",'')    \n");//D18
            sql.Append("           ,nullif(" + arr[22] + ",'')    \n");//D19
            sql.Append("           ,nullif(" + arr[23] + ",'')    \n");//D20
            sql.Append("           ,nullif(" + arr[24] + ",'')    \n");//D21
            sql.Append("           ,nullif(" + arr[25] + ",'')    \n");//D22
            sql.Append("           ,nullif(" + arr[26] + ",'')    \n");//D23
            sql.Append("           ,nullif(" + arr[27] + ",'')    \n");//D24
            sql.Append("           ,nullif(" + arr[28] + ",'')    \n");//D25
            sql.Append("           ,nullif(" + arr[29] + ",'')    \n");//D26
            sql.Append("           ,nullif(" + arr[30] + ",'')    \n");//D27
            sql.Append("           ,nullif(" + arr[31] + ",'')    \n");//D28
            sql.Append("           ,nullif(" + arr[32] + ",'')    \n");//D29
            sql.Append("           ,nullif(" + arr[33] + ",'')    \n");//D30
            sql.Append("           ,nullif(" + arr[34] + ",'')    \n");//D31
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

                strSql.AppendLine(" SELECT a.*,b.[N+1 O/L],b.[N+1 Units],b.[N+1 PCS], ");
                strSql.AppendLine(" c.[N+2 O/L],c.[N+2 Units],c.[N+2 PCS] ");
                strSql.AppendLine(" FROM ");
                strSql.AppendLine(" ( ");
                strSql.AppendLine("   SELECT ");
                strSql.AppendLine("   vcPart_id as 'PartsNo',");
                //发注工厂
                strSql.AppendLine("   vcFZGC as '发注工厂',");
                //订货频度
                strSql.AppendLine("   vcMakingOrderType as '订货频度',");
                strSql.AppendLine("   vcCarType as 'CFC',");
                strSql.AppendLine("   iQuantityPercontainer as 'OrdLot',");
                strSql.AppendLine("   iBoxes as 'N Units',");
                strSql.AppendLine("   iPartNums as 'N PCS',");
                strSql.AppendLine("   iD1,");
                strSql.AppendLine("   iD2,");
                strSql.AppendLine("   iD3,");
                strSql.AppendLine("   iD4,");
                strSql.AppendLine("   iD5,");
                strSql.AppendLine("   iD6,");
                strSql.AppendLine("   iD7,");
                strSql.AppendLine("   iD8,");
                strSql.AppendLine("   iD9,");
                strSql.AppendLine("   iD10,");
                strSql.AppendLine("   iD11,");
                strSql.AppendLine("   iD12,");
                strSql.AppendLine("   iD13,");
                strSql.AppendLine("   iD14,");
                strSql.AppendLine("   iD15,");
                strSql.AppendLine("   iD16,");
                strSql.AppendLine("   iD17,");
                strSql.AppendLine("   iD18,");
                strSql.AppendLine("   iD19,");
                strSql.AppendLine("   iD20,");
                strSql.AppendLine("   iD21,");
                strSql.AppendLine("   iD22,");
                strSql.AppendLine("   iD23,");
                strSql.AppendLine("   iD24,");
                strSql.AppendLine("   iD25,");
                strSql.AppendLine("   iD26,");
                strSql.AppendLine("   iD27,");
                strSql.AppendLine("   iD28,");
                strSql.AppendLine("   iD29,");
                strSql.AppendLine("   iD30,");
                strSql.AppendLine("   iD31,");
                strSql.AppendLine("   iAutoId");
                strSql.AppendLine("   FROM TSOQReply WHERE vcInOutFlag='1'  AND vcDXYM='" + strYearMonth + "'");//外注
                strSql.AppendLine(" ) a ");

                strSql.AppendLine(" LEFT JOIN (   ");
                strSql.AppendLine("   SELECT vcPart_id,vcCarType as 'N+1 O/L',iBoxes as 'N+1 Units',iPartNums as 'N+1 PCS' ");
                strSql.AppendLine("   FROM TSOQReply   ");
                strSql.AppendLine("   WHERE vcInOutFlag='1'  AND vcDXYM='" + strYearMonth_2 + "' ");//外注
                strSql.AppendLine("  ) b ");
                strSql.AppendLine(" ON a.PartsNo=b.vcPart_id ");

                strSql.AppendLine(" LEFT JOIN (   ");
                strSql.AppendLine("   SELECT vcPart_id,vcCarType as 'N+2 O/L',iBoxes as 'N+2 Units',iPartNums as 'N+2 PCS' ");
                strSql.AppendLine("   FROM TSOQReply   ");
                strSql.AppendLine("   WHERE vcInOutFlag='1'  AND vcDXYM='" + strYearMonth_3 + "' ");//外注
                strSql.AppendLine("  ) c ");
                strSql.AppendLine(" ON a.PartsNo=c.vcPart_id ");

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
                sql.Append("      	select * from TSOQReply where 1=0      \n");
                sql.Append("      ) a      ;\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcPart_id"].ToString() == "")
                        continue;
                    sql.Append("            \n");
                    sql.Append("      insert into #TSOQReply       \n");
                    sql.Append("       (         \n");
                    sql.Append("       vcPart_id      \n");
                    sql.Append("       ,iD1 ,iD2 ,iD3 ,iD4 ,iD5 ,iD6 ,iD7 ,iD8 ,iD9 ,iD10   \n");
                    sql.Append("       ,iD11 ,iD12 ,iD13 ,iD14 ,iD15 ,iD16 ,iD17 ,iD18 ,iD19 ,iD20   \n");
                    sql.Append("       ,iD21 ,iD22 ,iD23 ,iD24 ,iD25 ,iD26 ,iD27 ,iD28 ,iD29 ,iD30 ,iD31  \n");
                    sql.Append("       ) values         \n");
                    sql.Append("      (      \n");
                    sql.Append("      '" + dt.Rows[i]["vcPart_id"] + "'      \n");
                    for (int j = 1; j < 32; j++)
                    {
                        sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["iD"+ j], true) + "      \n");
                    }
                    sql.Append("      );      \n");
                }

                sql.Append("      update TSOQReply set vcOperatorID='"+ strUserId + "',dOperatorTime=getdate(),      \n");
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