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
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1801", null, strUserId);
                //查找采购master与构成不同
                DataTable dtDiff = this.SearchDiff();
                //更新构成删除履历
                bool isReNew = this.UpDateDelete(dtDiff, strUserId);
                //更新数据准备
                DataTable dtNewItem = this.SearchNewTPackItem(strUserId);
                if (dtNewItem.Rows.Count == 0)
                {//没有要请求的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1801", null, strUserId);
                    return true;
                }
                bool isok = this.UpDate(strUserId, dtNewItem);

                if (isok)
                {
                    //正常结束
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1802", null, strUserId);
                    return true;
                }
                else
                {
                    //更新失败
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1802", null, strUserId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1803", ex, strUserId);
                throw ex;
            }
        }  
        #endregion

        #region 查找采购master与构成不同
        public DataTable SearchDiff()
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder sql = new StringBuilder();
                sql.Append("  select vcPartsNo,vcPackNo,vcPackGPSNo,vcShouhuofangID,vcCar,dUsedFrom,dUsedTo,dFrom,       \n");
                sql.Append("  dTo,vcDistinguish,iBiYao,vcOperatorID,dOperatorTime,varChangedItem,vcPackSpot,vcReTime       \n");
                sql.Append("  from TPackItem where vcPartsNo in (       \n");
                sql.Append("  select vcPartsNo from        \n");
                sql.Append("  (select distinct vcPartId from TSPMaster where isnull(vcDelete,'0')<>'1' and dFromTime<>dToTime and dToTime>=GETDATE())a       \n");
                sql.Append("  full join       \n");
                sql.Append("  (select distinct vcPartsNo from TPackItem)b       \n");
                sql.Append("  on a.vcPartId=b.vcPartsNo       \n");
                sql.Append("  where isnull(vcPartId,'')=''        \n");
                sql.Append("  )       \n");
                dt = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                return dt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion

        #region 更新tpackitemdelete表

        public bool UpDateDelete(DataTable dt, string strUserId)
        {
            try
            {
                DataTable dtitemdelete = new DataTable();
                StringBuilder sql = new StringBuilder();
                sql.Append(" select * from TPackItemDelete  \n");
                dtitemdelete = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                StringBuilder sql1 = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dtitemdelete.Select("vcPartsNo='" + dt.Rows[i]["vcPartsNo"].ToString() + "'and vcPackNo='" + dt.Rows[i]["vcPackNo"].ToString() + "'").Length != 0)
                    {
                        sql1.Append("  update TPackItemDelete set \n");
                        sql1.Append("  vcPartsNo='" + dt.Rows[i]["vcPartsNo"].ToString().Trim() + "', \n");
                        sql1.Append("  vcPackNo='" + dt.Rows[i]["vcPackNo"].ToString().Trim() + "', \n");
                        sql1.Append("  vcPackGPSNo='" + dt.Rows[i]["vcPackGPSNo"].ToString().Trim() + "', \n");
                        sql1.Append("  vcShouhuofangID='" + dt.Rows[i]["vcShouhuofangID"].ToString().Trim() + "', \n");
                        sql1.Append("  vcCar='" + dt.Rows[i]["vcCar"].ToString().Trim() + "', \n");
                        sql1.Append("  dUsedFrom='" + dt.Rows[i]["dUsedFrom"].ToString().Trim() + "', \n");
                        sql1.Append("  dUsedTo='" + dt.Rows[i]["dUsedTo"].ToString().Trim() + "', \n");
                        sql1.Append("  dFrom='" + dt.Rows[i]["dFrom"].ToString().Trim() + "', \n");
                        sql1.Append("  dTo='" + dt.Rows[i]["dTo"].ToString().Trim() + "', \n");
                        sql1.Append("  vcDistinguish='" + dt.Rows[i]["vcDistinguish"].ToString().Trim() + "', \n");
                        sql1.Append("  iBiYao='" + dt.Rows[i]["iBiYao"].ToString().Trim() + "', \n");
                        sql1.Append("  varChangedItem='" + dt.Rows[i]["varChangedItem"].ToString().Trim() + "', \n");
                        sql1.Append("  vcReTime='" + dt.Rows[i]["vcReTime"].ToString().Trim() + "' \n");
                        sql1.Append("  where vcPartsNo='" + dt.Rows[i]["vcPartsNo"].ToString().Trim() + "' and vcPackNo='" + dt.Rows[i]["vcPackNo"].ToString().Trim() + "' \n");

                    }
                    else
                    {
                        sql1.Append(" insert into TPackItemDelete   \n");
                        sql1.Append(" (vcPartsNo,vcPackNo,vcPackGPSNo,vcShouhuofangID,  \n");
                        sql1.Append(" vcCar,dUsedFrom,dUsedTo,dFrom,dTo,vcDistinguish,  \n");
                        sql1.Append(" iBiYao,vcOperatorID,dOperatorTime,varChangedItem,  \n");
                        sql1.Append(" vcReTime  \n");
                        sql1.Append(" )values(  \n");
                        sql1.Append(" '" + dt.Rows[i]["vcPartsNo"].ToString().Trim() + "',  \n");
                        sql1.Append(" '" + dt.Rows[i]["vcPackNo"].ToString().Trim() + "',  \n");
                        sql1.Append(" '" + dt.Rows[i]["vcPackGPSNo"].ToString().Trim() + "',  \n");
                        sql1.Append(" '" + dt.Rows[i]["vcShouhuofangID"].ToString().Trim() + "',  \n");
                        sql1.Append(" '" + dt.Rows[i]["vcCar"].ToString().Trim() + "',  \n");
                        sql1.Append(" '" + dt.Rows[i]["dUsedFrom"].ToString().Trim() + "',  \n");
                        sql1.Append(" '" + dt.Rows[i]["dUsedTo"].ToString().Trim() + "',  \n");
                        if (string.IsNullOrEmpty(dt.Rows[i]["dFrom"].ToString().Trim()))
                        {
                            sql1.Append(" '1900-01-01 00:00:00',  \n");

                        }
                        else
                        {
                            sql1.Append(" '" + dt.Rows[i]["dFrom"].ToString().Trim() + "',  \n");
                        }

                        if (string.IsNullOrEmpty(dt.Rows[i]["dTo"].ToString().Trim()))
                        {
                            sql1.Append(" '9999-12-31 11:59:59',  \n");

                        }
                        else
                        {
                            sql1.Append(" '" + dt.Rows[i]["dTo"].ToString().Trim() + "',  \n");
                        }

                        sql1.Append(" '" + dt.Rows[i]["vcDistinguish"].ToString().Trim() + "',  \n");
                        if (string.IsNullOrEmpty(dt.Rows[i]["iBiYao"].ToString().Trim()))
                        {
                            sql1.Append(" NULL ,  \n");

                        }
                        else
                        {

                            sql1.Append(" '" + dt.Rows[i]["iBiYao"].ToString().Trim() + "',  \n");
                        }
                        sql1.Append(" '" + strUserId + "',  \n");
                        sql1.Append(" GETDATE(),  \n");
                        sql1.Append(" '" + dt.Rows[i]["varChangedItem"].ToString().Trim() + "',  \n");
                        sql1.Append(" '" + dt.Rows[i]["vcReTime"].ToString().Trim() + "'  \n");
                        sql1.Append(" )  \n");
                        sql1.Append("   \n");

                    }

                }
                int isok = 1;
                if (sql1.Length > 0)
                {
                    isok = excute.ExcuteSqlWithStringOper(sql1.ToString());
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
                string dtimenow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                StringBuilder sql = new StringBuilder();
      
                sql.Append("   select Table1.vcPartsNo,Table1.vcPackNo,Table1.vcPackGPSNo,Table1.vcShouhuofangID,        \n");
                sql.Append("   Table1.vcCar,Table1.dUsedFrom,Table1.dUsedTo,Table1.dFrom,Table1.dTo,Table1.vcDistinguish,        \n");
                sql.Append("   Table1.iBiYao,Table1.vcPackSpot,Table1.varChangedItem,Table1.vcReTime        \n");
                sql.Append("   from (        \n");
                sql.Append("  select   distinct      \n");
                sql.Append("           temp.[vcPartsNo]                    \n");
                sql.Append("          ,temp.[vcPackNo]                    \n");
                sql.Append("          ,temp.[vcPackGPSNo]                    \n");
                sql.Append("          , Temp2.vcReceiver  as vcShouhuofangID           \n");
                sql.Append("          , Temp2.vcCarModel as vcCar                      \n");
                sql.Append("          , Temp3.dFromTime  as dUsedFrom                  \n");
                sql.Append("          , Temp4.dToTime as dUsedTo                    \n");
                sql.Append("          ,temp.dFrom  as [dFrom]                 \n");
                sql.Append("          ,temp.dTo as [dTo]                \n");
                sql.Append("          ,temp.[vcDistinguish]                    \n");
                sql.Append("          ,temp.[iBiYao]                    \n");
                sql.Append("          ,'' as vcPackSpot                    \n");
                sql.Append("          ,Temp2.vcChanges as varChangedItem            \n");
                sql.Append("          ,temp.[vcReTime]   \n");
                sql.Append("           \n");
                sql.Append("   from         \n");
                sql.Append("  (         \n");
                sql.Append("   select ss.[vcPartsNo],ss.[vcPackNo],ss.[vcPackGPSNo],sss.dPackFrom    \n");
                sql.Append("   ,sss.dPackTo,ss.[vcDistinguish],ss.[iBiYao],ss.[vcOperatorID],ss.[dOperatorTime],ss.dFrom,ss.dTo     \n");
                sql.Append("   ,ss.vcReTime    \n");
                sql.Append("   from(     \n");
                sql.Append("   select *from TPackItem      \n");
                sql.Append("   )ss left join     \n");
                sql.Append("   (select * from TPackBase    \n");
                sql.Append("    where GETDATE() between dPackFrom and dPackTo  \n");
                sql.Append("   )sss on ss.vcPackNo=sss.vcPackNo     \n");
                sql.Append("  )temp          \n");
                sql.Append("  left join         \n");
                sql.Append("  (         \n");

                sql.Append("   select Temp0.vcPartId,min(Temp0.dTime) as dTime from(          \n");


                sql.Append("  select vcpartid,         \n");
                sql.Append("       case          \n");
                sql.Append("       when DQYX is not NULL and WLYX is not NULL and YQYX is not NULL then DQYX           \n");
                sql.Append("       when DQYX is not NULL and WLYX is NULL and YQYX is NULL then DQYX         \n");
                sql.Append("       when DQYX is not NULL and WLYX is not NULL and YQYX is NULL then DQYX         \n");
                sql.Append("       when DQYX is not NULL and WLYX is NULL and YQYX is not NULL then DQYX         \n");
                sql.Append("       when DQYX  is NULL and WLYX is not NULL and YQYX is not NULL then WLYX         \n");
                sql.Append("       when DQYX  is NULL and WLYX is not NULL and YQYX is NULL then WLYX         \n");
                sql.Append("       when DQYX  is NULL and WLYX is NULL and YQYX is not NULL then YQYX         \n");
                sql.Append("       else DQYX end  as dTime         \n");
                sql.Append("       from (         \n");
                sql.Append("           \n");
                sql.Append("       select distinct A1.vcPartId,A1.vcReceiver,         \n");
                sql.Append("       DQYX,WLYX,YQYX          \n");
                sql.Append("       from(         \n");
                sql.Append("       select vcChanges,vcPartId,vcReceiver,dFromTime,dToTime,vcCarModel from TSPMaster where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime         \n");
                sql.Append("       )A1 left join         \n");
                sql.Append("    (       \n");
                sql.Append("    --当前有效       \n");
                sql.Append("    select vcpartid,dFromTime as DQYX from dbo.TSPMaster  where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime     \n");
                sql.Append("    and dFromTime<= getdate() and dToTime>=getdate()       \n");
                sql.Append("    )A2 on A1.vcPartId=A2.vcPartId       \n");
                sql.Append("    left join       \n");
                sql.Append("    (       \n");
                sql.Append("    --未来有效       \n");
                sql.Append("    select vcpartid,min(dFromTime) as WLYX from TSPMaster  where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime     \n");
                sql.Append("    and dFromTime > getdate()       \n");
                sql.Append("    group by vcpartid        \n");
                sql.Append("           \n");
                sql.Append("    )A3 on A1.vcPartId=A3.vcPartId       \n");
                sql.Append("    left join       \n");
                sql.Append("    (       \n");
                sql.Append("    --以前有效       \n");
                sql.Append("       select TC.vcpartid,dFromTime as YQYX from (       \n");
                sql.Append("       select vcpartid,max(dToTime) as dToTime from TSPMaster   where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime     \n");
                sql.Append("       and dToTime < getdate()        \n");
                sql.Append("       group by vcpartid        \n");
                sql.Append("       ) TC left join       \n");
                sql.Append("       (select vcpartid,dFromTime,dToTime from TSPMaster   where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime     \n");
                sql.Append("        ) TD on TC.vcpartid = TD.vcpartid and TC.dToTime = TD.dToTime      \n");
                sql.Append("        )A4 on A1.vcPartId=A4.vcPartId      \n");
                sql.Append("      ) TT      \n");

                sql.Append("      )temp0   	     \n");
                sql.Append("      group by temp0.vcPartId	     \n");


                sql.Append("     	 )Temp1 on temp.vcPartsNo=Temp1.vcPartId      \n");
                sql.Append("     	 left join      \n");
                sql.Append("     (      \n");
                sql.Append("     select vcChanges,vcPartId,vcReceiver,dFromTime,dToTime,vcCarModel,vcSupplierId from TSPMaster where isnull(vcDelete,'')<>'1'     \n");
                sql.Append("      and dFromTime<>dToTime       \n");
                sql.Append("     )Temp2 on Temp1.vcpartid=Temp2.vcpartid and Temp2.dFromTime=Temp1.dTime       \n");
                sql.Append("     left join      \n");
                sql.Append("     (      \n");
                sql.Append("      select vcPartId,vcReceiver,min(dFromTime) as dFromTime from TSPMaster   where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime        \n");
                sql.Append("      group by vcPartId,vcReceiver         \n");
                sql.Append("     )Temp3 on Temp1.vcPartId=Temp3.vcPartId and Temp2.vcReceiver=Temp3.vcReceiver      \n");
                sql.Append("     left join      \n");
                sql.Append("     (      \n");
                sql.Append("     select vcPartId,vcReceiver,max(dToTime) as dToTime from TSPMaster where isnull(vcDelete,'')<>'1'  and dFromTime<>dToTime         \n");
                sql.Append("     group by vcPartId,vcReceiver         \n");
                sql.Append("     )Temp4 on Temp1.vcPartId=Temp4.vcPartId and Temp2.vcReceiver=Temp4.vcReceiver      \n");



                sql.Append("    union All       \n");


                sql.Append("  select    distinct     \n");
                sql.Append("     Temp1.vcPartId as vcPartsNo,                     \n");
                sql.Append("     temp5.vcPackNo as [vcPackNo],                        \n");
                sql.Append("     temp5.vcPackGPSNo as [vcPackGPSNo],                     \n");
                sql.Append("     Temp2.vcReceiver as [vcShouhuofangID],         \n");
                sql.Append("     Temp2.vcCarModel as [vcCar],                   \n");
                sql.Append("     Temp3.dFromTime as [dUsedFrom],                \n");
                sql.Append("     Temp4.dToTime as [dUsedTo],                    \n");


                sql.Append("      case when isnull(temp5.dFrom,'')='' then NULL else temp5.dFrom end as [dFrom],                         \n");
                sql.Append("      case when isnull(temp5.dTo,'')='' then NULL else temp5.dTo end as [dTo],                           \n");
                sql.Append("      case when isnull(temp5.vcDistinguish,'')='' then '' else temp5.vcDistinguish end as [vcDistinguish],          \n");
                sql.Append("      case when isnull(temp5.iBiYao,0)=0 then NULL else temp5.iBiYao end  as [iBiYao],                            \n");


                sql.Append("      '' as vcPackSpot ,         \n");
                sql.Append("      Temp2.vcChanges as [varChangedItem]           \n");
                ///todo
                sql.Append("     ,'" + dtimenow + "' as  vcReTime       \n");

                sql.Append("   from         \n");
                sql.Append("   (         \n");
                sql.Append("     select Temp0.vcPartId,min(Temp0.dTime) as dTime from(       \n");
                sql.Append("        select vcpartid,        \n");
                sql.Append("        case         \n");
                sql.Append("        when DQYX is not NULL and WLYX is not NULL and YQYX is not NULL then DQYX          \n");
                sql.Append("        when DQYX is not NULL and WLYX is NULL and YQYX is NULL then DQYX        \n");
                sql.Append("        when DQYX is not NULL and WLYX is not NULL and YQYX is NULL then DQYX        \n");
                sql.Append("        when DQYX is not NULL and WLYX is NULL and YQYX is not NULL then DQYX        \n");
                sql.Append("        when DQYX  is NULL and WLYX is not NULL and YQYX is not NULL then WLYX        \n");
                sql.Append("        when DQYX  is NULL and WLYX is not NULL and YQYX is NULL then WLYX        \n");
                sql.Append("        when DQYX  is NULL and WLYX is NULL and YQYX is not NULL then YQYX        \n");
                sql.Append("        else DQYX end  as dTime        \n");
                sql.Append("        from (        \n");
                sql.Append("           \n");
                sql.Append("        select distinct A1.vcPartId,A1.vcReceiver,        \n");
                sql.Append("        DQYX,WLYX,YQYX         \n");
                sql.Append("        from(        \n");
                sql.Append("        select vcChanges,vcPartId,vcReceiver,dFromTime,dToTime,vcCarModel from TSPMaster where isnull(vcDelete,'')<>'1'  and dFromTime<>dToTime   \n");
                sql.Append("        )A1 left join   \n");
                sql.Append("        (   \n");
                sql.Append("        --当前有效   \n");
                sql.Append("        select vcpartid,dFromTime as DQYX from dbo.TSPMaster  where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime  \n");
                sql.Append("        and dFromTime<= getdate() and dToTime>=getdate()   \n");
                sql.Append("        )A2 on A1.vcPartId=A2.vcPartId   \n");
                sql.Append("        left join   \n");
                sql.Append("        (   \n");
                sql.Append("        --未来有效   \n");
                sql.Append("        select vcpartid,min(dFromTime) as WLYX from TSPMaster where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime \n");
                sql.Append("        and dFromTime > getdate()   \n");
                sql.Append("        group by vcpartid    \n");
                sql.Append("         )A3 on A1.vcPartId=A3.vcPartId   \n");
                sql.Append("         left join   \n");
                sql.Append("         (   \n");
                sql.Append("         --以前有效   \n");
                sql.Append("            select TC.vcpartid,dFromTime as YQYX from (   \n");
                sql.Append("            select vcpartid,max(dToTime) as dToTime from TSPMaster where isnull(vcDelete,'')<>'1'and   \n");
                sql.Append("            dToTime < getdate()  and dFromTime<>dToTime \n");
                sql.Append("            group by vcpartid    \n");
                sql.Append("            ) TC left join   \n");
                sql.Append("            (select vcpartid,dFromTime,dToTime from TSPMaster  where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime  \n");
                sql.Append("            ) TD on TC.vcpartid = TD.vcpartid and TC.dToTime = TD.dToTime   \n");
                sql.Append("          )A4 on A1.vcPartId=A4.vcPartId   \n");
                sql.Append("         ) TT   \n");
                sql.Append("          left join   \n");
                sql.Append("          (   \n");
                sql.Append("          select vcPartsNo+vcShouhuofangID as fkey from TPackItem   \n");
                sql.Append("       	  )A5 on TT.vcPartId+TT.vcReceiver=fkey where fkey is  NULL    \n");
                sql.Append("      )Temp0         \n");
                sql.Append("      group by Temp0.vcPartId     \n");
                sql.Append("       )Temp1     \n");
                sql.Append("       left join    \n");
                sql.Append("       (    \n");
                sql.Append("       select vcChanges,vcPartId,vcReceiver,dFromTime,dToTime,vcCarModel,vcSupplierId from TSPMaster where isnull(vcDelete,'')<>'1'   \n");
                sql.Append("        and dFromTime<>dToTime   \n");
                sql.Append("       )Temp2 on Temp1.vcpartid=Temp2.vcpartid and Temp2.dFromTime=Temp1.dTime     \n");
                sql.Append("       left join    \n");
                sql.Append("       (    \n");
                sql.Append("        select vcPartId,vcReceiver,min(dFromTime) as dFromTime from TSPMaster   where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime  \n");
                sql.Append("        group by vcPartId,vcReceiver       \n");
                sql.Append("       )Temp3 on Temp1.vcPartId=Temp3.vcPartId and Temp2.vcReceiver=Temp3.vcReceiver    \n");
                sql.Append("       left join    \n");
                sql.Append("       (    \n");
                sql.Append("       select vcPartId,vcReceiver,max(dToTime) as dToTime from TSPMaster   where isnull(vcDelete,'')<>'1' and dFromTime<>dToTime  \n");
                sql.Append("       group by vcPartId,vcReceiver       \n");
                sql.Append("       )Temp4 on Temp1.vcPartId=Temp4.vcPartId and Temp2.vcReceiver=Temp4.vcReceiver    \n");
                sql.Append("      left join   \n");
                sql.Append("      (   \n");
                sql.Append("      select * from TPackItemDelete   \n");
                sql.Append("      )temp5 on Temp1.vcPartId=temp5.vcPartsNo   \n");
                sql.Append("         \n");
                sql.Append("      )Table1 where Table1.dUsedFrom<>Table1.dUsedTo    \n");




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
                    sql.Append("   [varChangedItem] , vcPackSpot\n");
                    ///todo
                    sql.Append(" ,vcReTime  \n");

                    sql.Append("   ) values  \n");
                    sql.Append("   ( \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPartsNo"].ToString().TrimEnd() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPackNo"].ToString().TrimEnd() + "' ,  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPackGPSNo"].ToString() + "',    \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcShouhuofangID"].ToString() + "',   \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcCar"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dUsedFrom"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dUsedTo"].ToString() + "',  \n");
                    if (string.IsNullOrEmpty(dtNewItem.Rows[i]["dFrom"].ToString()))
                    {
                        sql.Append("  '1900-01-01 00:00:00',  \n");
                    }
                    else
                    {
                        sql.Append("  '" + dtNewItem.Rows[i]["dFrom"].ToString() + "',  \n");
                    }
                    if (string.IsNullOrEmpty(dtNewItem.Rows[i]["dTo"].ToString()))
                    {
                        sql.Append("  '9999-12-31 11:59:59',  \n");
                    }
                    else
                    {
                        sql.Append("  '" + dtNewItem.Rows[i]["dTo"].ToString() + "',  \n");
                    }
                    sql.Append("  '" + dtNewItem.Rows[i]["vcDistinguish"].ToString() + "',   \n");
                    if (string.IsNullOrEmpty(dtNewItem.Rows[i]["iBiYao"].ToString()))
                    {
                        sql.Append("  NULL,  \n");

                    }
                    else
                    {
                        sql.Append("  '" + dtNewItem.Rows[i]["iBiYao"].ToString() + "',  \n");

                    }


                    sql.Append("  '" + strUserId + "', \n");
                    sql.Append("   GETDATE(), \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["varChangedItem"].ToString() + "', \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPackSpot"].ToString() + "' \n");

                    ////todo
                    sql.Append("  ,'" + dtNewItem.Rows[i]["vcReTime"].ToString() + "' \n");


                    sql.Append("    ) \n");
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