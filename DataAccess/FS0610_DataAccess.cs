using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;
using System.Collections;
using System.Collections.Generic;

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

        #region 生产计划方法（王立伟）2020-01-21
        /// <summary>
        /// 生成生产计划
        /// </summary>
        /// <param name="vcDxny">年月</param>
        /// <param name="vcFZGC">工厂</param>
        /// <returns></returns>
        public string createProPlan(string vcDxny, string vcFZGC)
        {
            return "";
        }

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
            string ssql = "";
            ssql += "  select t1.vcFZGC, t1.vcPart_id, t2.vcDock, t2.vcCarFamilyCode, t2.vcQJcontainer as kbsrs, t2.iQuantityPerContainer as srs, ";
            ssql += "  t1.iBoxes*t2.iQuantityPerContainer as num, t2.vcPorType, t2.vcZB, t3.KBpartType, t2.vcQFflag, ";
            ssql += "  t3.vcProName0,t3.vcLT0, t3.vcCalendar0, ";
            ssql += "  t3.vcProName1,t3.vcLT1, t3.vcCalendar1, ";
            ssql += "  t3.vcProName2,t3.vcLT2, t3.vcCalendar2, ";
            ssql += "  t3.vcProName3,t3.vcLT3, t3.vcCalendar3, ";
            ssql += "  t3.vcProName4,t3.vcLT4, t3.vcCalendar4  ";
            ssql += "  from TSoqReply t1 ";
            ssql += "  left join tPartInfoMaster t2 ";
            ssql += "  on t1.vcPart_id = t2.vcPartsNo and t1.vcCarType = t2.vcCarFamilyCode ";
            ssql += "  left join ProRuleMst t3 ";
            ssql += "  on t3.vcPorType=t2.vcPorType and t3.vcZB = t2.vcZB ";
            // 王立伟注释掉 //ssql += "  where t1.vcDXYM='" + mon + "' and updateFlag='0' and iPartNums <>'0' and vcFZGC='" + plant + "' and t2.dTimeFrom<='" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "'";
            ssql += "  where t1.vcDXYM='" + mon + "' and iPartNums <>'0' and vcFZGC='" + plant + "' and t2.dTimeFrom<='" + mon1 + "-01" + "' and t2.dTimeTo >= '" + mon1 + "-01" + "'";
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
                DataTable dt2 = new DataTable();//20180929实测没用，是为了把变量apt引出 - 李兴旺
                cmd.CommandText = "select TOP(1) * from " + TableName;//20180929实测没用，是为了把变量apt引出 - 李兴旺
                SqlDataAdapter apt = new SqlDataAdapter(cmd);//20180929实测没用，是为了把变量apt引出 - 李兴旺
                apt.Fill(dt2);//20180929实测没用，是为了把变量apt引出 - 李兴旺
                cmd.CommandText = "delete from " + TableName + " where (vcMonth='" + lbltime + "' or montouch ='" + lbltime + "') and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = " + TableName + ".vcPartsno   and dTimeFrom<= '" + lbltime + "-01" + "' and dTimeTo >= '" + lbltime + "-01" + "' ) ";
                cmd.ExecuteNonQuery();
                for (int i = 0; i < partsInfo.Rows.Count; i++)
                {
                    string vcPartsno = partsInfo.Rows[i]["partsno"].ToString();
                    string vcDock = partsInfo.Rows[i]["vcDock"].ToString();
                    string vcCarType = partsInfo.Rows[i]["vcCarFamilyCode"].ToString();
                    string vcProjectName = partsInfo.Rows[i]["vcProName1"].ToString();
                    string vcProject1 = partsInfo.Rows[i]["vcCalendar1"].ToString();
                    //string total = (Convert.ToInt32(partsInfo.Rows[i]["num"])/(Convert.ToInt32(partsInfo.Rows[i]["srs"]))).ToString();
                    string total = partsInfo.Rows[i]["num"].ToString();
                    //20180929在从SOQReply数据生成包装计划时就已经把5个工程的计划都生成一遍了，所以要在这里，生成看板打印数据时，把周度品番筛走 - 李兴旺
                    //20180929查看该品番的品番频度 - 李兴旺
                    string vcPartFrequence = "";
                    string sqlPartFrequence = "SELECT vcPartsNo, vcPartFrequence FROM SPPSBS.dbo.tPartInfoMaster where vcPartsNo = '" + vcPartsno + "' and vcDock = '" + vcDock + "' and vcCarFamilyCode = '" + vcCarType + "' and dTimeFrom<='" + lbltime + "-01' and dTimeTo>='" + lbltime + "-01' ";
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
                    if (!(TableName == "MonthKanBanPlanTblTMP" && vcPartFrequence == "周度"))
                    {
                        #region 插入、更新数据操作
                        foreach (DataRow dr in dt[i].Rows)
                        {
                            string tmpY = dr["vcYear"].ToString();
                            string tmpM = dr["vcMonth"].ToString().Length == 1 ? "0" + dr["vcMonth"].ToString() : dr["vcMonth"].ToString();
                            string vcMonth = tmpY + "-" + tmpM;
                            cmd.CommandText = "select top(1) * from " + TableName + " where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                            DataTable tmp = new DataTable();
                            apt.Fill(tmp);
                            if (tmp.Rows.Count > 0)
                            {
                                string upsql = "vc" + dr["days"].ToString() + "='" + dr["total"].ToString() + "'";
                                if (lbltime != vcMonth)
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + " , DUPDTIME=getdate(),CUPDUSER='" + user + "' ,montouch ='" + lbltime + "' where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                                }
                                else
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + " , DUPDTIME=getdate(),CUPDUSER='" + user + "'  where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                                }
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd.CommandText = " INSERT INTO " + TableName + " ([vcMonth],[vcPartsno],[vcDock],[vcCarType],[vcProject1],[vcProjectName] ,[DADDTIME],[CUPDUSER])";
                                cmd.CommandText += " values( '" + vcMonth + "' ,'" + vcPartsno + "','" + vcDock + "','" + vcCarType + "','" + vcProject1 + "','" + vcProjectName + "',getdate(),'" + user + "')  ";
                                cmd.ExecuteNonQuery();
                                string upsql = "vc" + dr["days"].ToString() + "='" + dr["total"].ToString() + "'";
                                if (lbltime != vcMonth)
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + " , DUPDTIME=getdate(),CUPDUSER='" + user + "' ,montouch ='" + lbltime + "'  where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                                }
                                else
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + " , DUPDTIME=getdate(),CUPDUSER='" + user + "' where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                                }
                                cmd.ExecuteNonQuery();
                            }
                        }
                        cmd.CommandText = "update " + TableName + " set vcMonTotal='" + total + "'  where vcMonth='" + lbltime + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
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