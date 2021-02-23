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
    public class FS1704_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string vcNaRuPart_id, string vcChuHePart_id, string vcSupplierName, string vcCarType, string vcProject)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select *,'0' as vcModFlag,'0' as vcAddFlag  \n");
                strSql.Append("from TSSPManagement  \n");
                strSql.Append("where 1=1  \n");
                if (vcNaRuPart_id != "" && vcNaRuPart_id != null)
                    strSql.Append("and ISNULL(vcNaRuPart_id,'') like '%" + vcNaRuPart_id + "%'  \n");
                if (vcChuHePart_id != "" && vcChuHePart_id != null)
                    strSql.Append("and ISNULL(vcChuHePart_id,'') like '%" + vcChuHePart_id + "%'  \n");
                if (vcSupplierName != "" && vcSupplierName != null)
                    strSql.Append("and ISNULL(vcSupplierName,'') like '%" + vcSupplierName + "%'  \n");
                if (vcCarType != "" && vcCarType != null)
                    strSql.Append("and ISNULL(vcCarType,'') like '%" + vcCarType + "%'  \n");
                if (vcProject != "" && vcProject != null)
                    strSql.Append("and ISNULL(vcProject,'') like '%" + vcProject + "%'  \n");
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

                    string strSR = listInfoData[i]["vcSR"].ToString();
                    string strPartsNoBefore5 = listInfoData[i]["vcPartsNoBefore5"].ToString();
                    string strBCPartsNo = listInfoData[i]["vcBCPartsNo"].ToString();
                    string strSmallPM = listInfoData[i]["vcSmallPM"].ToString();

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == true)
                    {//新增
                       
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("UPDATE [TSSPManagement]    \n");
                        sql.Append("   SET [vcNaRuPart_id] = '"+ listInfoData[i]["vcNaRuPart_id"].ToString() + "'    \n");
                        sql.Append("      ,[vcChuHePart_id] = '" + listInfoData[i]["vcChuHePart_id"].ToString() + "'    \n");
                        sql.Append("      ,[vcBackPart_id] = '" + listInfoData[i]["vcBackPart_id"].ToString() + "'    \n");
                        sql.Append("      ,[vcPart_Name] = '" + listInfoData[i]["vcPart_Name"].ToString() + "'    \n");
                        sql.Append("      ,[iCapacity] = nullif('" + listInfoData[i]["iCapacity"].ToString() + "','')    \n");
                        sql.Append("      ,[vcBoxType] = '" + listInfoData[i]["vcBoxType"].ToString() + "'    \n");
                        sql.Append("      ,[vcSupplierName] = '" + listInfoData[i]["vcSupplierName"].ToString() + "'    \n");
                        sql.Append("      ,[vcCarType] = '" + listInfoData[i]["vcCarType"].ToString() + "'    \n");
                        sql.Append("      ,[vcProject] = '" + listInfoData[i]["vcProject"].ToString() + "'    \n");
                        sql.Append("      ,[vcProjectPlace] = '" + listInfoData[i]["vcProjectPlace"].ToString() + "'    \n");
                        sql.Append("      ,[vcSR] = '" + listInfoData[i]["vcSR"].ToString() + "'    \n");
                        sql.Append("      ,[vcPlace] = '" + listInfoData[i]["vcPlace"].ToString() + "'    \n");
                        sql.Append("      ,[vcKBPrintWay] = '" + listInfoData[i]["vcKBPrintWay"].ToString() + "'    \n");
                        sql.Append("      ,[vcOperatorID] = '"+strUserId+"'    \n");
                        sql.Append("      ,[dOperatorTime] = getdate()    \n");
                        sql.Append(" WHERE iAutoId="+iAutoId+"    \n");
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
                    sql.Append("delete from TSSPManagement where iAutoId=" + iAutoId + "   \n");
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
                sql.Append("DELETE FROM [TPMSmall_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("INSERT INTO [TPMSmall_Temp]  \n");
                    sql.Append("           ([vcPartsNoBefore5]  \n");
                    sql.Append("           ,[vcSR]  \n");
                    sql.Append("           ,[vcBCPartsNo]  \n");
                    sql.Append("           ,[vcSmallPM]  \n");
                    sql.Append("           ,[vcOperatorID]  \n");
                    sql.Append("           ,[dOperatorTime])  \n");
                    sql.Append("     VALUES  \n");
                    sql.Append("           ('"+ dt.Rows[i]["vcPartsNoBefore5"].ToString() + "'   \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSR"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcBCPartsNo"].ToString() + "' \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSmallPM"].ToString() + "' \n"); 
                    sql.Append("           ,'"+strUserId+"'  \n");
                    sql.Append("           ,getdate())  \n");
                   
                }
                sql.Append("insert into TPMSmall (vcPartsNoBefore5,vcSR,vcBCPartsNo,vcSmallPM,vcOperatorID,dOperatorTime)  \n");
                sql.Append("select t1.vcPartsNoBefore5,t1.vcSR,t1.vcBCPartsNo,t1.vcSmallPM,t1.vcOperatorID,t1.dOperatorTime from TPMSmall_Temp t1  \n");
                sql.Append("left join TPMSmall t2 on isnull(t1.vcPartsNoBefore5,'')=isnull(t2.vcPartsNoBefore5,'') and isnull(t1.vcSR,'')=isnull(t2.vcSR,'') " +
                    "and isnull(t1.vcBCPartsNo,'')=isnull(t2.vcBCPartsNo,'')  \n");
                sql.Append("where t2.iAutoId is null and t1.vcOperatorID='" + strUserId + "'  \n");

                sql.Append("update t2 set t2.vcSmallPM=t1.vcSmallPM,  \n");
                sql.Append("t2.vcOperatorID=t1.vcOperatorID,t2.dOperatorTime=t1.dOperatorTime  \n");
                sql.Append("from  \n");
                sql.Append("(select * from TPMSmall_Temp) t1  \n");
                sql.Append("left join TPMSmall t2 on isnull(t1.vcPartsNoBefore5,'')=isnull(t2.vcPartsNoBefore5,'') and isnull(t1.vcSR,'')=isnull(t2.vcSR,'') " +
                    "and isnull(t1.vcBCPartsNo,'')=isnull(t2.vcBCPartsNo,'')  \n");
                sql.Append("where t2.iAutoId is not null and t1.vcSmallPM!=t2.vcSmallPM  \n");
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
