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
    public class FS0812_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string vcBox_id,string vcLabelId,string strFHF, string vcPart_id, string vcLianFanNo, string vcOrderNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select iAutoId,vcStatus,vcBoxNo,vcInstructionNo,vcPart_id,vcOrderNo,vcLianFanNo,  \n");
                strSql.Append("isnull(iQuantity,0) as iQuantity,dBZID,dBZTime,dZXID,dZXTime,vcOperatorID,dOperatorTime,    \n");
                strSql.Append("isnull(iRHQuantity,0) as iRHQuantity,vcLabelStart,vcLabelEnd,    \n");
                strSql.Append("'0' as vcModFlag,'0' as vcAddFlag from TBoxMaster  where isnull(vcDelete,'')!='1' and dPrintBoxTime is null  \n");
                if (vcBox_id!="" && vcBox_id !=null)
                    strSql.Append("and isnull(vcBoxNo,'') = '" + vcBox_id + "' \n");
                if (vcLabelId != "" && vcLabelId != null)
                    strSql.Append("and '"+vcLabelId+ "' between vcLabelStart and vcLabelEnd \n");
                if (strFHF != "" && strFHF != null)
                    strSql.Append("and isnull(vcBoxNo,'') like '"+strFHF+"%' ");
                if (vcPart_id != "" && vcPart_id != null)
                    strSql.Append("and isnull(vcPart_id,'') like '"+vcPart_id+"%'  \n");
                if (vcLianFanNo != "" && vcLianFanNo != null)
                    strSql.Append("and isnull(vcLianFanNo,'') like '"+vcLianFanNo+"%'  \n");
                if (vcOrderNo != "" && vcOrderNo != null)
                    strSql.Append("and isnull(vcOrderNo,'') like '"+vcOrderNo+"%'  \n");
                strSql.Append("order by vcBoxNo,vcInstructionNo, vcPart_id  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Search_sub(string vcBox_id, string iQuantity)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select t1.vcBoxNo,t1.vcCaseNo,t1.vcSheBeiNo,isnull(t2.iQuantity,0) as iQuantity,'0' as vcModFlag,'0' as vcAddFlag from ");
                strSql.AppendLine("(");
                strSql.AppendLine("	select * from TCaseInfo ");
                strSql.AppendLine(")t1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(");
                strSql.AppendLine("	select vcCaseNo,vcBoxNo,COUNT(vcInstructionNo) as iQuantity,sum(iPrint) as iPrint ");
                strSql.AppendLine("	from ");
                strSql.AppendLine("	(");
                strSql.AppendLine("		select vcCaseNo,vcBoxNo,vcInstructionNo,case when dPrintBoxTime is null then 0 else 1 end as iPrint ");
                strSql.AppendLine("		from TBoxMaster ");
                strSql.AppendLine("	)a");
                strSql.AppendLine("	group by vcCaseNo,vcBoxNo");
                strSql.AppendLine(")t2");
                strSql.AppendLine("on t1.vcCaseNo=t2.vcCaseNo");
                strSql.AppendLine("where ISNULL(t2.iPrint,0)=0 ");

                if (vcBox_id != "" && vcBox_id != null)
                    strSql.AppendLine("and isnull(t1.vcBoxNo,'') = '" + vcBox_id + "' ");
                if (iQuantity == "0")//=0个
                    strSql.AppendLine("and isnull(t2.iQuantity,0)=0 ");
                if(iQuantity=="1")//>=1个
                    strSql.AppendLine("and isnull(t2.iQuantity,0)>=1 ");

                strSql.AppendLine("ORDER BY t1.iAutoId");

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
                        sql.Append("UPDATE [TBoxMaster]  \n");
                        sql.Append("   SET   \n");
                        sql.Append("       [iQuantity] = " + listInfoData[i]["iQuantity"].ToString() + "  \n");
                        sql.Append("      ,[vcOperatorID] = '" + strUserId + "'  \n");
                        sql.Append("      ,[dOperatorTime] = getdate()  \n");
                        sql.Append(" WHERE iAutoId=" + iAutoId + " and isnull(vcDelete,'')!='1' and dPrintBoxTime is null  \n");
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
                    sql.Append("update TBoxMaster set vcDelete='1' where iAutoId=" + iAutoId + " and isnull(vcDelete,'')!='1' and dPrintBoxTime is null  \n");
                    //sql.Append("delete from TBoxMaster where iAutoId=" + iAutoId + "   \n");
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
        public void Del_sub(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string vcCaseNo = checkedInfoData[i]["vcCaseNo"].ToString();
                    int iQuantity =Convert.ToInt32(checkedInfoData[i]["iQuantity"].ToString());

                    if (iQuantity == 0)
                        sql.AppendLine("delete from TCaseInfo where vcCaseNo='"+vcCaseNo+"'");
                    if(iQuantity>=1)
                    {
                        sql.AppendLine("delete from TCaseInfo where vcCaseNo='" + vcCaseNo + "'");
                        sql.AppendLine("update TBoxMaster set vcDelete='1' where vcCaseNo='" + vcCaseNo + "'");
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

    }
}
