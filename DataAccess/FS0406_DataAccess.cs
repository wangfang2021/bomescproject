using System;
using Common;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DataAccess
{
    public class FS0406_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索

        public DataTable searchApi(string vcReceiver, string vcType, string vcState, string start, string end)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT a.iAuto_id,a.vcReceiver,b.vcName AS vcState, c.vcName AS vcType,CASE WHEN a.vcType = '0' THEN CONVERT(VARCHAR(7),dRangeStart,111) WHEN a.vcType='1' THEN (CONVERT(VARCHAR(10),dRangeStart,111)+'-'+CONVERT(VARCHAR(10),dRangeEnd,111)) END AS vcRange,a.dSendTime, a.dCommitTime, a.vcRelation FROM (");
                sbr.AppendLine("SELECT * FROM TIF_Master");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C064'");
                sbr.AppendLine(") b ON a.vcState = b.vcValue");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C065'");
                sbr.AppendLine(") c ON a.vcType = c.vcValue");
                sbr.AppendLine("WHERE 1=1");
                if (!string.IsNullOrWhiteSpace(vcReceiver))
                {
                    sbr.AppendLine("AND a.vcReceiver = '" + vcReceiver + "' ");
                }
                if (!string.IsNullOrWhiteSpace(vcType))
                {
                    sbr.AppendLine("AND a.vcType = '" + vcType + "' ");
                }
                if (!string.IsNullOrWhiteSpace(vcState))
                {
                    sbr.AppendLine("AND a.vcState = '" + vcState + "' ");
                }
                if (vcType == "0")
                {
                    if (!string.IsNullOrWhiteSpace(start))
                    {
                        start = start.Substring(0, 6);
                        sbr.AppendLine("AND CONVERT(VARCHAR(6),a.dRangeStart,112) >= '" + start + "'");
                    }
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        start = start.Substring(0, 6);
                        sbr.AppendLine("AND CONVERT(VARCHAR(6),a.dRangeEnd,112) <= '" + end + "'");
                    }
                }
                else if (vcType == "1")
                {
                    if (!string.IsNullOrWhiteSpace(start))
                    {
                        sbr.AppendLine("AND CONVERT(VARCHAR(8),a.dRangeStart,112) >= '" + start + "'");
                    }
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        sbr.AppendLine("AND CONVERT(VARCHAR(8),a.dRangeEnd,112) <= '" + end + "'");
                    }
                }

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void createInfo(string Receiver, bool inFlag, string inTime, bool outFlag, string outStart, string outEnd, string userId, ref string msg)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();


                if (inFlag)
                {
                    inTime = inTime.Replace("-", "");
                    bool flag = isHasData("0", inTime, inTime.Substring(0, 6), outStart, Receiver);
                    if (flag)
                    {
                        string Relation = "IN" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        sbr.AppendLine("INSERT INTO dbo.TIF_Master(vcReceiver, vcType, dRangeStart, vcState, vcOperatorId, dOperatorTime, vcRelation)");
                        sbr.AppendLine("VALUES('" + Receiver + "',");
                        sbr.AppendLine("'0',");
                        sbr.AppendLine("'" + inTime + "01',");
                        sbr.AppendLine("'0'  ,");
                        sbr.AppendLine("'" + userId + "',");
                        sbr.AppendLine("GETDATE(),");
                        sbr.AppendLine("'" + Relation + "'");
                        sbr.AppendLine(")");

                        inTime = inTime.Substring(0, 6);
                        sbr.AppendLine("INSERT INTO TIF_IN(vcDiff, vcNo, vcPart_id, dInTime, iQuantity,vcPartName, vcRelation, vcOperatorId, dOperatorTime)");
                        sbr.AppendLine("SELECT b.vcBillType,a.vcInputNo,a.vcPart_id, a.dStart, a.iQuantity,vcPartNameCn,'" + Relation + "' AS vcRelation,'" + userId + "' AS vcOperatorId,GETDATE() AS dOperatorTime FROM (");
                        sbr.AppendLine("SELECT vcPart_id,vcInputNo,dStart,iQuantity,vcSupplier_id,vcSHF ");
                        sbr.AppendLine("FROM TOperateSJ ");
                        sbr.AppendLine("WHERE vcZYType = 'S0' AND CONVERT(VARCHAR(6),dStart,112)  = '" + inTime + "' AND vcSHF = '" + Receiver + "'");
                        sbr.AppendLine(") a");
                        sbr.AppendLine("LEFT JOIN(");
                        sbr.AppendLine("SELECT vcBillType,vcPartId,vcSupplierId,vcReceiver,vcPartNameCn,dFromTime,dToTime FROM TSPMaster ");
                        sbr.AppendLine(") b ON a.vcPart_id = b.vcPartId AND a.vcSupplier_id = b.vcSupplierId AND a.vcSHF = b.vcReceiver AND a.dStart >= b.dFromTime AND a.dStart <= b.dToTime");
                        sbr.AppendLine("ORDER BY a.dStart,a.vcPart_id,a.vcSHF,a.vcInputNo");
                    }
                    else
                    {
                        msg = "该时间段内入库数据为空";
                        return;
                    }
                }

                if (outFlag)
                {
                    bool flag = isHasData("1", inTime, inTime.Substring(0, 6), outStart, Receiver);

                    if (flag)
                    {
                        string Relation = "OUT" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        sbr.AppendLine("INSERT INTO dbo.TIF_Master(vcReceiver, vcType, dRangeStart, dRangeEnd, vcState, vcOperatorId, dOperatorTime, vcRelation)");
                        sbr.AppendLine("VALUES('" + Receiver + "',");
                        sbr.AppendLine("'1'  ,");
                        sbr.AppendLine("'" + outStart + "',");
                        sbr.AppendLine("'" + outEnd + "',");
                        sbr.AppendLine("'0'  ,");
                        sbr.AppendLine("'" + userId + "'  ,");
                        sbr.AppendLine("GETDATE(), ");
                        sbr.AppendLine("'" + Relation + "'");
                        sbr.AppendLine("    )");
                        sbr.AppendLine("INSERT INTO TIF_Out(vcDiff, vcSellNo, vcSellShop, vcPart_id, vcPartName, dOutTime, iQuantity, decPrice, vcDepartment, vcOperatorId, dOperatorTime, vcRelation)");
                        sbr.AppendLine("SELECT b.vcBillType, a.vcSellNo, a.vcSHF,a.vcPart_id,b.vcPartNameCn, a.dOperatorTime, a.iQuantity, a.decPriceWithTax,'' AS vcDepartment,'" + userId + "' AS vcOperatorId,GETDATE() AS dOperatorTime,'" + Relation + "' AS vcRelation FROM (");
                        sbr.AppendLine("SELECT vcPart_id,vcSellNo,vcSHF,dOperatorTime,iQuantity,decPriceWithTax,vcSupplier_id FROM TSell");
                        sbr.AppendLine("WHERE '" + outStart + "'<=CONVERT(VARCHAR(8),dOperatorTime,112) AND '" + outEnd + "'>=CONVERT(VARCHAR(8),dOperatorTime,112) AND vcSHF = '" + Receiver + "' ");
                        sbr.AppendLine(") a");
                        sbr.AppendLine("LEFT JOIN(");
                        sbr.AppendLine("SELECT vcBillType,vcPartId,vcSupplierId,vcReceiver,vcPartNameCn,dFromTime,dToTime FROM TSPMaster");
                        sbr.AppendLine(") b ON a.vcPart_id = b.vcPartId AND a.vcSupplier_id = b.vcSupplierId AND a.vcSHF = b.vcReceiver AND a.dOperatorTime >= b.dFromTime AND a.dOperatorTime <= b.dToTime");
                        sbr.AppendLine("ORDER BY a.vcSellNo,a.vcSHF,a.vcPart_id,SUBSTRING( a.vcSellNo,3,8) DESC ");
                    }
                    else
                    {
                        msg = "该时间段内出库数据为空";
                        return;

                    }
                }

                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region 判断是否有数据

        public bool isHasData(string type, string month, string startTime, string endTime, string receiver)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                if (type == "0")
                {
                    sbr.AppendLine("SELECT * FROM TOperateSJ ");
                    sbr.AppendLine("WHERE vcZYType = 'S0' AND CONVERT(VARCHAR(6),dStart,112)  = '" + month + "' AND vcSHF = '" + receiver + "'");
                }
                else if (type == "0")
                {
                    sbr.AppendLine("SELECT * FROM TSell");
                    sbr.AppendLine("WHERE '" + endTime + "'<=CONVERT(VARCHAR(8),dOperatorTime,112) AND '" + startTime + "'>CONVERT(VARCHAR(8),dOperatorTime,112) AND vcSHF = '" + receiver + "'");
                }


                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 发送

        public void sendApi(int id, string userId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("UPDATE TIF_Master SET dSendTime = GETDATE(),dOperatorTime = GETDATE(),vcOperatorId = '" + userId + "' WHERE iAuto_id =" + id + " ");
                excute.ExcuteSqlWithStringOper(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}