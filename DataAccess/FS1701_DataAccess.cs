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
    public class FS1701_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string vcIsDQ, string vcTicketNo, string vcLJNo, string vcOldOrderNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select *,'0' as vcModFlag,'0' as vcAddFlag from TNRManagement where 1=1  \n");
                if(vcIsDQ != "" && vcIsDQ != null)
                    strSql.Append("and isnull(vcIsDQ,'') like '%" + vcIsDQ + "%'  \n");
                if(vcTicketNo != "" && vcTicketNo != null)
                    strSql.Append("and isnull(vcTicketNo,'') like '%" + vcTicketNo + "%'  \n");
                if(vcLJNo != "" && vcLJNo != null)
                    strSql.Append("and isnull(vcLJNo,'') like '%" + vcLJNo + "%'  \n");
                if(vcOldOrderNo != "" && vcOldOrderNo != null)
                    strSql.Append("and isnull(vcOldOrderNo,'') like '%" + vcOldOrderNo + "%'  \n");
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
                        string vcIsDQ= listInfoData[i]["vcIsDQ"].ToString()==""?"是": listInfoData[i]["vcIsDQ"].ToString();
                        sql.Append("UPDATE [TNRManagement]  \n");
                        sql.Append("   SET   \n");
                        sql.Append("       [iQuantity] = " + listInfoData[i]["iQuantity"].ToString() + "  \n");
                        sql.Append("      ,[vcIsDQ] = '" + vcIsDQ + "'  \n");
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
                    sql.Append("delete from TNRManagement where iAutoId=" + iAutoId + "   \n");
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
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [TNRManagement_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcIsDQ = dt.Rows[i]["vcIsDQ"].ToString() == "" ? "是" : dt.Rows[i]["vcIsDQ"].ToString();
                    sql.Append("INSERT INTO [TNRManagement_Temp]  \n");
                    sql.Append("           ([vcTicketNo]  \n");
                    sql.Append("           ,[vcLJNo]  \n");
                    sql.Append("           ,[vcOldOrderNo]  \n");
                    sql.Append("           ,[iQuantity]  \n");
                    sql.Append("           ,[vcIsDQ]  \n");
                    sql.Append("           ,[vcOperatorID]  \n");
                    sql.Append("           ,[dOperatorTime])  \n");
                    sql.Append("     VALUES  \n");
                    sql.Append("           ('"+dt.Rows[i]["vcTicketNo"].ToString()+"'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcLJNo"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcOldOrderNo"].ToString() + "'  \n");
                    sql.Append("           ," + dt.Rows[i]["iQuantity"].ToString() + "  \n");
                    sql.Append("           ,'" + vcIsDQ + "'  \n");
                    sql.Append("           ,'"+strUserId+"'  \n");
                    sql.Append("           ,getdate())  \n");
                }
                sql.Append("insert into TNRManagement (vcTicketNo,vcLJNo,vcOldOrderNo,iQuantity,vcIsDQ,vcOperatorID,dOperatorTime)  \n");
                sql.Append("select t1.vcTicketNo,t1.vcLJNo,t1.vcOldOrderNo,t1.iQuantity,t1.vcIsDQ,t1.vcOperatorID,t1.dOperatorTime from TNRManagement_Temp t1  \n");
                sql.Append("left join TNRManagement t2 on t1.vcTicketNo=t2.vcTicketNo and t1.vcLJNo=t2.vcLJNo and t1.vcOldOrderNo=t2.vcOldOrderNo \n");
                sql.Append("where t2.iAutoId is null and t1.vcOperatorID='" + strUserId + "'  \n");

                sql.Append("update t2 set t2.iQuantity=t1.iQuantity,t2.vcIsDQ=t1.vcIsDQ,  \n");
                sql.Append("t2.vcOperatorID=t1.vcOperatorID,t2.dOperatorTime=t1.dOperatorTime  \n");
                sql.Append("from  \n");
                sql.Append("(select * from TNRManagement_Temp) t1  \n");
                sql.Append("left join TNRManagement t2 on t1.vcTicketNo=t2.vcTicketNo and t1.vcLJNo=t2.vcLJNo and t1.vcOldOrderNo=t2.vcOldOrderNo \n");
                sql.Append("where t2.iAutoId is not null and (t1.iQuantity!=t2.iQuantity or t1.vcIsDQ!=t2.vcIsDQ) \n");
                sql.Append("and t1.vcOperatorID='" + strUserId + "'  \n");

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
