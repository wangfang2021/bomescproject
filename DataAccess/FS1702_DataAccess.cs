using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;

namespace DataAccess
{
    public class FS1702_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索所有的工程下拉框选择
        public DataTable getAllProject()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct vcProject as vcValue,vcProject as vcName from TChuHe_Detail  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索
        public DataTable Search(string vcProject, string dChuHeDateFrom, string dChuHeDateTo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select iAutoId,vcQueRenNo,vcProject,dChuHeDate,iQuantity,    \n");
                strSql.Append("case when dQueRenPrintTime is not null then '√' else '' end as QueRenPrintFlag,    \n");
                strSql.Append("case when vcQueRenNo like 'BJW%' then '━' when dKBPrintTime is not null then '√' else '' end as KBPrintFlag,    \n");
                strSql.Append("case when dChuHeOKTime is not null then '√' else '' end as ChuHeOKFlag    \n");
                strSql.Append("from TChuHe where 1=1   \n");
                if (vcProject != "" && vcProject != null)
                    strSql.Append("and vcProject = '" + vcProject + "'    \n");
                if (dChuHeDateFrom == "" || dChuHeDateFrom == null)
                    dChuHeDateFrom = "2001/01/01";
                if (dChuHeDateTo == "" || dChuHeDateTo == null)
                    dChuHeDateTo = "2099/12/31";
                strSql.Append("and dChuHeDate between '" + dChuHeDateFrom + "' and '" + dChuHeDateTo + "'  \n");
                strSql.Append("order by vcQueRenNo,dChuHeDate   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Search_jinji(string vcPart_id)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select *,iQuantity as iQuantity_old,'0' as vcModFlag,'0' as vcAddFlag from tChuHe_jinji   \n");
                if (vcPart_id != "" && vcPart_id != null)
                    sql.Append("where vcPart_id like '" + vcPart_id + "%'    \n");
                sql.Append("order by vcPart_id    \n");

                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Search_kb()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select *,'0' as vcModFlag,'0' as vcAddFlag from TChuHe_KB    \n");
                sql.Append("order by vcPart_id    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, ref string strErrorName)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [TChuHe_Detail_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    #region insert 子表 临时表
                    sql.Append("INSERT INTO [TChuHe_Detail_Temp]    \n");
                    sql.Append("           ([vcProject]    \n");
                    sql.Append("           ,[vcPart_id]    \n");
                    sql.Append("           ,[dChuHeDate]    \n");
                    sql.Append("           ,[iQuantity]    \n");
                    sql.Append("           ,[vcOperatorID]    \n");
                    sql.Append("           ,[dOperatorTime])    \n");
                    sql.Append("     VALUES    \n");
                    sql.Append("           ('" + dt.Rows[i]["vcProject"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcPart_id"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["dChuHeDate"].ToString() + "'    \n");
                    string qty = "";
                    if (dt.Rows[i]["iQuantity"].ToString() == "")
                        qty = "0";
                    else
                        qty = dt.Rows[i]["iQuantity"].ToString();
                    sql.Append("           ,'" + qty + "'    \n");
                    sql.Append("           ,'" + strUserId + "'    \n");
                    sql.Append("           ,getdate())    \n");
                    #endregion
                }
                #region insert 子表 sql
                sql.Append("INSERT INTO [TChuHe_Detail]    \n");
                sql.Append("           ([vcProject]    \n");
                sql.Append("           ,[vcPart_id]    \n");
                sql.Append("           ,[dChuHeDate]    \n");
                sql.Append("           ,[iQuantity]    \n");
                sql.Append("           ,[vcOperatorID]    \n");
                sql.Append("           ,[dOperatorTime])    \n");
                sql.Append("SELECT t1.[vcProject]    \n");
                sql.Append("      ,t1.[vcPart_id]    \n");
                sql.Append("      ,t1.[dChuHeDate]    \n");
                sql.Append("      ,t1.[iQuantity]    \n");
                sql.Append("      ,t1.[vcOperatorID]    \n");
                sql.Append("      ,t1.[dOperatorTime]    \n");
                sql.Append("  FROM [TChuHe_Detail_Temp] t1    \n");
                sql.Append("left join [TChuHe_Detail] t2    \n");
                sql.Append("on t1.vcPart_id=t2.vcPart_id and t1.dChuHeDate=t2.dChuHeDate    \n");
                sql.Append("where t2.iAutoId is null and t1.vcOperatorID='" + strUserId + "'     \n");
                #endregion

                #region update 子表 sql
                sql.Append("UPDATE t2    \n");
                sql.Append("   SET t2.[vcProject] = t1.[vcProject]    \n");
                sql.Append("      ,t2.[vcPart_id] = t1.[vcPart_id]    \n");
                sql.Append("      ,t2.[dChuHeDate] = t1.[dChuHeDate]    \n");
                sql.Append("      ,t2.[iQuantity] = t1.[iQuantity]    \n");
                sql.Append("      ,t2.[vcOperatorID] = t1.[vcOperatorID]    \n");
                sql.Append("      ,t2.[dOperatorTime] =t1.[dOperatorTime]    \n");
                sql.Append("from    \n");
                sql.Append("(select * from [TChuHe_Detail_Temp])t1     \n");
                sql.Append("inner join TChuHe_Detail t2      \n");
                sql.Append("on t1.vcPart_id=t2.vcPart_id and t1.dChuHeDate=t2.dChuHeDate    \n");
                sql.Append("where t1.vcOperatorID='" + strUserId + "'     \n");
                #endregion

                #region insert 主表 sql
                sql.Append("INSERT INTO [TChuHe]    \n");
                sql.Append("           ([vcQueRenNo]    \n");
                sql.Append("           ,[vcProject]    \n");
                sql.Append("           ,[dChuHeDate]    \n");
                sql.Append("           ,[iQuantity]    \n");
                sql.Append("           ,[dQueRenPrintTime]    \n");
                sql.Append("           ,[dKBPrintTime]    \n");
                sql.Append("           ,[dChuHeOKTime]    \n");
                sql.Append("           ,[vcOperatorID]    \n");
                sql.Append("           ,[dOperatorTime])    \n");
                sql.Append("select t1.vcQueRenNo,t1.vcProject,t1.dChuHeDate,t1.iQuantity,null,null,null,'" + strUserId + "',GETDATE()     \n");
                sql.Append("from (    \n");
                sql.Append("	select t1.vcQueRenNo,t1.vcProject,t1.dChuHeDate,sum(t1.iQuantity) as iQuantity     \n");
                sql.Append("	from (    \n");
                sql.Append("		select vcProject+replace(CONVERT(varchar(10),dChuHeDate,120),'-','') as vcQueRenNo,iQuantity,dChuHeDate,vcProject     \n");
                sql.Append("		from TChuHe_Detail    \n");
                sql.Append("	)t1    \n");
                sql.Append("	group by t1.vcQueRenNo,t1.dChuHeDate,t1.vcProject    \n");
                sql.Append(")t1    \n");
                sql.Append("left join TChuHe t2 on t1.vcQueRenNo=t2.vcQueRenNo    \n");
                sql.Append("where t2.iAutoId is null and t1.iQuantity!=0    \n");
                #endregion

                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 确认单打印
        public void qrdPrint(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DateTime time = DateTime.Now;
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();

                    sql.Append("update TChuHe set dQueRenPrintTime='" + time + "' where iAutoId=" + iAutoId + "   \n");
                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 出荷看板打印
        public void kbPrint(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DateTime time = DateTime.Now;
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();

                    sql.Append("update TChuHe set dKBPrintTime='" + time + "' where iAutoId=" + iAutoId + "   \n");
                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 出荷完了
        public void chuheOK(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DateTime time = DateTime.Now;
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    string vcProject = checkedInfoData[i]["vcProject"].ToString();
                    string dChuHeDate = checkedInfoData[i]["dChuHeDate"].ToString();

                    sql.Append("update TChuHe set dChuHeOKTime='" + time + "' where iAutoId=" + iAutoId + "   \n");

                    #region update TPanDian 数量减少
                    sql.AppendLine("update t2 set t2.iSystemQuantity=isnull(t2.iSystemQuantity,0)-isnull(t1.iQuantity,0),");
                    sql.AppendLine("t2.vcOperatorID='" + strUserId + "',t2.dOperatorTime='" + time + "'");
                    sql.AppendLine("from (");
                    sql.AppendLine("	select t2.vcNaRuPart_id,t1.iQuantity from (");
                    sql.AppendLine("		select * from TChuHe_Detail where vcProject='" + vcProject + "' and dChuHeDate='" + dChuHeDate + "'");
                    sql.AppendLine("	)t1");
                    sql.AppendLine("	inner join TSSPManagement t2 on t1.vcPart_id=t2.vcChuHePart_id");
                    sql.AppendLine(")t1");
                    sql.AppendLine("inner join (");
                    sql.AppendLine("	select * from TPanDian ");
                    sql.AppendLine(")t2 on t1.vcNaRuPart_id=t2.vcPart_id");
                    #endregion
                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取出确认单信息
        public DataTable GetqrdInfo(string vcProject, string dChuHeDate)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select ROW_NUMBER() over(order by t1.vcPart_id) as id,t1.vcPart_id,t1.iQuantity,t2.vcBackPart_id,'' as vcRemark from (    \n");
                sql.Append("	select * from TChuHe_Detail where vcProject='" + vcProject + "' and dChuHeDate='" + dChuHeDate + "' and iQuantity>0    \n");
                sql.Append(")t1    \n");
                sql.Append("left join TSSPManagement t2 on t1.vcPart_id=t2.vcChuHePart_id    \n");

                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取出看板信息
        public DataTable getKBData(string vcProject, string dChuHeDate)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select '补给品中心' as vcSupplierName,t1.vcCarType,t1.vcProject,t1.vcProjectPlace,t1.vcSR,t1.vcBackPart_id,    \n");
                sql.Append("t1.vcChuHePart_id,t1.vcPart_Name,t1.iCapacity,t1.vcBoxType    \n");
                sql.Append("from TSSPManagement t1    \n");
                sql.Append("inner join     \n");
                sql.Append("(    \n");
                sql.Append("	select * from TChuHe_Detail where vcProject='2W' and dChuHeDate='2021-01-29 00:00:00.000' and iQuantity>0    \n");
                sql.Append(")t2 on t1.vcProject=t2.vcPart_id    \n");

                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getKBData(string vcPart_id)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select '补给品中心' as vcSupplierName,vcCarType,vcProject,vcProjectPlace,vcSR,vcBackPart_id,    \n");
                sql.Append("vcChuHePart_id,vcPart_Name,iCapacity,vcBoxType    \n");
                sql.Append("from TSSPManagement where vcChuHePart_id='" + vcPart_id + "'    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save_jinji(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                DateTime time = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == true)
                    {//新增
                        sql.Append("insert into tChuHe_jinji (vcPart_id,iQuantity,vcReason,vcOperatorID,dOperatorTime) values   \n");
                        sql.Append("('" + listInfoData[i]["vcPart_id"].ToString() + "',nullif('" + listInfoData[i]["iQuantity"].ToString() + "','')," +
                            "'" + listInfoData[i]["vcReason"].ToString() + "','" + strUserId + "',getdate())  \n");

                        #region update TPanDian 数量减少
                        sql.AppendLine("update t2 set t2.iSystemQuantity=isnull(t2.iSystemQuantity,0)-isnull(t1.iQuantity,0),");
                        sql.AppendLine("t2.vcOperatorID='" + strUserId + "',t2.dOperatorTime='" + time + "'");
                        sql.AppendLine("from (");
                        sql.AppendLine("	select t2.vcNaRuPart_id,t1.iQuantity from (");
                        sql.AppendLine("		select * from TChuHe_jinji where vcPart_id='" + listInfoData[i]["vcPart_id"].ToString() + "' ");
                        sql.AppendLine("	)t1");
                        sql.AppendLine("	inner join TSSPManagement t2 on t1.vcPart_id=t2.vcChuHePart_id");
                        sql.AppendLine(")t1");
                        sql.AppendLine("inner join (");
                        sql.AppendLine("	select * from TPanDian ");
                        sql.AppendLine(")t2 on t1.vcNaRuPart_id=t2.vcPart_id");
                        #endregion
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        int qty_old = Convert.ToInt32(listInfoData[i]["iQuantity_old"].ToString());
                        int qty_now = Convert.ToInt32(listInfoData[i]["iQuantity"].ToString());
                        int cha = qty_old - qty_now;

                        sql.Append("update tChuHe_jinji set vcPart_id='" + listInfoData[i]["vcPart_id"].ToString() + "'," +
                            "iQuantity=nullif('" + listInfoData[i]["iQuantity"].ToString() + "',''),vcReason='" + listInfoData[i]["vcReason"].ToString() + "'," +
                            "vcOperatorID='" + strUserId + "',dOperatorTime=getdate()   \n");

                        if (cha != 0)
                        {//出多了或出少了，库存加回去或减回去
                            #region update TPanDian 数量增加
                            sql.AppendLine("update t2 set t2.iSystemQuantity=isnull(t2.iSystemQuantity,0)+" + cha + ",");
                            sql.AppendLine("t2.vcOperatorID='" + strUserId + "',t2.dOperatorTime='" + time + "'");
                            sql.AppendLine("from (");
                            sql.AppendLine("	select t2.vcNaRuPart_id,t1.iQuantity from (");
                            sql.AppendLine("		select * from TChuHe_jinji where vcPart_id='" + listInfoData[i]["vcPart_id"].ToString() + "' ");
                            sql.AppendLine("	)t1");
                            sql.AppendLine("	inner join TSSPManagement t2 on t1.vcPart_id=t2.vcChuHePart_id");
                            sql.AppendLine(")t1");
                            sql.AppendLine("inner join (");
                            sql.AppendLine("	select * from TPanDian ");
                            sql.AppendLine(")t2 on t1.vcNaRuPart_id=t2.vcPart_id");
                            #endregion
                        }

                    }
                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Save_kb(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == true)
                    {//新增
                        sql.Append("INSERT INTO [TChuHe_KB]    \n");
                        sql.Append("           ([vcPart_id]    \n");
                        sql.Append("           ,[vcProject]    \n");
                        sql.Append("           ,[iQuantity]    \n");
                        sql.Append("           ,[iKBQuantity]    \n");
                        sql.Append("           ,[vcOperatorID]    \n");
                        sql.Append("           ,[dOperatorTime])    \n");
                        sql.Append("     VALUES    \n");
                        sql.Append("           ('" + listInfoData[i]["vcPart_id"].ToString() + "'    \n");
                        sql.Append("           ,'" + listInfoData[i]["vcProject"].ToString() + "'    \n");
                        sql.Append("           ,nullif('" + listInfoData[i]["iQuantity"].ToString() + "','')    \n");
                        sql.Append("           ,nullif('" + listInfoData[i]["iKBQuantity"].ToString() + "','')    \n");
                        sql.Append("           ,'" + strUserId + "'    \n");
                        sql.Append("           ,getdate())    \n");
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("UPDATE [TChuHe_KB]    \n");
                        sql.Append("   SET [vcPart_id] = '" + listInfoData[i]["vcPart_id"].ToString() + "'    \n");
                        sql.Append("      ,[vcProject] = '" + listInfoData[i]["vcProject"].ToString() + "'    \n");
                        sql.Append("      ,[iQuantity] = nullif('" + listInfoData[i]["iQuantity"].ToString() + "','')    \n");
                        sql.Append("      ,[iKBQuantity] = nullif('" + listInfoData[i]["iKBQuantity"].ToString() + "','')    \n");
                        sql.Append("      ,[vcOperatorID] = '" + strUserId + "'    \n");
                        sql.Append("      ,[dOperatorTime] = getdate()    \n");
                        sql.Append(" WHERE iAutoId=" + iAutoId + "    \n");
                    }
                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除
        public void Del_jinji(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                DateTime time = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();

                    #region update TPanDian 数量增加
                    sql.AppendLine("update t2 set t2.iSystemQuantity=isnull(t2.iSystemQuantity,0)+isnull(t1.iQuantity,0),");
                    sql.AppendLine("t2.vcOperatorID='" + strUserId + "',t2.dOperatorTime='" + time + "'");
                    sql.AppendLine("from (");
                    sql.AppendLine("	select t2.vcNaRuPart_id,t1.iQuantity from (");
                    sql.AppendLine("		select * from TChuHe_jinji where iAutoId=" + iAutoId + " ");
                    sql.AppendLine("	)t1");
                    sql.AppendLine("	inner join TSSPManagement t2 on t1.vcPart_id=t2.vcChuHePart_id");
                    sql.AppendLine(")t1");
                    sql.AppendLine("inner join (");
                    sql.AppendLine("	select * from TPanDian ");
                    sql.AppendLine(")t2 on t1.vcNaRuPart_id=t2.vcPart_id");
                    #endregion

                    sql.Append("delete from tChuHe_jinji where iAutoId=" + iAutoId + "   \n");

                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Del_kb(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from tChuHe_kb where iAutoId=" + iAutoId + "   \n");

                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public int isExitInSSP(string vcPart_id)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(1) from TSSPManagement where vcChuHePart_id='" + vcPart_id + "'     \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
