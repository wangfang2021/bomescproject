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
    public class FS0313_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 按检索条件检索,返回dt
        public DataTable Search(string strMaxNum,string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            ,  string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState
            )
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                if(strMaxNum!="")
                    strSql.Append("       select top "+ strMaxNum + " *         \n");
                else
                    strSql.Append("       select *         \n");
                strSql.Append("       ,b.vcName as 'vcChange_Name'     \n");
                strSql.Append("       ,b2.vcName as 'vcHaoJiu_Name'      \n");
                strSql.Append("       ,b3.vcName as 'vcProjectType_Name'      \n");
                strSql.Append("       ,b4.vcName as 'vcOE_Name'      \n");
                strSql.Append("       ,b5.vcName as 'vcPriceState_Name'      \n");
                strSql.Append("       ,b6.vcName as 'vcOriginCompany_Name'      \n");
                strSql.Append("       ,b7.vcName as 'vcReceiver_Name'      \n");
                strSql.Append("       ,b8.vcName as 'vcPriceGS_Name'      \n");
                strSql.Append("       ,b9.vcName as 'vcPriceChangeInfo_Name'        \n");
                strSql.Append("       ,'0' as vcModFlag,'0' as vcAddFlag     \n");
                strSql.Append("       ,CONVERT(varchar(100),dUseBegin, 111) as dUseBeginStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dUseEnd, 111) as dUseEndStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dProjectBegin, 111) as dProjectBeginStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dProjectEnd, 111) as dProjectEndStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dJiuBegin, 111) as dJiuBeginStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dJiuEnd, 111) as dJiuEndStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dJiuBeginSustain, 111) as dJiuBeginSustainStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dPriceStateDate, 111) as dPriceStateDateStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dPricebegin, 111) as dPricebeginStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dPriceEnd, 111) as dPriceEndStr      \n");

                strSql.Append("       from TPrice a     \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C002'      \n");
                strSql.Append("       )b on a.vcChange=b.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C004'      \n");
                strSql.Append("       )b2 on a.vcHaoJiu=b2.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C003'      \n");
                strSql.Append("       )b3 on a.vcProjectType=b3.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C012'      \n");
                strSql.Append("       )b4 on a.vcOE=b4.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C013'      \n");
                strSql.Append("       )b5 on a.vcPriceState=b5.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C006'      \n");
                strSql.Append("       )b6 on a.vcOriginCompany=b6.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C005'      \n");
                strSql.Append("       )b7 on a.vcReceiver=b7.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C038'      \n");
                strSql.Append("       )b8 on a.vcPriceGS=b8.vcValue      \n");
                strSql.Append("       left join             \n");
                strSql.Append("       (             \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C002'        \n");
                strSql.Append("       )b9 on a.vcPriceChangeInfo=b9.vcValue           \n");
                strSql.Append("       where          \n");
                strSql.Append("       vcProjectType='0'        \n");//财务用户固定看内制的
                if (strChange != null && strChange != "" && strChange != "空" && strChange != "处理中")
                    strSql.Append("       and vcChange='" + strChange + "'         \n");

                if (strChange != null && strChange != "" && strChange == "处理中")
                    strSql.Append("       and isnull(vcChange,'')<>''         \n");

                if (strChange != null && strChange == "空")
                    strSql.Append("       and vcChange=''         \n");

                if (strPart_id != null && strPart_id != "")
                    strSql.Append("       and vcPart_id like '%" + strPart_id + "%'         \n");
                if (strOriginCompany != null && strOriginCompany != "")
                    strSql.Append("       and vcOriginCompany like '%" + strOriginCompany + "%'         \n");
                if (strHaoJiu != null && strHaoJiu != "")
                    strSql.Append("       and vcHaoJiu='" + strHaoJiu + "'         \n");
                if (strPriceChangeInfo != null && strPriceChangeInfo != "")
                    strSql.Append("       and vcPriceChangeInfo='" + strPriceChangeInfo + "'         \n");
                if (strCarTypeDev != null && strCarTypeDev != "")
                    strSql.Append("       and vcCarTypeDev like '" + strCarTypeDev + "%'         \n");
                if (strSupplier_id != null && strSupplier_id != "")
                    strSql.Append("       and vcSupplier_id like '" + strSupplier_id + "%'         \n");
                if (strReceiver != null && strReceiver != "")
                    strSql.Append("       and vcReceiver like '%" + strReceiver + "%'         \n");
                if (strPriceState != null && strPriceState != "")
                    strSql.Append("       and vcPriceState='" + strPriceState + "'         \n");
                else
                    strSql.Append("       and vcPriceState in('1','2','3','4')        \n");


                strSql.Append("     order by  vcPart_id,vcReceiver,vcSupplier_id,iAutoId asc    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索所有待处理的数据
        public DataTable getAllTask()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select  count(*) as iNum from TPrice where vcPriceState='1' and vcProjectType='0'     \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
 
        #region 财务保存
        public void SaveCaiWu(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append("  update TPrice set    \r\n");
                    sql.Append("   decPriceOrigin_CW=" + ComFunction.getSqlValue(listInfoData[i]["decPriceOrigin_CW"], true) + "   \r\n");
                    sql.Append("  ,vcOperatorID_CW='" + strUserId + "'   \r\n");
                    sql.Append("  ,dOperatorTime_CW=getDate()   \r\n");
                    sql.Append("  where iAutoId=" + iAutoId + " and vcPriceState='1'  ; \r\n");//有且只有已送信的才能修改
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 财务回复
        public void OKCaiWu(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append("  update TPrice set    \r\n");
                    sql.Append("  decPriceOrigin=decPriceOrigin_CW,vcPriceState='2'   \r\n");
                    sql.Append("  ,vcOperatorID_CW='" + strUserId + "'   \r\n");
                    sql.Append("  ,dOperatorTime_CW=getDate()   \r\n");
                    sql.Append("  where iAutoId=" + iAutoId + " and vcPriceState='1'  ; \r\n");//有且只有已送信的才能修改
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
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
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strAutoId = dt.Rows[i]["iAutoId"] == System.DBNull.Value ? "" : dt.Rows[i]["iAutoId"].ToString();
                    string vcPart_id = dt.Rows[i]["vcPart_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPart_id"].ToString();
                    sql.Append("  update TPrice set    \r\n");
                    sql.Append("  decPriceOrigin_CW=" + ComFunction.getSqlValue(dt.Rows[i]["decPriceOrigin_CW"], true) + "   \r\n");
                    sql.Append("  ,vcOperatorID_CW='" + strUserId + "'   \r\n");
                    sql.Append("  ,dOperatorTime_CW=getDate()   \r\n");
                    sql.Append("  where iAutoId=" + strAutoId + "  ; \r\n");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 验证品番是否都是已经送信
        public DataTable checkState(string strAutoIds)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  select vcPart_id from tprice  where iAutoId in     \r\n");
                sql.Append("  (     \r\n");
                sql.Append("  "+ strAutoIds + "     \r\n");
                sql.Append("  )     \r\n");
                sql.Append("  and vcPriceState<>'1'     \r\n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}