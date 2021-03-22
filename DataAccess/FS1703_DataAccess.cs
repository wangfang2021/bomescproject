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
        public DataTable Search_history(string vcPart_id, string vcPlace)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from TPanDian_History  where 1=1  \n");
                if (vcPart_id != "" && vcPart_id != null)
                    sql.Append("and vcPart_id like '" + vcPart_id + "%'    \n");
                if(vcPlace!="" && vcPlace!=null)
                    sql.Append("and vcPlace like '%" + vcPlace + "%'    \n");
                sql.Append("order by dOperatorTime desc,vcPart_id    \n");

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
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                DateTime now = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    sql.AppendLine("update TPanDian set iRealQuantity=nullif('" + dt.Rows[i]["iRealQuantity"].ToString() + "',''),vcOperatorID='" + strUserId + "',dOperatorTime='" + now + "' ");
                    sql.AppendLine("where vcPart_id='" + dt.Rows[i]["vcPart_id"].ToString() + "'");
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

        #region 覆盖
        public void cover(string strUserId)
        {
            try
            {
                DateTime now = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("insert into TPanDian_History (vcPart_id,vcPlace,iBefore,iAfter,vcOperatorID,dOperatorTime)");
                sql.AppendLine("select t1.vcPart_id,t2.vcPlace,t1.iSystemQuantity,iRealQuantity,'"+strUserId+"','"+now+"' from TPanDian t1");
                sql.AppendLine("left join TSSPManagement t2 on t1.vcPart_id=t2.vcNaRuPart_id");
                sql.AppendLine("where iRealQuantity is not null and iRealQuantity<>iSystemQuantity ");

                sql.AppendLine("update TPanDian set iSystemQuantity=iRealQuantity,vcOperatorID='"+strUserId+"',dOperatorTime='"+now+"'");
                sql.AppendLine("where iRealQuantity is not null and iRealQuantity<>iSystemQuantity ");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
