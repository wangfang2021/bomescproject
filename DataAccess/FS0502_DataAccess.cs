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

        #region 取C056中2个状态
        public DataTable getTCode(string strCodeId)
        {
            try
            {
                MultiExcute excute = new MultiExcute();
                DataTable dt = new DataTable();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select vcName,vcValue from TCode where vcCodeId='" + strCodeId + "' and vcValue in ('1','2')  ORDER BY iAutoId    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search(string vcSupplier_id, string vcStatus, string vcOrderNo, string vcPart_id, string vcDelete)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcStatus,t2.vcName as vcStatusName,t1.vcOrderNo,t1.vcPart_id,t1.vcSupplier_id,    \n");
                strSql.Append("t1.vcGQ,t1.vcChuHePlant,t1.dReplyOverDate,t1.iPackingQty,t1.iOrderQuantity,t3.iDuiYingQuantity,     \n");
                strSql.Append("cast(t1.iOrderQuantity/(t1.iPackingQty*1.0) as decimal(18,1)) as decBoxes,t3.dDeliveryDate,'0' as vcModFlag,'0' as vcAddFlag,         \n");
                strSql.Append("CASE WHEN t1.vcStatus='1' and t1.vcDelete='0' then '0'  else '1' end as bSelectFlag,t1.vcDelete,        \n");
                strSql.Append("case when iOrderQuantity%iPackingQty<>0 then 'red' else '' end as boxColor,    \n");
                strSql.Append("case when t3.iDuiYingQuantity<t1.iOrderQuantity then 'red' else '' end as DuiYingQuantityColor    \n");
                strSql.Append("from(        \n");
                strSql.Append("	select * from TUrgentOrder         \n");
                strSql.Append("	where vcSupplier_id='" + vcSupplier_id + "'      \n");
                strSql.Append("	and vcStatus in ('1','2')--0:未发送  1:待回复   2:已回复   3:回复销售   \n");
                strSql.Append("	and vcStatus='" + vcStatus + "'    \n");
                strSql.Append("	and vcOrderNo like '%" + vcOrderNo + "%'    \n");
                strSql.Append("	and vcPart_id like '" + vcPart_id + "%'    \n");
                strSql.Append(" and vcDelete ='" + vcDelete + "' ");
                strSql.Append(")t1        \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C056')t2 on t1.vcStatus=t2.vcValue        \n");
                strSql.Append("left join (        \n");
                strSql.Append("    select * from VI_UrgentOrder_OperHistory_s where vcInputType='supplier'    \n");
                strSql.Append(")t3 on t1.vcOrderNo=t3.vcOrderNo and t1.vcPart_id=t3.vcPart_id     \n");
                strSql.Append("order by t1.vcOrderNo,t1.vcPart_id,t3.dDeliveryDate    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 分批纳入子画面检索数据,返回dt
        public DataTable SearchSub(string vcOrderNo, string vcPart_id, string vcSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.*,t2.iOrderQuantity,t2.vcStatus,'1' as vcModFlag,'0' as vcAddFlag,     \n");
                strSql.Append("cast(t1.iDuiYingQuantity/(t2.iPackingQty*1.0) as decimal(18,1)) as decBoxes    \n");
                strSql.Append("from    \n");
                strSql.Append("(    \n");
                strSql.Append("	select * from VI_UrgentOrder_OperHistory_s where vcInputType='supplier'        \n");
                strSql.Append(")t1    \n");
                strSql.Append("inner join (            \n");
                strSql.Append("    select * from TUrgentOrder    \n");
                strSql.Append("	where vcOrderNo='" + vcOrderNo + "' and vcPart_id='" + vcPart_id + "' and vcSupplier_id='" + vcSupplier_id + "'     \n");
                strSql.Append(")t2 on t1.vcOrderNo=t2.vcOrderNo and t1.vcPart_id=t2.vcPart_id       \n");

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
                strSql.Append("where iAutoId=" + iAutoId + " \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, string strautoid_main,
            string vcPart_id, string vcOrderNo, string vcSupplier_id, ref string infopart)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                string strdate = System.DateTime.Now.ToString();
                List<string> lsOrderNo = new List<string>();
                List<string> lsPart_id = new List<string>();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增  只有分批纳入时才会新增，strautoid_main 是从主画面带过来的所选数据的iautoid
                        if (listInfoData[i]["iDuiYingQuantity"].ToString() != "0")
                        {
                            if (lsOrderNo.Contains(vcOrderNo) == false)
                                lsOrderNo.Add(vcOrderNo);
                            if (lsPart_id.Contains(vcPart_id) == false)
                                lsPart_id.Add(vcPart_id);
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
                            sql.Append("select vcOrderNo,vcPart_id,vcSupplier_id,nullif(" + listInfoData[i]["iDuiYingQuantity"] + ",''),     \n");
                            sql.Append("nullif('" + listInfoData[i]["dDeliveryDate"] + "',''),'supplier','" + strUserId + "','" + strdate + "'     \n");
                            sql.Append("from TUrgentOrder where iAutoId=" + strautoid_main + "      \n");
                        }
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改  主画面的修改和分批导入的修改可以通用
                        if (listInfoData[i]["iDuiYingQuantity"].ToString() != "0")
                        {
                            if (lsOrderNo.Contains(listInfoData[i]["vcOrderNo"].ToString()) == false)
                                lsOrderNo.Add(listInfoData[i]["vcOrderNo"].ToString());
                            if (lsPart_id.Contains(listInfoData[i]["vcPart_id"].ToString()) == false)
                                lsPart_id.Add(listInfoData[i]["vcPart_id"].ToString());
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
                            sql.Append("           ('" + listInfoData[i]["vcOrderNo"] + "'    \n");
                            sql.Append("           ,'" + listInfoData[i]["vcPart_id"] + "'    \n");
                            sql.Append("           ,'" + listInfoData[i]["vcSupplier_id"] + "'   \n");
                            sql.Append("           ,nullif(" + listInfoData[i]["iDuiYingQuantity"] + ",'')   \n");
                            sql.Append("           ,nullif('" + listInfoData[i]["dDeliveryDate"] + "','')    \n");
                            sql.Append("           ,'supplier'    \n");
                            sql.Append("           ,'" + strUserId + "'    \n");
                            sql.Append("           ,'" + strdate + "')    \n");
                        }
                        else
                        {
                            string iAutoId = listInfoData[i]["iAutoId"].ToString();
                            sql.Append("delete from TUrgentOrder_OperHistory where iAutoId=" + iAutoId + "    \n");
                        }
                    }
                }
                if (sql.Length > 0)
                {
                    string strOrderNo = "";
                    for (int i = 0; i < lsOrderNo.Count; i++)
                        strOrderNo += "'" + lsOrderNo[i].ToString() + "',";
                    if (strOrderNo.Length > 0)
                        strOrderNo = strOrderNo.Substring(0, strOrderNo.Length - 1);

                    string strPart_id = "";
                    for (int i = 0; i < lsPart_id.Count; i++)
                        strPart_id += "'" + lsPart_id[i].ToString() + "',";
                    if (strPart_id.Length > 0)
                        strPart_id = strPart_id.Substring(0, strPart_id.Length - 1);
                    //以下追加验证数据库中是否存在可对应数量>订货总数，如果存在则终止提交
                    sql.Append("  DECLARE @errorPart varchar(4000)   \r\n");
                    sql.Append("  set @errorPart=''   \r\n");
                    sql.Append("  set @errorPart=(   \r\n");
                    sql.Append("  	select a.vcPart_id+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("       select distinct t1.vcPart_id from (    \r\n");
                    sql.Append("       	select * from TUrgentOrder where vcSupplier_id='" + vcSupplier_id + "' and  vcStatus='1' and vcDelete='0'   \r\n");
                    sql.Append("          and vcOrderNo in (" + strOrderNo + ") and vcPart_id in (" + strPart_id + ")    \n");
                    sql.Append("       )t1    \r\n");
                    sql.Append("       left join (            \r\n");
                    sql.Append("           select vcOrderNo,vcPart_id,sum(iDuiYingQuantity) as iDuiYingQuantity from VI_UrgentOrder_OperHistory_s     \r\n");
                    sql.Append("       	where vcInputType='supplier'      \r\n");
                    sql.Append("       	group by vcOrderNo,vcPart_id    \r\n");
                    sql.Append("       )t2 on t1.vcOrderNo=t2.vcOrderNo and t1.vcPart_id=t2.vcPart_id        \r\n");
                    sql.Append("       where t2.iDuiYingQuantity>t1.iOrderQuantity    \r\n");
                    sql.Append("  	)a for xml path('')   \r\n");
                    sql.Append("  )   \r\n");
                    sql.Append("      \r\n");
                    sql.Append("  if @errorPart<>''  \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select CONVERT(int,'-->'+@errorPart+'<--')   \r\n");
                    sql.Append("  end    \r\n");
                    excute.ExcuteSqlWithStringOper(sql.ToString());

                    sql = new StringBuilder();
                    //验证箱数是否为小数
                    sql.Append("  DECLARE @errorPart2 varchar(4000)   \r\n");
                    sql.Append("  set @errorPart2=''   \r\n");
                    sql.Append("  set @errorPart2=(   \r\n");
                    sql.Append("  	select a.vcPart_id+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("        select distinct t1.vcPart_id     \n");
                    sql.Append("        from    \n");
                    sql.Append("        (    \n");
                    sql.Append("        	select * from VI_UrgentOrder_OperHistory_s where vcInputType='supplier'        \n");
                    sql.Append("        )t1    \n");
                    sql.Append("        inner join (            \n");
                    sql.Append("            select * from TUrgentOrder    \n");
                    sql.Append("        	where vcOrderNo in (" + strOrderNo + ") and vcPart_id in (" + strPart_id + ") and vcSupplier_id='" + vcSupplier_id + "'     \n");
                    sql.Append("        )t2 on t1.vcOrderNo=t2.vcOrderNo and t1.vcPart_id=t2.vcPart_id       \n");
                    sql.Append("        where t2.iOrderQuantity%t2.iPackingQty=0 and t1.iDuiYingQuantity%t2.iPackingQty<>0   \r\n");
                    sql.Append("  	)a for xml path('')   \r\n");
                    sql.Append("  )   \r\n");
                    sql.Append("  if @errorPart2<>''  \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select @errorPart2   \r\n");
                    sql.Append("  end    \r\n");
                    sql.Append("  else  \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select 'no'   \r\n");
                    sql.Append("  end    \r\n");
                    DataTable temp = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                    infopart = "";
                    for (int i = 0; i < temp.Rows.Count; i++)
                    {
                        if (temp.Rows[i][0].ToString() != "no")
                            infopart += "'" + temp.Rows[i][0].ToString() + "',";
                    }
                    if (infopart.Length > 0)
                        infopart = infopart.Substring(0, infopart.Length - 1);
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
        public DataSet IsDQR(List<Dictionary<string, Object>> listInfoData, string strType)//strType:save(保存)、submit(提交)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      if object_id('tempdb..#TUrgentOrder_temp_cr') is not null       \n");
                strSql.Append("      Begin      \n");
                strSql.Append("      drop  table #TUrgentOrder_temp_cr       \n");
                strSql.Append("      End      \n");
                strSql.Append("     select * into #TUrgentOrder_temp_cr from                \n");
                strSql.Append("     (               \n");
                strSql.Append("     	select t1.vcPart_id,t1.vcStatus,t3.iDuiYingQuantity,t3.dDeliveryDate,t1.vcDelete from TUrgentOrder t1         \n");
                strSql.Append("     	left join (select * from VI_UrgentOrder_OperHistory_s where vcInputType='supplier' )t3         \n");
                strSql.Append("     	on t1.vcOrderNo=t3.vcOrderNo and t1.vcPart_id=t3.vcPart_id           \n");
                strSql.Append("     	where 1=0               \n");
                strSql.Append("     ) a      ;         \n");

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
                            strSql.Append("       vcPart_id,vcStatus,iDuiYingQuantity,dDeliveryDate,vcDelete        \n");
                            strSql.Append("       ) values         \n");
                            strSql.Append("      (      \n");
                            strSql.Append("       '" + listInfoData[i]["vcPart_id"].ToString() + "',     \n");
                            strSql.Append("       '" + listInfoData[i]["vcStatus"].ToString() + "',     \n");
                            strSql.Append("       nullif('" + listInfoData[i]["iDuiYingQuantity"] + "',''),     \n");
                            strSql.Append("       nullif('" + listInfoData[i]["dDeliveryDate"] + "',''),     \n");
                            strSql.Append("       '" + listInfoData[i]["vcDelete"].ToString() + "'     \n");
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
                        strSql.Append("       vcPart_id,vcStatus,iDuiYingQuantity,dDeliveryDate,vcDelete        \n");
                        strSql.Append("       ) values         \n");
                        strSql.Append("      (      \n");
                        strSql.Append("       '" + listInfoData[i]["vcPart_id"].ToString() + "',     \n");
                        strSql.Append("       '" + listInfoData[i]["vcStatus"].ToString() + "',     \n");
                        strSql.Append("       nullif('" + listInfoData[i]["iDuiYingQuantity"] + "',''),     \n");
                        strSql.Append("       nullif('" + listInfoData[i]["dDeliveryDate"] + "',''),     \n");
                        strSql.Append("       '" + listInfoData[i]["vcDelete"].ToString() + "'     \n");
                        strSql.Append("      );      \n");
                        #endregion
                    }
                }
                strSql.Append(" select * from #TUrgentOrder_temp_cr where vcStatus!='1' or vcDelete!='0'  ");//这几个状态(1)是可操作的状态
                strSql.Append(" select * from #TUrgentOrder_temp_cr where iDuiYingQuantity is null or dDeliveryDate is null   \n");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 是否可操作-按检索条件
        public DataSet IsDQR(string vcSupplier_id, string vcStatus, string vcOrderNo, string vcPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append(" select * from TUrgentOrder ");
                strSql.Append(" WHERE (vcStatus!='1' or vcDelete!='0') ");//这几个状态(1)是可操作的状态
                strSql.Append("	and vcSupplier_id='" + vcSupplier_id + "'      \n");
                strSql.Append("	and vcStatus in ('1','2')--1:待回复   2:已回复    \n");
                strSql.Append("	and vcStatus='" + vcStatus + "'    \n");
                strSql.Append("	and vcOrderNo like '%" + vcOrderNo + "%'    \n");
                strSql.Append("	and vcPart_id like '%" + vcPart_id + "%'    \n");

                strSql.Append("select * from (   \n");
                strSql.Append("  select * from TUrgentOrder ");
                strSql.Append("  WHERE (vcStatus='1' and vcDelete='0') ");//这几个状态(1)是可操作的状态
                strSql.Append(" 	and vcSupplier_id='" + vcSupplier_id + "'      \n");
                strSql.Append(" 	and vcStatus in ('1','2')--1:待回复   2:已回复    \n");
                strSql.Append(" 	and vcStatus='" + vcStatus + "'    \n");
                strSql.Append(" 	and vcOrderNo like '%" + vcOrderNo + "%'    \n");
                strSql.Append(" 	and vcPart_id like '%" + vcPart_id + "%'    \n");
                strSql.Append(")t1        \n");
                strSql.Append("left join (        \n");
                strSql.Append("    select * from VI_UrgentOrder_OperHistory_s where vcInputType='supplier'    \n");
                strSql.Append(")t3 on t1.vcOrderNo=t3.vcOrderNo and t1.vcPart_id=t3.vcPart_id     \n");
                strSql.Append("where t3.iDuiYingQuantity is null or t3.dDeliveryDate is null    \n");

                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 提交-按列表所选数据
        public int ok(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
                StringBuilder strSql = new StringBuilder();
                strSql.Append("if object_id('tempdb..#TUrgentOrder_temp_cr') is not null       \n");
                strSql.Append("Begin      \n");
                strSql.Append("drop  table #TUrgentOrder_temp_cr       \n");
                strSql.Append("End      \n");
                strSql.Append("select * into #TUrgentOrder_temp_cr from       \n");
                strSql.Append("(      \n");
                strSql.Append("	select vcOrderNo,vcPart_id,vcSupplier_id from TUrgentOrder where 1=0      \n");
                strSql.Append(") a      ;\n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    #region 将所有的数据都插入临时表
                    strSql.Append("insert into #TUrgentOrder_temp_cr       \n");
                    strSql.Append(" (         \n");
                    strSql.Append(" vcOrderNo,vcPart_id,vcSupplier_id        \n");
                    strSql.Append(" ) values         \n");
                    strSql.Append("(      \n");
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], false) + ",   \n");
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + ",   \n");
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "   \n");
                    strSql.Append(")      \n");
                    #endregion
                }
                //更新对应状态为已回复，更新提交人和提单时间
                strSql.Append(" UPDATE TUrgentOrder SET ");
                strSql.Append("      vcStatus='2', "); //0：未发送；1：待回复；2：已回复；3：回复销售
                strSql.Append("      vcSupReplier='" + strUserId + "', ");
                strSql.Append("      dSupReplyTime=getdate(), ");
                strSql.Append("      vcOperatorID='" + strUserId + "', ");
                strSql.Append("      dOperatorTime=getDate() ");
                strSql.Append(" from TUrgentOrder a  \n ");
                strSql.Append(" inner join  \n ");
                strSql.Append(" (  \n ");
                strSql.Append("    select * from #TUrgentOrder_temp_cr  \n ");
                strSql.Append(" )b on a.vcOrderNo=b.vcOrderNo and a.vcPart_id=b.vcPart_id and a.vcSupplier_id=b.vcSupplier_id   \n ");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 提交-按检索条件
        public int ok(string vcSupplier_id, string vcStatus, string vcOrderNo, string vcPart_id, string strUserId)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine(" UPDATE TUrgentOrder SET ");
                strSql.Append("      vcStatus='2', ");//0：未发送；1：待回复；2：已回复；3：回复销售
                strSql.Append("      vcSupReplier='" + strUserId + "', ");
                strSql.Append("      dSupReplyTime=getdate(), ");
                strSql.Append("      vcOperatorID='" + strUserId + "', ");
                strSql.Append("      dOperatorTime=getDate() ");
                strSql.Append(" WHERE (vcStatus='1' and vcDelete='0') ");//这几个状态(1)是可操作的状态
                strSql.Append("	and vcSupplier_id='" + vcSupplier_id + "'      \n");
                strSql.Append("	and vcStatus in ('1','2')--1:待回复   2:已回复    \n");
                strSql.Append("	and vcStatus='" + vcStatus + "'    \n");
                strSql.Append("	and vcOrderNo like '%" + vcOrderNo + "%'    \n");
                strSql.Append("	and vcPart_id like '%" + vcPart_id + "%'    \n");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 分批导入子画面删除  不用
        public void DelSub(List<Dictionary<string, Object>> checkedInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from TUrgentOrder where iAutoId=" + iAutoId + "   \n");
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

    }
}