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
    public class FS1703_DataAccess
    {
        private MultiExcute excute = new MultiExcute();       

        #region 检索
        public DataTable Search(string vcPart_id, string vcChaYi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.*,t2.vcPlace,'0' as vcModFlag,'0' as vcAddFlag from TPanDian t1    \n");
                strSql.Append("left join TSSPManagement t2 on t1.vcPart_id=t2.vcNaRuPart_id    \n");
                strSql.Append("where 1=1    \n");
                if (vcPart_id != "" && vcPart_id != null)
                    strSql.Append("and t1.vcPart_id like '" + vcPart_id + "%'    \n");
                if (vcChaYi=="是")
                    strSql.Append("and isnull(t1.iSystemQuantity,0)<>isnull(t1.iRealQuantity,0)  \n");
                else if(vcChaYi=="否")
                    strSql.Append("and isnull(t1.iSystemQuantity,0)=isnull(t1.iRealQuantity,0)  \n");
                strSql.Append("order by t1.vcPart_id   \n");
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
                sql.Append("select *,'0' as vcModFlag,'0' as vcAddFlag from tChuHe_jinji   \n");
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

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
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
                        //无新增情况
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        #region modify sql
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        string iRealQuantity = listInfoData[i]["iRealQuantity"] == null ? "" : listInfoData[i]["iRealQuantity"].ToString();

                        sql.Append("update TPanDian set iRealQuantity=nullif('" + iRealQuantity + "',''),   \n");
                        sql.Append("vcOperatorID='"+iAutoId+"',dOperatorTime=getdate()  \n");
                        sql.Append("where iAutoId='" + iAutoId + "'    \n");
                        #endregion
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

    }
}
