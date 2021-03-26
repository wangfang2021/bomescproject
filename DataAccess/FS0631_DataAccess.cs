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

        #region 获取数据字典
        public DataTable getTCode(string strCodeId, string unit)
        {
            try
            {
                MultiExcute excute = new MultiExcute();
                System.Data.DataTable dt = new System.Data.DataTable();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select '" + unit + "'+vcValue as vcName,'" + unit + "'+vcValue as vcValue from TCode where vcCodeId='" + strCodeId + "'  ORDER BY iAutoId    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索NQC结果
        public DataTable SearchNQCResult(string strCLYM, string strDXYM, string strPartNo,string strPlant)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select iAutoId,Part_No,replace(Part_No+isnull(Part_Suffix,'')+isnull(Source_Code,'')+isnull(Parts_Master_Matching_Key,''),' ','') as Part_No_Disp,     \n");
                sql.Append("SUBSTRING(Process_YYYYMM,1,4)+'-'+SUBSTRING(Process_YYYYMM,5,2)+'-01' as Process_YYYYMM,    \n");
                sql.Append("SUBSTRING(Start_date_for_daily_qty,1,4)+'-'+SUBSTRING(Start_date_for_daily_qty,5,2)+'-01' as Start_date_for_daily_qty, Process_Factory,   \n");
                sql.Append("cast(Daily_Qty_01 as int) as Daily_Qty_01,cast(Daily_Qty_02 as int) as Daily_Qty_02,cast(Daily_Qty_03 as int) as Daily_Qty_03,    \n");
                sql.Append("cast(Daily_Qty_04 as int) as Daily_Qty_04,cast(Daily_Qty_05 as int) as Daily_Qty_05,cast(Daily_Qty_06 as int) as Daily_Qty_06,    \n");
                sql.Append("cast(Daily_Qty_07 as int) as Daily_Qty_07,cast(Daily_Qty_08 as int) as Daily_Qty_08,cast(Daily_Qty_09 as int) as Daily_Qty_09,    \n");
                sql.Append("cast(Daily_Qty_10 as int) as Daily_Qty_10,cast(Daily_Qty_11 as int) as Daily_Qty_11,cast(Daily_Qty_12 as int) as Daily_Qty_12,    \n");
                sql.Append("cast(Daily_Qty_13 as int) as Daily_Qty_13,cast(Daily_Qty_14 as int) as Daily_Qty_14,cast(Daily_Qty_15 as int) as Daily_Qty_15,    \n");
                sql.Append("cast(Daily_Qty_16 as int) as Daily_Qty_16,cast(Daily_Qty_17 as int) as Daily_Qty_17,cast(Daily_Qty_18 as int) as Daily_Qty_18,    \n");
                sql.Append("cast(Daily_Qty_19 as int) as Daily_Qty_19,cast(Daily_Qty_20 as int) as Daily_Qty_20,cast(Daily_Qty_21 as int) as Daily_Qty_21,    \n");
                sql.Append("cast(Daily_Qty_22 as int) as Daily_Qty_22,cast(Daily_Qty_23 as int) as Daily_Qty_23,cast(Daily_Qty_24 as int) as Daily_Qty_24,    \n");
                sql.Append("cast(Daily_Qty_25 as int) as Daily_Qty_25,cast(Daily_Qty_26 as int) as Daily_Qty_26,cast(Daily_Qty_27 as int) as Daily_Qty_27,    \n");
                sql.Append("cast(Daily_Qty_28 as int) as Daily_Qty_28,cast(Daily_Qty_29 as int) as Daily_Qty_29,cast(Daily_Qty_30 as int) as Daily_Qty_30,    \n");
                sql.Append("cast(Daily_Qty_31 as int) as Daily_Qty_31,    \n");
                sql.Append("'0' as vcModFlag,'0' as vcAddFlag,    \n");
                sql.Append("Process_YYYYMM as vcCLYM,SUBSTRING(Start_date_for_daily_qty,1,6) as vcDXYM    \n");
                sql.Append("from TNQCReceiveInfo where 1=1    \n");
                if (strPlant != null && strPlant != "")
                    sql.Append("and isnull(Process_Factory,'')='"+strPlant+"'    \n");
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
                        string Part_No_disp = listInfoData[i]["Part_No_Disp"].ToString();
                        string Part_No = Part_No_disp.Substring(0, 10);
                        string Part_Suffix = Part_No_disp.Substring(10, 2);
                        string Source_Code = Part_No_disp.Substring(12, 1);
                        string Parts_Master_Matching_Key = Part_No_disp.Substring(13, Part_No_disp.Length - 13);

                        #region insert sql
                        sql.Append("INSERT INTO [TNQCReceiveInfo]    \n");
                        sql.Append("           ([Process_YYYYMM]    \n");
                        sql.Append("           ,[Process_Factory]    \n");
                        sql.Append("           ,[Process_Cycle_NO]    \n");
                        sql.Append("           ,[User_Code]    \n");
                        sql.Append("           ,[Car_Family_Code]    \n");
                        sql.Append("           ,[Spec_Sheet_No]    \n");
                        sql.Append("           ,[Basic]    \n");
                        sql.Append("           ,[Part_No]    \n");
                        sql.Append("           ,[Part_Suffix]    \n");
                        sql.Append("           ,[Source_Code]    \n");
                        sql.Append("           ,[Parts_Master_Matching_Key]    \n");
                        sql.Append("           ,[Production_Routing_1]    \n");
                        sql.Append("           ,[Production_Routing_2]    \n");
                        sql.Append("           ,[Production_Routing_3]    \n");
                        sql.Append("           ,[Production_Routing_4]    \n");
                        sql.Append("           ,[Production_Routing_5]    \n");
                        sql.Append("           ,[Production_Routing_6]    \n");
                        sql.Append("           ,[Production_Routing_7]    \n");
                        sql.Append("           ,[Production_Routing_8]    \n");
                        sql.Append("           ,[Production_Routing_9]    \n");
                        sql.Append("           ,[Production_Routing_10]    \n");
                        sql.Append("           ,[Production_Routing_11]    \n");
                        sql.Append("           ,[Production_Routing_12]    \n");
                        sql.Append("           ,[Production_Routing_13]    \n");
                        sql.Append("           ,[Production_Routing_14]    \n");
                        sql.Append("           ,[Final_Routing]    \n");
                        sql.Append("           ,[Routing_Position]    \n");
                        sql.Append("           ,[ID_Line]    \n");
                        sql.Append("           ,[Hikiate_ID_Line]    \n");
                        sql.Append("           ,[Destination]    \n");
                        sql.Append("           ,[Parts_order_condition]    \n");
                        sql.Append("           ,[Shipping_Parts_Flag]    \n");
                        sql.Append("           ,[Ownership_Company]    \n");
                        sql.Append("           ,[Representative_Part_NO_fiag]    \n");
                        sql.Append("           ,[Off_Option_Code]    \n");
                        sql.Append("           ,[Production_Plan_Type]    \n");
                        sql.Append("           ,[Free_Area_NQC_Comp_Code]    \n");
                        sql.Append("           ,[Free_Area_IDE_Flg]    \n");
                        sql.Append("           ,[Free_Area_Blank]    \n");
                        sql.Append("           ,[Start_date_for_daily_qty]    \n");
                        sql.Append("           ,[Effective_range_for_daily_qty]    \n");
                        sql.Append("           ,[Daily_Signal_01]    \n");
                        sql.Append("           ,[Daily_Qty_01]    \n");
                        sql.Append("           ,[Daily_Signal_02]    \n");
                        sql.Append("           ,[Daily_Qty_02]    \n");
                        sql.Append("           ,[Daily_Signal_03]    \n");
                        sql.Append("           ,[Daily_Qty_03]    \n");
                        sql.Append("           ,[Daily_Signal_04]    \n");
                        sql.Append("           ,[Daily_Qty_04]    \n");
                        sql.Append("           ,[Daily_Signal_05]    \n");
                        sql.Append("           ,[Daily_Qty_05]    \n");
                        sql.Append("           ,[Daily_Signal_06]    \n");
                        sql.Append("           ,[Daily_Qty_06]    \n");
                        sql.Append("           ,[Daily_Signal_07]    \n");
                        sql.Append("           ,[Daily_Qty_07]    \n");
                        sql.Append("           ,[Daily_Signal_08]    \n");
                        sql.Append("           ,[Daily_Qty_08]    \n");
                        sql.Append("           ,[Daily_Signal_09]    \n");
                        sql.Append("           ,[Daily_Qty_09]    \n");
                        sql.Append("           ,[Daily_Signal_10]    \n");
                        sql.Append("           ,[Daily_Qty_10]    \n");
                        sql.Append("           ,[Daily_Signal_11]    \n");
                        sql.Append("           ,[Daily_Qty_11]    \n");
                        sql.Append("           ,[Daily_Signal_12]    \n");
                        sql.Append("           ,[Daily_Qty_12]    \n");
                        sql.Append("           ,[Daily_Signal_13]    \n");
                        sql.Append("           ,[Daily_Qty_13]    \n");
                        sql.Append("           ,[Daily_Signal_14]    \n");
                        sql.Append("           ,[Daily_Qty_14]    \n");
                        sql.Append("           ,[Daily_Signal_15]    \n");
                        sql.Append("           ,[Daily_Qty_15]    \n");
                        sql.Append("           ,[Daily_Signal_16]    \n");
                        sql.Append("           ,[Daily_Qty_16]    \n");
                        sql.Append("           ,[Daily_Signal_17]    \n");
                        sql.Append("           ,[Daily_Qty_17]    \n");
                        sql.Append("           ,[Daily_Signal_18]    \n");
                        sql.Append("           ,[Daily_Qty_18]    \n");
                        sql.Append("           ,[Daily_Signal_19]    \n");
                        sql.Append("           ,[Daily_Qty_19]    \n");
                        sql.Append("           ,[Daily_Signal_20]    \n");
                        sql.Append("           ,[Daily_Qty_20]    \n");
                        sql.Append("           ,[Daily_Signal_21]    \n");
                        sql.Append("           ,[Daily_Qty_21]    \n");
                        sql.Append("           ,[Daily_Signal_22]    \n");
                        sql.Append("           ,[Daily_Qty_22]    \n");
                        sql.Append("           ,[Daily_Signal_23]    \n");
                        sql.Append("           ,[Daily_Qty_23]    \n");
                        sql.Append("           ,[Daily_Signal_24]    \n");
                        sql.Append("           ,[Daily_Qty_24]    \n");
                        sql.Append("           ,[Daily_Signal_25]    \n");
                        sql.Append("           ,[Daily_Qty_25]    \n");
                        sql.Append("           ,[Daily_Signal_26]    \n");
                        sql.Append("           ,[Daily_Qty_26]    \n");
                        sql.Append("           ,[Daily_Signal_27]    \n");
                        sql.Append("           ,[Daily_Qty_27]    \n");
                        sql.Append("           ,[Daily_Signal_28]    \n");
                        sql.Append("           ,[Daily_Qty_28]    \n");
                        sql.Append("           ,[Daily_Signal_29]    \n");
                        sql.Append("           ,[Daily_Qty_29]    \n");
                        sql.Append("           ,[Daily_Signal_30]    \n");
                        sql.Append("           ,[Daily_Qty_30]    \n");
                        sql.Append("           ,[Daily_Signal_31]    \n");
                        sql.Append("           ,[Daily_Qty_31]    \n");
                        sql.Append("           ,[Cycle_Flag]    \n");
                        sql.Append("           ,[Start_Date_For_Total_Qty]    \n");
                        sql.Append("           ,[Effective_Range_For_Total_Qty]    \n");
                        sql.Append("           ,[Total_Signal_01]    \n");
                        sql.Append("           ,[Total_Qty_01]    \n");
                        sql.Append("           ,[Total_Signal_02]    \n");
                        sql.Append("           ,[Total_Qty_02]    \n");
                        sql.Append("           ,[Total_Signal_03]    \n");
                        sql.Append("           ,[Total_Qty_03]    \n");
                        sql.Append("           ,[Use_Part]    \n");
                        sql.Append("           ,[Management_Production]    \n");
                        sql.Append("           ,[Comment_Flag18]    \n");
                        sql.Append("           ,[Dummy_Data]    \n");
                        sql.Append("           ,[vcOperatorID]    \n");
                        sql.Append("           ,[dOperatorTime])    \n");
                        sql.Append("     VALUES    \n");
                        sql.Append("           ('" + listInfoData[i]["Process_YYYYMM"].ToString().Replace("/","") + "'    \n");//[Process_YYYYMM]
                        sql.Append("           ,'" + listInfoData[i]["Process_Factory"].ToString() + "'    \n");//[Process_Factory]
                        sql.Append("           ,0    \n");//[Process_Cycle_NO]
                        sql.Append("           ,''    \n");//[User_Code]
                        sql.Append("           ,''    \n");//[Car_Family_Code]
                        sql.Append("		   ,''    \n");//[Spec_Sheet_No]
                        sql.Append("           ,''    \n");//[Basic]
                        sql.Append("           ,'" + Part_No + "'    \n");//[Part_No]
                        sql.Append("           ,'" + Part_Suffix + "'    \n");//[Part_Suffix]
                        sql.Append("           ,'" + Source_Code + "'    \n");//[Source_Code]
                        sql.Append("           ,'" + Parts_Master_Matching_Key + "'    \n");//[Parts_Master_Matching_Key]
                        sql.Append("           ,''    \n");//[Production_Routing_1]
                        sql.Append("           ,''    \n");//[Production_Routing_2]
                        sql.Append("           ,''    \n");//[Production_Routing_3]
                        sql.Append("           ,''    \n");//[Production_Routing_4]
                        sql.Append("           ,''    \n");//[Production_Routing_5]
                        sql.Append("           ,''    \n");//[Production_Routing_6]
                        sql.Append("           ,''    \n");//[Production_Routing_7]
                        sql.Append("           ,''    \n");//[Production_Routing_8]
                        sql.Append("           ,''    \n");//[Production_Routing_9]
                        sql.Append("           ,''    \n");//[Production_Routing_10]
                        sql.Append("           ,''    \n");//[Production_Routing_11]
                        sql.Append("           ,''    \n");//[Production_Routing_12]
                        sql.Append("           ,''    \n");//[Production_Routing_13]
                        sql.Append("           ,''    \n");//[Production_Routing_14]
                        sql.Append("           ,''    \n");//[Final_Routing]
                        sql.Append("           ,''    \n");//[Routing_Position]
                        sql.Append("           ,''    \n");//[ID_Line]
                        sql.Append("           ,''    \n");//[Hikiate_ID_Line]
                        sql.Append("           ,''    \n");//[Destination]
                        sql.Append("           ,''    \n");//[Parts_order_condition]
                        sql.Append("           ,''    \n");//[Shipping_Parts_Flag]
                        sql.Append("           ,''    \n");//[Ownership_Company]
                        sql.Append("           ,''    \n");//[Representative_Part_NO_fiag]
                        sql.Append("           ,''    \n");//[Off_Option_Code]
                        sql.Append("           ,''    \n");//[Production_Plan_Type]
                        sql.Append("           ,''    \n");//[Free_Area_NQC_Comp_Code]
                        sql.Append("           ,''    \n");//[Free_Area_IDE_Flg]
                        sql.Append("           ,''    \n");//[Free_Area_Blank]
                        sql.Append("           ,'" + listInfoData[i]["Start_date_for_daily_qty"].ToString().Replace("/", "") + "01'    \n");//[Start_date_for_daily_qty]
                        sql.Append("           ,'31'    \n");//[Effective_range_for_daily_qty]
                        sql.Append("           ,''    \n");//[Daily_Signal_01]
                        string Daily_Qty_01 = listInfoData[i]["Daily_Qty_01"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_01"].ToString();
                        sql.Append("           ,'"+Daily_Qty_01+"'    \n");//[Daily_Qty_01]
                        sql.Append("           ,''    \n");//[Daily_Signal_02]
                        string Daily_Qty_02 = listInfoData[i]["Daily_Qty_02"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_02"].ToString();
                        sql.Append("           ,'" + Daily_Qty_02 + "'    \n");//[Daily_Qty_02]
                        sql.Append("           ,''    \n");//[Daily_Signal_03]
                        string Daily_Qty_03 = listInfoData[i]["Daily_Qty_03"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_03"].ToString();
                        sql.Append("           ,'" + Daily_Qty_03 + "'    \n");//[Daily_Qty_03]
                        sql.Append("           ,''    \n");//[Daily_Signal_04]
                        string Daily_Qty_04 = listInfoData[i]["Daily_Qty_04"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_04"].ToString();
                        sql.Append("           ,'" + Daily_Qty_04 + "'    \n");//[Daily_Qty_04]
                        sql.Append("           ,''    \n");//[Daily_Signal_05]
                        string Daily_Qty_05 = listInfoData[i]["Daily_Qty_05"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_05"].ToString();
                        sql.Append("           ,'" + Daily_Qty_05 + "'    \n");//[Daily_Qty_05]
                        sql.Append("           ,''    \n");//[Daily_Signal_06]
                        string Daily_Qty_06 = listInfoData[i]["Daily_Qty_06"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_06"].ToString();
                        sql.Append("           ,'" + Daily_Qty_06 + "'    \n");//[Daily_Qty_06]
                        sql.Append("           ,''    \n");//[Daily_Signal_07]
                        string Daily_Qty_07 = listInfoData[i]["Daily_Qty_07"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_07"].ToString();
                        sql.Append("           ,'" + Daily_Qty_07 + "'    \n");//[Daily_Qty_07]
                        sql.Append("           ,''    \n");//[Daily_Signal_08]
                        string Daily_Qty_08 = listInfoData[i]["Daily_Qty_08"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_08"].ToString();
                        sql.Append("           ,'" + Daily_Qty_08 + "'    \n");//[Daily_Qty_08]
                        sql.Append("           ,''    \n");//[Daily_Signal_09]
                        string Daily_Qty_09 = listInfoData[i]["Daily_Qty_09"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_09"].ToString();
                        sql.Append("           ,'" + Daily_Qty_09 + "'    \n");//[Daily_Qty_09]
                        sql.Append("           ,''    \n");//[Daily_Signal_10]
                        string Daily_Qty_10 = listInfoData[i]["Daily_Qty_10"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_10"].ToString();
                        sql.Append("           ,'" + Daily_Qty_10 + "'    \n");//[Daily_Qty_10]
                        sql.Append("           ,''    \n");//[Daily_Signal_11]
                        string Daily_Qty_11 = listInfoData[i]["Daily_Qty_11"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_11"].ToString();
                        sql.Append("           ,'" + Daily_Qty_11 + "'    \n");//[Daily_Qty_11]
                        sql.Append("           ,''    \n");//[Daily_Signal_12]
                        string Daily_Qty_12 = listInfoData[i]["Daily_Qty_12"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_12"].ToString();
                        sql.Append("           ,'" + Daily_Qty_12 + "'    \n");//[Daily_Qty_12]
                        sql.Append("           ,''    \n");//[Daily_Signal_13]
                        string Daily_Qty_13 = listInfoData[i]["Daily_Qty_13"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_13"].ToString();
                        sql.Append("           ,'" + Daily_Qty_13 + "'    \n");//[Daily_Qty_13]
                        sql.Append("           ,''    \n");//[Daily_Signal_14]
                        string Daily_Qty_14 = listInfoData[i]["Daily_Qty_14"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_14"].ToString();
                        sql.Append("           ,'" + Daily_Qty_14 + "'    \n");//[Daily_Qty_14]
                        sql.Append("           ,''    \n");//[Daily_Signal_15]
                        string Daily_Qty_15 = listInfoData[i]["Daily_Qty_15"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_15"].ToString();
                        sql.Append("           ,'" + Daily_Qty_15 + "'    \n");//[Daily_Qty_15]
                        sql.Append("           ,''    \n");//[Daily_Signal_16]
                        string Daily_Qty_16 = listInfoData[i]["Daily_Qty_16"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_16"].ToString();
                        sql.Append("           ,'" + Daily_Qty_16 + "'    \n");//[Daily_Qty_16]
                        sql.Append("           ,''    \n");//[Daily_Signal_17]
                        string Daily_Qty_17 = listInfoData[i]["Daily_Qty_17"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_17"].ToString();
                        sql.Append("           ,'" + Daily_Qty_17 + "'    \n");//[Daily_Qty_17]
                        sql.Append("           ,''    \n");//[Daily_Signal_18]
                        string Daily_Qty_18 = listInfoData[i]["Daily_Qty_18"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_18"].ToString();
                        sql.Append("           ,'" + Daily_Qty_18 + "'    \n");//[Daily_Qty_18]
                        sql.Append("           ,''    \n");//[Daily_Signal_19]
                        string Daily_Qty_19 = listInfoData[i]["Daily_Qty_19"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_19"].ToString();
                        sql.Append("           ,'" + Daily_Qty_19 + "'    \n");//[Daily_Qty_19]
                        sql.Append("           ,''    \n");//[Daily_Signal_20]
                        string Daily_Qty_20 = listInfoData[i]["Daily_Qty_20"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_20"].ToString();
                        sql.Append("           ,'" + Daily_Qty_20 + "'    \n");//[Daily_Qty_20]
                        sql.Append("           ,''    \n");//[Daily_Signal_21]
                        string Daily_Qty_21 = listInfoData[i]["Daily_Qty_21"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_21"].ToString();
                        sql.Append("           ,'" + Daily_Qty_21 + "'    \n");//[Daily_Qty_21]
                        sql.Append("           ,''    \n");//[Daily_Signal_22]
                        string Daily_Qty_22 = listInfoData[i]["Daily_Qty_22"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_22"].ToString();
                        sql.Append("           ,'" + Daily_Qty_22 + "'    \n");//[Daily_Qty_22]
                        sql.Append("           ,''    \n");//[Daily_Signal_23]
                        string Daily_Qty_23 = listInfoData[i]["Daily_Qty_23"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_23"].ToString();
                        sql.Append("           ,'" + Daily_Qty_23 + "'    \n");//[Daily_Qty_23]
                        sql.Append("           ,''    \n");//[Daily_Signal_24]
                        string Daily_Qty_24 = listInfoData[i]["Daily_Qty_24"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_24"].ToString();
                        sql.Append("           ,'" + Daily_Qty_24 + "'    \n");//[Daily_Qty_24]
                        sql.Append("           ,''    \n");//[Daily_Signal_25]
                        string Daily_Qty_25 = listInfoData[i]["Daily_Qty_25"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_25"].ToString();
                        sql.Append("           ,'" + Daily_Qty_25 + "'    \n");//[Daily_Qty_25]
                        sql.Append("           ,''    \n");//[Daily_Signal_26]
                        string Daily_Qty_26 = listInfoData[i]["Daily_Qty_26"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_26"].ToString();
                        sql.Append("           ,'" + Daily_Qty_26 + "'    \n");//[Daily_Qty_26]
                        sql.Append("           ,''    \n");//[Daily_Signal_27]
                        string Daily_Qty_27 = listInfoData[i]["Daily_Qty_27"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_27"].ToString();
                        sql.Append("           ,'" + Daily_Qty_27 + "'    \n");//[Daily_Qty_27]
                        sql.Append("           ,''    \n");//[Daily_Signal_28]
                        string Daily_Qty_28 = listInfoData[i]["Daily_Qty_28"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_28"].ToString();
                        sql.Append("           ,'" + Daily_Qty_28 + "'    \n");//[Daily_Qty_28]
                        sql.Append("           ,''    \n");//[Daily_Signal_29]
                        string Daily_Qty_29 = listInfoData[i]["Daily_Qty_29"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_29"].ToString();
                        sql.Append("           ,'" + Daily_Qty_29 + "'    \n");//[Daily_Qty_29]
                        sql.Append("           ,''    \n");//[Daily_Signal_30]
                        string Daily_Qty_30 = listInfoData[i]["Daily_Qty_30"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_30"].ToString();
                        sql.Append("           ,'" + Daily_Qty_30 + "'    \n");//[Daily_Qty_30]
                        sql.Append("           ,''    \n");//[Daily_Signal_31]
                        string Daily_Qty_31 = listInfoData[i]["Daily_Qty_31"].ToString() == "" ? "0" : listInfoData[i]["Daily_Qty_31"].ToString();
                        sql.Append("           ,'" + Daily_Qty_31 + "'    \n");//[Daily_Qty_31]
                        sql.Append("           ,''    \n");//[Cycle_Flag]
                        sql.Append("           ,''    \n");//[Start_Date_For_Total_Qty]
                        sql.Append("           ,''    \n");//[Effective_Range_For_Total_Qty]
                        sql.Append("           ,''    \n");//[Total_Signal_01]
                        sql.Append("           ,''    \n");//[Total_Qty_01]
                        sql.Append("           ,''    \n");//[Total_Signal_02]
                        sql.Append("           ,''    \n");//[Total_Qty_02]
                        sql.Append("           ,''    \n");//[Total_Signal_03]
                        sql.Append("           ,''    \n");//[Total_Qty_03]
                        sql.Append("           ,''    \n");//[Use_Part]
                        sql.Append("           ,''    \n");//[Management_Production]
                        sql.Append("           ,''    \n");//[Comment_Flag18]
                        sql.Append("           ,''    \n");//[Dummy_Data]
                        sql.Append("           ,'" + strUserId + "'    \n");//[vcOperatorID]
                        sql.Append("           ,getdate())    \n");//[dOperatorTime]
                        #endregion
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
                        sql.Append("vcOperatorID='" + strUserId + "',   \n");
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
