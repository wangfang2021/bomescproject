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
    public class FS0806_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string vcZYType, string vcBZPlant, string vcInputNo, string vcKBOrderNo , 
            string vcKBLFNo, string vcSellNo, string vcPart_id, string vcBoxNo, string dStart, string dEnd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.*,t3.vcName as vcBZPlantName,t2.vcName as vcZYTypeName, t4.vcUserName, \n");
                strSql.Append("'0' as vcModFlag,'0' as vcAddFlag   \n");
                strSql.Append("from TOperateSJ t1  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C022') t2 on t1.vcZYType=t2.vcValue  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C023') t3 on t1.vcBZPlant=t3.vcValue  \n");
                strSql.Append("left join SUser t4 on t1.vcOperatorID=t4.vcUserID  \n");
                strSql.Append("where 1=1  \n");
                if (vcZYType != "" && vcZYType != null)
                    strSql.Append("and isnull(t1.vcZYType,'') like '%" + vcZYType + "%'   \n");
                if(vcBZPlant != "" && vcBZPlant != null)
                    strSql.Append("and isnull(t1.vcBZPlant,'') like '%" + vcBZPlant + "%'  \n");
                if(vcInputNo != "" && vcInputNo != null)
                    strSql.Append("and isnull(t1.vcInputNo,'') like '%" + vcInputNo + "%'   \n");
                if (vcKBOrderNo != "" && vcKBOrderNo != null)
                    strSql.Append("and isnull(t1.vcKBOrderNo,'') like '%" + vcKBOrderNo + "%'   \n");
                if (vcKBLFNo != "" && vcKBLFNo != null)
                    strSql.Append("and isnull(t1.vcKBLFNo,'') like '%" + vcKBLFNo + "%'   \n");
                if (vcSellNo != "" && vcSellNo != null)
                    strSql.Append("and isnull(t1.vcSellNo,'') like '%" + vcSellNo + "%'   \n");
                if (vcPart_id!="" && vcPart_id!=null)
                    strSql.Append("and isnull(t1.vcPart_id,'') like '%" + vcPart_id + "%'  \n");
                if (vcBoxNo != "" && vcBoxNo != null)
                    strSql.Append("and isnull(t1.vcBoxNo,'') like '%" + vcBoxNo + "%'  \n");
                if (dStart == "" || dStart == null)
                    dStart = "2001/01/01 00:01:00";
                if (dEnd == "" || dEnd == null)
                    dEnd = "2099/12/31 23:59:59";
                strSql.Append("and isnull(t1.dStart,'2001/01/01 00:01:00') >= '" + dStart + "' and isnull(t1.dEnd,'2099/12/31 23:59:59') <= '" + dEnd + "'  \n");
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
                        #region modify sql
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("UPDATE [TOperateSJ]  \n");
                        sql.Append("   SET   \n");
                        sql.Append("       [iQuantity] = " + listInfoData[i]["iQuantity"].ToString() + "  \n");
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
                    sql.Append("delete from TOperateSJ where iAutoId=" + iAutoId + "   \n");
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
    }
}
