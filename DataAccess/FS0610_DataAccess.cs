using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;
using System.Collections;

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
                sql.Append("select * from TCalendar_PingZhun_Nei where vcFZGC='"+strPlant+"' and TARGETMONTH='"+vcDXYM+"'    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取soq数据
        public DataTable GetSoq(string strPlant, string strDXYM,string strType)
        {
            StringBuilder sql = new StringBuilder();
            string strhycolumn = "";
            if (strType == "dxym")
                strhycolumn = "iHySOQN";
            else if (strType == "nsym")
                strhycolumn = "iHySOQN1";
            else if (strType == "nnsym")
                strhycolumn = "iHySOQN2";
            sql.Append("select vcPart_id, "+strhycolumn+",iQuantityPercontainer from TSoq     \n");
            sql.Append("where vcYearMonth='"+strDXYM+"' and vcFZGC='"+strPlant+"' and vcInOutFlag='0' and vcHyState='2'    \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion

        #region 更新平准化结果
        public void SaveResult(string strCLYM, string strDXYM, string strNSYM, string strNNSYM, string strPlant,
            ArrayList arrResult_DXYM, ArrayList arrResult_NSYM, ArrayList arrResult_NNSYM,string strUserId)
        {
            SqlCommand cmd;
            SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString());//对账平台
            ComConnectionHelper.OpenConection_SQL(ref conn);
            cmd = new SqlCommand();
            SqlTransaction st = conn.BeginTransaction();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("delete from TSoqReply where vcCLYM='"+strCLYM+"' and vcDXYM in ('"+ strDXYM + "','"+ strNSYM + "','"+ strNNSYM + "') " +
                    "and vcFZGC='" + strPlant+"' and vcInOutFlag='0'    \n");
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
                sql.Append("	where vcCLYM='"+ strCLYM + "' and vcDXYM in ('" + strDXYM + "','" + strNSYM + "','" + strNNSYM + "') and vcFZGC='"+ strPlant + "' " +
                    "and vcInOutFlag='0')t1    \n");
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
        public StringBuilder Resultsql(string[] arr,string strPlant,string strCLYM,string strDXYM,string strUserId)
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
                sql.Append("           ('"+strPlant+"'    \n");//工厂
                sql.Append("           ,''    \n");//vcMakingOrderType 品番表关联
                sql.Append("           ,'0'    \n");//内制/外注
                sql.Append("           ,'"+strCLYM+"'    \n");//处理年月
                sql.Append("           ,'"+strDXYM+"'    \n");//对象年月
                sql.Append("           ,'"+ arr[0] + "'    \n");//品番
                sql.Append("           ,''    \n");//vcCarType 品番表关联
                sql.Append("           ,nullif("+arr[2]+",'')    \n");//收容数
                sql.Append("           ,nullif("+arr[3]+",'')    \n");//箱数
                sql.Append("           ,nullif("+arr[1]+",'')    \n");//订单数
                sql.Append("           ,nullif("+arr[4]+",'')    \n");//D1
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
                sql.Append("           ,'"+strUserId+"'    \n");//操作者
                sql.Append("           ,getdate())    \n");//操作时间
                #endregion
            return sql;
        }
        #endregion

        #region 将平准结果存储进数据库
        public int save(DataTable saveTable, string userId,string varDxny,int iMonthFlag)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varCreater", SqlDbType.VarChar),
                    new SqlParameter("@dCreateTime", SqlDbType.DateTime),
                    new SqlParameter("@TARGETMONTH", SqlDbType.VarChar),
                    new SqlParameter("@iMonthFlag", SqlDbType.Int),
                };

                parameters[0].Value = userId;
                parameters[1].Value = DateTime.Now;
                parameters[2].Value = varDxny;
                parameters[3].Value = iMonthFlag;

                //先删除，后插入
                strSql.AppendLine(" DELETE TSOQReply ");
                strSql.AppendLine(" WHERE ");
                strSql.AppendLine(" TARGETMONTH=@TARGETMONTH ");
                //内制
                strSql.AppendLine(" AND INOUTFLAG='0'; ");



                strSql.AppendLine(" INSERT INTO TSOQReply( ");
                for (int i = 0; i < saveTable.Columns.Count; i++)
                {
                    strSql.AppendLine(saveTable.Columns[i] + ",");
                }

                strSql.AppendLine(" TARGETMONTH, ");
                strSql.AppendLine(" iMonthFlag, ");
                strSql.AppendLine(" varCreater, ");
                strSql.AppendLine(" dCreateTime) ");


                strSql.AppendLine(" VALUES");
                for (int i = 0; i < saveTable.Rows.Count; i++)
                {
                    for (int j = 0; j < saveTable.Columns.Count; j++)
                    {
                        string re ;

                        string ColumnName = saveTable.Columns[j].ColumnName.ToString();
                        int days;
                        if (ColumnName[0] == 'D' && int.TryParse(ColumnName.Replace('D', ' '), out days))
                        {
                            re = string.IsNullOrEmpty(saveTable.Rows[i][j].ToString()) ? "0" : "'" + saveTable.Rows[i][j] + "'";
                        }
                        else {
                            re = string.IsNullOrEmpty(saveTable.Rows[i][j].ToString()) ? "NULL" : "'" + saveTable.Rows[i][j] + "'";
                        }

                        if (j == 0)
                        {
                            strSql.AppendLine("(" + re + ",");
                        }
                        else
                        {
                            strSql.AppendLine("" + re + ",");
                        }
                    }

                    strSql.AppendLine(" @TARGETMONTH, ");
                    strSql.AppendLine(" @iMonthFlag, ");
                    strSql.AppendLine(" @varCreater, ");

                    if (i == saveTable.Rows.Count - 1)
                        strSql.AppendLine(" @dCreateTime) ");
                    else
                        strSql.AppendLine(" @dCreateTime), ");
                }

                strSql.Append(" ; ");


                return excute.ExcuteSqlWithStringOper(strSql.ToString(),parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 展开SOQReply
        public int zk(string varDxny, string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@varZhanKai", SqlDbType.VarChar),
                    new SqlParameter("@dZhanKaiTime", SqlDbType.VarChar),
                };

                parameters[0].Value = varDxny;
                parameters[1].Value = userId;
                parameters[2].Value = DateTime.Now;

                strSql.AppendLine(" UPDATE TSOQProcess ");
                strSql.AppendLine(" SET iStatus=1, ");
                strSql.AppendLine(" varZhanKai=@varZhanKai, ");
                strSql.AppendLine(" dZhanKaiTime=@dZhanKaiTime ");
                strSql.AppendLine(" WHERE ");
                strSql.AppendLine(" varDxny=@varDxny ");
                //0:内制
                strSql.AppendLine(" AND INOUTFLAG=0; ");

                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 下载SOQReply（检索内容）
        public DataTable search(string varDxny)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                };
                parameters[0].Value = varDxny;

                strSql.AppendLine(" SELECT tn.*,tn1.[N+1 O/L],tn1.[N+1 Units],tn1.[N+1 PCS], ");
                strSql.AppendLine(" tn2.[N+2 O/L],tn2.[N+2 Units],tn2.[N+2 PCS] ");
                strSql.AppendLine(" FROM ");

                strSql.AppendLine(" (SELECT ");
                strSql.AppendLine(" PARTSNO as 'PartsNo',");
                //发注工厂
                strSql.AppendLine(" iFZGC as '发注工厂',");
                //订货频度
                strSql.AppendLine(" varMakingOrderType as '订货频度',");
                strSql.AppendLine(" CARFAMILYCODE as 'CFC',");
                strSql.AppendLine(" QUANTITYPERCONTAINER as 'OrdLot',");
                strSql.AppendLine(" iUnits as 'N Units',");
                strSql.AppendLine(" iPCS as 'N PCS',");
                strSql.AppendLine(" D1,");
                strSql.AppendLine(" D2,");
                strSql.AppendLine(" D3,");
                strSql.AppendLine(" D4,");
                strSql.AppendLine(" D5,");
                strSql.AppendLine(" D6,");
                strSql.AppendLine(" D7,");
                strSql.AppendLine(" D8,");
                strSql.AppendLine(" D9,");
                strSql.AppendLine(" D10,");
                strSql.AppendLine(" D11,");
                strSql.AppendLine(" D12,");
                strSql.AppendLine(" D13,");
                strSql.AppendLine(" D14,");
                strSql.AppendLine(" D15,");
                strSql.AppendLine(" D16,");
                strSql.AppendLine(" D17,");
                strSql.AppendLine(" D18,");
                strSql.AppendLine(" D19,");
                strSql.AppendLine(" D20,");
                strSql.AppendLine(" D21,");
                strSql.AppendLine(" D22,");
                strSql.AppendLine(" D23,");
                strSql.AppendLine(" D24,");
                strSql.AppendLine(" D25,");
                strSql.AppendLine(" D26,");
                strSql.AppendLine(" D27,");
                strSql.AppendLine(" D28,");
                strSql.AppendLine(" D29,");
                strSql.AppendLine(" D30,");
                strSql.AppendLine(" D31");

                strSql.AppendLine(" FROM TSOQReply ");

                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny ");
                //内制
                strSql.AppendLine(" AND INOUTFLAG='0' ");
                //对象月
                strSql.AppendLine(" AND iMonthFlag=0) tn ");

                strSql.AppendLine(" LEFT JOIN (");
                strSql.AppendLine(" SELECT PARTSNO, ");
                strSql.AppendLine(" QUANTITYPERCONTAINER as 'N+1 O/L',");
                strSql.AppendLine(" iUnits as 'N+1 Units',");
                strSql.AppendLine(" iPCS as 'N+1 PCS'");
                strSql.AppendLine(" FROM TSOQReply ");
                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny ");
                //内制
                strSql.AppendLine(" AND INOUTFLAG='0' ");
                //内示月
                strSql.AppendLine(" AND iMonthFlag=1) tn1 ");

                strSql.AppendLine(" ON tn.PartsNo=tn1.PARTSNO ");


                strSql.AppendLine(" LEFT JOIN (");
                strSql.AppendLine(" SELECT PARTSNO, ");
                strSql.AppendLine(" QUANTITYPERCONTAINER as 'N+2 O/L',");
                strSql.AppendLine(" iUnits as 'N+2 Units',");
                strSql.AppendLine(" iPCS as 'N+2 PCS'");
                strSql.AppendLine(" FROM TSOQReply ");
                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny ");
                //内制
                strSql.AppendLine(" AND INOUTFLAG='0' ");
                //内内示月
                strSql.AppendLine(" AND iMonthFlag=2) tn2 ");

                strSql.AppendLine(" ON tn.PartsNo=tn2.PARTSNO ");


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string varDxny)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar,10),
                };
                parameters[0].Value = varDxny;


                StringBuilder sql = new StringBuilder();

                for (int i = 0; i < dt.Rows.Count; i++) {
                    //如果为空则跳过
                    if (string.IsNullOrEmpty(dt.Rows[i]["PARTSNO"].ToString()))
                        continue;

                    //更新对象月数据。（内示与内内示月不更新）
                    sql.AppendLine("  UPDATE TSOQReply SET ");
                    for (int j = 1; j < 32; j++) {
                        sql.AppendLine(" D"+j+"=" + dt.Rows[i]["D"+j+""] + ",");
                    }
                    sql.AppendLine(" iPCS=" + dt.Rows[i]["iPCS"] + ",");
                    sql.AppendLine(" iUnits=" + dt.Rows[i]["iUnits"] + ",");
                    sql.AppendLine(" QUANTITYPERCONTAINER=" + dt.Rows[i]["QUANTITYPERCONTAINER"] );

                    sql.AppendLine(" WHERE PARTSNO='"+ dt.Rows[i]["PARTSNO"]+"'");
                    sql.AppendLine(" AND TARGETMONTH=@varDxny ");
                    //内制
                    sql.AppendLine(" AND INOUTFLAG='0' ");
                    //对象月
                    sql.AppendLine(" AND iMonthFlag=0; ");
                }
                
                excute.ExcuteSqlWithStringOper(sql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}