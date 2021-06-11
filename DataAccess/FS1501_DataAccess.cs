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
    public class FS1501_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 下拉框选择
        public DataTable getSR()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct vcSR as vcValue,vcSR as vcName from TNRBJSKBianCi  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getNRBianCi()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct vcNRBianCi as vcValue,vcNRBianCi as vcName from TNRBJSKBianCi  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索
        public DataTable Search(string vcSupplier_id, string vcGQ, string vcSR, string vcOrderNo, string vcNRBianCi, string vcNRBJSK)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select *,'0' as vcModFlag,'0' as vcAddFlag from TNRBJSKBianCi where 1=1  \n");
                if(vcSupplier_id!="" && vcSupplier_id!=null)
                    strSql.Append("and isnull(vcSupplier_id,'') like '%" + vcSupplier_id + "%'  \n");
                if(vcGQ!="" && vcGQ!=null)
                    strSql.Append("and isnull(vcGQ,'') like '%" + vcGQ + "%'  \n");
                if(vcSR!="" && vcSR!=null)
                    strSql.Append("and isnull(vcSR,'') like '%" + vcSR + "%'  \n");
                if(vcOrderNo!="" && vcOrderNo!=null)
                    strSql.Append("and isnull(vcOrderNo,'') like '%" + vcOrderNo + "%'  \n");
                if(vcNRBianCi!="" && vcNRBianCi!=null)
                    strSql.Append("and isnull(vcNRBianCi,'') like '%" + vcNRBianCi + "%'  \n");
                if(vcNRBJSK!="" && vcNRBJSK!=null)
                    strSql.Append("and isnull(vcNRBJSK,'') like '%" + vcNRBJSK + "%'  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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

                   if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("UPDATE [TNRBJSKBianCi]  \n");
                        sql.Append("   SET   \n");
                        sql.Append("       [vcNRBianCi] = '" + listInfoData[i]["vcNRBianCi"].ToString() + "'  \n");
                        sql.Append("      ,[vcNRBJSK] = '" + listInfoData[i]["vcNRBJSK"].ToString() + "'  \n");
                        sql.Append("      ,[vcOperatorID] = '" + strUserId + "'  \n");
                        sql.Append("      ,[dOperatorTime] = getdate()  \n");
                        sql.Append(" WHERE iAutoId=" + iAutoId + "  \n");
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
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from TNRBJSKBianCi where iAutoId=" + iAutoId + "   \n");
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
        public void importSave_Sub(DataTable dt, string strUserId)
        {
            SqlConnection conn = ComConnectionHelper.CreateSqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlTransaction st = null;
            ComConnectionHelper.OpenConection_SQL(ref conn);
            st = conn.BeginTransaction();
            StringBuilder sql = new StringBuilder();
            try
            {
                DateTime now = DateTime.Now;
                sql.Append("DELETE FROM [TNRBJSKBianCi_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("select * from TNRBJSKBianCi_Temp  \n");
                    sql.Append("where vcSupplier_id='" + dt.Rows[i]["vcSupplier_id"].ToString() + "' and vcGQ='" + dt.Rows[i]["vcGQ"].ToString() + "'\n");
                    sql.Append("and vcSR='" + dt.Rows[i]["vcSR"].ToString() + "' and vcOrderNo='" + dt.Rows[i]["vcOrderNo"].ToString() + "'   \n");
                    sql.Append("if (@@ROWCOUNT=0)    \n");
                    sql.Append("begin    \n");
                    sql.Append("    INSERT INTO [TNRBJSKBianCi_Temp]  \n");
                    sql.Append("               ([vcSupplier_id]  \n");
                    sql.Append("               ,[vcGQ]  \n");
                    sql.Append("               ,[vcSR]  \n");
                    sql.Append("               ,[vcOrderNo]  \n");
                    sql.Append("               ,[vcNRBianCi]  \n");
                    sql.Append("               ,[vcNRBJSK]  \n");
                    sql.Append("               ,[vcOperatorID]  \n");
                    sql.Append("               ,[dOperatorTime])  \n");
                    sql.Append("         VALUES  \n");
                    sql.Append("               ('"+dt.Rows[i]["vcSupplier_id"].ToString()+"'  \n");
                    sql.Append("               ,'" + dt.Rows[i]["vcGQ"].ToString() + "'  \n");
                    sql.Append("               ,'" + dt.Rows[i]["vcSR"].ToString() + "'  \n");
                    sql.Append("               ,'" + dt.Rows[i]["vcOrderNo"].ToString() + "'  \n");
                    sql.Append("               ,'" + dt.Rows[i]["vcNRBianCi"].ToString() + "'  \n");
                    string vcNRBJSK = dt.Rows[i]["vcNRBJSK"].ToString();
                    if(vcNRBJSK!="N+1")
                    {
                        vcNRBJSK = vcNRBJSK.PadLeft(4, '0');
                        vcNRBJSK = vcNRBJSK.Substring(0, 2) + ":" + vcNRBJSK.Substring(2, 2);
                    }
                    sql.Append("               ,'" + vcNRBJSK + "'  \n");
                    sql.Append("               ,'"+strUserId+"'  \n");
                    sql.Append("               ,'"+ now + "')  \n");
                    sql.Append("end  \n");
                    sql.Append("else  \n");
                    sql.Append("begin    \n");
                    sql.Append("    update TNRBJSKBianCi_Temp set vcNRBianCi='" + dt.Rows[i]["vcNRBianCi"].ToString() + "',vcNRBJSK='" + vcNRBJSK + "',vcOperatorID='" + strUserId + "',dOperatorTime='"+ now + "'    \n");
                    sql.Append("    where vcSupplier_id='" + dt.Rows[i]["vcSupplier_id"].ToString() + "' and vcGQ='" + dt.Rows[i]["vcGQ"].ToString() + "'\n");
                    sql.Append("    and vcSR='" + dt.Rows[i]["vcSR"].ToString() + "' and vcOrderNo='" + dt.Rows[i]["vcOrderNo"].ToString() + "'   \n");
                    sql.Append("end  \n");

                    if (i % 1000 == 0)
                    {
                        cmd = new SqlCommand(sql.ToString(), conn, st);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        sql.Length = 0;
                    }
                }

                sql.Append("insert into TNRBJSKBianCi (vcSupplier_id,vcGQ,vcSR,vcOrderNo,vcNRBianCi,vcNRBJSK,vcOperatorID,dOperatorTime)  \n");
                sql.Append("select t1.vcSupplier_id,t1.vcGQ,t1.vcSR,t1.vcOrderNo,t1.vcNRBianCi,t1.vcNRBJSK,t1.vcOperatorID,t1.dOperatorTime from TNRBJSKBianCi_Temp t1  \n");
                sql.Append("left join TNRBJSKBianCi t2 on t1.vcSupplier_id=t2.vcSupplier_id and t1.vcGQ=t2.vcGQ and t1.vcSR=t2.vcSR and t1.vcOrderNo=t2.vcOrderNo \n");
                sql.Append("where t2.iAutoId is null and t1.vcOperatorID='" + strUserId + "'  \n");

                sql.Append("update t2 set t2.vcNRBianCi=t1.vcNRBianCi,t2.vcNRBJSK=t1.vcNRBJSK,  \n");
                sql.Append("t2.vcOperatorID=t1.vcOperatorID,t2.dOperatorTime=t1.dOperatorTime  \n");
                sql.Append("from  \n");
                sql.Append("(select * from TNRBJSKBianCi_Temp) t1  \n");
                sql.Append("left join TNRBJSKBianCi t2 on t1.vcSupplier_id=t2.vcSupplier_id and t1.vcGQ=t2.vcGQ and t1.vcSR=t2.vcSR and t1.vcOrderNo=t2.vcOrderNo \n");
                sql.Append("where t2.iAutoId is not null and (t1.vcNRBianCi!=t2.vcNRBianCi or t1.vcNRBJSK!=t2.vcNRBJSK) \n");
                sql.Append("and t1.vcOperatorID='" + strUserId + "'  \n");

                cmd = new SqlCommand(sql.ToString(), conn, st);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();

                st.Commit();
                ComConnectionHelper.CloseConnection_SQL(ref conn);
            }
            catch (Exception ex)
            {
                st.Rollback();
                ComConnectionHelper.CloseConnection_SQL(ref conn);
                throw ex;
            }
        }
        #endregion
    }
}
