using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// 更新品番对应小品目信息
/// </summary>
namespace BatchProcess
{
    public class FP0012
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0012";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0200", null, strUserId);

                Start();

                //批处理结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PE0200", null, strUserId);
                throw ex;
            }
        }
        #endregion

        public void Start()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("update TPackageMaster set vcSmallPM=null ");

                sql.AppendLine("update TPackageMaster set vcSmallPM='无' where ISNULL(vcSR,'')=''");

                sql.AppendLine("update t1 set t1.vcSmallPM=t2.vcSmallPM");
                sql.AppendLine("from (");
                sql.AppendLine("	select * from TPackageMaster ");
                sql.AppendLine(")t1");
                sql.AppendLine("inner join TPMSmall t2 on t1.vcSR=t2.vcSR and t1.vcSupplierId=t2.vcSupplier_id ");
                sql.AppendLine("and LEFT(t1.vcPart_id,5)=t2.vcPartsNoBefore5");

                sql.AppendLine("update t1 set t1.vcSmallPM=t2.vcSmallPM ");
                sql.AppendLine("from (");
                sql.AppendLine("	select * from TPackageMaster where isnull(vcSmallPM,'')=''");
                sql.AppendLine(")t1");
                sql.AppendLine("inner join (select * from TPMSmall where ISNULL(vcSupplier_id,'')='') t2 ");
                sql.AppendLine("on t1.vcSR=t2.vcSR and LEFT(t1.vcPart_id,5)=t2.vcPartsNoBefore5");

                sql.AppendLine("update t1 set t1.vcSmallPM=t3.vcSmallPM ");
                sql.AppendLine("from (");
                sql.AppendLine("	select * from TPackageMaster where isnull(vcSmallPM,'')='' ");
                sql.AppendLine(")t1");
                sql.AppendLine("inner join (select * from TPackItem where getdate() between dFrom and dTo)t2");
                sql.AppendLine("on t1.vcPart_id=t2.vcPartsNo");
                sql.AppendLine("inner join (select * from TPMSmall where ISNULL(vcPartsNoBefore5,'')='')t3");
                sql.AppendLine("on t1.vcSR=t3.vcSR and t1.vcSupplierId=t3.vcSupplier_id and t2.vcPackNo=t3.vcBCPartsNo");

                sql.AppendLine("update t1 set t1.vcSmallPM=t3.vcSmallPM ");
                sql.AppendLine("from (");
                sql.AppendLine("	select * from TPackageMaster where isnull(vcSmallPM,'')='' ");
                sql.AppendLine(")t1");
                sql.AppendLine("inner join (select * from TPackItem where getdate() between dFrom and dTo)t2");
                sql.AppendLine("on t1.vcPart_id=t2.vcPartsNo");
                sql.AppendLine("inner join (select * from TPMSmall where ISNULL(vcSupplier_id,'')='' and ISNULL(vcPartsNoBefore5,'')='')t3");
                sql.AppendLine("on t1.vcSR=t3.vcSR and t2.vcPackNo=t3.vcBCPartsNo");

                sql.AppendLine("update t1 set t1.vcSmallPM=t2.vcSmallPM ");
                sql.AppendLine("from (");
                sql.AppendLine("	select * from TPackageMaster where isnull(vcSmallPM,'')='' ");
                sql.AppendLine(")t1");
                sql.AppendLine("inner join (");
                sql.AppendLine("	select * from TPMSmall where ISNULL(vcPartsNoBefore5,'')='' and ISNULL(vcBCPartsNo,'')=''");
                sql.AppendLine(")t2");
                sql.AppendLine("on t1.vcSR=t2.vcSR and t1.vcSupplierId=t2.vcSupplier_id");

                sql.AppendLine("update t1 set t1.vcSmallPM=t2.vcSmallPM ");
                sql.AppendLine("from (");
                sql.AppendLine("	select * from TPackageMaster where isnull(vcSmallPM,'')='' ");
                sql.AppendLine(")t1");
                sql.AppendLine("inner join (");
                sql.AppendLine("	select * from TPMSmall where ISNULL(vcSupplier_id,'')='' and ISNULL(vcPartsNoBefore5,'')='' ");
                sql.AppendLine("	and ISNULL(vcBCPartsNo,'')=''");
                sql.AppendLine(")t2");
                sql.AppendLine("on t1.vcSR=t2.vcSR ");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
