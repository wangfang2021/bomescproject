using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;
using System.Net;
using System.Drawing;

namespace DataAccess
{
    public class P00001_DataAccess
    {
        private MultiExcute excute = new MultiExcute();



        public DataTable Validate(string partId, string kanbanOrderNo, string kanbanSerial, string dock)
        {
            StringBuilder ValidateSql = new StringBuilder();
            ValidateSql.Append("select vcPart_id from TOperatorQB WHERE vcPart_id='" + partId + "'and vcSR = '" + dock + "' and vcKBOrderNo = '" + kanbanOrderNo + "' and vcKBLFNo = '" + kanbanSerial + "' and vcReflectFlag in (0,1)");

            return excute.ExcuteSqlWithSelectToDT(ValidateSql.ToString());
        }

        public int Insert(string trolley, string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string scanTime, String iP, string serverTime, string cpdCompany, string inno, string opearteId, string packingSpot, string packQuantity, string lblSart, string lblEnd, string supplierId, string supplierPlant, string trolleySeqNo, string inoutFlag, string kanBan)
        {
            StringBuilder InsertSql = new StringBuilder();
            InsertSql.Append("INSERT INTO TOperatorQB(vcZYType,dScanTime,vcHtNo,vcTrolleyNo ,vcInputNo,vcPart_id,vcCpdCompany,vcSR ,vcBoxNo,iQuantity  \n");
            InsertSql.Append(" ,vcSeqNo,vcReflectFlag,dStart,dEnd,vcHostIp,vcKBOrderNo,vcKBLFNo,vcOperatorID ,dOperatorTime,vcBZPlant,iPackingQty,vcLabelStart,vcLabelEnd,vcSupplierId,vcSupplierPlant,vcLotid,vcIOType,vcCheckType,vcKanBan,vcTrolleySeqNo)  \n");
            InsertSql.Append("    VALUES('S0', '" + scanTime + "', '', '" + trolley + "', '" + inno + "', '" + partId + "', '" + cpdCompany + "', '" + dock + "', '', " + int.Parse(quantity) + ", '', '0', null, null, '" + iP + "', '" + kanbanOrderNo + "', '" + kanbanSerial + "', '" + opearteId + "', '" + serverTime + "','" + packingSpot + "'," + packQuantity + ",'" + lblSart + "','" + lblEnd + "','" + supplierId + "','" + supplierPlant + "','','" + inoutFlag + "','','" + kanBan + "','" + trolleySeqNo + "')");
            return excute.ExcuteSqlWithStringOper(InsertSql.ToString());

        }

        public DataTable GetPrint1(string iP)
        {
            StringBuilder GetPrintSql = new StringBuilder();
            GetPrintSql.Append("select vcUserFlag from TPrint where vcKind in ('LABEL PRINTER','LASEL PRINTER') and vcPrinterIp='" + iP + "'");
            return excute.ExcuteSqlWithSelectToDT(GetPrintSql.ToString());
        }

        public DataTable GetPrint(string iP)
        {
            StringBuilder GetPrintSql = new StringBuilder();
            GetPrintSql.Append("select vcUserFlag from TPrint where vcKind in ('LABEL PRINTER','LASEL PRINTER','DOT PRINTER') and vcPrinterIp='" + iP + "'");
            return excute.ExcuteSqlWithSelectToDT(GetPrintSql.ToString());
        }

        public DataTable GetUserRole(string opearteId)
        {
            StringBuilder GetUserRoleSql = new StringBuilder();
            GetUserRoleSql.Append("select vcInPut, vcCheck, vcPack, vcOutPut from TPointPower where vcUserId = '" + opearteId + "'");
            return excute.ExcuteSqlWithSelectToDT(GetUserRoleSql.ToString());
        }

        public DataTable ValidateOpr1(string inno)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("select vcPart_id,vcSR,vcKBOrderNo,vcKBLFNo,iQuantity,vcSupplier_id from TOperateSJ where vcInputNo='" + inno + "' and vcZYType='S0'");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }

        public DataTable ValidateOpr2(string inno)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("select vcPart_id,vcSR,vcKBOrderNo,vcKBLFNo,iQuantity,vcSupplier_id from TOperateSJ where vcInputNo='" + inno + "' and vcZYType='S1'");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }

        public DataTable GetPackQuantity(string inno)
        {
            StringBuilder GetPackQuantitySql = new StringBuilder();
            GetPackQuantitySql.Append("select iQuantity from TBoxMaster where vcInstructionNo='" + inno + "' and  vcDelete='0' ");
            return excute.ExcuteSqlWithSelectToDT(GetPackQuantitySql.ToString());
        }

        public DataTable GetInputQuantity(string inno)
        {
            StringBuilder GetInputQuantitySql = new StringBuilder();
            GetInputQuantitySql.Append("select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR,iQuantity from TOperateSJ where vcZYType='S0' and vcInputNo='" + inno + "'");
            return excute.ExcuteSqlWithSelectToDT(GetInputQuantitySql.ToString());
        }

        public DataTable GetKanBanSum(string iP)
        {
            StringBuilder GetKanBanSumSql = new StringBuilder();
            GetKanBanSumSql.Append("select count(*) from TOperatorQB where vcHostIp='" + iP + "' and vcReflectFlag='0' and vcZYType='S0'");
            return excute.ExcuteSqlWithSelectToDT(GetKanBanSumSql.ToString());
        }

        public DataTable GetInfoData(string iP)
        {
            StringBuilder GetInfoDataSql = new StringBuilder();

            GetInfoDataSql.Append("select top(1) vcPart_id,vcKBOrderNo,vcKBLFNo,iQuantity,vcSR,vcTrolleyNo,vcTrolleySeqNo from TOperatorQB where vcHostIp='" + iP + "' order by iAutoId desc\n");
            return excute.ExcuteSqlWithSelectToDT(GetInfoDataSql.ToString());
        }

        public DataTable GetSeqNo(string tmpString, string formatServerTime)
        {
            StringBuilder GetSeqNo = new StringBuilder();
            GetSeqNo.Append("select SEQNO from TSeqNo where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
            return excute.ExcuteSqlWithSelectToDT(GetSeqNo.ToString());
        }


        public int InsertTrolley1(string seqNo, string trolley, string iP, string opearteId, string serverTime)
        {
            StringBuilder InsertTrolleySql = new StringBuilder();
            InsertTrolleySql.Append("INSERT INTO TOperateSJ_TrolleyInfo(vcSheBeiNo,vcHostIp,vcTrolleyNo,vcStatus\n");
            InsertTrolleySql.Append(",vcOperatorID,dOperatorTime,vcLotid,vcTrolleySeqNo)VALUES('','" + iP + "','" + trolley + "','0','" + opearteId + "','" + serverTime + "','','" + seqNo + "')\n");
            return excute.ExcuteSqlWithStringOper(InsertTrolleySql.ToString());
        }

        public int UpdateSeqNo(string tmpString, string formatServerTime, int seqNoNew)
        {
            StringBuilder UpdateSeqNoSql = new StringBuilder();
            UpdateSeqNoSql.Append("update TSeqNo set SEQNO='" + seqNoNew + "' where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
            return excute.ExcuteSqlWithStringOper(UpdateSeqNoSql.ToString());
        }

        public int InsertSeqNo(string tmpString, string formatServerTime)
        {
            StringBuilder InsertSeqNoSql = new StringBuilder();
            InsertSeqNoSql.Append("INSERT INTO TSeqNo(FLAG ,DDATE ,SEQNO) \n");
            InsertSeqNoSql.Append("VALUES( '" + tmpString + "','" + formatServerTime + "','1')");
            return excute.ExcuteSqlWithStringOper(InsertSeqNoSql.ToString());
        }

        public DataTable ValidateUser(string opearteId)
        {
            StringBuilder ValidateUserSql = new StringBuilder();
            ValidateUserSql.Append("select vcPointNo from TPointState where vcState='正常' and vcOperater='" + opearteId + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateUserSql.ToString());
        }

        public DataTable GetTrolleyData(string iP)
        {
            StringBuilder GetTrolleyDataSql = new StringBuilder();

            GetTrolleyDataSql.Append("select vcTrolleyNo,vcLotid from TOperateSJ_TrolleyInfo\n");
            GetTrolleyDataSql.Append("where dOperatorTime = (select Max(dOperatorTime) from TOperateSJ_TrolleyInfo )\n");
            GetTrolleyDataSql.Append("and vcHostIp='" + iP + "' and  vcStatus='0'\n");







            return excute.ExcuteSqlWithSelectToDT(GetTrolleyDataSql.ToString());
        }

        public DataTable GetQuantity1(string inno)
        {
            StringBuilder GetQuantitySql = new StringBuilder();
            GetQuantitySql.Append("select isnull(sum(iQuantity),0) from TOperateSJ where vcInputNo='" + inno + "' and vcZYType='S1'");
            return excute.ExcuteSqlWithSelectToDT(GetQuantitySql.ToString());
        }

        public DataTable GetTrolley(string iP)
        {
            StringBuilder GetTrolleySql = new StringBuilder();

            GetTrolleySql.Append("select top(1)vcTrolleyNo, vcStatus,vcTrolleySeqNo from TOperateSJ_TrolleyInfo\n");
            GetTrolleySql.Append("where vcHostIp='" + iP + "' order by iAutoId desc\n");
            return excute.ExcuteSqlWithSelectToDT(GetTrolleySql.ToString());
        }

        public DataTable GetKanBan(string iP, string trolley, string trolleySeqNo)
        {
            StringBuilder GetKanBanSql = new StringBuilder();
            GetKanBanSql.Append("select top(1) vcReflectFlag from TOperatorQB where vcHostIp='" + iP + "' and vcTrolleyNo='" + trolley + "' and vcTrolleySeqNo='" + trolleySeqNo + "' order by iAutoId desc");
            return excute.ExcuteSqlWithSelectToDT(GetKanBanSql.ToString());
        }

        public DataTable GetQuantity(string inno)
        {
            StringBuilder GetQuantitySql = new StringBuilder();
            GetQuantitySql.Append("select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR,iQuantity from TOperateSJ where vcInputNo='" + inno + "' and vcZYType='S0'");
            return excute.ExcuteSqlWithSelectToDT(GetQuantitySql.ToString());
        }

        public DataTable GetQuantity(string partId, string scanTime, string dock)
        {
            StringBuilder GetQuantitySql = new StringBuilder();
            GetQuantitySql.Append("select vcBZUnit from TPackageMaster where vcSR='" + dock + "' and vcPart_id='" + partId + "' and dTimeFrom<'" + scanTime + "' and dTimeTo>'" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(GetQuantitySql.ToString());
        }

        public DataTable GetPointDetails(string iP)
        {
            StringBuilder GetPointDetailsSql = new StringBuilder();
            GetPointDetailsSql.Append("select vcPointName from TPointInfo where vcPointIp='" + iP + "' and vcUsed='在用'");


            return excute.ExcuteSqlWithSelectToDT(GetPointDetailsSql.ToString());
        }

        public DataTable GetDetail(string pointNo)
        {
            StringBuilder GetDetailSql = new StringBuilder();
            GetDetailSql.Append("select  UUID from TPointDetails where vcPointNo='" + pointNo + "' and dDestroyTime is null order by dOperateDate desc");
            return excute.ExcuteSqlWithSelectToDT(GetDetailSql.ToString());
        }

        public int UpdateDetail(string uuid, string serverTime)
        {
            StringBuilder UpdateDetailSql = new StringBuilder();
            UpdateDetailSql.Append("update TPointDetails set dDestroyTime='" + serverTime + "' where UUID='" + uuid + "'");
            return excute.ExcuteSqlWithStringOper(UpdateDetailSql.ToString());
        }

        public int UpdateStatus5(string pointNo)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='未登录',vcOperater='' where vcPointNo='" + pointNo + "' and vcPlant='H2'");
            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
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

        public DataTable GetPointType(string pointNo)
        {
            StringBuilder GetPointTypeSql = new StringBuilder();
            GetPointTypeSql.Append("select vcPointType from TPointInfo where vcPointNo='" + pointNo + "' and vcUsed='在用'");
            return excute.ExcuteSqlWithSelectToDT(GetPointTypeSql.ToString());
        }

        public int UpdateCase(string iP)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("update TCaseInfo set vcStatus='1' where vcHostIp='" + iP + "'");
            return excute.ExcuteSqlWithStringOper(UpdateCaseSql.ToString());
        }

        public int InsertDetail(string date, string banZhi, string pointNo, string uuid, string serverTime, string opearteId)
        {
            StringBuilder InsertDetailSql = new StringBuilder();
            InsertDetailSql.Append("INSERT INTO TPointDetails(vcPlant,dHosDate,vcBanZhi,vcPointNo,UUID,dEntryTime,dDestroyTime,iStopSS,vcOperater,dOperateDate)\n");
            InsertDetailSql.Append("VALUES('H2','" + date + "','" + banZhi + "','" + pointNo + "','" + uuid + "','" + serverTime + "',NULL,0,'" + opearteId + "','" + serverTime + "')\n");
            return excute.ExcuteSqlWithStringOper(InsertDetailSql.ToString());
        }

        public int UpdateStatus4(string pointNo, string opearteId)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='正常',vcOperater='" + opearteId + "' where vcPointNo='" + pointNo + "' and vcPlant='H2'");
            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
        }



        public DataTable GetPointStatus3(string iP, string opearteId)
        {
            StringBuilder GetPointStatusSql = new StringBuilder();
            GetPointStatusSql.Append("select t1.vcPointNo from TPointState t1,TPointInfo t2 where t1.vcPointNo=t2.vcPointNo and t2.vcPointIp='" + iP + "' and t2.vcUsed='在用' and t1.vcState='未登录' and t1.vcOperater='" + opearteId + "'");
            return excute.ExcuteSqlWithSelectToDT(GetPointStatusSql.ToString());
        }


        public int UpdateStatus3(string pointNo)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='未用' where vcPointNo='" + pointNo + "' and vcPlant='H2'");
            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
        }

        public int UpdateStatus2(string pointNo, string iP, string opearteId)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='未用' where vcOperater='" + opearteId + "' and vcPointNo='" + pointNo + "'");


            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
        }

        public DataTable GetPointStatus2(string opearteId, string iP)
        {
            StringBuilder GetPointStatusSql = new StringBuilder();
            GetPointStatusSql.Append("select t1.vcPointNo from TPointState t1,TPointInfo  t2 where t1.vcPointNo=t2.vcPointNo and t2.vcPointIp='" + iP + "' and t1.vcState='正常' and t1.vcOperater='" + opearteId + "' and t1.vcPlant='H2'");
            return excute.ExcuteSqlWithSelectToDT(GetPointStatusSql.ToString());
        }

        public int UpdateStatus1(string pointNo, string opearteId)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update  TPointState set vcState='未登录',vcOperater='" + opearteId + "' where vcPointNo='" + pointNo + "' and vcPlant='H2'");



            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
        }

        public DataTable GetPointStatus1(string opearteId, string iP)
        {
            StringBuilder GetPointStatusSql = new StringBuilder();
            GetPointStatusSql.Append("select t1.vcPointNo from TPointState t1,TPointInfo  t2 where t1.vcPointNo=t2.vcPointNo and t2.vcPointIp='" + iP + "' and t1.vcState='未登录'");




            return excute.ExcuteSqlWithSelectToDT(GetPointStatusSql.ToString());
        }

        public int UpdateStatus(string opearteId, string iP, string pointNo)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();


            UpdateStatusSql.Append("update  TPointState set vcState='正常',vcOperater='" + opearteId + "' where vcPointNo='" + pointNo + "'");
            return excute.ExcuteSqlWithStringOper(UpdateStatusSql.ToString());
        }

        public DataTable GetPoint(string iP)
        {
            StringBuilder GetPointSql = new StringBuilder();
            GetPointSql.Append("select t1.vcPointNo from TPointInfo t1,TPointState t2 where t1.vcPointNo=t2.vcPointNo and t1.vcPointIp='172.23.238.154' and t2.vcPlant='H2' and t2.vcState in ('未用','未登录') and t1.vcUsed='在用'  ");
            return excute.ExcuteSqlWithSelectToDT(GetPointSql.ToString());
        }

        public int InsertPoint(string pointNo)
        {
            StringBuilder InsertPointSql = new StringBuilder();
            InsertPointSql.Append("INSERT INTO TPointState (vcPointNo,vcPlant,vcState,vcOperater)   \n");
            InsertPointSql.Append("     VALUES('" + pointNo + "','H2','未登录','')   \n");
            return excute.ExcuteSqlWithStringOper(InsertPointSql.ToString());
        }

        public int UpdatePoint(string pointNo)
        {
            StringBuilder UpdatePointSql = new StringBuilder();
            UpdatePointSql.Append("  update TPointState set vcState='未登录' where vcPointNo='" + pointNo + "' and vcPlant='H2' and vcState='未用'");


            return excute.ExcuteSqlWithStringOper(UpdatePointSql.ToString());
        }

        public DataTable GetPointStatus(string pointNo)
        {
            StringBuilder GetPointStatusSql = new StringBuilder();
            GetPointStatusSql.Append("  select vcOperater from TPointState where vcPointNo='" + pointNo + "' and vcPlant='H2' and vcState in ('未用','正常') ");
            return excute.ExcuteSqlWithSelectToDT(GetPointStatusSql.ToString());
        }

        public DataTable GetTrolley(string opearteId, string iP)
        {
            StringBuilder GetTrolleySql = new StringBuilder();
            GetTrolleySql.Append(" select vcTrolleyNo,vcLotid from TOperateSJ_TrolleyInfo \n");
            GetTrolleySql.Append("where dOperatorTime = (select Max(dOperatorTime) from TOperateSJ_TrolleyInfo )\n");
            GetTrolleySql.Append("and vcHostIp='" + iP + "' and vcOperatorID='" + opearteId + "' and vcStatus='0' \n");

            return excute.ExcuteSqlWithSelectToDT(GetTrolleySql.ToString());
        }

        public DataTable GetPoint1(string iP)
        {
            StringBuilder GetPointSql = new StringBuilder();
            GetPointSql.Append("select vcPointNo from TPointInfo where vcPointIp='" + iP + "' and vcUsed='在用'");
            return excute.ExcuteSqlWithSelectToDT(GetPointSql.ToString());
        }

        public DataTable GetCase(string opearteId, string iP)
        {
            StringBuilder GetCaseSql = new StringBuilder();
            GetCaseSql.Append("select vcBoxNo from TCaseInfo where vcHostIp='" + iP + "' and vcOperatorID='" + opearteId + "' order by dOperatorTime desc");
            return excute.ExcuteSqlWithSelectToDT(GetCaseSql.ToString());
        }

        public DataTable GetCase1(string caseNo)
        {
            StringBuilder GetCaseSql = new StringBuilder();
            GetCaseSql.Append("select vcCaseno from TCaseList  where vcCaseno='" + caseNo + "'");
            return excute.ExcuteSqlWithSelectToDT(GetCaseSql.ToString());
        }

        public int UpdateCase(string iP, string serverTime, string opearteId, string caseNo)
        {
            StringBuilder UpdateCaseSql = new StringBuilder();
            UpdateCaseSql.Append("update TCaseInfo set vcHostIp='" + iP + "',vcStatus='0',dOperatorTime='" + serverTime + "' where vcBoxNo='" + caseNo + "' and vcOperatorID='" + opearteId + "'");
            return excute.ExcuteSqlWithStringOper(UpdateCaseSql.ToString());
        }

        public DataTable GetPoint2(string iP)
        {
            StringBuilder GetPoingSql = new StringBuilder();
            GetPoingSql.Append("select vcPointNo from TPointInfo where vcPointIp='" + iP + "' and vcUsed='在用' and vcPlant='H2'");
            return excute.ExcuteSqlWithSelectToDT(GetPoingSql.ToString());
        }

        public int UpdatePoint1(string pointNo)
        {
            StringBuilder UpdatePointSql = new StringBuilder();
            UpdatePointSql.Append("update TPointState set vcState='未登录' where vcPointNo='" + pointNo + "' and vcPlant='H2'");
            return excute.ExcuteSqlWithStringOper(UpdatePointSql.ToString());
        }

        public int InsertPoint1(string pointNo)
        {
            StringBuilder InsertPointSql = new StringBuilder();
            InsertPointSql.Append("INSERT INTO TPointState (vcPlant,vcPointNo,vcState,vcOperater,decEfficacy) VALUES ('H2','" + pointNo + "','未登录','',null)");
            return excute.ExcuteSqlWithStringOper(InsertPointSql.ToString());
        }

        public DataTable GetPointStatus4(string pointNo)
        {
            StringBuilder GetPointStatusSql = new StringBuilder();
            GetPointStatusSql.Append("select vcState from TPointState where vcPlant='H2' and vcPointNo='" + pointNo + "'");
            return excute.ExcuteSqlWithSelectToDT(GetPointStatusSql.ToString());
        }

        public DataTable GetTrolleyInfo(string trolley, string iP, string lotId)
        {
            StringBuilder GetTrolleyInfoSql = new StringBuilder();
            GetTrolleyInfoSql.Append("select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR from TOperatorQB where vcZYType='S0' and vcTrolleySeqNo='" + lotId + "' and vcHostIp='" + iP + "' and vcTrolleyNo='" + trolley + "' and vcReflectFlag='0' order by dOperatorTime desc");

            return excute.ExcuteSqlWithSelectToDT(GetTrolleyInfoSql.ToString());
        }

        public int DeleteKanban(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string iP)
        {
            StringBuilder DeleteKanbanSql = new StringBuilder();
            DeleteKanbanSql.Append("update TOperatorQB set vcReflectFlag='4'  where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcHostIp='" + iP + "' and vcZYType='S0' and vcReflectFlag='0'");
            return excute.ExcuteSqlWithStringOper(DeleteKanbanSql.ToString());
        }

        public DataTable GetPackingSpot(string iP)
        {
            StringBuilder GetPackingSpotSql = new StringBuilder();
            GetPackingSpotSql.Append("  select distinct(vcBZPlant) from TOperatorQB where vcZYType='S0'  and vcHostIp='" + iP + "'");
            return excute.ExcuteSqlWithSelectToDT(GetPackingSpotSql.ToString());
        }

        public int DeleteTrolley(string trolley, string iP, string lotId)
        {
            StringBuilder DeleteTrolleySql = new StringBuilder();
            DeleteTrolleySql.Append(" update TOperatorQB set vcReflectFlag='4' where vcHostIp='" + iP + "' and vcTrolleyNo='" + trolley + "' and vcZYType='S0' and vcTrolleySeqNo='" + lotId + "' and vcReflectFlag='0'");
            return excute.ExcuteSqlWithStringOper(DeleteTrolleySql.ToString());
        }



        public DataTable GetSum(string iP)
        {
            StringBuilder GetSumSql = new StringBuilder();
            GetSumSql.Append("select vcTrolleyNo, count(vcTrolleyNo) as kanbanSum, vcTrolleySeqNo  from TOperatorQB   where vcZYType = 'S0' and vcReflectFlag = '0' and vcHostIp = '" + iP + "'  group by vcTrolleyNo, vcTrolleySeqNo  order by vcTrolleySeqNo desc");
            return excute.ExcuteSqlWithSelectToDT(GetSumSql.ToString());
        }

        public DataTable ValidateQB(string trolley)
        {
            StringBuilder ValidateQBSql = new StringBuilder();
            ValidateQBSql.Append("select vcPart_id from TOperatorQB where vcTrolleyNo='" + trolley + "' and vcReflectFlag='0'");
            return excute.ExcuteSqlWithSelectToDT(ValidateQBSql.ToString());
        }

        public int UpdateQB(string lotId, string iP, string trolley)
        {
            StringBuilder UpdateQBSql = new StringBuilder();
            UpdateQBSql.Append(" update TOperatorQB set vcLotid = '" + lotId + "' where vcTrolleyNo = '" + trolley + "' and vcHostIp = '" + iP + "' and vcLotid = ''");


            return excute.ExcuteSqlWithStringOper(UpdateQBSql.ToString());
        }

        public int InsertSeqNo(string packingSpot, string serverTime, string tmpString)
        {
            StringBuilder InsertSeqNoSql = new StringBuilder();
            InsertSeqNoSql.Append("INSERT INTO TSeqNo(FLAG ,DDATE ,SEQNO) \n");
            InsertSeqNoSql.Append("VALUES( '" + tmpString + "'+'" + packingSpot + "','" + serverTime + "','1')");
            return excute.ExcuteSqlWithStringOper(InsertSeqNoSql.ToString());
        }

        public int InsertInv(string packingSpot, string inno, string partId, string cpdCompany, string quantity, string serverTime, string kanbanOrderNo, string kanbanSerial, string scanTime, string opearteId)
        {
            StringBuilder InsertInvSql = new StringBuilder();
            InsertInvSql.Append("INSERT INTO TOperateSJ_InOutput(vcBZPlant,vcSHF,vcInputNo,vcKBOrderNo,vcKBLFNo ,vcPart_id ,iQuantity");
            InsertInvSql.Append(" ,iDBZ,iDZX,iDCH,dInDate,vcOperatorID,dOperatorTime)");
            InsertInvSql.Append("     VALUES('" + packingSpot + "','" + cpdCompany + "','" + inno + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "'," + quantity + "," + quantity + ",0,0,'" + scanTime + "','" + opearteId + "','" + serverTime + "')");
            return excute.ExcuteSqlWithStringOper(InsertInvSql.ToString());
        }

        public DataTable GetQBData(string iP)
        {
            StringBuilder getQBDataSql = new StringBuilder();
            getQBDataSql.Append("  select vcTrolleyNo,vcInputNo,vcPart_id,vcCpdCompany,vcSR,iQuantity,vcKBOrderNo,vcKBLFNo,dScanTime,vcBZPlant,vcSupplierId,vcSupplierPlant,vcLabelStart,vcLabelEnd,iPackingQty,vcLotid,vcCheckType,vcIOType,vcKanBan from TOperatorQB  where vcHostIp='" + iP + "' and vcReflectFlag='0' and vcZYType='S0'");
            return excute.ExcuteSqlWithSelectToDT(getQBDataSql.ToString());
        }

        public int UpdateOpr(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder UpdateOprSql = new StringBuilder();
            UpdateOprSql.Append("update TOperatorQB set vcReflectFlag='1' where vcPart_id='" + partId + "' and vcSR='" + dock + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");
            return excute.ExcuteSqlWithStringOper(UpdateOprSql.ToString());
        }

        public DataTable ValidateSJ(string partId, string kanbanOrderNo, string kanbanSerial, string inputNo)
        {
            StringBuilder validateSJSql = new StringBuilder();
            validateSJSql.Append("select * from TOperateSJ_InOutput where vcInputNo = '" + inputNo + "' and vcPart_id = '" + partId + "' and vcKBOrderNo = '" + kanbanOrderNo + "' and vcKBLFNo = '" + kanbanSerial + "'");
            return excute.ExcuteSqlWithSelectToDT(validateSJSql.ToString());
        }



        public DataTable GetPackItem(string scanTime, string partId)
        {
            StringBuilder getPackItemSql = new StringBuilder();

            getPackItemSql.Append("select vcPackNo,iBiYao from TPackItem where vcPartsNo='" + partId + "' and dUsedFrom<='" + scanTime + "' and dUsedTo>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(getPackItemSql.ToString());
        }

        public DataTable ValidateOrd1(string partId)
        {
            StringBuilder validateOrdSql = new StringBuilder();
            validateOrdSql.Append(" select  vcTargetYearMonth,vcOrderType,vcOrderNo,vcSeqno,(CAST(ISNULL(vcPlantQtyDaily1,0) as int)-CAST(ISNULL(vcInputQtyDaily1,0) as int)) as day1 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily2,0) as int)-CAST(ISNULL(vcInputQtyDaily2,0) as int)) as day2 ,(CAST(ISNULL(vcPlantQtyDaily3,0) as int)-CAST(ISNULL(vcInputQtyDaily3,0) as int)) as day3 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily4,0) as int)-CAST(ISNULL(vcInputQtyDaily4,0) as int)) as day4 ,(CAST(ISNULL(vcPlantQtyDaily5,0) as int)-CAST(ISNULL(vcInputQtyDaily5,0) as int)) as day5 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily6,0) as int)-CAST(ISNULL(vcInputQtyDaily6,0) as int)) as day6 ,(CAST(ISNULL(vcPlantQtyDaily7,0) as int)-CAST(ISNULL(vcInputQtyDaily7,0) as int)) as day7 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily8,0) as int)-CAST(ISNULL(vcInputQtyDaily8,0) as int)) as day8 ,(CAST(ISNULL(vcPlantQtyDaily9,0) as int)-CAST(ISNULL(vcInputQtyDaily9,0) as int)) as day9 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily10,0) as int)-CAST(ISNULL(vcInputQtyDaily10,0) as int)) as day10 ,(CAST(ISNULL(vcPlantQtyDaily11,0) as int)-CAST(ISNULL(vcInputQtyDaily11,0) as int)) as day11 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily12,0) as int)-CAST(ISNULL(vcInputQtyDaily12,0) as int)) as day12 ,(CAST(ISNULL(vcPlantQtyDaily13,0) as int)-CAST(ISNULL(vcInputQtyDaily13,0) as int)) as day13 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily14,0) as int)-CAST(ISNULL(vcInputQtyDaily14,0) as int)) as day14 ,(CAST(ISNULL(vcPlantQtyDaily15,0) as int)-CAST(ISNULL(vcInputQtyDaily15,0) as int)) as day15 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily16,0) as int)-CAST(ISNULL(vcInputQtyDaily16,0) as int)) as day16 ,(CAST(ISNULL(vcPlantQtyDaily17,0) as int)-CAST(ISNULL(vcInputQtyDaily17,0) as int)) as day17 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily18,0) as int)-CAST(ISNULL(vcInputQtyDaily18,0) as int)) as day18 ,(CAST(ISNULL(vcPlantQtyDaily19,0) as int)-CAST(ISNULL(vcInputQtyDaily19,0) as int)) as day19 ,   \n");
            validateOrdSql.Append("   (CAST(ISNULL(vcPlantQtyDaily20,0) as int)-CAST(ISNULL(vcInputQtyDaily20,0) as int)) as day20 ,(CAST(ISNULL(vcPlantQtyDaily21,0) as int)-CAST(ISNULL(vcInputQtyDaily21,0) as int)) as day21 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily22,0) as int)-CAST(ISNULL(vcInputQtyDaily22,0) as int)) as day22 ,(CAST(ISNULL(vcPlantQtyDaily23,0) as int)-CAST(ISNULL(vcInputQtyDaily23,0) as int)) as day23 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily24,0) as int)-CAST(ISNULL(vcInputQtyDaily24,0) as int)) as day24 ,(CAST(ISNULL(vcPlantQtyDaily25,0) as int)-CAST(ISNULL(vcInputQtyDaily25,0) as int)) as day25 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily26,0) as int)-CAST(ISNULL(vcInputQtyDaily26,0) as int)) as day26 ,(CAST(ISNULL(vcPlantQtyDaily27,0) as int)-CAST(ISNULL(vcInputQtyDaily27,0) as int)) as day27 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily28,0) as int)-CAST(ISNULL(vcInputQtyDaily28,0) as int)) as day28 ,(CAST(ISNULL(vcPlantQtyDaily29,0) as int)-CAST(ISNULL(vcInputQtyDaily29,0) as int)) as day29 ,   \n");
            validateOrdSql.Append("   (CAST(ISNULL(vcPlantQtyDaily30,0) as int)-CAST(ISNULL(vcInputQtyDaily30,0) as int)) as day30 ,(CAST(ISNULL(vcPlantQtyDaily31,0) as int)-CAST(ISNULL(vcInputQtyDaily31,0) as int)) as day31    \n");
            validateOrdSql.Append("  from  SP_M_ORD where vcPartNo='" + partId + "' and vcOrderNo!=''   \n");
            return excute.ExcuteSqlWithSelectToDT(validateOrdSql.ToString());
        }



        public DataSet CheckDb(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string scanTime)
        {
            StringBuilder CheckDbSql = new StringBuilder();

            CheckDbSql.AppendLine("select * from TOperateSJ where vcSR='" + dock + "' and vcPart_id = '" + partId + "' and vcKBOrderNo = '" + kanbanOrderNo + "' and vcKBLFNo = '" + kanbanSerial + "'");
            CheckDbSql.AppendLine("select  vcBZQF, vcRHQF, vcBZUnit from TPackageMaster where vcPart_id = '" + partId + "' and dTimeFrom <= '" + scanTime + "' and dTimeTo >= '" + scanTime + "' and vcBZQF is not null and vcRHQF is not null and vcBZUnit is not null");
            CheckDbSql.AppendLine("select vcReceiver, vcPackingPlant from TSPMaster_SufferIn where vcPackingPlant = 'TFTM' and vcPartId = '" + partId + "' and  dFromTime <= '" + scanTime + "' and dToTime >= '" + scanTime + "' and vcSufferIn = '" + dock + "'");
            CheckDbSql.AppendLine("select iPackingQty,vcSupplierId,vcSupplierPlant from TSPMaster_Box where vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");
            CheckDbSql.AppendLine("select vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace	 from TSPMaster WHERE vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");
            CheckDbSql.AppendLine("select vcPackNo,iBiYao from TPackItem where vcPartsNo='" + partId + "' and dUsedFrom<='" + scanTime + "' and dUsedTo>='" + scanTime + "'");
            CheckDbSql.AppendLine("select vcPartNameCN, vcSCSName, vcSCSAdress, vcZXBZNo from TtagMaster where vcPart_Id = '" + partId + "' and dTimeFrom <= '" + scanTime + "' and dTimeTo >= '" + scanTime + "'");
            CheckDbSql.AppendLine("select (ISNULL(SUM(CAST(vcPlantQtyDailySum as int)),0)-ISNULL(SUM(CAST(vcInputQtyDailySum as int)),0)) as sum from SP_M_ORD where ISNULL(vcPlantQtyDailySum,0)!=ISNULL(vcInputQtyDailySum,0) and vcPartNo='" + partId + "' and vcOrderNo!=''");
            CheckDbSql.AppendLine("select ISNULL(sum(iQuantity), 0) as sum from TOperatorQB where vcZYType = 'S0' and vcPart_id = '" + partId + "' and vcReflectFlag = '0'");
            return excute.ExcuteSqlWithSelectToDS(CheckDbSql.ToString());
        }

        public DataTable GetQBData1(string iP)
        {
            StringBuilder GetQBDataSql = new StringBuilder();
            GetQBDataSql.Append(" select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR,count(*) from TOperatorQB where  vcZYType='S0' and vcReflectFlag='0' and vcHostIp='" + iP + "' group by vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR having count(*)>=2");
            return excute.ExcuteSqlWithSelectToDT(GetQBDataSql.ToString());
        }

        public int UpdatePack2(string iP, string serverTime)
        {
            StringBuilder UpdatePackSql = new StringBuilder();
            UpdatePackSql.Append("update TPackList set dFirstPrintTime='" + serverTime + "' where vcHostip='" + iP + "' and dFirstPrintTime is null");
            return excute.ExcuteSqlWithStringOper(UpdatePackSql.ToString());
        }

        public DataTable GetPrintName(string iP)
        {
            StringBuilder GetPrintNameSql = new StringBuilder();
            GetPrintNameSql.Append("select vcPrinterName from TPrint where vcPrinterIp='" + iP + "' and vcKind='LABEL PRINTER'");
            return excute.ExcuteSqlWithSelectToDT(GetPrintNameSql.ToString());
        }

        public DataTable GetData1(string trolley1, string trolleySeqNo1, string iP)
        {
            StringBuilder GetDataSql = new StringBuilder();
            GetDataSql.Append("select count(*) from TOperatorQB where vcTrolleyNo = '" + trolley1 + "' and vcTrolleySeqNo = '" + trolleySeqNo1 + "' and vcHostIp = '" + iP + "' and vcReflectFlag  = '0'  ");
            return excute.ExcuteSqlWithSelectToDT(GetDataSql.ToString());
        }

        public DataTable GetData(string trolley1, string trolleySeqNo1, string iP)
        {
            StringBuilder GetDataSql = new StringBuilder();
            GetDataSql.Append("select top(1) vcReflectFlag from TOperatorQB where vcTrolleyNo = '" + trolley1 + "' and vcTrolleySeqNo = '" + trolleySeqNo1 + "' and vcHostIp = '" + iP + "' order by iAutoId desc");
            return excute.ExcuteSqlWithSelectToDT(GetDataSql.ToString());
        }

        public DataTable GetSeqNo2(string iP, string kanbanOrderNo, string kanbanSerial, string dock, string partId)
        {
            StringBuilder GetSeqNoSql = new StringBuilder();
            GetSeqNoSql.Append("select vcTrolleySeqNo, vcTrolleyNo from TOperatorQB where vcPart_id = '" + partId + "' and vcKBOrderNo = '" + kanbanOrderNo + "' and vcKBLFNo = '" + kanbanSerial + "' and vcSR = '" + dock + "' and vcReflectFlag = '4' and vcHostIp = '" + iP + "' order by iAutoId desc");//问题区域
            return excute.ExcuteSqlWithSelectToDT(GetSeqNoSql.ToString());
        }

        public int UpdateTrolley3(string trolley, string trolleySeqNo, string iP)
        {
            StringBuilder UpdateTrolleySql = new StringBuilder();
            UpdateTrolleySql.Append("update TOperateSJ_TrolleyInfo set vcStatus='4' where vcTrolleyNo='" + trolley + "' and vcTrolleySeqNo='" + trolleySeqNo + "' and vcHostIp='" + iP + "'");
            return excute.ExcuteSqlWithStringOper(UpdateTrolleySql.ToString());
        }

        public DataTable GetQB(string trolleySeqNo, string iP, string trolley)
        {
            StringBuilder GetQBSql = new StringBuilder();
            GetQBSql.Append("select vcPart_id from TOperatorQB where vcZYType='S0' and vcReflectFlag='0' and vcTrolleyNo='" + trolley + "' and vcTrolleySeqNo='" + trolleySeqNo + "' and vcHostIp='" + iP + "'");

            return excute.ExcuteSqlWithSelectToDT(GetQBSql.ToString());
        }

        public DataTable GetSeqNoSql(string iP, string kanbanOrderNo, string kanbanSerial, string dock, string partId)
        {
            StringBuilder GetSeqNoSql = new StringBuilder();
            GetSeqNoSql.Append("select vcTrolleySeqNo,vcTrolleyNo from TOperatorQB where vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcReflectFlag='4' and vcHostIp='" + iP + "' order by iAutoId desc ");
            return excute.ExcuteSqlWithSelectToDT(GetSeqNoSql.ToString());
        }

        public int UpdateInv(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string inno, string iP)
        {
            StringBuilder UpdateInvSql = new StringBuilder();

            UpdateInvSql.Append("update TOperatorQB set vcInputNo='" + inno + "' where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "'\n");
            UpdateInvSql.Append("and vcReflectFlag='0' and vcHostIp='" + iP + "' and vcZYType='S0'\n");
            return excute.ExcuteSqlWithStringOper(UpdateInvSql.ToString());
        }

        public DataTable GetInv(string iP)
        {
            StringBuilder GetInvSql = new StringBuilder();
            GetInvSql.Append("select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR from TOperatorQB where vcHostIp='" + iP + "' and vcZYType='S0' and vcReflectFlag='0'");
            return excute.ExcuteSqlWithSelectToDT(GetInvSql.ToString());
        }

        public int UpdateLabel2(string lblSart, string lblEnd, string iP, string partId, string kanbanOrderNo, string kanbanSerial, string dock)
        {
            StringBuilder UpdateLabelSql = new StringBuilder();
            UpdateLabelSql.Append("update TOperatorQB set vcLabelStart='" + lblSart + "',vcLabelEnd='" + lblEnd + "' where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'\n");
            UpdateLabelSql.Append("and vcSR='" + dock + "' and vcHostIp='" + iP + "' and vcReflectFlag='0' and vcZYType='S0'\n");
            return excute.ExcuteSqlWithStringOper(UpdateLabelSql.ToString());
        }

        public DataTable GetID(string iP, string serverTime)
        {
            StringBuilder GetIDSql = new StringBuilder();
            GetIDSql.Append("select t1.vcPart_id,t1.vcKBOrderNo,t1.vcKBLFNo,t1.vcSR,t1.iQuantity,isnull(t2.vcBZUnit,0) from TOperatorQB t1,TPackageMaster t2\n");
            GetIDSql.Append("where t1.vcPart_id=t2.vcPart_id and t2.vcBZQF='1' and t2.dTimeFrom<='" + serverTime + "' and t2.dTimeTo>='" + serverTime + "' and \n");
            GetIDSql.Append("t1.vcReflectFlag='0' and t1.vcZYType='S0' and vcHostIp='" + iP + "' order by t1.dOperatorTime\n");
            return excute.ExcuteSqlWithSelectToDT(GetIDSql.ToString());
        }

        public int UpdateTrolley2(string trolleySeqNo, string lotId, string iP)
        {
            StringBuilder UpdateTrolleySql = new StringBuilder();
            UpdateTrolleySql.Append("update TOperatorQB set vcLotid='" + lotId + "' where vcTrolleySeqNo='" + trolleySeqNo + "' and vcHostIp='" + iP + "' and vcReflectFlag='0'");
            return excute.ExcuteSqlWithStringOper(UpdateTrolleySql.ToString());
        }

        public DataTable GetLotInfo(string iP)
        {
            StringBuilder GetLotInfoSql = new StringBuilder();
            GetLotInfoSql.Append("select vcTrolleySeqNo from TOperatorQB where vcHostIp='" + iP + "' and vcZYType='S0' and vcReflectFlag='0' group by vcTrolleySeqNo order by vcTrolleySeqNo");
            return excute.ExcuteSqlWithSelectToDT(GetLotInfoSql.ToString());
        }

        public int UpdateQB(string iP)
        {
            StringBuilder UpdateQBSql = new StringBuilder();
            UpdateQBSql.Append("update TOperatorQB set vcReflectFlag='1' where vcHostIp='" + iP + "' and vcReflectFlag='0'");
            return excute.ExcuteSqlWithStringOper(UpdateQBSql.ToString());
        }

        public DataTable ValidatePrint(string iP)
        {
            StringBuilder ValidatePrintSql = new StringBuilder();
            ValidatePrintSql.Append(" select * from TLabelList t1,TSPMaster t2 where t1.dFirstPrintTime is null and  t1.vcPart_id=t2.vcPartId and t2.vcSupplierId in ('TF1R','TF2R','TF3R') and  t1.dFirstPrintTime is null  and t1.vcHostip='" + iP + "' ");
            return excute.ExcuteSqlWithSelectToDT(ValidatePrintSql.ToString());
        }

        public int InsertLbl(string partsNameEn, string partId, string cpdCompany, string quantity, string printCount, string supplierName, string supplierAddress, string carFamilyCode, string opearteId, string scanTime, string iP, string partsNameCn, string inputNo, byte[] vs, string printCount1, byte[] vs2, string excuteStand, string packingQuatity, string partId1)
        {
            /*
                  StringBuilder insertLblSql = new StringBuilder();
                  insertLblSql.Append("INSERT INTO dbo.TLabelList(vcPartsnameen,vcPart_id,vcInno,vcCpdcompany,vcLabel,vcGetnum \n");
                  insertLblSql.Append(" ,iDateprintflg,vcComputernm,vcPrindate,iQrcode,vcPrintcount,vcPartnamechineese \n");
                  insertLblSql.Append(",vcSuppliername,vcSupplieraddress,vcExecutestandard,vcCartype,vcHostip,vcOperatorID \n");
                  insertLblSql.Append(" ,dOperatorTime) VALUES('" + partsNameEn + "','" + partId + "','" + inputNo + "','" + cpdCompany + "','" + printCount + "','" + quantity + "', \n");
                  insertLblSql.Append("null,'','','','" + printCount + "','" + partsNameCn + "','" + supplierName + "','" + supplierAddress + "','','" + carFamilyCode + "','" + iP + "','" + opearteId + "','" + scanTime + "')\n");
                  return excute.ExcuteSqlWithStringOper(insertLblSql.ToString());
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
                strSql_sub.Append("INSERT INTO [dbo].[TLabelList]");
                strSql_sub.Append("           ([vcPartsnameen]");
                strSql_sub.Append("           ,[vcPart_id]");
                strSql_sub.Append("           ,[vcPart_id1]");
                strSql_sub.Append("           ,[vcInno]");
                strSql_sub.Append("           ,[vcCpdcompany]");
                strSql_sub.Append("           ,[vcLabel]");
                strSql_sub.Append("           ,[vcLabel1]");
                strSql_sub.Append("           ,[vcGetnum]");
                strSql_sub.Append("           ,[iDateprintflg]");
                strSql_sub.Append("           ,[vcComputernm]");
                strSql_sub.Append("           ,[vcPrindate]");
                strSql_sub.Append("           ,[iQrcode]");
                strSql_sub.Append("           ,[iQrcode1]");
                strSql_sub.Append("           ,[vcPrintcount]");
                strSql_sub.Append("           ,[vcPrintcount1]");
                strSql_sub.Append("           ,[vcPartnamechineese]");
                strSql_sub.Append("           ,[vcSuppliername]");
                strSql_sub.Append("           ,[vcSupplieraddress]");
                strSql_sub.Append("           ,[vcExecutestandard]");
                strSql_sub.Append("           ,[vcCartype]");
                strSql_sub.Append("           ,[vcHostip]");
                strSql_sub.Append("           ,[vcOperatorID]");
                strSql_sub.Append("           ,[dOperatorTime]");
                strSql_sub.Append("           ,[dFirstPrintTime]");
                strSql_sub.Append("           ,[dLatelyPrintTime])");
                strSql_sub.Append("     VALUES");

                strSql_sub.AppendLine("           ('" + partsNameEn + "'");
                strSql_sub.AppendLine("           ,'" + partId + "'");
                strSql_sub.AppendLine("           ,'" + partId1 + "'");
                strSql_sub.AppendLine("           ,'" + inputNo + "'");
                strSql_sub.AppendLine("           ,'" + cpdCompany + "'");
                strSql_sub.AppendLine("           ,'" + ("*" + printCount + "*") + "'");
                strSql_sub.AppendLine("           ,'" + ("*" + printCount1 + "*") + "'");
                strSql_sub.AppendLine("           ,'" + packingQuatity + "'");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,@iQrcode");
                strSql_sub.AppendLine("           ,@iQrcode1");
                strSql_sub.AppendLine("           ,'" + printCount + "'");
                strSql_sub.AppendLine("           ,'" + printCount1 + "'");
                strSql_sub.AppendLine("           ,'" + partsNameCn + "'");
                strSql_sub.AppendLine("           ,'" + supplierName + "'");
                strSql_sub.AppendLine("           ,'" + supplierAddress + "'");
                strSql_sub.AppendLine("           ,'" + excuteStand + "'");
                strSql_sub.AppendLine("           ,'" + carFamilyCode + "'");
                strSql_sub.AppendLine("           ,'" + iP + "'");
                strSql_sub.AppendLine("           ,'" + opearteId + "'");
                strSql_sub.AppendLine("           ,'" + scanTime + "'");
                strSql_sub.AppendLine("           ,null");
                strSql_sub.AppendLine("           ,null)");
                sqlCommand_sub.CommandText = strSql_sub.ToString();
                sqlCommand_sub.Parameters.Add("@iQrcode", SqlDbType.Image);
                sqlCommand_sub.Parameters.Add("@iQrcode1", SqlDbType.Image);
                sqlCommand_sub.Parameters["@iQrcode"].Value = vs;
                sqlCommand_sub.Parameters["@iQrcode1"].Value = vs2;
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

        public DataTable GetPointNo(string iP)
        {
            StringBuilder GetPointNoSql = new StringBuilder();
            GetPointNoSql.Append("select vcPointType,vcPointNo from TPointInfo where vcPointIp='" + iP + "' and vcUsed='在用'");
            return excute.ExcuteSqlWithSelectToDT(GetPointNoSql.ToString());
        }

        public DataTable GetTrolleyInfo1(string trolley, string iP, string opearteId)
        {
            StringBuilder GetTrolleyInfoSql = new StringBuilder();
            GetTrolleyInfoSql.Append("select * from TOperateSJ_TrolleyInfo where vcStatus='0' and vcTrolleyNo='" + trolley + "' and vcHostIp='" + iP + "' and vcOperatorID='" + opearteId + "'");
            return excute.ExcuteSqlWithSelectToDT(GetTrolleyInfoSql.ToString());
        }

        public DataTable GetTrolleyInfo(string trolley)
        {
            throw new NotImplementedException();
        }

        public DataTable ValidateQB5(string lotIdNew)
        {
            StringBuilder ValidateQBSql = new StringBuilder();
            ValidateQBSql.Append("select * from TOperatorQB where vcLotid='" + lotIdNew + "' and vcReflectFlag='1'");
            return excute.ExcuteSqlWithSelectToDT(ValidateQBSql.ToString());
        }

        public int UpdateTrolley1(string iP, string opearteId, string trolley, string lotId)
        {
            StringBuilder UpdateTrolleySql = new StringBuilder();
            UpdateTrolleySql.Append("update TOperateSJ_TrolleyInfo set vcStatus='4' where vcTrolleySeqNo='" + lotId + "' and vcTrolleyNo='" + trolley + "' and vcStatus='0' and vcOperatorID='" + opearteId + "' and vcHostIp='" + iP + "'");
            return excute.ExcuteSqlWithStringOper(UpdateTrolleySql.ToString());
        }

        public int UpdateLabel1(string iP, string serverTime)
        {
            StringBuilder UpdateLabelSql = new StringBuilder();
            UpdateLabelSql.Append("update TLabelList set dFirstPrintTime='" + serverTime + "' where vcHostip='" + iP + "'");
            return excute.ExcuteSqlWithStringOper(UpdateLabelSql.ToString());
        }

        public int UpdatePack(string iP, string minLabel, string maxLabel, string lotIdNew)
        {
            StringBuilder UpdatePackSql = new StringBuilder();
            UpdatePackSql.Append("update TPackList set vcLabelStart='" + minLabel + "',vcLabelEnd='" + maxLabel + "' where   vcLotid='" + lotIdNew + "' and  vcHostip='" + iP + "'");
            return excute.ExcuteSqlWithStringOper(UpdatePackSql.ToString());
        }

        public DataTable GetLabel(string iP, string lotIdNew)
        {
            StringBuilder GetLabelSql = new StringBuilder();
            GetLabelSql.Append("select MIN(vcLabelStart) as minLabel,MAX(vcLabelEnd) as maxLabel from TPackList  where vcLabelEnd <>'' and vcLabelEnd <>'' and  vcLotid='" + lotIdNew + "'  and vcHostip='" + iP + "'");
            return excute.ExcuteSqlWithSelectToDT(GetLabelSql.ToString());
        }

        public int UpdateTrolley(string opearteId, string iP)
        {
            StringBuilder UpdateTrolleySql = new StringBuilder();
            UpdateTrolleySql.Append("  update TOperateSJ_TrolleyInfo set vcStatus='1' where vcOperatorID='" + opearteId + "' and vcHostIp='" + iP + "'");
            return excute.ExcuteSqlWithStringOper(UpdateTrolleySql.ToString());
        }

        public int UpdateTrolley(string trolley, string opearteId, string serverTime, string iP, string lotId)
        {
            StringBuilder UpdateTrolleySql = new StringBuilder();
            UpdateTrolleySql.Append("update TOperateSJ_TrolleyInfo set vcLotid='" + lotId + "',dOperatorTime='" + serverTime + "',vcHostIp='" + iP + "' where vcTrolleyNo='" + trolley + "' and vcOperatorID='" + opearteId + "' ");
            return excute.ExcuteSqlWithStringOper(UpdateTrolleySql.ToString());
        }

        public int InsertTrolley(string trolley, string opearteId, string serverTime, string iP, string lotId)
        {
            StringBuilder InsertTrolleySql = new StringBuilder();
            InsertTrolleySql.Append("INSERT INTO  TOperateSJ_TrolleyInfo  (vcSheBeiNo,vcHostIp,vcTrolleyNo,vcStatus,vcOperatorID,dOperatorTime,vcLotid)\n");
            InsertTrolleySql.Append("     VALUES ('','" + iP + "','" + trolley + "','0','" + opearteId + "','" + serverTime + "','" + lotId + "')\n");
            return excute.ExcuteSqlWithStringOper(InsertTrolleySql.ToString());
        }

        public DataTable ValidateTrolley(string trolley, string opearteId, string iP)
        {
            StringBuilder ValidateTrolleySql = new StringBuilder();
            ValidateTrolleySql.Append("  select vcTrolleyNo from TOperateSJ_TrolleyInfo where vcHostIp='" + iP + "' and vcTrolleyNo='" + trolley + "' and vcStatus='0' and vcOperatorID='" + opearteId + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateTrolleySql.ToString());
        }

        public int UpdateLabel(string iP, string serverTime)
        {
            StringBuilder UpdateLabelSql = new StringBuilder();
            UpdateLabelSql.Append("update TLabelList set dFirstPrintTime='" + serverTime + "' from TSPMaster where TLabelList.vcPart_id=TSPMaster.vcPartId and TSPMaster.vcSupplierId in ('TF1R','TF2R','TF3R') and TLabelList.vcHostip='" + iP + "'");

            return excute.ExcuteSqlWithStringOper(UpdateLabelSql.ToString());
        }

        public int UpdatePrint1(string iP)
        {
            StringBuilder UpdatePrintSql = new StringBuilder();
            UpdatePrintSql.Append("update TPrint_Temp set vcFlag='1' where vcTableName='TLabelList' and vcClientIP='" + iP + "' and vcKind=2 and vcFlag='0'");
            return excute.ExcuteSqlWithStringOper(UpdatePrintSql.ToString());
        }

        public int UpdatePrint(string iP)
        {
            StringBuilder UpdatePrintSql = new StringBuilder();

            UpdatePrintSql.Append("update TPrint_Temp set vcFlag='1' where vcFlag='0' and vcTableName='TPackList' and vcLotid!='' and vcKind=1 and vcClientIP='" + iP + "'");


            return excute.ExcuteSqlWithStringOper(UpdatePrintSql.ToString());
        }

        public int UpdatePack1(string iP, string serverTime)
        {
            StringBuilder UpdatePackSql = new StringBuilder();
            UpdatePackSql.Append("  update TPackList set dFirstPrintTime='" + serverTime + "' where vcHostip='" + iP + "'");


            return excute.ExcuteSqlWithStringOper(UpdatePackSql.ToString());
        }

        public int InsertInvList(string data1, string printDate, string inputNo, string partId, string partsNameEn, string quantity, string packingQuantity, string itemname1, string packLocation1, string suppName1, string outNum1, string itemname2, string packLocation2, string suppName2, string outNum2, string itemname3, string packLocation3, string suppName3, string outNum3, string itemname4, string packLocation4, string suppName4, string outNum4, string itemname5, string packLocation5, string suppName5, string outNum5, string itemname6, string packLocation6, string suppName6, string outNum6, string itemname7, string packLocation7, string suppName7, string outNum7, string itemname8, string packLocation8, string suppName8, string outNum8, string itemname9, string packLocation9, string suppName9, string outNum9, string itemname10, string packLocation10, string suppName10, string outNum10, string partsAndNum, string cpdCompany, string opearteId, string scanTime, byte[] vs1)
        {
            /*



            StringBuilder InsertInvList = new StringBuilder();
            InsertInvList.Append("INSERT INTO TInvList (vcNo,vcData,vcPrintdate,vcInno,vcPart_Id,vcPartsnamechn,vcPartslocation,vcInputnum,vcPackingquantity,vcItemname1,vcPackingpartslocation1,vcSuppliernamechn1\n");
            InsertInvList.Append(",vcOutnum1,vcTemname2,vcPackingpartslocation2,vcSuppliernamechn2,vcOutnum2,vcItemname3,vcPackingpartslocation3,vcSuppliernamechn3,vcOutnum3,vcItemname4,vcPackingpartslocation4,\n");
            InsertInvList.Append("vcSuppliernamechn4,vcOutnum4,vcItemname5,vcPackingpartslocation5,vcSuppliernamechn5,vcOutnum5,vcItemname6,vcPackingpartslocation6,vcSuppliernamechn6,vcOutnum6,vcItemname7\n");
            InsertInvList.Append(",vcPackingpartslocation7,vcSuppliernamechn7,vcOutnum7,vcItemname8,vcPackingpartslocation8,vcSuppliernamechn8,vcOutnum8,vcItemname9,vcPackingpartslocation9,vcSuppliernamechn9\n");
            InsertInvList.Append(",vcOutnum9,vcItemname10,vcPackingpartslocation10,vcSuppliernamechn10,vcOutnum10,vcPartsnoandnum,vcLabel,vcComputernm,vcCpdcompany,vcPlantcode\n");
            InsertInvList.Append(",vcCompanyname,vcPlantname,iQrcode,vcOperatorID,dOperatorTime,dFirstPrintTime,dLatelyPrintTime) VALUES ( '','"+data1+ "','"+printDate+ "','"+inputNo+ "','"+partId+ "','"+partsNameEn+ "','','"+quantity+ "','"+packingQuantity+ "','"+itemname1+"','"+ packLocation1 + "','"+ suppName1 + "','"+ outNum1 + "',\n");
            InsertInvList.Append("'" + itemname2 + "','" + packLocation2 + "','" + suppName2 + "','" + outNum2 + "','" + itemname3 + "','" + packLocation3 + "','" + suppName3 + "','" + outNum3 + "','" + itemname4 + "','" + packLocation4 + "','" + suppName4 + "','" + outNum4 + "','" + itemname5 + "','" + packLocation5 + "','" + suppName5 + "','" + outNum5 + "','" + itemname6 + "','" + packLocation6 + "','" + suppName6 + "'\n");
            InsertInvList.Append(",'" + outNum6 + "','" + itemname7 + "','" + packLocation7 + "','" + suppName7 + "','" + outNum7 + "','" + itemname8 + "','" + packLocation8 + "','" + suppName8 + "','" + outNum8 + "','" + itemname9 + "','" + packLocation9 + "','" + suppName9 + "','" + outNum9 + "','" + itemname10 + "','" + packLocation10 + "','" + suppName10 + "','" + outNum10 + "','"+partsAndNum+"','','','"+cpdCompany+"','','','','','"+opearteId+"','"+scanTime+"',null,null)\n");



            return excute.ExcuteSqlWithStringOper(InsertInvList.ToString());
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
                strSql_sub.Append("INSERT INTO [dbo].[TInvList]");
                strSql_sub.Append("           ([vcNo]");
                strSql_sub.Append("           ,[vcData]");
                strSql_sub.Append("           ,[vcPrintdate]");
                strSql_sub.Append("           ,[vcInno]");
                strSql_sub.Append("           ,[vcPart_Id]");
                strSql_sub.Append("           ,[vcPartsnamechn]");
                strSql_sub.Append("           ,[vcPartslocation]");
                strSql_sub.Append("           ,[vcInputnum]");
                strSql_sub.Append("           ,[vcPackingquantity]");
                strSql_sub.Append("           ,[vcItemname1]");
                strSql_sub.Append("           ,[vcPackingpartslocation1]");
                strSql_sub.Append("           ,[vcSuppliernamechn1]");
                strSql_sub.Append("           ,[vcOutnum1]");
                strSql_sub.Append("           ,[vcTemname2]");
                strSql_sub.Append("           ,[vcPackingpartslocation2]");
                strSql_sub.Append("           ,[vcSuppliernamechn2]");
                strSql_sub.Append("           ,[vcOutnum2]");
                strSql_sub.Append("           ,[vcItemname3]");
                strSql_sub.Append("           ,[vcPackingpartslocation3]");
                strSql_sub.Append("           ,[vcSuppliernamechn3]");
                strSql_sub.Append("           ,[vcOutnum3]");
                strSql_sub.Append("           ,[vcItemname4]");
                strSql_sub.Append("           ,[vcPackingpartslocation4]");
                strSql_sub.Append("           ,[vcSuppliernamechn4]");
                strSql_sub.Append("           ,[vcOutnum4]");
                strSql_sub.Append("           ,[vcItemname5]");
                strSql_sub.Append("           ,[vcPackingpartslocation5]");
                strSql_sub.Append("           ,[vcSuppliernamechn5]");
                strSql_sub.Append("           ,[vcOutnum5]");
                strSql_sub.Append("           ,[vcItemname6]");
                strSql_sub.Append("           ,[vcPackingpartslocation6]");
                strSql_sub.Append("           ,[vcSuppliernamechn6]");
                strSql_sub.Append("           ,[vcOutnum6]");
                strSql_sub.Append("           ,[vcItemname7]");
                strSql_sub.Append("           ,[vcPackingpartslocation7]");
                strSql_sub.Append("           ,[vcSuppliernamechn7]");
                strSql_sub.Append("           ,[vcOutnum7]");
                strSql_sub.Append("           ,[vcItemname8]");
                strSql_sub.Append("           ,[vcPackingpartslocation8]");
                strSql_sub.Append("           ,[vcSuppliernamechn8]");
                strSql_sub.Append("           ,[vcOutnum8]");
                strSql_sub.Append("           ,[vcItemname9]");
                strSql_sub.Append("           ,[vcPackingpartslocation9]");
                strSql_sub.Append("           ,[vcSuppliernamechn9]");
                strSql_sub.Append("           ,[vcOutnum9]");
                strSql_sub.Append("           ,[vcItemname10]");
                strSql_sub.Append("           ,[vcPackingpartslocation10]");
                strSql_sub.Append("           ,[vcSuppliernamechn10]");
                strSql_sub.Append("           ,[vcOutnum10]");
                strSql_sub.Append("           ,[vcPartsnoandnum]");
                strSql_sub.Append("           ,[vcLabel]");
                strSql_sub.Append("           ,[vcComputernm]");
                strSql_sub.Append("           ,[vcCpdcompany]");
                strSql_sub.Append("           ,[vcPlantcode]");
                strSql_sub.Append("           ,[vcCompanyname]");
                strSql_sub.Append("           ,[vcPlantname]");
                strSql_sub.Append("           ,[iQrcode]");
                strSql_sub.Append("           ,[vcOperatorID]");
                strSql_sub.Append("           ,[dOperatorTime]");
                strSql_sub.Append("           ,[dFirstPrintTime]");
                strSql_sub.Append("           ,[dLatelyPrintTime])");
                strSql_sub.Append("     VALUES");
                strSql_sub.AppendLine("           (''");
                strSql_sub.AppendLine("           ,'" + data1 + "'");
                strSql_sub.AppendLine("           ,'" + printDate + "'");
                strSql_sub.AppendLine("           ,'" + inputNo + "'");
                strSql_sub.AppendLine("           ,'" + partId + "'");
                strSql_sub.AppendLine("           ,'" + partsNameEn + "'");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,'" + quantity + "'");
                strSql_sub.AppendLine("           ,'" + packingQuantity + "'");
                strSql_sub.AppendLine("           ,'" + itemname1 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation1 + "'");
                strSql_sub.AppendLine("           ,'" + suppName1 + "'");
                strSql_sub.AppendLine("           ,'" + outNum1 + "'");
                strSql_sub.AppendLine("           ,'" + itemname2 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation2 + "'");
                strSql_sub.AppendLine("           ,'" + suppName2 + "'");
                strSql_sub.AppendLine("           ,'" + outNum2 + "'");

                strSql_sub.AppendLine("           ,'" + itemname3 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation3 + "'");
                strSql_sub.AppendLine("           ,'" + suppName3 + "'");
                strSql_sub.AppendLine("           ,'" + outNum3 + "'");

                strSql_sub.AppendLine("           ,'" + itemname4 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation4 + "'");
                strSql_sub.AppendLine("           ,'" + suppName4 + "'");
                strSql_sub.AppendLine("           ,'" + outNum4 + "'");

                strSql_sub.AppendLine("           ,'" + itemname5 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation5 + "'");
                strSql_sub.AppendLine("           ,'" + suppName5 + "'");
                strSql_sub.AppendLine("           ,'" + outNum5 + "'");

                strSql_sub.AppendLine("           ,'" + itemname6 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation6 + "'");
                strSql_sub.AppendLine("           ,'" + suppName6 + "'");
                strSql_sub.AppendLine("           ,'" + outNum6 + "'");

                strSql_sub.AppendLine("           ,'" + itemname7 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation7 + "'");
                strSql_sub.AppendLine("           ,'" + suppName7 + "'");
                strSql_sub.AppendLine("           ,'" + outNum7 + "'");

                strSql_sub.AppendLine("           ,'" + itemname8 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation8 + "'");
                strSql_sub.AppendLine("           ,'" + suppName8 + "'");
                strSql_sub.AppendLine("           ,'" + outNum8 + "'");

                strSql_sub.AppendLine("           ,'" + itemname9 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation9 + "'");
                strSql_sub.AppendLine("           ,'" + suppName9 + "'");
                strSql_sub.AppendLine("           ,'" + outNum9 + "'");

                strSql_sub.AppendLine("           ,'" + itemname10 + "'");
                strSql_sub.AppendLine("           ,'" + packLocation10 + "'");
                strSql_sub.AppendLine("           ,'" + suppName10 + "'");
                strSql_sub.AppendLine("           ,'" + outNum10 + "'");

                strSql_sub.AppendLine("           ,'" + partsAndNum + "'");
                strSql_sub.AppendLine("           ,'" + partsAndNum + "'");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,'" + cpdCompany + "'");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,''");
                strSql_sub.AppendLine("           ,@iQrcode");
                strSql_sub.AppendLine("           ,'" + opearteId + "'");
                strSql_sub.AppendLine("           ,'" + scanTime + "'");
                strSql_sub.AppendLine("           ,null");
                strSql_sub.AppendLine("           ,null)");
                sqlCommand_sub.CommandText = strSql_sub.ToString();
                sqlCommand_sub.Parameters.Add("@iQrcode", SqlDbType.Image);
                sqlCommand_sub.Parameters["@iQrcode"].Value = vs1;
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

        public DataTable GetInv()
        {
            StringBuilder GetInvSql = new StringBuilder();
            GetInvSql.Append("SELECT TOP(1) iAutoId,vcNo,vcData,vcPrintdate,vcInno,vcPart_Id,vcPartsnamechn,vcPartslocation,vcInputnum,vcPackingquantity\n");
            GetInvSql.Append(",vcItemname1,vcPackingpartslocation1,vcSuppliernamechn1,vcOutnum1,vcTemname2,vcPackingpartslocation2,vcSuppliernamechn2,vcOutnum2\n");
            GetInvSql.Append(",vcItemname3,vcPackingpartslocation3,vcSuppliernamechn3,vcOutnum3,vcItemname4,vcPackingpartslocation4,vcSuppliernamechn4\n");
            GetInvSql.Append(",vcOutnum4,vcItemname5,vcPackingpartslocation5,vcSuppliernamechn5,vcOutnum5,vcItemname6,vcPackingpartslocation6\n");
            GetInvSql.Append(",vcSuppliernamechn6,vcOutnum6,vcItemname7,vcPackingpartslocation7,vcSuppliernamechn7,vcOutnum7,vcItemname8\n");
            GetInvSql.Append(",vcPackingpartslocation8,vcSuppliernamechn8,vcOutnum8,vcItemname9,vcPackingpartslocation9,vcSuppliernamechn9\n");
            GetInvSql.Append(",vcOutnum9,vcItemname10,vcPackingpartslocation10,vcSuppliernamechn10,vcOutnum10,vcPartsnoandnum,vcLabel\n");
            GetInvSql.Append(",vcComputernm,vcCpdcompany,vcPlantcode,vcCompanyname,vcPlantname,iQrcode,vcOperatorID,dOperatorTime,dFirstPrintTime,dLatelyPrintTime FROM TInvList\n");


            return excute.ExcuteSqlWithSelectToDT(GetInvSql.ToString());
        }

        public DataTable GetPackInfo(string packingQuantity, string partId, string scanTime, string quantity)
        {
            StringBuilder GetPackInfoSql = new StringBuilder();
            //GetPackInfoSql.Append("select t1.vcDistinguish,t2.vcPackLocation,t1.vcPackNo,t1.iBiYao*"+int.Parse(quantity)+"/"+int.Parse(packingQuantity)+" as quantity from  TPackItem t1 left join TPackBase t2 on t1.vcPackNo=t2.vcPackNo \n");
            //GetPackInfoSql.Append("and t1.dFrom<='"+scanTime+ "' and t1.dTo>='" + scanTime + "'  and t2.dPackFrom<='" + scanTime + "' and t2.dPackTo>='" + scanTime + "' and t1.vcPartsNo='"+partId+ " ' order by t1.vcDistinguish \n");
            GetPackInfoSql.Append("select t1.vcDistinguish,t2.vcPackLocation,t1.vcPackNo,t1.iBiYao*4/1 as quantity from  TPackItem t1,TPackBase t2 where vcPartsNo='" + partId + "' and t1.vcPackNo=t2.vcPackNo and t1.dUsedFrom<='" + scanTime + "' and t1.dUsedTo>='" + scanTime + "'  and t2.dPackFrom<='" + scanTime + "' and t2.dPackTo>='" + scanTime + "' order by t1.vcDistinguish");
            return excute.ExcuteSqlWithSelectToDT(GetPackInfoSql.ToString());
        }
        //获取TinvList表结构

        public DataTable GetTagInfo(string partId, string scanTime)
        {
            StringBuilder GetTagInfoSql = new StringBuilder();
            GetTagInfoSql.Append("  select vcPartNameCN,vcSCSName,vcSCSAdress,vcZXBZNo from  TtagMaster where vcPart_Id='" + partId + "' and dTimeFrom<='" + scanTime + "' and dTimeTo>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(GetTagInfoSql.ToString());
        }

        public DataTable ValidateQB(string partId, string kanbanOrderNo, string kanbanSerial, string dock)
        {
            StringBuilder validateQBSql = new StringBuilder();
            validateQBSql.Append("  select vcPart_id from TOperatorQB  where vcPart_id='" + partId + "' and vcSR='" + dock + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcReflectFlag in ('0','1')");
            return excute.ExcuteSqlWithSelectToDT(validateQBSql.ToString());
        }

        public DataTable ValidateSR(string partId, string scanTime)
        {
            StringBuilder ValidateSRSql = new StringBuilder();
            ValidateSRSql.Append("select  vcBZUnit from TPackageMaster where dTimeFrom<='" + scanTime + "' and dTimeTo>='" + scanTime + "' and vcPart_id='" + partId + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateSRSql.ToString());

        }
        public DataTable GetPackBase(string iP)
        {
            StringBuilder GetPackBaseSql = new StringBuilder();
            GetPackBaseSql.Append("  select  distinct(vcLotid) from TPackList   where dFirstPrintTime  is null  and vcHostip='" + iP + "' order by vcLotid");

            return excute.ExcuteSqlWithSelectToDT(GetPackBaseSql.ToString());
        }

        public void UpdateOpr1(string kanbanOrderNo, string kanbanSerial, string partId, string dock)
        {
            StringBuilder UpdateOprSql = new StringBuilder();
            UpdateOprSql.Append("update TOperatorQB set vcReflectFlag='4' where vcPart_id='" + partId + "' and vcSR='" + dock + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");
            excute.ExcuteSqlWithStringOper(UpdateOprSql.ToString());
        }

        public DataTable ValidateQF(string partId, string scanTime)
        {
            StringBuilder ValidateQFSql = new StringBuilder();
            ValidateQFSql.Append("  select  vcBZQF,vcRHQF,vcBZUnit from TPackageMaster where vcPart_id='" + partId + "' and dTimeFrom<='" + scanTime + "' and dTimeTo>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateQFSql.ToString());
        }

        public DataTable ValidatePrice(string partId, string scanTime, string sHF, string supplierId)
        {
            StringBuilder ValidatePriceSql = new StringBuilder();
            ValidatePriceSql.Append("  select decPriceOrigin  from TPrice where vcSupplier_id='" + supplierId + "' and vcReceiver='" + sHF + "' and vcPart_id='" + partId + "' and dPricebegin<='" + scanTime + "' and dPriceEnd>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidatePriceSql.ToString());
        }

        public int InsertTP1(string iP, string opearteId, string serverTime, string printName)
        {
            StringBuilder insertTPSql = new StringBuilder();
            insertTPSql.Append("INSERT INTO TPrint_Temp(vcTableName,vcReportName,vcClientIP,vcPrintName,vcKind,vcOperatorID,dOperatorTime,vcCaseNo,vcSellNo,vcLotid,vcSupplierId, vcInno, vcFlag)");
            insertTPSql.Append("   VALUES ('TLabelList','SPR06LBIP','" + iP + "','" + printName + "','2','" + opearteId + "','" + serverTime + "','','','','','','0')");
            return excute.ExcuteSqlWithStringOper(insertTPSql.ToString());
        }
        /*
        public int InsertLbl(string partsNameEn, string partId, string cpdCompany, string quantity, string printCount, string supplierName, string supplierAddress, string carFamilyCode, string opearteId, string scanTime, string iP, Bitmap vs, string partsNameCn, string inputNo)
        {
          StringBuilder insertLblSql = new StringBuilder();
          insertLblSql.Append("INSERT INTO dbo.TLabelList(vcPartsnameen,vcPart_id,vcInno,vcCpdcompany,vcLabel,vcGetnum \n");
          insertLblSql.Append(" ,iDateprintflg,vcComputernm,vcPrindate,iQrcode,vcPrintcount,vcPartnamechineese \n");
          insertLblSql.Append(",vcSuppliername,vcSupplieraddress,vcExecutestandard,vcCartype,vcHostip,vcOperatorID \n");
          insertLblSql.Append(" ,dOperatorTime) VALUES('"+partsNameEn+"','"+partId+"','"+inputNo+"','"+cpdCompany+"','"+printCount+"','"+quantity+"', \n");
          insertLblSql.Append("null,'','','"+vs+"','"+printCount+"','"+partsNameCn+"','"+supplierName+"','"+supplierAddress+"','','"+carFamilyCode+"','"+iP+"','"+opearteId+"','"+scanTime+"')\n");
          return excute.ExcuteSqlWithStringOper(insertLblSql.ToString());
        }
        */

        public int InsertTP(string iP, string opearteId, string serverTime, string lotIdNew)
        {
            StringBuilder insertTPSql = new StringBuilder();
            insertTPSql.Append("INSERT INTO TPrint_Temp(vcTableName,vcReportName,vcClientIP,vcPrintName,vcKind,vcOperatorID,dOperatorTime,vcCaseNo,vcSellNo,vcLotid,vcSupplierId, vcInno, vcFlag)");
            insertTPSql.Append("   VALUES ('TPackList','SPR13PKBP','" + iP + "','LASEL PRINTER','1','" + opearteId + "','" + serverTime + "','','','" + lotIdNew + "','','','0')");
            return excute.ExcuteSqlWithStringOper(insertTPSql.ToString());
        }

        public int UpdatePack(string lotId, string packNo, string iP, string opearteId, double qty)
        {
            StringBuilder updatePackSql = new StringBuilder();
            updatePackSql.Append("  update TPackList set dQty=dQty+" + qty + " where vcLotid='" + lotId + "' and vcPackingpartsno='" + packNo + "' and vcHostip='" + iP + "' and vcOperatorID='" + opearteId + "'");
            return excute.ExcuteSqlWithStringOper(updatePackSql.ToString());
        }

        public int InsertPack(string lotId, string packNo, string distinguish, string inputNo, double qty, string packLocation, string scanTime, string serverTime, string opearteId, string iP, string trolleyNo, string lblStart, string lblEnd)
        {
            StringBuilder insertPackSql = new StringBuilder();
            insertPackSql.Append("INSERT INTO TPackList(vcLotid ,iNo ,vcPackingpartsno ,vcPackinggroup ,vcInno ,dQty ,vcPackingpartslocation,dDaddtime ,vcPcname ,vcHostip ,vcOperatorID ,dOperatorTime,vcTrolleyNo, dFirstPrintTime ,dLatelyPrintTime,vcLabelStart,vcLabelEnd)\n");
            insertPackSql.Append("     VALUES ('" + lotId + "',NULL,'" + packNo + "','" + distinguish + "','" + inputNo + "','" + qty + "','" + packLocation + "','" + scanTime + "','','" + iP + "','" + opearteId + "','" + serverTime + "','" + trolleyNo + "',null,null,'" + lblStart + "','" + lblEnd + "')\n");
            return excute.ExcuteSqlWithStringOper(insertPackSql.ToString());
        }

        public DataTable ValidatePack(string lotId, string iP, string opearteId, string packNo)
        {
            StringBuilder validatePackSql = new StringBuilder();
            validatePackSql.Append(" select * from TPackList where vcLotid='" + lotId + "' and vcHostip='" + iP + "' and  vcOperatorID='" + opearteId + "' and vcPackingpartsno='" + packNo + "'");
            return excute.ExcuteSqlWithSelectToDT(validatePackSql.ToString());
        }

        public DataTable GetPackBase(string scanTime, string packNo, string packingSpot)
        {
            StringBuilder getPackBaseSql = new StringBuilder();

            getPackBaseSql.Append("select vcPackLocation,vcDistinguish from TPackBase where vcPackNo='" + packNo + "' and vcPackSpot='" + packingSpot + "' and dPackFrom<='" + scanTime + "' and dPackTo>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(getPackBaseSql.ToString());
        }

        public DataTable ValidateOpr(string partId, string kanbanOrderNo, string kanbanSerial, string dock)
        {
            StringBuilder validateOpr = new StringBuilder();
            validateOpr.Append("select * from TOperateSJ where vcSR='" + dock + "' and vcPart_id = '" + partId + "' and vcKBOrderNo = '" + kanbanOrderNo + "' and vcKBLFNo = '" + kanbanSerial + "'");
            return excute.ExcuteSqlWithSelectToDT(validateOpr.ToString());
        }

        public DataTable GetInOutFlag(string partId, string scanTime)
        {
            StringBuilder getInOutFlagSql = new StringBuilder();

            getInOutFlagSql.Append("select vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace	 from TSPMaster WHERE vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(getInOutFlagSql.ToString());
        }

        public DataTable GetCheckType(string partId, string scanTime)
        {
            StringBuilder getCheckTypeSql = new StringBuilder();

            getCheckTypeSql.Append("select vcCheckP from tCheckQf  where vcPartId='" + partId + "' and vcTimeFrom<='" + scanTime + "' and vcTimeTo>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(getCheckTypeSql.ToString());
        }

        public int InsertOpr(string packingSpot, string inno, string kanbanOrderNo, string kanbanSerial, string partId, string inoutFlag, string supplierCode, string supplierPlant, string scanTime, string serverTime, string quantity, int packingQuantity, string cpdCompany, string dock, string checkType, string lblSart, string lblEnd, string opearteId, string pointType, string iP)
        {
            StringBuilder InsertOprSql = new StringBuilder();
            InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
            InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
            InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime,vcHostIp,packingcondition,vcPackingPlant)\n");
            InsertOprSql.Append("VALUES('S0','" + packingSpot + "','" + inno + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inoutFlag + "','" + supplierCode + "','" + supplierPlant + "','" + scanTime + "','" + serverTime + "',\n");
            InsertOprSql.Append("" + int.Parse(quantity) + ",'" + packingQuantity + "','" + cpdCompany + "','" + dock + "','','" + pointType + "','" + checkType + "','','','" + lblSart + "','" + lblEnd + "','',null,'','" + opearteId + "','" + serverTime + "','" + iP + "','0','')\n");
            return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
        }

        public DataTable ValidateData1(string partId, string scanTime)
        {
            StringBuilder validateDataSql = new StringBuilder();

            validateDataSql.Append("select iPackingQty,vcSupplierId,vcSupplierPlant from TSPMaster_Box where vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");

            return excute.ExcuteSqlWithSelectToDT(validateDataSql.ToString());
        }

        public DataTable GetCount(string partId)
        {
            StringBuilder getCountSql = new StringBuilder();
            getCountSql.Append("select ISNULL(sum(iQuantity),0) as sum from TOperatorQB where vcZYType='S0' and vcPart_id='" + partId + "' and vcReflectFlag='0'");//标志位为0表示只存在实绩情报表中
            return excute.ExcuteSqlWithSelectToDT(getCountSql.ToString());
        }

        public int UpdateOrd(string targetMonth, string orderNo, string seqNo, int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, int v9, int v10, int v11, int v12, int v13, int v14, int v15, int v16, int v17, int v18, int v19, int v20, int v21, int v22, int v23, int v24, int v25, int v26, int v27, int v28, int v29, int v30, int v31, int newSum, string partId)
        {
            StringBuilder UpdateOrdSql = new StringBuilder();
            UpdateOrdSql.Append("update SP_M_ORD set vcInputQtyDaily1=isnull(cast(vcInputQtyDaily1 as int),0)+" + v1 + ",vcInputQtyDaily2=isnull(cast(vcInputQtyDaily2 as int),0)+" + v2 + ",vcInputQtyDaily3=isnull(cast(vcInputQtyDaily3 as int),0)+" + v3 + ",vcInputQtyDaily4=isnull(cast(vcInputQtyDaily4 as int),0)+" + v4 + ",vcInputQtyDaily5=isnull(cast(vcInputQtyDaily5 as int),0)+" + v5 + ",vcInputQtyDaily6=isnull(cast(vcInputQtyDaily6 as int),0)+" + v6 + ",vcInputQtyDaily7=isnull(cast(vcInputQtyDaily7 as int),0)+" + v7 + ",vcInputQtyDaily8=isnull(cast(vcInputQtyDaily8 as int),0)+" + v8 + ",vcInputQtyDaily9=isnull(cast(vcInputQtyDaily9 as int),0)+" + v9 + ",vcInputQtyDaily10=isnull(cast(vcInputQtyDaily10 as int),0)+" + v10 + ",\n");
            UpdateOrdSql.Append("vcInputQtyDaily11=isnull(cast(vcInputQtyDaily11 as int),0)+" + v11 + ",vcInputQtyDaily12=isnull(cast(vcInputQtyDaily12 as int),0)+" + v12 + ",vcInputQtyDaily13=isnull(cast(vcInputQtyDaily13 as int),0)+" + v13 + ",vcInputQtyDaily14=isnull(cast(vcInputQtyDaily14 as int),0)+" + v14 + ",vcInputQtyDaily15=isnull(cast(vcInputQtyDaily15 as int),0)+" + v15 + ",vcInputQtyDaily16=isnull(cast(vcInputQtyDaily16 as int),0)+" + v16 + ",vcInputQtyDaily17=isnull(cast(vcInputQtyDaily17 as int),0)+" + v17 + ",vcInputQtyDaily18=isnull(cast(vcInputQtyDaily18 as int),0)+" + v18 + ",vcInputQtyDaily19=isnull(cast(vcInputQtyDaily19 as int),0)+" + v19 + ",vcInputQtyDaily20=isnull(cast(vcInputQtyDaily20 as int),0)+" + v20 + ",\n");
            UpdateOrdSql.Append("vcInputQtyDaily21=isnull(cast(vcInputQtyDaily21 as int),0)+" + v21 + ",vcInputQtyDaily22=isnull(cast(vcInputQtyDaily22 as int),0)+" + v22 + ",vcInputQtyDaily23=isnull(cast(vcInputQtyDaily23 as int),0)+" + v23 + ",vcInputQtyDaily24=isnull(cast(vcInputQtyDaily24 as int),0)+" + v24 + ",vcInputQtyDaily25=isnull(cast(vcInputQtyDaily25 as int),0)+" + v25 + ",vcInputQtyDaily26=isnull(cast(vcInputQtyDaily26 as int),0)+" + v26 + ",vcInputQtyDaily27=isnull(cast(vcInputQtyDaily27 as int),0)+" + v27 + ",vcInputQtyDaily28=isnull(cast(vcInputQtyDaily28 as int),0)+" + v28 + ",vcInputQtyDaily29=isnull(cast(vcInputQtyDaily29 as int),0)+" + v29 + ",vcInputQtyDaily30=isnull(cast(vcInputQtyDaily30 as int),0)+" + v30 + ",\n");
            UpdateOrdSql.Append("vcInputQtyDaily31=isnull(cast(vcInputQtyDaily31 as int),0)+" + v31 + ",vcInputQtyDailySum=isnull(cast(vcInputQtyDailySum as int),0)+" + newSum + " where vcTargetYearMonth='" + targetMonth + "' and vcOrderNo='" + orderNo + "' and vcSeqno='" + seqNo + "' and vcPartNo='" + partId + "' \n");
            return excute.ExcuteSqlWithStringOper(UpdateOrdSql.ToString());
        }




        public int UpdateSeqNo(string packingSpot, string serverTime, int seqNoNew, string tmpString)
        {
            StringBuilder UpdateSeqNoSql = new StringBuilder();
            UpdateSeqNoSql.Append("UPDATE TSeqNo SET SEQNO = " + seqNoNew + "\n");
            UpdateSeqNoSql.Append("  WHERE FLAG = ('" + tmpString + "'+'" + packingSpot + "') and DDATE='" + serverTime + "'\n");
            return excute.ExcuteSqlWithStringOper(UpdateSeqNoSql.ToString());
        }





        public DataTable ValidateSeqNo(string packingSpot, string serverTime, string tmpString)
        {
            StringBuilder ValidateSeqNoSql = new StringBuilder();
            ValidateSeqNoSql.Append("SELECT  FLAG,DDATE,SEQNO\n");
            ValidateSeqNoSql.Append("  FROM TSeqNo where FLAG='" + tmpString + "'+'" + packingSpot + "' AND DDATE='" + serverTime + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateSeqNoSql.ToString());
        }

        public DataTable ValidateOrd(string partId)
        {
            StringBuilder validateOrdSql = new StringBuilder();
            validateOrdSql.Append("select (ISNULL(SUM(CAST(vcPlantQtyDailySum as int)),0)-ISNULL(SUM(CAST(vcInputQtyDailySum as int)),0)) as sum  \n");
            validateOrdSql.Append("from SP_M_ORD where ISNULL(vcPlantQtyDailySum,0)!=ISNULL(vcInputQtyDailySum,0)  \n");
            validateOrdSql.Append("and vcPartNo='" + partId + "' and vcOrderNo!=''\n");
            return excute.ExcuteSqlWithSelectToDT(validateOrdSql.ToString());
        }

        public DataTable ValidateData(string partId, string scanTime, string dock)
        {
            StringBuilder ValidateDataSql = new StringBuilder();

            ValidateDataSql.Append("select vcReceiver,vcPackingPlant from TSPMaster_SufferIn where vcPackingPlant='TFTM' and vcPartId='" + partId + "'\n");
            ValidateDataSql.Append("and  dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "' and vcSufferIn='" + dock + "'\n");
            return excute.ExcuteSqlWithSelectToDT(ValidateDataSql.ToString());
        }
        public DataSet getInputInfoFromDB(string strIP, string serverTime)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("--本次全部需要上传的数据");
                stringBuilder.AppendLine("select t1.*,isnull(t2.vcBZUnit,0) as vcBZUnit,vcPartENName from ");
                stringBuilder.AppendLine("(select * from TOperatorQB where vcReflectFlag='0' and vcZYType='S0' and vcHostIp='" + strIP + "')t1");
                stringBuilder.AppendLine("left join");
                stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<='" + serverTime + "' and dTimeTo>='" + serverTime + "' )t2");
                stringBuilder.AppendLine("on t1.vcPart_id=t2.vcPart_id and t2.vcBZQF='1' ");
                stringBuilder.AppendLine("left join ");
                stringBuilder.AppendLine("(select vcPackingPlant,vcPartId,vcSupplierId,vcReceiver,vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace from TSPMaster WHERE dFromTime<='" + serverTime + "' and dToTime>='" + serverTime + "')b ");
                stringBuilder.AppendLine("on t1.vcPart_id = b.vcPartId and t1.vcCpdCompany=b.vcReceiver and t1.vcSupplierId=b.vcSupplierId  ");
                stringBuilder.AppendLine("--查询台车连番");
                stringBuilder.AppendLine("select distinct vcTrolleySeqNo from TOperatorQB ");
                stringBuilder.AppendLine("where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0' order by vcTrolleySeqNo");
                stringBuilder.AppendLine("--查询标签总个数");
                stringBuilder.AppendLine("select  sum(cast(t1.iQuantity as int)/cast(isnull(t2.vcBZUnit,0 )as int)) as iTagNum from ");
                stringBuilder.AppendLine("(select * from TOperatorQB where vcReflectFlag='0' and vcZYType='S0' and vcHostIp='" + strIP + "')t1");
                stringBuilder.AppendLine("left join");
                stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<='" + serverTime + "' and dTimeTo>='" + serverTime + "' )t2");
                stringBuilder.AppendLine("on t1.vcPart_id=t2.vcPart_id and t2.vcBZQF='1' ");
                stringBuilder.AppendLine("--查询断取指示书表结构");
                stringBuilder.AppendLine("SELECT TOP (1)*  FROM [TPackList]");
                stringBuilder.AppendLine("--查询标签表结构");
                stringBuilder.AppendLine("SELECT TOP (1)*  FROM [TLabelList]");
                stringBuilder.AppendLine("--查询订单表结构");
                stringBuilder.AppendLine("SELECT top(1)* FROM [dbo].[SP_M_ORD]");
                stringBuilder.AppendLine("--查询档次入库品番合计数量");
                stringBuilder.AppendLine("select vcPart_id,vcCpdCompany,vcSR,sum(iQuantity) as iSumQuantity from TOperatorQB");
                stringBuilder.AppendLine("where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0'");
                stringBuilder.AppendLine("group by vcPart_id,vcCpdCompany,vcSR");
                stringBuilder.AppendLine("--查询入库指令书结构");
                stringBuilder.AppendLine("SELECT top(1)* FROM [dbo].[TInvList]");
                return excute.ExcuteSqlWithSelectToDS(stringBuilder.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getPackInfo(string strIP, string serverTime)
        {
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
            stringBuilder.AppendLine("		,a.vcLotid");
            stringBuilder.AppendLine("		,a.vcInputNo as [vcInno]");
            stringBuilder.AppendLine("		,a.vcTrolleyNo as vcTrolleyNo");
            stringBuilder.AppendLine("		,a.vcLabelStart  as vcLabelStart");
            stringBuilder.AppendLine("		,a.vcLabelEnd as vcLabelEnd");
            stringBuilder.AppendLine("		from ");
            stringBuilder.AppendLine("(select * from TOperatorQB  where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPackNo,iBiYao,vcPartsNo,vcDistinguish  from TPackItem where dUsedFrom<='" + serverTime + "' and dUsedTo>='" + serverTime + "')b");
            stringBuilder.AppendLine("on a.vcPart_id = b.vcPartsNo");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPackLocation,vcDistinguish,vcPackSpot,vcPackNo from TPackBase where dPackFrom<='" + serverTime + "' and dPackTo>='" + serverTime + "')c");
            stringBuilder.AppendLine("on b.vcPackNo=c.vcPackNo and a.vcBZPlant=c.vcPackSpot");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<='" + serverTime + "' and dTimeTo>='" + serverTime + "' and vcBZQF='1' )d");
            stringBuilder.AppendLine("on a.vcPart_id=d.vcPart_id");
            //stringBuilder.AppendLine("group by c.vcPackLocation,c.vcDistinguish,c.vcPackLocation,a.vcLotid,a.vcTrolleyNo"); 

            return excute.ExcuteSqlWithSelectToDT(stringBuilder.ToString());
        }

        public DataTable getLabelInfo(string strIP, string serverTime)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select b.vcPartENName");
            stringBuilder.AppendLine("		,a.vcPart_id");
            stringBuilder.AppendLine("		,substring(a.vcPart_id,1,5)+'-'+SUBSTRING(a.vcPart_id,6,5)+'-'+SUBSTRING(a.vcPart_id,11,2) as [vcPart_id1]");
            stringBuilder.AppendLine("		,a.vcInputNo");
            stringBuilder.AppendLine("		,a.vcCpdCompany");
            stringBuilder.AppendLine("		,cast(a.iQuantity as int)/cast(c.vcBZUnit as int) as [vcGetnum]");
            stringBuilder.AppendLine("		,0 as [iDateprintflg]");
            stringBuilder.AppendLine("		,'' as [vcComputernm]");
            stringBuilder.AppendLine("		,'' as [vcPrindate]");
            stringBuilder.AppendLine("		,d.vcPartNameCN as [vcPartnamechineese]");
            stringBuilder.AppendLine("		,d.vcSCSName as [vcSuppliername]");
            stringBuilder.AppendLine("		,d.vcSCSAdress as [vcSupplieraddress]");
            stringBuilder.AppendLine("		,d.vcZXBZNo as [vcExecutestandard]");
            stringBuilder.AppendLine("		,b.vcCarfamilyCode as [vcCartype]");
            stringBuilder.AppendLine("		,c.vcBZQF");
            stringBuilder.AppendLine("		,a.vcLabelStart");
            stringBuilder.AppendLine("		,a.vcLabelEnd");
            stringBuilder.AppendLine("		from ");
            stringBuilder.AppendLine("(select * from TOperatorQB  where vcHostIp='" + strIP + "' and vcZYType='S0' and vcReflectFlag='0')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPackingPlant,vcPartId,vcSupplierId,vcReceiver,vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace from TSPMaster WHERE dFromTime<='" + serverTime + "' and dToTime>='" + serverTime + "')b");
            stringBuilder.AppendLine("on a.vcPart_id = b.vcPartId and a.vcCpdCompany=b.vcReceiver and a.vcSupplierId=b.vcSupplierId ");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster where dTimeFrom<='" + serverTime + "' and dTimeTo>='" + serverTime + "' and vcBZQF='1' )c");
            stringBuilder.AppendLine("on a.vcPart_id=c.vcPart_id");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPart_Id,vcCPDCompany,vcSupplier_id,vcPartNameCN,vcSCSName,vcSCSAdress,vcZXBZNo from  TtagMaster where dTimeFrom<='" + serverTime + "' and dTimeTo>='" + serverTime + "')d");
            stringBuilder.AppendLine("on a.vcPart_id=d.vcPart_id and a.vcCpdCompany=d.vcCPDCompany and a.vcSupplierId=d.vcSupplier_id");

            return excute.ExcuteSqlWithSelectToDT(stringBuilder.ToString());
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

            return excute.ExcuteSqlWithSelectToDT(stringBuilder.ToString());
        }
        public DataTable setSeqNo(string tmpString, int iAddNum, string formatServerTime)
        {
            try
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
                stringBuilder.AppendLine("select SEQNO as vcSeqNo  from  where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
                stringBuilder.AppendLine("update TSeqNo set SEQNO=SEQNO+" + iAddNum + "  where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
                stringBuilder.AppendLine("end");
                return excute.ExcuteSqlWithSelectToDT(stringBuilder.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setLotToOperatorQB(DataTable dtInfo, string strIP, string formatServerTime, string strLotSeqNo_begin)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < dtInfo.Rows.Count; i++)
                {
                    string strLotId = formatServerTime.Substring(2, 6).Trim() + "-" + "2" + "-" + (int.Parse(strLotSeqNo_begin) + i).ToString().PadLeft(5, '0').Trim();
                    string trolleySeqNo = dtInfo.Rows[i][0].ToString();
                    stringBuilder.AppendLine("update TOperatorQB set vcLotid='" + strLotId + "' where vcTrolleySeqNo='" + trolleySeqNo + "' and vcHostIp='" + strIP + "' and vcReflectFlag='0'");
                }
                excute.ExcuteSqlWithStringOper(stringBuilder.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setInvToOperatorQB(DataTable dataTable, string iP, string serverTime, string invSeqNo)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string partId = dataTable.Rows[i][0].ToString();
                    string kanbanOrderNo = dataTable.Rows[i][1].ToString();
                    string kanbanSerial = dataTable.Rows[i][2].ToString();
                    string dock = dataTable.Rows[i][3].ToString();
                    string quantity = dataTable.Rows[i][4].ToString();
                    string bzUnit = dataTable.Rows[i][5].ToString();
                    string inno = serverTime.Replace("-", "").Substring(2, 7).Trim() + "2" + (int.Parse(invSeqNo) + i).ToString().PadLeft(5, '0');

                    stringBuilder.AppendLine("update TOperatorQB set vcInputNo='" + inno + "' where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "'");
                    stringBuilder.AppendLine("and vcReflectFlag='0' and vcHostIp='" + iP + "' and vcZYType='S0'");


                }
                excute.ExcuteSqlWithStringOper(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setTagToOperatorQB(DataTable dtInfo, string strIP, string serverTime, string strTagSeqNo_begin)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < dtInfo.Rows.Count; i++)
                {
                    //计算标签首,标签尾
                    string partId = dtInfo.Rows[i][0].ToString();
                    string kanbanOrderNo = dtInfo.Rows[i][1].ToString();
                    string kanbanSerial = dtInfo.Rows[i][2].ToString();
                    string dock = dtInfo.Rows[i][3].ToString();
                    string quantity = dtInfo.Rows[i][4].ToString();
                    string bzUnit = dtInfo.Rows[i][5].ToString();

                    string lblSart = serverTime.Replace("-", "").Substring(2, 7).Trim() + strTagSeqNo_begin.PadLeft(5, '0').Trim();
                    string lblEnd = serverTime.Replace("-", "").Substring(2, 7).Trim() + (int.Parse(strTagSeqNo_begin) + (int.Parse(quantity) / int.Parse(bzUnit) - 1)).ToString().PadLeft(5, '0').Trim();
                    stringBuilder.AppendLine("update TOperatorQB set vcLabelStart='" + lblSart + "',vcLabelEnd='" + lblEnd + "' where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");
                    stringBuilder.AppendLine("and vcSR='" + dock + "' and vcHostIp='" + strIP + "' and vcReflectFlag='0' and vcZYType='S0'");
                }
                excute.ExcuteSqlWithStringOper(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool setInputInfo(string strIP, string strPointName, DataTable dtPackList_Temp, DataTable dtLabelList_Temp, DataTable dtInv_Temp, DataTable dtOrder_Temp, string strOperId)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
            string vcppp = "";
            string vcsss = "";
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
                strSql_addinfo.AppendLine("vcInputNo,");
                strSql_addinfo.AppendLine("vcKBOrderNo,");
                strSql_addinfo.AppendLine("vcKBLFNo,");
                strSql_addinfo.AppendLine("vcPart_id,");
                strSql_addinfo.AppendLine("iQuantity,");
                strSql_addinfo.AppendLine("0,");
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
                strSql_modinfo.AppendLine("           ,@vcInno");
                strSql_modinfo.AppendLine("           ,@dQty");
                strSql_modinfo.AppendLine("           ,@vcPackingpartslocation");
                strSql_modinfo.AppendLine("           ,@dDaddtime");
                strSql_modinfo.AppendLine("           ,@vcPcname");
                strSql_modinfo.AppendLine("           ,@vcHostip");
                strSql_modinfo.AppendLine("           ,@vcOperatorID");
                strSql_modinfo.AppendLine("           ,@dOperatorTime");
                strSql_modinfo.AppendLine("           ,@vcTrolleyNo");
                strSql_modinfo.AppendLine("           ,null");
                strSql_modinfo.AppendLine("           ,null");
                strSql_modinfo.AppendLine("           ,@vcLabelStart");
                strSql_modinfo.AppendLine("           ,@vcLabelEnd)");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcLotid", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPackingpartsno", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPackinggroup", "");
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

                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                return true;
                #endregion

            }
            catch (Exception ex)
            {
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
                return false;
            }
        }


    }
}
