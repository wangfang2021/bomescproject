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
    public class FS0631_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索NQC结果
        public DataTable SearchNQCResult(string strCLYM, string strDXYM, string strPartNo)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select iAutoId,Part_No,Part_No+isnull(Source_Code,'')+isnull(Parts_Master_Matching_Key,'') as Part_No_Disp,     \n");
                sql.Append("SUBSTRING(Process_YYYYMM,1,4)+'-'+SUBSTRING(Process_YYYYMM,5,2)+'-01' as Process_YYYYMM,    \n");
                sql.Append("SUBSTRING(Start_date_for_daily_qty,1,4)+'-'+SUBSTRING(Start_date_for_daily_qty,5,2)+'-01' as Start_date_for_daily_qty,    \n");
                sql.Append("Daily_Qty_01,Daily_Qty_02,Daily_Qty_03,Daily_Qty_04,Daily_Qty_05,Daily_Qty_06,Daily_Qty_07,Daily_Qty_08,    \n");
                sql.Append("Daily_Qty_09,Daily_Qty_10,Daily_Qty_11,Daily_Qty_12,Daily_Qty_13,Daily_Qty_14,Daily_Qty_15,Daily_Qty_16,    \n");
                sql.Append("Daily_Qty_17,Daily_Qty_18,Daily_Qty_19,Daily_Qty_20,Daily_Qty_21,Daily_Qty_22,Daily_Qty_23,Daily_Qty_24,    \n");
                sql.Append("Daily_Qty_25,Daily_Qty_26,Daily_Qty_27,Daily_Qty_28,Daily_Qty_29,Daily_Qty_30,Daily_Qty_31,    \n");
                sql.Append("'0' as vcModFlag,'0' as vcAddFlag    \n");
                sql.Append("from TNQCReceiveInfo where 1=1    \n");
                if (strCLYM != null && strCLYM != "")
                    sql.Append("and isnull(Process_YYYYMM,'')='" + strCLYM + "'    \n");
                if (strDXYM != null && strDXYM != "")
                    sql.Append("and isnull(Start_date_for_daily_qty,'') like '" + strDXYM + "%'    \n");
                if (strPartNo != null && strPartNo != "")
                    sql.Append("and Part_No+isnull(Source_Code,'')+isnull(Parts_Master_Matching_Key,'') like '%" + strPartNo + "%'    \n");
                sql.Append("order by Part_No,Start_date_for_daily_qty    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存NQC结果
        public void SaveNQCResult(List<Dictionary<string, Object>> listInfoData, string strUserId)
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

                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        #region modify sql
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("update TNQCReceiveInfo set   \n");
                        sql.Append("Daily_Qty_01='" + listInfoData[i]["Daily_Qty_01"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_02='" + listInfoData[i]["Daily_Qty_02"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_03='" + listInfoData[i]["Daily_Qty_03"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_04='" + listInfoData[i]["Daily_Qty_04"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_05='" + listInfoData[i]["Daily_Qty_05"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_06='" + listInfoData[i]["Daily_Qty_06"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_07='" + listInfoData[i]["Daily_Qty_07"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_08='" + listInfoData[i]["Daily_Qty_08"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_09='" + listInfoData[i]["Daily_Qty_09"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_10='" + listInfoData[i]["Daily_Qty_10"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_11='" + listInfoData[i]["Daily_Qty_11"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_12='" + listInfoData[i]["Daily_Qty_12"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_13='" + listInfoData[i]["Daily_Qty_13"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_14='" + listInfoData[i]["Daily_Qty_14"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_15='" + listInfoData[i]["Daily_Qty_15"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_16='" + listInfoData[i]["Daily_Qty_16"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_17='" + listInfoData[i]["Daily_Qty_17"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_18='" + listInfoData[i]["Daily_Qty_18"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_19='" + listInfoData[i]["Daily_Qty_19"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_20='" + listInfoData[i]["Daily_Qty_20"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_21='" + listInfoData[i]["Daily_Qty_21"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_22='" + listInfoData[i]["Daily_Qty_22"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_23='" + listInfoData[i]["Daily_Qty_23"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_24='" + listInfoData[i]["Daily_Qty_24"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_25='" + listInfoData[i]["Daily_Qty_25"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_26='" + listInfoData[i]["Daily_Qty_26"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_27='" + listInfoData[i]["Daily_Qty_27"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_28='" + listInfoData[i]["Daily_Qty_28"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_29='" + listInfoData[i]["Daily_Qty_29"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_30='" + listInfoData[i]["Daily_Qty_30"].ToString() + "',    \n");
                        sql.Append("Daily_Qty_31='" + listInfoData[i]["Daily_Qty_31"].ToString() + "',    \n");
                        sql.Append("vcOperatorID='"+strUserId+"',   \n");
                        sql.Append("dOperatorTime=getdate()    \n");
                        sql.Append("where iAutoId=" + iAutoId + "    \n");
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

        #region 删除NQC结果
        public void DelNQCResult(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from TNQCReceiveInfo where iAutoId=" + iAutoId + "   \n");
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
