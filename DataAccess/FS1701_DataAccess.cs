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
                strSql.Append("select *,vcIsDQ as vcIsDQ_old,iQuantity as iQuantity_old,'0' as vcModFlag,'0' as vcAddFlag from TNRManagement where 1=1  \n");
                if (vcIsDQ != "" && vcIsDQ != null)
                    strSql.Append("and isnull(vcIsDQ,'') like '%" + vcIsDQ + "%'  \n");
                if (vcTicketNo != "" && vcTicketNo != null)
                    strSql.Append("and isnull(vcTicketNo,'') like '%" + vcTicketNo + "%'  \n");
                if (vcLJNo != "" && vcLJNo != null)
                    strSql.Append("and isnull(vcLJNo,'') like '%" + vcLJNo + "%'  \n");
                if (vcOldOrderNo != "" && vcOldOrderNo != null)
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
                DateTime now = System.DateTime.Now;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("delete from TNRManagement_Temp where vcOperatorID='" + strUserId + "'");
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
                        string vcIsDQ = listInfoData[i]["vcIsDQ"].ToString() == "" ? "是" : listInfoData[i]["vcIsDQ"].ToString();
                        string vcIsDQ_old = listInfoData[i]["vcIsDQ_old"].ToString();
                        string strTickNo = listInfoData[i]["vcTicketNo"].ToString();
                        string vcLJNo = listInfoData[i]["vcLJNo"].ToString();
                        string iQuantity = string.IsNullOrEmpty(listInfoData[i]["iQuantity"].ToString()) ? "0" : listInfoData[i]["iQuantity"].ToString();
                        string iQuantity_old = string.IsNullOrEmpty(listInfoData[i]["iQuantity_old"].ToString()) ? "0" : listInfoData[i]["iQuantity_old"].ToString();

                        sql.Append("UPDATE [TNRManagement]  \n");
                        sql.Append("   SET   \n");
                        sql.Append("       [iQuantity] = " + iQuantity + "  \n");
                        sql.Append("      ,[vcIsDQ] = '" + vcIsDQ + "'  \n");
                        sql.Append("      ,[vcOperatorID] = '" + strUserId + "'  \n");
                        sql.Append("      ,[dOperatorTime] = '" + now + "'  \n");
                        sql.Append(" WHERE iAutoId=" + iAutoId + "  \n");

                        #region 插入中间表
                        int qty = 0;
                        if (vcIsDQ_old == "否" && vcIsDQ == "否")
                            qty = 0;
                        else if (vcIsDQ_old == "是" && vcIsDQ == "否")
                            qty = Convert.ToInt32(iQuantity_old) * (-1);
                        else if (vcIsDQ_old == "否" && vcIsDQ == "是")
                            qty = Convert.ToInt32(iQuantity);
                        else if (vcIsDQ_old == "是" && vcIsDQ == "是")
                        {
                            int cha = Convert.ToInt32(iQuantity_old) - Convert.ToInt32(iQuantity);
                            qty = cha * (-1);
                        }
                        sql.AppendLine("INSERT INTO [TNRManagement_Temp]");
                        sql.AppendLine("           ([vcTicketNo]");
                        sql.AppendLine("           ,[vcLJNo]");
                        sql.AppendLine("           ,[iQuantity]");
                        sql.AppendLine("           ,[vcIsDQ]");
                        sql.AppendLine("           ,[vcOperatorID]");
                        sql.AppendLine("           ,[dOperatorTime])");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("           ('" + strTickNo + "'");
                        sql.AppendLine("           ,'" + vcLJNo + "'");
                        sql.AppendLine("           ," + qty + " ");
                        sql.AppendLine("           ,'" + vcIsDQ + "'");
                        sql.AppendLine("           ,'" + strUserId + "'");
                        sql.AppendLine("           ,'" + now + "')");
                        #endregion

                    }
                }
                #region 更新库存
                sql.AppendLine("update t2 set t2.iSystemQuantity=isnull(t2.iSystemQuantity,0)+isnull(t1.iQuantity,0),");
                sql.AppendLine("t2.vcOperatorID='" + strUserId + "',t2.dOperatorTime='" + now + "' ");
                sql.AppendLine("from (");
                sql.AppendLine("    select vcLJNo,sum(isnull(iQuantity,0)) as iQuantity from TNRManagement_Temp where vcOperatorID='" + strUserId + "' ");
                sql.AppendLine("    group by vcLJNo");
                sql.AppendLine(")t1");
                sql.AppendLine("inner join (");
                sql.AppendLine("	select * from TPanDian ");
                sql.AppendLine(")t2 on t1.vcLJNo=t2.vcPart_id");
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

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    string strTickNo = checkedInfoData[i]["vcTicketNo"].ToString();
                    string vcLJNo = checkedInfoData[i]["vcLJNo"].ToString();

                    #region update TPanDian 数量减少
                    sql.AppendLine("update t2 set t2.iSystemQuantity=isnull(t2.iSystemQuantity,0)-isnull(t1.iQuantity,0),");
                    sql.AppendLine("t2.vcOperatorID='" + strUserId + "',t2.dOperatorTime=getdate() ");
                    sql.AppendLine("from (");
                    sql.AppendLine("	select * from TNRManagement where vcTicketNo='" + strTickNo + "' and vcLJNo='" + vcLJNo + "' and vcIsDQ='是' ");
                    sql.AppendLine(")t1");
                    sql.AppendLine("inner join (");
                    sql.AppendLine("	select * from TPanDian where vcPart_id='" + vcLJNo + "'");
                    sql.AppendLine(")t2 on t1.vcLJNo=t2.vcPart_id");
                    #endregion

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
        public void importSave_Sub(DataSet ds, string strUserId)
        {
            try
            {
                DateTime now = System.DateTime.Now;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("delete from TNRManagement_Temp where vcOperatorID='" + strUserId + "'");
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    string strTickNo = dt.TableName;
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string vcIsDQ = dt.Rows[j]["vcIsDQ"].ToString() == "" ? "是" : dt.Rows[j]["vcIsDQ"].ToString();
                        #region insert TNRManagement
                        sql.Append("INSERT INTO [TNRManagement]    \n");
                        sql.Append("           ([vcTicketNo]    \n");
                        sql.Append("           ,[vcLJNo]    \n");
                        sql.Append("           ,[vcLJName]    \n");
                        sql.Append("           ,[vcCarType]    \n");
                        sql.Append("           ,[vcOldOrderNo]    \n");
                        sql.Append("           ,[vcUnit]    \n");
                        sql.Append("           ,[iQuantity]    \n");
                        sql.Append("           ,[decPrice]    \n");
                        sql.Append("           ,[decMoney]    \n");
                        sql.Append("           ,[vcIsDQ]    \n");
                        sql.Append("           ,[vcOperatorID]    \n");
                        sql.Append("           ,[dOperatorTime])    \n");
                        sql.Append("     VALUES    \n");
                        sql.Append("           ('" + strTickNo + "'    \n");
                        sql.Append("           ,'" + dt.Rows[j]["vcLJNo"].ToString() + "'    \n");
                        sql.Append("           ,'" + dt.Rows[j]["vcLJName"].ToString() + "'    \n");
                        sql.Append("           ,'" + dt.Rows[j]["vcCarType"].ToString() + "'    \n");
                        sql.Append("           ,'" + dt.Rows[j]["vcOldOrderNo"].ToString() + "'    \n");
                        sql.Append("           ,'" + dt.Rows[j]["vcUnit"].ToString() + "'    \n");
                        sql.Append("           ,nullif('" + dt.Rows[j]["iQuantity"].ToString() + "','')    \n");
                        sql.Append("           ,nullif('" + dt.Rows[j]["decPrice"].ToString() + "','')    \n");
                        sql.Append("           ,nullif('" + dt.Rows[j]["decMoney"].ToString() + "','')    \n");
                        sql.Append("           ,'" + vcIsDQ + "'    \n");
                        sql.Append("           ,'" + strUserId + "'    \n");
                        sql.Append("           ,'" + now + "')    \n");
                        #endregion

                        #region 插入中间表
                        sql.AppendLine("INSERT INTO [TNRManagement_Temp]");
                        sql.AppendLine("           ([vcTicketNo]");
                        sql.AppendLine("           ,[vcLJNo]");
                        sql.AppendLine("           ,[iQuantity]");
                        sql.AppendLine("           ,[vcIsDQ]");
                        sql.AppendLine("           ,[vcOperatorID]");
                        sql.AppendLine("           ,[dOperatorTime])");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("           ('" + strTickNo + "'");
                        sql.AppendLine("           ,'" + dt.Rows[j]["vcLJNo"].ToString() + "'");
                        sql.AppendLine("           ,case when '" + vcIsDQ + "'='是' then nullif('" + dt.Rows[j]["iQuantity"].ToString() + "','') when '" + vcIsDQ + "'='否' then '0' else null end ");
                        sql.AppendLine("           ,'" + vcIsDQ + "'");
                        sql.AppendLine("           ,'" + strUserId + "'");
                        sql.AppendLine("           ,'" + now + "')");
                        #endregion
                    }
                }
                #region 更新库存
                sql.AppendLine("update t2 set t2.iSystemQuantity=isnull(t2.iSystemQuantity,0)+isnull(t1.iQuantity,0),");
                sql.AppendLine("t2.vcOperatorID='" + strUserId + "',t2.dOperatorTime='" + now + "' ");
                sql.AppendLine("from (");
                sql.AppendLine("    select vcLJNo,sum(isnull(iQuantity,0)) as iQuantity from TNRManagement_Temp where vcOperatorID='" + strUserId + "' ");
                sql.AppendLine("    group by vcLJNo");
                sql.AppendLine(")t1");
                sql.AppendLine("inner join (");
                sql.AppendLine("	select * from TPanDian ");
                sql.AppendLine(")t2 on t1.vcLJNo=t2.vcPart_id");

                sql.AppendLine("insert into TPanDian (vcPart_id,iSystemQuantity,iRealQuantity,vcOperatorID,dOperatorTime)");
                sql.AppendLine("select t1.vcLJNo,t1.iQuantity,null,'" + strUserId + "','" + now + "' from(");
                sql.AppendLine("    select vcLJNo,sum(isnull(iQuantity,0)) as iQuantity from TNRManagement_Temp where vcOperatorID='" + strUserId + "' ");
                sql.AppendLine("    group by vcLJNo");
                sql.AppendLine(")t1 ");
                sql.AppendLine("left join (");
                sql.AppendLine("	select * from TPanDian  ");
                sql.AppendLine(")t2 on t1.vcLJNo=t2.vcPart_id");
                sql.AppendLine("where t2.iAutoId is null");
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

        public int isImport(string strTickNo)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(1) as num from [TNRManagement] where vcTicketNo='" + strTickNo + "'    \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
