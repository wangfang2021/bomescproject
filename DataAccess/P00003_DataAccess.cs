using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;
using System.Net;

namespace DataAccess
{
    public class P00003_DataAccess
    {
        private MultiExcute excute = new MultiExcute();



        public DataTable ValidateInv(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateInvSql = new StringBuilder();
            ValidateInvSql.Append("  select vcBZPlant,vcSHF,vcInputNo,iDBZ,iDZX,iDCH from TOperateSJ_InOutput where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' ");
            return excute.ExcuteSqlWithSelectToDT(ValidateInvSql.ToString());
        }

        public int UpdateCase1(string opearteId, string iP)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("  update TCaseInfo set vcStatus='1' where vcHostIp='" + iP + "' and vcOperatorID='" + opearteId + "' and vcStatus='0'");

            return excute.ExcuteSqlWithStringOper(UpdateCaseSql.ToString());
        }

        public DataTable GetCaseList(string iP, string caseNo)
        {
            StringBuilder GetCaseListSql = new StringBuilder();
            GetCaseListSql.Append("  select count(*) as count,sum(iQty) as sum from TCaseList where  vcCaseno='" + caseNo + "'");



            return excute.ExcuteSqlWithSelectToDT(GetCaseListSql.ToString());
        }

        public DataTable GetCaseList(string opearteId)
        {
            StringBuilder GetCaseListSql = new StringBuilder();
            GetCaseListSql.Append("select top 4(vcBoxNo) from TCaseInfo where  vcStatus='0' and vcOperatorID='" + opearteId + "' order by dOperatorTime desc");
            return excute.ExcuteSqlWithSelectToDT(GetCaseListSql.ToString());
        }

        public DataTable GetUserRole(string user)
        {
            StringBuilder GetUserRoleSql = new StringBuilder();
            GetUserRoleSql.Append("select vcPackUnLock from TPointPower  where vcUserId='" + user + "'");
            return excute.ExcuteSqlWithSelectToDT(GetUserRoleSql.ToString());
        }

        public DataTable ValidateSJ1(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateSJSql = new StringBuilder();
            ValidateSJSql.Append("  select iQuantity from TOperateSJ where vcPart_id='" + partId + "' and vcSR='" + dock + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcZYType='S3'");
            return excute.ExcuteSqlWithSelectToDT(ValidateSJSql.ToString());
        }

        public int UpdateEffi1(string pointNo, decimal effi)
        {
            StringBuilder UpdateEffiSql = new StringBuilder();
            UpdateEffiSql.Append("update TPointState set decEfficacy='" + effi * 100 + "' where vcPointNo='" + pointNo + "'");
            return excute.ExcuteSqlWithStringOper(UpdateEffiSql.ToString());
        }

        public DataTable getBanZhiTime(string strPackPlant, string strFlag)
        {
            try
            {
                SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
                SqlParameter[] pars = new SqlParameter[]{
                    new SqlParameter("@flag", strFlag),
                    new SqlParameter("@PackPlant",strPackPlant)
                };
                string cmdText = "BSP0811_getBanZhiTime";
                SqlDataAdapter sa = new SqlDataAdapter(cmdText, sqlConnection);
                if (pars != null && pars.Length > 0)
                {
                    foreach (SqlParameter p in pars)
                    {
                        sa.SelectCommand.Parameters.Add(p);
                    }
                }
                sa.SelectCommand.CommandTimeout = 0;
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetPoint(string iP)
        {
            StringBuilder GetPointSql = new StringBuilder();
            GetPointSql.Append("select vcPointNo from TPointInfo where vcPointIp = '" + iP + "' and vcUsed = '在用'");
            return excute.ExcuteSqlWithSelectToDT(GetPointSql.ToString());
        }

        public DataSet getOperPointInfo(string strPackPlant, string strBanZhi, string strHosDate, string strOperater, string strFromTime_nw)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("--TBZTime_Rest_point	所在班值休息字典");
            strSql.AppendLine("select ROW_NUMBER() OVER(PARTITION BY vcPackPlant,vcBanZhi ");
            strSql.AppendLine("					ORDER BY dateadd(day,cast(vcBeforCross as int),cast('" + strHosDate + "' as datetime))+' '+tBeforTime) AS TANK");
            strSql.AppendLine("		,vcPackPlant");
            strSql.AppendLine("		,vcBanZhi");
            strSql.AppendLine("		,dateadd(day,cast(vcBeforCross as int),cast('" + strHosDate + "' as datetime))+' '+tBeforTime as  tBeforTime");
            strSql.AppendLine("		,dateadd(day,cast(vcLastCross as int),cast('" + strHosDate + "' as datetime))+' '+tLastTime  as  tLastTime");
            strSql.AppendLine("		,iMinute");
            strSql.AppendLine("from TBZTime_Rest_point where vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "'");
            strSql.AppendLine("--TPointDetails	点位登录履历");
            strSql.AppendLine("select dHosDate,vcPlant as vcPackPlant,vcBanZhi,vcPointNo,UUID,dEntryTime,case when dDestroyTime is null then GETDATE() else dDestroyTime end as dDestroyTime");
            strSql.AppendLine("from TPointDetails where vcPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and dHosDate='" + strHosDate + "' and vcOperater='" + strOperater + "'");
            strSql.AppendLine("order by dEntryTime");
            strSql.AppendLine("--TOperateSJ	操作人当日作业基准");
            strSql.AppendLine("SELECT ISNULL(SUM(T1.decQuantity*T2.vcStandardTime),0) AS decOperStandard FROM ");
            strSql.AppendLine("(SELECT A1.vcBZPlant,A2.vcSmallPM,sum(iQuantity) AS decQuantity  FROM ");
            strSql.AppendLine("(select vcBZPlant,vcPart_id,CAST(iQuantity AS int) AS iQuantity,CAST(vcBZUnit AS int) AS vcBZUnit ");
            strSql.AppendLine("from TOperateSJ ");
            strSql.AppendLine("where vcZYType='S2' and vcBZPlant='" + strPackPlant + "' and vcOperatorID='" + strOperater + "' AND dOperatorTime>='" + strFromTime_nw + "' and dOperatorTime<=GETDATE())A1");
            strSql.AppendLine("LEFT JOIN");
            strSql.AppendLine("(select vcBZPlant,vcPart_id,vcSmallPM");
            strSql.AppendLine("from TPackageMaster");
            strSql.AppendLine("where dTimeFrom<=CONVERT(varchar(10),GETDATE(),23) AND dTimeTo>=CONVERT(varchar(10),GETDATE(),23))A2");
            strSql.AppendLine("ON A1.vcBZPlant=A2.vcBZPlant AND A1.vcPart_id=A2.vcPart_id");
            strSql.AppendLine("GROUP BY A1.vcBZPlant,A2.vcSmallPM)T1");
            strSql.AppendLine("LEFT JOIN");
            strSql.AppendLine("(SELECT * FROM TPMRelation)T2");
            strSql.AppendLine("ON T1.vcSmallPM=T2.vcSmallPM");
            return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
        }


        public int UpdateStatus4(string pointNo, string opearteId)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='暂停' where vcPointNo='" + pointNo + "' and vcOperater='" + opearteId + "'");
            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
        }

        public int UpdateStatus5(string pointNo, string opearteId)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='正常' where vcPointNo='" + pointNo + "' and vcOperater='" + opearteId + "'");
            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
        }

        public int UpdateEffi(string formatDate, string opearteId, string stopTime)
        {
            StringBuilder UpdateEffiSql = new StringBuilder();
            UpdateEffiSql.Append("update  TOperateSJ_Effiency set iStopTime=" + stopTime + " where vcDate='" + formatDate + "' and vcBZPlant='H2' and vcOperatorID='" + opearteId + "'");
            return excute.ExcuteSqlWithStringOper(UpdateEffiSql.ToString());
        }

        public DataTable GetStatus2(string iP, string opearteId)
        {
            StringBuilder GetStatusSql = new StringBuilder();
            GetStatusSql.Append("select t1.vcState,t1.vcPointNo from TPointState t1,TPointInfo t2 where t1.vcPointNo=t2.vcPointNo and t2.vcPointIp='" + iP + "' and t2.vcUsed='在用'   and t1.vcOperater='" + opearteId + "'");
            return excute.ExcuteSqlWithSelectToDT(GetStatusSql.ToString());
        }

        public int UpdateCase2(string caseNo, string serverTime)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("update TCaseInfo set dOperatorTime='" + serverTime + "' where vcBoxNo='" + caseNo + "' and vcStatus='0'");
            return excute.ExcuteSqlWithStringOper(UpdateCaseSql.ToString());
        }

        public DataTable GetTime(string formatDate, string opearteId)
        {
            StringBuilder GetTimeSql = new StringBuilder();
            GetTimeSql.Append("  select vcTotalTime,iFrequency,vcEffiency,dStartTime,ISNULL(vcPackTotalTime,0) from TOperateSJ_Effiency where vcOperatorID='" + opearteId + "' and vcDate='" + formatDate + "'");
            return excute.ExcuteSqlWithSelectToDT(GetTimeSql.ToString());

        }

        public int UpdateFre(string time, string serverTime, string formatDate, string opearteId)
        {
            StringBuilder UpdateFreSql = new StringBuilder();
            UpdateFreSql.Append("update TOperateSJ_Effiency set dStartTime='" + time + "',dOperatorTime='" + serverTime + "' where vcOperatorID='" + opearteId + "' and vcDate='" + formatDate + "'");
            return excute.ExcuteSqlWithStringOper(UpdateFreSql.ToString());
        }

        public int InsertFre(string time, string formatDate, string effiEncy, string opearteId, string serverTime, string iP, string date, string banZhi)
        {
            StringBuilder InsertFreSql = new StringBuilder();
            // InsertFreSql.Append("INSERT INTO TOperateSJ_Effiency (dStartTime ,vcDate ,vcTotalTime,iFrequency ,vcEffiency,vcOperatorID,dOperatorTime) \n");
            // InsertFreSql.Append("     VALUES ('" + time + "','" + formatDate + "','0',0,'" + effiEncy + "','" + opearteId + "','" + serverTime + "') \n");
            InsertFreSql.Append("INSERT INTO TOperateSJ_Effiency(vcBZPlant,vcPointIp,dHosDate,vcBanZhi,dStartTime\n");
            InsertFreSql.Append(" ,iStopTime,vcDate,vcTotalTime,iFrequency,vcEffiency,vcOperatorID,dOperatorTime,vcPackTotalTime)\n");
            InsertFreSql.Append("     VALUES('H2','" + iP + "','" + date + "','" + banZhi + "','" + time + "',0,'" + formatDate + "',0,0,'" + effiEncy + "','" + opearteId + "','" + serverTime + "','0')\n");


            return excute.ExcuteSqlWithStringOper(InsertFreSql.ToString());
        }

        public DataTable ValidateData1(string partId, string scanTime)
        {
            StringBuilder validateDataSql = new StringBuilder();

            validateDataSql.Append("select iPackingQty,vcSupplierId,vcSupplierPlant from TSPMaster_Box where vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");

            return excute.ExcuteSqlWithSelectToDT(validateDataSql.ToString());
        }

        public DataTable GetPackInfo(string partId, string scanTime, string packingQuatity, string quantity)
        {
            StringBuilder GetPackInfoSql = new StringBuilder();
            GetPackInfoSql.Append("  select vcPackNo,vcDistinguish,iBiYao*" + int.Parse(quantity) + "/" + int.Parse(packingQuatity) + "  as sum from TPackItem where vcPartsNo='" + partId + "' and dUsedFrom<='" + scanTime + "' and dUsedTo>='" + scanTime + "' order by vcDistinguish");
            return excute.ExcuteSqlWithSelectToDT(GetPackInfoSql.ToString());
        }

        public int UpdateFre1(int totalTime, string opearteId, string formatDate)
        {
            StringBuilder UpdateFreSql = new StringBuilder();
            UpdateFreSql.Append("  update TOperateSJ_Effiency set vcTotalTime='" + totalTime + "' where vcDate='" + formatDate + "' and vcOperatorID='" + opearteId + "'");
            return excute.ExcuteSqlWithStringOper(UpdateFreSql.ToString());
        }

        public DataTable GetCheckType(string partId, string scanTime, string supplier_id)
        {
            StringBuilder getCheckTypeSql = new StringBuilder();

            getCheckTypeSql.Append("select vcCheckP from tCheckQf  where vcSupplierCode='" + supplier_id + "' and  vcPartId='" + partId + "' and vcTimeFrom<='" + scanTime + "' and vcTimeTo>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(getCheckTypeSql.ToString());
        }

        public DataTable ValidateSJ(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateSJSql = new StringBuilder();
            ValidateSJSql.Append("  select iQuantity from TOperateSJ where  vcPart_id='" + partId + "' and vcSR='" + dock + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcZYType='S2'");
            return excute.ExcuteSqlWithSelectToDT(ValidateSJSql.ToString());
        }

        public int InsertTP(string iP, string opearteId, string serverTime, string caseNo)
        {
            StringBuilder insertTPSql = new StringBuilder();
            insertTPSql.Append("INSERT INTO TPrint_Temp(vcTableName,vcReportName,vcClientIP,vcPrintName,vcKind,vcOperatorID,dOperatorTime,vcCaseNo,vcSellNo,vcLotid,vcSupplierId, vcInno, vcFlag)");
            insertTPSql.Append("   VALUES ('TCaseList','SPR09PACP','" + iP + "','LASEL PRINTER','3','" + opearteId + "','" + serverTime + "','" + caseNo + "','','','','','0')");
            return excute.ExcuteSqlWithStringOper(insertTPSql.ToString());
        }

        public int UpdateCase(string count, string sum, string caseNo)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("  update TCaseList set iTotalcnt='" + count + "',iTotalpiece='" + sum + "' where vcCaseno='" + caseNo + "' ");
            return excute.ExcuteSqlWithStringOper(UpdateCaseSql.ToString());
        }

        public DataTable ValidateCaseNo(string caseNo)
        {
            StringBuilder ValidateCaseNoSql = new StringBuilder();
            ValidateCaseNoSql.Append("   select vcStatus,vcSheBeiNo,vcHostIp,vcOperatorID from TCaseInfo where vcBoxNo='" + caseNo + "'");//vcStatus0正在使用中,1没有使用,2表示已经出荷可以重复使用 
            return excute.ExcuteSqlWithSelectToDT(ValidateCaseNoSql.ToString());
        }

        public int UpdateStatus3(string pointNo, string opearteId)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='未登录',vcOperater='" + opearteId + "' where vcPointNo='" + pointNo + "'");
            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
        }



        public DataTable GetStatus1(string iP, string opearteId)
        {
            StringBuilder GetStatusSql = new StringBuilder();
            GetStatusSql.Append("select t1.vcPointNo from TPointState t1,TPointInfo t2 where t1.vcPointNo=t2.vcPointNo and t2.vcPointIp='" + iP + "' and t2.vcUsed='在用' and t1.vcState='正常'  and t1.vcOperater='" + opearteId + "'");
            return excute.ExcuteSqlWithSelectToDT(GetStatusSql.ToString());
        }

        public DataTable GetCase1(string caseNo)
        {
            StringBuilder GetCaseSql = new StringBuilder();

            GetCaseSql.Append("select * from TCaseInfo where vcStatus='0' and vcBoxNo='" + caseNo + "'");
            return excute.ExcuteSqlWithSelectToDT(GetCaseSql.ToString());
        }

        public DataTable GetStanTime()
        {
            StringBuilder GetStanTimeSql = new StringBuilder();
            GetStanTimeSql.Append("select decObjective from TDisplaySettings");
            return excute.ExcuteSqlWithSelectToDT(GetStanTimeSql.ToString());
        }

        public DataTable GetCaseNo(string iP)
        {
            StringBuilder GetCaseNoSql = new StringBuilder();
            GetCaseNoSql.Append("select vcBoxNo from TCaseInfo where  vcStatus='0' and vcHostIp='" + iP + "' order by dOperatorTime desc");
            return excute.ExcuteSqlWithSelectToDT(GetCaseNoSql.ToString());
        }

        public int UpdateCase3(string caseNo)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("update TCaseInfo set vcStatus='1' where vcBoxNo='" + caseNo + "' and vcStatus='0'");
            return excute.ExcuteSqlWithStringOper(UpdateCaseSql.ToString());
        }

        public DataTable GetPackData(string partId, string scanTime)
        {
            StringBuilder GetPackDataSql = new StringBuilder();
            string time = scanTime.Replace("-", "").Substring(0, 8);
            GetPackDataSql.Append("  select vcDistinguish as distinguish,count(vcDistinguish) as sum from TPackItem where vcPartsNo='" + partId + "' and dFrom<'" + time + "' and dTo>'" + time + "' group by vcDistinguish order by vcDistinguish");
            return excute.ExcuteSqlWithSelectToDT(GetPackDataSql.ToString());
        }



        public int InsertSj(string supplierId, string supplierPlant, string packingQuantity, string checkType, string lblStart, string lblEnd, string inOutFlag, string checkStatus, string packingSpot, string inputNo, string checkNum, string partId, string kanbanOrderNo, string kanbanSerial, string dock, string opearteId, string scanTime, string serverTime, string iP, string sHF, string quantity1, string caseNo, string pointType)
        {
            StringBuilder InsertOprSql = new StringBuilder();
            InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
            InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
            InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime,vcHostIp,packingcondition,vcPackingPlant)\n");
            InsertOprSql.Append("VALUES('S3','" + packingSpot + "','" + inputNo + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inOutFlag + "','" + supplierId + "','" + supplierPlant + "','" + scanTime + "','" + serverTime + "',\n");
            InsertOprSql.Append("" + quantity1 + ",'" + packingQuantity + "','" + sHF + "','" + dock + "','" + caseNo + "','" + pointType + "','" + checkType + "'," + quantity1 + ",'" + checkStatus + "','" + lblStart + "','" + lblEnd + "','',null,'','" + opearteId + "','" + serverTime + "','" + iP + "','1','')\n");
            return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
        }

        public DataTable GetLabel(string inputNo)
        {
            StringBuilder GetLabelSql = new StringBuilder();
            GetLabelSql.Append("select dFirstPrintTime from TLabelList where vcInno='" + inputNo + "' ");
            return excute.ExcuteSqlWithSelectToDT(GetLabelSql.ToString());
        }

        public DataTable ValidateCaseNo4(string caseNo)
        {
            StringBuilder ValidateCaseNoSql = new StringBuilder();
            ValidateCaseNoSql.Append("select * from TCaseNoInfo where vcCaseNo='" + caseNo + "' and dFirstPrintTime<>''");
            return excute.ExcuteSqlWithSelectToDT(ValidateCaseNoSql.ToString());
        }

        public DataTable GetBanZhi(string serverTime)
        {
            StringBuilder GetBanZhiSql = new StringBuilder();
            GetBanZhiSql.Append("select dHosDate,vcBanZhi from (select convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,-1,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,-1,GETDATE())),23) else convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='夜' and vcPackPlant='H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 0, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '白' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 0, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '夜' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 1, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '白' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append(")t where tFromTime<=GETDATE() and tToTime>=GETDATE()\n");
            return excute.ExcuteSqlWithSelectToDT(GetBanZhiSql.ToString());
        }

        public DataTable ValidateOpr5(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append(" select vcSupplier_id,vcSupplierGQ,vcBZUnit,vcCheckType,vcLabelStart,vcLabelEnd,vcIOType,vcCheckStatus,vcBZPlant,vcInputNo,iCheckNum,vcSHF,iQuantity from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S0' ");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }

        public DataTable ValidateSJ2(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateSJSql = new StringBuilder();
            ValidateSJSql.Append("select iQuantity from TBoxMaster  where vcDelete='0' and vcPart_id='" + partId + "' and vcOrderNo='" + kanbanOrderNo + "' and vcLianFanNo='" + kanbanSerial + "' and vcSR='" + dock + "'");



            return excute.ExcuteSqlWithSelectToDT(ValidateSJSql.ToString());
        }

        public DataTable GetCaseList1(string caseNo)
        {
            StringBuilder GetCaseListSql = new StringBuilder();
            GetCaseListSql.Append("select vcInputNo,iQuantity from TOperateSJ where vcZYType='S3' and vcBoxNo='" + caseNo + "'");
            return excute.ExcuteSqlWithSelectToDT(GetCaseListSql.ToString());
        }

        public DataTable GetStatus(string iP)
        {
            StringBuilder GetStatusSql = new StringBuilder();
            GetStatusSql.Append("select t1.vcPointNo from TPointState t1,TPointInfo t2 where t1.vcPointNo=t2.vcPointNo and t2.vcPointIp='" + iP + "' and t2.vcUsed='在用' and t1.vcState='未登录'");
            return excute.ExcuteSqlWithSelectToDT(GetStatusSql.ToString());
        }

        public int UpdateInv(string inputNo, string quantity)
        {
            StringBuilder UpdateInvSql = new StringBuilder();
            UpdateInvSql.Append("update TOperateSJ_InOutput set iDZX = iDZX - " + int.Parse(quantity) + ", iDCH = iDCH + " + int.Parse(quantity) + " where vcInputNo = '" + inputNo + "'");
            return excute.ExcuteSqlWithStringOper(UpdateInvSql.ToString());
        }

        public DataTable ValidateInv(string inputNo)
        {
            StringBuilder ValidateInvSql = new StringBuilder();
            ValidateInvSql.Append("select iDZX,iDCH,vcBZPlant,vcSHF from TOperateSJ_InOutput where vcInputNo='" + inputNo + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateInvSql.ToString());
        }

        public int UpdateTime(string formatDate, int totalTime, string opearteId)
        {
            StringBuilder UpdateTimeSql = new StringBuilder();
            UpdateTimeSql.Append("update TOperateSJ_Effiency set vcPackTotalTime=(CAST(ISNULL(vcPackTotalTime,0) as int)+" + totalTime + ") where vcDate='" + formatDate + "' and vcOperatorID='" + opearteId + "'");
            return excute.ExcuteSqlWithStringOper(UpdateTimeSql.ToString());
        }

        public int UpdateStatus2(string iP, string opearteId, string pointNo)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='正常',vcOperater='" + opearteId + "' where vcPointNo='" + pointNo + "'");
            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
        }

        public DataTable GetInputQuantity(string kanbanOrderNo, string kanbanSerial, string partId, string dock)
        {
            StringBuilder GetInputQuantitySql = new StringBuilder();
            GetInputQuantitySql.Append("select iQuantity from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S0'");
            return excute.ExcuteSqlWithSelectToDT(GetInputQuantitySql.ToString());
        }

        public DataTable GetCase(string opearteId)
        {
            StringBuilder GetCaseSql = new StringBuilder();
            GetCaseSql.Append("select top(1)vcBoxNo from TCaseInfo where  vcStatus='0' and vcOperatorID='" + opearteId + "' order by dOperatorTime desc");
            return excute.ExcuteSqlWithSelectToDT(GetCaseSql.ToString());
        }

        public int UpdateEffi(string formatDate, double effiencyNew, string opearteId, string serverTime)
        {
            StringBuilder UpdateEffiSql = new StringBuilder();
            UpdateEffiSql.Append("  update TOperateSJ_Effiency set iFrequency=iFrequency+1,vcEffiency='" + effiencyNew + "',dOperatorTime='" + serverTime + "' where vcOperatorID='" + opearteId + "' and vcDate='" + formatDate + "'");
            return excute.ExcuteSqlWithStringOper(UpdateEffiSql.ToString());
        }

        public DataTable ValidateData(string partId, string scanTime)
        {
            StringBuilder ValidateDataSql = new StringBuilder();
            ValidateDataSql.Append("select vcPartId,vcSupplierId from TSPMaster where vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateDataSql.ToString());
        }

        public DataTable ValidateTime(string partId, string dock)
        {
            StringBuilder ValidateTimeSql = new StringBuilder();
            ValidateTimeSql.Append(" select vcStandardTime from TPackageMaster t1,TPMRelation t2 where t1.vcSmallPM=t2.vcSmallPM  and t1.vcSR='" + dock + "' and t1.vcPart_id='" + partId + "'");



            return excute.ExcuteSqlWithSelectToDT(ValidateTimeSql.ToString());
        }

        public int InsertTP1(string iP, string opearteId, string serverTime, string inputNo, string printName)
        {
            StringBuilder insertTPSql = new StringBuilder();
            insertTPSql.Append("INSERT INTO TPrint_Temp(vcTableName,vcReportName,vcClientIP,vcPrintName,vcKind,vcOperatorID,dOperatorTime,vcCaseNo,vcSellNo,vcLotid,vcSupplierId,vcInno,vcFlag)");
            insertTPSql.Append("   VALUES ('TLabelList','SPR06LBIP','" + iP + "','" + printName + "','6','" + opearteId + "','" + serverTime + "','','','','','" + inputNo + "','0')");
            return excute.ExcuteSqlWithStringOper(insertTPSql.ToString());
        }

        public DataTable ValidateOpr4(string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string partId)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append(" select vcSupplier_id,vcSupplierGQ,vcBZUnit,vcCheckType,vcLabelStart,vcLabelEnd,vcIOType,vcCheckStatus,vcBZPlant,vcInputNo,iCheckNum,vcSHF,iQuantity from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S2' and vcCheckStatus='OK'");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }

        public int InsertBox(string caseNo, string inputNo, string partId, string kanbanOrderNo, string kanbanSerial, string quantity, string opearteId, string scanTime, string labelStart, string labelEnd, string rhQuantity, string serverTime, string dock)
        {
            StringBuilder InsertBoxSql = new StringBuilder();
            InsertBoxSql.Append("INSERT INTO TBoxMaster(vcStatus,vcBoxNo,vcInstructionNo,vcPart_id,vcOrderNo,vcLianFanNo,iQuantity\n");
            InsertBoxSql.Append(",dBZID,dBZTime,dZXID,dZXTime,vcOperatorID,dOperatorTime,iRHQuantity,vcLabelStart,vcLabelEnd,vcDelete,dPrintBoxTime,vcSR)\n");
            InsertBoxSql.Append("VALUES('','" + caseNo + "','" + inputNo + "','" + partId + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + quantity + "','" + opearteId + "','" + scanTime + "','" + opearteId + "','" + scanTime + "','" + opearteId + "','" + serverTime + "','" + rhQuantity + "','" + labelStart + "','" + labelEnd + "','0',null,'" + dock + "')\n");

            return excute.ExcuteSqlWithStringOper(InsertBoxSql.ToString());
        }

        public DataTable GetPrintName(string iP)
        {
            StringBuilder GetPrintNameSql = new StringBuilder();
            GetPrintNameSql.Append("select vcPrinterName from TPrint where vcPrinterIp='" + iP + "' and vcKind='LABEL PRINTER'");
            return excute.ExcuteSqlWithSelectToDT(GetPrintNameSql.ToString());
        }

        public DataTable GetPackData1(string partId, string serverTime)
        {
            StringBuilder GetPackDataSql = new StringBuilder();
            GetPackDataSql.Append("select vcPackNo,iBiYao,vcPackGPSNo from TPackItem where vcPartsNo='" + partId + "' and dUsedFrom<='" + serverTime + "' and dUsedTo>='" + serverTime + "'");
            return excute.ExcuteSqlWithSelectToDT(GetPackDataSql.ToString());
        }

        public DataTable ValidateCaseNo(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string caseNo)
        {
            StringBuilder ValidateCaseNoSql = new StringBuilder();
            ValidateCaseNoSql.Append("select vcPart_id from TBoxMaster  where vcSR='" + dock + "' and vcBoxNo='" + caseNo + "' and vcPart_id='" + partId + "' and vcOrderNo='" + kanbanOrderNo + "' and vcLianFanNo='" + kanbanSerial + "'  and vcDelete='0'");
            return excute.ExcuteSqlWithSelectToDT(ValidateCaseNoSql.ToString());
        }

        public int InsertPackWork(string packNo, string gpsNo, string packsupplier, string bZUnit, string biYao, string opearteId, string serverTime, string quantity)
        {
            double quantity1 = double.Parse(quantity) / double.Parse(bZUnit) * double.Parse(biYao);
            StringBuilder InsertPackWorkSql = new StringBuilder();
            InsertPackWorkSql.Append("INSERT INTO TPackWork(vcZuoYeQuFen,vcOrderNo,vcPackNo,vcPackGPSNo,vcSupplierID\n");
            InsertPackWorkSql.Append(",vcPackSpot,iNumber,dBuJiTime,dZiCaiTime,vcYanShouID,vcOperatorID,dOperatorTime)\n");
            InsertPackWorkSql.Append("VALUES('2','','" + packNo + "','" + gpsNo + "','" + packsupplier + "','H2'," + quantity1 + ",'" + serverTime + "',null,'','" + opearteId + "','" + serverTime + "')\n");
            return excute.ExcuteSqlWithStringOper(InsertPackWorkSql.ToString());
        }

        public int InsertZaiKu(string packNo, string gpsNo, string packsupplier, string opearteId, string serverTime)
        {
            StringBuilder InsertZaiKuSql = new StringBuilder();
            InsertZaiKuSql.Append("INSERT INTO TPackZaiKu(vcPackSpot,vcPackNo,vcPackGPSNo,vcSupplierID,iLiLun,iAnQuan,dChange,vcOperatorID,dOperatorTime)\n");
            InsertZaiKuSql.Append("VALUES ('H2','" + packNo + "','" + gpsNo + "','" + packsupplier + "',0,0,0,'" + opearteId + "','" + serverTime + "')\n");
            return excute.ExcuteSqlWithStringOper(InsertZaiKuSql.ToString());
        }

        public DataTable GetZaiKu(string packNo, string gpsNo, string packsupplier)
        {
            StringBuilder GetZaiKuSql = new StringBuilder();
            GetZaiKuSql.Append("  select dChange from TPackZaiKu where vcPackSpot='H2' AND vcPackNo='" + packNo + "' AND vcPackGPSNo='" + gpsNo + "' AND vcSupplierID='" + packsupplier + "'");
            return excute.ExcuteSqlWithSelectToDT(GetZaiKuSql.ToString());
        }

        public int UpdateCase5(string iP, string caseNo)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("update TCaseInfo set vcStatus='1' where vcHostIp='" + iP + "' and vcStatus='0' and vcBoxNo='" + caseNo + "'");
            return excute.ExcuteSqlWithStringOper(UpdateCaseSql.ToString());
        }

        public int UpdateInv2(string packNo, string gpsNo, string packsupplier, string bZUnit, string biYao, string opearteId, string serverTime, string quantity)
        {
            double quantity1 = double.Parse(quantity) / double.Parse(bZUnit) * double.Parse(biYao);
            StringBuilder UpdateInvSql = new StringBuilder();
            UpdateInvSql.Append("update TPackZaiKu set iLiLun=iLiLun-" + quantity1 + ",dChange = dChange - " + quantity1 + " where vcPackNo = '" + packNo + "' and vcPackGPSNo = '" + gpsNo + "' and vcSupplierID = '" + packsupplier + "'");
            return excute.ExcuteSqlWithStringOper(UpdateInvSql.ToString());
        }

        public DataTable GetPackBase(string packNo, string serverTime)
        {
            StringBuilder GetPackBaseSql = new StringBuilder();
            GetPackBaseSql.Append("select vcSupplierCode,vcSupplierPlant from TPackBase where vcPackNo='" + packNo + "' and dPackFrom<='" + serverTime + "' and dPackTo>='" + serverTime + "'");
            return excute.ExcuteSqlWithSelectToDT(GetPackBaseSql.ToString());
        }

        public DataTable GetPointNo(string iP)
        {
            StringBuilder GetPointNoSql = new StringBuilder();
            GetPointNoSql.Append("select vcPointType,vcPointNo from TPointInfo where vcPointIp='" + iP + "' and vcUsed='在用'");
            return excute.ExcuteSqlWithSelectToDT(GetPointNoSql.ToString());
        }

        public int UpdateBox(string caseNo, string serverTime)
        {
            StringBuilder UpdateBoxSql = new StringBuilder();
            UpdateBoxSql.Append("update TBoxMaster set dPrintBoxTime='" + serverTime + "' where vcBoxNo='" + caseNo + "'");
            return excute.ExcuteSqlWithStringOper(UpdateBoxSql.ToString());
        }

        public DataTable GetCaseInfo1(string caseNo)
        {
            StringBuilder GetCaseInfoSql = new StringBuilder();
            GetCaseInfoSql.Append("select vcPart_id from TCaseList where vcCaseno='" + caseNo + "'");
            return excute.ExcuteSqlWithSelectToDT(GetCaseInfoSql.ToString());
        }

        public DataTable GetCaseInfo(string caseNo)
        {
            StringBuilder GetCaseInfoSql = new StringBuilder();
            GetCaseInfoSql.Append("select vcInstructionNo,vcPart_id,vcOrderNo,vcLianFanNo,iQuantity,vcSR from TBoxMaster where vcBoxNo='" + caseNo + "' and vcDelete='0' and dPrintBoxTime is null");
            return excute.ExcuteSqlWithSelectToDT(GetCaseInfoSql.ToString());
        }

        public DataTable GetQuantity(string kanbanOrderNo, string kanbanSerial, string partId, string dock)
        {
            StringBuilder GetQuantitySql = new StringBuilder();
            GetQuantitySql.Append("select iQuantity,vcInputNo from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S0'");
            return excute.ExcuteSqlWithSelectToDT(GetQuantitySql.ToString());
        }

        public int InsertCase(string sHF, string cpdName, string cpdAddress, string caseNo, string inputNo, string partId, string quantity, string partsName, string opearteId, string serverTime, string iP, byte[] vs, string labelStart, string labelEnd)
        {
            /*
            StringBuilder InsertCaseSql = new StringBuilder();
            InsertCaseSql.Append("INSERT INTO TCaseList(vcCpdcode ,vcCpdname,vcCpdaddress,vcCaseno,vcCasebarcode,iNo ,vcInno \n");
            InsertCaseSql.Append("           ,vcPart_id,vcPartsname,iQty,iTotalcnt ,iTotalpiece,vcPcname,vcHostip,iDatamatrixcode,vcOperatorID,dOperatorTime) \n");
            InsertCaseSql.Append("   VALUES('" + sHF + "','" + cpdName + "','" + cpdAddress + "','" + caseNo + "','',null,'" + inputNo + "','" + partId + "','" + partsName + "','" + int.Parse(quantity) + "','','',null,'" + iP + "','','" + opearteId + "','" + serverTime + "')\n");
            return excute.ExcuteSqlWithStringOper(InsertCaseSql.ToString());
            */
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                SqlCommand sqlCommand_sub = sqlConnection.CreateCommand();
                sqlCommand_sub.Transaction = sqlTransaction;
                sqlCommand_sub.CommandType = CommandType.Text;
                StringBuilder strSql_sub = new StringBuilder();
                strSql_sub.Append("INSERT INTO [dbo].[TCaseList]");
                strSql_sub.Append("           ([vcCpdcode]");
                strSql_sub.Append("           ,[vcCpdname]");
                strSql_sub.Append("           ,[vcCpdaddress]");
                strSql_sub.Append("           ,[vcCaseno]");
                strSql_sub.Append("           ,[vcCasebarcode]");
                strSql_sub.Append("           ,[iNo]");
                strSql_sub.Append("           ,[vcInno]");
                strSql_sub.Append("           ,[vcPart_id]");
                strSql_sub.Append("           ,[vcPartsname]");
                strSql_sub.Append("           ,[iQty]");
                strSql_sub.Append("           ,[iTotalcnt]");
                strSql_sub.Append("           ,[iTotalpiece]");
                strSql_sub.Append("           ,[vcPcname]");
                strSql_sub.Append("           ,[vcHostip]");
                strSql_sub.Append("           ,[iDatamatrixcode]");
                strSql_sub.Append("           ,[vcOperatorID]");
                strSql_sub.Append("           ,[dOperatorTime]");
                strSql_sub.Append("           ,[dFirstPrintTime]");
                strSql_sub.Append("           ,[dLatelyPrintTime]");
                strSql_sub.Append("           ,[vcLabelStart]");
                strSql_sub.Append("           ,[vcLabelEnd])");
                strSql_sub.Append("     VALUES");


                strSql_sub.AppendLine("           ('" + sHF + "'");
                strSql_sub.AppendLine("           ,'" + cpdName + "'");
                strSql_sub.AppendLine("           ,'" + cpdAddress + "'");
                strSql_sub.AppendLine("           ,'" + caseNo + "'");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,null");
                strSql_sub.AppendLine("           ,'" + inputNo + "'");
                strSql_sub.AppendLine("           ,'" + partId + "'");
                strSql_sub.AppendLine("           ,'" + partsName + "'");
                strSql_sub.AppendLine("           ,'" + quantity + "'");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,null");
                strSql_sub.AppendLine("           ,'" + iP + "'");

                strSql_sub.AppendLine("           ,@iQrcode");
                strSql_sub.AppendLine("           ,'" + opearteId + "'");
                strSql_sub.AppendLine("           ,'" + serverTime + "'");
                strSql_sub.AppendLine("           ,null");
                strSql_sub.AppendLine("           ,null");
                strSql_sub.AppendLine("           ,'" + labelStart + "'");
                strSql_sub.AppendLine("           ,'" + labelEnd + "')");



                sqlCommand_sub.CommandText = strSql_sub.ToString();
                sqlCommand_sub.Parameters.Add("@iQrcode", SqlDbType.Image);
                sqlCommand_sub.Parameters["@iQrcode"].Value = vs;
                sqlCommand_sub.ExecuteNonQuery();
                sqlTransaction.Commit();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {

                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }

            }
            return 1;















        }

        public int InsertSj1(string supplier_id, string supplierGQ, string bZUnit, string checkType, string labelStart, string labelEnd, string inoutFlag, string checkStatus, string bzPlant, string inputNo, string quantity, string partId, string kanbanOrderNo, string kanbanSerial, string dock, string opearteId, string scanTime, string serverTime, string iP, string sHF, string caseNo)
        {
            StringBuilder InsertOprSql = new StringBuilder();
            InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
            InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
            InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime)\n");
            InsertOprSql.Append("VALUES('S2','" + bzPlant + "','" + inputNo + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inoutFlag + "','" + supplier_id + "','" + supplierGQ + "','" + scanTime + "','" + serverTime + "',\n");
            InsertOprSql.Append("" + quantity + ",'" + quantity + "','" + sHF + "','" + dock + "','','1','" + checkType + "'," + quantity + ",'" + checkStatus + "','" + labelStart + "','" + labelEnd + "','',null,'','system','" + serverTime + "')\n");
            return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
        }

        public DataTable GetPartsName(string scanTime, string partId)
        {
            StringBuilder getPartsNameSql = new StringBuilder();
            getPartsNameSql.Append("select vcPartENName	 from TSPMaster WHERE vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(getPartsNameSql.ToString());
        }

        public int UpdateInv1(string partId, string kanbanOrderNo, string kanbanSerial, string quantity)
        {
            StringBuilder UpdateInvSql = new StringBuilder();
            UpdateInvSql.Append("  update TOperateSJ_InOutput set iDZX=iDZX-" + int.Parse(quantity) + ",iDCH=iDCH+" + int.Parse(quantity) + " where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");
            return excute.ExcuteSqlWithStringOper(UpdateInvSql.ToString());
        }


        public DataTable ValidateInv1(string partId, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateInvSql = new StringBuilder();
            ValidateInvSql.Append("select iDBZ,iDZX,iDCH from TOperateSJ_InOutput where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");

            return excute.ExcuteSqlWithSelectToDT(ValidateInvSql.ToString());
        }

        public int UpdateCase(string opearteId, string caseNo)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("  update TCaseInfo set vcStatus='1' where vcOperatorID='" + opearteId + "' and vcBoxNo='" + caseNo + "'");
            return excute.ExcuteSqlWithStringOper(UpdateCaseSql.ToString());
        }

        public DataTable ValidateOpr1(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("  select vcSupplier_id,vcSupplierGQ,vcBZUnit,vcCheckType,vcLabelStart,vcLabelEnd,vcIOType,vcCheckStatus,vcBZPlant,vcInputNo,vcSheBeiNo,iCheckNum from TOperateSJ where vcSR='" + dock + "' and vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' AND vcZYType='S0' ");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }

        public DataTable ValidateCaseNo3(string caseNo)
        {
            StringBuilder ValidateCaseNoSql = new StringBuilder();
            ValidateCaseNoSql.Append("select vcPart_id,vcOrderNo,vcLianFanNo,vcSR,iQuantity from TBoxMaster where vcDelete='0' and vcBoxNo='" + caseNo + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateCaseNoSql.ToString());
        }

        public DataTable ValidateCaseNo2(string caseNo)
        {
            StringBuilder ValidateCaseNoSql = new StringBuilder();
            ValidateCaseNoSql.Append("select vcCaseno from TShipList where vcCaseno = '" + caseNo + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateCaseNoSql.ToString());
        }



        public int UpdateCaseInfo(string caseNo, string opearteId, string iP, string serverTime)
        {
            StringBuilder UpdateCaseInfoSql = new StringBuilder();
            UpdateCaseInfoSql.Append("update tCaseInfo set  dOperatorTime='" + serverTime + "',vcOperatorID='" + opearteId + "',vcHostIp='" + iP + "',vcStatus='0' where vcBoxNo='" + caseNo + "'");

            return excute.ExcuteSqlWithStringOper(UpdateCaseInfoSql.ToString());
        }

        public int InsertCaseInfo(string caseNo, string opearteId, string iP, string serverTime)
        {
            StringBuilder InsertCaseInfoSql = new StringBuilder();
            InsertCaseInfoSql.Append("INSERT INTO TCaseInfo(vcSheBeiNo,vcHostIp,vcBoxNo,vcStatus,vcOperatorID,dOperatorTime)");
            InsertCaseInfoSql.Append("VALUES('','" + iP + "','" + caseNo + "','0','" + opearteId + "','" + serverTime + "')");
            return excute.ExcuteSqlWithStringOper(InsertCaseInfoSql.ToString());
        }

        public DataTable ValidateCaseNo1(string caseNo)
        {
            StringBuilder ValidateCaseNoSql = new StringBuilder();
            ValidateCaseNoSql.Append("select vcCaseno from TPrint_Temp  where vcCaseno = '" + caseNo + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateCaseNoSql.ToString());
        }







        public DataTable GetPM(string dock, string partId)
        {
            StringBuilder GetPMSql = new StringBuilder();
            string formatpartId = partId.Substring(0, 5);
            GetPMSql.Append("  select vcSmallPM from TPackageMaster where vcPart_id='" + partId + "' and vcSR='" + dock + "'");
            return excute.ExcuteSqlWithSelectToDT(GetPMSql.ToString());
        }



        public DataTable ValidateOpr2(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("  select vcSupplier_id,vcSupplierGQ,vcBZUnit,vcCheckType,vcLabelStart,vcLabelEnd,vcIOType,vcCheckStatus,iQuantity from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and iQuantity=" + int.Parse(quantity) + "AND vcZYType='S2' and vcCheckStatus='OK'");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }




        public DataTable ValidateOpr3(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("  select vcSupplier_id,vcSupplierGQ,vcBZUnit,vcCheckType,vcLabelStart,vcLabelEnd,vcIOType,vcCheckStatus from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' AND vcZYType='S2' and vcCheckStatus='OK'");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }

        public DataTable GetData(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder GetDataSql = new StringBuilder();
            GetDataSql.Append("  select vcBZUnit,vcCheckType from  TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S0' ");
            return excute.ExcuteSqlWithSelectToDT(GetDataSql.ToString());
        }






        public int UpdateInv(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string scanTime, string serverTime, string opearteId)
        {
            StringBuilder UpdateInvSql = new StringBuilder();
            UpdateInvSql.Append(" update TOperateSJ_InOutput set iDBZ=iDBZ-" + int.Parse(quantity) + ",iDZX=iDZX+" + int.Parse(quantity) + ",vcOperatorID='" + opearteId + "',dOperatorTime='" + serverTime + "' where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' ");
            return excute.ExcuteSqlWithStringOper(UpdateInvSql.ToString());
        }

        public DataTable ValidateOpr(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("  select vcSupplier_id,vcSupplierGQ,vcBZUnit,vcCheckType,vcLabelStart,vcLabelEnd,vcIOType,vcCheckStatus from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' AND vcZYType='S1' and vcCheckStatus='OK'");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }

        public int InsertOpr(string bzPlant, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inoutFlag, string supplier_id, string supplierGQ, string scanTime, string serverTime, string quantity, int packingQuantity, string sHF, string dock, string checkType, string labelStart, string labelEnd, string checkStatus, string opearteId, string timeStart, string timeEnd, string iP, string pointType)
        {
            StringBuilder InsertOprSql = new StringBuilder();
            InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
            InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
            InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime,vcHostIp,packingcondition,vcPackingPlant)\n");
            InsertOprSql.Append("VALUES('S2','" + bzPlant + "','" + inputNo + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inoutFlag + "','" + supplier_id + "','" + supplierGQ + "','" + timeStart + "','" + timeEnd + "',\n");
            InsertOprSql.Append("" + int.Parse(quantity) + ",'" + packingQuantity + "','" + sHF + "','" + dock + "','','" + pointType + "','" + checkType + "'," + int.Parse(quantity) + ",'" + checkStatus + "','" + labelStart + "','" + labelEnd + "','',null,'','" + opearteId + "','" + serverTime + "','" + iP + "','1','')\n");
            return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
        }
        //========================================================================重写========================================================================
        public DataTable GetCaseNoInfo(string strCaseNo)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT A.vcPlant,A.vcReceiver,A.vcCaseNo,");
            stringBuilder.Append("		B.vcBoxNo,B.vcHostIp,B.vcSheBeiNo,B.vcOperatorID,B.vcPointState,B.dBoxPrintTime,");
            stringBuilder.Append("		C.vcPointType+C.vcPointNo as vcPointType,");
            stringBuilder.Append("		D.kanbanQuantity");
            stringBuilder.Append("		FROM ");
            stringBuilder.Append("(select * from TCaseNoInfo where vcPlant+'*'+vcCaseNo='" + strCaseNo + "')A");
            stringBuilder.Append("LEFT JOIN");
            stringBuilder.Append("(select * from TCaseInfo)B");
            stringBuilder.Append("ON A.vcPlant+'*'+A.vcCaseNo=B.vcCaseNo");
            stringBuilder.Append("LEFT JOIN");
            stringBuilder.Append("(select vcPointType,vcPointNo,vcPointIp from TPointInfo where vcUsed='在用')C");
            stringBuilder.Append("ON B.vcHostIp=c.vcPointIp");
            stringBuilder.Append(" LEFT JOIN");
            stringBuilder.Append("(select vcCaseNo,COUNT(*) AS kanbanQuantity from TBoxMaster where vcDelete='0' and vcCaseNo='" + strCaseNo + "')D");
            stringBuilder.Append("ON A.vcPlant+'*'+A.vcCaseNo=D.vcCaseNo");
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
        public void SetCaseNoInfo(string strBoxNo, string strCaseNo, string strHostIp, string strSheBeiNo, string strPointState, string strOperatorID)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM [TCaseInfo] WHERE [vcBoxNo]='" + strBoxNo + "'");
            stringBuilder.AppendLine("INSERT INTO [dbo].[TCaseInfo]([vcBoxNo],[vcCaseNo],[vcHostIp],[vcSheBeiNo],[vcPointState],[dBoxPrintTime],[vcOperatorID],[dOperatorTime])");
            stringBuilder.AppendLine("     VALUES('" + strBoxNo + "','" + strCaseNo + "','" + strHostIp + "','" + strSheBeiNo + "','" + strPointState + "',NULL,'" + strOperatorID + "',GETDATE())");
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
