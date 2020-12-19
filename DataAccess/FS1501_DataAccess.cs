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

        #region 检索
        public DataTable Search(string vcSupplier_id, string vcGQ, string vcSR, string vcOrderNo, string vcNRBianCi, string vcNRBJSK)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select *,'0' as vcModFlag,'0' as vcAddFlag from TNRBJSKBianCi  \n");
                strSql.Append("where isnull(vcSupplier_id,'') like '%" + vcSupplier_id + "%' and isnull(vcGQ,'') like '%" + vcGQ + "%'  \n");
                strSql.Append("and isnull(vcSR,'') like '%" + vcSR + "%' and isnull(vcOrderNo,'') like '%" + vcOrderNo + "%'  \n");
                strSql.Append("and isnull(vcNRBianCi,'') like '%" + vcNRBianCi + "%' and isnull(vcNRBJSK,'') like '%" + vcNRBJSK + "%'  \n");
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

                    if (baddflag == true && bmodflag == true)
                    {//新增
                        #region insert sql
                        sql.Append("INSERT INTO [TNRBJSKBianCi]  \n");
                        sql.Append("           ([vcSupplier_id]  \n");
                        sql.Append("           ,[vcGQ]  \n");
                        sql.Append("           ,[vcSR]  \n");
                        sql.Append("           ,[vcOrderNo]  \n");
                        sql.Append("           ,[vcNRBianCi]  \n");
                        sql.Append("           ,[vcNRBJSK]  \n");
                        sql.Append("           ,[vcOperatorID]  \n");
                        sql.Append("           ,[dOperatorTime])  \n");
                        sql.Append("     VALUES  \n");
                        sql.Append("           ('" + listInfoData[i]["vcSupplier_id"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcGQ"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcSR"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcOrderNo"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcNRBianCi"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcNRBJSK"].ToString() + "'  \n");
                        sql.Append("           ,'" + strUserId + "'  \n");
                        sql.Append("           ,getdate())  \n");
                        #endregion
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        #region modify sql
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("UPDATE [TNRBJSKBianCi]  \n");
                        sql.Append("   SET   \n");
                        sql.Append("       [vcNRBianCi] = '" + listInfoData[i]["vcNRBianCi"].ToString() + "'  \n");
                        sql.Append("      ,[vcNRBJSK] = '" + listInfoData[i]["vcNRBJSK"].ToString() + "'  \n");
                        sql.Append("      ,[vcOperatorID] = '" + strUserId + "'  \n");
                        sql.Append("      ,[dOperatorTime] = getdate()  \n");
                        sql.Append(" WHERE iAutoId=" + iAutoId + "  \n");
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

        #region 取出所有部品纳入补给时刻便次
        public DataTable GetNRBJSKBianCiInfo()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from TNRBJSKBianCi  \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion
    }
}
