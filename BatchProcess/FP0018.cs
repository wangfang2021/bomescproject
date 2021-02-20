using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// 包材构成获取采购品番信息
/// </summary>
namespace BatchProcess
{
    public class FP0018
    {
        private MultiExcute excute = new MultiExcute();
        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0018";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "批处理开始", null, strUserId);
                //更新数据准备
                DataTable dtNewItem = this.SearchNewTPackItem(strUserId);
                if (dtNewItem.Rows.Count == 0)
                {//没有要请求的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "没有要请求的数据", null, strUserId);
                    return true;
                }
                bool isok = this.UpDate(strUserId, dtNewItem);

                if (isok)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行成功", null, strUserId);
                    return true;
                }
                else
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行失败", null, strUserId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, ex.ToString(), null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 生成待更新数据
        public DataTable SearchNewTPackItem(string strUserId)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder sql = new StringBuilder();
                sql.Append("  SELECT      \n");
                sql.Append("         a.[vcPartsNo]     \n");
                sql.Append("        ,a.[vcPackNo]     \n");
                sql.Append("        ,a.[vcPackGPSNo]     \n");
                sql.Append("        ,a.vcShouhuofangID      \n");
                sql.Append("        ,case when b.vcCarModel is not null then b.vcCarModel else a.vcCar  end as vcCar      \n");
                sql.Append("        ,case when b.dFromTime is not null then b.dFromTime else a.dUsedFrom  end as dUsedFrom   \n");
                sql.Append("        ,case when b.dToTime is not null then b.dToTime else a.dUsedTo  end as dUsedTo     \n");
                sql.Append("        ,a.[dFrom]     \n");
                sql.Append("        ,a.[dTo]     \n");
                sql.Append("        ,a.[vcDistinguish]     \n");
                sql.Append("        ,a.[iBiYao]     \n");
                sql.Append("        ,a.[vcOperatorID]     \n");
                sql.Append("        ,a.[dOperatorTime]     \n");
                sql.Append("          , case  when b.vcChanges is not null then b.vcChanges else  a.varChangedItem end as  varChangedItem  \n");
                sql.Append("      FROM     \n");
                sql.Append("     (    \n");
                sql.Append("      select * from TPackItem     \n");
                sql.Append("     )a left join    \n");
                sql.Append("     (    \n");
                sql.Append("    select * from TSPMaster    \n");
                sql.Append("    )b on a.vcPartsNo=b.vcPartId and a.vcShouhuofangID=b.vcReceiver and a.vcPackSpot=b.vcPackingPlant   \n");
                sql.Append("    union   \n");
                sql.Append("    (   \n");
                sql.Append("    select    \n");
                sql.Append("      vcPartId as vcPartsNo,   \n");
                sql.Append("      '' as [vcPackNo],   \n");
                sql.Append("      '' as [vcPackGPSNo],   \n");
                sql.Append("      vcReceiver as [vcShouhuofangID], \n");
                sql.Append("      vcCarModel as [vcCar],    \n");
                sql.Append("      dFromTime as [dUsedFrom],   \n");
                sql.Append("      dToTime as [dUsedTo],   \n");
                sql.Append("      '' as [dFrom],--自己填写？   \n");
                sql.Append("      '' as [dTo],   \n");
                sql.Append("       '' as [vcDistinguish],    \n");
                sql.Append("       '' as [iBiYao],    \n");
                sql.Append("    '" + strUserId + "' as  [vcOperatorID],  \n");
                sql.Append("    GETDATE() as [dOperatorTime],  \n");
                sql.Append("     --varChangedItem \n");
                sql.Append("      vcChanges as [varChangedItem] \n");
                sql.Append("        from TSPMaster  where vcPartId+vcReceiver not in (select vcPartsNo+vcShouhuofangID  from TPackItem )  \n");
                sql.Append("   ) \n");
                dt = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                return dt;

            }
            catch (Exception ex)
            {

                //批处理异常结束
                //ComMessage.GetInstance().ProcessMessage("FP0018", ex.ToString(), null, strUserId);
                throw ex;

            }
        }
        #endregion

        #region 更新数据准备
        public bool UpDate(string strUserId, DataTable dtNewItem)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder sql = new StringBuilder();
                sql.Append("  truncate  table TPackItem   \n");
                for (int i = 0; i < dtNewItem.Rows.Count; i++)
                {
                    sql.Append("  insert into TPackItem  \n");
                    sql.Append("  ([vcPartsNo],  \n");
                    sql.Append("   [vcPackNo],  \n");
                    sql.Append("   [vcPackGPSNo],  \n");
                    sql.Append("   [vcShouhuofangID],  \n");
                    sql.Append("   [vcCar],  \n");
                    sql.Append("   [dUsedFrom],  \n");
                    sql.Append("   [dUsedTo],  \n");
                    sql.Append("   [dFrom],  \n");
                    sql.Append("   [dTo],  \n");
                    sql.Append("   [vcDistinguish],  \n");
                    sql.Append("   [iBiYao],  \n");
                    sql.Append("   [vcOperatorID],  \n");
                    sql.Append("   [dOperatorTime],  \n");
                    sql.Append("   [varChangedItem]  \n");
                    sql.Append("   ) values  \n");
                    sql.Append("   ( \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPartsNo"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPackNo"].ToString() + "' ,  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPackGPSNo"].ToString() + "',    \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcShouhuofangID"].ToString() + "',   \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcCar"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dUsedFrom"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dUsedTo"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dFrom"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dTo"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcDistinguish"].ToString() + "',   \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["iBiYao"].ToString() + "',  \n");
                    sql.Append("  '" + strUserId + "', \n");
                    sql.Append("   GETDATE(), \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["varChangedItem"].ToString() + "' ) \n");

                }
                int isok = 1;
                if (sql.Length > 0)
                {
                    isok = excute.ExcuteSqlWithStringOper(sql.ToString());
                }
                if (isok > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                //批处理异常结束
                //ComMessage.GetInstance().ProcessMessage("FP0018", ex.ToString(), null, strUserId);
                throw ex;

            }
        }

        #endregion
    }
}