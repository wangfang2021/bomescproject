using System;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0407_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索

        public DataTable searchApi(string OrderNo, string strPartId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT T2.iAutoId as LinId,T7.vcName as vcState_name,T1.vcState,T1.vcOrderNo,T1.vcPart_id,T5.vcName as vcOrderPlant,");
                sbr.AppendLine("t3.vcName as vcInOut,T4.vcName as vcHaoJiu,T6.vcName as vcOESP,T1.vcSupplierId,T1.vcSupplierPlant,");
                sbr.AppendLine("t1.vcSupplierPlace,T1.vcSufferIn,t1.iPackingQty,t1.iOrderQuantity,");
                sbr.AppendLine("t2.iDuiYingQuantity,t2.dDeliveryDate,t2.dOutPutDate,t1.dReplyOverDate,");
                sbr.AppendLine("'0' as bModFlag,'0' as bAddFlag,'' as vcBgColor,");
                sbr.AppendLine("CASE WHEN T1.vcDelete='1' THEN '0' ELSE (CASE WHEN T1.vcState='3' THEN '0' ELSE '1' END)  END bSelectFlag FROM ");
                sbr.AppendLine("(SELECT [iAutoId] as LinId,[vcStatus] as vcState,[vcOrderNo] as vcOrderNo,[vcPart_id] as vcPart_id,[vcOrderPlant] as vcOrderPlant,");
                sbr.AppendLine("	   [vcInOut] as vcInOut,[vcHaoJiu] as vcHaoJiu,[vcOESP] as vcOESP,[vcSupplier_id] as vcSupplierId,[vcGQ] as vcSupplierPlant,");
                sbr.AppendLine("	   [vcChuHePlant] as vcSupplierPlace,[vcSufferIn] as vcSufferIn,[iPackingQty] as iPackingQty,[iOrderQuantity] as iOrderQuantity,");
                sbr.AppendLine("	   [dReplyOverDate] as dReplyOverDate,isnull([vcDelete],'0') as [vcDelete]");
                sbr.AppendLine("  FROM [TUrgentOrder]");
                sbr.AppendLine("  WHERE isnull([vcDelete],'0')='0' AND vcStatus = '3' ");
                if (OrderNo != "")
                {
                    sbr.AppendLine("AND [vcOrderNo] LIKE '" + OrderNo + "%'");
                }
                if (strPartId != "")
                {
                    sbr.AppendLine("AND [vcPart_id] LIKE '" + strPartId + "%'");
                }
                sbr.AppendLine("  )T1");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(select * from VI_UrgentOrder_OperHistory)T2");
                sbr.AppendLine("ON T1.vcOrderNo=T2.vcOrderNo AND T1.vcPart_id=T2.vcPart_id AND T1.vcSupplierId=T2.vcSupplier_id");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T3--内外区分");
                sbr.AppendLine("ON T1.vcInOut=T3.vcValue");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T4--号旧区分");
                sbr.AppendLine("ON T1.vcHaoJiu=T4.vcValue");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T5--发注工厂");
                sbr.AppendLine("ON T1.vcOrderPlant=T5.vcValue");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C012')T6--OE=SP");
                sbr.AppendLine("ON T1.vcOESP=T6.vcValue");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C056')T7--状态");
                sbr.AppendLine("ON T1.vcState=T7.vcValue");
                sbr.AppendLine("ORDER BY T1.vcOrderNo,t1.vcPart_id,t1.vcState");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getOrder()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT DISTINCT vcOrderNo FROM TUrgentOrder WHERE vcStatus = '3'");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getPartId()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT DISTINCT vcPart_id FROM TUrgentOrder WHERE vcStatus = '3'");
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