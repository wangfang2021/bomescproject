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
            string vcKBLFNo, string vcSellNo, string vcPart_id, string vcBoxNo, string dStart, string dEnd,string vcLabelNo,string vcStatus)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcZYType,t1.vcBZPlant,t1.vcInputNo,t1.vcKBOrderNo,t1.vcKBLFNo,t1.vcPart_id,t1.vcIOType, \n");
                strSql.Append("t1.vcSupplier_id,t1.vcSupplierGQ,t1.dStart,t1.dEnd,cast(t1.iQuantity as int) as iQuantity,t1.vcBZUnit,    \n");
                strSql.Append("t1.vcSHF,t1.vcSR,t1.vcBoxNo,t1.vcSheBeiNo,t1.vcCheckType,cast(t1.iCheckNum as int) as iCheckNum,    \n");
                strSql.Append("t1.vcCheckStatus,t1.vcLabelStart,t1.vcLabelEnd,t1.vcUnlocker,t1.dUnlockTime,t1.vcSellNo,    \n");
                strSql.Append("t1.vcOperatorID,t1.dOperatorTime,t1.vcHostIp,    \n");
                strSql.Append("t3.vcName as vcBZPlantName,t2.vcName as vcZYTypeName, t4.vcUserName,'0' as vcModFlag,'0' as vcAddFlag,   \n");
                strSql.Append("case when t1.vcZYType='S1' and t5.vcPart_id is not null then 'NG' else t1.vcCheckStatus end as vcStatus,    \n");
                strSql.Append("case when t1.vcZYType='S1' and t5.vcPart_id is not null then 'NG过' else '' end as vcHaveNG    \n");
                strSql.Append("from TOperateSJ t1  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C022') t2 on t1.vcZYType=t2.vcValue  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C023') t3 on t1.vcBZPlant=t3.vcValue  \n");
                strSql.Append("left join SUser t4 on t1.vcOperatorID=t4.vcUserID  \n");
                strSql.Append("left join (select distinct vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR from TOperateSJ_NG)t5    \n");
                strSql.Append("on t1.vcPart_id=t5.vcPart_id and t1.vcKBOrderNo=t5.vcKBOrderNo     \n");
                strSql.Append("and t1.vcKBLFNo=t5.vcKBLFNo and t1.vcSR=t5.vcSR    \n");
                strSql.Append("where 1=1  \n");
                if (vcZYType != "" && vcZYType != null)
                    strSql.Append("and isnull(t1.vcZYType,'') = '" + vcZYType + "'   \n");
                if(vcBZPlant != "" && vcBZPlant != null)
                    strSql.Append("and isnull(t1.vcBZPlant,'') = '" + vcBZPlant + "'  \n");
                if(vcInputNo != "" && vcInputNo != null)
                    strSql.Append("and isnull(t1.vcInputNo,'') = '" + vcInputNo + "'   \n");
                if (vcKBOrderNo != "" && vcKBOrderNo != null)
                    strSql.Append("and isnull(t1.vcKBOrderNo,'') = '" + vcKBOrderNo + "'   \n");
                if (vcKBLFNo != "" && vcKBLFNo != null)
                    strSql.Append("and isnull(t1.vcKBLFNo,'') = '" + vcKBLFNo + "'   \n");
                if (vcSellNo != "" && vcSellNo != null)
                    strSql.Append("and isnull(t1.vcSellNo,'') = '" + vcSellNo + "'   \n");
                if (vcPart_id!="" && vcPart_id!=null)
                    strSql.Append("and isnull(t1.vcPart_id,'') = '" + vcPart_id + "'  \n");
                if (vcBoxNo != "" && vcBoxNo != null)
                    strSql.Append("and isnull(t1.vcBoxNo,'') = '" + vcBoxNo + "'  \n");
                if (dStart == "" || dStart == null)
                    dStart = "2001/01/01 00:01:00";
                if (dEnd == "" || dEnd == null)
                    dEnd = "2099/12/31 23:59:59";
                strSql.Append("and isnull(t1.dStart,'2001/01/01 00:01:00') >= '" + dStart + "' and isnull(t1.dEnd,'2099/12/31 23:59:59') <= '" + dEnd + "'  \n");
                if (vcLabelNo != "" && vcLabelNo != null)
                    strSql.Append("and '"+vcLabelNo+"' between t1.vcLabelStart and t1.vcLabelEnd   \n");
                if (vcStatus != "" && vcStatus != null)
                    strSql.Append("and (case when t1.vcZYType='S1' and  t5.vcPart_id is not null then 'NG' else t1.vcCheckStatus end)='" + vcStatus+"'    \n");
                strSql.Append("order by t1.vcZYType,t1.vcInputNo,t1.dStart    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable initSubApi(string vcPart_id, string vcKBOrderNo, string vcKBLFNo, string vcSR)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from TOperateSJ_NG where vcPart_id='"+vcPart_id+"' and vcKBOrderNo='"+vcKBOrderNo+"'     \n");
                sql.Append("and vcKBLFNo='"+vcKBLFNo+"' and vcSR='"+vcSR+"'    \n");
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

        #region 校验 数量<上一层数量
        #endregion
        public DataTable isQuantityOK(string vcPart_id,string vcKBOrderNo,string vcKBLFNo,string vcSR,string vcZYType)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("--取出上一层数量    \n");
                sql.Append("select top(1) ISNULL(iQuantity,0) as iQuantity from TOperateSJ     \n");
                sql.Append("where vcPart_id='"+vcPart_id+"' and vcKBOrderNo='"+ vcKBOrderNo + "'     \n");
                sql.Append("and vcKBLFNo='"+vcKBLFNo+"' and vcSR='"+vcSR+"' and vcZYType<'"+vcZYType+"'    \n");
                sql.Append("order by vcZYType desc    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
