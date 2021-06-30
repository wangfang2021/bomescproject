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
    public class FS0813_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string strSellNo, string strStartTime, string strEndTime, string strYinQuType, string strSHF,string strLabelID,
            string vcBanZhi,string vcQianFen,string vcBianCi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.vcYinQuType,t2.vcName as vcYinQuTypeName,t1.vcBianCi,t1.vcSellNo,t1.iToolQuantity,t1.vcTruckNo,t1.vcBanZhi,t1.vcQianFen, \n");
                strSql.Append("convert(varchar(16),t1.dOperatorTime,120) as dOperatorTime,convert(varchar(10),t1.vcDate,120) as vcDate,isnull(t1.vcSender,'') as vcSender,'0' as vcModFlag,'0' as vcAddFlag      \n");
                strSql.Append("from TSell_Sum t1   \n");
                strSql.Append("left join (select * from tCode where vcCodeid='C058') t2 on t1.vcYinQuType=t2.vcValue   \n");
                //strSql.Append("left join (select vcSellNo,sum(iToolQuantity) as iToolQuantity from TSell_Tool group by vcSellNo)t3 on t1.vcSellNo=t3.vcSellNo  \n");
                strSql.Append("    \n");
                strSql.Append("where 1=1 \n");
                if (strSellNo != "" && strSellNo != null)
                    strSql.Append("and isnull(t1.vcSellNo,'') = '" + strSellNo + "' \n");
                if (strStartTime == "" || strStartTime == null)
                    strStartTime = "2001-01-01 00:00";
                if (strEndTime == "" || strEndTime == null)
                    strEndTime = "2099-12-31 23:59";
                strSql.Append("and isnull(t1.vcDate,'2001-01-01 00:00') >= '" + strStartTime + "' and isnull(t1.vcDate,'2099-12-31 23:59') <= '" + strEndTime + "'  \n");
                if (strYinQuType != "" && strYinQuType != null)
                    strSql.Append("and isnull(t1.vcYinQuType,'') = '" + strYinQuType + "' \n");
                if (strSHF != "" && strSHF != null)
                    strSql.Append("and isnull(t1.vcSHF,'')='" + strSHF + "'    \n");
                if (strLabelID != "" && strLabelID != null)
                    strSql.Append("and '"+strLabelID+ "' between isnull(t1.vcLabelStart,'') and isnull(t1.vcLabelEnd,'')    \n");
                if(vcBanZhi!="" && vcBanZhi!=null)
                    strSql.Append("and isnull(t1.vcBanZhi,'') = '" + vcBanZhi + "' \n");
                if (vcQianFen != "" && vcQianFen != null)
                    strSql.Append("and isnull(t1.vcQianFen,'') = '" + vcQianFen + "' \n");
                if (vcBianCi != "" && vcBianCi != null)
                    strSql.Append("and isnull(t1.vcBianCi,'') = '" + vcBianCi + "' \n");
                strSql.Append("order by t1.vcDate,t1.vcSellNo    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 子画面初始化
        public DataTable initSubApi(string strSellNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from TSell  \n");
                strSql.Append("where isnull(vcSellNo,'') = '" + strSellNo + "' \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable initSubApi2(string strSellNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from TSell_Tool  \n");
                strSql.Append("where isnull(vcSellNo,'') = '" + strSellNo + "' \n");
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

                    if (baddflag == true)
                    {//新增
                        //无新增情况
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        #region modify sql
                        string vcSellNo = listInfoData[i]["vcSellNo"] == null ? "" : listInfoData[i]["vcSellNo"].ToString();
                        string vcBianCi = listInfoData[i]["vcBianCi"] == null ? "" : listInfoData[i]["vcBianCi"].ToString();
                        string vcTruckNo = listInfoData[i]["vcTruckNo"] == null ? "" : listInfoData[i]["vcTruckNo"].ToString();
                        string vcDate= listInfoData[i]["vcDate"] == null ? "" : listInfoData[i]["vcDate"].ToString();
                        string vcBanZhi= listInfoData[i]["vcBanZhi"] == null ? "" : listInfoData[i]["vcBanZhi"].ToString();
                        string vcQianFen= listInfoData[i]["vcQianFen"] == null ? "" : listInfoData[i]["vcQianFen"].ToString();

                        sql.Append("update TSell set vcBianCi='" + vcBianCi + "',vcTruckNo='" + vcTruckNo + "' where vcSellNo='" + vcSellNo + "'   \n");
                        sql.Append("update TSell_Sum set vcBianCi='" + vcBianCi + "',vcTruckNo='" + vcTruckNo + "',vcDate=nullif('"+vcDate+ "',''),vcBanZhi='"+vcBanZhi+ "',vcQianFen='"+vcQianFen+"' where vcSellNo='" + vcSellNo + "'   \n");
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
    }
}
