using Common;
using System;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// 品番信息传送调达
/// </summary>
namespace BatchProcess
{
    public class FP0010
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0010";
            try
            {
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1001", null, strUserId);

                syncSJ();

                //批处理
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1002", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1003", ex, strUserId);
                throw ex;
            }
        }
        #endregion

        public void syncSJ()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

                sbr.AppendLine("UPDATE a");
                sbr.AppendLine("SET a.dTimeFromSJ = b.dFromTime");
                sbr.AppendLine("FROM TUnit a");
                sbr.AppendLine("    LEFT JOIN");
                sbr.AppendLine("    (");
                sbr.AppendLine("        SELECT dFromTime,");
                sbr.AppendLine("               vcReceiver,");
                sbr.AppendLine("               vcSupplierId,");
                sbr.AppendLine("               vcPartId,");
                sbr.AppendLine("               vcPackingPlant");
                sbr.AppendLine("        FROM VI_SPMaster_sync");
                sbr.AppendLine("    ) b");
                sbr.AppendLine("        ON a.vcReceiver = b.vcReceiver");
                sbr.AppendLine("           AND a.vcSupplier_id = b.vcSupplierId");
                sbr.AppendLine("           AND (CASE WHEN LEN(REPLACE(a.vcPart_id,'-','')) = 12 THEN REPLACE(a.vcPart_id,'-','') WHEN LEN(REPLACE(a.vcPart_id,'-','')) = 10 THEN REPLACE(a.vcPart_id,'-','')+'00' END) = b.vcPartId");
                sbr.AppendLine("           AND a.vcSYTCode = b.vcPackingPlant;");

                excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
