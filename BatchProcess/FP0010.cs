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
                ComMessage.GetInstance().ProcessMessage(PageId, "批处理开始执行", null, strUserId);

                bool flag = syncSJ();

                if (!flag)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行失败", null, strUserId);
                    return false;
                }

                //批处理
                ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行结束", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行有误", null, strUserId);
                throw ex;
            }
        }
        #endregion

        public bool syncSJ()
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

                excute.ExcuteSqlWithStringOper(sbr.ToString());

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
