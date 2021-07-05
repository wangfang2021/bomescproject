using System;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;

namespace DataAccess
{
    public class P00001_DataAccess
    {
        private MultiExcute excute = new MultiExcute();


        public DataTable GetPrint(string iP)
        {
            StringBuilder GetPrintSql = new StringBuilder();
            GetPrintSql.Append("select vcUserFlag from TPrint where vcKind in ('LABEL PRINTER','LASEL PRINTER','DOT PRINTER') and vcPrinterIp='" + iP + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPrintSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetUserRole(string opearteId)
        {
            StringBuilder GetUserRoleSql = new StringBuilder();
            GetUserRoleSql.Append("select vcInPut, vcCheck, vcPack, vcOutPut from TPointPower where vcUserId = '" + opearteId + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetUserRoleSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable ValidateOpr1(string inno)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("select vcPart_id,vcSR,vcKBOrderNo,vcKBLFNo,iQuantity,vcSupplier_id from TOperateSJ where vcInputNo='" + inno + "' and vcZYType='S0'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = ValidateOprSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable ValidateOpr2(string inno)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("select vcPart_id,vcSR,vcKBOrderNo,vcKBLFNo,iQuantity,vcSupplier_id from TOperateSJ where vcInputNo='" + inno + "' and vcZYType='S1'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = ValidateOprSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetPackQuantity(string inno)
        {
            StringBuilder GetPackQuantitySql = new StringBuilder();
            GetPackQuantitySql.Append("select iQuantity from TBoxMaster where vcInstructionNo='" + inno + "' and  vcDelete='0' ");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPackQuantitySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetInputQuantity(string inno)
        {
            StringBuilder GetInputQuantitySql = new StringBuilder();
            GetInputQuantitySql.Append("select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR,iQuantity from TOperateSJ where vcZYType='S0' and vcInputNo='" + inno + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetInputQuantitySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetKanBanSum(string iP)
        {
            StringBuilder GetKanBanSumSql = new StringBuilder();
            GetKanBanSumSql.Append("select count(*) as sum from TOperatorQB where vcHostIp='" + iP + "' and vcReflectFlag='0' and vcZYType='S0'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetKanBanSumSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetInfoData(string iP)
        {
            StringBuilder GetInfoDataSql = new StringBuilder();

            GetInfoDataSql.Append("select top(1) vcPart_id,vcKBOrderNo,vcKBLFNo,iQuantity,vcSR,vcTrolleyNo,vcTrolleySeqNo from TOperatorQB where vcHostIp='" + iP + "' order by iAutoId desc\n");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetInfoDataSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetSeqNo(string tmpString, string formatServerTime)
        {
            StringBuilder GetSeqNo = new StringBuilder();
            GetSeqNo.Append("select SEQNO from TSeqNo where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetSeqNo.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }


        public void InsertTrolley1(string seqNo, string trolley, string iP, string opearteId, string serverTime)
        {
            StringBuilder InsertTrolleySql = new StringBuilder();
            InsertTrolleySql.Append("INSERT INTO TOperateSJ_TrolleyInfo(vcSheBeiNo,vcHostIp,vcTrolleyNo,vcStatus\n");
            InsertTrolleySql.Append(",vcOperatorID,dOperatorTime,vcLotid,vcTrolleySeqNo)VALUES('','" + iP + "','" + trolley + "','0','" + opearteId + "','" + serverTime + "','','" + seqNo + "')\n");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = InsertTrolleySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateSeqNo(string tmpString, string formatServerTime, int seqNoNew)
        {
            StringBuilder UpdateSeqNoSql = new StringBuilder();
            UpdateSeqNoSql.Append("update TSeqNo set SEQNO='" + seqNoNew + "' where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateSeqNoSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void InsertSeqNo(string tmpString, string formatServerTime)
        {
            StringBuilder InsertSeqNoSql = new StringBuilder();
            InsertSeqNoSql.Append("INSERT INTO TSeqNo(FLAG ,DDATE ,SEQNO) \n");
            InsertSeqNoSql.Append("VALUES( '" + tmpString + "','" + formatServerTime + "','1')");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = InsertSeqNoSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }





        public DataTable GetTrolley(string iP)
        {
            StringBuilder GetTrolleySql = new StringBuilder();

            GetTrolleySql.Append("select top(1)vcTrolleyNo, vcStatus,vcTrolleySeqNo from TOperateSJ_TrolleyInfo\n");
            GetTrolleySql.Append("where vcHostIp='" + iP + "' order by iAutoId desc\n");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetTrolleySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetKanBan(string iP, string trolley, string trolleySeqNo)
        {
            StringBuilder GetKanBanSql = new StringBuilder();
            GetKanBanSql.Append("select top(1) vcReflectFlag from TOperatorQB where vcHostIp='" + iP + "' and vcTrolleyNo='" + trolley + "' and vcTrolleySeqNo='" + trolleySeqNo + "' order by iAutoId desc");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetKanBanSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }



        public DataTable GetQuantity(string partId, string scanTime, string dock)
        {
            StringBuilder GetQuantitySql = new StringBuilder();
            GetQuantitySql.Append("select vcBZUnit from TPackageMaster where vcSR='" + dock + "' and vcPart_id='" + partId + "' and dTimeFrom<'" + scanTime + "' and dTimeTo>'" + scanTime + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetQuantitySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }



        public DataTable GetDetail(string pointNo)
        {
            StringBuilder GetDetailSql = new StringBuilder();
            GetDetailSql.Append("select  UUID from TPointDetails where vcPointNo='" + pointNo + "' and dDestroyTime is null order by dOperateDate desc");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetDetailSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateDetail(string uuid, string serverTime)
        {
            StringBuilder UpdateDetailSql = new StringBuilder();
            UpdateDetailSql.Append("update TPointDetails set dDestroyTime='" + serverTime + "' where UUID='" + uuid + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateDetailSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }


        public DataTable GetBanZhi(string serverTime)
        {
            StringBuilder GetBanZhiSql = new StringBuilder();
            GetBanZhiSql.Append("select dHosDate,vcBanZhi from (select convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,-1,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,-1,GETDATE())),23) else convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='夜' and vcPackPlant='H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 0, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '白' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 0, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '夜' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 1, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '白' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append(")t where tFromTime<=GETDATE() and tToTime>=GETDATE()\n");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetBanZhiSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetPointType(string pointNo)
        {
            StringBuilder GetPointTypeSql = new StringBuilder();
            GetPointTypeSql.Append("select vcPointType from TPointInfo where vcPointNo='" + pointNo + "' and vcUsed='在用'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPointTypeSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateCase(string iP)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("update TCaseInfo set vcStatus='1' where vcHostIp='" + iP + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateCaseSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void InsertDetail(string date, string banZhi, string pointNo, string uuid, string serverTime, string opearteId)
        {
            StringBuilder InsertDetailSql = new StringBuilder();
            InsertDetailSql.Append("INSERT INTO TPointDetails(vcPlant,dHosDate,vcBanZhi,vcPointNo,UUID,dEntryTime,dDestroyTime,iStopSS,vcOperater,dOperateDate)\n");
            InsertDetailSql.Append("VALUES('H2','" + date + "','" + banZhi + "','" + pointNo + "','" + uuid + "','" + serverTime + "',NULL,0,'" + opearteId + "','" + serverTime + "')\n");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = InsertDetailSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateStatus4(string pointNo, string opearteId)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='正常',vcOperater='" + opearteId + "' where vcPointNo='" + pointNo + "' and vcPlant='H2'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateStatusSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }




        public void UpdateStatus3(string pointNo)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='未用' where vcPointNo='" + pointNo + "' and vcPlant='H2'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateStatusSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }





        public DataTable GetPointStatus1(string opearteId, string iP)
        {
            StringBuilder GetPointStatusSql = new StringBuilder();
            GetPointStatusSql.Append("select t1.vcPointNo from TPointState t1,TPointInfo  t2 where t1.vcPointNo=t2.vcPointNo and t2.vcPointIp='" + iP + "' and t1.vcState='未登录'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPointStatusSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateStatus(string opearteId, string iP, string pointNo)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();


            UpdateStatusSql.Append("update  TPointState set vcState='正常',vcOperater='" + opearteId + "' where vcPointNo='" + pointNo + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateStatusSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }



        public DataTable GetTrolley(string opearteId, string iP)
        {
            StringBuilder GetTrolleySql = new StringBuilder();
            GetTrolleySql.Append(" select vcTrolleyNo,vcLotid from TOperateSJ_TrolleyInfo \n");
            GetTrolleySql.Append("where dOperatorTime = (select Max(dOperatorTime) from TOperateSJ_TrolleyInfo )\n");
            GetTrolleySql.Append("and vcHostIp='" + iP + "' and vcOperatorID='" + opearteId + "' and vcStatus='0' \n");

            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetTrolleySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }



        public DataTable GetCase(string opearteId, string iP)
        {
            StringBuilder GetCaseSql = new StringBuilder();
            GetCaseSql.Append("select vcCaseNo from TCaseInfo where vcHostIp='" + iP + "' and vcOperatorID='" + opearteId + "' order by dOperatorTime desc");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetCaseSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetCase1(string caseNo)
        {
            StringBuilder GetCaseSql = new StringBuilder();
            GetCaseSql.Append("select vcCaseno from TCaseList  where vcCaseno='" + caseNo + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetCaseSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateCase(string iP, string serverTime, string opearteId, string caseNo)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("update TCaseInfo set vcHostIp='" + iP + "',vcPointState='1',dOperatorTime='" + serverTime + "' where vcCaseNo='" + caseNo + "' and vcOperatorID='" + opearteId + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateCaseSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetPoint2(string iP)
        {
            StringBuilder GetPoingSql = new StringBuilder();
            GetPoingSql.Append("select vcPointNo from TPointInfo where vcPointIp='" + iP + "' and vcUsed='在用' and vcPlant='H2'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPoingSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdatePoint1(string pointNo)
        {
            StringBuilder UpdatePointSql = new StringBuilder();
            UpdatePointSql.Append("update TPointState set vcState='未登录' where vcPointNo='" + pointNo + "' and vcPlant='H2'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdatePointSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void InsertPoint1(string pointNo)
        {
            StringBuilder InsertPointSql = new StringBuilder();
            InsertPointSql.Append("INSERT INTO TPointState (vcPlant,vcPointNo,vcState,vcOperater,decEfficacy) VALUES ('H2','" + pointNo + "','未登录','',null)");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = InsertPointSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetPointStatus4(string pointNo)
        {
            StringBuilder GetPointStatusSql = new StringBuilder();
            GetPointStatusSql.Append("select vcState from TPointState where vcPlant='H2' and vcPointNo='" + pointNo + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPointStatusSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetTrolleyInfo(string trolley, string iP, string lotId)
        {
            StringBuilder GetTrolleyInfoSql = new StringBuilder();
            GetTrolleyInfoSql.Append("select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR from TOperatorQB where vcZYType='S0' and vcTrolleySeqNo='" + lotId + "' and vcHostIp='" + iP + "' and vcTrolleyNo='" + trolley + "' and vcReflectFlag='0' order by dOperatorTime desc");

            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetTrolleyInfoSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void DeleteKanban(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string iP)
        {
            StringBuilder DeleteKanbanSql = new StringBuilder();
            DeleteKanbanSql.Append("update TOperatorQB set vcReflectFlag='4'  where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcHostIp='" + iP + "' and vcZYType='S0' and vcReflectFlag='0'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = DeleteKanbanSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetPackingSpot(string iP)
        {
            StringBuilder GetPackingSpotSql = new StringBuilder();
            GetPackingSpotSql.Append("  select distinct(vcBZPlant) from TOperatorQB where vcZYType='S0'  and vcHostIp='" + iP + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPackingSpotSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void DeleteTrolley(string trolley, string iP, string lotId)
        {
            StringBuilder DeleteTrolleySql = new StringBuilder();
            DeleteTrolleySql.Append(" update TOperatorQB set vcReflectFlag='4' where vcHostIp='" + iP + "' and vcTrolleyNo='" + trolley + "' and vcZYType='S0' and vcTrolleySeqNo='" + lotId + "' and vcReflectFlag='0'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = DeleteTrolleySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }



        public DataTable GetSum(string iP)
        {
            StringBuilder GetSumSql = new StringBuilder();
            GetSumSql.Append("select vcTrolleyNo, count(vcTrolleyNo) as kanbanSum, vcTrolleySeqNo  from TOperatorQB   where vcZYType = 'S0' and vcReflectFlag = '0' and vcHostIp = '" + iP + "'  group by vcTrolleyNo, vcTrolleySeqNo  order by vcTrolleySeqNo desc");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetSumSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable ValidateQB(string trolley)
        {
            StringBuilder ValidateQBSql = new StringBuilder();
            ValidateQBSql.Append("select vcPart_id from TOperatorQB where vcTrolleyNo='" + trolley + "' and vcReflectFlag='0'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = ValidateQBSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateQB(string lotId, string iP, string trolley)
        {
            StringBuilder UpdateQBSql = new StringBuilder();
            UpdateQBSql.Append(" update TOperatorQB set vcLotid = '" + lotId + "' where vcTrolleyNo = '" + trolley + "' and vcHostIp = '" + iP + "' and vcLotid = ''");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateQBSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void InsertSeqNo(string packingSpot, string serverTime, string tmpString)
        {
            StringBuilder InsertSeqNoSql = new StringBuilder();
            InsertSeqNoSql.Append("INSERT INTO TSeqNo(FLAG ,DDATE ,SEQNO) \n");
            InsertSeqNoSql.Append("VALUES( '" + tmpString + "'+'" + packingSpot + "','" + serverTime + "','1')");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = InsertSeqNoSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetData1(string trolley1, string trolleySeqNo1, string iP)
        {
            StringBuilder GetDataSql = new StringBuilder();
            GetDataSql.Append("select count(*) as sum from TOperatorQB where vcTrolleyNo = '" + trolley1 + "' and vcTrolleySeqNo = '" + trolleySeqNo1 + "' and vcHostIp = '" + iP + "' and vcReflectFlag  = '0'  ");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetDataSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetData(string trolley1, string trolleySeqNo1, string iP)
        {
            StringBuilder GetDataSql = new StringBuilder();
            GetDataSql.Append("select top(1) vcReflectFlag from TOperatorQB where vcTrolleyNo = '" + trolley1 + "' and vcTrolleySeqNo = '" + trolleySeqNo1 + "' and vcHostIp = '" + iP + "' order by iAutoId desc");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetDataSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetSeqNo2(string iP, string kanbanOrderNo, string kanbanSerial, string dock, string partId)
        {
            StringBuilder GetSeqNoSql = new StringBuilder();
            GetSeqNoSql.Append("select vcTrolleySeqNo, vcTrolleyNo from TOperatorQB where vcPart_id = '" + partId + "' and vcKBOrderNo = '" + kanbanOrderNo + "' and vcKBLFNo = '" + kanbanSerial + "' and vcSR = '" + dock + "' and vcReflectFlag = '4' and vcHostIp = '" + iP + "' order by iAutoId desc");//问题区域
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetSeqNoSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateTrolley3(string trolley, string trolleySeqNo, string iP)
        {
            StringBuilder UpdateTrolleySql = new StringBuilder();
            UpdateTrolleySql.Append("update TOperateSJ_TrolleyInfo set vcStatus='4' where vcTrolleyNo='" + trolley + "' and vcTrolleySeqNo='" + trolleySeqNo + "' and vcHostIp='" + iP + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateTrolleySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetSeqNoSql(string iP, string kanbanOrderNo, string kanbanSerial, string dock, string partId)
        {
            StringBuilder GetSeqNoSql = new StringBuilder();
            GetSeqNoSql.Append("select vcTrolleySeqNo,vcTrolleyNo from TOperatorQB where vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcReflectFlag='4' and vcHostIp='" + iP + "' order by iAutoId desc ");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetSeqNoSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetTrolleyInfo1(string trolley, string iP, string opearteId)
        {
            StringBuilder GetTrolleyInfoSql = new StringBuilder();
            GetTrolleyInfoSql.Append("select * from TOperateSJ_TrolleyInfo where vcStatus='0' and vcTrolleyNo='" + trolley + "' and vcHostIp='" + iP + "' and vcOperatorID='" + opearteId + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetTrolleyInfoSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateTrolley1(string iP, string opearteId, string trolley, string lotId)
        {
            StringBuilder UpdateTrolleySql = new StringBuilder();
            UpdateTrolleySql.Append("update TOperateSJ_TrolleyInfo set vcStatus='4' where vcTrolleySeqNo='" + lotId + "' and vcTrolleyNo='" + trolley + "' and vcStatus='0' and vcOperatorID='" + opearteId + "' and vcHostIp='" + iP + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateTrolleySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateTrolley(string trolley, string opearteId, string serverTime, string iP, string lotId)
        {
            StringBuilder UpdateTrolleySql = new StringBuilder();
            UpdateTrolleySql.Append("update TOperateSJ_TrolleyInfo set vcLotid='" + lotId + "',dOperatorTime='" + serverTime + "',vcHostIp='" + iP + "' where vcTrolleyNo='" + trolley + "' and vcOperatorID='" + opearteId + "' ");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateTrolleySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void InsertTrolley(string trolley, string opearteId, string serverTime, string iP, string lotId)
        {
            StringBuilder InsertTrolleySql = new StringBuilder();
            InsertTrolleySql.Append("INSERT INTO  TOperateSJ_TrolleyInfo  (vcSheBeiNo,vcHostIp,vcTrolleyNo,vcStatus,vcOperatorID,dOperatorTime,vcLotid)\n");
            InsertTrolleySql.Append("     VALUES ('','" + iP + "','" + trolley + "','0','" + opearteId + "','" + serverTime + "','" + lotId + "')\n");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = InsertTrolleySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable ValidateTrolley(string trolley, string opearteId, string iP)
        {
            StringBuilder ValidateTrolleySql = new StringBuilder();
            ValidateTrolleySql.Append("  select vcTrolleyNo from TOperateSJ_TrolleyInfo where vcHostIp='" + iP + "' and vcTrolleyNo='" + trolley + "' and vcStatus='0' and vcOperatorID='" + opearteId + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = ValidateTrolleySql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateSeqNo(string packingSpot, string serverTime, int seqNoNew, string tmpString)
        {
            StringBuilder UpdateSeqNoSql = new StringBuilder();
            UpdateSeqNoSql.Append("UPDATE TSeqNo SET SEQNO = " + seqNoNew + "\n");
            UpdateSeqNoSql.Append("  WHERE FLAG = ('" + tmpString + "'+'" + packingSpot + "') and DDATE='" + serverTime + "'\n");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateSeqNoSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }


        public DataTable ValidateSeqNo(string packingSpot, string serverTime, string tmpString)
        {
            StringBuilder ValidateSeqNoSql = new StringBuilder();
            ValidateSeqNoSql.Append("SELECT  FLAG,DDATE,SEQNO\n");
            ValidateSeqNoSql.Append("  FROM TSeqNo where FLAG='" + tmpString + "'+'" + packingSpot + "' AND DDATE='" + serverTime + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = ValidateSeqNoSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }


        //========================================================================重写========================================================================
        public DataTable checkPrintName(string iP, string strPointType)
        {
            StringBuilder GetPrintSql = new StringBuilder();
            if (strPointType == "PDA" || strPointType == "PAD")
            {
                GetPrintSql.Append("select vcUserFlag from TPrint where vcKind in ('PAC PRINTER','LABEL PRINTER R') and vcPrinterIp='" + iP + "'");
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPrintSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public void setCaseState(string strIP)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("update TCaseInfo set vcPointState='1' where vcHostIp='" + strIP + "' and dBoxPrintTime is null");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable ValidateUser(string opearteId)
        {
            StringBuilder ValidateUserSql = new StringBuilder();
            ValidateUserSql.Append("select vcPointNo from TPointState where vcState='正常' and vcOperater='" + opearteId + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = ValidateUserSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataSet getCheckQBandSJInfo(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string packingSpot, string scanTime, string strType)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (strType == "QB")
            {
                stringBuilder.AppendLine("--0.扫描重复");
                stringBuilder.AppendLine("select vcPart_id from TOperatorQB where vcReflectFlag in ('0','1') and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcPart_id='" + partId + "' and vcSR='" + dock + "' and vcZYType='S0'");
                stringBuilder.AppendLine("--1.检验包装区分");
                stringBuilder.AppendLine("select  vcBZQF, vcRHQF, vcBZUnit from TPackageMaster where vcPart_id = '" + partId + "' and dTimeFrom <= GETDATE() and dTimeTo >=GETDATE() and vcBZQF is not null and vcRHQF is not null and vcBZUnit is not null");
                stringBuilder.AppendLine("--2.检验受入");
                stringBuilder.AppendLine("select vcReceiver, vcPackingPlant from TSPMaster_SufferIn where vcPackingPlant = 'TFTM' and vcPartId = '" + partId + "' and  dFromTime <= GETDATE() and dToTime >= GETDATE() and vcSufferIn = '" + dock + "'");
                stringBuilder.AppendLine("--3.检验收容数");
                stringBuilder.AppendLine("select iPackingQty,vcSupplierId,vcSupplierPlant from TSPMaster_Box where vcPartId='" + partId + "' and dFromTime<=GETDATE() and dToTime>=GETDATE()");
                stringBuilder.AppendLine("--4.检验内外");
                stringBuilder.AppendLine("select vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace from TSPMaster WHERE vcPartId='" + partId + "' and dFromTime<=GETDATE() and dToTime>=GETDATE()");
                stringBuilder.AppendLine("--5.包材");
                stringBuilder.AppendLine("select a.vcPackNo,a.iBiYao,b.vcPackLocation,b.vcDistinguish from ");
                stringBuilder.AppendLine("(select vcPackNo,iBiYao from TPackItem where vcPartsNo='" + partId + "' and dUsedFrom<=GETDATE() and dUsedTo>=GETDATE())a");
                stringBuilder.AppendLine("left join");
                stringBuilder.AppendLine("(select vcPackLocation,vcDistinguish,vcPackNo from TPackBase where vcPackSpot='" + packingSpot + "' and dPackFrom<=GETDATE() and dPackTo>=GETDATE())b");
                stringBuilder.AppendLine("on a.vcPackNo=b.vcPackNo");
                stringBuilder.AppendLine("--6.标签");
                stringBuilder.AppendLine("select vcPartNameCN, vcSCSName, vcSCSAdress, vcZXBZNo from TtagMaster where vcPart_Id = '" + partId + "' and dTimeFrom <= GETDATE() and dTimeTo >= GETDATE()");
                stringBuilder.AppendLine("--7.订单量");
                stringBuilder.AppendLine("select (ISNULL(SUM(CAST(vcPlantQtyDailySum as int)),0)-ISNULL(SUM(CAST(vcInputQtyDailySum as int)),0)) as sum from SP_M_ORD where ISNULL(vcPlantQtyDailySum,0)!=ISNULL(vcInputQtyDailySum,0) and vcPartNo='" + partId + "' and vcOrderNo!=''");
                stringBuilder.AppendLine("--8.扫描总数");
                stringBuilder.AppendLine("select ISNULL(sum(iQuantity), 0) as sum from TOperatorQB where vcZYType = 'S0' and vcPart_id = '" + partId + "' and vcReflectFlag = '0'");
                stringBuilder.AppendLine("--9.价格");
                stringBuilder.AppendLine("select b.decPriceOrigin_CW from");
                stringBuilder.AppendLine("(select vcPartId,vcSupplierId,vcReceiver from TSPMaster where vcPartId='" + partId + "' )a left join");
                stringBuilder.AppendLine("(select vcPart_id,vcSupplier_id,vcReceiver,decPriceOrigin_CW from TPrice where dPricebegin<=GETDATE() and dPriceEnd>=GETDATE()) b");
                stringBuilder.AppendLine("on a.vcPartId=b.vcPart_id and a.vcSupplierId=b.vcSupplier_id and a.vcReceiver=b.vcReceiver");
                stringBuilder.AppendLine("where isnull(b.decPriceOrigin_CW,0) <>0");
                stringBuilder.AppendLine("--10.发注工厂");
                stringBuilder.AppendLine("select a.vcPackingPlant,a.vcPartId,a.vcReceiver,a.vcSupplierId,b.vcSupplierPlant,c.vcOrderPlant from ");
                stringBuilder.AppendLine("(select * from TSPMaster where vcPartId='" + partId + "' and dFromTime<=GETDATE() and dToTime>=GETDATE())a");
                stringBuilder.AppendLine("left join");
                stringBuilder.AppendLine("(SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSupplierPlant ");
                stringBuilder.AppendLine("FROM [TSPMaster_SupplierPlant] ");
                stringBuilder.AppendLine("WHERE cast(dFromTime as datetime)<=GETDATE() AND cast(dToTime as datetime)>=GETDATE())b");
                stringBuilder.AppendLine("ON A.vcPackingPlant=B.vcPackingPlant AND A.vcPartId=B.vcPartId AND A.vcSupplierId=B.vcSupplierId AND A.vcReceiver=B.vcReceiver");
                stringBuilder.AppendLine("left join");
                stringBuilder.AppendLine("(select vcValue1 as [vcSupplierId]");
                stringBuilder.AppendLine("		,vcValue2 as vcSupplierPlant");
                stringBuilder.AppendLine("		,convert(varchar(10),cast(vcValue3 as datetime),23) as [dFromTime]");
                stringBuilder.AppendLine("		,convert(varchar(10),cast(vcValue4 as datetime),23) as [dToTime]");
                stringBuilder.AppendLine("		,vcValue5 as vcOrderPlant ");
                stringBuilder.AppendLine("from TOutCode where vcCodeId='C010' and vcIsColum='0'");
                stringBuilder.AppendLine("and cast(vcValue3 as datetime)<=GETDATE() AND cast(vcValue4 as datetime)>=GETDATE())c");
                stringBuilder.AppendLine("on a.vcSupplierId=c.vcSupplierId and b.vcSupplierPlant=c.vcSupplierPlant");
            }
            if (strType == "SJ")
            {
                stringBuilder.AppendLine("--0.上传重复");
                stringBuilder.AppendLine("select * from TOperateSJ where vcSR='" + dock + "' and vcPart_id = '" + partId + "' and vcKBOrderNo = '" + kanbanOrderNo + "' and vcKBLFNo = '" + kanbanSerial + "'");
                stringBuilder.AppendLine("--1.检验包装区分");
                stringBuilder.AppendLine("select  vcBZQF, vcRHQF, vcBZUnit from TPackageMaster where vcPart_id = '" + partId + "' and dTimeFrom <= GETDATE() and dTimeTo >= GETDATE() and vcBZQF is not null and vcRHQF is not null and vcBZUnit is not null");
                stringBuilder.AppendLine("--2.检验受入");
                stringBuilder.AppendLine("select vcReceiver, vcPackingPlant from TSPMaster_SufferIn where vcPackingPlant = 'TFTM' and vcPartId = '" + partId + "' and  dFromTime <= GETDATE() and dToTime >= GETDATE() and vcSufferIn = '" + dock + "'");
                stringBuilder.AppendLine("--3.检验收容数");
                stringBuilder.AppendLine("select iPackingQty,vcSupplierId,vcSupplierPlant from TSPMaster_Box where vcPartId='" + partId + "' and dFromTime<=GETDATE() and dToTime>=GETDATE()");
                stringBuilder.AppendLine("--4.检验内外");
                stringBuilder.AppendLine("select vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace	 from TSPMaster WHERE vcPartId='" + partId + "' and dFromTime<=GETDATE() and dToTime>=GETDATE()");
                stringBuilder.AppendLine("--5.包材");
                stringBuilder.AppendLine("select vcPackNo,iBiYao from TPackItem where vcPartsNo='" + partId + "' and dUsedFrom<=GETDATE() and dUsedTo>=GETDATE()");
                stringBuilder.AppendLine("--6.标签");
                stringBuilder.AppendLine("select vcPartNameCN, vcSCSName, vcSCSAdress, vcZXBZNo from TtagMaster where vcPart_Id = '" + partId + "' and dTimeFrom <= GETDATE() and dTimeTo >= GETDATE()");
                stringBuilder.AppendLine("--7.订单量");
                stringBuilder.AppendLine("select (ISNULL(SUM(CAST(vcPlantQtyDailySum as int)),0)-ISNULL(SUM(CAST(vcInputQtyDailySum as int)),0)) as sum from SP_M_ORD where ISNULL(vcPlantQtyDailySum,0)!=ISNULL(vcInputQtyDailySum,0) and vcPartNo='" + partId + "' and vcOrderNo!=''");
                stringBuilder.AppendLine("--8.扫描总数");
                stringBuilder.AppendLine("select ISNULL(sum(iQuantity), 0) as sum from TOperatorQB where vcZYType = 'S0' and vcPart_id = '" + partId + "' and vcReflectFlag = '0'");
                stringBuilder.AppendLine("--9.价格");
                stringBuilder.AppendLine("select b.decPriceOrigin_CW from");
                stringBuilder.AppendLine("(select vcPartId,vcSupplierId,vcReceiver from TSPMaster where vcPartId='" + partId + "' )a left join");
                stringBuilder.AppendLine("(select vcPart_id,vcSupplier_id,vcReceiver,decPriceOrigin_CW from TPrice where dPricebegin<=GETDATE() and dPriceEnd>=GETDATE()) b");
                stringBuilder.AppendLine("on a.vcPartId=b.vcPart_id and a.vcSupplierId=b.vcSupplier_id and a.vcReceiver=b.vcReceiver");
                stringBuilder.AppendLine("where isnull(b.decPriceOrigin_CW,0) <>0");
                stringBuilder.AppendLine("--10.发注工厂");
                stringBuilder.AppendLine("select a.vcPackingPlant,a.vcPartId,a.vcReceiver,a.vcSupplierId,b.vcSupplierPlant,c.vcOrderPlant from ");
                stringBuilder.AppendLine("(select * from TSPMaster where vcPartId='" + partId + "' and dFromTime<=GETDATE() and dToTime>=GETDATE())a");
                stringBuilder.AppendLine("left join");
                stringBuilder.AppendLine("(SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSupplierPlant ");
                stringBuilder.AppendLine("FROM [TSPMaster_SupplierPlant] ");
                stringBuilder.AppendLine("WHERE cast(dFromTime as datetime)<=GETDATE() AND cast(dToTime as datetime)>=GETDATE())b");
                stringBuilder.AppendLine("ON A.vcPackingPlant=B.vcPackingPlant AND A.vcPartId=B.vcPartId AND A.vcSupplierId=B.vcSupplierId AND A.vcReceiver=B.vcReceiver");
                stringBuilder.AppendLine("left join");
                stringBuilder.AppendLine("(select vcValue1 as [vcSupplierId]");
                stringBuilder.AppendLine("		,vcValue2 as vcSupplierPlant");
                stringBuilder.AppendLine("		,convert(varchar(10),cast(vcValue3 as datetime),23) as [dFromTime]");
                stringBuilder.AppendLine("		,convert(varchar(10),cast(vcValue4 as datetime),23) as [dToTime]");
                stringBuilder.AppendLine("		,vcValue5 as vcOrderPlant ");
                stringBuilder.AppendLine("from TOutCode where vcCodeId='C010' and vcIsColum='0'");
                stringBuilder.AppendLine("and cast(vcValue3 as datetime)<=GETDATE() AND cast(vcValue4 as datetime)>=GETDATE())c");
                stringBuilder.AppendLine("on a.vcSupplierId=c.vcSupplierId and b.vcSupplierPlant=c.vcSupplierPlant");
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public void Insert(string trolley, string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string scanTime, String iP, string serverTime, string cpdCompany, string inno, string opearteId, string packingSpot, string packQuantity, string lblSart, string lblEnd, string supplierId, string supplierPlant, string trolleySeqNo, string inoutFlag, string kanBan, string orderplant)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("INSERT INTO TOperatorQB(vcZYType,dScanTime,vcHtNo,vcTrolleyNo ,vcInputNo,vcPart_id,vcCpdCompany,vcSR ,vcBoxNo,iQuantity  \n");
            stringBuilder.Append(" ,vcSeqNo,vcReflectFlag,dStart,dEnd,vcHostIp,vcKBOrderNo,vcKBLFNo,vcOperatorID ,dOperatorTime,vcBZPlant,iPackingQty,vcLabelStart,vcLabelEnd,vcSupplierId,vcSupplierPlant,vcLotid,vcIOType,vcCheckType,vcKanBan,vcTrolleySeqNo,vcPackingPlant)  \n");
            stringBuilder.Append("    VALUES('S0', '" + scanTime + "', '', '" + trolley + "', '" + inno + "', '" + partId + "', '" + cpdCompany + "', '" + dock + "', '', " + int.Parse(quantity) + ", '', '0', null, null, '" + iP + "', '" + kanbanOrderNo + "', '" + kanbanSerial + "', '" + opearteId + "', '" + serverTime + "','" + packingSpot + "'," + packQuantity + ",'" + lblSart + "','" + lblEnd + "','" + supplierId + "','" + supplierPlant + "','','" + inoutFlag + "','','" + kanBan + "','" + trolleySeqNo + "','" + orderplant + "')");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable GetPointNo(string iP)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select vcPointType,vcPointNo from TPointInfo where vcPointIp='" + iP + "' and vcUsed='在用'");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable GetPointState(string strOperater)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select b.vcPointType,b.vcPointNo,b.vcPointIp from ");
            stringBuilder.AppendLine("(select * from TPointState where vcOperater='" + strOperater + "' and vcState='正常')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPointInfo)b");
            stringBuilder.AppendLine("on a.vcPlant=b.vcPlant and a.vcPointNo=b.vcPointNo");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataSet getInputInfoFromDB(string strIP, string serverTime)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("--0本次全部需要上传的数据");
            stringBuilder.AppendLine("select t1.*,isnull(t2.vcBZUnit,0) as vcBZUnit,vcPartENName,t2.vcBZQF from ");
            stringBuilder.AppendLine("(select * from TOperatorQB where vcReflectFlag='0' and vcZYType='S0' and vcHostIp='" + strIP + "')t1");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE() )t2");
            stringBuilder.AppendLine("on t1.vcPart_id=t2.vcPart_id  ");
            stringBuilder.AppendLine("left join ");
            stringBuilder.AppendLine("(select vcPackingPlant,vcPartId,vcSupplierId,vcReceiver,vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace from TSPMaster WHERE dFromTime<=GETDATE() and dToTime>=GETDATE())b ");
            stringBuilder.AppendLine("on t1.vcPart_id = b.vcPartId and t1.vcCpdCompany=b.vcReceiver and t1.vcSupplierId=b.vcSupplierId  ");
            stringBuilder.AppendLine("order by t1.dScanTime ");
            stringBuilder.AppendLine("--1查询台车连番");
            stringBuilder.AppendLine("select distinct vcTrolleySeqNo from TOperatorQB ");
            stringBuilder.AppendLine("where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0' order by vcTrolleySeqNo");
            stringBuilder.AppendLine("--2查询标签总个数");
            stringBuilder.AppendLine("select isnull(sum(cast(t1.iQuantity as int)/cast(isnull(t2.vcBZUnit,0 )as int)),0) as iTagNum from ");
            stringBuilder.AppendLine("(select * from TOperatorQB where vcReflectFlag='0' and vcZYType='S0' and vcHostIp='" + strIP + "')t1");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE() )t2");
            stringBuilder.AppendLine("on t1.vcPart_id=t2.vcPart_id ");
            stringBuilder.AppendLine("where t2.vcBZQF<>'1'");
            stringBuilder.AppendLine("--3查询断取指示书表结构");
            stringBuilder.AppendLine("SELECT TOP (1)*  FROM [TPackList]");
            stringBuilder.AppendLine("--4查询标签表结构");
            stringBuilder.AppendLine("SELECT TOP (1)*  FROM [TLabelList]");
            stringBuilder.AppendLine("--5查询订单表结构");
            stringBuilder.AppendLine("SELECT top(1)* FROM [dbo].[SP_M_ORD]");
            stringBuilder.AppendLine("--6查询档次入库品番合计数量");
            stringBuilder.AppendLine("select vcPart_id,vcCpdCompany,vcSR,sum(iQuantity) as iSumQuantity from TOperatorQB");
            stringBuilder.AppendLine("where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0'");
            stringBuilder.AppendLine("group by vcPart_id,vcCpdCompany,vcSR");
            stringBuilder.AppendLine("--7查询入库指令书结构");
            stringBuilder.AppendLine("SELECT top(1)* FROM [dbo].[TInvList]");
            stringBuilder.AppendLine("--8查询本次上传一场构内数据");
            stringBuilder.AppendLine("select vcKanBan from TOperatorQB where vcReflectFlag='0' and  vcHostIp='" + strIP + "' and vcPackingPlant='1' and vcIOType='1' and vcKanBan!=''");
            stringBuilder.AppendLine("--9查询本次上传二场构内数据");
            stringBuilder.AppendLine("select vcKanBan from TOperatorQB where vcReflectFlag='0' and  vcHostIp='" + strIP + "' and vcPackingPlant='2' and vcIOType='1' and vcKanBan!=''");
            stringBuilder.AppendLine("--10查询本次上传三场构内数据");
            stringBuilder.AppendLine("select vcKanBan from TOperatorQB where vcReflectFlag='0' and  vcHostIp='" + strIP + "' and vcPackingPlant='3' and vcIOType='1' and vcKanBan!=''");




            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable getPackInfo(string strIP, string serverTime)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select c.vcPackLocation");
            stringBuilder.AppendLine("		,c.vcDistinguish as vcPackinggroup");
            stringBuilder.AppendLine("		,b.vcDistinguish as vcDistinguish");
            stringBuilder.AppendLine("		,b.vcPackNo as vcPackNo");
            stringBuilder.AppendLine("		,cast(cast(b.iBiYao as decimal(16,5))*cast(a.iQuantity as int)/cast(d.vcBZUnit as decimal(16,5)) as decimal(16,5)) as dQty");
            stringBuilder.AppendLine("		,c.vcPackLocation as vcPackingpartslocation");
            stringBuilder.AppendLine("		,GETDATE() as dDaddtime");
            stringBuilder.AppendLine("		,'' as vcPcname");
            stringBuilder.AppendLine("		,a.vcBZPlant");
            stringBuilder.AppendLine("		,a.vcPart_id ");
            stringBuilder.AppendLine("		,a.vcCpdCompany ");
            stringBuilder.AppendLine("		,a.vcSR ");
            stringBuilder.AppendLine("		,a.vcLotid");
            stringBuilder.AppendLine("		,a.vcInputNo as [vcInno]");
            stringBuilder.AppendLine("		,a.vcTrolleyNo as vcTrolleyNo");
            stringBuilder.AppendLine("		,e.vcLabelStart  as vcLabelStart");
            stringBuilder.AppendLine("		,e.vcLabelEnd as vcLabelEnd");
            stringBuilder.AppendLine("		from ");
            stringBuilder.AppendLine("(select * from TOperatorQB  where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPackNo,iBiYao,vcPartsNo,vcDistinguish  from TPackItem where dUsedFrom<=GETDATE() and dUsedTo>=GETDATE())b");
            stringBuilder.AppendLine("on a.vcPart_id = b.vcPartsNo");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPackLocation,vcDistinguish,vcPackSpot,vcPackNo from TPackBase where dPackFrom<=GETDATE() and dPackTo>=GETDATE())c");
            stringBuilder.AppendLine("on b.vcPackNo=c.vcPackNo and a.vcBZPlant=c.vcPackSpot");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE())d");
            stringBuilder.AppendLine("on a.vcPart_id=d.vcPart_id");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(SELECT vcLotid,MIN(vcLabelStart) as vcLabelStart,MAX(vcLabelEnd) as vcLabelEnd FROM ");
            stringBuilder.AppendLine("(select * from TOperatorQB  where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0')t1");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE())t2");
            stringBuilder.AppendLine("on t1.vcPart_id=t2.vcPart_id");
            stringBuilder.AppendLine("where t2.vcBZQF<>'1'");
            stringBuilder.AppendLine("group by vcLotid)e");
            stringBuilder.AppendLine("on a.vcLotid=e.vcLotid");
            stringBuilder.AppendLine("order by a.dScanTime");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable getLabelInfo(string strIP, string serverTime)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select b.vcPartENName");
            stringBuilder.AppendLine("		,a.vcPart_id");
            stringBuilder.AppendLine("		,substring(a.vcPart_id,1,5)+'-'+SUBSTRING(a.vcPart_id,6,5)+'-'+SUBSTRING(a.vcPart_id,11,2) as [vcPart_id1]");
            stringBuilder.AppendLine("		,a.vcInputNo");
            stringBuilder.AppendLine("		,a.vcCpdCompany");
            stringBuilder.AppendLine("		,cast(c.vcBZUnit as int) as [vcGetnum]");
            //stringBuilder.AppendLine("		,cast(a.iQuantity as int)/cast(c.vcBZUnit as int) as [vcGetnum]");
            stringBuilder.AppendLine("		,0 as [iDateprintflg]");
            stringBuilder.AppendLine("		,'' as [vcComputernm]");
            stringBuilder.AppendLine("		,'' as [vcPrindate]");
            stringBuilder.AppendLine("		,d.vcPartNameCN as [vcPartnamechineese]");
            stringBuilder.AppendLine("		,d.vcSCSName as [vcSuppliername]");
            stringBuilder.AppendLine("		,d.vcSCSAdress as [vcSupplieraddress]");
            stringBuilder.AppendLine("		,d.vcZXBZNo as [vcExecutestandard]");
            stringBuilder.AppendLine("		,d.vcCarTypeName as [vcCartype]");
            //stringBuilder.AppendLine("		,b.vcCarfamilyCode as [vcCartype]");
            stringBuilder.AppendLine("		,c.vcBZQF");
            stringBuilder.AppendLine("		,a.vcLabelStart");
            stringBuilder.AppendLine("		,a.vcLabelEnd");
            stringBuilder.AppendLine("		from ");
            stringBuilder.AppendLine("(select * from TOperatorQB  where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPackingPlant,vcPartId,vcSupplierId,vcReceiver,vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace from TSPMaster WHERE dFromTime<=GETDATE() and dToTime>=GETDATE())b");
            stringBuilder.AppendLine("on a.vcPart_id = b.vcPartId and a.vcCpdCompany=b.vcReceiver and a.vcSupplierId=b.vcSupplierId ");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE())c");
            stringBuilder.AppendLine("on a.vcPart_id=c.vcPart_id");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPart_Id,vcCPDCompany,vcSupplier_id,vcPartNameCN,vcCarTypeName,vcSCSName,vcSCSAdress,vcZXBZNo from  TtagMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE())d");
            stringBuilder.AppendLine("on a.vcPart_id=d.vcPart_id and a.vcCpdCompany=d.vcCPDCompany and a.vcSupplierId=d.vcSupplier_id");
            stringBuilder.AppendLine("order by a.dScanTime");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable getOrderInfo(string strIP, string serverTime)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select a.iSumQuantity,b.* from ");
            stringBuilder.AppendLine("(select vcPart_id,vcCpdCompany,vcSR,sum(iQuantity) as iSumQuantity from TOperatorQB  ");
            stringBuilder.AppendLine("where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0'");
            stringBuilder.AppendLine("group by vcPart_id,vcCpdCompany,vcSR)a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcTargetYearMonth,vcOrderType,vcOrderNo,vcSeqno,vcPartNo,vcDock,vcCpdcompany,iAutoId,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily1,0) as int)-CAST(ISNULL(vcInputQtyDaily1,0) as int)) as day1 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily2,0) as int)-CAST(ISNULL(vcInputQtyDaily2,0) as int)) as day2 ,(CAST(ISNULL(vcPlantQtyDaily3,0) as int)-CAST(ISNULL(vcInputQtyDaily3,0) as int)) as day3 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily4,0) as int)-CAST(ISNULL(vcInputQtyDaily4,0) as int)) as day4 ,(CAST(ISNULL(vcPlantQtyDaily5,0) as int)-CAST(ISNULL(vcInputQtyDaily5,0) as int)) as day5 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily6,0) as int)-CAST(ISNULL(vcInputQtyDaily6,0) as int)) as day6 ,(CAST(ISNULL(vcPlantQtyDaily7,0) as int)-CAST(ISNULL(vcInputQtyDaily7,0) as int)) as day7 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily8,0) as int)-CAST(ISNULL(vcInputQtyDaily8,0) as int)) as day8 ,(CAST(ISNULL(vcPlantQtyDaily9,0) as int)-CAST(ISNULL(vcInputQtyDaily9,0) as int)) as day9 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily10,0) as int)-CAST(ISNULL(vcInputQtyDaily10,0) as int)) as day10 ,(CAST(ISNULL(vcPlantQtyDaily11,0) as int)-CAST(ISNULL(vcInputQtyDaily11,0) as int)) as day11 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily12,0) as int)-CAST(ISNULL(vcInputQtyDaily12,0) as int)) as day12 ,(CAST(ISNULL(vcPlantQtyDaily13,0) as int)-CAST(ISNULL(vcInputQtyDaily13,0) as int)) as day13 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily14,0) as int)-CAST(ISNULL(vcInputQtyDaily14,0) as int)) as day14 ,(CAST(ISNULL(vcPlantQtyDaily15,0) as int)-CAST(ISNULL(vcInputQtyDaily15,0) as int)) as day15 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily16,0) as int)-CAST(ISNULL(vcInputQtyDaily16,0) as int)) as day16 ,(CAST(ISNULL(vcPlantQtyDaily17,0) as int)-CAST(ISNULL(vcInputQtyDaily17,0) as int)) as day17 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily18,0) as int)-CAST(ISNULL(vcInputQtyDaily18,0) as int)) as day18 ,(CAST(ISNULL(vcPlantQtyDaily19,0) as int)-CAST(ISNULL(vcInputQtyDaily19,0) as int)) as day19 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily20,0) as int)-CAST(ISNULL(vcInputQtyDaily20,0) as int)) as day20 ,(CAST(ISNULL(vcPlantQtyDaily21,0) as int)-CAST(ISNULL(vcInputQtyDaily21,0) as int)) as day21 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily22,0) as int)-CAST(ISNULL(vcInputQtyDaily22,0) as int)) as day22 ,(CAST(ISNULL(vcPlantQtyDaily23,0) as int)-CAST(ISNULL(vcInputQtyDaily23,0) as int)) as day23 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily24,0) as int)-CAST(ISNULL(vcInputQtyDaily24,0) as int)) as day24 ,(CAST(ISNULL(vcPlantQtyDaily25,0) as int)-CAST(ISNULL(vcInputQtyDaily25,0) as int)) as day25 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily26,0) as int)-CAST(ISNULL(vcInputQtyDaily26,0) as int)) as day26 ,(CAST(ISNULL(vcPlantQtyDaily27,0) as int)-CAST(ISNULL(vcInputQtyDaily27,0) as int)) as day27 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily28,0) as int)-CAST(ISNULL(vcInputQtyDaily28,0) as int)) as day28 ,(CAST(ISNULL(vcPlantQtyDaily29,0) as int)-CAST(ISNULL(vcInputQtyDaily29,0) as int)) as day29 ,   ");
            stringBuilder.AppendLine("(CAST(ISNULL(vcPlantQtyDaily30,0) as int)-CAST(ISNULL(vcInputQtyDaily30,0) as int)) as day30 ,(CAST(ISNULL(vcPlantQtyDaily31,0) as int)-CAST(ISNULL(vcInputQtyDaily31,0) as int)) as day31    ");
            stringBuilder.AppendLine("from  SP_M_ORD where vcPlantQtyDailySum>vcInputQtyDailySum)b");
            stringBuilder.AppendLine("on a.vcCpdCompany=b.vcCpdcompany and a.vcSR=b.vcDock and a.vcPart_id=b.vcPartNo");
            stringBuilder.AppendLine("order by b.vcTargetYearMonth");

            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable getInvInfo(string strIP, string serverTime)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select t1.*,isnull(t2.vcBZUnit,0) as vcBZUnit,vcPartENName,t2.vcBZQF from ");
            stringBuilder.AppendLine("(select * from TOperatorQB where vcReflectFlag='0' and vcZYType='S0' and vcHostIp='" + strIP + "')t1");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE())t2");
            stringBuilder.AppendLine("on t1.vcPart_id=t2.vcPart_id  ");
            stringBuilder.AppendLine("left join ");
            stringBuilder.AppendLine("(select vcPackingPlant,vcPartId,vcSupplierId,vcReceiver,vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace from TSPMaster WHERE dFromTime<=GETDATE() and dToTime>=GETDATE())b ");
            stringBuilder.AppendLine("on t1.vcPart_id = b.vcPartId and t1.vcCpdCompany=b.vcReceiver and t1.vcSupplierId=b.vcSupplierId  ");
            stringBuilder.AppendLine("order by t1.dScanTime ");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable setSeqNo(string tmpString, int iAddNum, string formatServerTime)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("declare @flag int");
            stringBuilder.AppendLine("set @flag=(select count(*) from TSeqNo where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "')");
            stringBuilder.AppendLine("if(@flag=0)");
            stringBuilder.AppendLine("begin");
            stringBuilder.AppendLine("INSERT INTO TSeqNo(FLAG ,DDATE ,SEQNO)VALUES( '" + tmpString + "','" + formatServerTime + "'," + iAddNum + ")");
            stringBuilder.AppendLine("select 1 as vcSeqNo");
            stringBuilder.AppendLine("end");
            stringBuilder.AppendLine("else");
            stringBuilder.AppendLine("begin");
            stringBuilder.AppendLine("select SEQNO as vcSeqNo  from TSeqNo   where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
            stringBuilder.AppendLine("update TSeqNo set SEQNO=SEQNO+" + iAddNum + "  where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
            stringBuilder.AppendLine("end");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }

        }
        public void setLotToOperatorQB(DataTable dtInfo, string strIP, string formatServerTime, string strLotSeqNo_begin)
        {

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < dtInfo.Rows.Count; i++)
            {
                string strLotId = formatServerTime.Substring(2, 6).Trim() + "-" + "2" + "-" + (int.Parse(strLotSeqNo_begin) + i).ToString().PadLeft(5, '0').Trim();
                string trolleySeqNo = dtInfo.Rows[i]["vcTrolleySeqNo"].ToString();
                stringBuilder.AppendLine("update TOperatorQB set vcLotid='" + strLotId + "' where vcTrolleySeqNo='" + trolleySeqNo + "' and vcHostIp='" + strIP + "' and vcReflectFlag='0'");
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void setInvToOperatorQB(DataTable dataTable, string iP, string serverTime, string invSeqNo)
        {

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string partId = dataTable.Rows[i]["vcPart_id"].ToString();
                string kanbanOrderNo = dataTable.Rows[i]["vcKBOrderNo"].ToString();
                string kanbanSerial = dataTable.Rows[i]["vcKBLFNo"].ToString();
                string dock = dataTable.Rows[i]["vcSR"].ToString();
                string quantity = dataTable.Rows[i]["iQuantity"].ToString();
                string bzUnit = dataTable.Rows[i]["vcBZUnit"].ToString();
                string inno = serverTime.Replace("-", "").Substring(2, 7).Trim() + "2" + (int.Parse(invSeqNo) + i).ToString().PadLeft(5, '0');

                stringBuilder.AppendLine("update TOperatorQB set vcInputNo='" + inno + "'");
                stringBuilder.AppendLine("where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "'");
                stringBuilder.AppendLine("and vcReflectFlag='0' and vcHostIp='" + iP + "' and vcZYType='S0'");


            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public void setTagToOperatorQB(DataTable dtInfo, string strIP, string serverTime, string strTagSeqNo_begin)
        {

            StringBuilder stringBuilder = new StringBuilder();
            int head = Convert.ToInt32(strTagSeqNo_begin);

            for (int i = 0; i < dtInfo.Rows.Count; i++)
            {
                //计算标签首,标签尾
                string partId = dtInfo.Rows[i]["vcPart_id"].ToString();
                string kanbanOrderNo = dtInfo.Rows[i]["vcKBOrderNo"].ToString();
                string kanbanSerial = dtInfo.Rows[i]["vcKBLFNo"].ToString();
                string dock = dtInfo.Rows[i]["vcSR"].ToString();
                string quantity = dtInfo.Rows[i]["iQuantity"].ToString();
                string bzUnit = dtInfo.Rows[i]["vcBZUnit"].ToString();
                string BZQF = dtInfo.Rows[i]["vcBZQF"].ToString();
                if (BZQF != "1")
                {
                    string lblSart = serverTime.Replace("-", "").Substring(2, 7).Trim() + head.ToString().PadLeft(5, '0').Trim();
                    string lblEnd = serverTime.Replace("-", "").Substring(2, 7).Trim() + (head + (int.Parse(quantity) / int.Parse(bzUnit) - 1)).ToString().PadLeft(5, '0').Trim();
                    stringBuilder.AppendLine("update TOperatorQB set vcLabelStart='" + lblSart + "',vcLabelEnd='" + lblEnd + "' where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");
                    stringBuilder.AppendLine("and vcSR='" + dock + "' and vcHostIp='" + strIP + "' and vcReflectFlag='0' and vcZYType='S0'");
                    head += Convert.ToInt32(quantity) / Convert.ToInt32(bzUnit);
                }
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                if (strSQL != "")
                {
                    SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                    da.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public bool setInputInfo(string strIP, string strPointName, string strPrinterName, DataTable dtPackList_Temp, DataTable dtLabelList_Temp, DataTable dtInv_Temp, DataTable dtOrder_Temp, string strOperId, string strPackPrinterName)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                #region 1.sqlCommand_addinfo
                SqlCommand sqlCommand_addinfo = sqlConnection.CreateCommand();
                sqlCommand_addinfo.Transaction = sqlTransaction;
                sqlCommand_addinfo.CommandType = CommandType.Text;
                StringBuilder strSql_addinfo = new StringBuilder();

                #region SQL and Parameters
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[TOperateSJ]");
                strSql_addinfo.AppendLine("           ([vcZYType]");
                strSql_addinfo.AppendLine("           ,[vcBZPlant]");
                strSql_addinfo.AppendLine("           ,[vcInputNo]");
                strSql_addinfo.AppendLine("           ,[vcKBOrderNo]");
                strSql_addinfo.AppendLine("           ,[vcKBLFNo]");
                strSql_addinfo.AppendLine("           ,[vcPart_id]");
                strSql_addinfo.AppendLine("           ,[vcIOType]");
                strSql_addinfo.AppendLine("           ,[vcSupplier_id]");
                strSql_addinfo.AppendLine("           ,[vcSupplierGQ]");
                strSql_addinfo.AppendLine("           ,[dStart]");
                strSql_addinfo.AppendLine("           ,[dEnd]");
                strSql_addinfo.AppendLine("           ,[iQuantity]");
                strSql_addinfo.AppendLine("           ,[vcBZUnit]");
                strSql_addinfo.AppendLine("           ,[vcSHF]");
                strSql_addinfo.AppendLine("           ,[vcSR]");
                strSql_addinfo.AppendLine("           ,[vcBoxNo]");
                strSql_addinfo.AppendLine("           ,[vcSheBeiNo]");
                strSql_addinfo.AppendLine("           ,[vcCheckType]");
                strSql_addinfo.AppendLine("           ,[iCheckNum]");
                strSql_addinfo.AppendLine("           ,[vcCheckStatus]");
                strSql_addinfo.AppendLine("           ,[vcLabelStart]");
                strSql_addinfo.AppendLine("           ,[vcLabelEnd]");
                strSql_addinfo.AppendLine("           ,[vcUnlocker]");
                strSql_addinfo.AppendLine("           ,[dUnlockTime]");
                strSql_addinfo.AppendLine("           ,[vcSellNo]");
                strSql_addinfo.AppendLine("           ,[vcOperatorID]");
                strSql_addinfo.AppendLine("           ,[dOperatorTime]");
                strSql_addinfo.AppendLine("           ,[vcHostIp]");
                strSql_addinfo.AppendLine("           ,[packingcondition]");
                strSql_addinfo.AppendLine("           ,[vcPackingPlant])");
                strSql_addinfo.AppendLine("select vcZYType,");
                strSql_addinfo.AppendLine("vcBZPlant,");
                strSql_addinfo.AppendLine("vcInputNo,");
                strSql_addinfo.AppendLine("vcKBOrderNo,");
                strSql_addinfo.AppendLine("vcKBLFNo,");
                strSql_addinfo.AppendLine("vcPart_id,");
                strSql_addinfo.AppendLine("vcIOType,");
                strSql_addinfo.AppendLine("vcSupplierId,");
                strSql_addinfo.AppendLine("vcSupplierPlant,");
                strSql_addinfo.AppendLine("dScanTime,");
                strSql_addinfo.AppendLine("getdate(),");
                strSql_addinfo.AppendLine("iQuantity,");
                strSql_addinfo.AppendLine("iPackingQty,");
                strSql_addinfo.AppendLine("vcCpdCompany,");
                strSql_addinfo.AppendLine("vcSR,");
                strSql_addinfo.AppendLine("vcBoxNo,");
                strSql_addinfo.AppendLine("@PointName,");
                strSql_addinfo.AppendLine("null,");
                strSql_addinfo.AppendLine("null,");
                strSql_addinfo.AppendLine("null,");
                strSql_addinfo.AppendLine("vcLabelStart,");
                strSql_addinfo.AppendLine("vcLabelEnd,");
                strSql_addinfo.AppendLine("null,");
                strSql_addinfo.AppendLine("null,");
                strSql_addinfo.AppendLine("null,");
                strSql_addinfo.AppendLine("'" + strOperId + "',");
                strSql_addinfo.AppendLine("getdate(),");
                strSql_addinfo.AppendLine("'" + strIP + "',");
                strSql_addinfo.AppendLine("0,");
                strSql_addinfo.AppendLine("vcPackingPlant from TOperatorQB");
                strSql_addinfo.AppendLine("where vcReflectFlag='0' and vcZYType='S0' and vcHostIp='" + strIP + "'");
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[TOperateSJ_InOutput]");
                strSql_addinfo.AppendLine("           ([vcBZPlant]");
                strSql_addinfo.AppendLine("           ,[vcSHF]");
                strSql_addinfo.AppendLine("           ,[vcSR]");
                strSql_addinfo.AppendLine("           ,[vcInputNo]");
                strSql_addinfo.AppendLine("           ,[vcKBOrderNo]");
                strSql_addinfo.AppendLine("           ,[vcKBLFNo]");
                strSql_addinfo.AppendLine("           ,[vcPart_id]");
                strSql_addinfo.AppendLine("           ,[iQuantity]");
                strSql_addinfo.AppendLine("           ,[iDBZ]");
                strSql_addinfo.AppendLine("           ,[iDZX]");
                strSql_addinfo.AppendLine("           ,[iDCH]");
                strSql_addinfo.AppendLine("           ,[dInDate]");
                strSql_addinfo.AppendLine("           ,[vcOperatorID]");
                strSql_addinfo.AppendLine("           ,[dOperatorTime])");
                strSql_addinfo.AppendLine("select vcBZPlant,");
                strSql_addinfo.AppendLine("vcCpdCompany,");
                strSql_addinfo.AppendLine("vcSR,");
                strSql_addinfo.AppendLine("vcInputNo,");
                strSql_addinfo.AppendLine("vcKBOrderNo,");
                strSql_addinfo.AppendLine("vcKBLFNo,");
                strSql_addinfo.AppendLine("vcPart_id,");
                strSql_addinfo.AppendLine("iQuantity,");
                strSql_addinfo.AppendLine("iQuantity,");
                strSql_addinfo.AppendLine("0,");
                strSql_addinfo.AppendLine("0,");
                strSql_addinfo.AppendLine("dScanTime,");
                strSql_addinfo.AppendLine("'" + strOperId + "',");
                strSql_addinfo.AppendLine("getdate()");
                strSql_addinfo.AppendLine(" from TOperatorQB");
                strSql_addinfo.AppendLine(" where vcReflectFlag='0' and vcZYType='S0' and vcHostIp='" + strIP + "'");
                strSql_addinfo.AppendLine("update TOperatorQB set vcReflectFlag='1' where  vcReflectFlag='0' and vcZYType='S0' and vcHostIp='" + strIP + "'");
                strSql_addinfo.AppendLine("update TOperateSJ_TrolleyInfo set vcStatus='1' where vcOperatorID='" + strOperId + "' and vcHostIp='" + strIP + "'");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                sqlCommand_addinfo.Parameters.AddWithValue("@PointName", strPointName);
                #endregion
                sqlCommand_addinfo.ExecuteNonQuery();
                #endregion

                #region 2.sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TPackList]");
                strSql_modinfo.AppendLine("           ([vcLotid]");
                strSql_modinfo.AppendLine("           ,[iNo]");
                strSql_modinfo.AppendLine("           ,[vcPackingpartsno]");
                strSql_modinfo.AppendLine("           ,[vcPackinggroup]");
                strSql_modinfo.AppendLine("           ,[vcDistinguish]");
                strSql_modinfo.AppendLine("           ,[vcInno]");
                strSql_modinfo.AppendLine("           ,[dQty]");
                strSql_modinfo.AppendLine("           ,[vcPackingpartslocation]");
                strSql_modinfo.AppendLine("           ,[dDaddtime]");
                strSql_modinfo.AppendLine("           ,[vcPcname]");
                strSql_modinfo.AppendLine("           ,[vcHostip]");
                strSql_modinfo.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo.AppendLine("           ,[dOperatorTime]");
                strSql_modinfo.AppendLine("           ,[vcTrolleyNo]");
                strSql_modinfo.AppendLine("           ,[dFirstPrintTime]");
                strSql_modinfo.AppendLine("           ,[dLatelyPrintTime]");
                strSql_modinfo.AppendLine("           ,[vcLabelStart]");
                strSql_modinfo.AppendLine("           ,[vcLabelEnd])");
                strSql_modinfo.AppendLine("     VALUES");
                strSql_modinfo.AppendLine("           (@vcLotid");
                strSql_modinfo.AppendLine("           ,null");
                strSql_modinfo.AppendLine("           ,@vcPackingpartsno");
                strSql_modinfo.AppendLine("           ,@vcPackinggroup");
                strSql_modinfo.AppendLine("           ,@vcDistinguish");
                strSql_modinfo.AppendLine("           ,@vcInno");
                strSql_modinfo.AppendLine("           ,@dQty");
                strSql_modinfo.AppendLine("           ,@vcPackingpartslocation");
                strSql_modinfo.AppendLine("           ,@dDaddtime");
                strSql_modinfo.AppendLine("           ,@vcPcname");
                strSql_modinfo.AppendLine("           ,@vcHostip");
                strSql_modinfo.AppendLine("           ,@vcOperatorID");
                strSql_modinfo.AppendLine("           ,getdate()");
                strSql_modinfo.AppendLine("           ,@vcTrolleyNo");
                strSql_modinfo.AppendLine("           ,null");
                strSql_modinfo.AppendLine("           ,null");
                strSql_modinfo.AppendLine("           ,@vcLabelStart");
                strSql_modinfo.AppendLine("           ,@vcLabelEnd)");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcLotid", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPackingpartsno", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPackinggroup", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcDistinguish", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcInno", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dQty", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPackingpartslocation", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dDaddtime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPcname", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcHostip", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOperatorID", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dOperatorTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcTrolleyNo", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcLabelStart", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcLabelEnd", "");
                #endregion
                foreach (DataRow item in dtPackList_Temp.Rows)
                {
                    #region Value
                    sqlCommand_modinfo.Parameters["@vcLotid"].Value = item["vcLotid"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPackingpartsno"].Value = item["vcPackingpartsno"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPackinggroup"].Value = item["vcPackinggroup"].ToString();
                    sqlCommand_modinfo.Parameters["@vcDistinguish"].Value = item["vcDistinguish"].ToString();
                    sqlCommand_modinfo.Parameters["@vcInno"].Value = item["vcInno"].ToString();
                    sqlCommand_modinfo.Parameters["@dQty"].Value = item["dQty"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPackingpartslocation"].Value = item["vcPackingpartslocation"].ToString();
                    sqlCommand_modinfo.Parameters["@dDaddtime"].Value = item["dDaddtime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPcname"].Value = item["vcPcname"].ToString();
                    sqlCommand_modinfo.Parameters["@vcHostip"].Value = item["vcHostip"].ToString();
                    sqlCommand_modinfo.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    sqlCommand_modinfo.Parameters["@dOperatorTime"].Value = item["dOperatorTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcTrolleyNo"].Value = item["vcTrolleyNo"].ToString();
                    sqlCommand_modinfo.Parameters["@vcLabelStart"].Value = item["vcLabelStart"].ToString();
                    sqlCommand_modinfo.Parameters["@vcLabelEnd"].Value = item["vcLabelEnd"].ToString();
                    #endregion
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                #endregion

                #region 4.sqlCommand_modinfo_sp_add
                SqlCommand sqlCommand_modinfo_sp_add = sqlConnection.CreateCommand();
                sqlCommand_modinfo_sp_add.Transaction = sqlTransaction;
                sqlCommand_modinfo_sp_add.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_sp_add = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo_sp_add.AppendLine("INSERT INTO [dbo].[TLabelList]");
                strSql_modinfo_sp_add.AppendLine("           ([vcPartsnameen]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcPart_id]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcPart_id1]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcInno]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcCpdcompany]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcLabel]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcLabel1]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcGetnum]");
                strSql_modinfo_sp_add.AppendLine("           ,[iDateprintflg]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcComputernm]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcPrindate]");
                strSql_modinfo_sp_add.AppendLine("           ,[iQrcode]");
                strSql_modinfo_sp_add.AppendLine("           ,[iQrcode1]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcPrintcount]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcPrintcount1]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcPartnamechineese]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcSuppliername]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcSupplieraddress]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcExecutestandard]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcCartype]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcHostip]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo_sp_add.AppendLine("           ,[dOperatorTime]");
                strSql_modinfo_sp_add.AppendLine("           ,[dFirstPrintTime]");
                strSql_modinfo_sp_add.AppendLine("           ,[dLatelyPrintTime])");
                strSql_modinfo_sp_add.AppendLine("     VALUES");
                strSql_modinfo_sp_add.AppendLine("           (@vcPartsnameen");
                strSql_modinfo_sp_add.AppendLine("           ,@vcPart_id");
                strSql_modinfo_sp_add.AppendLine("           ,@vcPart_id1");
                strSql_modinfo_sp_add.AppendLine("           ,@vcInno");
                strSql_modinfo_sp_add.AppendLine("           ,@vcCpdcompany");
                strSql_modinfo_sp_add.AppendLine("           ,@vcLabel");
                strSql_modinfo_sp_add.AppendLine("           ,@vcLabel1");
                strSql_modinfo_sp_add.AppendLine("           ,@vcGetnum");
                strSql_modinfo_sp_add.AppendLine("           ,@iDateprintflg");
                strSql_modinfo_sp_add.AppendLine("           ,@vcComputernm");
                strSql_modinfo_sp_add.AppendLine("           ,@vcPrindate");
                strSql_modinfo_sp_add.AppendLine("           ,@iQrcode");
                strSql_modinfo_sp_add.AppendLine("           ,@iQrcode1");
                strSql_modinfo_sp_add.AppendLine("           ,@vcPrintcount");
                strSql_modinfo_sp_add.AppendLine("           ,@vcPrintcount1");
                strSql_modinfo_sp_add.AppendLine("           ,@vcPartnamechineese");
                strSql_modinfo_sp_add.AppendLine("           ,@vcSuppliername");
                strSql_modinfo_sp_add.AppendLine("           ,@vcSupplieraddress");
                strSql_modinfo_sp_add.AppendLine("           ,@vcExecutestandard");
                strSql_modinfo_sp_add.AppendLine("           ,@vcCartype");
                strSql_modinfo_sp_add.AppendLine("           ,@vcHostip");
                strSql_modinfo_sp_add.AppendLine("           ,@vcOperatorID");
                strSql_modinfo_sp_add.AppendLine("           ,GETDATE()");
                strSql_modinfo_sp_add.AppendLine("           ,null");
                strSql_modinfo_sp_add.AppendLine("           ,null)");
                sqlCommand_modinfo_sp_add.CommandText = strSql_modinfo_sp_add.ToString();
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPartsnameen", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPart_id1", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcInno", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcCpdcompany", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcLabel", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcLabel1", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcGetnum", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@iDateprintflg", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcComputernm", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPrindate", "");
                sqlCommand_modinfo_sp_add.Parameters.Add("@iQrcode", SqlDbType.Image);
                sqlCommand_modinfo_sp_add.Parameters.Add("@iQrcode1", SqlDbType.Image);
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPrintcount", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPrintcount1", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPartnamechineese", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcSuppliername", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcSupplieraddress", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcExecutestandard", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcCartype", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcHostip", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcOperatorID", "");
                #endregion
                foreach (DataRow item in dtLabelList_Temp.Rows)
                {
                    #region Value
                    sqlCommand_modinfo_sp_add.Parameters["@vcPartsnameen"].Value = item["vcPartsnameen"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcPart_id1"].Value = item["vcPart_id1"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcInno"].Value = item["vcInno"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcCpdcompany"].Value = item["vcCpdcompany"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcLabel"].Value = item["vcLabel"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcLabel1"].Value = item["vcLabel1"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcGetnum"].Value = item["vcGetnum"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@iDateprintflg"].Value = item["iDateprintflg"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcComputernm"].Value = item["vcComputernm"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcPrindate"].Value = item["vcPrindate"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@iQrcode"].Value = item["iQrcode"];
                    sqlCommand_modinfo_sp_add.Parameters["@iQrcode1"].Value = item["iQrcode1"];
                    sqlCommand_modinfo_sp_add.Parameters["@vcPrintcount"].Value = item["vcPrintcount"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcPrintcount1"].Value = item["vcPrintcount1"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcPartnamechineese"].Value = item["vcPartnamechineese"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcSuppliername"].Value = item["vcSuppliername"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcSupplieraddress"].Value = item["vcSupplieraddress"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcExecutestandard"].Value = item["vcExecutestandard"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcCartype"].Value = item["vcCartype"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcHostip"].Value = item["vcHostip"].ToString();
                    sqlCommand_modinfo_sp_add.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    #endregion
                    sqlCommand_modinfo_sp_add.ExecuteNonQuery();
                }
                #endregion

                #region 5.sqlCommand_modinfo_sp_mod
                SqlCommand sqlCommand_modinfo_sp_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_sp_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_sp_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_sp_mod = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo_sp_mod.AppendLine("INSERT INTO [dbo].[TInvList]");
                strSql_modinfo_sp_mod.AppendLine("           ([vcNo]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcData]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPrintdate]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcInno]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPart_Id]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPartsnamechn]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPartslocation]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcInputnum]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingquantity]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcItemname1]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation1]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn1]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum1]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcTemname2]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation2]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn2]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum2]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcItemname3]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation3]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn3]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum3]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcItemname4]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation4]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn4]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum4]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcItemname5]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation5]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn5]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum5]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcItemname6]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation6]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn6]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum6]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcItemname7]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation7]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn7]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum7]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcItemname8]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation8]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn8]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum8]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcItemname9]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation9]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn9]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum9]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcItemname10]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPackingpartslocation10]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcSuppliernamechn10]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOutnum10]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPartsnoandnum]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcLabel]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcComputernm]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcCpdcompany]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPlantcode]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcCompanyname]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcPlantname]");
                strSql_modinfo_sp_mod.AppendLine("           ,[iQrcode]");
                strSql_modinfo_sp_mod.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo_sp_mod.AppendLine("           ,[dOperatorTime]");
                strSql_modinfo_sp_mod.AppendLine("           ,[dFirstPrintTime]");
                strSql_modinfo_sp_mod.AppendLine("           ,[dLatelyPrintTime])");
                strSql_modinfo_sp_mod.AppendLine("     VALUES");
                strSql_modinfo_sp_mod.AppendLine("           (@vcNo");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcData");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPrintdate");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcInno");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPart_Id");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPartsnamechn");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPartslocation");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcInputnum");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingquantity");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcItemname1");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation1");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn1");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum1");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcTemname2");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation2");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn2");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum2");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcItemname3");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation3");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn3");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum3");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcItemname4");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation4");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn4");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum4");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcItemname5");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation5");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn5");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum5");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcItemname6");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation6");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn6");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum6");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcItemname7");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation7");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn7");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum7");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcItemname8");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation8");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn8");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum8");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcItemname9");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation9");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn9");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum9");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcItemname10");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPackingpartslocation10");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcSuppliernamechn10");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOutnum10");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPartsnoandnum");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcLabel");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcComputernm");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcCpdcompany");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPlantcode");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcCompanyname");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcPlantname");
                strSql_modinfo_sp_mod.AppendLine("           ,@iQrcode");
                strSql_modinfo_sp_mod.AppendLine("           ,@vcOperatorID");
                strSql_modinfo_sp_mod.AppendLine("           ,GETDATE()");
                strSql_modinfo_sp_mod.AppendLine("           ,null");
                strSql_modinfo_sp_mod.AppendLine("           ,null)");
                sqlCommand_modinfo_sp_mod.CommandText = strSql_modinfo_sp_mod.ToString();
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcNo", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcData", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPrintdate", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcInno", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPart_Id", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPartsnamechn", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPartslocation", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcInputnum", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingquantity", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcItemname1", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation1", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn1", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum1", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcTemname2", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation2", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn2", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum2", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcItemname3", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation3", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn3", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum3", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcItemname4", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation4", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn4", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum4", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcItemname5", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation5", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn5", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum5", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcItemname6", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation6", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn6", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum6", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcItemname7", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation7", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn7", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum7", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcItemname8", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation8", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn8", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum8", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcItemname9", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation9", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn9", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum9", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcItemname10", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPackingpartslocation10", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcSuppliernamechn10", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOutnum10", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPartsnoandnum", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcLabel", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcComputernm", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcCpdcompany", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPlantcode", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcCompanyname", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcPlantname", "");
                sqlCommand_modinfo_sp_mod.Parameters.Add("@iQrcode", SqlDbType.Image);
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@vcOperatorID", "");
                #endregion
                foreach (DataRow item in dtInv_Temp.Rows)
                {
                    #region Value
                    sqlCommand_modinfo_sp_mod.Parameters["@vcNo"].Value = item["vcNo"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcData"].Value = item["vcData"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPrintdate"].Value = item["vcPrintdate"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcInno"].Value = item["vcInno"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPart_Id"].Value = item["vcPart_Id"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPartsnamechn"].Value = item["vcPartsnamechn"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPartslocation"].Value = item["vcPartslocation"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcInputnum"].Value = item["vcInputnum"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingquantity"].Value = item["vcPackingquantity"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcItemname1"].Value = item["vcItemname1"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation1"].Value = item["vcPackingpartslocation1"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn1"].Value = item["vcSuppliernamechn1"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum1"].Value = item["vcOutnum1"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcTemname2"].Value = item["vcTemname2"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation2"].Value = item["vcPackingpartslocation2"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn2"].Value = item["vcSuppliernamechn2"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum2"].Value = item["vcOutnum2"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcItemname3"].Value = item["vcItemname3"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation3"].Value = item["vcPackingpartslocation3"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn3"].Value = item["vcSuppliernamechn3"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum3"].Value = item["vcOutnum3"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcItemname4"].Value = item["vcItemname4"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation4"].Value = item["vcPackingpartslocation4"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn4"].Value = item["vcSuppliernamechn4"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum4"].Value = item["vcOutnum4"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcItemname5"].Value = item["vcItemname5"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation5"].Value = item["vcPackingpartslocation5"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn5"].Value = item["vcSuppliernamechn5"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum5"].Value = item["vcOutnum5"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcItemname6"].Value = item["vcItemname6"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation6"].Value = item["vcPackingpartslocation6"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn6"].Value = item["vcSuppliernamechn6"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum6"].Value = item["vcOutnum6"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcItemname7"].Value = item["vcItemname7"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation7"].Value = item["vcPackingpartslocation7"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn7"].Value = item["vcSuppliernamechn7"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum7"].Value = item["vcOutnum7"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcItemname8"].Value = item["vcItemname8"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation8"].Value = item["vcPackingpartslocation8"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn8"].Value = item["vcSuppliernamechn8"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum8"].Value = item["vcOutnum8"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcItemname9"].Value = item["vcItemname9"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation9"].Value = item["vcPackingpartslocation9"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn9"].Value = item["vcSuppliernamechn9"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum9"].Value = item["vcOutnum9"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcItemname10"].Value = item["vcItemname10"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPackingpartslocation10"].Value = item["vcPackingpartslocation10"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcSuppliernamechn10"].Value = item["vcSuppliernamechn10"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOutnum10"].Value = item["vcOutnum10"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPartsnoandnum"].Value = item["vcPartsnoandnum"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcLabel"].Value = item["vcLabel"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcComputernm"].Value = item["vcComputernm"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcCpdcompany"].Value = item["vcCpdcompany"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPlantcode"].Value = item["vcPlantcode"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcCompanyname"].Value = item["vcCompanyname"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@vcPlantname"].Value = item["vcPlantname"].ToString();
                    sqlCommand_modinfo_sp_mod.Parameters["@iQrcode"].Value = item["iQrcode"];
                    sqlCommand_modinfo_sp_mod.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    #endregion
                    sqlCommand_modinfo_sp_mod.ExecuteNonQuery();
                }
                #endregion

                #region 7.sqlCommand_modinfo_pq_mod
                SqlCommand sqlCommand_modinfo_pq_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_pq_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_pq_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_pq_mod = new StringBuilder();
                #region SQL and Parameters
                strSql_modinfo_pq_mod.AppendLine("update SP_M_ORD set ");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily1=isnull(cast(vcInputQtyDaily1 as int),0)+cast(@vcInputQtyDaily1 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily2=isnull(cast(vcInputQtyDaily2 as int),0)+cast(@vcInputQtyDaily2 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily3=isnull(cast(vcInputQtyDaily3 as int),0)+cast(@vcInputQtyDaily3 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily4=isnull(cast(vcInputQtyDaily4 as int),0)+cast(@vcInputQtyDaily4 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily5=isnull(cast(vcInputQtyDaily5 as int),0)+cast(@vcInputQtyDaily5 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily6=isnull(cast(vcInputQtyDaily6 as int),0)+cast(@vcInputQtyDaily6 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily7=isnull(cast(vcInputQtyDaily7 as int),0)+cast(@vcInputQtyDaily7 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily8=isnull(cast(vcInputQtyDaily8 as int),0)+cast(@vcInputQtyDaily8 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily9=isnull(cast(vcInputQtyDaily9 as int),0)+cast(@vcInputQtyDaily9 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily10=isnull(cast(vcInputQtyDaily10 as int),0)+cast(@vcInputQtyDaily10 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily11=isnull(cast(vcInputQtyDaily11 as int),0)+cast(@vcInputQtyDaily11 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily12=isnull(cast(vcInputQtyDaily12 as int),0)+cast(@vcInputQtyDaily12 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily13=isnull(cast(vcInputQtyDaily13 as int),0)+cast(@vcInputQtyDaily13 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily14=isnull(cast(vcInputQtyDaily14 as int),0)+cast(@vcInputQtyDaily14 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily15=isnull(cast(vcInputQtyDaily15 as int),0)+cast(@vcInputQtyDaily15 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily16=isnull(cast(vcInputQtyDaily16 as int),0)+cast(@vcInputQtyDaily16 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily17=isnull(cast(vcInputQtyDaily17 as int),0)+cast(@vcInputQtyDaily17 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily18=isnull(cast(vcInputQtyDaily18 as int),0)+cast(@vcInputQtyDaily18 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily19=isnull(cast(vcInputQtyDaily19 as int),0)+cast(@vcInputQtyDaily19 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily20=isnull(cast(vcInputQtyDaily20 as int),0)+cast(@vcInputQtyDaily20 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily21=isnull(cast(vcInputQtyDaily21 as int),0)+cast(@vcInputQtyDaily21 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily22=isnull(cast(vcInputQtyDaily22 as int),0)+cast(@vcInputQtyDaily22 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily23=isnull(cast(vcInputQtyDaily23 as int),0)+cast(@vcInputQtyDaily23 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily24=isnull(cast(vcInputQtyDaily24 as int),0)+cast(@vcInputQtyDaily24 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily25=isnull(cast(vcInputQtyDaily25 as int),0)+cast(@vcInputQtyDaily25 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily26=isnull(cast(vcInputQtyDaily26 as int),0)+cast(@vcInputQtyDaily26 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily27=isnull(cast(vcInputQtyDaily27 as int),0)+cast(@vcInputQtyDaily27 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily28=isnull(cast(vcInputQtyDaily28 as int),0)+cast(@vcInputQtyDaily28 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily29=isnull(cast(vcInputQtyDaily29 as int),0)+cast(@vcInputQtyDaily29 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily30=isnull(cast(vcInputQtyDaily30 as int),0)+cast(@vcInputQtyDaily30 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDaily31=isnull(cast(vcInputQtyDaily31 as int),0)+cast(@vcInputQtyDaily31 as int),");
                strSql_modinfo_pq_mod.AppendLine("vcInputQtyDailySum=isnull(cast(vcInputQtyDailySum as int),0)+cast(@vcInputQtyDailySum as int) where iAutoId=@iAutoId");
                sqlCommand_modinfo_pq_mod.CommandText = strSql_modinfo_pq_mod.ToString();
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@iAutoId", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDailySum", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily1", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily2", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily3", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily4", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily5", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily6", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily7", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily8", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily9", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily10", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily11", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily12", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily13", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily14", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily15", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily16", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily17", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily18", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily19", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily20", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily21", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily22", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily23", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily24", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily25", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily26", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily27", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily28", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily29", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily30", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@vcInputQtyDaily31", "");
                #endregion
                foreach (DataRow item in dtOrder_Temp.Rows)
                {
                    #region Value
                    sqlCommand_modinfo_pq_mod.Parameters["@iAutoId"].Value = item["iAutoId"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDailySum"].Value = item["vcInputQtyDailySum"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily1"].Value = item["vcInputQtyDaily1"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily2"].Value = item["vcInputQtyDaily2"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily3"].Value = item["vcInputQtyDaily3"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily4"].Value = item["vcInputQtyDaily4"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily5"].Value = item["vcInputQtyDaily5"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily6"].Value = item["vcInputQtyDaily6"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily7"].Value = item["vcInputQtyDaily7"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily8"].Value = item["vcInputQtyDaily8"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily9"].Value = item["vcInputQtyDaily9"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily10"].Value = item["vcInputQtyDaily10"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily11"].Value = item["vcInputQtyDaily11"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily12"].Value = item["vcInputQtyDaily12"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily13"].Value = item["vcInputQtyDaily13"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily14"].Value = item["vcInputQtyDaily14"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily15"].Value = item["vcInputQtyDaily15"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily16"].Value = item["vcInputQtyDaily16"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily17"].Value = item["vcInputQtyDaily17"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily18"].Value = item["vcInputQtyDaily18"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily19"].Value = item["vcInputQtyDaily19"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily20"].Value = item["vcInputQtyDaily20"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily21"].Value = item["vcInputQtyDaily21"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily22"].Value = item["vcInputQtyDaily22"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily23"].Value = item["vcInputQtyDaily23"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily24"].Value = item["vcInputQtyDaily24"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily25"].Value = item["vcInputQtyDaily25"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily26"].Value = item["vcInputQtyDaily26"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily27"].Value = item["vcInputQtyDaily27"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily28"].Value = item["vcInputQtyDaily28"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily29"].Value = item["vcInputQtyDaily29"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily30"].Value = item["vcInputQtyDaily30"].ToString();
                    sqlCommand_modinfo_pq_mod.Parameters["@vcInputQtyDaily31"].Value = item["vcInputQtyDaily31"].ToString();
                    #endregion
                    sqlCommand_modinfo_pq_mod.ExecuteNonQuery();
                }
                #endregion

                #region 11.sqlCommand_printinfo
                SqlCommand sqlCommand_printinfo = sqlConnection.CreateCommand();
                sqlCommand_printinfo.Transaction = sqlTransaction;
                sqlCommand_printinfo.CommandType = CommandType.Text;
                StringBuilder strSql_printinfo = new StringBuilder();

                #region SQL and Parameters
                strSql_printinfo.AppendLine("INSERT INTO [TPackList_Temp]");
                strSql_printinfo.AppendLine(" ([vcLotid]");
                strSql_printinfo.AppendLine(" ,[iNo]");
                strSql_printinfo.AppendLine(",[vcPackingpartsno]");
                strSql_printinfo.AppendLine(",[vcPackinggroup]");
                strSql_printinfo.AppendLine(" ,[vcInno]");
                strSql_printinfo.AppendLine(",[dQty]");
                strSql_printinfo.AppendLine(" ,[vcPackingpartslocation]");
                strSql_printinfo.AppendLine(",[dDaddtime]");
                strSql_printinfo.AppendLine(",[vcPcname]");
                strSql_printinfo.AppendLine(" ,[vcHostip]");
                strSql_printinfo.AppendLine(",[vcOperatorID]");
                strSql_printinfo.AppendLine(" ,[dOperatorTime]");
                strSql_printinfo.AppendLine(",[vcTrolleyNo]");
                strSql_printinfo.AppendLine(",[dFirstPrintTime]");
                strSql_printinfo.AppendLine(",[dLatelyPrintTime]");
                strSql_printinfo.AppendLine(" ,[vcLabelStart]");
                strSql_printinfo.AppendLine(" ,[vcLabelEnd])");
                strSql_printinfo.AppendLine("   select");
                strSql_printinfo.AppendLine("vcLotid,");
                strSql_printinfo.AppendLine("null,");
                strSql_printinfo.AppendLine("vcPackingpartsno,");
                strSql_printinfo.AppendLine("vcPackinggroup,");
                strSql_printinfo.AppendLine("null,");
                strSql_printinfo.AppendLine("sum(dQty)as dQty,");
                strSql_printinfo.AppendLine("vcPackingpartslocation,");
                strSql_printinfo.AppendLine("null,");
                strSql_printinfo.AppendLine("null,");
                strSql_printinfo.AppendLine("@strIP,");
                strSql_printinfo.AppendLine("'" + strOperId + "',");
                strSql_printinfo.AppendLine("GETDATE(),");
                strSql_printinfo.AppendLine("vcTrolleyNo,");
                strSql_printinfo.AppendLine("null,");
                strSql_printinfo.AppendLine("null,");
                strSql_printinfo.AppendLine("vcLabelStart,");
                strSql_printinfo.AppendLine("vcLabelEnd");
                strSql_printinfo.AppendLine("from TPackList");
                strSql_printinfo.AppendLine("where vcHostip=@strIP");
                strSql_printinfo.AppendLine("and dFirstPrintTime is null");
                strSql_printinfo.AppendLine("group by");
                strSql_printinfo.AppendLine("vcLotid,");
                strSql_printinfo.AppendLine("vcPackingpartsno,");
                //strSql_printinfo.AppendLine("dQty,");
                strSql_printinfo.AppendLine("vcPackingpartslocation,");
                strSql_printinfo.AppendLine("vcPackinggroup,");
                strSql_printinfo.AppendLine("vcHostip,");
                strSql_printinfo.AppendLine("vcTrolleyNo,");
                strSql_printinfo.AppendLine("vcLabelStart,");
                strSql_printinfo.AppendLine("vcLabelEnd");
                strSql_printinfo.AppendLine("INSERT INTO [dbo].[TPrint_Temp]");
                strSql_printinfo.AppendLine("           ([vcTableName]");
                strSql_printinfo.AppendLine("           ,[vcReportName]");
                strSql_printinfo.AppendLine("           ,[vcClientIP]");
                strSql_printinfo.AppendLine("           ,[vcPrintName]");
                strSql_printinfo.AppendLine("           ,[vcKind]");
                strSql_printinfo.AppendLine("           ,[vcOperatorID]");
                strSql_printinfo.AppendLine("           ,[dOperatorTime]");
                strSql_printinfo.AppendLine("           ,[vcCaseNo]");
                strSql_printinfo.AppendLine("           ,[vcSellNo]");
                strSql_printinfo.AppendLine("           ,[vcLotid]");
                strSql_printinfo.AppendLine("           ,[vcSupplierId]");
                strSql_printinfo.AppendLine("           ,[vcInno]");
                strSql_printinfo.AppendLine("           ,[vcFlag])");
                strSql_printinfo.AppendLine("		   select distinct 'TPackList_Temp'");
                strSql_printinfo.AppendLine("		   ,'SPR13PKBP'");
                strSql_printinfo.AppendLine("		   ,@strIP");
                strSql_printinfo.AppendLine("		   ,'" + strPackPrinterName + "'");
                strSql_printinfo.AppendLine("		   ,'1'");
                strSql_printinfo.AppendLine("		   ,'" + strOperId + "'");
                strSql_printinfo.AppendLine("		   ,getdate()");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,vcLotid");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,'0'");
                strSql_printinfo.AppendLine("		   from TPackList");
                strSql_printinfo.AppendLine("		   where dFirstPrintTime  is null  and vcHostip=@strIP");
                strSql_printinfo.AppendLine("		   order by vcLotid");
                strSql_printinfo.AppendLine("INSERT INTO [dbo].[TPrint_Temp]");
                strSql_printinfo.AppendLine("           ([vcTableName]");
                strSql_printinfo.AppendLine("           ,[vcReportName]");
                strSql_printinfo.AppendLine("           ,[vcClientIP]");
                strSql_printinfo.AppendLine("           ,[vcPrintName]");
                strSql_printinfo.AppendLine("           ,[vcKind]");
                strSql_printinfo.AppendLine("           ,[vcOperatorID]");
                strSql_printinfo.AppendLine("           ,[dOperatorTime]");
                strSql_printinfo.AppendLine("           ,[vcCaseNo]");
                strSql_printinfo.AppendLine("           ,[vcSellNo]");
                strSql_printinfo.AppendLine("           ,[vcLotid]");
                strSql_printinfo.AppendLine("           ,[vcSupplierId]");
                strSql_printinfo.AppendLine("           ,[vcInno]");
                strSql_printinfo.AppendLine("           ,[vcFlag])");
                strSql_printinfo.AppendLine("		   select distinct 'TLabelList'");
                strSql_printinfo.AppendLine("		   ,'SPR06LBIP'");
                strSql_printinfo.AppendLine("		   ,@strIP");
                strSql_printinfo.AppendLine("		   ,'" + strPrinterName + "'");
                strSql_printinfo.AppendLine("		   ,'2'");
                strSql_printinfo.AppendLine("		   ,'" + strOperId + "'");
                strSql_printinfo.AppendLine("		   ,GETDATE()");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,'0'");
                sqlCommand_printinfo.CommandText = strSql_printinfo.ToString();
                sqlCommand_printinfo.Parameters.AddWithValue("@strIP", strIP);
                #endregion
                sqlCommand_printinfo.ExecuteNonQuery();
                #endregion

                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                return true;
                #endregion

            }
            catch (Exception ex)
            {
                //0613记录日志

                ComMessage.GetInstance().ProcessMessage("P00001", "M03UE0901", ex, "000000");
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
                return false;
            }
        }
        public DataTable GetPrintName(string iP, string strKind)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (strKind == "LABEL PRINTER R")
            {
                stringBuilder.Append("select vcPrinterName from TPrint where vcPrinterIp='" + iP + "' and vcKind='LABEL PRINTER R'");
            }
            if (strKind == "PAC PRINTER")
            {
                stringBuilder.Append("select vcPrinterName from TPrint where vcPrinterIp='" + iP + "' and vcKind='PAC PRINTER'");

            }



            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable checkPointState(string strOperater, string strPlant, string strIP)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SELECT *");
            stringBuilder.AppendLine("FROM [TPointState_Site]");
            stringBuilder.AppendLine("WHERE [vcOperater]='" + strOperater + "' AND [vcState]='登录中' and vcIP='" + strIP + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }

        }
        public DataTable getPointState_Site(string strOperater, string strPlant, string strIP)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SELECT *");
            stringBuilder.AppendLine("FROM [TPointState_Site]");
            stringBuilder.AppendLine("WHERE [vcOperater]='" + strOperater + "' AND [vcState]='登录中'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }

        }

        public void setPointState_Site(string strOperater, string strPlant, string strIP, string strSiteType, string strOperType)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("delete from [TPointState_Site] where [vcIP]='" + strIP + "'");
            if (strOperType == "登录")
            {
                stringBuilder.AppendLine("INSERT INTO [dbo].[TPointState_Site]");
                stringBuilder.AppendLine("         ([vcPlant],[vcIP],[vcPointNo],[vcPointType],[vcSiteType],[vcState],[vcOperater],[dOperateTime])");
                stringBuilder.AppendLine("select  vcPlant,vcPointIp,vcPointNo,vcPointType,'" + strSiteType + "' as vcSiteType,'登录中' as vcState,'" + strOperater + "' as vcOperater,GETDATE() as dOperateTime");
                stringBuilder.AppendLine("from TPointInfo");
                stringBuilder.AppendLine("where vcPointIp='" + strIP + "'");
            }
            if (strOperType == "销毁")
            {
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public void setSysExit(string strIP, string strType)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("declare @uuid varchar(100)");
            stringBuilder.AppendLine("select top(1)@uuid=b.UUID from ");
            stringBuilder.AppendLine("(select * from TPointInfo where vcPointIp='" + strIP + "')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPointDetails where dDestroyTime is null)b");
            stringBuilder.AppendLine("on a.vcPlant=b.vcPlant and a.vcPointNo=b.vcPointNo");
            stringBuilder.AppendLine("order by b.dOperateDate desc");
            if (strType == "包装-返回导航")
            {
                stringBuilder.AppendLine("update TPointDetails set dDestroyTime=GETDATE() where UUID=@uuid");
            }
            else if (strType == "包装-重新登录")
            {
                stringBuilder.AppendLine("update b set b.vcState='未登录',decEfficacy='0.00',vcOperater=null from ");
                stringBuilder.AppendLine("(select * from TPointInfo where vcPointIp='" + strIP + "')a");
                stringBuilder.AppendLine("left join");
                stringBuilder.AppendLine("(select * from TPointState)b");
                stringBuilder.AppendLine("on a.vcPointNo=b.vcPointNo and a.vcPlant=b.vcPlant");
                stringBuilder.AppendLine("update TPointDetails set dDestroyTime=GETDATE() where UUID=@uuid");
                stringBuilder.AppendLine("update TCaseInfo set vcPointState='0',dOperatorTime=GETDATE() where vcHostIp='" + strIP + "' and vcPointState='1' and dBoxPrintTime is null");
                stringBuilder.AppendLine("delete from [TPointState_Site] where [vcIP]='" + strIP + "' ");
            }
            else
            {
                stringBuilder.AppendLine("update b set b.vcState='未登录',decEfficacy='0.00',vcOperater=null from ");
                stringBuilder.AppendLine("(select * from TPointInfo where vcPointIp='" + strIP + "')a");
                stringBuilder.AppendLine("left join");
                stringBuilder.AppendLine("(select * from TPointState)b");
                stringBuilder.AppendLine("on a.vcPointNo=b.vcPointNo and a.vcPlant=b.vcPlant");
                stringBuilder.AppendLine("update TCaseInfo set vcPointState='0',dOperatorTime=GETDATE() where vcHostIp='" + strIP + "' and vcPointState='1' and dBoxPrintTime is null");
                stringBuilder.AppendLine("delete from [TPointState_Site] where [vcIP]='" + strIP + "' ");
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public void setAppHide(string strIP, string strPage)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("declare @uuid varchar(100)");
            stringBuilder.AppendLine("select top(1)@uuid=b.UUID from ");
            stringBuilder.AppendLine("(select * from TPointInfo where vcPointIp='" + strIP + "')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPointDetails where dDestroyTime is null)b");
            stringBuilder.AppendLine("on a.vcPlant=b.vcPlant and a.vcPointNo=b.vcPointNo");
            stringBuilder.AppendLine("order by b.dOperateDate desc");
            stringBuilder.AppendLine("update b set b.vcState='未登录',decEfficacy='0.00',vcOperater=null from ");
            stringBuilder.AppendLine("(select * from TPointInfo where vcPointIp='" + strIP + "')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPointState)b");
            stringBuilder.AppendLine("on a.vcPointNo=b.vcPointNo and a.vcPlant=b.vcPlant");
            stringBuilder.AppendLine("update TCaseInfo set vcPointState='0',dOperatorTime=GETDATE() where vcHostIp='" + strIP + "' and vcPointState='1' and dBoxPrintTime is null");
            stringBuilder.AppendLine("delete from [TPointState_Site] where [vcIP]='" + strIP + "' ");
            if (strPage == "包装")
            {
                stringBuilder.AppendLine("update TPointDetails set dDestroyTime=GETDATE() where UUID=@uuid");
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
    }
}
