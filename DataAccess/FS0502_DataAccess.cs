using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DataAccess
{
    public class FS0502_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件检索,返回dt
        public DataTable Search(string vcSupplier_id, string vcStatus, string vcOrderNo, string vcPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcStatus,t2.vcName as vcStatusName,t1.vcOrderNo,t1.vcPart_id,t1.vcSupplier_id,    \n");
                strSql.Append("t1.vcGQ,t1.vcChuHePlant,t1.dReplyOverDate,iOrderQuantity,t3.iDuiYingQuantity,t3.dDeliveryDate,     \n");
                strSql.Append("'0' as vcModFlag,'0' as vcAddFlag,         \n");
                strSql.Append("CASE WHEN vcStatus='1' then '0'  else '1' end as bSelectFlag        \n");
                strSql.Append("from(        \n");
                strSql.Append("	select * from TUrgentOrder         \n");
                strSql.Append("	where vcSupplier_id='" + vcSupplier_id + "'      \n");
                strSql.Append("	and vcStatus in ('1','2')--1:待回复   2:已回复    \n");
                strSql.Append("	and vcStatus='"+vcStatus+"'    \n");
                strSql.Append("	and vcOrderNo like '%"+vcOrderNo+"%'    \n");
                strSql.Append("	and vcPart_id like '%"+vcPart_id+"%'    \n");
                strSql.Append(")t1        \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C056')t2 on t1.vcStatus=t2.vcValue        \n");
                strSql.Append("left join (        \n");
                strSql.Append("	select a.vcOrderNo,a.vcPart_id,a.vcSupplier_id,a.dDeliveryDate,a.iDuiYingQuantity    \n");
                strSql.Append("	from (        \n");
                strSql.Append("		select * from TUrgentOrder_OperHistory where vcInputType='supplier'        \n");
                strSql.Append("	)a        \n");
                strSql.Append("	inner join (        \n");
                strSql.Append("		select vcOrderNo,vcPart_id,vcSupplier_id,dDeliveryDate,    \n");
                strSql.Append("		MAX(dOperatorTime) as dOperatorTime from TUrgentOrder_OperHistory         \n");
                strSql.Append("		where vcInputType='supplier'        \n");
                strSql.Append("		group by vcOrderNo,vcPart_id,vcSupplier_id,dDeliveryDate        \n");
                strSql.Append("	)b on a.vcOrderNo=b.vcOrderNo and a.vcPart_id=b.vcPart_id and a.vcSupplier_id=b.vcSupplier_id     \n");
                strSql.Append("	and a.dDeliveryDate=b.dDeliveryDate and a.dOperatorTime=b.dOperatorTime           \n");
                strSql.Append(")t3 on t1.vcOrderNo=t3.vcOrderNo and t1.vcPart_id=t3.vcPart_id and t1.vcSupplier_id=t3.vcSupplier_id    \n");
                strSql.Append("and t1.dDeliveryDate=t3.dDeliveryDate    \n");
                strSql.Append("order by t1.vcOrderNo,t1.vcPart_id,t1.vcSupplier_id,t3.dDeliveryDate    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 子画面初始化
        public DataTable initSubApi(string iAutoId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from TUrgentOrder  \n");
                strSql.Append("where iAutoId="+ iAutoId + " \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                string strdate = System.DateTime.Now.ToString();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        //插历史
                        sql.Append("INSERT INTO [TUrgentOrder_OperHistory]    \n");
                        sql.Append("           ([vcOrderNo]    \n");
                        sql.Append("           ,[vcPart_id]    \n");
                        sql.Append("           ,[vcSupplier_id]    \n");
                        sql.Append("           ,[iDuiYingQuantity]    \n");
                        sql.Append("           ,[dDeliveryDate]    \n");
                        sql.Append("           ,[vcInputType]    \n");
                        sql.Append("           ,[vcOperatorID]    \n");
                        sql.Append("           ,[dOperatorTime])    \n");
                        sql.Append("     VALUES    \n");
                        sql.Append("           ('"+ listInfoData[i]["vcOrderNo"] + "'    \n");
                        sql.Append("           ,'"+listInfoData[i]["vcPart_id"] +"'    \n");
                        sql.Append("           ,'" + listInfoData[i]["vcSupplier_id"] + "'   \n");
                        sql.Append("           ,nullif(" + listInfoData[i]["iDuiYingQuantity"] + ",'')   \n");
                        sql.Append("           ,nullif('" + listInfoData[i]["dDeliveryDate"] + "','')    \n");
                        sql.Append("           ,'supplier'    \n");
                        sql.Append("           ,'"+strUserId+"'    \n");
                        sql.Append("           ,'"+strdate+"')    \n");
                        //更新TUrgentOrder
                        sql.Append("update TUrgentOrder set iDuiYingQuantity=nullif(" + listInfoData[i]["iDuiYingQuantity"] + ",''),    \n");
                        sql.Append("dDeliveryDate=nullif('" + listInfoData[i]["dDeliveryDate"] + "',''),    \n");
                        sql.Append("vcOperatorID='"+strUserId+"',dOperatorTime='"+ strdate + "'    \n");
                        sql.Append("where iAutoId="+iAutoId+"    \n");
                    }
                }
                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在可对应数量与订货总数不同判断，如果存在则终止提交
                    sql.Append("  DECLARE @errorPart varchar(4000)   \r\n");
                    sql.Append("  set @errorPart=''   \r\n");
                    sql.Append("  set @errorPart=(   \r\n");
                    sql.Append("  	select a.vcPart_id+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("        select distinct vcPart_id from (    \r\n");
                    sql.Append("        	select vcOrderNo,vcPart_id,vcSupplier_id,iOrderQuantity,SUM(iDuiYingQuantity) as quanlity from TUrgentOrder    \r\n");
                    sql.Append("        	group by vcOrderNo,vcPart_id,vcSupplier_id,iOrderQuantity    \r\n");
                    sql.Append("        )t1    \r\n");
                    sql.Append("        where iOrderQuantity<>quanlity    \r\n");
                    sql.Append("  	)a for xml path('')   \r\n");
                    sql.Append("  )   \r\n");
                    sql.Append("      \r\n");
                    sql.Append("  if @errorPart<>''   \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select CONVERT(int,'-->'+@errorPart+'<--')   \r\n");
                    sql.Append("  end    \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion

        #region 是否可操作-按列表所选数据
        public DataTable IsDQR(List<Dictionary<string, Object>> listInfoData, string strType)//strType:save(保存)、submit(提交)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      if object_id('tempdb..#TUrgentOrder_temp_cr') is not null       \n");
                strSql.Append("      Begin      \n");
                strSql.Append("      drop  table #TUrgentOrder_temp_cr       \n");
                strSql.Append("      End      \n");
                strSql.Append("      select * into #TUrgentOrder_temp_cr from       \n");
                strSql.Append("      (      \n");
                strSql.Append("      	select vcPart_id,vcStatus from TUrgentOrder where 1=0      \n");
                strSql.Append("      ) a      ;\n");
                if (strType == "save")
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                        bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                        if (baddflag == true)
                        {//新增
                         //没有新增情况
                        }
                        else if (baddflag == false && bmodflag == true)
                        {//修改
                            #region 将所有的数据都插入临时表
                            strSql.Append("      insert into #TUrgentOrder_temp_cr       \n");
                            strSql.Append("       (         \n");
                            strSql.Append("       vcPart_id,vcStatus        \n");
                            strSql.Append("       ) values         \n");
                            strSql.Append("      (      \n");
                            strSql.Append("       '" + listInfoData[i]["vcPart_id"].ToString() + "',     \n");
                            strSql.Append("       '" + listInfoData[i]["vcStatus"].ToString() + "'     \n");
                            strSql.Append("      );      \n");
                            #endregion
                        }
                    }
                }
                else if (strType == "submit")
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        #region 将所有的数据都插入临时表
                        strSql.Append("      insert into #TUrgentOrder_temp_cr       \n");
                        strSql.Append("       (         \n");
                        strSql.Append("       vcPart_id,vcStatus        \n");
                        strSql.Append("       ) values         \n");
                        strSql.Append("      (      \n");
                        strSql.Append("       '" + listInfoData[i]["vcPart_id"].ToString() + "',     \n");
                        strSql.Append("       '" + listInfoData[i]["vcStatus"].ToString() + "'     \n");
                        strSql.Append("      );      \n");
                        #endregion
                    }
                }
                strSql.AppendLine(" select * from #TUrgentOrder_temp_cr where vcStatus!='1'  ");//这几个状态(1)是可操作的状态
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}