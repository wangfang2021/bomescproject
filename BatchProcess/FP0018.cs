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
                sql.Append(" SELECT            \n");
                sql.Append("         a.[vcPartsNo]           \n");
                sql.Append("        ,a.[vcPackNo]           \n");
                sql.Append("        ,a.[vcPackGPSNo]           \n");
                sql.Append("        ,case when b.vcReceiver is not null then b.vcReceiver else a.vcShouhuofangID   end as vcShouhuofangID       \n");
                sql.Append("        ,case when b.vcCarModel is not null then b.vcCarModel else a.vcCar  end as vcCar            \n");
                sql.Append("        ,case when b.dFromTime is not null then b.dFromTime else a.dUsedFrom  end as dUsedFrom         \n");
                sql.Append("        ,case when b.dToTime is not null then b.dToTime else a.dUsedTo  end as dUsedTo           \n");
                sql.Append("        ,a.[dFrom]           \n");
                sql.Append("        ,a.[dTo]           \n");
                sql.Append("        ,a.[vcDistinguish]           \n");
                sql.Append("        ,a.[iBiYao]           \n");
                sql.Append("        ,a.[vcOperatorID]           \n");
                sql.Append("        ,a.[dOperatorTime]           \n");
                sql.Append("        ,c.vcBZPlant as vcPackSpot           \n");
                sql.Append("        , case  when b.vcChanges is not null then b.vcChanges else  a.varChangedItem end as  varChangedItem        \n");
                sql.Append("      FROM           \n");
                sql.Append("     (          \n");
                sql.Append("      select * from TPackItem           \n");
                sql.Append("      )a       \n");
                sql.Append(" 	  left join          \n");
                sql.Append("     (          \n");
                sql.Append("      select * from TSPMaster          \n");
                sql.Append(" 	  where GETDATE() between dFromTime and dToTime      \n");
                sql.Append("      )b on a.vcPartsNo=b.vcPartId and a.vcShouhuofangID=b.vcReceiver       \n");
                sql.Append("    left join     \n");
                sql.Append("    (     \n");
                sql.Append("      select * from TPackageMaster        \n");
                sql.Append("    )c on  a.vcPartsNo=c.vcPart_id     \n");
                sql.Append("       \n");
                sql.Append("   union All     \n");
                sql.Append("       \n");
                sql.Append("      select             \n");
                sql.Append("     a.vcPartId as vcPartsNo,            \n");
                sql.Append("     '' as [vcPackNo],            \n");
                sql.Append("     '' as [vcPackGPSNo],            \n");
                sql.Append("     a.vcReceiver as [vcShouhuofangID],          \n");
                sql.Append("     a.vcCarModel as [vcCar],             \n");
                sql.Append("     a.dFromTime as [dUsedFrom],            \n");
                sql.Append("     a.dToTime as [dUsedTo],            \n");
                sql.Append("     '1990-01-01 0:00:00' as [dFrom],            \n");
                sql.Append("     '3000-01-01 0:00:00' as [dTo],            \n");
                sql.Append("      '' as [vcDistinguish],             \n");
                sql.Append("      '' as [iBiYao],             \n");
                sql.Append("   '000000' as  [vcOperatorID],           \n");
                sql.Append("   GETDATE() as [dOperatorTime],           \n");
                sql.Append("    --varChangedItem          \n");
                sql.Append("    a.vcChanges as [varChangedItem]          \n");
                sql.Append("    ,b.vcBZPlant as vcPackSpot      \n");
                sql.Append("   from         \n");
                sql.Append("   (         \n");
                sql.Append("   select a.vcPartId,a.vcReceiver,b.dFromTime,c.dToTime,a.vcChanges,a.vcCarModel     \n");
                sql.Append("  from(     \n");
                sql.Append("     select vcChanges,vcPartId,vcReceiver,dFromTime,dToTime,vcCarModel from TSPMaster     \n");
                sql.Append("       where vcPartId+vcReceiver not in          \n");
                sql.Append("      (select vcPartsNo+vcShouhuofangID from TPackItem )      \n");
                sql.Append("    and GETDATE() between dFromTime  and dToTime     \n");
                sql.Append("    )      \n");
                sql.Append("    a left join     \n");
                sql.Append("    (     \n");
                sql.Append("  	   select vcPartId,vcReceiver,min(dFromTime) as dFromTime from TSPMaster           \n");
                sql.Append("  	   where vcPartId+vcReceiver not in          \n");
                sql.Append("  	  (select vcPartsNo+vcShouhuofangID from TPackItem )      \n");
                sql.Append("  	  group by vcPartId,vcReceiver     \n");
                sql.Append("  	  )b on a.vcPartId=b.vcPartId  and a.vcReceiver=b.vcReceiver     \n");
                sql.Append("  	   left join     \n");
                sql.Append("  	  (     \n");
                sql.Append("  		   select vcPartId,vcReceiver,max(dToTime) as dToTime from TSPMaster     \n");
                sql.Append("  		   where vcPartId+vcReceiver not in          \n");
                sql.Append("  		  (select vcPartsNo+vcShouhuofangID from TPackItem )      \n");
                sql.Append("  		  group by vcPartId,vcReceiver     \n");
                sql.Append("  	  )c on a.vcPartId=c.vcPartId  and a.vcReceiver=c.vcReceiver     \n");
                sql.Append("  	       \n");
                sql.Append("     )A       \n");
                sql.Append("     inner join        \n");
                sql.Append("     (        \n");
                sql.Append("       select * from TPackageMaster        \n");
                sql.Append("      )B on A.vcPartId=B.vcPart_id     \n");


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
         
                    string dTo = dtNewItem.Rows[i]["dTo"].ToString() == "" ? "9999-12-31 23:59:59" : dtNewItem.Rows[i]["dTo"].ToString();

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
                    sql.Append("   [varChangedItem],  \n");
                    sql.Append("   [vcSupplierId],  \n");
                    sql.Append("   [vcPackingPlant]  \n");
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
                    sql.Append("  '" + dTo + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcDistinguish"].ToString() + "',   \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["iBiYao"].ToString() + "',  \n");
                    sql.Append("  '" + strUserId + "', \n");
                    sql.Append("   GETDATE(), \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["varChangedItem"].ToString() + "',  \n");
                    sql.Append("  '"+ dtNewItem.Rows[i]["vcSupplierId"].ToString() + "', \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPackingPlant"].ToString() + "')  \n");

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